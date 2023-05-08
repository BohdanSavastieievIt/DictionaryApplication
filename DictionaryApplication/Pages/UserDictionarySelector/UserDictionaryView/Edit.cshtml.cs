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
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Lexeme Lexeme { get; set; } = null!;
        [BindProperty]
        public Lexeme Translation { get; set; } = null!;

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

            if (lexemepair == null || lexemepair.Lexeme == null || lexemepair.Translation == null)
            {
                return NotFound();
            }

            UserDictionaryId = userDictionaryId;
            LexemeTranslationPair = lexemepair;
            Lexeme = lexemepair.Lexeme;
            Translation = lexemepair.Translation;
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

            _context.Attach(Lexeme).State = EntityState.Modified;
            _context.Attach(Translation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LexemePairExists(LexemeTranslationPair.LexemeId))
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
          return (_context.LexemeTranslationPairs?.Any(e => e.LexemeId == id)).GetValueOrDefault();
        }
    }
}
