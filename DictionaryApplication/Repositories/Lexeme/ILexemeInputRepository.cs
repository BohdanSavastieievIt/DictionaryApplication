using DictionaryApplication.Models;

namespace DictionaryApplication.Repositories
{
    public interface ILexemeInputRepository
    {
        Task CreateAsync(int dictionaryId, LexemeInput lexemeInput);
        Task UpdateAsync(int lexemeId, LexemeInput lexemeInput);
        Task DeleteAsync(int lexemeId);
        Task<LexemeInput?> GetByIdAsync(int lexemeId);
        Task<List<LexemeInput>> GetAllAsync(params int[] userDictionaryIds);

    }
}
