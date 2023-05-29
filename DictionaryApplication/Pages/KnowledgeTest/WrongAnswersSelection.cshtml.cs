using DictionaryApplication.Extensions;
using DictionaryApplication.Models;
using DictionaryApplication.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DictionaryApplication.Pages.KnowledgeTest
{
    [Authorize]
    public class WrongAnswersSelectionModel : PageModel
    {
        private readonly KnowledgeTestService _knowledgeTestService;

        public WrongAnswersSelectionModel(
            KnowledgeTestService knowledgeTestService)
        {
            _knowledgeTestService = knowledgeTestService;
        }

        [BindProperty]
        public List<int> CorrectedLexemeIds { get; set; } = null!;
        public List<LexemeTestAttempt> LexemeTestAttempts { get; set; } = null!;

        public IActionResult OnGet()
        {
            var lexemeTestAttempts = HttpContext.Session.GetList<LexemeTestAttempt>("lexemeTestAttempts");
            var knowledgeTestParameters = HttpContext.Session.GetKnowledgeTest("knowledgeTestParameters");

            if (lexemeTestAttempts == null || knowledgeTestParameters == null)
            {
                return RedirectToPage("KnowledgeTestStart");
            }

            _knowledgeTestService.CheckAnswers(ref lexemeTestAttempts, knowledgeTestParameters);
            HttpContext.Session.SetList("lexemeTestAttempts", lexemeTestAttempts);

            lexemeTestAttempts = lexemeTestAttempts.Where(ta => !string.IsNullOrEmpty(ta.TestAnswer)).ToList();

            if (!lexemeTestAttempts.Any(ta => !ta.IsCorrectAnswer))
            {
                return RedirectToPage("TestResult");
            }

            LexemeTestAttempts = lexemeTestAttempts;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var lexemeTestAttempts = HttpContext.Session.GetList<LexemeTestAttempt>("lexemeTestAttempts");

            foreach(var id in CorrectedLexemeIds)
            {
                lexemeTestAttempts.First(x => x.Lexeme.Id == id).IsCorrectAnswer = true;
            }

            await _knowledgeTestService.SetResults(lexemeTestAttempts);
            HttpContext.Session.SetList("lexemeTestAttempts", lexemeTestAttempts);

            return RedirectToPage("TestResult");
        }

    }
}
