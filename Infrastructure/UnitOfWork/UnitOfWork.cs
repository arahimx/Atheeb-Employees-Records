using Core.IRepository;
using Infrastructure.Data;
using Infrastructure.Repository;

namespace Infrastructure.UnitOfWork
{
    public class UnitOfWork<T> : IUnitOfWork<T> where T : class
    {
        private readonly ApplicationDbContext context;
        private IGenericRepository<T> entity;
        public UnitOfWork(ApplicationDbContext context)
        {
            this.context = context;
        }


        public IGenericRepository<T> Entity
        {
            get
            {
                return entity ?? (entity = new GenericRepository<T>(context));
            }
        }



        public void Save()
        {
            context.SaveChanges();
        }

        public void Dispose()
        {
            context.Dispose();
        }
    }
}
