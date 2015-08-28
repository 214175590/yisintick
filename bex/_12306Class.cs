using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Ping.Helper;

namespace YisinTick
{
    class _12306Class
    {

        public static Station From;
        public static Station To;

        public static CookieCollection Cookies
        {
            get { return cookies; }
        }

        static CookieCollection cookies = new CookieCollection();

        private const string UrlLoginImage = "https://kyfw.12306.cn/otn/passcodeNew/getPassCodeNew.do?module=login&rand=sjrand&";
        private const string UrlMainPage = "http://www.12306.cn/mormhweb/";


        private const string UrlLoginStepFrom = "https://kyfw.12306.cn/otn/leftTicket/init";
        private const string UrlLoginStepTo = "https://kyfw.12306.cn/otn/login/loginUserAsyn";
        //private const string UrlLoginStepTo = "https://kyfw.12306.cn/otn/login/userLogin";
        private const string UrlFavoriteContacts = "https://kyfw.12306.cn/otn/confirmPassenger/getPassengerDTOs";

        private const string UrlCreateFavoriteContact1_GetToken = "https://dynamic.12306.cn/otsweb/passengerAction.do?method=initUsualPassenger12306#";
        private const string UrlCreateFavoriteContact2_Create = "https://dynamic.12306.cn/otsweb/passengerAction.do?method=savePassenger";

        private const string UrlGetStations = "https://kyfw.12306.cn/otn/resources/js/framework/station_name.js";
        private const string UrlGetTrains = "https://kyfw.12306.cn/otn/leftTicket/queryT?leftTicketDTO.train_date={0}&leftTicketDTO.from_station={1}&leftTicketDTO.to_station={2}&purpose_codes=ADULT";


        private const string UrlSelectTrainSeats = "https://dynamic.12306.cn/otsweb/order/querySingleAction.do";


        private const string UrlCreateTask1_PostData = "https://dynamic.12306.cn/otsweb/order/querySingleAction.do?method=submutOrderRequest";
        private const string UrlCreateTask2_Get = "https://dynamic.12306.cn/otsweb/order/confirmPassengerAction.do?method=init";


        private const string GetTask_1_GetToken = "https://kyfw.12306.cn/otn/confirmPassenger/autoSubmitOrderRequest";
        private const string GetTask_2_GetQueueCount = "https://kyfw.12306.cn/otn/confirmPassenger/getQueueCountAsync";
        public const string GetTask_3_Image = "https://kyfw.12306.cn/otn/passcodeNew/getPassCodeNew.do?module=login&rand=sjrand&";
        private const string GetTask_4_SubmitStatus = "https://kyfw.12306.cn/otn/confirmPassenger/confirmSingleForQueueAsys";

