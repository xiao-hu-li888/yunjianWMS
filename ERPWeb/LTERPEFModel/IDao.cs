using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Text;

namespace LTERPEFModel
{
    public interface IDao<T> : IDisposable
    {
        DbContextTransaction BeginTransaction();
        SimpleBackValue AddIfNotExists<TKey>(T Entity, Func<T, TKey> predicate);
        SimpleBackValue Add(T Entity);
        SimpleBackValue AddRange(List<T> Entity);
        SimpleBackValue Delete(T Entity);
        //SimpleBackValue Delete(Expression<Func<T, bool>> whereLambda);
        SimpleBackValue Update(T Entity);
        //bool Update(Expression<Func<T, bool>> WhereLambda, Expression<Func<T, T>> UpdateLambda); 
        SimpleBackValue Update(List<T> Entity);
        //  SimpleBackValue Update(T model, Expression<Func<T, bool>> WhereLambda, params string[] ModifiedProNames);
        //   T FindByID(dynamic ID);
       /* 并发出问题，禁用！！！
        * T GetFirstDefaultAsTracking(Expression<Func<T, bool>> WhereLambda = null);
        List<T> GetAllQueryAsTracking(Expression<Func<T, bool>> WhereLambda = null);
       */
        T GetModel(System.Guid guid);

        T GetFirstDefault(Expression<Func<T, bool>> WhereLambda = null);
        //List<T> GetAll(string Order = null);
        List<T> GetAllQuery(Expression<Func<T, bool>> WhereLambda = null);
        List<T> GetAllQueryOrderby<TKey>(Expression<Func<T, TKey>> OrderBy, Expression<Func<T, bool>> WhereLambda = null, bool IsAsc = true);
        int GetCount(Expression<Func<T, bool>> WhereLambda = null);
        bool GetAny(Expression<Func<T, bool>> WhereLambda = null);
        List<T> Pagination<TKey>(int PageIndex, int PageSize, out int TotalCount, Expression<Func<T, TKey>> OrderBy, Expression<Func<T, bool>> WhereLambda = null, bool IsAsc = true);
        int ExecuteSQLCommand(string sqlcommand, DbParameter[] parameters);
    }
}
