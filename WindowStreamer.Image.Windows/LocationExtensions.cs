using System.Drawing;

namespace WindowStreamer.Image.Windows
{
    public static class LocationExtensions
    {
        public static Location ToLocation(this Point point) => new Location(point.X, point.Y);

        public static Point ToPoint(this Location location) => new Point(location.X, location.Y);
    }
}
