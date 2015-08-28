using HtmlAgilityPack;
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
using System.Xml;

namespace YisinTick
{
    class _88448Class
    {

        public static CookieCollection Cookies
        {
            get { return cookies; }
        }

        public static CookieCollection cookies = new CookieCollection();

        public static CookieCollection AdminCookies
        {
            get { return adminCookies; }
        }

        public static CookieCollection adminCookies = new CookieCollection();

        private const String mainUrl = "http://yisin.88448.com/";

        private const String adminUrl = "http://yisin.88448.com/admincp.php?";
        private const String ajaxUrl = "http://yisin.88448.com/ajax.php?action=updateseccode_admincp&inajax=1&ajaxtarget=seccodeverify_admincp_menu";
               
        private const String loginUrl = "http://yisin.88448.com/logging.php?action=login&loginsubmit=yes";
        private const String loginFormUrl = "index.php";
        private static String userSpaceHref = "http://yisin.88448.com/space.php?uid={0}";
        private static String logoutUrl = "http://yisin.88448.com/logging.php?action=logout&formhash={0}";
        public static bool islogin = false;
        private static String uid = "";
        private static String account = "";
        private static String uname = "";

        private static String formhash = "";
        public static UserInfo user = null;

        public static void GetWebCookie(int? timeout = null, string userAgent = null, CookieCollection cookie = null)
        {
            var response = HttpHelper.CreateGetHttpResponse(mainUrl, timeout, userAgent, cookie ?? Cookies, "");
            cookies.Add(response.Cookies);
            try
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    String html = reader.ReadToEnd();
                    if(!String.IsNullOrEmpty(html)){
                        islogin = html.IndexOf("umenu") != -1 && html.IndexOf("登录") != -1 && html.IndexOf("注册") != -1 ;
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public static String GetAdminCookie(int? timeout = null, string userAgent = null)
        {
            var response = HttpHelper.CreateGetHttpResponse(adminUrl, timeout, userAgent, Cookies, "");
            adminCookies.Add(response.Cookies);
            try
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static Stream GetAdminVerifyCode(int? timeout = null, string userAgent = null)
        {
            var response = HttpHelper.CreateGetHttpResponse(ajaxUrl, timeout, userAgent, Cookies, "");
            String result = "";
            try
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    result = reader.ReadToEnd();
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(result);
                    XmlNode root = xmlDoc.SelectSingleNode("root");
                    result = root.InnerText;
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(result);
                    HtmlNode node = doc.DocumentNode;
                    HtmlNode img = node.SelectSingleNode("//img");
                    result = mainUrl + img.Attributes[3].Value;
                    response = HttpHelper.CreateGetHttpResponse(result, timeout, userAgent, response.Cookies, "");
                    StreamReader reader2 = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    result = reader2.ReadToEnd();
                    Console.WriteLine(result);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Data + "\n" + e.Message);
                return null;
            }
            return response.GetResponseStream();
        }

        public static void LoginAdmin(string verificationCode, int? timeout = null, string userAgent = null, CookieCollection cookie = null)
        {
            var codeResquest = HttpHelper.Post(adminUrl + "?random=1389191" + new Random().Next(107720, 957720),
                new Dictionary<string, string>()
                {
                    {"frames", "yes"},
                    {"admin_password", ""},
                    {"seccodeverify", verificationCode},
                    {"sid", "aDiXr7"},
                    {"submit", "%E6%8F%90%E4%BA%A4"}
                    //{"referer", "index.php"}  
                    // sid=aDiXr7&frames=yes&admin_password=&seccodeverify=cccb&submit=%E6%8F%90%E4%BA%A4
                }, Encoding.UTF8, Encoding.UTF8, timeout, userAgent, cookie ?? Cookies, loginFormUrl);

            String result = GetLoginResult(codeResquest);
        }

        public static LoginResponse Login(string userName, string pass, string verificationCode
            , int? timeout = null, string userAgent = null, CookieCollection cookie = null, bool noa = true)
        {
            if (noa)
            {
                Program.mainForm.ShowMessage("开始登录隐心论坛...", false);
            }
            var codeResquest = HttpHelper.Post(loginUrl + "?random=1389191" + new Random().Next(107720, 957720),
                new Dictionary<string, string>()
                {
                    {"username", userName},
                    {"password", pass},
                    {"randCode", verificationCode},
                    {"formhash", "91f72709"},
                    {"loginsubmit", "true"}
                    //{"referer", "index.php"}  
                    // formhash=91f72709&referer=&username=admin&password=96e79218965eb72c92a549dd5a330112&loginsubmit=true
                }, Encoding.UTF8, Encoding.UTF8, timeout, userAgent, cookie ?? Cookies, loginFormUrl);

            String result = GetLoginResult(codeResquest);            
            if (result != null && result.IndexOf("欢迎您回来") != -1)
            {
                String[] str1 = result.Split('。');
                Program.mainForm.ShowMessage(str1[0]);
                if (noa)
                {
                    uid = GetUserId(codeResquest);
                    account = userName;                   
                    uname = str1[0].Split('，')[1];
                }
               
                //
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(codeResquest);
                HtmlNode node = doc.DocumentNode;
                formhash = node.SelectSingleNode("//input[@name='formhash']").Attributes[2].Value;

                return new LoginResponse() { IsLogined = true, Message = str1[0], type = ErrorType.None, LoginName = uname };          
            }
            Program.mainForm.ShowMessage(result);
            return new LoginResponse() { IsLogined = false, Message = result, type = ErrorType.OtherError };
        }


        public static String GetLoginResult(String html)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            HtmlNode node = doc.DocumentNode;
            HtmlNodeCollection divs = node.SelectNodes("//div[@class='postbox']/div/p[1]");
            String result = divs != null && divs.Count > 0 ? divs[0].InnerText : "";
            if (result.IndexOf("setTimeout") != -1)
            {
                result = result.Substring(0, result.IndexOf("setTimeout"));
            }
            return result;
        }

