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
        public string? TestAnswer { get; set; }
        public string DisplayedLexeme { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync()
        {
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

            DisplayedLexemes = lexemes.Where(x => x.LexemeId == currentLexemeId).Select(x => x.Lexeme).ToList();

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

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

            return RedirectToPage("TestWord");
        }
    }
}
