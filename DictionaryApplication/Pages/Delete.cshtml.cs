using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DictionaryApp.Data;
using DictionaryApp.Models;

namespace DictionaryApplication.Pages
{
    public class DeleteModel : PageModel
    {
        private readonly DictionaryApp.Data.ApplicationDbContext _context;

        public DeleteModel(DictionaryApp.Data.ApplicationDbContext context)
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

            var userdictionary = await _context.UserDictionaries.FirstOrDefaultAsync(m => m.Id == id);

            if (userdictionary == null)
            {
                return NotFound();
            }
            else 
            {
                UserDictionary = userdictionary;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.UserDictionaries == null)
            {
                return NotFound();
            }
            var userdictionary = await _context.UserDictionaries.FindAsync(id);

            if (userdictionary != null)
            {
                UserDictionary = userdictionary;
                _context.UserDictionaries.Remove(UserDictionary);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
