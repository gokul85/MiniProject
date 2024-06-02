using System.Linq.Expressions;

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
        Task<IEnumerable<T>> FindAllWithIncludes(Func<T, bool> predicate, params Expression<Func<T, object>>[] includes);
        Task<IEnumerable<T>> GetAllWithIncludes(params Expression<Func<T, object>>[] includes);
        //Task<T> GetWithIncludes(K key, params Expression<Func<T, object>>[] includes);
    }
}
