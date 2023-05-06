using DictionaryApp.Data;
using DictionaryApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DictionaryApplication.Pages.KnowledgeTest
{
    public class KnowledgeTestErrorModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public KnowledgeTestErrorModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync(StartOfTheTestError errorType)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            return Page();
        }
    }
}
