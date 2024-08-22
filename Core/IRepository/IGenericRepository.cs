using Core.DTO;
using System.Linq.Expressions;

namespace Core.IRepository
{
    public interface IGenericRepository<T> where T : class
    {
        IEnumerable<T> GetAll(
    Expression<Func<T, bool>>? filter = null,
    Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
    string? includeProperties = null,
    bool isTracking = true);

        T GetById(object id);
        void Insert(T entity);
        //void Insert(T entity,IList<int> T);
        void Update(T entity);
        void Delete(object id);

        public bool CheckIfEntityExists(Expression<Func<T, bool>> expr);
        Task<T> GetEntityByPropertyAsync<T>(string propertyName, string propertyValue) where T : class;
    }

    public interface IUnitOfWork<T> where T : class
    {
        IGenericRepository<T> Entity { get; }
        void Save();
    } 
}

