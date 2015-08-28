using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YisinTick
{
    class Stations
    {
        static List<Station> list = new List<Station>();

        private static object obj = new object();

        public static List<Station> List
        {
            get
            {
                if (list.Count <= 0)
                {
                    lock (obj)
                    {
                        if (list.Count <= 0)
                        {
                            list = _12306Class.GetStations();
                        }
                    }
                }
                return list;
            }
        }
    }

    /// <summary>
    /// 站点信息
    /// </summary>
    class Station
    {
        /// <summary>
        /// 站点名
        /// </summary>
        public string Name;

        /// <summary>
        /// 
        /// </summary>
        public string Code;

        /// <summary>
        /// 首字母
        /// </summary>
        public string FirstLetter;

        /// <summary>
        /// 全拼
        /// </summary>
        public string Pinyin;

        /// <summary>
        /// 简写
        /// </summary>
        public string Shorthand;

        /// <summary>
        /// 排序
        /// </summary>
        public string Order;
    }
}
