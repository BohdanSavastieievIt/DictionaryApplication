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
        public List<(UserDictionary UserDictionary, int TotalLexemes)> UserDictionaries { get; set; } = null!;

        public bool IsSampleEmpty { get; set; } = false;

        public async Task<IActionResult> OnGetAsync(bool isEmpty)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            if (isEmpty)
            {
                IsSampleEmpty = true;
                ViewData["ErrorInfo"] = "Selected dictionaries contain no words.";
            }

            UserDictionaries = await _userDictionaryRepository.GetAllWithLexemesAmountAsync(currentUser.Id);
            UserDictionaries = UserDictionaries.Where(ud => ud.TotalLexemes > 0).ToList();

            if (UserDictionaries.Count == 0)
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

            return RedirectToPage("SelectOtherTestParameters");
        }
    }

    public enum StartOfTheTestError
    {
        UserHasNoDictionaries,
        SelectedDictionariesHaveNoWords
    }
}
