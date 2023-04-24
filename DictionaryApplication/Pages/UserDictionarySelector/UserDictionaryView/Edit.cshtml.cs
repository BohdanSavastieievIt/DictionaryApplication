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

namespace DictionaryApplication.Pages.UserDictionarySelector.UserDictionaryView
{
    public class EditModel : PageModel
    {
        private readonly DictionaryApp.Data.ApplicationDbContext _context;

        public EditModel(DictionaryApp.Data.ApplicationDbContext context)
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

            var lexemepair =  await _context.LexemePairs.FirstOrDefaultAsync(m => m.Lexeme1Id == id);
            if (lexemepair == null)
            {
                return NotFound();
            }
            LexemePair = lexemepair;
           ViewData["Lexeme1Id"] = new SelectList(_context.Lexemes, "Id", "Word");
           ViewData["Lexeme2Id"] = new SelectList(_context.Lexemes, "Id", "Word");
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

            _context.Attach(LexemePair).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LexemePairExists(LexemePair.Lexeme1Id))
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

        private bool LexemePairExists(int id)
        {
          return (_context.LexemePairs?.Any(e => e.Lexeme1Id == id)).GetValueOrDefault();
        }
    }
}
