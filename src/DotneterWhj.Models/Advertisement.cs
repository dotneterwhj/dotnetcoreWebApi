using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotneterWhj.Models
{
    public class Advertisement : BaseEntity
    {
        /// <summary>
        /// 广告图片
        /// </summary>
        public string ImgUrl { get; set; }

        /// <summary>
        /// 广告标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 广告链接
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

    }
}
