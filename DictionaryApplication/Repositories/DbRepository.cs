using DictionaryApplication.Data;
using Microsoft.EntityFrameworkCore;

namespace DictionaryApplication.Repositories
{
    public class DbRepository<TEntity> : IDbRepository<TEntity> where TEntity : class
    {
        protected readonly ApplicationDbContext _context;
        public DbRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task CreateAsync(TEntity entity)
        {
            CheckEntity(entity);

            _context.Add(entity);
            await _context.SaveChangesAsync();
        }

        protected void CheckEntity(TEntity? entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
        }

        public async Task UpdateAsync(TEntity entity)
        {
            CheckEntity(entity);

            _context.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Set<TEntity>().FindAsync(id);
            CheckEntity(entity);

            _context.Set<TEntity>().Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<TEntity?> GetByIdAsync(int id)
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }

        public async Task<List<TEntity>> GetAllAsync()
        {
            return await _context.Set<TEntity>().ToListAsync();
        }

        public async Task<(List<TEntity>, int)> GetAllFilterAsync(int skip, int take)
        {
            var all = _context.Set<TEntity>();
            var relevant = await all.Skip(skip).Take(take).ToListAsync();
            var total = all.Count();

            (List<TEntity>, int) result = (relevant, total);

            return result;
        }
    }
}
