using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using MonolithicNetCore.Common;

namespace MonolithicNetCore.Data.Infrastructure
{
    public class EntityFrameworkRepository<T> : IDataRepository<T> where T : class
    {
        #region Field

        private BaseContext _dbContext;
        private DbSet<T> _dbSet;

        
        //private IDbFactory dbFactory;

        #endregion Field

        #region Ctr

        protected IDbFactory DbFactory
        {
            get;
            private set;
        }

        public IDbConnection Connection
        {
            get
            {
                return new MySqlConnection(ConfigurationUtility.GetConnectionStrings().PrimaryDatabaseConnectionString);
            }
        }

        protected BaseContext DbContext
        {
            get => _dbContext ?? (_dbContext = DbFactory.Init());
        }

        public EntityFrameworkRepository(IDbFactory dbFactory)
        {
            DbFactory = dbFactory;
            _dbSet = DbContext.Set<T>();
        }

        public virtual int Count() => _dbSet.Count();

        public virtual void Delete(T entity)
        {
            _dbSet.Remove(entity);
            DbContext.SaveChanges();
        }

        public virtual IEnumerable<T> GetAll() => DbContext.Set<T>().ToList();

        public virtual IEnumerable<T> GetAll(string[] includes = null)
        {
            if (includes != null && includes.Count() > 0)
            {
                var query = DbContext.Set<T>().Include(includes.First());
                foreach (var include in includes.Skip(1))
                    query = query.Include(include);
                return query.AsQueryable();
            }

            return _dbSet.AsQueryable();
        }

        public T GetById(Expression<Func<T, bool>> where) => _dbSet.Where(where).FirstOrDefault();
        public T GetSingleByCondition(Expression<Func<T, bool>> where, string[] includes = null)
        {
            if (includes != null && includes.Count() > 0)
            {
                var query = _dbSet.Where(where).Include(includes.First());
                foreach (var include in includes.Skip(1))
                    query = query.Include(include).AsQueryable();
                return query.FirstOrDefault();
            }
            return _dbSet.Where(where).FirstOrDefault();
        }

        public IEnumerable<T> GetMany(Expression<Func<T, bool>> where) => _dbSet.Where(where);

        public IEnumerable<T> GetMulti(Expression<Func<T, bool>> where, string[] includes = null)
        {
            if (includes != null && includes.Count() > 0)
            {
                var query = _dbSet.Where(where).Include(includes.First());
                foreach (var include in includes.Skip(1))
                    query = query.Include(include).AsQueryable();
                return query;
            }
            return _dbSet.Where(where);
        }

        public T Insert(T entity)
        {
            _dbSet.Add(entity);
            DbContext.SaveChanges();
            return entity;
        }

        public void InsertList(List<T> entities)
        {
            entities.ForEach(entity => _dbSet.Add(entity));
            DbContext.SaveChanges();
        }

        public T Update(T entity)
        {
            _dbSet.Attach(entity);
            DbContext.Entry(entity).State = EntityState.Modified;
            DbContext.SaveChanges();
            return entity;
        }

        #endregion Ctr
    }
}
