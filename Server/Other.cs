namespace Server
{
    class Other
    {
        public class Preferences
        {
            public bool GlassMode { get; set; } = false;

            public bool VideoStreaming { get; set; } = true;
        }

        public static Preferences Prefs;
    }
}
