using Microsoft.AspNetCore.Identity;

namespace DictionaryApp.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public IdentityUser IdentityUser { get; set; } = null!;
        public ICollection<UserDictionary> UserDictionaries { get; set; } = null!;
    }
}
