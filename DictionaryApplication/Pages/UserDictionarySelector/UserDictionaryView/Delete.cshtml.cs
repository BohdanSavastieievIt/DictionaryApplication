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
        public LexemeTranslationPair LexemeTranslationPair { get; set; } = null!;
        public int UserDictionaryId { get; set; }

        public async Task<IActionResult> OnGetAsync(int userDictionaryId, int lexemeId, int translationId)
        {
            if (_context.LexemeTranslationPairs == null)
            {
                return NotFound();
            }

            var lexemepair = await _context.LexemeTranslationPairs
                .Include(lp => lp.Lexeme)
                .Include(lp => lp.Translation)
                .FirstOrDefaultAsync(m => m.LexemeId == lexemeId && m.TranslationId == translationId);

            if (lexemepair == null)
            {
                return NotFound();
            }
            else 
            {
                UserDictionaryId = userDictionaryId;
                LexemeTranslationPair = lexemepair;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int userDictionaryId, int lexemeId, int translationId)
        {
            if (_context.LexemeTranslationPairs == null)
            {
                return NotFound();
            }
            UserDictionaryId = userDictionaryId;
            var lexemeTranslationPair = await _context.LexemeTranslationPairs.FindAsync(lexemeId, translationId);

            if (lexemeTranslationPair != null)
            {
                LexemeTranslationPair = lexemeTranslationPair;
                var lexeme = _context.Lexemes.First(m => m.Id == lexemeId);
                var translation = _context.Lexemes.First(m => m.Id == translationId);

                _context.Lexemes.Remove(lexeme);
                _context.Lexemes.Remove(translation);
                _context.LexemeTranslationPairs.Remove(LexemeTranslationPair);

                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index", new { id = UserDictionaryId});
        }
    }
}
