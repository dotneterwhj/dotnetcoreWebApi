using DotneterWhj.IRepository;
using DotneterWhj.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DotneterWhj.Repository
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity>
        where TEntity : BaseEntity, new()
    {
        protected DbContext _dbContext;

        private async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public BaseRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> IsExist(object objId)
        {
            var entity = await _dbContext.Set<TEntity>().FindAsync(objId);
            if (entity != null)
            {
                _dbContext.Entry(entity).State = EntityState.Detached;
            }
            return entity != null;
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            _dbContext.Set<TEntity>().Add(entity);

            await this.SaveChangesAsync();

            return entity;
        }

        public async Task<int> AddAsync(List<TEntity> listEntity)
        {
            _dbContext.Set<TEntity>().AddRange(listEntity);

            return await this.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(TEntity entity)
        {
            var removeModel = _dbContext.Set<TEntity>().Remove(entity);

            return await this.SaveChangesAsync();
        }

        public async Task<int> DeleteByIdAsync(object id)
        {
            var entity = _dbContext.Set<TEntity>().Find(id);

            if (entity != null)
            {
                _dbContext.Set<TEntity>().Remove(entity);

                return await this.SaveChangesAsync();
            }

            return 0;
        }

        public async Task<int> DeleteByUnionKeyAsync(params object[] keys)
        {
            var entity = _dbContext.Set<TEntity>().Find(keys);

            if (entity != null)
            {
                _dbContext.Set<TEntity>().Remove(entity);

                return await this.SaveChangesAsync();
            }

            return 0;
        }

        public async Task<int> DeleteAsync(List<TEntity> entities)
        {
            _dbContext.Set<TEntity>().RemoveRange(entities);

            return await this.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(TEntity tEntity)
        {
            return await UpdateAsync(tEntity, null);
        }


        public async Task<int> UpdateAsync(List<TEntity> tEntities, params string[] fileds)
        {
            var updateCount = 0;

            foreach (var entity in tEntities)
            {
                updateCount += await UpdateAsync(entity, fileds);
            }

            return updateCount;
        }

        public async Task<int> UpdateAsync(TEntity tEntity, params string[] fileds)
        {
            var entity = _dbContext.Set<TEntity>().Attach(tEntity);

            if (fileds != null)
            {
                _dbContext.Entry(entity).State = EntityState.Unchanged;

                // 遍历修改的字段并将其设置为更改;
                foreach (string filedName in fileds)
                {
                    _dbContext.Entry(entity).Property(filedName).IsModified = true;
                }
            }

            // 寻找主键
            IEnumerable<PropertyInfo> pkProps = typeof(TEntity).GetProperties()
                .Where(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Length > 0);

            // 排除主键
            foreach (var pkProp in pkProps)
            {
                _dbContext.Entry(entity).Property(nameof(pkProp.Name)).IsModified = false;
            }

            // 排除不允许修改的字段
            _dbContext.Entry(entity).Property(nameof(BaseEntity.Creator)).IsModified = false;
            _dbContext.Entry(entity).Property(nameof(BaseEntity.CreateTime)).IsModified = false;

            entity.Entity.LastModifytime = DateTime.Now;

            if (string.IsNullOrEmpty(entity.Entity.LastModifier))
            {
                // todo 
                entity.Entity.LastModifier = "";
            }

            // 必须修改的字段
            _dbContext.Entry(entity).Property(nameof(BaseEntity.LastModifier)).IsModified = true;
            _dbContext.Entry(entity).Property(nameof(BaseEntity.LastModifytime)).IsModified = true;

            return await this.SaveChangesAsync();
        }


        public async Task<int> UpdateWithFiledsNotAsync(TEntity tEntity, params string[] filedsNotUpdate)
        {
            var entity = _dbContext.Set<TEntity>().Attach(tEntity);

            _dbContext.Entry(entity).State = EntityState.Modified;

            if (filedsNotUpdate != null)
            {
                // 遍历修改
                foreach (string filedName in filedsNotUpdate)
                {
                    _dbContext.Entry(entity).Property(filedName).IsModified = false;
                }
            }

            // 寻找主键
            IEnumerable<PropertyInfo> pkProps = typeof(TEntity).GetProperties().Where(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Length > 0);

            // 排除主键
            foreach (var pkProp in pkProps)
            {
                _dbContext.Entry(entity).Property(nameof(pkProp.Name)).IsModified = false;
            }

            // 排除不允许修改的字段
            _dbContext.Entry(entity).Property(nameof(BaseEntity.Creator)).IsModified = false;
            _dbContext.Entry(entity).Property(nameof(BaseEntity.CreateTime)).IsModified = false;

            entity.Entity.LastModifytime = DateTime.Now;

            if (string.IsNullOrEmpty(entity.Entity.LastModifier))
            {
                // todo 
                entity.Entity.LastModifier = "";
            }

            // 必须修改的字段
            _dbContext.Entry(entity).Property(nameof(BaseEntity.LastModifytime)).IsModified = true;
            _dbContext.Entry(entity).Property(nameof(BaseEntity.LastModifier)).IsModified = true;

            return await this.SaveChangesAsync();
        }

        public async Task<IQueryable<TEntity>> QueryAsync()
        {
            return await Task.FromResult(_dbContext.Set<TEntity>().AsNoTracking());
        }

        public async Task<IQueryable<TEntity>> QueryAsync(Expression<Func<TEntity, bool>> whereExpression)
        {
            return await Task.FromResult(_dbContext.Set<TEntity>().AsNoTracking().Where(whereExpression));
        }

        public async Task<IQueryable<TEntity>> QueryAsync(Expression<Func<TEntity, bool>> whereExpression, params string[] orderByFileds)
        {
            var query = await QueryAsync(whereExpression);

            // 创建表达式变量参数
            var parameter = Expression.Parameter(typeof(TEntity));

            if (orderByFileds != null && orderByFileds.Length > 0)
            {
                for (int i = 0; i < orderByFileds.Length; i++)
                {
                    var orderfiled = orderByFileds[i].Split(' ');

                    // 根据属性名获取属性
                    var property = typeof(TEntity).GetProperty(orderfiled[0],
                        BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                    // 创建一个访问属性的表达式
                    var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                    var orderByExp = Expression.Lambda(propertyAccess, parameter);

                    string orderName = string.Empty;
                    if (i > 0)
                    {
                        orderName = string.Equals(orderfiled[1], "asc", StringComparison.InvariantCultureIgnoreCase) ? "ThenByDescending" : "ThenBy";
                    }
                    else
                    {
                        orderName = string.Equals(orderfiled[1], "desc", StringComparison.InvariantCultureIgnoreCase) ? "OrderByDescending" : "OrderBy";
                    }

                    MethodCallExpression resultExp = Expression.Call(typeof(Queryable), orderName, new Type[] { typeof(TEntity), property.PropertyType }, query.Expression, Expression.Quote(orderByExp));

                    query = query.Provider.CreateQuery<TEntity>(resultExp);
                }
            }
            else
            {
                var name = typeof(TEntity).GetProperties()[0].Name;
                query = query.OrderBy(d => name);
            }

            return query;
        }

        public async Task<IQueryable<TEntity>> QueryAsync(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, object>> orderByExpression, bool isAsc = true)
        {
            IQueryable<TEntity> entities = null;

            if (isAsc)
            {
                entities = _dbContext.Set<TEntity>().AsNoTracking().Where(whereExpression).OrderBy(orderByExpression);
            }
            else
            {
                entities = _dbContext.Set<TEntity>().AsNoTracking().Where(whereExpression).OrderByDescending(orderByExpression);
            }

            return await Task.FromResult(entities);
        }

        public async Task<IQueryable<TEntity>> QueryAsync(Expression<Func<TEntity, bool>> whereExpression, int intTop, params string[] orderByFileds)
        {
            var entities = await QueryAsync(whereExpression, orderByFileds);

            return entities.Take(intTop);
        }

        public async Task<IQueryable<TEntity>> QueryPageAsync(Expression<Func<TEntity, bool>> whereExpression, int intPageIndex, int intPageSize, params string[] orderByFileds)
        {
            intPageIndex = intPageIndex < 1 ? 1 : intPageIndex;

            intPageSize = intPageSize <= 0 ? (await QueryAsync(whereExpression)).Count() : intPageSize;

            var entities = await QueryAsync(whereExpression, orderByFileds);

            return entities.Skip((intPageIndex - 1) * intPageSize).Take(intPageSize);
        }

        public async Task<TEntity> QueryByIdAsync(object objId)
        {
            var entity = await _dbContext.Set<TEntity>().FindAsync(objId);

            if (entity != null)
            {
                _dbContext.Entry(entity).State = EntityState.Detached;
            }

            return entity;
        }

        public async Task<TEntity> QueryByKeyAsync(params object[] objId)
        {
            return await _dbContext.Set<TEntity>().FindAsync(objId);
        }

        public async Task<List<TEntity>> QueryByIDsAsync(object[] lstIds)
        {
            List<TEntity> entities = new List<TEntity>();

            foreach (var id in lstIds)
            {
                entities.Add(await _dbContext.Set<TEntity>().FindAsync(id));
            }

            return entities;
        }

        public async Task<List<TResult>> QueryMuchAsync<T, T2, T3, TResult>(Expression<Func<T, T2, T3, object[]>> joinExpression, Expression<Func<T, T2, T3, TResult>> selectExpression, Expression<Func<T, T2, T3, bool>> whereLambda = null) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public async Task<IQueryable<TEntity>> QuerySqlAsync(string strSql, SqlParameter[] parameters = null)
        {
            var entities = _dbContext.Set<TEntity>().FromSqlRaw(strSql, parameters);

            return await Task.FromResult(entities);
        }

        public async Task<int> QueryCountSqlAsync(string strSql, SqlParameter[] parameters = null)
        {
            return await _dbContext.Database.ExecuteSqlRawAsync(strSql, parameters);
        }

        public async Task<bool> IsExistAsync(object objId)
        {
            var entity = await _dbContext.Set<TEntity>().FindAsync(objId);

            return entity != null;
        }


    }
}
