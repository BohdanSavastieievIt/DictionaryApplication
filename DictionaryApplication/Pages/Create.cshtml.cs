using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DictionaryApp.Data;
using DictionaryApp.Models;

namespace DictionaryApplication.Pages
{
    public class CreateModel : PageModel
    {
        private readonly DictionaryApp.Data.ApplicationDbContext _context;

        public CreateModel(DictionaryApp.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["StudiedLangId"] = new SelectList(_context.Languages, "Id", "LangCode");
        ViewData["TranslationLangId"] = new SelectList(_context.Languages, "Id", "LangCode");
        ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return Page();
        }

        [BindProperty]
        public UserDictionary UserDictionary { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _context.UserDictionaries == null || UserDictionary == null)
            {
                return Page();
            }

            _context.UserDictionaries.Add(UserDictionary);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
