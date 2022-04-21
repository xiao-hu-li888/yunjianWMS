using LTWMSEFModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace LTWMSEFModel
{
    public class ComDao<T> : IDao<T> where T : BaseBaseEntity
    {
        protected LTModel dbcontext;
        public ComDao(LTModel dbcontext)
        {
            this.dbcontext = dbcontext;
        }
        /// <summary>
        /// 开启事务
        /// </summary>
        /// <returns></returns>
        public DbContextTransaction BeginTransaction()
        {
            return dbcontext.Database.BeginTransaction();
            //dbcontext.Database.BeginTransaction()
            //return dbcontext.Database.BeginTransaction(IsolationLevel.ReadCommitted);
        }
        /// <summary>
        /// 根据指定的字段值查找数据库中是否存在，不存在则添加，如果存在则提示数据已存在错误。
        /// 用法:
        /// var user1 = new User { Name = "Peter", Age = 32 };
        /// AddIfNotExists(user1, u => u.Name);
        /// AddIfNotExists(user1, u => u.Age);
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="Entity"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public SimpleBackValue AddIfNotExists<TKey>(T Entity, Func<T, TKey> predicate)
        {
            try
            {
                var newValues = predicate.Invoke(Entity);
                Expression<Func<T, bool>> compare = arg => predicate(arg)!=null&&predicate(arg).Equals(newValues);
                var compiled = compare.Compile();
                var existing = dbcontext.Set<T>().Any(compiled);
                if (existing)
                {
                    return SimpleBackValue.ExistsOfAdd;
                }
                else
                {
                    dbcontext.Set<T>().Add(Entity);
                }
                return dbcontext.SaveChanges() > 0 ? SimpleBackValue.True : SimpleBackValue.False;
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException ex)
            {//修改的数据已被修改
                dbcontext.Set<T>().Remove(Entity);
                //1/正常并发报异常 2、查询未加【AsNoTracking】 导致修改报并发异常(修改的版本比数据库版本低)

                /*
                  foreach (var entry in ex.Entries)
                {
                    //if (entry.Entity is T)
                    //{
                    var proposedValues = entry.CurrentValues;
                    var databaseValues = entry.GetDatabaseValues();

                    foreach (var property in proposedValues.PropertyNames)
                    {
                        var proposedValue = proposedValues[property];
                        var databaseValue = databaseValues[property];

                        // TODO: decide which value should be written to database
                        // proposedValues[property] = <value to be saved>;
                    }

                    // Refresh original values to bypass next concurrency check
                    entry.OriginalValues.SetValues(databaseValues);
                    //}
                    //else
                    //{
                    //    throw new NotSupportedException(
                    //        "Don't know how to handle concurrency conflicts for "
                    //        + entry.Metadata.Name);
                    //}
                }
                */
                return SimpleBackValue.DbUpdateConcurrencyException;
            }
            catch (DbEntityValidationException ex)
            {
                dbcontext.Set<T>().Remove(Entity);
                throw new Exception(GetDbEntityValidationException(ex));
            }
            catch (Exception ex)
            {
                dbcontext.Set<T>().Remove(Entity);
                throw ex;
            }
        }
        /// <summary>
        /// 获取数据验证模型具体错误信息
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private string GetDbEntityValidationException(DbEntityValidationException ex)
        {
            StringBuilder errors = new StringBuilder();
            IEnumerable<DbEntityValidationResult> validationResult = ex.EntityValidationErrors;
            foreach (DbEntityValidationResult result in validationResult)
            {
                ICollection<DbValidationError> validationError = result.ValidationErrors;
                foreach (DbValidationError err in validationError)
                {
                    errors.Append(err.PropertyName + ":" + err.ErrorMessage + ";\r\n");
                }
            }
            return errors.ToString();
        }
        public SimpleBackValue Add(T Entity)
        {
            try
            {
               // 添加时报并发异常，测试一下添加并发异常
                var enty = dbcontext.Set<T>().Add(Entity);
                //   dbcontext.Set<T>.Delete(enty);
                return dbcontext.SaveChanges() > 0 ? SimpleBackValue.True : SimpleBackValue.False;
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException ex)
            {//修改的数据已被修改
                dbcontext.Set<T>().Remove(Entity);
                //1/正常并发报异常 2、查询未加【AsNoTracking】 导致修改报并发异常(修改的版本比数据库版本低)
                return SimpleBackValue.DbUpdateConcurrencyException;
            }
            catch (DbEntityValidationException ex)
            {
                dbcontext.Set<T>().Remove(Entity);
                throw new Exception(GetDbEntityValidationException(ex));
            }
            catch (Exception ex)
            {
                dbcontext.Set<T>().Remove(Entity);
                throw ex;
            }
            //catch(Exception ex)
            //{
            //  如果添加异常，从dbcontext中删除
            //异常情况处理。。。。
            return SimpleBackValue.False;
            //} 
        }
        public SimpleBackValue AddRange(List<T> Entity)
        {
            try
            {
                dbcontext.Set<T>().AddRange(Entity);
                return dbcontext.SaveChanges() >= Entity.Count ? SimpleBackValue.True : SimpleBackValue.False;
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException ex)
            {//修改的数据已被修改
                dbcontext.Set<T>().RemoveRange(Entity);
                //1/正常并发报异常 2、查询未加【AsNoTracking】 导致修改报并发异常(修改的版本比数据库版本低)
                return SimpleBackValue.DbUpdateConcurrencyException;
            }
            catch (DbEntityValidationException ex)
            {
                dbcontext.Set<T>().RemoveRange(Entity);
                throw new Exception(GetDbEntityValidationException(ex));
            }
            catch (Exception ex)
            {
                dbcontext.Set<T>().RemoveRange(Entity);
                throw ex;
            }
            //catch (Exception ex)
            //{
            //如果添加异常，从dbcontext中删除
            //异常情况处理。。。。
            return SimpleBackValue.False;
            //}
        }
        
        /// <summary>
        /// 删除数据 
        /// </summary>
        /// <param name="Entity"></param>
        /// <returns></returns>
        public SimpleBackValue Delete(T Entity)
        {
            try
            {
                T obj = dbcontext.Set<T>().Find(Entity.guid);
                if (obj != null)
                {//高并发时，本地可能有数据，先删除本地（find 先找本地 再找数据库。。如果是从数据库找的也删除）
                    dbcontext.Entry<T>(obj).State = EntityState.Detached;
                }
                //从数据库查找最新的版本
                var aaa = dbcontext.Set<T>().AsNoTracking().FirstOrDefault(w => w.guid == Entity.guid);
                dbcontext.Entry<T>(aaa).State = EntityState.Deleted;
                return dbcontext.SaveChanges() > 0 ? SimpleBackValue.True : SimpleBackValue.False;
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException ex)
            {//修改的数据已被修改
             //1/正常并发报异常 2、查询未加【AsNoTracking】 导致修改报并发异常(修改的版本比数据库版本低)
             //return SimpleBackValue.DbUpdateConcurrencyException;
                return Delete(Entity);
            }
            return SimpleBackValue.False; 
        } 
        /// <summary>
        /// 批量修改数据（支持并发） 
        /// 如果数据在修改之前被修改或删除则提示并发冲突
        /// </summary>
        /// <param name="Entity"></param>
        /// <returns>批量成功或批量失败</returns>
        public SimpleBackValue Update(List<T> Entity)
        {
            try
            {
                //要么批量成功/要么批量失败
                int Count = 0;
                if (Entity != null)
                {
                    foreach (var items in Entity)
                    {
                        SetUpdate2(items);
                    }
                    Count = dbcontext.SaveChanges();
                }
                return Count >= Entity.Count? SimpleBackValue.True : SimpleBackValue.False;
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException ex)
            {//修改的数据已被修改
             //1/正常并发报异常 2、查询未加【AsNoTracking】 导致修改报并发异常(修改的版本比数据库版本低)
                return SimpleBackValue.DbUpdateConcurrencyException;
            }
            catch (DbEntityValidationException ex)
            {
                throw new Exception(GetDbEntityValidationException(ex));
            }
            return SimpleBackValue.False;
        }
        /// <summary>
        /// 修改数据（支持并发）
        /// 如果数据在修改之前被修改或删除则提示并发冲突
        /// </summary>
        /// <param name="Entity"></param>
        /// <returns></returns>
        public SimpleBackValue Update(T Entity)
        {
            if (Entity == null)
            {
                return SimpleBackValue.False;
            }
            try
            {
                SetUpdate2(Entity);
                return dbcontext.SaveChanges() > 0 ? SimpleBackValue.True : SimpleBackValue.False;
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException ex)
            {//修改的数据已被修改
             //1/正常并发报异常 2、查询未加【AsNoTracking】 导致修改报并发异常(修改的版本比数据库版本低)
                return SimpleBackValue.DbUpdateConcurrencyException;
            }
            catch (DbEntityValidationException ex)
            {
                throw new Exception(GetDbEntityValidationException(ex));
            }
            return SimpleBackValue.False;
        }
        private void SetUpdate2(T Entity)
        {
            //以下代码可用#####以上代码在使用AsNoTracking后不好使
            if (Entity == null)
            {
                return;
            }
            var oldversion = Entity.GetPropertyValue("OldRowVersion");
            T t = dbcontext.Set<T>().Local.FirstOrDefault(a => a.guid == Entity.guid);
            if (t != null)
            {//手动删除已存在的实体
             //_DbContextHandle.Set<T>().Attach(Entity);
                dbcontext.Entry<T>(t).State = EntityState.Detached;
            }
            dbcontext.Set<T>().Attach(Entity);
            var _editObj = dbcontext.Entry<T>(Entity);
            _editObj.State = EntityState.Modified;
            foreach (var item in _editObj.OriginalValues.PropertyNames)
            {
                if (item.ToLower() == "rowversion")
                {//版本号自动+1  
                    if (oldversion == null)
                    {//如果旧版本为空，则默认原始版本 
                        _editObj.CurrentValues["rowversion"] = Convert.ToInt64(_editObj.OriginalValues["rowversion"]) + 1;
                    }
                    else
                    {
                        _editObj.OriginalValues["rowversion"] = oldversion;
                        _editObj.CurrentValues["rowversion"] = Convert.ToInt64(oldversion) + 1;
                    }
                    break;
                }
            }
        }
        /// <summary>
        /// 修改前判断指定字段是否存在记录（不包含当前修改对象）
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="Entity"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public SimpleBackValue UpdateIfNotExists<TKey>(T Entity, Func<T, TKey> predicate)
        {
            if (Entity == null)
            {
                return SimpleBackValue.False;
            }
            try
            {
                var newValues = predicate.Invoke(Entity);
                Expression<Func<T, bool>> compare = arg => (predicate(arg)!=null&&predicate(arg).Equals(newValues)) && arg.guid!=Entity.guid;
                var compiled = compare.Compile();
                var existing = dbcontext.Set<T>().Any(compiled);
                if (existing)
                {
                    return SimpleBackValue.ExistsOfAdd;
                }
                else
                {
                    //dbcontext.Set<T>().Add(Entity);
                    SetUpdate2(Entity);
                }
                return dbcontext.SaveChanges() > 0 ? SimpleBackValue.True : SimpleBackValue.False;
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException ex)
            {//修改的数据已被修改 
                return SimpleBackValue.DbUpdateConcurrencyException;
            }
            catch (DbEntityValidationException ex)
            {
                throw new Exception(GetDbEntityValidationException(ex));
            }
        } 
        /*********以下查询默认不缓存至本地追踪数据（提高性能）*********/
        public List<T> GetAllQuery(Expression<Func<T, bool>> WhereLambda = null)
        {
            return WhereLambda != null ? dbcontext.Set<T>().AsNoTracking().Where(WhereLambda).ToList() ??
                 null : dbcontext.Set<T>().AsNoTracking().ToList() ?? null;
        }
        public List<T> GetAllQueryOrderby<TKey>(Expression<Func<T, TKey>> OrderBy, Expression<Func<T, bool>> WhereLambda = null, bool IsAsc = true)
        {
            IQueryable<T> QueryList = IsAsc == true ? dbcontext.Set<T>().AsNoTracking().OrderBy(OrderBy) : dbcontext.Set<T>().AsNoTracking().OrderByDescending(OrderBy);
            if (WhereLambda != null)
            {
                QueryList = QueryList.Where(WhereLambda);
            }
            return QueryList.AsNoTracking().ToList() ?? null;
        }
        public bool GetAny(Expression<Func<T, bool>> WhereLambda = null)
        {
            return WhereLambda != null ? dbcontext.Set<T>().AsNoTracking().Where(WhereLambda).Any() : dbcontext.Set<T>().AsNoTracking().Any();
        }
        public int GetCount(Expression<Func<T, bool>> WhereLambda = null)
        {
            return WhereLambda != null ? dbcontext.Set<T>().AsNoTracking().Where(WhereLambda).Count() : dbcontext.Set<T>().AsNoTracking().Count();
        }
        public T GetModel(System.Guid guid)
        {
            return dbcontext.Set<T>().Where(w => w.guid == guid).AsNoTracking().FirstOrDefault() ?? null;
        }
        public T GetFirstDefault(Expression<Func<T, bool>> WhereLambda = null)
        {
            return WhereLambda != null ? dbcontext.Set<T>().Where(WhereLambda).AsNoTracking().FirstOrDefault()
              ?? null : dbcontext.Set<T>().AsNoTracking().FirstOrDefault() ?? null;
        }

        /// <summary>
        /// 分页查询每页的最大显示条数，最大每页限制200条记录
        /// </summary>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        public int MaxPageSize(int pagesize)
        {
            if (pagesize > 200)
            {
                pagesize = 200;
            }
            else if (pagesize <= 0)
            {
                pagesize = 1;
            }
            return pagesize;
        }

        public List<T> Pagination<TKey>(int PageIndex, int PageSize, out int TotalCount, Expression<Func<T, TKey>> OrderBy, Expression<Func<T, bool>> WhereLambda = null, bool IsAsc = true)
        {
            PageSize = MaxPageSize(PageSize);
            IQueryable<T> QueryList = IsAsc == true ? dbcontext.Set<T>().AsNoTracking().OrderBy(OrderBy) : dbcontext.Set<T>().AsNoTracking().OrderByDescending(OrderBy);
            if (WhereLambda != null)
            {
                QueryList = QueryList.Where(WhereLambda).AsNoTracking();
            }
            TotalCount =QueryList.Count();
            //判断总页数小于当前页，默认显示最后一页
            int mod = TotalCount % PageSize;
            int _totalPage = TotalCount / PageSize;
            if (mod > 0)
            {
                _totalPage = _totalPage + 1;
            }
            if (PageIndex > _totalPage)
            {
                PageIndex = _totalPage;
            }
            if (PageIndex <= 0)
            {
                PageIndex = 1;
            }
            return QueryList.Skip(PageSize * (PageIndex - 1)).Take(PageSize).ToList() ?? null;
        }
        /// <summary>
        /// 执行sql语句，返回影响的行数
        /// </summary>
        /// <param name="sqlcommand"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public int ExecuteSQLCommand(string sqlcommand, DbParameter[] parameters)
        {
            return dbcontext.Database.ExecuteSqlCommand(sqlcommand, parameters);
        }
        public void Dispose()
        {
            try
            {
                dbcontext.Dispose();
            }
            catch (Exception ex)
            {
            }
        }
    }
}
