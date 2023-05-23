using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DictionaryApplication.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using DictionaryApplication.Models;
using DictionaryApplication.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace DictionaryApplication.Pages.UserDictionarySelector
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserDictionaryRepository _dictRepository;
        private readonly IDbRepository<Language> _langRepository;


        public CreateModel(UserManager<ApplicationUser> userManager, 
            IUserDictionaryRepository dictRepository,
            IDbRepository<Language> langRepository)
        {
            _userManager = userManager;
            _dictRepository = dictRepository;
            _langRepository = langRepository;
        }

        [BindProperty]
        public UserDictionary UserDictionary { get; set; } = null!;
        public bool ShowNoDictionariesError { get; set; }

        public async Task<IActionResult> OnGet(bool isNoDictionaries = false)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            if (isNoDictionaries)
            {
                ShowNoDictionariesError = true;
                ViewData["NoDictionariesError"] = "Firstly, you need to create at least one dictionary and fill it with some lexemes.";
            }

            ViewData["CurrentUserId"] = currentUser.Id;
            var languages = await _langRepository.GetAllAsync();
            ViewData["StudiedLangId"] = new SelectList(languages, "Id", "LangCode");
            ViewData["TranslationLangId"] = new SelectList(languages, "Id", "LangCode", languages.Skip(1).First().Id);

            return Page();
        }
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || UserDictionary == null)
            {
                return Page();
            }

            await _dictRepository.CreateAsync(UserDictionary);

            return RedirectToPage("./Index");
        }
    }
}
