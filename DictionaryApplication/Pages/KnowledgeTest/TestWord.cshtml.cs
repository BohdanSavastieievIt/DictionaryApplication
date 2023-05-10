using DictionaryApp.Data;
using DictionaryApp.Models;
using DictionaryApplication.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;

namespace DictionaryApplication.Pages.KnowledgeTest
{
    public class TestWordModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<SelectOtherTestParametersModel> _logger;

        public TestWordModel(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<SelectOtherTestParametersModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public string EnteredTranslation { get; set; }
        public string DisplayedLexeme { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            await HttpContext.Session.LoadAsync();

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var currentLexemeId = HttpContext.Session.GetInt32("currentLexemeId");
            var lexemes = HttpContext.Session.GetList<(int LexemeId, string Lexeme)>("testLexemesLeft");

            if (lexemes == null)
            {
                return RedirectToPage("KnowledgeTestStart");
            }

            var displayedLexemes = lexemes.Where(x => x.LexemeId == currentLexemeId).Select(x => x.Lexeme).ToList();
            DisplayedLexeme = displayedLexemes.Count > 1 
                ? string.Join(Environment.NewLine, displayedLexemes.Select((s, i) => $"{i + 1}. {s}"))
                : displayedLexemes.First();

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await HttpContext.Session.LoadAsync();

            var testAnswers = HttpContext.Session.GetList<(int LexemeId, string Answer)>("testAnswers");
            var currentLexemeId = HttpContext.Session.GetInt32("currentLexemeId");
            if (currentLexemeId != null)
            {
                testAnswers.Add(((int)currentLexemeId, EnteredTranslation));
            }
            HttpContext.Session.SetList("testAnswers", testAnswers);

            var testLexemesLeft = HttpContext.Session.GetList<(int LexemeId, string Lexeme)>("testLexemesLeft");
            testLexemesLeft = testLexemesLeft.Where(x => x.LexemeId != currentLexemeId).ToList();
            if (!testLexemesLeft.Any())
            {
                HttpContext.Session.Remove("testLexemesLeft");
                HttpContext.Session.Remove("currentLexemeId");
                return RedirectToPage("WrongAnswersSelection");
            }

            HttpContext.Session.SetList("testLexemesLeft", testLexemesLeft);
            var nextLexemeId = testLexemesLeft.First().LexemeId;
            HttpContext.Session.SetInt32("currentLexemeId", nextLexemeId);

            await HttpContext.Session.CommitAsync();

            return RedirectToPage("TestWord");
        }
    }
}
