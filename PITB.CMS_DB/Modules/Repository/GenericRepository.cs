using PITB.CMS_DB.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
//using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;

namespace PITB.CMS_DB.Modules.Repository
{
    public class GenericRepository<T> where T : class//, IDisposable
    {
        internal DBContextHelperLinq dbContext = null;
        internal DbSet<T> dbSet;
        private bool isDisposed = false;

        public GenericRepository()
        {
            dbContext = new DBContextHelperLinq();
            dbSet = dbContext.Set<T>();
        }

        public GenericRepository(DBContextHelperLinq dbContext)
        {
            this.dbContext = dbContext==null ? new DBContextHelperLinq(): dbContext;
            dbSet = this.dbContext.Set<T>();
        }

        public T GetSingle(Expression<Func<T, bool>> where = null, bool asNoTracking = true,
            string includeProperties = "")
        {
            IQueryable<T> query = dbSet;
            if (where != null)
            {
                query = query.Where(where);
            }
            if (asNoTracking)
            {
                query.AsNoTracking();
            }
            return query.FirstOrDefault();
        }

        public IEnumerable<T> Get(Expression<Func<T,bool>> where = null, 
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            bool asNoTracking = true,
            string includeProperties = "")
        {
            IQueryable<T> query = dbSet;
            if (where != null)
            {
                query = query.Where(where);
            }
            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }
            if (orderBy != null)
            {
                orderBy(query);
            }
            if(asNoTracking)
            {
                query.AsNoTracking();
            }
            
            return query.AsEnumerable();
        }

        public virtual T GetById(object id, bool asNoTracking = true)
        {
            IQueryable<T> query = dbSet;
            ParameterExpression pe = Expression.Parameter(typeof(T), "x");
            Expression left = Expression.Property(pe, typeof(T).GetProperty("Id"));
            Expression right = Expression.Constant(id, typeof(int));
            Expression e1 = Expression.Equal(left, right);
            Expression<Func<T, bool>> lambda = Expression.Lambda<Func<T, bool>>(e1, pe);

            //Expression<Func<T, bool>> where = (Expression<Func<T, bool>>)lambda;
            //where.
            query = query.Where(lambda);
            if (asNoTracking)
            {
                query.AsNoTracking();
            }
            return (T) query.FirstOrDefault();
            //return dbSet.Find(id);
        }

        public virtual void Insert(T entity)
        {
            dbSet.Add(entity);
        }

        public virtual void Delete(object id)
        {
            T entityToDelete = dbSet.Find(id);
            Delete(entityToDelete);
        }

        public virtual void Delete(T entityToDelete)
        {
            if (dbContext.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);
        }

        public virtual void Update(T entityToUpdate)
        {
            dbSet.Attach(entityToUpdate);
            dbContext.Entry(entityToUpdate).State = EntityState.Modified;
        }

        protected void Dispose(bool isDisposing)
        {
            if (!isDisposed)
            {
                if (isDisposing)
                {
                    dbContext.Dispose();
                }
            }
            isDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
