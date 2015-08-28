using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Web;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace YisinTick
{
    class GenerateVerify
    {

        #region 验证码长度(默认4个验证码的长度)
        int length = 4;
        public int Length
        {
            get { return length; }
            set { length = value; }
        }
        #endregion

        #region 验证码字体大小(为了显示扭曲效果，默认40像素，可以自行修改)
        int fontSize = 14;
        public int FontSize
        {
            get { return fontSize; }
            set { fontSize = value; }
        }
        #endregion

        #region 边框补(默认1像素)
        int padding = 1;
        public int Padding
        {
            get { return padding; }
            set { padding = value; }
        }
        #endregion

        #region 是否输出燥点(默认不输出)
        bool chaos = false;
        public bool Chaos
        {
            get { return chaos; }
            set { chaos = value; }
        }
        #endregion

        #region 输出燥点的颜色(默认灰色)
        Color chaosColor = Color.LightGray;
        public Color ChaosColor
        {
            get { return chaosColor; }
            set { chaosColor = value; }
        }
        #endregion

        #region 自定义背景色(默认白色)
        Color backgroundColor = Color.White;
        public Color BackgroundColor
        {
            get { return backgroundColor; }
            set { backgroundColor = value; }
        }
        #endregion

        #region 自定义随机颜色数组
        Color[] colors = { Color.Black, Color.Red, Color.DarkBlue, Color.Green, Color.Orange, Color.Brown, Color.DarkCyan, Color.Purple };
        public Color[] Colors
        {
            get { return colors; }
            set { colors = value; }
        }
        #endregion

        #region 自定义字体数组
        string[] fonts = { "Arial", "Georgia" };
        public string[] Fonts
        {
            get { return fonts; }
            set { fonts = value; }
        }
        #endregion

        #region 自定义随机码字符串序列(使用逗号分隔)
        string codeSerial = "0,1,2,3,4,5,6,7,8,9,a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z";
        public string CodeSerial
        {
            get { return codeSerial; }
            set { codeSerial = value; }
        }
        #endregion

        #region 产生波形滤镜效果

        private const double PI = 3.1415926535897932384626433832795;
        private const double PI2 = 6.283185307179586476925286766559;

        /// <summary>
        /// 正弦曲线Wave扭曲图片（Edit By 51aspx.com）
        /// </summary>
        /// <param name="srcBmp">图片路径</param>
        /// <param name="bXDir">如果扭曲则选择为True</param>
        /// <param name="nMultValue">波形的幅度倍数，越大扭曲的程度越高，一般为3</param>
        /// <param name="dPhase">波形的起始相位，取值区间[0-2*PI)</param>
        /// <returns></returns>
        public System.Drawing.Bitmap TwistImage(Bitmap srcBmp, bool bXDir, double dMultValue, double dPhase)
        {
            System.Drawing.Bitmap destBmp = new Bitmap(srcBmp.Width, srcBmp.Height);

            // 将位图背景填充为白色
            System.Drawing.Graphics graph = System.Drawing.Graphics.FromImage(destBmp);
            graph.FillRectangle(new SolidBrush(System.Drawing.Color.White), 0, 0, destBmp.Width, destBmp.Height);
            graph.Dispose();

            double dBaseAxisLen = bXDir ? (double)destBmp.Height : (double)destBmp.Width;

            for (int i = 0; i < destBmp.Width; i++)
            {
                for (int j = 0; j < destBmp.Height; j++)
                {
                    double dx = 0;
                    dx = bXDir ? (PI2 * (double)j) / dBaseAxisLen : (PI2 * (double)i) / dBaseAxisLen;
                    dx += dPhase;
                    double dy = Math.Sin(dx);

                    // 取得当前点的颜色
                    int nOldX = 0, nOldY = 0;
                    nOldX = bXDir ? i + (int)(dy * dMultValue) : i;
                    nOldY = bXDir ? j : j + (int)(dy * dMultValue);

                    System.Drawing.Color color = srcBmp.GetPixel(i, j);
                    if (nOldX >= 0 && nOldX < destBmp.Width
                     && nOldY >= 0 && nOldY < destBmp.Height)
                    {
                        destBmp.SetPixel(nOldX, nOldY, color);
                    }
                }
            }

            return destBmp;
        }
        #endregion

        #region 生成校验码图片
        public Bitmap CreateImageCode(string code)
        {
            int fSize = FontSize;
            int fWidth = fSize + Padding;

            int imageWidth = (int)(code.Length * fWidth) + 4 + Padding * 2;
            int imageHeight = fSize * 2 + Padding;

            System.Drawing.Bitmap image = new System.Drawing.Bitmap(imageWidth, imageHeight);

            Graphics g = Graphics.FromImage(image);

            g.Clear(BackgroundColor);

            Random rand = new Random();

            //给背景添加随机生成的燥点
            /**if (this.Chaos)
            {
                Pen pen = new Pen(ChaosColor, 0);
                int c = Length * 10;
                for (int i = 0; i < c; i++)
                {
                    int x = rand.Next(image.Width);
                    int y = rand.Next(image.Height);
                    g.DrawRectangle(pen, x, y, 1, 1);
                }
            }**/

            int left = 0, top = 0, top1 = 1, top2 = 1;
            int n1 = (imageHeight - FontSize - Padding * 2);
            int n2 = n1 / 4;
            top1 = n2;
            top2 = n2 * 2;

            Font f;
            Brush b;

            int cindex, findex;
            //随机字体和颜色的验证码字符
            findex = rand.Next(Fonts.Length - 1);
            cindex = rand.Next(Colors.Length - 1);
            f = new System.Drawing.Font(Fonts[findex], fSize, System.Drawing.FontStyle.Bold);
            b = new System.Drawing.SolidBrush(Colors[cindex]);

            for (int i = 0; i < code.Length; i++)
            {
                if (i % 2 == 1)
                {
                    top = top2;
                }
                else
                {
                    top = top1;
                }
                left = i * fWidth;

                g.DrawString(code.Substring(i, 1), f, b, left, top);
                float f2 = (float)rand.Next(10);
                image = KiRotate(image, f2);
            }

            //画一个边框 边框颜色为Color.Gainsboro
            g.DrawRectangle(new Pen(Color.Gainsboro, 0), 0, 0, image.Width - 1, image.Height - 1);
            g.Dispose();

            //产生波形（Add By 51aspx.com）
            //image = TwistImage(image, true, 8, 4);           

            return image;
        }
        #endregion

        #region 将创建好的图片输出到页面
        public Bitmap CreateImageOnPage(string code)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            Bitmap image = this.CreateImageCode(code);
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            return image;
        }
        #endregion

        #region 生成随机字符码
        public string CreateVerifyCode(int codeLen)
        {
            if (codeLen == 0)
            {
                codeLen = Length;
            }
            string[] arr = CodeSerial.Split(',');
            string code = "";
            int randValue = -1;
            Random rand = new Random(unchecked((int)DateTime.Now.Ticks));
            for (int i = 0; i < codeLen; i++)
            {
                randValue = rand.Next(0, arr.Length - 1);
                code += arr[randValue];
            }
            return code;
        }
        public string CreateVerifyCode()
        {
            return CreateVerifyCode(0);
        }
        #endregion


        /// <summary>
        /// 任意角度旋转
        /// </summary>
        /// <param name="bmp">原始图Bitmap</param>
        /// <param name="angle">旋转角度</param>
        /// <param name="bkColor">背景色</param>
        /// <returns>输出Bitmap</returns>
        public Bitmap KiRotate(Bitmap bmp, float angle)
        {
            int w = bmp.Width + 2;
            int h = bmp.Height + 2;
            PixelFormat pf;
            
                pf = PixelFormat.Format32bppArgb;
           

            Bitmap tmp = new Bitmap(w, h, pf);
            Graphics g = Graphics.FromImage(tmp);
            //g.Clear(bkColor);
            g.DrawImageUnscaled(bmp, 1, 1);
            g.Dispose();

            GraphicsPath path = new GraphicsPath();
            path.AddRectangle(new RectangleF(0f, 0f, w, h));
            Matrix mtrx = new Matrix();
            mtrx.Rotate(angle);
            RectangleF rct = path.GetBounds(mtrx);

            Bitmap dst = new Bitmap((int)rct.Width, (int)rct.Height, pf);
            g = Graphics.FromImage(dst);
            //g.Clear(bkColor);
            g.TranslateTransform(-rct.X, -rct.Y);
            g.RotateTransform(angle);
            g.InterpolationMode = InterpolationMode.HighQualityBilinear;
            g.DrawImageUnscaled(tmp, 0, 0);
            g.Dispose();

            tmp.Dispose();

            return dst;
        }

























        public enum RandomStringMode
        {
            /// <summary>
            /// 小写字母
            /// </summary>
            LowerLetter,
            /// <summary>
            /// 大写字母
            /// </summary>
            UpperLetter,
            /// <summary>
            /// 混合大小写字母
            /// </summary>
            Letter,
            /// <summary>
            /// 数字
            /// </summary>
            Digital,
            /// <summary>
            /// 混合数字与大小字母
            /// </summary>
            Mix
        }
        
        public string GenerateRandomString(int length, RandomStringMode mode)
        {
            string rndStr = string.Empty;
            if (length == 0)
                return rndStr;

            //以数组方式候选字符，可以更方便的剔除不要的字符，如数字 0 与字母 o
            char[] digitals = new char[10] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            char[] lowerLetters = new char[26] {
                'a', 'b', 'c', 'd', 'e', 'f', 'g', 
                'h', 'i', 'j', 'k', 'l', 'm', 'n', 
                'o', 'p', 'q', 'r', 's', 't', 
                'u', 'v', 'w', 'x', 'y', 'z' };
            char[] upperLetters = new char[26] {
                'A', 'B', 'C', 'D', 'E', 'F', 'G', 
                'H', 'I', 'J', 'K', 'L', 'M', 'N', 
                'O', 'P', 'Q', 'R', 'S', 'T', 
                'U', 'V', 'W', 'X', 'Y', 'Z' };
            char[] letters = new char[52]{
                'a', 'b', 'c', 'd', 'e', 'f', 'g', 
                'h', 'i', 'j', 'k', 'l', 'm', 'n', 
                'o', 'p', 'q', 'r', 's', 't', 
                'u', 'v', 'w', 'x', 'y', 'z',
                'A', 'B', 'C', 'D', 'E', 'F', 'G', 
                'H', 'I', 'J', 'K', 'L', 'M', 'N', 
                'O', 'P', 'Q', 'R', 'S', 'T', 
                'U', 'V', 'W', 'X', 'Y', 'Z' };
            char[] mix = new char[62]{
                '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
                'a', 'b', 'c', 'd', 'e', 'f', 'g', 
                'h', 'i', 'j', 'k', 'l', 'm', 'n', 
                'o', 'p', 'q', 'r', 's', 't', 
                'u', 'v', 'w', 'x', 'y', 'z',
                'A', 'B', 'C', 'D', 'E', 'F', 'G', 
                'H', 'I', 'J', 'K', 'L', 'M', 'N', 
                'O', 'P', 'Q', 'R', 'S', 'T', 
                'U', 'V', 'W', 'X', 'Y', 'Z' };

            int[] range = new int[2] { 0, 0 };
            Random random = new Random();

            switch (mode)
            {
                case RandomStringMode.Digital:
                    for (int i = 0; i < length; ++i)
                        rndStr += digitals[random.Next(0, digitals.Length)];
                    break;

                case RandomStringMode.LowerLetter:
                    for (int i = 0; i < length; ++i)
                        rndStr += lowerLetters[random.Next(0, lowerLetters.Length)];
                    break;

                case RandomStringMode.UpperLetter:
                    for (int i = 0; i < length; ++i)
                        rndStr += upperLetters[random.Next(0, upperLetters.Length)];
                    break;

                case RandomStringMode.Letter:
                    for (int i = 0; i < length; ++i)
                        rndStr += letters[random.Next(0, letters.Length)];
                    break;

                default:
                    for (int i = 0; i < length; ++i)
                        rndStr += mix[random.Next(0, mix.Length)];
                    break;
            }

            return rndStr;
        }

        string currtext = "";

        /// <summary>
        /// 显示验证码
        /// </summary>
        /// <param name="seed">随机数辅助种子</param>
        /// <param name="strLen">验证码字符长度</param>
        /// <param name="fontSize">字体大小</param>
        /// <param name="mode">随机字符模式</param>
        /// <param name="clrBg">背景颜色</param>
        public Bitmap ShowValidationCode(int seed, int strLen, int fontSize, RandomStringMode mode, Color clrBg)
        {
            Random rnd = new Random(seed);

            currtext = GenerateRandomString(strLen, mode);
            int height = 35;
            int width = 25 * currtext.Length;
            Bitmap bmp = new Bitmap(width, height);
            String fontName = "微软雅黑";

            Graphics graphics = Graphics.FromImage(bmp);
            Font font = null;//Courier New
            int findex = rnd.Next(Fonts.Length - 1);
            Brush brush = new SolidBrush(Colors[findex]);
            Brush brushBg = new SolidBrush(clrBg);
            Brush brushDd = new SolidBrush(Color.Transparent);
            graphics.FillRectangle(brushBg, 0, 0, width, height);
            Bitmap tmpBmp = null;
            Graphics tmpGph = null;
            int degree = 40;
            Point tmpPoint = new Point();
            for (int i = 0; i < currtext.Length; i++)
            {
                tmpBmp = new Bitmap(35, 55);
                tmpGph = Graphics.FromImage(tmpBmp);
                tmpGph.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                tmpGph.FillRectangle(brushDd, 0, 0, 35, 55);
                //填充透明色
                //tmpGph.Clear(Color.Transparent); 

                degree = rnd.Next(5, 20); // [20, 50]随机角度
                if (rnd.Next(0, 2) == 0)
                {
                    tmpPoint.X = 4; // 调整文本坐标以适应旋转后的图象
                    tmpPoint.Y = -2;
                }
                else
                {
                    degree = ~degree + 1; // 逆时针旋转
                    tmpPoint.X = -4;
                    tmpPoint.Y = 2;
                }

                tmpGph.RotateTransform(degree);
                font = new Font(fontName, rnd.Next(fontSize - 3, fontSize), FontStyle.Regular);
                tmpGph.DrawString(currtext[i].ToString(), font, brush, tmpPoint);
                graphics.DrawImage(tmpBmp, 10 + i * 11, 0); // 拼接图象
            }

            //释放资源
            font.Dispose();
            brush.Dispose();
            brushBg.Dispose();
            tmpGph.Dispose();
            graphics.Dispose();
            tmpBmp.Dispose();
            return bmp;
        }

        public String getCurrCode()
        {
            return currtext.ToLower();
        }







    }
}
