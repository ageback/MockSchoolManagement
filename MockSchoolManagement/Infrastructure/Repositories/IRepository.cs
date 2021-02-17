using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MockSchoolManagement.Infrastructure.Repositories
{
    /// <summary>
    /// 此接口是所有仓储的约定，用于标识它们。
    /// <typeparamref name="TEntity">当前传入仓储的实体类型</typeparamref>
    /// <typeparamref name="TPrimaryKey">当前传入仓储的主键类型</typeparamref>
    /// </summary>
    public interface IRepository<TEntity, TPrimaryKey> where TEntity : class
    {
        #region 查询
        IQueryable<TEntity> GetAll();
        List<TEntity> GetAllList();
        Task<List<TEntity>> GetAllListAsync();
        List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate);
        Task<List<TEntity>> GetAllListAsync(Expression<Func<TEntity, bool>> predicate);
        TEntity Single(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate);
        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
        #endregion

        #region Insert
        TEntity Insert(TEntity entity);
        Task<TEntity> InsertAsync(TEntity entity);
        #endregion

        #region Update
        TEntity Update(TEntity entity);
        Task<TEntity> UpdateAsync(TEntity entity);
        #endregion

        #region Delete
        void Delete(TEntity entity);
        Task DeleteAsync(TEntity entity);
        void Delete(Expression<Func<TEntity, bool>> predicate);
        Task DeleteAsync(Expression<Func<TEntity, bool>> predicate);
        #endregion

        #region 总和计算
        int Count();
        Task<int> CountAsync();
        int Count(Expression<Func<TEntity, bool>> predicate);
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);
        long LongCount();
        Task<long> LongCountAsync();
        long LongCount(Expression<Func<TEntity, bool>> predicate);
        Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate);
        #endregion
    }
}
