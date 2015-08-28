using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Xml;
using System.IO;

namespace YisinTick
{
    class ConfigStore
    {
        public static string xmlPath = System.Environment.CurrentDirectory + "\\conf\\config.xml";
        public static string appDir = System.Environment.CurrentDirectory;

        public static Hashtable codeTable = new Hashtable();

        public static Hashtable userTable = new Hashtable();

        public static ArrayList libarays = new ArrayList();
        public static String currVersion = "1.0.1";
        public static bool islocalhost = false;
        public static String skin = "";
        public static String startType = "1"; // 1 随机启动，2 不随机启动
        public static String updateType = "1"; // 1 自动更新，2 更新提醒，3 不更新
        public static String algorithm = "1"; // 相似度比较算法：1 灰度直方图计算法，2 像素点颜色值相似计算法
        public static bool isAutoBaojing = false; // 购票成功后是否报警
        public static bool isAutoWriterVerify = true; // 是否自动输入验证码
        public static int failedTimes = 3; // 失败N次数后改为手动输入验证码
        public static int yesFailedTimes = 0; // 失败N次数后改为手动输入验证码
        public static DateTime lastFailedTime = DateTime.Now;
        public static String fromCity = "";
        public static String toCity = "";
        public static DateTime tempDate = DateTime.Now;
        public static String musicPath1 = "\\data\\22050.wav";
        public static String musicPath2 = "\\data\\22060.wav";

        public static String bbsUser = "";
        public static String bbsPwd = "";

        public static void InitConfig()
        {
            // 先加载基本配置
            loadConfig();

            // 加载验证码特征库
            XmlUtils.loadLibarayXml(null, null);
            
        }

        public static void loadConfig()
        {
            Program.mainForm.ShowMessage("加载系统配置...", false);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlPath);
            //查找<users>
            XmlNode root = xmlDoc.SelectSingleNode("config");
            //获取到所有<users>的子节点
            XmlNodeList nodeList = root.ChildNodes;
            //遍历所有子节点
            XmlElement xe = null, ce = null;
            Libaray lib = null;
            XmlNodeList xnl = null;
            foreach (XmlNode xn in nodeList)
            {
                xe = (XmlElement)xn;
                if (xe.Name.Equals("library"))
                {
                    xnl = xe.ChildNodes;
                    foreach (XmlNode cn in xnl)
                    {
                        ce = (XmlElement)cn;
                        lib = new Libaray();
                        lib.status = Convert.ToInt32(ce.GetAttribute("status"));
                        lib.id = ce.GetAttribute("id");
                        if(lib.status != 999){
                            lib.name = ce.InnerText;
                            libarays.Add(lib);
                        }
                        else
                        {
                            try
                            {
                                CommonUtil.DeleteDir(ConfigStore.appDir + "\\data\\" + lib.id);
                            }
                            catch {}
                        }
                    }
                }
                else if (xe.Name.Equals("users"))
                {
                    User user = null;
                    xnl = xe.ChildNodes;
                    foreach (XmlNode cn in xnl)
                    {
                        ce = (XmlElement)cn;
                        user = new User();
                        user.Acc = ce.GetAttribute("acc");
                        user.Pwd = MyEncrypt.DecryptB(ce.GetAttribute("pwd"));
                        user.Status = Convert.ToInt32(ce.GetAttribute("status"));
                        user.Name = ce.InnerText;
                        userTable.Add(user.Acc, user);
                    }
                }
                else if (xe.Name.Equals("skin"))
                {
                    skin = xe.InnerText;
                }
                else if (xe.Name.Equals("start"))
                {
                    startType = xe.InnerText; 
                }
                else if (xe.Name.Equals("update"))
                {
                    updateType = xe.InnerText;
                }
                else if (xe.Name.Equals("algorithm"))
                {
                    algorithm = xe.InnerText;
                }
                else if (xe.Name.Equals("islocalhost"))
                {
                    islocalhost = Convert.ToBoolean(xe.InnerText);
                }
                else if (xe.Name.Equals("isAutoBaojing"))
                {
                    isAutoBaojing = Convert.ToBoolean(xe.InnerText);
                }
                else if (xe.Name.Equals("isAutoWriterVerify"))
                {
                    isAutoWriterVerify = Convert.ToBoolean(xe.InnerText);
                }
                else if (xe.Name.Equals("failedTimes"))
                {
                    failedTimes = Convert.ToInt32(xe.InnerText);
                }
                else if (xe.Name.Equals("fromCity"))
                {
                    fromCity = xe.InnerText;
                }
                else if (xe.Name.Equals("toCity"))
                {
                    toCity = xe.InnerText;
                }
                else if (xe.Name.Equals("bbsUser"))
                {
                    bbsUser = xe.InnerText;
                }
                else if (xe.Name.Equals("bbsPwd"))
                {
                    bbsPwd = MyEncrypt.DecryptB(xe.InnerText);
                }
                
            }
            Program.mainForm.ShowMessage("完成");
            SaveConfig();
        }


        public static void SaveConfig()
        {
            Program.mainForm.ShowMessage("正在保存系统配置...", false);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlPath);
            XmlElement root = xmlDoc.DocumentElement;
            root.RemoveAll();

            XmlElement one = xmlDoc.CreateElement("library");
            XmlElement two = null;
            /** 特征库设置 **/
            foreach (Libaray lib in libarays)
            {
                two = xmlDoc.CreateElement("lib");
                two.SetAttribute("id", lib.id);
                two.SetAttribute("status", "" + lib.status);
                two.InnerText = lib.name;
                one.AppendChild(two);
            }
            //在节点中添加元素
            root.AppendChild(one);

