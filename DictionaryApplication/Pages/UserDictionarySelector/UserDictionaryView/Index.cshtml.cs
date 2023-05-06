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

        public IList<LexemePair> LexemePairs { get;set; } = null!;
        public int UserDictionaryId { get; set; }

        public async Task OnGetAsync(int userDictionaryId = -1)
        {
            if (_context.LexemePairs != null)
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

                var lexemeIdsFromCurrentDict = _context.DictionaryLexemePairs
                    .Where(d => d.UserDictionaryId == UserDictionaryId).Select(x => x.LexemeId);
                
                
                LexemePairs = await _context.LexemePairs
                    .Where(x => lexemeIdsFromCurrentDict.Contains(x.Lexeme1Id) 
                        || lexemeIdsFromCurrentDict.Contains(x.Lexeme2Id) 
                        && x.LexemeRelationType == LexemeRelationType.Translations)
                    .Include(l => l.Lexeme1)
                    .Include(l => l.Lexeme2).ToListAsync();
            }
        }
    }
}
