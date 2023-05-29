using DictionaryApplication.Data;
using DictionaryApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace DictionaryApplication.Repositories
{
    public class UserDictionaryRepository : IUserDictionaryRepository
    {
        private readonly ApplicationDbContext _context;
        public UserDictionaryRepository(ApplicationDbContext context)
        { 
            _context = context;
        }

        public async Task CreateAsync(UserDictionary dict)
        {
            CheckUserDictionary(dict);

            _context.Add(dict);
            await _context.SaveChangesAsync();
        }

        private void CheckUserDictionary(UserDictionary? entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
        }

        public async Task UpdateAsync(UserDictionary dict)
        {
            CheckUserDictionary(dict);

            _context.Update(dict);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var dict = await _context.Set<UserDictionary>().FindAsync(id);
            CheckUserDictionary(dict);

            _context.Set<UserDictionary>().Remove(dict);
            await _context.SaveChangesAsync();
        }
        public async Task<(List<UserDictionary>, int)> GetAllFilterAsync(string userId, int skip, int take)
        {
            var all = _context.Set<UserDictionary>()
                .Where(x => x.UserId == userId)
                .Include(ud => ud.StudiedLanguage)
                .Include(ud => ud.TranslationLanguage)
                .Include(ud => ud.Lexemes);
            var relevant = await all.Skip(skip).Take(take).ToListAsync();
            var total = all.Count();

            (List<UserDictionary>, int) result = (relevant, total);

            return result;
        }

        public async Task<UserDictionary?> GetByIdAsync(int id)
        {
            return await _context.Set<UserDictionary>()
                .Include(ud => ud.StudiedLanguage)
                .Include(ud => ud.TranslationLanguage)
                .Include(ud => ud.Lexemes)
                .FirstOrDefaultAsync(ud => ud.Id == id);
        }

        public async Task<List<UserDictionary>> GetAllAsync(string userId)
        {
            return await _context.Set<UserDictionary>()
                .Where(x => x.UserId == userId)
                .Include(ud => ud.StudiedLanguage)
                .Include(ud => ud.TranslationLanguage)
                .Include(ud => ud.Lexemes)
                .ToListAsync();
        }

        public async Task<List<(UserDictionary, int)>> GetAllWithLexemesAmountAsync(string userId)
        {
            var dictionaries = await GetAllAsync(userId);
            var result = new List<(UserDictionary, int)>();
            foreach(var dictionary in dictionaries)
            {
                result.Add((dictionary, await GetLexemesAmount(dictionary.Id)));
            }

            return result;
        }

        public async Task<int> GetLexemesAmount(int id)
        {
            var dict = await GetByIdAsync(id);
            return dict == null || dict.Lexemes == null 
                ? 0 
                : dict.Lexemes.Count(l => l.LangId == dict.StudiedLangId);
        }
    }
}