        public static void Logout(int? timeout = null, string userAgent = null, CookieCollection cookie = null)
        {
            var codeResquest = HttpHelper.Post(String.Format(logoutUrl, formhash),
                new Dictionary<string, string>() { },
                Encoding.UTF8, Encoding.UTF8, timeout, userAgent, cookie ?? Cookies, loginFormUrl);
        }

        public static String GetUserId(String html)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            HtmlNode node = doc.DocumentNode;
            HtmlNodeCollection a = node.SelectNodes("//cite/a[1]");
            String result = a != null && a.Count > 0 ? a[0].Attributes[0].Value : "0";
            if (result.IndexOf("uid") != -1)
            {
                result = result.Substring(result.IndexOf("uid") + 4);
            }
            return result;
        }

        public static UserInfo GetUserInfo(int? timeout = null, string userAgent = null, CookieCollection cookie = null)
        {
            Program.mainForm.ShowMessage("开始获取用户信息...", false);
            var codeResquest = HttpHelper.Post(String.Format(userSpaceHref, uid),
                new Dictionary<string, string>() {}, 
                Encoding.UTF8, Encoding.UTF8, timeout, userAgent, cookie ?? Cookies, loginFormUrl);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(codeResquest);
            HtmlNode node = doc.DocumentNode;
            user = new UserInfo();
            user.name = uname;
            user.uid = uid;
            user.sex = replaceChar(node.SelectNodes("//table[@class='formtable'][1]/tr/td[1]")[0].InnerText);
            user.emailOrQQ = node.SelectNodes("//table[@class='formtable'][2]/tr/th")[0].InnerText 
                + replaceChar(node.SelectNodes("//table[@class='formtable'][2]/tr/td/a[1]")[0].InnerText);
            user.account = account;
            user.userGroup = node.SelectNodes("//h3[@class='blocktitle lightlink'][1]/a[1]")[0].InnerHtml;

            user.headImg = replaceImg(node.SelectNodes("//div[@class='avatar']/img")[0].Attributes[0].Value);
            user.levelImg = replaceImg(node.SelectNodes("//div[@class='itemtitle s_clear']/h1/img")[0].Attributes[0].Value);

            user.registerDate = node.SelectNodes("//ul[@class='commonlist right']/li[1]")[0].InnerHtml;
            user.shangTime = node.SelectNodes("//ul[@class='commonlist right']/li[2]")[0].InnerHtml;
            user.lastFabiao = node.SelectNodes("//ul[@class='commonlist right']/li[3]")[0].InnerHtml;
            user.registerIP = node.SelectNodes("//ul[@class='commonlist right']/li[4]")[0].InnerHtml;
            user.shangIP = node.SelectNodes("//ul[@class='commonlist right']/li[5]")[0].InnerHtml;

            user.fatieLevel = node.SelectNodes("//ul[@class='commonlist']/li[1]")[0].InnerText;
            user.fatieLevelImg = replaceImg(node.SelectNodes("//ul[@class='commonlist']/li[1]/img")[0].Attributes[0].Value);
            user.readPri = node.SelectNodes("//ul[@class='commonlist']/li[2]")[0].InnerHtml;
            user.tiezi = node.SelectNodes("//ul[@class='commonlist']/li[3]")[0].InnerHtml;
            user.avgTiezi = node.SelectNodes("//ul[@class='commonlist']/li[4]")[0].InnerHtml;
            user.jinghua = node.SelectNodes("//ul[@class='commonlist']/li[5]")[0].InnerHtml;
            user.pageCount = node.SelectNodes("//ul[@class='commonlist']/li[6]")[0].InnerHtml;

            int count = node.SelectNodes("//h3[@class='blocktitle lightlink']").Count;

            user.onlineTime = node.SelectNodes("//div[@id='profilecontent']/p[" + (count -1) + "]")[0].InnerText;
            user.onlineImg = replaceImg(node.SelectNodes("//div[@id='profilecontent']/p[" + (count - 1) + "]/img")[0].Attributes[0].Value);

            user.dajifen = parseAttr(node.SelectNodes("//h3[@class='blocktitle lightlink'][" + count + "]")[0].InnerHtml);
            String strs = node.SelectNodes("//div[@id='profilecontent']/p[" + count + "]")[0].InnerText;
            if (!String.IsNullOrEmpty(strs) && strs.IndexOf(",") != -1)
            {
                String[] str = strs.Split(',');
                user.expri = parseAttr(str[0]);
                user.godle = parseAttr(str[1]);
                user.jifen = parseAttr(str[2]);
            }

            HtmlNodeCollection nodeColl = node.SelectNodes("//h3[@class='blocktitle lightlink'][1]/img");
            if (nodeColl != null && nodeColl.Count > 0)
            {
                String[] pics = new String[nodeColl.Count];
                for (int i = 0; i < nodeColl.Count; i++)
                {
                    pics[i] = replaceImg(nodeColl[i].Attributes[0].Value);
                }
                user.levelPic = pics;
            }

            nodeColl = node.SelectNodes("//div[@class='content']/img");
            if (nodeColl != null && nodeColl.Count > 0)
            {
                String[] pics = new String[nodeColl.Count];
                for (int i = 0; i < nodeColl.Count; i++)
                {
                    pics[i] = replaceImg(nodeColl[i].Attributes[0].Value);
                }
                user.xuanzhangImg = pics;
            }

            Program.mainForm.ShowMessage("完成");
            return user;
        }

