namespace Server
{
    class Other
    {
        public class Preferences
        {
            public bool glassMode { get; set; } = false;

            public bool videoStreaming { get; set; } = true;
        }

        public static LogWindow LogWindow = null;

        public static Preferences Prefs;
    }
}
