using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YisinTick
{
    /// <summary>
    /// 验证码特征库单个实体
    /// </summary>
    class VerifyEntity
    {
        /// <summary>
        /// 字符码
        /// </summary>
        public String Code {get; set; } 

        /// <summary>
        /// 图片路径
        /// </summary>
        public String ImgPath { get; set; } 

        /// <summary>
        /// 数据
        /// </summary>
        public String Data { get; set; }

        /// <summary>
        /// 特征库ID
        /// </summary>
        public String LibId { get; set; }

        /// <summary>
        /// 特征库Name
        /// </summary>
        public String LibName { get; set; }

    }
}
