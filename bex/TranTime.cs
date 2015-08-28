using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YisinTick
{
    class TranTime
    {
        static List<string> List = new List<string>();

        public static List<string> GetTimes()
        {
            string[] times = new string[List.Count];

            List.CopyTo(times);

            return times.ToList();
        }

        public static void Clear()
        {
            List.Clear();

            if (TimeChanged != null)
            {
                TimeChanged(null, List);
            }
        }


        public static void AddTime(string time)
        {
            if (List.Count(fun => fun == time) <= 0)
            {
                List.Add(time);

                if (TimeChanged != null)
                {
                    TimeChanged(null, List);
                }
            }
        }

        /// <summary>
        /// 已重载.计算两个日期的时间间隔,返回的是时间间隔的日期差的绝对值.
        /// </summary>
        /// <param name="DateTime1">第一个日期和时间</param>
        /// <param name="DateTime2">第二个日期和时间</param>
        /// <returns></returns>
        public static int DateDiff(DateTime DateTime1, DateTime DateTime2)
        {
            int dateDiff = 0;
            try
            {
                TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
                TimeSpan ts2 = new TimeSpan(DateTime2.Ticks);
                TimeSpan ts = ts1.Subtract(ts2).Duration();
                dateDiff = (int) ts.TotalMilliseconds;
            }
            catch
            {
            }
            return dateDiff;
        }


        public static event EventHandler<List<string>> TimeChanged;
    }

}
