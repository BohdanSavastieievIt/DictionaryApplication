using DictionaryApplication.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DictionaryApplication.Services;
using DictionaryApplication.Extensions;
using DictionaryApplication.Models;
using DictionaryApplication.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace DictionaryApplication.Pages.KnowledgeTest
{
    [Authorize]
    public class SelectDictionariesForTestModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly KnowledgeTestService _knowledgeTestService;
        private readonly IUserDictionaryRepository _userDictionaryRepository;
        public SelectDictionariesForTestModel(
            UserManager<ApplicationUser> userManager, 
            KnowledgeTestService knowledgeTestService,
            IUserDictionaryRepository userDictionaryRepository)
        {
            _userManager = userManager;
            _knowledgeTestService = knowledgeTestService;
            _userDictionaryRepository = userDictionaryRepository;
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
                return RedirectToPage("/UserDictionarySelector/Create", new { isNoDictionaries = true });
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (!(await _knowledgeTestService.ContainAnyLexemesAsync(IdsOfSelectedDictionariesForTest)))
            {
                return RedirectToPage("KnowledgeTestStart", new { isEmpty = true });
            }

            HttpContext.Session.SetList("idsOfSelectedDictionariesForTest", IdsOfSelectedDictionariesForTest);

            await HttpContext.Session.CommitAsync();

            return RedirectToPage("SelectOtherTestParameters");
        }
    }
}
