using DictionaryApplication.Data;
using DictionaryApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace DictionaryApplication.Repositories
{
    public class LexemeDetailsRepository : ILexemeDetailsRepository
    {
        private readonly ApplicationDbContext _context;

        public LexemeDetailsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        private void CheckLexeme(Lexeme? lexeme)
        {
            if (lexeme == null)
            {
                throw new ArgumentException("Lexeme with id specified was not found.");
            }
        }
        public async Task<LexemeDetails?> GetByIdAsync(int id)
        {
            var lexeme = await _context.Lexemes.FirstOrDefaultAsync(x => x.Id == id);
            CheckLexeme(lexeme);

            var result = new LexemeDetails(_context, lexeme.Id);
            return result;
        }

        public async Task<List<LexemeDetails>> GetAllAsync(params int[] userDictionaryIds)
        {
            List<int> studiedLexemeIds = await _context.Lexemes.Where(x => userDictionaryIds.Contains(x.DictionaryId)
                && x.LexemePairs != null && x.LexemePairs.Count > 0).Select(x => x.Id).ToListAsync();
            List<LexemeDetails> result = new List<LexemeDetails>();
            foreach (var id in studiedLexemeIds)
            {
                result.Add(new LexemeDetails(_context, id));
            }
            return result;
        }
        public async Task<(List<LexemeDetails>, int)> GetAllFilterAsync(int skip, int take, params int[] userDictionaryIds)
        {
            List<LexemeDetails> all = await GetAllAsync(userDictionaryIds);
            return await GetFilteredForPagingAsync(all, skip, take);
        }

        public async Task<(List<LexemeDetails>, int)> GetAllFilterAsync(List<LexemeDetails> lexemeDetails, int skip, int take)
        {
            return await GetFilteredForPagingAsync(lexemeDetails, skip, take);
        }

        private async Task<(List<LexemeDetails>, int)> GetFilteredForPagingAsync(List<LexemeDetails> lexemeDetails, int skip, int take)
        {
            List<LexemeDetails> relevant = lexemeDetails.Skip(skip).Take(take).ToList();
            var total = lexemeDetails.Count;

            (List<LexemeDetails>, int) result = (relevant, total);

            return result;
        }
    }
}
