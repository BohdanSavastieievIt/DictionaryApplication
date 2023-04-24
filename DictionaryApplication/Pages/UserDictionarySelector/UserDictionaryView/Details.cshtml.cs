using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DictionaryApp.Data;
using DictionaryApp.Models;

namespace DictionaryApplication.Pages.UserDictionarySelector.UserDictionaryView
{
    public class DetailsModel : PageModel
    {
        private readonly DictionaryApp.Data.ApplicationDbContext _context;

        public DetailsModel(DictionaryApp.Data.ApplicationDbContext context)
        {
            _context = context;
        }

      public LexemePair LexemePair { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.LexemePairs == null)
            {
                return NotFound();
            }

            var lexemepair = await _context.LexemePairs.FirstOrDefaultAsync(m => m.Lexeme1Id == id);
            if (lexemepair == null)
            {
                return NotFound();
            }
            else 
            {
                LexemePair = lexemepair;
            }
            return Page();
        }
    }
}
