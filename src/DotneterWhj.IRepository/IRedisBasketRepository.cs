using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotneterWhj.IRepository
{
    public interface IRedisBasketRepository
    {
        string GetValue(string key);

        T Get<T>(string key);

        void Set(string key, object value, TimeSpan timeSpan);
    }
}
