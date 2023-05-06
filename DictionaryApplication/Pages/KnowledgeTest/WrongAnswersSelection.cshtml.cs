using DictionaryApp.Data;
using DictionaryApp.Models;
using DictionaryApplication.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DictionaryApplication.Pages.KnowledgeTest
{
    public class WrongAnswersSelectionModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<SelectOtherTestParametersModel> _logger;
        private readonly KnowledgeTestManager _testManager;

        public WrongAnswersSelectionModel(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<SelectOtherTestParametersModel> logger,
            KnowledgeTestManager testManager)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _testManager = testManager;
        }

        [BindProperty]
        public List<bool> IsAnswerChangedToCorrect { get; set; } = null!;
        public List<(int LexemeId, string Lexeme)> FailedLexemes { get; set; } = null!;
        public List<(int LexemeId, string Lexeme)> FailedTranslations { get; set; } = null!;
        public List<(int LexemeId, string Answer)> WrongAnswers { get; set; } = null!;



        public async Task<IActionResult> OnGetAsync()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var testLexemes = HttpContext.Session.GetList<(int LexemeId, string Lexeme)>("testLexemes");
            var testTranslations = HttpContext.Session.GetList<(int LexemeId, string Lexeme)>("testTranslations");
            var testAnswers = HttpContext.Session.GetList<(int LexemeId, string Answer)>("testAnswers");

            WrongAnswers = _testManager.GetWrongAnswers(testLexemes, testTranslations, testAnswers);
            if (WrongAnswers.Count == 0)
            {
                return RedirectToPage("TestResult");
            }
            HttpContext.Session.SetList("wrongAnswers", WrongAnswers);

            FailedLexemes = testLexemes.Where(x => 
                WrongAnswers.Select(y => y.LexemeId)
                    .Contains(x.LexemeId))
                .ToList();
            FailedTranslations = testTranslations.Where(x => 
                WrongAnswers.Select(y => y.LexemeId)
                    .Contains(x.LexemeId))
                .ToList();



            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var wrongAnswers = HttpContext.Session.GetList<(int LexemeId, string Answer)>("wrongAnswers");
            
            for (int i = 0; i < wrongAnswers.Count; i++)
            {
                var lexeme = _context.Lexemes.First(x => x.Id == wrongAnswers[i].LexemeId);
                lexeme.TotalTestAttempts++;
                if (IsAnswerChangedToCorrect[i])
                {
                    lexeme.CorrectTestAttempts++;
                }
                _context.SaveChanges();
            }

            HttpContext.Session.Remove("testLexemes");
            HttpContext.Session.Remove("testTranslations");
            HttpContext.Session.Remove("testAnswers");
            HttpContext.Session.Remove("wrongLexemes");

            var wrongAnswersAmount = IsAnswerChangedToCorrect.Count(x => !x);
            HttpContext.Session.SetInt32("wrongAnswersAmount", wrongAnswersAmount);

            return RedirectToPage("TestResult");
        }

    }
}
