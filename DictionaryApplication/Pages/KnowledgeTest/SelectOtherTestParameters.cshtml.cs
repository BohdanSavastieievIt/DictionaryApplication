using DictionaryApp.Data;
using DictionaryApp.Models;
using DictionaryApplication.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DictionaryApplication.Pages.KnowledgeTest
{
    public class SelectOtherTestParametersModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<SelectOtherTestParametersModel> _logger;
        private readonly KnowledgeTestManager _knowledgeTestManager;

        public SelectOtherTestParametersModel(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<SelectOtherTestParametersModel> logger,
            KnowledgeTestManager knowledgeTestManager)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _knowledgeTestManager = knowledgeTestManager;
        }

        [BindProperty]
        public KnowledgeTestModel KnowledgeTest { get; set; } = null!;
        public List<UserDictionary> SelectedUserDictionaries { get; set; } = null!;
        public List<Lexeme> LexemesIncluded { get; set; } = null!;
        public int MaxLexemesAmount { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var idsOfSelectedDictionariesForTest = HttpContext.Session.GetList<int>("idsOfSelectedDictionariesForTest");
            if (idsOfSelectedDictionariesForTest == null || idsOfSelectedDictionariesForTest.Count == 0)
            {
                return RedirectToPage("KnowledgeTestStart");
            }

            MaxLexemesAmount = _knowledgeTestManager.GetTotalLexemesAmount(idsOfSelectedDictionariesForTest);

            return Page();
        }

        public IActionResult OnPost()
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

            KnowledgeTest.SelectedDictionaryIds = HttpContext.Session.GetList<int>("idsOfSelectedDictionariesForTest");
            HttpContext.Session.SetKnowledgeTest("knowledgeTestObject", KnowledgeTest);

            var testLexemesAndTranslations = _knowledgeTestManager.GetTestLexemesAndTranslations(KnowledgeTest);
            var testLexemes = testLexemesAndTranslations.Item1;
            var testTranslations = testLexemesAndTranslations.Item2;
            var testAnswers = new List<(int LexemeId, string Answer)>();

            HttpContext.Session.SetList("testLexemes", testLexemes);
            HttpContext.Session.SetList("testLexemesLeft", testLexemes);
            HttpContext.Session.SetList("testTranslations", testTranslations);
            HttpContext.Session.SetList("testAnswers", testAnswers);

            var currentLexemeId = testLexemes.First().LexemeId;
            HttpContext.Session.SetInt32("currentLexemeId", currentLexemeId);

            return RedirectToPage("TestWord");
        }
    }
}
