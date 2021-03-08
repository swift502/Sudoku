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
    //Tlačítko
    //Používané v menu liště
    //

    class menuButton : Prvek_Zaklad, IPrvek
    {
        public string text; //Text zobrazovaný uvnitř tlačítka
        Font font = new Font("Century Gothic", 16, FontStyle.Bold);

        public menuButton()
        {
            sizeOffset = 1f;
        }

        public void logic(PointF mouse, bool selecting, bool checking)
        {
            //Určení toho, zda je kurzor myši nad prvkem
            if (checking) mouseOver = false;
            else
            {
                if (mouse.X > pos.X && mouse.X < pos.X + size.Width &&
                    mouse.Y > pos.Y && mouse.Y < pos.Y + size.Height)
                    mouseOver = true;
                else mouseOver = false;
            }
        }

        public void mouseDown(MouseEventArgs e)
        {
            if (mouseOver)
            {
                OnClick(this, e);
            }
        }

        public void draw(Graphics g)
        {
            //Zvýraznění
            if (mouseOver) g.FillRectangle(new SolidBrush(C.MenuMid), pos.X, pos.Y, size.Width, size.Height);

            //Text
            SizeF textSize = g.MeasureString(text, font);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            g.DrawString(text, font, new SolidBrush(C.MenuLight),
                pos.X + ((size.Width - textSize.Width) / 2), pos.Y + ((size.Height - textSize.Height) / 2));
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
        }
    }
}
