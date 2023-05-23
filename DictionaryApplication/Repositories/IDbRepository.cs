namespace DictionaryApplication.Repositories
{
    public interface IDbRepository<TEntity> where TEntity : class
    {
        Task CreateAsync(TEntity entity);
        Task DeleteAsync(int id);
        Task UpdateAsync(TEntity entity);
        Task<List<TEntity>> GetAllAsync();
        Task<(List<TEntity>, int)> GetAllFilterAsync(int skip, int take);
        Task<TEntity?> GetByIdAsync(int id);
    }
}
