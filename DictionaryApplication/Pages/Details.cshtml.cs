using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DictionaryApp.Data;
using DictionaryApp.Models;

namespace DictionaryApplication.Pages
{
    public class DetailsModel : PageModel
    {
        private readonly DictionaryApp.Data.ApplicationDbContext _context;

        public DetailsModel(DictionaryApp.Data.ApplicationDbContext context)
        {
            _context = context;
        }

      public UserDictionary UserDictionary { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.UserDictionaries == null)
            {
                return NotFound();
            }

            var userdictionary = await _context.UserDictionaries.FirstOrDefaultAsync(m => m.Id == id);
            if (userdictionary == null)
            {
                return NotFound();
            }
            else 
            {
                UserDictionary = userdictionary;
            }
            return Page();
        }
    }
}
