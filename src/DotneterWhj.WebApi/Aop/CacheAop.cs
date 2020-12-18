using Castle.DynamicProxy;
using DotneterWhj.Core.Attributes;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotneterWhj.WebApi
{
    public class CacheAop : AopBase
    {
        private readonly IMemoryCache _cache;

        public CacheAop(IMemoryCache cache)
        {
            this._cache = cache;
        }

        public override void Intercept(IInvocation invocation)
        { 
            // 获取自定义缓存键
            var cacheKey = CustomCacheKey(invocation);

            // 根据key获取相应的缓存值
            var cacheValue = _cache.Get(cacheKey);

            if (cacheValue != null)
            {
                // 将当前获取到的缓存值，赋值给当前执行方法
                invocation.ReturnValue = cacheValue;
                return;
            }

            // 去执行当前的方法
            invocation.Proceed();

            // 存入缓存
            if (!string.IsNullOrWhiteSpace(cacheKey))
            {
                _cache.Set(cacheKey, invocation.ReturnValue);
            }
        }
    }
}
