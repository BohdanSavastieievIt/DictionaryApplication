using DictionaryApplication.Models;

namespace DictionaryApplication.Repositories
{
    public interface ILexemeTestAttemptRepository
    {
        Task<LexemeTestAttempt?> GetByIdAsync(int lexemeId);
        Task<List<LexemeTestAttempt>> GetAllAsync(params int[] userDictionaryIds);
        Task UpdateTestResultAsync(LexemeTestAttempt lexeme);
    }
}
