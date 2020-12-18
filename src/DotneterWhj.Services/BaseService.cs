using DotneterWhj.Core.CommonModel;
using DotneterWhj.IRepository;
using DotneterWhj.IServices;
using DotneterWhj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DotneterWhj.Services
{
    public class BaseService<TEntity> : IBaseService<TEntity>
        where TEntity : BaseEntity, new()
    {
        private readonly IBaseRepository<TEntity> _baseRepository;

        public BaseService(IBaseRepository<TEntity> baseRepository)
        {
            this._baseRepository = baseRepository;
        }

        public async Task<TEntity> AddAsync(TEntity tDto)
        {
            return await _baseRepository.AddAsync(tDto);
        }

        public async Task<int> AddAsync(List<TEntity> lisTDto)
        {
            return await _baseRepository.AddAsync(lisTDto);
        }

        public async Task<int> DeleteAsync(TEntity tDto)
        {
            return await _baseRepository.DeleteAsync(tDto);
        }

        public async Task<int> DeleteAsync(List<TEntity> tDtos)
        {
            return await _baseRepository.DeleteAsync(tDtos);
        }

        public async Task<int> DeleteByIdAsync(object id)
        {
            return await _baseRepository.DeleteByIdAsync(id);
        }

        public async Task<bool> IsExistAsync(object objId)
        {
            return await _baseRepository.IsExistAsync(objId);
        }

        public async Task<IEnumerable<TEntity>> QueryAsync()
        {
            return await _baseRepository.QueryAsync();
        }

        public async Task<IEnumerable<TEntity>> QueryAsync(string[] fields)
        {
            // todo
            return await _baseRepository.QueryAsync();
        }

        public async Task<IEnumerable<TEntity>> QueryAsync(Expression<Func<TEntity, bool>> whereExpression)
        {
            return await _baseRepository.QueryAsync(whereExpression);
        }

        public async Task<IEnumerable<TEntity>> QueryAsync(Expression<Func<TEntity, bool>> whereExpression, string[] fields)
        {
            // todo
            return await _baseRepository.QueryAsync(whereExpression);
        }

        private string[] OrderFiledModelToOrderFileds(OrderFiledModel[] orderByFileds)
        {
            var orderFileds = new List<string>();

            if (orderByFileds != null)
            {
                foreach (var orderfiled in orderByFileds)
                {
                    var order = orderfiled.IsAsc ? "asc" : "desc";
                    orderFileds.Add($"{orderfiled.PropertyName} {order}");
                }
            }

            return orderFileds.ToArray();
        }

        public async Task<IEnumerable<TEntity>> QueryAsync(Expression<Func<TEntity, bool>> whereExpression, params OrderFiledModel[] orderByFileds)
        {
            var orderfields = OrderFiledModelToOrderFileds(orderByFileds);

            return await _baseRepository.QueryAsync(whereExpression, orderfields);
        }

        public async Task<IEnumerable<TEntity>> QueryAsync(Expression<Func<TEntity, bool>> whereExpression, string[] fields, params OrderFiledModel[] orderByFileds)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TEntity>> QueryAsync(Expression<Func<TEntity, bool>> whereExpression, int intTop, params OrderFiledModel[] orderByFileds)
        {
            var orderfields = OrderFiledModelToOrderFileds(orderByFileds);

            return await _baseRepository.QueryAsync(whereExpression, intTop, orderfields);
        }

        public async Task<IEnumerable<TEntity>> QueryAsync(Expression<Func<TEntity, bool>> whereExpression, int intTop, string[] fields, params OrderFiledModel[] orderByFileds)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TEntity>> QueryByIDsAsync(object[] lstIds)
        {
            return await _baseRepository.QueryByIDsAsync(lstIds);
        }

        public async Task<TEntity> QueryByKeyAsync(object objId)
        {
            return await _baseRepository.QueryByKeyAsync(objId);
        }

        public async Task<TEntity> QueryByUnionKeyAsync(params object[] objId)
        {
            return await _baseRepository.QueryByKeyAsync(objId);
        }

        public async Task<List<TResult>> QueryMuchAsync<T, T2, T3, TResult>(Expression<Func<T, T2, T3, object[]>> joinExpression, Expression<Func<T, T2, T3, TResult>> selectExpression, Expression<Func<T, T2, T3, bool>> whereLambda = null) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TEntity>> QueryPageAsync(Expression<Func<TEntity, bool>> whereExpression, int intPageIndex = 1, int intPageSize = 20, params OrderFiledModel[] orderByFileds)
        {
            var orderfields = OrderFiledModelToOrderFileds(orderByFileds);

            return await _baseRepository.QueryPageAsync(whereExpression, intPageIndex, intPageSize, orderfields);
        }

        public async Task<IEnumerable<TEntity>> QueryPageAsync(Expression<Func<TEntity, bool>> whereExpression, string[] fields, int intPageIndex = 1, int intPageSize = 20, params OrderFiledModel[] orderByFileds)
        {
            throw new NotImplementedException();
        }

        public async Task<int> UpdateAsync(TEntity tDto)
        {
            return await _baseRepository.UpdateAsync(tDto);
        }

        public async Task<int> UpdateAsync(TEntity[] tDtos)
        {
            return await _baseRepository.UpdateAsync(tDtos.ToList());
        }

        public async Task<int> UpdateAsync(TEntity tDto, params string[] filedsNotUpdate)
        {
            return await _baseRepository.UpdateAsync(tDto, filedsNotUpdate);
        }

        public async Task<int> UpdateWithFiledsAsync(TEntity tDto, params string[] fileds)
        {
            return await _baseRepository.UpdateWithFiledsNotAsync(tDto, fileds);
        }
    }
}
