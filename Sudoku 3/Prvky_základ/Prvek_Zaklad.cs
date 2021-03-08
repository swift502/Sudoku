using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Sudoku_3
{
    //
    //Základní vlastnosti každého prvku
    //

    class Prvek_Zaklad
    {
        //Basic
        public PointF pos;  //Absolutní pozice prvku (v pixelech)
        public SizeF size;  //Absolutní velikost prvku (v pixelech)
        public bool mouseOver { get; set; } //Proměnná uchovávající, zda se kurzor myši nachází nad prvkem
        public float sizeOffset { get; set; }   //Udává relativní velikost prvku, absolutní velikost se násobí touto hodnotou

        public int draw_layer { get; set; }
        public string hint { get; set; }    //Nápověda k funkci daného prvku, zobrazuje se v horní liště

        //Text font
        Font font = new Font("Century Gothic", 16, FontStyle.Bold);

        //Eventy
        public event MouseEventHandler MouseDown;   
        public MouseEventArgs e = null; //Výchozí poskytovaný argument
        public delegate void MouseEventHandler(IPrvek c, MouseEventArgs e);

        //Při kliknutí
        protected virtual void OnClick(IPrvek c, MouseEventArgs e)
        {
            //Ošetření pro případ, že MouseDown bude prázdný
            if (MouseDown != null)
            {
                MouseDown(c, e);
            }
        }
    }
}
