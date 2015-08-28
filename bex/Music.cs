using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Media;

namespace YisinTick
{
    class Music
    {
        private String filename = "";
        private SoundPlayer s = null;

        public Music(int type)
        {
            if (type == 1)
                filename = ConfigStore.appDir + ConfigStore.musicPath1;
            else if (type == 2)
                filename = ConfigStore.appDir + ConfigStore.musicPath2;
        }

        public Music(String fname)
        {
            filename = fname;
        }

        public void Play()
        {
            if (System.IO.File.Exists(filename))
            {
                ThreadPool.QueueUserWorkItem((a) => {
                    s = new SoundPlayer(filename);
                    s.Play();
                });                
            }
        }

        public void Stop()
        {
            if (s != null)
            {
                s.Stop();
            }
        }

    }
}
