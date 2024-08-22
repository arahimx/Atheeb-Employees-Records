using Core.IRepository;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq.Expressions;

namespace Infrastructure.Repository
{
    internal class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext context;

        private DbSet<T>? table = null;

        public GenericRepository()
        {
        }

        public GenericRepository(ApplicationDbContext context)
        {
            this.context = context;
            table = this.context.Set<T>();
        }

        public bool CheckIfEntityExists(Expression<Func<T, bool>> expr)
        {
            return table.Any(expr);
        }

        public void Delete(object id)
        {
            T existing = GetById(id);
            table.Remove(existing);
        }
        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, string? includeProperties = null, bool isTracking = true)
        {
            IQueryable<T> query = context.Set<T>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includeProperties != null)
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (!isTracking)
            {
                query = query.AsNoTracking();
            }
            return query.ToList();
        }
        public T GetById(object id)
        {
            return table!.Find(id);
        }
        public async Task<T> GetEntityByPropertyAsync<T>(string propertyName, string propertyValue) where T : class
        {
            var entityType = typeof(T);
            var parameter = Expression.Parameter(entityType, "x");
            var property = Expression.Property(parameter, propertyName);
            var value = Expression.Constant(propertyValue);
            var equalsMethod = typeof(string).GetMethod("Equals", new[] { typeof(string) });
            var equalsExpression = Expression.Call(property, equalsMethod, value);
            var lambda = Expression.Lambda<Func<T, bool>>(equalsExpression, parameter);

            return await context.Set<T>()
                .Where(lambda)
                .FirstOrDefaultAsync();
        }
        public void Insert(T entity)
        {
            table!.Add(entity);
        }

        public void Update(T entity)
        {
            //table.Attach(entity);
            context.Entry(entity).State = EntityState.Modified;
        }     
    }
}
