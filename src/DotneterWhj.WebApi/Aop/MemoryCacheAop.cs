using Castle.DynamicProxy;
using DotneterWhj.Core.Attributes;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotneterWhj.WebApi
{
    public class MemoryCacheAop : AopBase
    {
        private readonly IMemoryCache _cache;

        public MemoryCacheAop(IMemoryCache cache)
        {
            this._cache = cache;
        }

        public override void Intercept(IInvocation invocation)
        {
            var method = invocation.MethodInvocationTarget ?? invocation.Method;

            // 对当前方法的特性验证
            var qCachingAttribute = method.GetCustomAttributes(true).FirstOrDefault(x => x.GetType() == typeof(CachingAttribute)) as CachingAttribute;
           
            // 只有那些指定的才可以被缓存，需要验证
            if (qCachingAttribute != null)
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
            else
            {
                invocation.Proceed();
            }
        }
    }
}
