using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YisinTick
{
    public class Seat
    {
        static List<SeatsType> SelectSeats = new List<SeatsType>();

        public static List<SeatsType> GetSeats()
        {
            List<SeatsType> list = new List<SeatsType>();

            list.AddRange(SelectSeats);

            return list;
        }

        public static event EventHandler<List<SeatsType>> SelectSeatsChange;

        public static void ClearSelectSeats()
        {
            SelectSeats.Clear();

            if (SelectSeatsChange != null)
            {
                SelectSeatsChange(null, SelectSeats);
            }
        }

        public static void AddSelectSeats(SeatsType type)
        {
            if (!SelectSeats.Contains(type))
            {
                SelectSeats.Add(type);

                if (SelectSeatsChange != null)
                {
                    SelectSeatsChange(null, SelectSeats);
                }
            }
        }

        private Seat() { }

        public static Seat Create(SeatsType type, string count)
        {
            var seat = new Seat();
            seat.Name = type.ToString();
            seat.Count = count;
            switch (type)
            {
                case SeatsType.商务座:
                    seat.Id = "9";
                    break;
                case SeatsType.特等座:
                    seat.Id = "P";
                    break;
                case SeatsType.二等座:
                    seat.Id = "M";
                    break;
                case SeatsType.一等座:
                    seat.Id = "O";
                    break;
                case SeatsType.硬卧:
                    seat.Id = "3";
                    break;
                case SeatsType.硬座:
                    seat.Id = "1";
                    break;
                case SeatsType.软卧:
                    seat.Id = "4";
                    break;
                case SeatsType.软座:
                    seat.Id = "2";
                    break;
                case SeatsType.无座:
                    seat.Id = "1";
                    break;
            }

            return seat;
        }

        public string Name;
        public string Id;
        public string Count;
    }

    public class TrainSeat
    {
        public List<Seat> List = new List<Seat>();

        public string station_train_code;
        public string train_date;
        public string seattype_num;
        public string from_station_telecode;
        public string to_station_telecode;
        public string include_student;
        public string from_station_telecode_name;
        public string to_station_telecode_name;
        public string round_train_date;
        public string round_start_time_str;
        public string single_round_type;
        public string train_pass_type;
        public string train_class_arr;
        public string start_time_str;
        public string lishi;
        public string train_start_time;
        public string trainno4;
        public string arrive_time;
        public string from_station_name;
        public string to_station_name;
        public string from_station_no;
        public string to_station_no;
        public string ypInfoDetail;
        public string mmStr;
        public string locationCode;
    }

    public enum SeatsType
    {
        无座 = 14,
        硬座 = 13,
        软座 = 12,
        硬卧 = 11,
        软卧 = 10,
        高级软卧 = 9,
        一等座 = 8,
        二等座 = 7,
        特等座 = 6,
        商务座 = 5
    }
}
