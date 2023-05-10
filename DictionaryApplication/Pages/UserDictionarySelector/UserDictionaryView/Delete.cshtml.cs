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
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
        }

        public string Lexeme { get; set; }
        public string? Description { get; set; }
        public string Translations { get; set; }
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
            var currentTranslations = await _context.LexemeTranslationPairs.Where(x => x.LexemeId == lexemeId)
                .Select(x => x.Translation.Word).ToListAsync();

            Translations = currentTranslations.Count > 1 ?
                string.Join(Environment.NewLine, currentTranslations.Select((s, i) => $"{i + 1}. {s}"))
                : currentTranslations.First();

            return Page();
        }

        public IActionResult OnPostAsync(int userDictionaryId, int lexemeId)
        {
            var translations = _context.LexemeTranslationPairs.Where(x => x.LexemeId == lexemeId).Select(x => x.Translation);
            _context.Lexemes.RemoveRange(translations);
            _context.Lexemes.Remove(_context.Lexemes.First(x => x.Id == lexemeId));
            var pairs = _context.LexemeTranslationPairs.Where(x => x.LexemeId == lexemeId);
            _context.LexemeTranslationPairs.RemoveRange(pairs);

            _context.SaveChanges();

            return RedirectToPage("./Index", new { userDictionaryId = userDictionaryId});
        }
    }
}
