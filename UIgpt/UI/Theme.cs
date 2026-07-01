using System.Drawing;

namespace ElstanLab.Themes
{
    public static class Theme
    {
        public static readonly Color FormBack = Color.FromArgb(240, 243, 248);

        public static readonly Color GroupBack = Color.White;

        public static readonly Color HeaderColor = Color.FromArgb(0, 51, 102);

        public static readonly Color Border = Color.FromArgb(210, 210, 210);

        public static readonly Font HeaderFont =
            new Font("Segoe UI", 16, FontStyle.Bold);

        public static readonly Font GroupFont =
            new Font("Segoe UI", 10, FontStyle.Bold);

        public static readonly Font DefaultFont =
            new Font("Segoe UI", 9);
    }
}
