using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Text;

namespace DbBackUpService
{
    public interface IDao<T> : IDisposable
    {
        DbContextTransaction BeginTransaction();
        SimpleBackValue AddIfNotExists<TKey>(T Entity, Func<T, TKey> predicate);
        SimpleBackValue Add(T Entity);
        SimpleBackValue AddRange(List<T> Entity);
        SimpleBackValue Delete(T Entity); 
        SimpleBackValue Update(T Entity); 
        SimpleBackValue Update(List<T> Entity); 
        T GetModel(System.Guid guid);

        T GetFirstDefault(Expression<Func<T, bool>> WhereLambda = null); 
        List<T> GetAllQuery(Expression<Func<T, bool>> WhereLambda = null);
        List<T> GetAllQueryOrderby<TKey>(Expression<Func<T, TKey>> OrderBy, Expression<Func<T, bool>> WhereLambda = null, bool IsAsc = true);
        int GetCount(Expression<Func<T, bool>> WhereLambda = null);
        bool GetAny(Expression<Func<T, bool>> WhereLambda = null);
        List<T> Pagination<TKey>(int PageIndex, int PageSize, out int TotalCount, Expression<Func<T, TKey>> OrderBy, Expression<Func<T, bool>> WhereLambda = null, bool IsAsc = true);
        int ExecuteSQLCommand(string sqlcommand, DbParameter[] parameters);
    }
}
