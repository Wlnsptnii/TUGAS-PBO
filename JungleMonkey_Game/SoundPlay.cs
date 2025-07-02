using System;
using WMPLib;

namespace JungleMonkey
{
    public static class SoundPlay
    {
        public static WindowsMediaPlayer Bgm { get; private set; }

        public static void Play(string path)
        {
            Bgm = new WindowsMediaPlayer();
            Bgm.URL = path;
            Bgm.settings.setMode("loop", true);
            Bgm.settings.volume = 50;
            Bgm.controls.play();
        }

        public static void Stop()
        {
            if (Bgm != null)
            {
                Bgm.controls.stop();
                Bgm.close();
                Bgm = null;
            }
        }
    }
}