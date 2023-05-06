using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DictionaryApp.Data;
using DictionaryApp.Models;
using Microsoft.AspNetCore.Identity;

namespace DictionaryApplication.Pages.UserDictionarySelector.UserDictionaryView
{
    public class DeleteModel : PageModel
    {
        private readonly DictionaryApp.Data.ApplicationDbContext _context;

        public DeleteModel(DictionaryApp.Data.ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
        }

        [BindProperty]
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

            if (lexemepair == null)
            {
                return NotFound();
            }
            else 
            {
                UserDictionaryId = userDictionaryId;
                LexemePair = lexemepair;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int userDictionaryId, int lexeme1Id, int lexeme2Id)
        {
            if (_context.LexemePairs == null)
            {
                return NotFound();
            }
            UserDictionaryId = userDictionaryId;
            var lexemepair = await _context.LexemePairs.FindAsync(lexeme1Id, lexeme2Id);

            if (lexemepair != null)
            {
                LexemePair = lexemepair;
                var lexeme1 = await _context.Lexemes.FirstOrDefaultAsync(m => m.Id == lexeme1Id);
                var lexeme2 = await _context.Lexemes.FirstOrDefaultAsync(m => m.Id == lexeme2Id);

                _context.Lexemes.Remove(lexeme1);
                _context.Lexemes.Remove(lexeme2);
                _context.LexemePairs.Remove(LexemePair);

                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index", new { id = UserDictionaryId});
        }
    }
}
