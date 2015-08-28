using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MS.Internal.Xml.XPath;

namespace YisinTick
{
    class Trains
    {
        static List<Train> list = new List<Train>();

        public static List<Train> GetTrains()
        {
                List<Train> _list = new List<Train>();

                _list.AddRange(list);

                return list;
            
        }
        
        public static void AddTrains(List<Train> trains)
        {
            /*var v = trains.Where(fun => list.SingleOrDefault(m => m.Id == fun.Id) == null);
            if (v.Any())
            {
                list.AddRange(v);*/
                if (TrainsChanged != null)
                {
                    TrainsChanged(null, trains);
                }
            //}
        }

        public static event EventHandler<List<Train>> TrainsChanged;
    }

    public class Train
    {
        public string Id;
        public string StartStation;
        public string EndStation;
        public string StartTime;
        public string EndTime;
        public string TrainValue;
        public string StationTrainCode ;
        public string TrainNo;

        public string from_station_telecode;
        public string end_station_telecode;


        /// <summary>
        /// 商务座
        /// </summary>
        public string SWZ { get; set; }

        /// <summary>
        /// 特等座
        /// </summary>
        public string TZ { get; set; }

        /// <summary>
        /// 一等座
        /// </summary>
        public string ZY { get; set; }

        /// <summary>
        /// 二等座
        /// </summary>
        public string ZE { get; set; }

        /// <summary>
        /// 高级软卧
        /// </summary>
        public string GR { get; set; }
        
        /// <summary>
        /// 软卧
        /// </summary>
        public string RW { get; set; }

        /// <summary>
        /// 硬卧
        /// </summary>
        public string YW { get; set; }

        /// <summary>
        /// 软座
        /// </summary>
        public string RZ { get; set; }

        /// <summary>
        /// 硬座
        /// </summary>
        public string YZ { get; set; }

        /// <summary>
        /// 无座
        /// </summary>
        public string WZ { get; set; }

        /// <summary>
        /// 其他
        /// </summary>
        public string QT { get; set; }
        public override string ToString()
        {
            return string.Format("{0} {1}({3})->{2}({4}),一等座({5}),二等座({6}),软卧({7}),硬卧({8}),硬座({9}),无座({10})", 
                TrainValue, StartStation, EndStation, StartTime, EndTime, ZY, ZE, RW, YW, YZ, WZ);
        }
    }
}
