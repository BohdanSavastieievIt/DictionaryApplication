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

        public IList<LexemeTranslationPair> LexemeTranslationPairs { get;set; } = null!;
        public int UserDictionaryId { get; set; }

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

                ViewData["CurrentDictionaryName"] = _context.UserDictionaries.First(x => x.Id == UserDictionaryId).Name;

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
