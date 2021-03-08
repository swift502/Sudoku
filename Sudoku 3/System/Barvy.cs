using System.Drawing;

namespace Sudoku_3
{
    //Barvy používané při vykreslování veškeré grafiky
    public static class C
    {
        public static Color MenuDark = Color.FromArgb(82,72, 55);
        public static Color MenuMid = Color.FromArgb(166,147,106);
        public static Color MenuLight = Color.FromArgb(255,255,255);

        public static Color CellDark = Color.FromArgb(46, 35, 18);
        public static Color CellMid = Color.FromArgb(255, 192, 72);
        public static Color CellLight = Color.FromArgb(253, 253, 189);

        public static Color CoronaDark = Color.FromArgb( 81, 60, 36);
        public static Color CoronaMid = Color.FromArgb(255, 192, 72);
        public static Color CoronaLight = Color.FromArgb(255, 253, 185);

        public static Color wrong = Color.FromArgb(255, 114, 88);
    }
}
