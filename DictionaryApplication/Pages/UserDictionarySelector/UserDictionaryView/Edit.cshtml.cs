using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DictionaryApplication.Data;
using DictionaryApplication.Models;
using DictionaryApplication.Repositories;

namespace DictionaryApplication.Pages.UserDictionarySelector.UserDictionaryView
{
    public class EditModel : UserDictionaryViewPageModel
    {
        private readonly ILexemeInputRepository _lexemeInputRepository;

        public EditModel(ILexemeInputRepository lexemeInputRepository,
            IUserDictionaryRepository userDictionaryRepository) : base(userDictionaryRepository)
        {
            _lexemeInputRepository = lexemeInputRepository;
        }

        [BindProperty]
        public LexemeInput LexemeInput { get; set; } = null!;


        public async Task<IActionResult> OnGetAsync(int userDictionaryId, int lexemeId)
        {
            await LoadUserDictionaryAsync(userDictionaryId);
            var lexemeInput = await _lexemeInputRepository.GetByIdAsync(lexemeId);
            if (lexemeInput == null)
            {
                return RedirectToPage("./Index", new { userDictionaryId = userDictionaryId });
            }
            else
            {
                LexemeInput = lexemeInput;
            }

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(int userDictionaryId, int lexemeId)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _lexemeInputRepository.UpdateAsync(lexemeId, LexemeInput);

            return RedirectToPage("./Index", new { userDictionaryId = userDictionaryId });
        }
    }
}
