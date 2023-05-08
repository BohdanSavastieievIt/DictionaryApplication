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
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(ApplicationDbContext context, ILogger<CreateModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public Lexeme Lexeme { get; set; } = null!;
        [BindProperty]
        public Lexeme Translation { get; set; } = null!;

        public int UserDictionaryId { get; set; }
        public int LexemeLangId { get; set; }
        public int TranslationLangId { get; set; }
        public LexemeTranslationPair LexemeTranslationPair { get; set; } = null!;


        public IActionResult OnGet()
        {
            if (HttpContext.Request.Query.ContainsKey("id"))
            {
                UserDictionaryId = int.Parse(HttpContext.Request.Query["id"].ToString());
                LexemeLangId = _context.UserDictionaries.First(x => x.Id == UserDictionaryId).StudiedLangId;
                TranslationLangId = _context.UserDictionaries.First(x => x.Id == UserDictionaryId).TranslationLangId;
            }

            return Page();
        }        


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
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

            _context.Lexemes.Add(Lexeme);
            _context.Lexemes.Add(Translation);

            LexemeTranslationPair = new LexemeTranslationPair 
            { 
                Lexeme = Lexeme, 
                Translation = Translation, 
            };
            _context.LexemeTranslationPairs.Add(LexemeTranslationPair);

            if (HttpContext.Request.Query.ContainsKey("id"))
            {
                UserDictionaryId = int.Parse(HttpContext.Request.Query["id"].ToString());
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index", new { userDictionaryId = UserDictionaryId});
        }
    }
}
