using DictionaryApplication.Data;
using DictionaryApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace DictionaryApplication.Repositories
{
    public class LexemeTestAttemptRepository : ILexemeTestAttemptRepository
    {
        private readonly ApplicationDbContext _context;
        public LexemeTestAttemptRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<LexemeTestAttempt>> GetAllAsync(params int[] userDictionaryIds)
        {
            var result = new List<LexemeTestAttempt>();
            List<int> studiedLexemeIds = await _context.Lexemes.Where(x => userDictionaryIds.Contains(x.DictionaryId)
                && x.LexemePairs != null && x.LexemePairs.Count > 0).Select(x => x.Id).ToListAsync();
            foreach (var lexemeId in studiedLexemeIds)
            {
                var lexeme = await GetByIdAsync(lexemeId);
                if (lexeme != null)
                {
                    result.Add(lexeme);
                }
            }

            return result;
        }

        public async Task<LexemeTestAttempt?> GetByIdAsync(int lexemeId)
        {
            var lexeme = await _context.Lexemes.FirstOrDefaultAsync(x => x.Id == lexemeId)
                ?? throw new ArgumentNullException(nameof(lexemeId), "The lexeme for update was not found.");

            var translations = await _context.Lexemes.Where(x =>
                        _context.LexemeTranslationPairs
                            .Where(y => y.LexemeId == lexeme.Id)
                            .Select(y => y.TranslationId).Contains(x.Id))
                        .ToListAsync();

            var result = new LexemeTestAttempt
            {
                Lexeme = lexeme,
                Translations = translations,
            };

            return result;
        }

        public async Task UpdateTestResultAsync(LexemeTestAttempt lexemeTest)
        {
            var lexemeToUpdate = await _context.Lexemes.FindAsync(lexemeTest.Lexeme.Id)
                ?? throw new ArgumentNullException(nameof(lexemeTest), "The lexeme for update was not found.");

            lexemeToUpdate.CorrectTestAttempts = lexemeTest.Lexeme.CorrectTestAttempts;
            lexemeToUpdate.TotalTestAttempts = lexemeTest.Lexeme.TotalTestAttempts;

            _context.Update(lexemeToUpdate);
            await _context.SaveChangesAsync();
        }

    }
}
