﻿using Castle.DynamicProxy;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DotneterWhj.WebApi
{
    public class LogAop : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            // 事前处理: 在服务方法执行之前,做相应的逻辑处理
            var dataIntercept = "" +
                $"【当前执行方法】：{ invocation.Method.Name} \r\n" +
                $"【携带的参数有】： {string.Join(", ", invocation.Arguments.Select(a => (a ?? "").ToString()).ToArray())} \r\n";

            // 执行当前访问的服务方法,(注意:如果下边还有其他的AOP拦截器的话,会跳转到其他的AOP里)
            invocation.Proceed();

            // 事后处理: 在service被执行了以后,做相应的处理,这里是输出到日志文件
            dataIntercept += ($"【执行完成结果】：{invocation.ReturnValue}");

            // 输出到日志文件
            Parallel.For(0, 1, e =>
            {
                Console.WriteLine("AOPLog:{0}", new string[] { dataIntercept });
            });
        }
    }
}