            /** 用户设置 **/
            one = xmlDoc.CreateElement("users");
            ArrayList userArray = new ArrayList(userTable.Keys);
            User user = null;
            foreach (String acc in userArray)
            {
                user = (User) userTable[acc];
                two = xmlDoc.CreateElement("user");
                two.SetAttribute("acc", user.Acc);
                two.SetAttribute("pwd", MyEncrypt.EncryptA(user.Pwd));
                two.SetAttribute("status", "" + user.Status);
                two.InnerText = user.Name;
                one.AppendChild(two);
            }
            //在节点中添加元素
            root.AppendChild(one);

            /** 皮肤设置 **/
            one = xmlDoc.CreateElement("skin");
            one.InnerText = skin;
            //在节点中添加元素
            root.AppendChild(one);

            /** 是否开机启动设置 **/
            one = xmlDoc.CreateElement("start");
            one.InnerText = startType;
            //在节点中添加元素
            root.AppendChild(one);

            /** 更新设置 **/
            one = xmlDoc.CreateElement("update");
            one.InnerText = updateType;
            //在节点中添加元素
            root.AppendChild(one);

            /** 算法设置 **/
            one = xmlDoc.CreateElement("algorithm");
            one.InnerText = algorithm;
            //在节点中添加元素
            root.AppendChild(one);

            /** 购票成功后是否报警设置 **/
            one = xmlDoc.CreateElement("isAutoBaojing");
            one.InnerText = Convert.ToString(isAutoBaojing);
            //在节点中添加元素
            root.AppendChild(one);

            /** 是否自动输入验证码设置 **/
            one = xmlDoc.CreateElement("isAutoWriterVerify");
            one.InnerText = Convert.ToString(isAutoWriterVerify);
            //在节点中添加元素
            root.AppendChild(one);

            /** 失败N次数后改为手动输入验证码设置 **/
            one = xmlDoc.CreateElement("failedTimes");
            one.InnerText = Convert.ToString(failedTimes);
            //在节点中添加元素
            root.AppendChild(one);

            if (fromCity != null && !fromCity.Equals(""))
            {
                /** 出发站设置 **/
                one = xmlDoc.CreateElement("fromCity");
                one.InnerText = fromCity;
                //在节点中添加元素
                root.AppendChild(one);
            }
            if (toCity != null && !toCity.Equals(""))
            {
                /** 到达站设置 **/
                one = xmlDoc.CreateElement("toCity");
                one.InnerText = toCity;
                //在节点中添加元素
                root.AppendChild(one);
            }
            if (islocalhost)
            {
                /** 设置 **/
                one = xmlDoc.CreateElement("islocalhost");
                one.InnerText = Convert.ToString(islocalhost);
                //在节点中添加元素
                root.AppendChild(one);
            }

            if (currVersion != null)
            {
                /** 版本号设置 **/
                one = xmlDoc.CreateElement("currVersion");
                one.InnerText = currVersion;
                //在节点中添加元素
                root.AppendChild(one);
            }

            if (!String.IsNullOrEmpty(bbsUser))
            {
                /** bbs用户名设置 **/
                one = xmlDoc.CreateElement("bbsUser");
                one.InnerText = bbsUser;
                //在节点中添加元素
                root.AppendChild(one);
            }

            if (!String.IsNullOrEmpty(bbsPwd))
            {
                /** bbs密码设置 **/
                one = xmlDoc.CreateElement("bbsPwd");
                one.InnerText = MyEncrypt.EncryptA(bbsPwd);
                //在节点中添加元素
                root.AppendChild(one);
            }

            //保存
            xmlDoc.Save(xmlPath);
            Program.mainForm.ShowMessage("完成");
        }

        public static void SetLibarayStatus(int index, int status)
        {
            if (libarays != null && index < libarays.Count && index >= 0)
            {
                ((Libaray)libarays[index]).status = status;
            }            
        }

        public static void SetAllLibarayStatus(int status)
        {
            foreach (Libaray lib in libarays)
            {
                lib.status = status;
            }
        }

        public static void AddUser(String acc, String pwd, int status, String name)
        {
            if(status == 1){
                ArrayList userArray = new ArrayList(userTable.Keys);
                foreach (String uacc in userArray)
                {
                    ((User)userTable[uacc]).Status = 0;
                }
            }
            if(userTable.ContainsKey(acc)){
                ((User)userTable[acc]).Status = 1;
            }
            else
            {
                User user = new User();
                user.Acc = acc;
                user.Pwd = pwd;
                user.Status = status;
                user.Name = name;
                userTable.Add(acc, user);
            }            
        }

        public static User GetActiveUser()
        {
            User user = null;
            ArrayList userArray = new ArrayList(userTable.Keys);
            foreach (String uacc in userArray)
            {
                user = (User)userTable[uacc];
                if (user.Status == 1)
                {
                    break;
                }
            }
            return user;
        }

        public static void RemoveUser(String acc)
        {
            userTable.Remove(acc);
        }

    }

    class User
    {
        public String Acc { get; set; }
        public String Name { get; set; }
        public String Pwd { get; set; }
        public int Status { get; set; }
    }

    class Libaray
    {
        public String id { get; set; }
        public int status { get; set; }
        public String name { get; set; }
    }

}