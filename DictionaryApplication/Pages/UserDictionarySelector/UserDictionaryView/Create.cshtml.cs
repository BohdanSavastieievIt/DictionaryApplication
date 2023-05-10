using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DictionaryApp.Data;
using DictionaryApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DictionaryApplication.Pages.UserDictionarySelector.UserDictionaryView
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(ApplicationDbContext context, ILogger<CreateModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public string Lexeme { get; set; } = null!;
        [BindProperty]
        public string RequiredTranslation { get; set; } = null!;
        [BindProperty]
        public List<string?> AdditionalTranslations { get; set; }
        [BindProperty]
        public string? Description { get; set; }

        public int UserDictionaryId { get; set; }
        public string StudiedLang { get; set; }
        public string TranslationLang { get; set; }

        //public LexemeTranslationPair LexemeTranslationPair { get; set; } = null!;


        public IActionResult OnGet(int userDictionaryId)
        {
            UserDictionaryId = userDictionaryId;
            var currentDictionary = _context.UserDictionaries
                    .Include(x => x.StudiedLanguage)
                    .Include(x => x.TranslationLanguage)
                    .First(x => x.Id == UserDictionaryId);
            StudiedLang = currentDictionary.StudiedLanguage.LangCode;
            TranslationLang = currentDictionary.TranslationLanguage.LangCode;
            return Page();
        }        


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync(int userDictionaryId)
        {
            if (!ModelState.IsValid)
            {
                foreach (var key in ModelState.Keys)
                {
                    foreach (var error in ModelState[key].Errors)
                    {
                        _logger.LogError($"Model error: {key}, {error.ErrorMessage}");
                    }
                }
                return Page();
            }

            if (!HttpContext.Request.Query.ContainsKey("id"))
            {
                return RedirectToPage("/Index");
            }

            userDictionaryId = int.Parse(HttpContext.Request.Query["id"].ToString());
            var lexemeLangId = _context.UserDictionaries.First(x => x.Id == userDictionaryId).StudiedLangId;
            var translationLangId = _context.UserDictionaries.First(x => x.Id == userDictionaryId).TranslationLangId;

            var lexeme = new Lexeme { DictionaryId = userDictionaryId, LangId = lexemeLangId, Word = Lexeme, Description = Description };
            var requiredTranslation = new Lexeme { DictionaryId = userDictionaryId, LangId = translationLangId, Word = RequiredTranslation };
            var mainLexemeTranslationPair = new LexemeTranslationPair { Lexeme = lexeme, Translation = requiredTranslation };
            _context.Lexemes.Add(lexeme);
            _context.Lexemes.Add(requiredTranslation);
            _context.LexemeTranslationPairs.Add(mainLexemeTranslationPair);

            if (AdditionalTranslations != null) 
            {
                foreach (var translationWord in AdditionalTranslations)
                {
                    if (translationWord != null)
                    {
                        var translation = new Lexeme { DictionaryId = userDictionaryId, LangId = translationLangId, Word = translationWord };
                        var lexemeTranslationPair = new LexemeTranslationPair { Lexeme = lexeme, Translation = translation };
                        _context.Lexemes.Add(translation);
                        _context.LexemeTranslationPairs.Add(lexemeTranslationPair);
                    }     
                }
            }


            //_context.Lexemes.Add(Lexeme);
            //foreach(var translation in Translations)
            //{
            //    if (translation == null) continue;

            //    _context.Lexemes.Add(translation);
            //    var lexemeTranslationPair = new LexemeTranslationPair
            //    {
            //        Lexeme = Lexeme,
            //        Translation = translation
            //    };
            //    _context.LexemeTranslationPairs.Add(lexemeTranslationPair);
            //}

            //_context.LexemeTranslationPairs.Add(LexemeTranslationPair);

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index", new { userDictionaryId = userDictionaryId});
        }
    }
}
