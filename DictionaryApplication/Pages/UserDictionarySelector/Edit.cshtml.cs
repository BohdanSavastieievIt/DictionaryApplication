using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DictionaryApp.Data;
using DictionaryApp.Models;

namespace DictionaryApplication.Pages.UserDictionarySelector
{
    public class EditModel : PageModel
    {
        private readonly DictionaryApp.Data.ApplicationDbContext _context;

        public EditModel(DictionaryApp.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public UserDictionary UserDictionary { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.UserDictionaries == null)
            {
                return NotFound();
            }

            var userdictionary =  await _context.UserDictionaries.FirstOrDefaultAsync(m => m.Id == id);
            if (userdictionary == null)
            {
                return NotFound();
            }
            UserDictionary = userdictionary;
           ViewData["StudiedLangId"] = new SelectList(_context.Languages, "Id", "LangCode");
           ViewData["TranslationLangId"] = new SelectList(_context.Languages, "Id", "LangCode");
           ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(UserDictionary).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserDictionaryExists(UserDictionary.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool UserDictionaryExists(int id)
        {
          return (_context.UserDictionaries?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
