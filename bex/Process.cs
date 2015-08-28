using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace YisinTick
{
    class Process
    {
        private static Process process = null;

        public static Process GetInstance()
        {
            if(process == null){
                process = new Process();
            }
            return process;
        }

        public void SetMessage(String msg)
        {
            Program.mainForm.ShowProcess();
            Program.mainForm.SetProcessMessage(msg);
        }

        public void StartShowLoginThread()
        {
            try
            {
                SetMessage("窗口加载中...");
                ThreadPool.QueueUserWorkItem((a) =>
                {
                    FrmLogin login = new FrmLogin();
                    login.ShowFrm();
                });
            }
            catch (Exception ex)
            { }
        }

        public void StartShowLoginBBSThread()
        {
            try
            {
                SetMessage("窗口加载中...");
                ThreadPool.QueueUserWorkItem((a) =>
                {
                    Login88448.Get88448LoginForm().ShowWindow();
                });
            }
            catch (Exception ex)
            {}
        }
        public void StartShowStudyThread()
        {
            try
            {
                SetMessage("窗口加载中...");
                ThreadPool.QueueUserWorkItem((a) =>
                {
                    VerifyStudy gm = new VerifyStudy();
                    Program.mainForm.HideProcess();
                    gm.ShowDialog(Program.mainForm);
                });
            }
            catch (Exception ex)
            { }
        }

        public void StartCloseThread()
        {
            try
            {
                SetMessage("数据保存中，请稍候...");
                ThreadPool.QueueUserWorkItem((a) =>
                {
                    CloseSystem();
                });
            }
            catch (Exception ex)
            {

            }
        }

        public void CloseSystem()
        {
            Thread12306.Stop();

            XmlUtils.WriteLibarayXml();

            ConfigStore.isAutoBaojing = Program.mainForm.isAutoBaojing();
            ConfigStore.isAutoWriterVerify = Program.mainForm.isAutoWriterVerify();
            ConfigStore.failedTimes = Program.mainForm.getFailedTimes();
            ConfigStore.SaveConfig();
            _88448Class.Logout();
            Form1.firstExit = false;
            Form1.CloseApplication();
        }

    }
}
