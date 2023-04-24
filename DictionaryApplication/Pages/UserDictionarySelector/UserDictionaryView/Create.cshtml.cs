using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DictionaryApp.Data;
using DictionaryApp.Models;
using Microsoft.AspNetCore.Identity;

namespace DictionaryApplication.Pages.UserDictionarySelector.UserDictionaryView
{
    public class CreateModel : PageModel
    {
        private readonly DictionaryApp.Data.ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CreateModel(DictionaryApp.Data.ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult OnGet()
        {
            ViewData["Lexeme1Id"] = new SelectList(_context.Lexemes, "Id", "Word");
            ViewData["Lexeme2Id"] = new SelectList(_context.Lexemes, "Id", "Word");
            return Page();
        }

        [BindProperty]
        public LexemePair LexemePair { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {

            if (!ModelState.IsValid || _context.LexemePairs == null || LexemePair == null)
            {
                return Page();
            }


            _context.LexemePairs.Add(LexemePair);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
