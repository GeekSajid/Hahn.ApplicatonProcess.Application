using Hahn.ApplicatonProcess.December2020.Data.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Hahn.ApplicatonProcess.December2020.Data.Generics
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private DbSet<T> dbEntity;
        private ApplicationDbContext _context;

        public Repository(ApplicationDbContext context)
        {
            this._context = context;
            dbEntity = _context.Set<T>();
        }

        #region Generics

        public virtual IEnumerable<T> Get(
           Expression<Func<T, bool>> filter = null,
           Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
           string includeProperties = "")
        {
            IQueryable<T> query = dbEntity;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.AsNoTracking().ToList();
            }
        }


        public void Delete(dynamic id)
        {
            T model = dbEntity.Find(id);
            dbEntity.Remove(model);
        }

        public IEnumerable<T> GetAll()
        {
            return dbEntity.ToList();
        }

        public IQueryable<T> GetQueryable()
        {
            return dbEntity;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await dbEntity.ToListAsync();
        }

        public T GetByID(dynamic id)
        {
            return dbEntity.Find(id);
        }

        public T GetByDoubleID(dynamic id1, dynamic id2)
        {
            return dbEntity.Find(id1, id2);
        }

        public void Insert(T model)
        {
            try
            {
                dbEntity.Add(model);

            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

        }

        public void Update(T model)
        {
            _context.Set<T>().Update(model);
        }

        public bool IsExist(dynamic id)
        {
            try
            {
                var ty = dbEntity.Find(id);
                if (ty != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool IsExist(dynamic id1, dynamic id2)
        {
            try
            {
                var ty = dbEntity.Find(id1, id2);
                if (ty != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }

        }

        T IRepository<T>.GetByDoubleID(dynamic id1, dynamic id2)
        {
            return dbEntity.Find(id1, id2);
        }

        public void Save()
        {
            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }       

        #endregion
    }

    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        Task<IEnumerable<T>> GetAllAsync();
        IQueryable<T> GetQueryable();
        // object GetAllForJoin(T model);
        T GetByID(dynamic id);
        // T GetByStringID(dynamic id);
        T GetByDoubleID(dynamic id1, dynamic id2);
        IEnumerable<T> Get(
           Expression<Func<T, bool>> filter = null,
           Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
           string includeProperties = "");
        void Insert(T model);

        void Update(T model);

        void Delete(dynamic id);

        bool IsExist(dynamic id);

        bool IsExist(dynamic id1, dynamic id2);

        //T FindBy(Expression<Func<T, bool>> predicate);

        void Save();

    }
}
