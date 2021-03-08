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
    //Buňka hracího pole
    //

    class cell : Prvek_Zaklad, IPrvek
    {
        #region Globální proměnné

        public int value;   //Číselná hodnota buňky
        public Point index; //Poziční index buňky
        Font font;
        public float highlight; //Určuje intenzitu zvyraznění objektu (resp. průhlednost 0-255)
        public bool editable;   //Určuje, zda může uživatel upravovat hodnotu buňky
        public bool wrong;      //Určuje, zda hodnota zadaná do buňky je správně či špatně
                                //Používáno při kontrolování správnosti

        //Pružinová fyzika
        public float mass;  //Hmotnost buňky
        public float damp;  //Faktor zpomalení
        public PointF targetPos, velocity;  //Cílová pozice buňky a rychlost buňky
        public float targetSize, sizeVelocity;  //Cílová relativní velikost a rychlost zvětšování/zmeněování buňky

        #endregion

        public cell()
        {
            //Inicializace proměnných
            sizeOffset = 1f;
            velocity = PointF.Empty;
            sizeVelocity = 0f;
            targetSize = 1f;

            mass = Func.rnd.Next(5, 20);
            damp = (float)Func.rnd.Next(80, 90) / 100;

            editable = false;
        }

        public void logic(PointF mouse, bool selecting, bool checking)
        {
            //Určení, zda je kurzor myši nad buňkou
            if (!selecting && !checking)
            {
                if (mouse.X >= pos.X - (size.Width / 2) && mouse.X < pos.X + (size.Width / 2) &&
                    mouse.Y >= pos.Y - (size.Height / 2) && mouse.Y < pos.Y + (size.Height / 2))
                    mouseOver = true;
                else mouseOver = false;
            }

            //Přiblížení buňky ke své cílové pozici podle pravidel pružinové fyziky
            if (pos != targetPos)
              Func.spring(ref pos, ref velocity, targetPos, mass, damp);

            //Zvětšení/zmenšení podle pravidel pružinové fyziky
            sizeOffset = Func.spring(sizeOffset, ref sizeVelocity, targetSize, mass, damp);
            if (sizeOffset < 0.1f) sizeOffset = 0.1f;

            //Velikostní zvýraznění
            if(mouseOver)
            sizeOffset = Func.exponential(sizeOffset, 1.4f, 4);

            //Zaokrouhlení 
            sizeOffset = (float)Math.Round(sizeOffset, 3);

            //Barevné zvýraznění
            if (mouseOver)
                highlight = Func.exponential(highlight, 255, 3);
            else
                highlight = Func.exponential(highlight, 0, 20);
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
            if (wrong)
            {
                //Hodnota v buňce je špatně - červená
                g.FillRectangle(new SolidBrush(C.wrong),
                pos.X - ((size.Width * sizeOffset) / 2), pos.Y - ((size.Height * sizeOffset) / 2),
                size.Width * sizeOffset, size.Height * sizeOffset);
            }
            else
            {
                if (editable)
                {
                    if (value == 0)
                        //Buňka nenabývá žádné hodnoty - tmavě šedivá
                        g.FillRectangle(new SolidBrush(C.MenuMid),
                        pos.X - ((size.Width * sizeOffset) / 2), pos.Y - ((size.Height * sizeOffset) / 2),
                        size.Width * sizeOffset, size.Height * sizeOffset);
                    else
                    {
                        //Buňka nabývá jisté hodnoty ale není zkontrolována - oranžová
                        g.FillRectangle(new SolidBrush(C.CoronaMid),
                        pos.X - ((size.Width * sizeOffset) / 2), pos.Y - ((size.Height * sizeOffset) / 2),
                        size.Width * sizeOffset, size.Height * sizeOffset);
                    }
                }
                else
                {
                    //Buňku nelze upravit / Buňka byla zkontrolována a její obsah odpovídá původním hodnotám - bílá
                    g.FillRectangle(new SolidBrush(C.CellLight),
                    pos.X - ((size.Width * sizeOffset) / 2), pos.Y - ((size.Height * sizeOffset) / 2),
                    size.Width * sizeOffset, size.Height * sizeOffset);
                }
            }

            //Barevné zvýraznění buňky
            if (editable)
                g.FillRectangle(new SolidBrush(Color.FromArgb((int)highlight, C.CellMid)),
                pos.X - ((size.Width * sizeOffset) / 2), pos.Y - ((size.Height * sizeOffset) / 2),
                size.Width * sizeOffset, size.Height * sizeOffset);

            //Ohraničení buňky
            g.DrawRectangle(new Pen(C.CellDark, 2),
                pos.X - ((size.Width * sizeOffset) / 2), pos.Y - ((size.Height * sizeOffset) / 2),
                size.Width * sizeOffset, size.Height * sizeOffset);

            //Vypsání číselné hodnoty buňky
            if (value != 0)
            {
                font = new Font("Century Gothic", 20 * sizeOffset);
                string text = value.ToString();
                SizeF textSize = g.MeasureString(text, font);
                g.DrawString(text, font, Brushes.Black, pos.X - (textSize.Width / 2), pos.Y - (textSize.Height / 2));
            }
        }
    }
}
