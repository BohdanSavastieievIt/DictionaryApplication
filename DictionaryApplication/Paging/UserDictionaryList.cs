using DictionaryApplication.Models;

namespace DictionaryApplication.Paging
{
    public class UserDictionaryList
    {
        public IEnumerable<UserDictionary> Dictionaries { get; set; } = null!;
        public PagingInfo PagingInfo { get; set; } = null!;
    }
}
