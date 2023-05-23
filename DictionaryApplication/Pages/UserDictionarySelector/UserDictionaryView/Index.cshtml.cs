using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DictionaryApplication.Data;
using DictionaryApplication.Models;
using DictionaryApplication.Paging;
using DictionaryApplication.Repositories;

namespace DictionaryApplication.Pages.UserDictionarySelector.UserDictionaryView
{
    public class IndexModel : UserDictionaryViewPageModel
    {
        private readonly ILexemeDetailsRepository _lexemeDetailsRepository;

        public IndexModel(ILexemeDetailsRepository lexemeDetailsRepository,
            IUserDictionaryRepository userDictionaryRepository) : base(userDictionaryRepository)
        {
            _lexemeDetailsRepository = lexemeDetailsRepository;
        }

        public LexemeDetailsList LexemeDetailsList { get; set; } = null!;
        public string? SortOrder { get; set; } = null!;
        public int FirstItemId { get; set; }

        public async Task<IActionResult> OnGetAsync(int userDictionaryId, int? pageId, string? sortOrder, string? searchString)
        {
            await LoadUserDictionaryAsync(userDictionaryId);

            // инициализация необходимых лексем
            LexemeDetailsList = new LexemeDetailsList();
            LexemeDetailsList.LexemeDetails = await _lexemeDetailsRepository.GetAllAsync(userDictionaryId);

            // обработка сортировки
            SortOrder = sortOrder;
            switch (SortOrder)
            {
                case "date_asc":
                    LexemeDetailsList.LexemeDetails = LexemeDetailsList.LexemeDetails.OrderBy(x => x.Lexeme.Id).ToList();
                    break;
                case "date_desc":
                    LexemeDetailsList.LexemeDetails = LexemeDetailsList.LexemeDetails.OrderByDescending(x => x.Lexeme.Id).ToList();
                    break;
                case "alphabetical_asc":
                    LexemeDetailsList.LexemeDetails = LexemeDetailsList.LexemeDetails.OrderBy(x => x.Lexeme.Word).ToList();
                    break;
                case "alphabetical_desc":
                    LexemeDetailsList.LexemeDetails = LexemeDetailsList.LexemeDetails.OrderByDescending(x => x.Lexeme.Word).ToList();
                    break;
                case "test_results_asc":
                    LexemeDetailsList.LexemeDetails = LexemeDetailsList.LexemeDetails.OrderBy(x => x.TestResults).ThenBy(x => x.Lexeme.TotalTestAttempts).ToList();
                    break;
                case "test_results_desc":
                    LexemeDetailsList.LexemeDetails = LexemeDetailsList.LexemeDetails.OrderByDescending(x => x.TestResults).ThenByDescending(x => x.Lexeme.TotalTestAttempts).ToList();
                    break;
                default:
                    break;
            }

            // обработка поиска
            ViewData["CurrentFilter"] = searchString;
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                LexemeDetailsList.LexemeDetails = LexemeDetailsList.LexemeDetails
                    .Where(s => s.Lexeme.Word.ToLower().Contains(searchString)
                        || s.TranslationsRepresentation.ToLower().Contains(searchString))
                    .ToList();
            }

            // обработка пагинации
            if (pageId == null)
            {
                pageId = 0;
            }
            int pageSize = 20;
            PagingInfo pagingInfo = new PagingInfo();
            pagingInfo.CurrentPage = pageId == 0 ? 1 : (int)pageId;
            pagingInfo.ItemsPerPage = pageSize;
            FirstItemId = pageSize * (pagingInfo.CurrentPage - 1) + 1;

            int skipTemp = (int)pageId == 0 ? 1 : (int)pageId;
            var skip = pageSize * (Convert.ToInt32(skipTemp) - 1);
            var resultTuple = await _lexemeDetailsRepository.GetAllFilterAsync(LexemeDetailsList.LexemeDetails.ToList(), skip, pageSize);

            pagingInfo.TotalItems = resultTuple.Item2;
            LexemeDetailsList.LexemeDetails = resultTuple.Item1;
            LexemeDetailsList.PagingInfo = pagingInfo;

            return Page();
        }
    }
}
