using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.IO;
using System.Threading;
using System.Security.Cryptography;

namespace YisinTick
{
    class TickCute
    {

        public static string GetRegeditInfo(string info)
        {
            string key = "";
           String logs = readFileStr();

            if (logs != null)
            {
                key = MyEncrypt.DecryptB(logs);
            }
            else
            {
                key = CreateKey(info, "0001");
                WriteLogsToFile(key);
                ThreadPool.QueueUserWorkItem((a) => {
                    String s = "我的机器码：" + info + "\n"
                        + CommonUtil.GetWindowInfo();
                    //CommonUtil.SendEMail("yisin2014@163.com", info, "214175590@qq.com", "隐心", "隐心抢票安装报告", s, "", "smtp.163.com", "yisin2014@163.com", "");
                });
            }
            return key;
        }

        public static string CreateKey(string info, string s)
        {
            string mw16 = MyEncrypt.GetMd5_16(info, true);
            string mw32 = MyEncrypt.GetMD5(info);
            List<int> i41 = MyEncrypt.GenerateNumber(4, 16);
            List<int> i42 = MyEncrypt.GenerateNumber(4, 32);
            mw16 = MyEncrypt.getRjCode(i41, MyEncrypt.split(mw16));
            mw32 = MyEncrypt.getRjCode(i42, MyEncrypt.split(mw32));
            string date = DateTime.Now.Year + bu0(DateTime.Now.Month) + bu0(DateTime.Now.Day);
            return ListToString(i41) + MyEncrypt.FanZhuan(MyEncrypt.getNumChar(s)) + ListToString(i42) + MyEncrypt.FanZhuan(MyEncrypt.EncryptA(date)) + mw16 + mw32;
        }

        public static int GetUseTick(string info, string key, bool isdate)
        {
            int count = 0, error = 0;
            if (key.Length > 50)
            {
                string mw16 = MyEncrypt.GetMd5_16(info, true);
                string mw32 = MyEncrypt.GetMD5(info);
                string qian8 = key.Substring(0, 8);
                string num = MyEncrypt.FanZhuan(key.Substring(8, 4));
                string zhong8 = key.Substring(12, 8);
                string date = MyEncrypt.DecryptB(MyEncrypt.FanZhuan(key.Substring(20, key.Length - 28)));
                mw16 = MyEncrypt.getRjCode(NumerStrToList(qian8), MyEncrypt.split(mw16));
                mw32 = MyEncrypt.getRjCode(NumerStrToList(zhong8), MyEncrypt.split(mw32));
                string str1 = mw16 + mw32;
                string str2 = key.Substring(key.Length - 8, 8);
                string today = DateTime.Now.Year + bu0(DateTime.Now.Month) + bu0(DateTime.Now.Day);
                Regex r = new Regex(@"^[0-9]*$");
                int d1 = int.Parse(date);
                int d2 = int.Parse(today);
                if (!r.IsMatch(qian8))
                {
                    error++;
                }
                if (!r.IsMatch(zhong8))
                {
                    error++;
                }
                if (!r.IsMatch(date))
                {
                    error++;
                }
                if (!str1.Equals(str2))
                {
                    error++;
                }
                if (d1 > d2)
                {
                    error++;
                }
                if (isdate && (d2 - d1) > 1)
                {
                    error++;
                }
                if (error == 0)
                {
                    num = MyEncrypt.CharToNumerStr(num);
                    count = int.Parse(num);
                }
                else
                {
                    count = count - error;
                }
            }
            return count;
        }

        public static void ChangeTickCount(int count)
        {
            Form1.TickCount = Form1.TickCount + count;
            string key = CreateKey(Form1.MachineCode, NumBuStr(Form1.TickCount));
            WriteLogsToFile(key);
            Program.mainForm.SetTickText();
        }

