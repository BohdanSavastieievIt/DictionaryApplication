using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DictionaryApp.Data;
using DictionaryApp.Models;

namespace DictionaryApplication.Pages.UserDictionarySelector
{
    public class IndexModel : PageModel
    {
        private readonly DictionaryApp.Data.ApplicationDbContext _context;

        public IndexModel(DictionaryApp.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<UserDictionary> UserDictionary { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.UserDictionaries != null)
            {
                UserDictionary = await _context.UserDictionaries
                .Include(u => u.StudiedLanguage)
                .Include(u => u.TranslationLanguage)
                .Include(u => u.User).ToListAsync();
            }
        }
    }
}