        /// <summary>
        /// 获取登录图片
        /// </summary>
        /// <param name="userAgent"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static Bitmap GetLoginImage(int? timeout = null, string userAgent = null, CookieCollection cookie = null)
        {
            try
            {
                var response = HttpHelper.CreateGetHttpResponse(UrlLoginImage + new Random().NextDouble().ToString(), timeout, userAgent, cookie ?? Cookies);
                Stream resStream = response.GetResponseStream();//得到验证码数据流
                return new Bitmap(resStream);//初始化Bitmap图片
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 登录首页，获取Cookie
        /// </summary>
        /// <param name="userAgent"></param>
        /// <param name="cookie"></param>
        public static void GetMainPage(int? timeout = null, string userAgent = null, CookieCollection cookie = null)
        {
            var response = HttpHelper.CreateGetHttpResponse(UrlMainPage, timeout, userAgent, cookie ?? Cookies, "");

            cookies.Add(response.Cookies);

            response = HttpHelper.CreateGetHttpResponse(UrlLoginStepFrom, timeout, userAgent, cookie ?? Cookies, "");

            cookies.Add(response.Cookies);
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="pass"></param>
        /// <param name="verificationCode"></param>
        /// <param name="timeout"></param>
        /// <param name="userAgent"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static LoginResponse Login(string userName, string pass, string verificationCode
            , int? timeout = null, string userAgent = null, CookieCollection cookie = null)
        {
            var codeResquest = HttpHelper.Post(UrlLoginStepTo + "?random=1389191" + new Random().Next(107720, 957720),
                new Dictionary<string, string>()
                {
                    {"loginUserDTO.user_name", userName},
                    {"userDTO.password", pass},
                    {"randCode", verificationCode}

                }, Encoding.UTF8, Encoding.UTF8, timeout, userAgent, cookie ?? Cookies, UrlLoginStepFrom);



            var serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new[] { new DynamicJsonConverter() });
            dynamic data = serializer.Deserialize<object>(codeResquest);

            if (data.data != null && data.data.status)
            {
                //HttpHelper.Post("https://kyfw.12306.cn/otn/login/userLogin", new Dictionary<string, string>(), Encoding.UTF8, Encoding.UTF8,cookies:Cookies
                //    , Referer: "https://kyfw.12306.cn/otn/login/init", headers: new Dictionary<string, string>() { { "Origin", " https://kyfw.12306.cn" } });
                //HttpHelper.CreateGetHttpResponse(" https://kyfw.12306.cn/otn/index/init ", null, "", null);

                return new LoginResponse() { IsLogined = true, Message = "", type = ErrorType.None, LoginName = data.data.username };
       
            }

            //else if (data.messages.Count <=0)
            //{
            //    return new LoginResponse() { IsLogined = true, Message = "", type = ErrorType.None, LoginName = userName };
       
            //}


            return new LoginResponse() { IsLogined = false, Message = "验证码或密码错误", type = ErrorType.OtherError };
        }

        /// <summary>
        /// 获取常用联系人
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="userAgent"></param>
        /// <param name="cookie"></param>
        public static List<Contact> GetFavoriteContacts( int? timeout = null, string userAgent = null,
                                               CookieCollection cookie = null)
        {
            List<Contact> contacts = new List<Contact>();
          
            Dictionary<string, string> dic = new Dictionary<string, string>();
            var response = HttpHelper.CreatePostHttpResponse(UrlFavoriteContacts, dic, timeout, userAgent, Encoding.UTF8, cookie ?? Cookies, "https://kyfw.12306.cn/otn/leftTicket/init");

            cookies.Add(response.Cookies);
            //dynamic dy;
            try
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    var str = reader.ReadToEnd();

                    var serializer = new JavaScriptSerializer();
                    serializer.RegisterConverters(new[] { new DynamicJsonConverter() });
                    dynamic data = serializer.Deserialize<object>(str);

                    if (data.data != null
                        && data.data.normal_passengers != null
                        && data.data.normal_passengers.Count > 0)
                    {
                        var list = data.data.normal_passengers;

                        foreach (dynamic o in list)
                        {
                            contacts.Add(new Contact()
                            {
                                Name = o.passenger_name,
                                IdTypeName = o.passenger_id_type_name,
                                IdNo = o.passenger_id_no,
                                IdTypeCode = o.passenger_id_type_code,
                                Mobile = o.mobile_no,
                                PassengerTypeName = o.passenger_type_name,
                                PassengerType = o.passenger_type
                            });
                        }
                    }
                }
            }
            catch (Exception)
            {

            }

            return contacts;
        }


