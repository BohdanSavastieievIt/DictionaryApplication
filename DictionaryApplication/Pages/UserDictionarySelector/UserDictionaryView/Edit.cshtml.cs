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
        public string Lexeme { get; set; } = null!;
        [BindProperty]
        public List<string?> Translations { get; set; }
        [BindProperty]
        public string? Description { get; set; }

        public int UserDictionaryId { get; set; }
        public string StudiedLang { get; set; }
        public string TranslationLang { get; set; }


        public async Task<IActionResult> OnGetAsync(int userDictionaryId, int lexemeId)
        {
            UserDictionaryId = userDictionaryId;
            var currentDictionary = _context.UserDictionaries
                    .Include(x => x.StudiedLanguage)
                    .Include(x => x.TranslationLanguage)
                    .First(x => x.Id == UserDictionaryId);
            StudiedLang = currentDictionary.StudiedLanguage.LangCode;
            TranslationLang = currentDictionary.TranslationLanguage.LangCode;

            Lexeme = _context.Lexemes.First(x => x.Id == lexemeId).Word;
            Description = _context.Lexemes.First(x => x.Id == lexemeId).Description;
            Translations = await _context.LexemeTranslationPairs.Where(x => x.LexemeId == lexemeId)
                .Select(x => x.Translation.Word).ToListAsync();

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(int userDictionaryId, int lexemeId)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var lexeme = await _context.Lexemes.FindAsync(lexemeId);
            if (lexeme == null)
            {
                return NotFound();
            }

            lexeme.Word = Lexeme;
            lexeme.Description = Description;

            // creating translations and pairs
            var langId = _context.UserDictionaries.First(x => x.Id == userDictionaryId).TranslationLangId;
            var translations = new List<Lexeme>();
            var lexemeTranslationPairs = new List<LexemeTranslationPair>();
            foreach(var transWord in Translations)
            {
                if (transWord == null) continue;
                var translation = new Lexeme { DictionaryId = userDictionaryId, Word = transWord, LangId = langId };
                translations.Add(translation);
                var lexemeTranslationPair = new LexemeTranslationPair { Lexeme = lexeme, Translation = translation };
                lexemeTranslationPairs.Add(lexemeTranslationPair);
            }

            // changing db
            var oldTranslations = _context.LexemeTranslationPairs.Where(x => x.LexemeId == lexemeId).Select(x => x.Translation);
            _context.Lexemes.RemoveRange(oldTranslations);
            var oldPairs = _context.LexemeTranslationPairs.Where(x => x.LexemeId == lexemeId);
            _context.LexemeTranslationPairs.RemoveRange(oldPairs);

            await _context.Lexemes.AddRangeAsync(translations);
            await _context.LexemeTranslationPairs.AddRangeAsync(lexemeTranslationPairs);

            //_context.Attach(Lexeme).State = EntityState.Modified;
            //_context.Attach(Translations).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LexemePairExists(lexemeId))
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
