using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DictionaryApplication.Data;
using Microsoft.AspNetCore.Identity;
using DictionaryApplication.Paging;
using NuGet.Protocol.Core.Types;
using DictionaryApplication.Models;
using DictionaryApplication.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace DictionaryApplication.Pages.UserDictionarySelector
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserDictionaryRepository _userDictionaryRepository;

        public IndexModel(UserManager<ApplicationUser> userManager, IUserDictionaryRepository userDictionaryRepository)
        {
            _userManager = userManager;
            _userDictionaryRepository = userDictionaryRepository;
        }

        //public List<UserDictionary> UserDictionaries { get; set; } = null!;
        public List<(UserDictionary UserDictionary, int TotalLexemes)> UserDictionaries { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            UserDictionaries = await _userDictionaryRepository.GetAllWithLexemesAmountAsync(currentUser.Id);
            if (UserDictionaries.Count == 0)
            {
                return RedirectToPage("Create", new { isNoDictionaries = true });
            }

            return Page();
        }
    }
}
