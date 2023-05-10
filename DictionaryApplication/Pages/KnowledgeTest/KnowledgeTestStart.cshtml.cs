using DictionaryApp.Data;
using DictionaryApp.Models;
using DictionaryApplication.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DictionaryApplication.Pages.KnowledgeTest
{
    public class SelectDictionariesForTestModel : PageModel
    {
        private readonly ApplicationDbContext _context; 
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<SelectDictionariesForTestModel> _logger;
        private readonly KnowledgeTestManager _knowledgeTestManager;
        public SelectDictionariesForTestModel(
            ApplicationDbContext context, 
            UserManager<ApplicationUser> userManager, 
            ILogger<SelectDictionariesForTestModel> logger,
            KnowledgeTestManager knowledgeTestManager)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _knowledgeTestManager = knowledgeTestManager;
        }

        [BindProperty]
        public List<int> IdsOfSelectedDictionariesForTest { get; set; } = null!;
        public List<UserDictionary> UserDictionaries { get; set; } = null!;
        public async Task<IActionResult> OnGetAsync()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            UserDictionaries = await _context.UserDictionaries
                .Where(x => x.UserId == currentUser.Id)
                .ToListAsync();

            if(UserDictionaries.Count == 0)
            {
                return RedirectToPage("/KnowledgeTestError", new { errorType = StartOfTheTestError.UserHasNoDictionaries });
            }

            return Page();
        }

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

            if (!_knowledgeTestManager.ContainAnyLexemes(IdsOfSelectedDictionariesForTest))
            {
                return RedirectToPage("/KnowledgeTestError", new { errorType = StartOfTheTestError.SelectedDictionariesHaveNoWords });
            }

            HttpContext.Session.SetList("idsOfSelectedDictionariesForTest", IdsOfSelectedDictionariesForTest);

            await HttpContext.Session.CommitAsync();

            return RedirectToPage("SelectOtherTestParameters");
        }
    }

    public enum StartOfTheTestError
    {
        UserHasNoDictionaries,
        SelectedDictionariesHaveNoWords
    }
}