        /// <summary>
        /// 添加常用联系人，暂时只支持添加成人
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sexCode">男：M，女：F</param>
        /// <param name="bornDate"></param>
        /// <param name="cardType"></param>
        /// <param name="cardNo"></param>
        /// <param name="mobileNo"></param>
        /// <param name="timeout"></param>
        /// <param name="userAgent"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static CreateFavoriteContactResponse CreateCreateFavoriteContact(string name, string sexCode, string bornDate, string cardType, string cardNo, string mobileNo = ""
            , int? timeout = null, string userAgent = null, CookieCollection cookie = null)
        {
            string token = "";
            string str;
            HttpWebResponse response;

            #region 第一步获取Token
            try
            {

                response = HttpHelper.CreateGetHttpResponse(UrlCreateFavoriteContact1_GetToken, timeout, userAgent, cookie ?? Cookies, "");

                cookies.Add(response.Cookies);

                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    str = reader.ReadToEnd();
                }
            }
            catch (Exception)
            {
                return new CreateFavoriteContactResponse() { IsCreated = false, Message = "网络可能存在问题，请您重试一下！", type = ErrorType.NetworkError };
            }


            Regex Token = new Regex("<input type=\"hidden\" name=\"org.apache.struts.taglib.html.TOKEN\" value=\"(?<token>[^\"]*?)\"></div>");

            if (Token.IsMatch(str))
            {
                token = Token.Matches(str)[0].Groups["token"].Value;
            }

            #endregion

            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("org.apache.struts.taglib.html.TOKEN", token);
            dic.Add("name", name);
            dic.Add("sex_code", sexCode);
            dic.Add("born_date", bornDate);
            dic.Add("country_code", "CN");
            dic.Add("card_type", cardType);
            dic.Add("card_no", cardNo);
            dic.Add("passenger_type", "1");//暂时只支持添加成人
            dic.Add("mobile_no", mobileNo);
            dic.Add("phone_no", "");
            dic.Add("email", "");
            dic.Add("address", "");
            dic.Add("postalcode", "");
            dic.Add("studentInfo.province_code", "11");
            dic.Add("studentInfo.school_code", "");
            dic.Add("studentInfo.school_name", "简码/汉字");
            dic.Add("studentInfo.department", "");
            dic.Add("studentInfo.school_class", "");
            dic.Add("studentInfo.student_no", "");
            dic.Add("studentInfo.school_system", "4");
            dic.Add("studentInfo.enter_year", "2002");
            dic.Add("studentInfo.preference_card_no", "");
            dic.Add("studentInfo.preference_from_station_name", "简码/汉字");
            dic.Add("studentInfo.preference_from_station_code", "");
            dic.Add("studentInfo.preference_to_station_name", "简码/汉字");
            dic.Add("studentInfo.preference_to_station_code", "");

            try
            {

                response = HttpHelper.CreatePostHttpResponse(UrlCreateFavoriteContact2_Create, dic, timeout, userAgent, Encoding.UTF8, cookie ?? Cookies, "https://dynamic.12306.cn/otsweb/passengerAction.do?method=initAddPassenger&");

                cookies.Add(response.Cookies);

                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    str = reader.ReadToEnd();
                }
            }
            catch (Exception)
            {
                return new CreateFavoriteContactResponse() { IsCreated = false, Message = "网络可能存在问题，请您重试一下！", type = ErrorType.NetworkError };
            }

            Regex message = new Regex("var message = \"(?<message>[^\"]*?)\";");

            if (message.IsMatch(str))
            {
                str = message.Matches(str)[0].Groups["message"].Value;
                if (str.Contains("添加常用联系人成功"))
                {
                    return new CreateFavoriteContactResponse() { IsCreated = true, Message = "", type = ErrorType.None };
                }
                else
                {
                    return new CreateFavoriteContactResponse() { IsCreated = false, Message = str, type = ErrorType.OtherError };
                }
            }

            return new CreateFavoriteContactResponse() { IsCreated = false, Message = "其他错误！", type = ErrorType.OtherError };
        }

        /// <summary>
        /// 获取车站信息
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="userAgent"></param>
        /// <param name="cookie"></param>
        public static List<Station> GetStations(int? timeout = null, string userAgent = null,
                                               CookieCollection cookie = null)
        {
            List<Station> list = new List<Station>();

            var response = HttpHelper.CreateGetHttpResponse(UrlGetStations, timeout, userAgent, cookie ?? Cookies);

            cookies.Add(response.Cookies);
            try
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    var str = reader.ReadToEnd();
                    //@spd|山坡东|SBN|shanpodong|spd|0@tdd|土地堂东|TTN|tuditangdong|tdtd|1
                    Regex stationNamesRegex = new Regex("'(?<stationNames>[^\']*?)'");

                    if (stationNamesRegex.IsMatch(str))
                    {
                        string stationNames = stationNamesRegex.Matches(str)[0].Groups["stationNames"].Value;


                        string[] stations = stationNames.Split('@');

                        foreach (var station in stations)
                        {
                            if (string.IsNullOrEmpty(station))
                            {
                                continue;
                            }

                            string[] names = station.Split('|');

                            list.Add(new Station()
                            {
                                Shorthand = names[0],
                                Name = names[1],
                                Code = names[2],
                                Pinyin = names[3],
                                FirstLetter = names[4],
                                Order = names[5]
                            });
                        }
                    }
                }
            }
            catch (Exception)
            {

            }

            return list;
        }

        /// <summary>
        /// 获取车次信息
        /// </summary>
        /// <param name="date"></param>
        /// <param name="fromstation"></param>
        /// <param name="tostation"></param>
        /// <param name="timeout"></param>
        /// <param name="userAgent"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static List<Train> GetTrains(string date, string fromstation, string tostation,
                                            int? timeout = null, string userAgent = null, CookieCollection cookie = null)
        {
            List<Train> list = new List<Train>();
            string str = "";
            try
            {
                str = new HttpHelper2().Get(string.Format(UrlGetTrains, date, fromstation, tostation), Encoding.UTF8
                    , timeout, userAgent, cookie ?? Cookies, "https://kyfw.12306.cn/otn/leftTicket/init"
                    , new Dictionary<string, string>() { { "X-Requested-With", "XMLHttpRequest" } });
            }
            catch (Exception)
            {
                return new List<Train>();
            }
            if (!string.IsNullOrEmpty(str))
            {
                var serializer = new JavaScriptSerializer();
                serializer.RegisterConverters(new[] {new DynamicJsonConverter()});
                try
                {
                    dynamic data = serializer.Deserialize<object>(str);
                    if (data.data != null)
                    {
                        foreach (dynamic o in data.data)
                        {
                            list.Add(new Train()
                            {
                                Id = o.secretStr,
                                EndStation = o.queryLeftNewDTO.to_station_name,
                                EndTime = o.queryLeftNewDTO.arrive_time,
                                StartStation = o.queryLeftNewDTO.start_station_name,
                                StartTime = o.queryLeftNewDTO.start_time,
                                TrainValue = o.queryLeftNewDTO.station_train_code,
                                StationTrainCode = o.queryLeftNewDTO.station_train_code,
                                TrainNo = o.queryLeftNewDTO.train_no,
                                end_station_telecode = o.queryLeftNewDTO.end_station_telecode,
                                from_station_telecode = o.queryLeftNewDTO.from_station_telecode,
                                SWZ = o.queryLeftNewDTO.swz_num,
                                TZ = o.queryLeftNewDTO.tz_num,
                                ZY = o.queryLeftNewDTO.zy_num,
                                ZE = o.queryLeftNewDTO.ze_num,
                                GR = o.queryLeftNewDTO.gr_num,
                                RW = o.queryLeftNewDTO.rw_num,
                                RZ = o.queryLeftNewDTO.rz_num,
                                YW = o.queryLeftNewDTO.yw_num,
                                YZ = o.queryLeftNewDTO.yz_num,
                                WZ = o.queryLeftNewDTO.wz_num,
                                QT = o.queryLeftNewDTO.qt_num
                            });
                        }
                    }
                }
                catch (Exception)
                {
                    return new List<Train>();
                }
            }
            return list;
        }

        /// <summary>
        /// 获取座位信息
        /// </summary>
        /// <param name="date"></param>
        /// <param name="startStationCode"></param>
        /// <param name="endStationCode"></param>
        /// <param name="trainNo"></param>
        /// <param name="timeout"></param>
        /// <param name="userAgent"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static TrainSeat CreateTask(TrainSeat trainSeat,
                                  int? timeout = null, string userAgent = null, CookieCollection cookie = null)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("station_train_code", trainSeat.station_train_code);
            dic.Add("train_date", trainSeat.train_date);
            dic.Add("seattype_num", trainSeat.seattype_num);
            dic.Add("from_station_telecode", trainSeat.from_station_telecode);
            dic.Add("to_station_telecode", trainSeat.to_station_telecode);
            dic.Add("include_student", trainSeat.include_student);
            dic.Add("from_station_telecode_name", trainSeat.from_station_telecode_name);
            dic.Add("to_station_telecode_name", trainSeat.to_station_telecode_name);
            dic.Add("round_train_date", trainSeat.round_train_date);
            dic.Add("round_start_time_str", trainSeat.round_start_time_str);
            dic.Add("single_round_type", trainSeat.single_round_type);
            dic.Add("train_pass_type", trainSeat.train_pass_type);
            dic.Add("train_class_arr", trainSeat.train_class_arr);
            dic.Add("start_time_str", trainSeat.start_time_str);
            dic.Add("lishi", trainSeat.lishi);
            dic.Add("train_start_time", trainSeat.train_start_time);
            dic.Add("trainno4", trainSeat.trainno4);
            dic.Add("arrive_time", trainSeat.arrive_time);
            dic.Add("from_station_name", trainSeat.from_station_name);
            dic.Add("to_station_name", trainSeat.to_station_name);
            dic.Add("from_station_no", trainSeat.from_station_no);
            dic.Add("to_station_no", trainSeat.to_station_no);
            dic.Add("ypInfoDetail", trainSeat.ypInfoDetail);
            dic.Add("mmStr", trainSeat.mmStr);
            dic.Add("locationCode", trainSeat.locationCode);

            string str = "";

            try
            {
                var response = HttpHelper.CreatePostHttpResponse(UrlCreateTask1_PostData, dic, timeout, userAgent, UTF8Encoding.UTF8, cookie ?? Cookies
                                                                    , "https://dynamic.12306.cn/otsweb/order/querySingleAction.do?method=init");

                cookies.Add(response.Cookies);

                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    str = reader.ReadToEnd();
                }
            }
            catch (Exception)
            {
                return null;
            }

            try
            {
                var response = HttpHelper.CreateGetHttpResponse(UrlCreateTask2_Get, timeout, userAgent, cookie ?? Cookies
                                                                    , "https://dynamic.12306.cn/otsweb/order/querySingleAction.do?method=init");

                cookies.Add(response.Cookies);

                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    str = reader.ReadToEnd();
                }
            }
            catch (Exception)
            {
                return null;
            }

            return null;
            // return trainSeat;
        }

        public static CreateResponse GetTask(Train train, string date, SeatsType seat, List<Contact> selectContactList)
        {
            string seatType = "";

            switch (seat)
            {
                case SeatsType.商务座:
                    seatType = "9";
                    break;
                case SeatsType.特等座:
                    seatType = "P";
                    break;
                case SeatsType.一等座:
                    seatType = "M";
                    break;
                case SeatsType.二等座:
                    seatType = "O";
                    break;
                case SeatsType.高级软卧:
                    seatType = "5";
                    break;
                case SeatsType.软卧:
                    seatType = "4";
                    break;
                case SeatsType.硬卧:
                    seatType = "3";
                    break;
                case SeatsType.软座:
                    seatType = "2";
                    break;
                case SeatsType.硬座:
                    seatType = "1";
                    break;
                case SeatsType.无座:
                    seatType = "1";
                    break;
            }

            StringBuilder passengerTicketStr = new StringBuilder();
            StringBuilder oldPassengerStr = new StringBuilder();

            foreach (Contact contact in selectContactList)
            {
                passengerTicketStr.AppendFormat("{0},0,1,{1},{2},{3},{4},N_", seatType, contact.Name, contact.IdTypeCode, contact.IdNo, contact.Mobile);
                oldPassengerStr.AppendFormat("{1},{2},{3},1_", seatType, contact.Name, contact.IdTypeCode, contact.IdNo, contact.Mobile);
            }


            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("secretStr", train.Id);
            dic.Add("train_date", date);
            dic.Add("tour_flag", "dc");
            dic.Add("purpose_codes", "ADULT");
            dic.Add("query_from_station_name", From.Name);
            dic.Add("query_to_station_name", To.Name);
            dic.Add("", "");
            dic.Add("cancel_flag", "2");
            dic.Add("bed_level_order_num", "000000000000000000000000000000");
            dic.Add("passengerTicketStr", passengerTicketStr.ToString().TrimEnd('_'));
            dic.Add("oldPassengerStr", oldPassengerStr.ToString());

            var codeResquest = new HttpHelper2().Post(GetTask_1_GetToken,
                dic, Encoding.UTF8, Encoding.UTF8, Referer: "https://kyfw.12306.cn/otn/leftTicket/init",cookies:Cookies
                , headers: new Dictionary<string, string>() { { "Origin", "https://kyfw.12306.cn" } });

            if (codeResquest == null)
            {
                return new CreateResponse(){IsCreate =  false,Message = "Errorl"};
            }

            var serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new[] { new DynamicJsonConverter() });
            dynamic data = serializer.Deserialize<object>(codeResquest);

            if (data.status == null || !data.status || data.data == null || data.data.result == null)
            {
                try
                {
                    return new CreateResponse() { IsCreate = false, Message = data.messages != null ? data.messages[0] ?? "未知错误" : "未知错误" };
                }
                catch (Exception)
                {
                    return new CreateResponse() { IsCreate = false, Message = data.errMsg ?? "未知错误"  };
                }
            }

            //Q6#BA6C4F23E49E84F96A07B8ECA37A9FF350DAD2E2F484AD96F61C2046#O007450669M0099501499019950025#1
            Token token = new Token();

            string[] tokens = data.data.result.Split('#');;

            token.Q = tokens[0];
            token.LongToken = tokens[1];
            token.ShortToken = tokens[2];


            dic.Clear();
            dic.Add("train_date",
                (Convert.ToDateTime(date).ToString("ddd MMM dd yyy ", DateTimeFormatInfo.InvariantInfo) +
                 DateTime.Now.ToString("HH:mm:ss").Replace(":", "%3A") + " GMT%2B0800  (China Standard Time)").Replace(' ', '+'));
            dic.Add("train_no", train.TrainNo);
            dic.Add("stationTrainCode", train.StationTrainCode);
            dic.Add("seatType", seatType);
            dic.Add("fromStationTelecode", train.from_station_telecode);
            dic.Add("toStationTelecode", train.end_station_telecode);
            dic.Add("leftTicket", token.ShortToken);
            dic.Add("purpose_codes", "ADULT");
            dic.Add("_json_att", "");


            CookieCollection cookiesTemp = new CookieCollection();

            cookiesTemp.Add(Cookies);

            cookiesTemp.Add(new Cookie("_jc_save_fromStation", escape(From.Name) + "%2C" + From.Code, "/otn") { Domain = "kyfw.12306.cn" });
            cookiesTemp.Add(new Cookie("_jc_save_toStation", escape(To.Name) + "%2C" + To.Code, "/otn") { Domain = "kyfw.12306.cn" });
            cookiesTemp.Add(new Cookie("_jc_save_fromDate", date, "/otn") { Domain = "kyfw.12306.cn" });
            cookiesTemp.Add(new Cookie("_jc_save_toDate", Convert.ToDateTime(date).AddDays(15).ToString("yyyy-MM-dd"), "/otn") { Domain = "kyfw.12306.cn" });
            cookiesTemp.Add(new Cookie("_jc_save_wfdc_flag", "dc", "/otn") { Domain = "kyfw.12306.cn" });

            codeResquest = new HttpHelper2().Post(GetTask_2_GetQueueCount,
                dic, Encoding.UTF8, Encoding.UTF8, cookies: cookiesTemp,
                Referer: "https://kyfw.12306.cn/otn/leftTicket/init"
                ,
                headers:
                    new Dictionary<string, string>()
                    {
                        {"Origin", "https://kyfw.12306.cn"},
                        {"X-Requested-With", "XMLHttpRequest"}
                    });

            serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new[] { new DynamicJsonConverter() });
            data = serializer.Deserialize<object>(codeResquest);

            if (data.status == null || !data.status || data.data == null || data.data.ticket == null)
            {
                return new CreateResponse() { IsCreate = false, Message = data.messages != null ? data.messages[0] ?? "未知错误" : "未知错误" };
            }

            token.ticket = data.data.ticket;
            string Code = "";
            try
            {
                do
                {
                    var response = HttpHelper.CreateGetHttpResponse(GetTask_3_Image + new Random().NextDouble().ToString(),
                                null, "https://kyfw.12306.cn/otn/leftTicket/init", Cookies);
                    Stream resStream = response.GetResponseStream();//得到验证码数据流
                    Bitmap bmp =  new Bitmap(resStream);//初始化Bitmap图片
                    new Music(2).Play();
                    ImageFrom.GetImageFrom.Show(bmp);
                    Code = ImageFrom.GetImageFrom.Code;
                } while (Code.Length != 4 && Program.mainForm.GetRunStatus());
            }
            catch (Exception)
            {
                return new CreateResponse() { IsCreate = false, Message = data.data.errMsg ?? "未知错误" };
            }

            //dic.Clear();
            //dic.Add("randCode", Code);
            //dic.Add("rand", "sjrand");
            //dic.Add("_json_att", "");

            //codeResquest = new HttpHelper2().Post("https://kyfw.12306.cn/otn/passcodeNew/checkRandCodeAnsyn",
            //  dic, Encoding.UTF8, Encoding.UTF8, Referer: "https://kyfw.12306.cn/otn/leftTicket/init", cookies: cookiesTemp
            //  , headers:
            //      new Dictionary<string, string>()
            //        {
            //            {"Origin", "https://kyfw.12306.cn"},
            //            {"X-Requested-With", "XMLHttpRequest"}
            //        });



            dic.Clear();
            dic.Add("passengerTicketStr", System.Web.HttpUtility.UrlEncode((passengerTicketStr.ToString().TrimEnd('_'))).ToUpper());
            dic.Add("oldPassengerStr", System.Web.HttpUtility.UrlEncode(oldPassengerStr.ToString()).ToUpper());
            dic.Add("randCode", Code);
            dic.Add("purpose_codes", "ADULT");
            dic.Add("key_check_isChange", token.LongToken);
            dic.Add("leftTicketStr", token.ShortToken);
            dic.Add("train_location", token.Q);
            dic.Add("_json_att", "");

            cookiesTemp = new CookieCollection();

            cookiesTemp.Add(Cookies);

            cookiesTemp.Add(new Cookie("_jc_save_fromStation", escape(From.Name) + "%2C" + From.Code, "/otn") { Domain = "kyfw.12306.cn" });
            cookiesTemp.Add(new Cookie("_jc_save_toStation", escape(To.Name) + "%2C" + To.Code, "/otn") { Domain = "kyfw.12306.cn" });
            cookiesTemp.Add(new Cookie("_jc_save_fromDate", date, "/otn") { Domain = "kyfw.12306.cn" });
            cookiesTemp.Add(new Cookie("_jc_save_toDate", Convert.ToDateTime(date).AddDays(15).ToString("yyyy-MM-dd"), "/otn") { Domain = "kyfw.12306.cn" });
            cookiesTemp.Add(new Cookie("_jc_save_wfdc_flag", "dc", "/otn") { Domain = "kyfw.12306.cn" });

            codeResquest = new HttpHelper2().Post(GetTask_4_SubmitStatus,
                dic, Encoding.UTF8, Encoding.UTF8, Referer: "https://kyfw.12306.cn/otn/leftTicket/init", cookies: cookiesTemp
                , headers:
                    new Dictionary<string, string>()
                    {
                        {"Origin", "https://kyfw.12306.cn"},
                        {"X-Requested-With", "XMLHttpRequest"}
                    });


            if (codeResquest == null)
            {
                return new CreateResponse() { IsCreate = false, Message = "Errorl" };
            }

            serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new[] { new DynamicJsonConverter() });
            data = serializer.Deserialize<object>(codeResquest);

            if (data.status == null || !data.status || data.data == null || data.data.submitStatus == null)
            {
                try
                {
                    return new CreateResponse() { IsCreate = false, Message = data.messages != null ? data.messages[0] ?? "未知错误" : "未知错误" };
                }
                catch (Exception)
                {
                    return new CreateResponse() { IsCreate = false, Message = data.data.errMsg ?? "未知错误" };
                }
            }
            else if (data.data != null && data.data.submitStatus!=null)
            {
                if (data.data.submitStatus)
                {
                    TickCute.ChangeTickCount(-1);
                    new Music(1).Play();
                    Program.mainForm.SetLinkLabelText("订票成功！登录12306查看！<-");
                    return new CreateResponse() { IsCreate = true, Message = "订票成功！" };
                }
                else
                {
                    return new CreateResponse() { IsCreate = false, Message = "订票失败！" };
                }
            }


            return new CreateResponse() { IsCreate = false, Message = data.messages != null ? data.messages[0] ?? "未知错误" : "未知错误" };
            //else if (data.messages.Count <=0)
            //{
            //    return new LoginResponse() { IsLogined = true, Message = "", type = ErrorType.None, LoginName = userName };

            //}
        }

        private static string escape(string s)
        {
            StringBuilder sb = new StringBuilder();
            byte[] ba = System.Text.Encoding.Unicode.GetBytes(s);
            for (int i = 0; i < ba.Length; i += 2)
            {
                sb.Append("%u");
                sb.Append(ba[i + 1].ToString("X2"));

                sb.Append(ba[i].ToString("X2"));
            }
            return sb.ToString();

        }  
    }



    class Contact
    {
        public string Name;
        public string IdTypeName;
        public string IdNo;
        public string IdTypeCode;
        public string Mobile;
        public string PassengerTypeName;
        public string PassengerType;

        public override string ToString()
        {
            return string.Format("{0}\t{1}\t{2}", Name, IdTypeName, IdNo);
        }
    }

    class LoginResponse
    {
        public bool IsLogined;
        public string Message;
        public string LoginName;
        public ErrorType type;
    }
    class CreateResponse
    {
        public bool IsCreate;
        public string Message;
    }

    class CreateFavoriteContactResponse
    {
        public bool IsCreated;
        public string Message;
        public ErrorType type;
    }

    public enum ErrorType
    {
        None,
        NetworkError,
        VerificationError,
        PassordError,
        EmailError,
        OtherError
    }

    class Token
    {
        public string Q;
        public string LongToken;
        public string ShortToken;
        public string ticket;
    }
}
