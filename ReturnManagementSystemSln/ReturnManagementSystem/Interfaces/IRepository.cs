namespace ReturnManagementSystem.Interfaces
{
    public interface IRepository<K, T> where T : class
    {
        Task<T> Add(T entity);
        Task<T> Update(T entity);
        Task<T> Delete(T entity);
        Task<IEnumerable<T>> GetAll();
        Task<T> Get(K key);
        Task<IEnumerable<T>> FindAll(Func<T, bool> predicate);
    }
}
