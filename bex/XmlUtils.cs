using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Collections;

namespace YisinTick
{
    class XmlUtils
    {

        public static string xmlPath = System.Environment.CurrentDirectory + "\\data\\";

        public static void loadLibarayXml(String xmlFile, String libId){
            Program.mainForm.ShowMessage("正在加载特征码数据...", false);
            XmlDocument xmlDoc = null;
            XmlNode root = null;
            XmlNodeList nodeList = null;            
            XmlElement xe = null;
            VerifyEntity ve = null;
            if (xmlFile == null || libId == null)
            {
                foreach (Libaray lib in ConfigStore.libarays)
                {
                    xmlDoc = new XmlDocument();
                    xmlDoc.Load(xmlPath + lib.id + "\\data.xml");
                    // root节点
                    root = xmlDoc.SelectSingleNode("datas");
                    //获取到所有<datas>的子节点
                    nodeList = root.ChildNodes;
                    //遍历所有子节点
                    foreach (XmlNode xn in nodeList)
                    {
                        xe = (XmlElement)xn;
                        String title = "";
                        if (xe.Name.Equals("data"))
                        {
                            ve = new VerifyEntity();
                            if (!ConfigStore.codeTable.ContainsKey(xe.InnerText))
                            {
                                ve.Code = xe.GetAttribute("code");
                                ve.ImgPath = xe.GetAttribute("imgPath");
                                ve.Data = xe.InnerText;
                                ve.LibId = lib.id;
                                ve.LibName = title;
                                ConfigStore.codeTable.Add(ve.Data, ve);
                            }
                        }
                        else if (xe.Name.Equals("title"))
                        {
                            title = xe.InnerText;
                        }
                    }
                }
            }
            else
            {
                xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlFile);
                // root节点
                root = xmlDoc.SelectSingleNode("datas");
                //获取到所有<datas>的子节点
                nodeList = root.ChildNodes;
                //遍历所有子节点
                foreach (XmlNode xn in nodeList)
                {
                    xe = (XmlElement)xn;
                    String title = "";
                    if (xe.Name.Equals("data"))
                    {
                        ve = new VerifyEntity();
                        if (!ConfigStore.codeTable.ContainsKey(xe.InnerText))
                        {
                            ve.Code = xe.GetAttribute("code");
                            ve.ImgPath = xe.GetAttribute("imgPath");
                            ve.Data = xe.InnerText;
                            ve.LibId = libId;
                            ve.LibName = title;
                            ConfigStore.codeTable.Add(ve.Data, ve);
                        }
                    }
                    else if (xe.Name.Equals("title"))
                    {
                        title = xe.InnerText;
                    }
                }
            }
            
            Program.mainForm.ShowMessage("完成");
        }

       public static void WriteLibarayXml()
       {
           Program.mainForm.ShowMessage("正在保存特征码数据...", false);
           XmlDocument myDoc = null;
           XmlElement root = null;
           XmlElement ele = null;
           ArrayList akeys = new ArrayList(ConfigStore.codeTable.Keys);
           VerifyEntity ve = null;
           foreach (Libaray lib in ConfigStore.libarays)
           {
               //初始化XML文档操作类
               myDoc = new XmlDocument();
               //加载XML文件
               myDoc.Load(xmlPath + lib.id + "\\data.xml");
               //将节点添加到文档中
               root = myDoc.DocumentElement;
               root.RemoveAll();

               //
               ele = myDoc.CreateElement("title");
               ele.InnerText = lib.name;
               //在节点中添加元素
               root.AppendChild(ele);

               foreach (string skey in akeys)
               {
                   ve = (VerifyEntity)ConfigStore.codeTable[skey];
                   if(ve.LibId.Equals(lib.id)){
                       ele = myDoc.CreateElement("data");
                       ele.SetAttribute("code", ve.Code);
                       ele.SetAttribute("imgPath", ve.ImgPath);
                       ele.InnerText = skey;
                       //在节点中添加元素
                       root.AppendChild(ele);
                   }                   
               }
               //保存
               myDoc.Save(xmlPath + lib.id + "\\data.xml");
           }
           Program.mainForm.ShowMessage("完成");         
       }

       public static void CreateEmptyXml(String xmlPath, String rootName)
       {
           XmlDocument xmlDoc = new XmlDocument();
           //创建类型声明节点
           XmlNode node = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "");
           xmlDoc.AppendChild(node);
           //创建根节点
           XmlNode root = xmlDoc.CreateElement(rootName);
           xmlDoc.AppendChild(root);
           try
           {
               xmlDoc.Save(xmlPath);
           }
           catch (Exception e)
           {
               //显示错误信息
               Console.WriteLine(e.Message);
           }
       }

    }
}
