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
            LanguageOptions = new SelectList(_context.Languages.ToList(), "Id", "Name");
        }

        public SelectList LanguageOptions { get; set; }
        public UserDictionary UserDictionary { get; set; }
        [BindProperty]
        public string Name { get; set; }
        [BindProperty]
        public string Description { get; set; }
        [BindProperty]
        public int StudiedLangId { get; set; }
        [BindProperty]
        public int TranslationLangId { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            return Page();
        }
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                //foreach (var key in ModelState.Keys)
                //{
                //    foreach (var error in ModelState[key].Errors)
                //    {
                //        _logger.LogError($"Model error: {key}, {error.ErrorMessage}");
                //    }
                //}
                return Page();
            }
            UserDictionary = new UserDictionary();
            var currentUser = await _userManager.GetUserAsync(User);
            UserDictionary.User = currentUser;
            UserDictionary.Name = Name;
            if (Description != null)
            {
                UserDictionary.Description = Description;
            }
            
            UserDictionary.StudiedLanguage = await _context.Languages.FindAsync(StudiedLangId);
            UserDictionary.TranslationLanguage = await _context.Languages.FindAsync(TranslationLangId);
  
            _context.UserDictionaries.Add(UserDictionary);
            await _context.SaveChangesAsync();  

            return RedirectToPage("./Index");
        }
    }
}
