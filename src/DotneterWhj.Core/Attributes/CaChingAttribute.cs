using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotneterWhj.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CaChingAttribute : Attribute
    {
        public int AbsoluteExpiration { get; set; } = 30;
    }
}
