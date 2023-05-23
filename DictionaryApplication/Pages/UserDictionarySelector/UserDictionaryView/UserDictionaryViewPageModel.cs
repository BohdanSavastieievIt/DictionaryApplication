using DictionaryApplication.Models;
using DictionaryApplication.Repositories;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace DictionaryApplication.Pages.UserDictionarySelector.UserDictionaryView
{
    [Authorize]
    public abstract class UserDictionaryViewPageModel : PageModel
    {
        protected readonly IUserDictionaryRepository _userDictionaryRepository;
        public UserDictionary UserDictionary { get; set; } = null!;

        protected UserDictionaryViewPageModel(IUserDictionaryRepository userDictionaryRepository)
        {
            _userDictionaryRepository = userDictionaryRepository;
        }
        protected async Task<IActionResult> LoadUserDictionaryAsync(int userDictionaryId)
        {
            if (userDictionaryId == 0)
            {
                return RedirectToPage("/UserDictionarySelector/Index");
            }

            var userDictionary = await _userDictionaryRepository.GetByIdAsync(userDictionaryId);
            if (userDictionary == null)
            {
                return RedirectToPage("/UserDictionarySelector/Index");
            }

            UserDictionary = userDictionary;
            return null;
        }
    }

}
