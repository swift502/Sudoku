using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Sudoku_3
{
    //Interface všech prvků
    //Umožňuje různé prvky vložit do stejného seznamu
    //Lze se poté spolehnout, že prvky v daném seznamu budou
    //schopny provést požadované činnosti 

    interface IPrvek
    {
        bool mouseOver { get; set; }
        float sizeOffset { get; set; }
        int draw_layer { get; set; }

        string hint { get; set; }

        void draw(Graphics g);
        void logic(PointF mouse, bool selecting, bool checking/*, PointF lateMouse*/);
        void mouseDown(MouseEventArgs e);
    }
}
