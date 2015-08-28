using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections;

namespace YisinTick
{
    [System.Runtime.Remoting.Contexts.Synchronization]
    class UnCode : UnCodebase
    {
        string dir = System.Environment.CurrentDirectory;
        public UnCode(Bitmap pic) : base(pic)
        {
        }

        public void initDictData()
        {
            

        }

        /// <summary>
        /// 识别验证码
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string getPicnum(int type)
        {
            Bitmap[] pics = getImg();
            if (pics.Length != 4)
            {
                return "????"; //分割错误
            }            
            string result = "";
            String singleChar = "";
            {
                for (int i = 0; i < 4; i++)
                {
                    if (type == 0)
                    {
                        if(ConfigStore.algorithm.Equals("1")){
                            singleChar = similarity(pics[i]);
                            result += singleChar;
                        }
                        else if (ConfigStore.algorithm.Equals("2"))
                        {
                            string code = GetSingleBmpCode(pics[i], 128);   //得到代码串
                            singleChar = similarity(code);
                            result += singleChar;
                        }
                    }
                    else
                    {
                        if (type == 1)
                        {
                            singleChar = similarity(pics[i]);
                            result += singleChar;
                        }
                        else if (type == 2)
                        {
                            string code = GetSingleBmpCode(pics[i], 128);   //得到代码串
                            singleChar = similarity(code);
                            result += singleChar;
                        }
                    }
                    //string code = GetSingleBmpCode(pics[i], 128);   //得到代码串
                    //singleChar = similarity(pics[i]);//similarity(code);
                    //result += singleChar;                    
                }
            }
           return result;
        }

        /// <summary>
        /// 根据数据点 获取相似码
        /// </summary>
        /// <returns></returns>
        public String similarity(string data)
        {
            string result = "?";
            ArrayList akeys = new ArrayList(ConfigStore.codeTable.Keys);
            Hashtable table = new Hashtable();
            VerifyEntity ve1 = null;
            char[] datas = data.ToCharArray();
            char[] keys = null;            
            foreach (string skey in akeys)
            {
                ve1 = (VerifyEntity) ConfigStore.codeTable[skey];
                keys = skey.ToCharArray();                
                int leng = keys.Length > datas.Length ? datas.Length : keys.Length;
                int count = 0;
                float simi = 0.0f;
                for (int i = 0; i < leng; i++ )
                {
                    if(keys[i].Equals(datas[i])){
                        count++;
                    }
                }
                simi = (float)count / (float)leng;
                if (simi >= 0.95)
                {
                    result = ve1.Code;
                    break;
                }
                else if (simi >= 0.60)
                {
                    if (!table.Contains(simi))
                    {
                        table.Add(simi, ve1.Code);
                    }
                }
            }
            if(result.Equals("?") && table.Count > 0){
                ArrayList simis = new ArrayList(table.Keys);
                simis.Sort();
                result = table[simis[simis.Count - 1]].ToString();
            }
            return result;
        }

        public String similarity(Bitmap img)
        {
            string result = "?";
            ArrayList akeys = new ArrayList(ConfigStore.codeTable.Keys);
            Hashtable table = new Hashtable();
            VerifyEntity ve1 = null;
            int[] imgInt1 = GetHisogram(img);
            int[] imgInt2 = null;
            Bitmap img2 = null;
            float simi = 0.0f;
            foreach (string skey in akeys)
            {
                ve1 = (VerifyEntity)ConfigStore.codeTable[skey];
                img2 = (Bitmap)Image.FromFile(dir + ve1.ImgPath);
                if(img2 != null){
                    imgInt2 = GetHisogram(img2);
                    simi = GetResult(imgInt1, imgInt2);
                    if (simi >= 0.95)
                    {
                        result = ve1.Code;
                        break;
                    }
                    else if (simi >= 0.60)
                    {
                        if (!table.Contains(simi))
                        {
                            table.Add(simi, ve1.Code);
                        }
                    }
                }         
                
            }
            if (result.Equals("?") && table.Count > 0)
            {
                ArrayList simis = new ArrayList(table.Keys);
                simis.Sort();
                result = table[simis[simis.Count - 1]].ToString();
            }
            return result;
        }

        /// <summary>
        /// 计算图像的直方图
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public int[] GetHisogram(Bitmap img)
        {
            BitmapData data = img.LockBits(new System.Drawing.Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int[] histogram = new int[256];
            unsafe
            {
                byte* ptr = (byte*)data.Scan0;
                int remain = data.Stride - data.Width * 3;
                for (int i = 0; i < histogram.Length; i++)
                {
                    histogram[i] = 0;
                }                    
                for (int i = 0; i < data.Height; i++)
                {
                    for (int j = 0; j < data.Width; j++)
                    {
                        int mean = ptr[0] + ptr[1] + ptr[2];
                        mean /= 3;
                        histogram[mean]++;
                        ptr += 3;
                    }
                    ptr += remain;
                }
            }
            img.UnlockBits(data);
            return histogram;
        }

        /// <summary>
        /// 计算相减后的绝对值
        /// </summary>
        /// <param name="firstNum"></param>
        /// <param name="secondNum"></param>
        /// <returns></returns>
        private float GetAbs(int firstNum, int secondNum)
        {
            float abs = Math.Abs((float)firstNum - (float)secondNum);
            float result = Math.Max(firstNum, secondNum);
            if (result == 0)
                result = 1;
            return abs / result;
        }

        /// <summary>
        /// 最终计算结果
        /// </summary>
        /// <param name="firstNum"></param>
        /// <param name="scondNum"></param>
        /// <returns></returns>
        public float GetResult(int[] firstNum, int[] scondNum)
        {
            float result = 0;
            int j = firstNum.Length;
            for (int i = 0; i < j; i++)
            {
                result += 1 - GetAbs(firstNum[i], scondNum[i]);
            }
            return result / j;
        }

    }
}
