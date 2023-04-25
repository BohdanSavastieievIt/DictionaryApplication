using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DictionaryApp.Data;
using DictionaryApp.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DictionaryApplication.Pages.UserDictionarySelector
{
    public class CreateModel : PageModel
    {
        private readonly DictionaryApp.Data.ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(DictionaryApp.Data.ApplicationDbContext context, 
            UserManager<ApplicationUser> userManager, 
            ILogger<CreateModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }


        [BindProperty]
        public UserDictionary UserDictionary { get; set; } = null!;
        public string CurrentUserId { get; set; } = null!;
        public async Task<IActionResult> OnGet()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            CurrentUserId = currentUser.Id;
            ViewData["StudiedLangId"] = new SelectList(_context.Languages, "Id", "LangCode");
            ViewData["TranslationLangId"] = new SelectList(_context.Languages, "Id", "LangCode", _context.Languages.Skip(1).First().Id);

            return Page();
        }
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || _context.UserDictionaries == null || UserDictionary == null)
            {
                return Page();
            }
            
            _context.UserDictionaries.Add(UserDictionary);
            await _context.SaveChangesAsync();  

            return RedirectToPage("./Index");
        }
    }
}
