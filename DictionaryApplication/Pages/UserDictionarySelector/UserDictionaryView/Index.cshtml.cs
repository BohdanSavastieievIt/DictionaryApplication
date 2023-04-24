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
        private readonly DictionaryApp.Data.ApplicationDbContext _context;

        public IndexModel(DictionaryApp.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<LexemePair> LexemePair { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.LexemePairs != null)
            {
                LexemePair = await _context.LexemePairs
                .Include(l => l.Lexeme1)
                .Include(l => l.Lexeme2).ToListAsync();
            }
        }
    }
}
