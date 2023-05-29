using DictionaryApplication.Models;

namespace DictionaryApplication.Repositories
{
    public interface IUserDictionaryRepository
    {
        Task CreateAsync(UserDictionary dict);
        Task DeleteAsync(int id);
        Task UpdateAsync(UserDictionary dict);
        Task<List<UserDictionary>> GetAllAsync(string userId);
        Task<List<(UserDictionary, int)>> GetAllWithLexemesAmountAsync(string userId);
        Task<(List<UserDictionary>, int)> GetAllFilterAsync(string userId, int skip, int take);
        Task<UserDictionary?> GetByIdAsync(int id);
        Task<int> GetLexemesAmount(int id);
    }
}
