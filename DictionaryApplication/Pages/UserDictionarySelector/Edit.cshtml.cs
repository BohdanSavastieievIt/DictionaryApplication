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
using Microsoft.AspNetCore.Authorization;

namespace DictionaryApplication.Pages.UserDictionarySelector
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly IUserDictionaryRepository _dictRepository;
        private readonly IDbRepository<Language> _langRepository;
        private readonly ILogger<CreateModel> _logger;

        public EditModel(IUserDictionaryRepository dictRepository,IDbRepository<Language> langRepository, ILogger<CreateModel> logger)
        {
            _dictRepository = dictRepository;
            _langRepository = langRepository;
            _logger = logger;
        }

        [BindProperty]
        public UserDictionary UserDictionary { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var userdictionary = await _dictRepository.GetByIdAsync(id);
            if (userdictionary == null)
            {
                return NotFound();
            }
            UserDictionary = userdictionary;

            var languages = await _langRepository.GetAllAsync();

            ViewData["StudiedLangId"] = new SelectList(languages, "Id", "LangCode");
            ViewData["TranslationLangId"] = new SelectList(languages, "Id", "LangCode");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || UserDictionary == null)
            {
                return Page();
            }

            await _dictRepository.UpdateAsync(UserDictionary);

            return RedirectToPage("./Index");
        }
    }
}
