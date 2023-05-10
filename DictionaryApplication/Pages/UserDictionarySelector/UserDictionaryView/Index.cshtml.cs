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
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<LexemeTranslationPair> LexemeTranslationPairs { get;set; } = null!;
        public List<Lexeme> StudiedLexemes { get; set; } = null!;
        public List<string> Translations { get; set; } = new List<string>();
        public List<string> TestResultsPercent { get; set; } = new List<string>();

        public int UserDictionaryId { get; set; }
        public string StudiedLang { get; set; }
        public string TranslationLang { get; set; }

        public async Task OnGetAsync(int userDictionaryId = -1)
        {
            if (_context.LexemeTranslationPairs != null)
            {
                if (userDictionaryId != -1)
                {
                    UserDictionaryId = userDictionaryId;
                }
                else if (HttpContext.Request.Query.ContainsKey("id"))
                {
                    UserDictionaryId = int.Parse(HttpContext.Request.Query["id"].ToString());
                }
                else
                {
                    RedirectToPage("/UserDictionarySelector/Index");
                }

                var currentDictionary = _context.UserDictionaries
                    .Include(x => x.StudiedLanguage)
                    .Include(x => x.TranslationLanguage)
                    .First(x => x.Id == UserDictionaryId);
                StudiedLang = currentDictionary.StudiedLanguage.LangCode;
                TranslationLang = currentDictionary.TranslationLanguage.LangCode;
                ViewData["CurrentDictionaryName"] = currentDictionary.Name;

                StudiedLexemes = await _context.Lexemes.Where(x => x.DictionaryId == userDictionaryId 
                    && x.LexemePairs != null && x.LexemePairs.Count > 0).ToListAsync();
                foreach (var lexeme in StudiedLexemes)
                {
                    var currentTranslations = await _context.Lexemes.Where(x => 
                        _context.LexemeTranslationPairs
                            .Where(y => y.LexemeId == lexeme.Id)
                            .Select(y => y.TranslationId).Contains(x.Id))
                        .Select(x => x.Word)
                        .ToListAsync();

                    string translations = currentTranslations.Count > 1 ? 
                        string.Join(Environment.NewLine, currentTranslations.Select((s, i) => $"{i + 1}. {s}"))
                        : currentTranslations.First();
                    Translations.Add(translations);

                    if (lexeme.TotalTestAttempts > 0)
                    {
                        double percent = lexeme.CorrectTestAttempts / lexeme.TotalTestAttempts * 100;
                        TestResultsPercent.Add(percent.ToString("F2"));
                    }  
                    else
                    {
                        TestResultsPercent.Add(string.Empty);
                    }
                }

                var lexemeIdsFromCurrentDict = await _context.Lexemes
                    .Where(x => x.DictionaryId == userDictionaryId)
                    .Select(x => x.Id).ToListAsync();
                
                
                LexemeTranslationPairs = await _context.LexemeTranslationPairs
                    .Where(x => lexemeIdsFromCurrentDict.Contains(x.LexemeId) 
                        || lexemeIdsFromCurrentDict.Contains(x.TranslationId))
                    .Include(l => l.Lexeme)
                    .Include(l => l.Translation)
                    .ToListAsync();
            }
        }
    }
}
