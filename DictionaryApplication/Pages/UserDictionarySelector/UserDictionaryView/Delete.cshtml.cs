using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DictionaryApplication.Data;
using Microsoft.AspNetCore.Identity;
using DictionaryApplication.Models;
using DictionaryApplication.Repositories;
using Azure;
using Microsoft.AspNetCore.Authorization;

namespace DictionaryApplication.Pages.UserDictionarySelector.UserDictionaryView
{
    public class DeleteModel : UserDictionaryViewPageModel
    {
        private readonly ILexemeInputRepository _lexemeInputRepository;
        public DeleteModel(ILexemeInputRepository lexemeInputRepository,
            IUserDictionaryRepository userDictionaryRepository) : base(userDictionaryRepository)
        {
            _lexemeInputRepository = lexemeInputRepository;
        }

        public LexemeInput LexemeInput { get; set; } = null!;
        public int CurrentPage { get; set; }

        public async Task<IActionResult> OnGetAsync(int userDictionaryId, int lexemeId, int pageId)
        {
            await LoadUserDictionaryAsync(userDictionaryId);
            var lexemeInput = await _lexemeInputRepository.GetByIdAsync(lexemeId);
            if (lexemeInput == null)
            {
                return RedirectToPage("./Index", new { userDictionaryId = userDictionaryId, pageId = pageId });
            }
            else
            {
                LexemeInput = lexemeInput;
            }

            CurrentPage = pageId;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int userDictionaryId, int lexemeId, int pageId)
        {
            await _lexemeInputRepository.DeleteAsync(lexemeId);

            return RedirectToPage("./Index", new { userDictionaryId = userDictionaryId, pageId = pageId});
        }
    }
}
