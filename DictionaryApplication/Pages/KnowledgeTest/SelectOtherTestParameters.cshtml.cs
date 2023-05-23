using DictionaryApplication.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DictionaryApplication.Services;
using DictionaryApplication.Models;
using DictionaryApplication.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace DictionaryApplication.Pages.KnowledgeTest
{
    [Authorize]
    public class SelectOtherTestParametersModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly KnowledgeTestService _knowledgeTestService;

        public SelectOtherTestParametersModel(
            UserManager<ApplicationUser> userManager,
            KnowledgeTestService knowledgeTestService)
        {
            _userManager = userManager;
            _knowledgeTestService = knowledgeTestService;
        }

        [BindProperty]
        public KnowledgeTestParameters TestParameters { get; set; } = null!;
        public List<int> SelectedDictionaryIds { get; set; } = null!;
        public int MaxLexemesAmount { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            await HttpContext.Session.LoadAsync();

            var idsOfSelectedDictionariesForTest = HttpContext.Session.GetList<int>("idsOfSelectedDictionariesForTest");
            if (idsOfSelectedDictionariesForTest == null || idsOfSelectedDictionariesForTest.Count == 0)
            {
                return RedirectToPage("KnowledgeTestStart");
            }
            SelectedDictionaryIds = idsOfSelectedDictionariesForTest;

            MaxLexemesAmount = await _knowledgeTestService.GetTotalLexemesAmount(idsOfSelectedDictionariesForTest.ToArray());

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await HttpContext.Session.LoadAsync();

            HttpContext.Session.SetKnowledgeTest("knowledgeTestParameters", TestParameters);

            var lexemeTestAttempts = await _knowledgeTestService.GetLexemeTestAttemptsAsync(TestParameters);
            HttpContext.Session.SetList("lexemeTestAttempts", lexemeTestAttempts);
            HttpContext.Session.SetInt32("currentLexemeId", 0);

            await HttpContext.Session.CommitAsync();

            return RedirectToPage("TestWord");
        }
    }
}
