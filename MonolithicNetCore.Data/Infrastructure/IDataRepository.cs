using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace MonolithicNetCore.Data.Infrastructure
{
    public interface IDataRepository<T> where T : class
    {
        T Insert(T entity);

        void Delete(T entity);

        T Update(T entity);

        T GetById(Expression<Func<T, bool>> where);

        T GetSingleByCondition(Expression<Func<T, bool>> where, string[] includes = null);

        IEnumerable<T> GetMany(Expression<Func<T, bool>> where);

        IEnumerable<T> GetMulti(Expression<Func<T, bool>> where, string[] includes);

        IEnumerable<T> GetAll();

        IEnumerable<T> GetAll(string[] includes);

        int Count();

        void InsertList(List<T> entities);
    }
}
