
namespace RawImageService
{
    public static class Thumbnail
    {
        //urithumb.Uri, 1280, 1024, 320, 240

        public static string Uri { get; set; }
        public static string Html { get; set; }
        public static int Width { get { return 1280; } }
        public static int Hight { get { return 1024; } }
        public static int ThumbWidth { get { return 320; } }
        public static int ThumbHight { get { return 240; } }
        public static string NoImage { get { return @"D:\Templates\no_image.gif"; } }
    }
}
