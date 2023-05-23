using DictionaryApplication.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DictionaryApplication.Services;
using DictionaryApplication.Models;
using DictionaryApplication.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace DictionaryApplication.Pages.KnowledgeTest
{
    [Authorize]
    public class TestResultModel : PageModel
    {
        public TestResultModel() {}

        public async Task<IActionResult> OnGetAsync()
        {
            await HttpContext.Session.LoadAsync();

            var lexemeTestAttempts = HttpContext.Session.GetList<LexemeTestAttempt>("lexemeTestAttempts");
            int totalAnswers = lexemeTestAttempts.Count;
            int totalCorrectAnswers = lexemeTestAttempts.Count(ta => ta.IsCorrectAnswer);
            double correctAnswersPercentage = Math.Round((double)totalCorrectAnswers / totalAnswers * 100, 2);

            ViewData["TotalAnswers"] = totalAnswers;
            ViewData["TotalCorrectAnswers"] = totalCorrectAnswers;
            ViewData["CorrectAnswersPercentage"] = correctAnswersPercentage;

            HttpContext.Session.Remove("knowledgeTestParameters");
            HttpContext.Session.Remove("lexemeTestAttempts");

            await HttpContext.Session.CommitAsync();

            return Page();
        }
    }
}
