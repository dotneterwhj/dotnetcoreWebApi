using DotneterWhj.Core.CommonModel;
using DotneterWhj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DotneterWhj.IServices
{
    public interface IBaseService<TEntity>
        where TEntity : BaseEntity, new()
    {
        /// <summary>
        /// 新增数据
        /// </summary>
        /// <param name="tDto">待新增的实体</param>
        /// <returns></returns>
        Task<TEntity> AddAsync(TEntity tDto);

        /// <summary>
        /// 批量新增数据
        /// </summary>
        /// <param name="lisTDto">待新增的实体集合</param>
        /// <returns></returns>
        Task<int> AddAsync(List<TEntity> lisTDto);

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<int> DeleteByIdAsync(object id);

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="tDto"></param>
        /// <returns></returns>
        Task<int> DeleteAsync(TEntity tDto);

        /// <summary>
        /// 批量删除数据
        /// </summary>
        /// <param name="tDtos"></param>
        /// <returns></returns>
        Task<int> DeleteAsync(List<TEntity> tDtos);

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="tDto"></param>
        /// <returns></returns>
        Task<int> UpdateAsync(TEntity tDto);

        /// <summary>
        /// 批量更新数据
        /// </summary>
        /// <param name="tDtos"></param>
        /// <remarks>需要开启事务操作,否则可能会造成数据不一致</remarks>
        /// <returns></returns>
        Task<int> UpdateAsync(TEntity[] tDtos);

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="tDto"></param>
        /// <param name="fileds">要更新的字段</param>
        /// <returns></returns>
        Task<int> UpdateWithFiledsAsync(TEntity tDto, params string[] fileds);

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="tDto"></param>
        /// <param name="filedsNotUpdate">忽略的字段</param>
        /// <returns></returns>
        Task<int> UpdateAsync(TEntity tDto, params string[] filedsNotUpdate);

        /// <summary>
        /// 是否存在数据
        /// </summary>
        /// <param name="objId"></param>
        /// <returns></returns>
        Task<bool> IsExistAsync(object objId);

        /// <summary>
        /// 通过主键查询数据
        /// </summary>
        /// <param name="objId">主键</param>
        /// <returns></returns>
        Task<TEntity> QueryByKeyAsync(object objId);

        /// <summary>
        /// 通过组合主键查询数据
        /// </summary>
        /// <param name="objId"></param>
        /// <returns></returns>
        Task<TEntity> QueryByUnionKeyAsync(params object[] objId);

        /// <summary>
        /// 通过主键查询多条数据
        /// </summary>
        /// <param name="lstIds">主键数组</param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> QueryByIDsAsync(object[] lstIds);

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> QueryAsync();

        /// <summary>
        /// 查询数据(按需查询)
        /// </summary>
        /// <param name="fields">需要查询的字段,必须在数据库中存在</param>
        /// <exception cref="ArgumentNullException">fields</exception>
        /// <exception cref="ParamterNotExistException">字段不存在异常</exception>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> QueryAsync(string[] fields);

        /// <summary>
        /// 按条件查询数据
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> QueryAsync(Expression<Func<TEntity, bool>> whereExpression);

        /// <summary>
        /// 按条件查询数据
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="fields">查询的字段</param>
        /// <exception cref="ArgumentNullException">fields</exception>
        /// <exception cref="ParamterNotExistException">字段不存在异常</exception>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> QueryAsync(Expression<Func<TEntity, bool>> whereExpression, string[] fields);

        /// <summary>
        /// 按条件查询数据
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="orderByFileds">排序字段</param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> QueryAsync(Expression<Func<TEntity, bool>> whereExpression, params OrderFiledModel[] orderByFileds);

        /// <summary>
        /// 按条件查询数据
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="fields">查询的字段</param>
        /// <param name="orderByFileds">排序字段</param>
        /// <exception cref="ArgumentNullException">fields</exception>
        /// <exception cref="ParamterNotExistException">字段不存在异常</exception>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> QueryAsync(Expression<Func<TEntity, bool>> whereExpression, string[] fields, params OrderFiledModel[] orderByFileds);

        
        /// <summary>
        /// 按条件查询数据
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="intTop">取前几项</param>
        /// <param name="orderByFileds">排序字段</param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> QueryAsync(Expression<Func<TEntity, bool>> whereExpression, int intTop, params OrderFiledModel[] orderByFileds);

        /// <summary>
        /// 按条件查询数据
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="intTop">取前几项</param>
        /// <param name="fields">查询的字段</param>
        /// <param name="orderByFileds">排序字段</param>
        /// <exception cref="ArgumentNullException">fields</exception>
        /// <exception cref="ParamterNotExistException">字段不存在异常</exception>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> QueryAsync(Expression<Func<TEntity, bool>> whereExpression, int intTop, string[] fields, params OrderFiledModel[] orderByFileds);

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="intPageIndex">第几页</param>
        /// <param name="intPageSize"></param>
        /// <param name="orderByFileds">排序</param>
        /// <exception cref="ParamterNotExistException">字段不存在异常</exception>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> QueryPageAsync(Expression<Func<TEntity, bool>> whereExpression, int intPageIndex = 1, int intPageSize = 20, params OrderFiledModel[] orderByFileds);


        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="fields">查询的字段</param>
        /// <param name="intPageIndex">第几页</param>
        /// <param name="intPageSize"></param>
        /// <param name="orderByFileds">排序</param>
        /// <exception cref="ArgumentNullException">fields</exception>
        /// <exception cref="ParamterNotExistException">字段不存在异常</exception>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> QueryPageAsync(Expression<Func<TEntity, bool>> whereExpression, string[] fields, int intPageIndex = 1, int intPageSize = 20, params OrderFiledModel[] orderByFileds);


        Task<List<TResult>> QueryMuchAsync<T, T2, T3, TResult>(
            Expression<Func<T, T2, T3, object[]>> joinExpression,
            Expression<Func<T, T2, T3, TResult>> selectExpression,
            Expression<Func<T, T2, T3, bool>> whereLambda = null) where T : class, new();
    }
}