        public static String replaceChar(String str)
        {
            if(!String.IsNullOrEmpty(str)){
                Regex r = new Regex(@"&#64;");
                str = r.Replace(str, "@");
                r = new Regex(@"&#46;");
                str = r.Replace(str, ".");
                str = str.Replace("\n", "");
            }
            return str;
        }

        public static String replaceImg(String str)
        {
            if (!String.IsNullOrEmpty(str))
            {
                str = str.Replace("../../", mainUrl);
                str = str.Replace("./", mainUrl);
            }
            return str;
        }

        public static int parseAttr(String str)
        {
            int result = 0;
            if (!String.IsNullOrEmpty(str) && str.IndexOf(":") != -1)
            {
                String[] strs = str.Split(':');
                result =  int.Parse(strs[1].Replace(" ", ""));
            }
            return result;
        }

    }

    

    class UserInfo
    {
        public String account { get; set; }

        public String uid { get; set; }

        public String name { get; set; }

        public String sex { get; set; }

        public String emailOrQQ { get; set; }

        public String headImg { get; set; }

        public String registerDate { get; set; }

        public String userGroup { get; set; }
        public String shangTime { get; set; }
        public String lastFabiao { get; set; }
        public String registerIP { get; set; }
        public String shangIP { get; set; }

        public String fatieLevel { get; set; }
        public String readPri { get; set; }
        public String tiezi { get; set; }
        public String avgTiezi { get; set; }
        public String jinghua { get; set; }
        public String pageCount { get; set; }

        public String onlineTime { get; set; }

        public String onlineImg { get; set; }

        public String fatieLevelImg { get; set; }

        public String timesImg { get; set; }

        public int dajifen { get; set; }

        public int jifen { get; set; }
        public int expri { get; set; }
        public int godle { get; set; }

        public String levelImg { get; set; }

        public String[] levelPic { get; set; }

        public String[] xuanzhangImg { get; set; }

    }
}
