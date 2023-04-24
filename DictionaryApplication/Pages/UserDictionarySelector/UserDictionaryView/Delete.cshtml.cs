using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DictionaryApp.Data;
using DictionaryApp.Models;

namespace DictionaryApplication.Pages.UserDictionarySelector.UserDictionaryView
{
    public class DeleteModel : PageModel
    {
        private readonly DictionaryApp.Data.ApplicationDbContext _context;

        public DeleteModel(DictionaryApp.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
      public LexemePair LexemePair { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.LexemePairs == null)
            {
                return NotFound();
            }

            var lexemepair = await _context.LexemePairs.FirstOrDefaultAsync(m => m.Lexeme1Id == id);

            if (lexemepair == null)
            {
                return NotFound();
            }
            else 
            {
                LexemePair = lexemepair;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.LexemePairs == null)
            {
                return NotFound();
            }
            var lexemepair = await _context.LexemePairs.FindAsync(id);

            if (lexemepair != null)
            {
                LexemePair = lexemepair;
                _context.LexemePairs.Remove(LexemePair);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
