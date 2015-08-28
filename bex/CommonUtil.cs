using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace YisinTick
{
    class CommonUtil
    {
        #region 发送邮件

        /// <summary>
        /// C#发送邮件函数
        /// </summary>
        /// <param name="from">发送者邮箱</param>
        /// <param name="fromer">发送人</param>
        /// <param name="to">接受者邮箱</param>
        /// <param name="toer">收件人</param>
        /// <param name="Subject">主题</param>
        /// <param name="Body">内容</param>
        /// <param name="file">附件</param>
        /// <param name="SMTPHost">smtp服务器</param>
        /// <param name="SMTPuser">邮箱</param>
        /// <param name="SMTPpass">密码</param>
        /// <returns></returns>
        public static bool SendEMail(string sfrom, string sfromer, string sto, string stoer, string sSubject, string sBody, string sfile, string sSMTPHost, string sSMTPuser, string sSMTPpass)
        {
            ////设置from和to地址
            MailAddress from = new MailAddress(sfrom, sfromer);
            MailAddress to = new MailAddress(sto, stoer);

            ////创建一个MailMessage对象
            MailMessage oMail = new MailMessage(from, to);

            //// 添加附件
            if (sfile != "")
            {
                oMail.Attachments.Add(new Attachment(sfile));
            }
            
            ////邮件标题
            oMail.Subject = sSubject;
            
            ////邮件内容
            oMail.Body = sBody;

            ////邮件格式
            oMail.IsBodyHtml = false;

            ////邮件采用的编码
            oMail.BodyEncoding = System.Text.Encoding.GetEncoding("GB2312");

            ////设置邮件的优先级为高
            oMail.Priority = MailPriority.High;

            ////发送邮件
            SmtpClient client = new SmtpClient();
            ////client.UseDefaultCredentials = false; 
            client.Host = sSMTPHost;
            client.Credentials = new NetworkCredential(sSMTPuser, sSMTPpass);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            try
            {
                client.Send(oMail);
                return true;
            }
            catch (Exception err)
            {
                return false;
            }
            finally
            {
                ////释放资源
                oMail.Dispose();
            }

        }

        #endregion


        public static String GetWindowInfo()
        {
            String str = "系统信息：\n";
            try { 
                //1.获取操作系统版本(PC,PDA均支持)
                str += "操作系统版本：" + Environment.OSVersion;
            } catch (Exception e) { }

            try { 
                //2.获取应用程序当前目录(PC支持)
                str += "，\n应用程序当前目录：" + Environment.CurrentDirectory;
            } catch (Exception e) { }

            string dri = "";
            try { 
                //3.列举本地硬盘驱动器(PC支持)
                string [] strDrives = Environment.GetLogicalDrives();                
                foreach(string strDrive in strDrives) {
                    dri += strDrive + " | "; 
                }
                str += "，\n拥有磁盘：" + dri; 
            } catch (Exception e) { }

            try { 
                //4.获取.Net Framework版本号(PC,PDA均支持)
                str += "，\n.Net Framework版本号：" + Environment.Version;
            } catch (Exception e) { }

            try { 
                //5.获取机器名(PC支持)
                str += "，\n机器名：" + Environment.MachineName;
            }
            catch (Exception e) { }

            try { 
                //7.获取处理器个数(PC支持)
                str += "，\n处理器个数：" + Environment.ProcessorCount;
            } catch (Exception e) { }

            try { 
             //8.获取当前登录用户(PC支持)
                str += "，\n当前登录用户：" + Environment.UserName;
            } catch (Exception e) { }

            try { 
                //9.获取系统保留文件夹路径(PC,PDA均支持)
                str += "，\n应用程序当前目录：" + Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            } catch (Exception e) { }
            //Environment.SpecialFolder对象提供系统保留文件夹标识，例如:Environment.SpecialFolder.Desktop表示桌面文件夹的路径。

            try { 
                //10.获取命令行参数(PC支持)
                string [] strParams = Environment.GetCommandLineArgs();
                dri = "";
                foreach(string strParam in strParams){
                    dri += strParam + " | ";
                }
                str += "，\n命令行参数：" + dri;
            } catch (Exception e) { }

            try { 
                //11.获取系统环境变量(PC支持)
                System.Collections.IDictionary dict = Environment.GetEnvironmentVariables();
                str += "，\n系统环境变量：" + dict["Path"].ToString();
            } catch (Exception e) { }

            try { 
                //12.获取域名(PC支持)
                str += "，\n域名：" + Environment.UserDomainName;
            } catch (Exception e) { }

            try{
                //13.获取截至到当前时间，操作系统启动的毫秒数(PDA,PC均支持)
                str += "，\n截至到当前时间操作系统启动的毫秒数：" + ((float)Environment.TickCount/(float)(1000 * 60)) + "分钟";
            } catch (Exception e) { }

            try { 
                //14.映射到当前进程的物理内存数
                str += "，\n映射到当前进程的物理内存数：" + ((float)Environment.WorkingSet / (float)(1024 * 1024)) + "MB";
            } catch (Exception e) { }
            return str;
        }


        #region 文件解压缩

        /// <summary>
        /// 压缩
        /// </summary>
        /// <param name="strFile"></param>
        /// <param name="strZip"></param>
        public static void ZipFile(string dir, string strZip, List<String> files)
        {
            if (dir[dir.Length - 1] != Path.DirectorySeparatorChar)
                dir += Path.DirectorySeparatorChar;
            ZipOutputStream s = new ZipOutputStream(File.Create(strZip));
            s.SetLevel(6); // 0 - store only to 9 - means best compression
            zip(dir, s, dir, files);
            s.Finish();
            s.Close();
        }

        /// <summary>
        /// 压缩
        /// </summary>
        /// <param name="strFile"></param>
        /// <param name="s"></param>
        /// <param name="staticFile"></param>
        private static void zip(string strFile, ZipOutputStream s, string staticFile, List<String> files)
        {
            if (strFile[strFile.Length - 1] != Path.DirectorySeparatorChar) strFile += Path.DirectorySeparatorChar;
            Crc32 crc = new Crc32();
            string[] filenames = Directory.GetFileSystemEntries(strFile);
            foreach (string file in filenames)
            {
                if (Directory.Exists(file))
                {
                    zip(file, s, staticFile, null);
                }
                else // 否则直接压缩文件
                {
                    //打开压缩文件
                    FileStream fs = File.OpenRead(file);

                    byte[] buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);
                    string tempfile = file.Substring(staticFile.LastIndexOf("\\") + 1);
                    ZipEntry entry = new ZipEntry(tempfile);

                    entry.DateTime = DateTime.Now;
                    entry.Size = fs.Length;
                    fs.Close();
                    crc.Reset();
                    crc.Update(buffer);
                    entry.Crc = crc.Value;
                    s.PutNextEntry(entry);

                    s.Write(buffer, 0, buffer.Length);
                }
            }

            if(files != null){
                foreach (string file in files)
                {
                    //打开压缩文件
                    FileStream fs = File.OpenRead(file);

                    byte[] buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);
                    string tempfile = file.Substring(file.LastIndexOf("\\"));
                    ZipEntry entry = new ZipEntry(tempfile);

                    entry.DateTime = DateTime.Now;
                    entry.Size = fs.Length;
                    fs.Close();
                    crc.Reset();
                    crc.Update(buffer);
                    entry.Crc = crc.Value;
                    s.PutNextEntry(entry);

                    s.Write(buffer, 0, buffer.Length);
                }
            }
        }

        /// <summary>
        /// 解压
        /// </summary>
        /// <param name="TargetFile"></param>
        /// <param name="fileDir"></param>
        /// <returns></returns>
        public static string unZipFile(string TargetFile, string fileDir)
        {
            string rootFile = " ";
            try
            {
                //读取压缩文件(zip文件)，准备解压缩
                ZipInputStream s = new ZipInputStream(File.OpenRead(TargetFile.Trim()));
                ZipEntry theEntry;
                string path = fileDir;
                //解压出来的文件保存的路径

                string rootDir = " ";
                //根目录下的第一个子文件夹的名称
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    rootDir = Path.GetDirectoryName(theEntry.Name);
                    //得到根目录下的第一级子文件夹的名称
                    if (rootDir.IndexOf("\\") >= 0)
                    {
                        rootDir = rootDir.Substring(0, rootDir.IndexOf("\\") + 1);
                    }
                    string dir = Path.GetDirectoryName(theEntry.Name);
                    //根目录下的第一级子文件夹的下的文件夹的名称
                    string fileName = Path.GetFileName(theEntry.Name);
                    //根目录下的文件名称
                    if (dir != " ")
                    //创建根目录下的子文件夹,不限制级别
                    {
                        if (!Directory.Exists(fileDir + "\\" + dir))
                        {
                            path = fileDir + "\\" + dir;
                            //在指定的路径创建文件夹
                            Directory.CreateDirectory(path);
                        }
                    }
                    else if (dir == " " && fileName != "")
                    //根目录下的文件
                    {
                        path = fileDir;
                        rootFile = fileName;
                    }
                    else if (dir != " " && fileName != "")
                    //根目录下的第一级子文件夹下的文件
                    {
                        if (dir.IndexOf("\\") > 0)
                        //指定文件保存的路径
                        {
                            path = fileDir + "\\" + dir;
                        }
                    }

                    if (dir == rootDir)
                    //判断是不是需要保存在根目录下的文件
                    {
                        path = fileDir + "\\" + rootDir;
                    }

                    //以下为解压缩zip文件的基本步骤
                    //基本思路就是遍历压缩文件里的所有文件，创建一个相同的文件。
                    if (fileName != String.Empty)
                    {
                        FileStream streamWriter = File.Create(path + "\\" + fileName);

                        int size = 2048;
                        byte[] data = new byte[2048];
                        while (true)
                        {
                            size = s.Read(data, 0, data.Length);
                            if (size > 0)
                            {
                                streamWriter.Write(data, 0, size);
                            }
                            else
                            {
                                break;
                            }
                        }

                        streamWriter.Close();
                    }
                }
                s.Close();

                return rootFile;
            }
            catch (Exception ex)
            {
                return "1; " + ex.Message;
            }
        }  

        #endregion


        public static bool CreateDir(String dir)
        {
            DirectoryInfo d = new DirectoryInfo(dir);
            if (!d.Exists)
            {
                d.Create();
            }
            return true;
        }


        public static bool DeleteDir(String path)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            if (dir.Exists)
            {
                DirectoryInfo[] childs = dir.GetDirectories();
                foreach (DirectoryInfo child in childs)
                {
                    child.Delete(true);
                }
                dir.Delete(true);
            }
            return true;
        }


    }
}
