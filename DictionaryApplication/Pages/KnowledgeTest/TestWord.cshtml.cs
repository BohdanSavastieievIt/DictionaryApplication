using DictionaryApplication.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using DictionaryApplication.Models;
using DictionaryApplication.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace DictionaryApplication.Pages.KnowledgeTest
{
    [Authorize]
    public class TestWordModel : PageModel
    {
        public TestWordModel() {}

        [BindProperty]
        public string? TestAnswer { get; set; }
        public string DisplayedLexeme { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync()
        {
            await HttpContext.Session.LoadAsync();

            var currentLexemeId = HttpContext.Session.GetInt32("currentLexemeId");
            if (currentLexemeId == null)
            {
                return RedirectToPage("KnowledgeTestStart");
            }

            var currentLexeme = HttpContext.Session.GetList<LexemeTestAttempt>("lexemeTestAttempts")[(int)currentLexemeId];
            if (currentLexeme == null || currentLexeme.LexemeTestRepresentation == null)
            {
                return RedirectToPage("KnowledgeTestStart");
            }

            DisplayedLexeme = currentLexeme.LexemeTestRepresentation;

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await HttpContext.Session.LoadAsync();

            var lexemeTestAttempts = HttpContext.Session.GetList<LexemeTestAttempt>("lexemeTestAttempts");
            var currentLexemeId = HttpContext.Session.GetInt32("currentLexemeId");
            if (currentLexemeId == null)
            {
                return RedirectToPage("KnowledgeTestStart");
            }

            lexemeTestAttempts[(int)currentLexemeId].TestAnswer = TestAnswer;
            HttpContext.Session.SetList("lexemeTestAttempts", lexemeTestAttempts);

            currentLexemeId++;
            if (lexemeTestAttempts.Count <= currentLexemeId)
            {
                HttpContext.Session.Remove("currentLexemeId");

                return RedirectToPage("WrongAnswersSelection");
            }
            HttpContext.Session.SetInt32("currentLexemeId", (int)currentLexemeId);

            await HttpContext.Session.CommitAsync();

            return RedirectToPage("TestWord");
        }
    }
}
