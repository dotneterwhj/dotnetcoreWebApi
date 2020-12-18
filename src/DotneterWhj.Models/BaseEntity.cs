using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotneterWhj.Models
{
    public class BaseEntity
    {
        /// <summary>
        /// 主键id
        /// </summary>
        [Key]
        public long Id { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 上次修改人
        /// </summary>
        public string LastModifier { get; set; }

        /// <summary>
        /// 上次修改时间
        /// </summary>
        public DateTime LastModifytime { get; set; } = DateTime.Now;

        /// <summary>
        /// 状态
        /// </summary>
        public byte State { get; set; }
    }
}
