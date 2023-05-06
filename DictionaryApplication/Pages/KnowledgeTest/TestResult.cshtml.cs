using DictionaryApp.Data;
using DictionaryApp.Models;
using DictionaryApplication.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DictionaryApplication.Pages.KnowledgeTest
{
    public class TestResultModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<SelectOtherTestParametersModel> _logger;
        private readonly KnowledgeTestManager _testManager;

        public TestResultModel(
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


        public async Task<IActionResult> OnGetAsync()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            int totalAnswers = HttpContext.Session.GetKnowledgeTest("knowledgeTestObject").NumberOfWords;
            int totalWrongAnswers = HttpContext.Session.GetInt32("wrongAnswersAmount") ?? 0;
            int totalCorrectAnswers = totalAnswers - totalWrongAnswers;
            double correctAnswersPercentage = Math.Round((double)totalCorrectAnswers / totalAnswers * 100, 2);

            ViewData["TotalAnswers"] = totalAnswers;
            ViewData["TotalCorrectAnswers"] = totalCorrectAnswers;
            ViewData["CorrectAnswersPercentage"] = correctAnswersPercentage;

            return Page();
        }
    }
}
