using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DictionaryApplication.Data;
using DictionaryApplication.Models;
using DictionaryApplication.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace DictionaryApplication.Pages.UserDictionarySelector
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly IUserDictionaryRepository _repository;
        public DeleteModel(IUserDictionaryRepository repository)
        {
            _repository = repository;
        }

        [BindProperty]
        public UserDictionary UserDictionary { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var userDictionary = await _repository.GetByIdAsync(id);

            if (userDictionary == null)
            {
                return NotFound();
            }
            else 
            {
                UserDictionary = userDictionary;
                ViewData["TotalLexemes"] = await _repository.GetLexemesAmount(id);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            await _repository.DeleteAsync(id);

            return RedirectToPage("./Index");
        }
    }
}
