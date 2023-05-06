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
        public Lexeme Lexeme1 { get; set; } = null!;
        [BindProperty]
        public Lexeme Lexeme2 { get; set; } = null!;

        public LexemePair LexemePair { get; set; } = null!;
        public int UserDictionaryId { get; set; }


        public async Task<IActionResult> OnGetAsync(int userDictionaryId, int lexeme1Id, int lexeme2Id)
        {
            if (_context.LexemePairs == null)
            {
                return NotFound();
            }

            var lexemepair = await _context.LexemePairs
                .Include(lp => lp.Lexeme1)
                .Include(lp => lp.Lexeme2)
                .FirstOrDefaultAsync(m => m.Lexeme1Id == lexeme1Id && m.Lexeme2Id == lexeme2Id);

            if (lexemepair == null || lexemepair.Lexeme1 == null || lexemepair.Lexeme2 == null)
            {
                return NotFound();
            }

            UserDictionaryId = userDictionaryId;
            LexemePair = lexemepair;
            Lexeme1 = lexemepair.Lexeme1;
            Lexeme2 = lexemepair.Lexeme2;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(int userDictionaryId)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Lexeme1).State = EntityState.Modified;
            _context.Attach(Lexeme2).State = EntityState.Modified;

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

            return RedirectToPage("./Index", new { userDictionaryId = userDictionaryId });
        }

        private bool LexemePairExists(int id)
        {
          return (_context.LexemePairs?.Any(e => e.Lexeme1Id == id)).GetValueOrDefault();
        }
    }
}