        public static String readFileStr()
        {
            string strLine, Line = null;
            try
            {
                string dir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                dir = dir + "\\sys\\a\\";
                string inFile = dir + "Log.dat";
                //DESFile.EncryptFile(inFile, outFile, "yisin2014");//加密文件
                FileStream aFile = new FileStream(inFile, FileMode.OpenOrCreate);
                StreamReader sr = new StreamReader(aFile);
                strLine = sr.ReadLine();
                //Read data in line by line 这个兄台看的懂吧~一行一行的读取     
                while (strLine != null && strLine.Length > 10520)
                {
                    Line = strLine.Substring(5295, strLine.Length - 5295 - 5215);
                    strLine = sr.ReadLine();
                }
                sr.Close();               
            }
            catch (IOException ex)
            {
                return Line;
            }
            return Line;
        }

        public static List<String> readHistoryStr()
        {
            string strLine, Line = "";
            List<String> result = new List<String>();
            try
            {
                string dir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                dir = dir + "\\sys\\a\\";
                string inFile = dir + "Log2.dat";
                //DESFile.EncryptFile(inFile, outFile, "yisin2014");//加密文件
                FileStream aFile = new FileStream(inFile, FileMode.OpenOrCreate);
                StreamReader sr = new StreamReader(aFile);
                strLine = sr.ReadLine();
                //Read data in line by line 这个兄台看的懂吧~一行一行的读取     
                while (strLine != null && strLine.Length > 7520)
                {
                    Line = strLine.Substring(3295, strLine.Length - 3295 - 4215);
                    result.Add(Line);
                    strLine = sr.ReadLine();
                }
                sr.Close();
            }
            catch (IOException ex)
            {
                return result;
            }
            return result;
        }

        public static void WriteLogsToFile(string key)
        {
            string strLine = "";
            try
            {
                string dir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                dir = dir + "\\sys\\a\\";
                string inFile = dir + "Log.dat";
                key = MyEncrypt.EncryptA(key);
                strLine = MyEncrypt.GetStrByLength(5295) + key + MyEncrypt.GetStrByLength(5215);
                FileStream aFile = new FileStream(inFile, FileMode.OpenOrCreate);
                StreamWriter sw = new StreamWriter(aFile);
                sw.WriteLine(strLine);
                sw.Close();;
            }
            catch (IOException ex)
            {
            }
        }

        public static void WriteHistoryToFile(string key)
        {
            string strLine = "", Line = "";
            try
            {
                key = MyEncrypt.EncryptA(key);
                string dir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                dir = dir + "\\sys\\a\\";
                string inFile = dir + "Log2.dat";
                StreamReader sr = new StreamReader(inFile);
                strLine = sr.ReadLine();
                while (strLine != null && strLine.Length > 7520)
                {
                    Line += strLine + "\r\n";
                    strLine = sr.ReadLine();
                }
                sr.Close();
                strLine = MyEncrypt.GetStrByLength(3295) + key + MyEncrypt.GetStrByLength(4215);
                FileStream aFile = new FileStream(inFile, FileMode.OpenOrCreate);
                StreamWriter sw = new StreamWriter(aFile);
                Line += strLine + "\r\n";
                sw.WriteLine(Line);
                sw.Close(); ;
            }
            catch (IOException ex)
            {
            }
        }

        public static void CreateDir()
        {
            string dir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (!Directory.Exists(dir + "\\sys\\a\\"))//判断是否存在
            {
                Directory.CreateDirectory(dir + "\\sys\\a\\");
                File.SetAttributes(dir + "\\sys\\", FileAttributes.Hidden);
                File.SetAttributes(dir + "\\sys\\a\\", FileAttributes.Hidden);
            }
        }

        public static List<int> NumerStrToList(string str)
        {
            List<int> list = new List<int>();
            while (str != "")
            {
                list.Add(int.Parse(str.Substring(0, 2)));
                str = str.Substring(2);
            }
            return list;
        }

        public static string ListToString(List<int> list)
        {
            string re = "";
            foreach (int i in list)
            {
                re += bu0(i);
            }
            return re;
        }

        public static string bu0(int i)
        {
            string re = "";
            if (i < 10)
            {
                re = "0" + i;
            }
            else
            {
                re = "" + i;
            }
            return re;
        }

        public static string NumBuStr(int num)
        {
            string re = "";
            if(num < 10){
                re += "000" + num;
            }
            else if (num < 100)
            {
                re += "00" + num;
            }
            else if (num < 1000)
            {
                re += "0" + num;
            }
            else
            {
                re += "" + num;
            }
            return re;
        }

    }

}
