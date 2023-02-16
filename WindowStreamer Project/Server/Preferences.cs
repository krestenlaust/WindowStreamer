using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowStreamer
{
    namespace Preferences
    {
        public class Settings
        {
            public static Prefs Preferences1;

            public struct Prefs
            {
                public bool Glassmode; //false

                public bool VideoStreaming; //true
            }

            public static void SetDefault()
            {
                Prefs prefs;
                prefs.Glassmode = false;
                prefs.VideoStreaming = true;

                Preferences1 = prefs;
            }
        }
    }
}
