using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DictionaryApp.Data;
using DictionaryApp.Models;

namespace DictionaryApplication.Pages.UserDictionarySelector.UserDictionaryView
{
    public class CreateModel : PageModel
    {
        private readonly DictionaryApp.Data.ApplicationDbContext _context;

        public CreateModel(DictionaryApp.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Lexeme Lexeme1 { get; set; } = null!;
        [BindProperty]
        public Lexeme Lexeme2 { get; set; } = null!;
        public int UserDictionaryId { get; set; }
        public int Lexeme1LangId { get; set; }
        public int Lexeme2LangId { get; set; }
        public LexemePair LexemePair { get; set; } = null!;
        public DictionaryLexemePair[] DictionaryLexemePairs { get; set; } = new DictionaryLexemePair[2];


        public IActionResult OnGet()
        {
            if (HttpContext.Request.Query.ContainsKey("id"))
            {
                UserDictionaryId = int.Parse(HttpContext.Request.Query["id"].ToString());
                Lexeme1LangId = _context.UserDictionaries.First(x => x.Id == UserDictionaryId).StudiedLangId;
                Lexeme2LangId = _context.UserDictionaries.First(x => x.Id == UserDictionaryId).TranslationLangId;
            }

            return Page();
        }        


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Lexemes.Add(Lexeme1);
            _context.Lexemes.Add(Lexeme2);

            LexemePair = new LexemePair 
            { 
                Lexeme1 = Lexeme1, 
                Lexeme2 = Lexeme2, 
                LexemeRelationType = LexemeRelationType.Translations
            };
            _context.LexemePairs.Add(LexemePair);

            if (HttpContext.Request.Query.ContainsKey("id"))
            {
                UserDictionaryId = int.Parse(HttpContext.Request.Query["id"].ToString());
            }
            var userDictionary = await _context.UserDictionaries.FindAsync(UserDictionaryId);
            DictionaryLexemePairs[0] = new DictionaryLexemePair { Lexeme = Lexeme1, UserDictionary = userDictionary };
            DictionaryLexemePairs[1] = new DictionaryLexemePair { Lexeme = Lexeme2, UserDictionary = userDictionary };

            foreach (var pair in DictionaryLexemePairs)
            {
                _context.DictionaryLexemePairs.Add(pair);
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index", new { userDictionaryId = UserDictionaryId});
        }
    }
}
