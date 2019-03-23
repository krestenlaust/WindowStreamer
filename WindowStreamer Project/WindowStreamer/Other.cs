using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Other
    {
        public class Preferences
        {
            public bool glassMode { get; set; } = false;
            public bool videoStreaming { get; set; } = true;
        }

        public class Statics
        {
            public static LogWindow logWindow = null;
        }

        public static Preferences prefs;
    }
}
