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

namespace DictionaryApplication.Pages.UserDictionarySelector
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<UserDictionary> UserDictionary { get;set; } = default!;

        public async Task OnGetAsync()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (_context.UserDictionaries != null && currentUser != null)
            {
                UserDictionary = await _context.UserDictionaries
                    .Where(u => u.UserId == currentUser.Id)
                    .Include(u => u.StudiedLanguage)
                    .Include(u => u.TranslationLanguage)
                    .Include(u => u.User).ToListAsync();
            }
            else if (currentUser == null)
            {
                RedirectToPage("/Account/Login", new { area = "Identity" });
            }
        }
    }
}
