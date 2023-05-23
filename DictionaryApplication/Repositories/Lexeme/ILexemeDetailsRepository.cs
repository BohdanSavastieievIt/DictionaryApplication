using DictionaryApplication.Models;

namespace DictionaryApplication.Repositories
{
    public interface ILexemeDetailsRepository
    {
        Task<LexemeDetails?> GetByIdAsync(int id);
        Task<List<LexemeDetails>> GetAllAsync(params int[] userDictionaryIds);
        Task<(List<LexemeDetails>, int)> GetAllFilterAsync(int skip, int take, params int[] userDictionaryIds);
        Task<(List<LexemeDetails>, int)> GetAllFilterAsync(List<LexemeDetails> lexemeDetails, int skip, int take);
    }
}
