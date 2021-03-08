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
    //Nabídka čísel
    //

    class Selector
    {
        //Nabídka čísel je velmi odlišný objekt v porovnání s buňkami a tlačítky
        //Nedědí proto z Prvek_Zaklad ani IPrvek interfacu
        //Základní proměnné jako pozice je tedy třeba nadefinovat ručně

        #region Globální proměnné

        public PointF pos;  //Absolutní pozice nabídky (v pixelech)
        float size = 100;   //Absolutní velikost nabídky (průměr kruhu v pixelech)
        public float sizeOffset, transparency;  //Relativní velikost a průhlednost celé nabídky
        public bool obsolete;   //Určuje, zda uživatel ještě nabídku využívá
        public List<numberOffer> offers;    //Jednotlivá čísla v nabídce

        public int selected;    //Číslo určující poslední zvolené číslo, které se případně zapíše do vybrané buňky

        public cell parent; //Určuje buňku, které touto nabídkou vybíráme číslo
        Font font;

        #endregion

        public Selector(cell parent)
        {
            //Inicializace proměnných
            sizeOffset = 0.5f;
            transparency = 0;

            this.parent = parent;
            offers = new List<numberOffer>();
        }

        public void logic(PointF mouse)
        {
            if (!obsolete)
            {
                pos = parent.pos;   //Sledování rodičovské buňky

                //Zviditelnění zvětšením/barvou
                sizeOffset = Func.exponential(sizeOffset, 1f, 5);
                transparency = Func.exponential(transparency, 255, 5);
            }
            else
            {
                //Ukončení zviditelnění zvětšením/barvou
                sizeOffset = Func.exponential(sizeOffset, 0.5f, 5);
                transparency = Func.exponential(transparency, 0, 3);
            }

            bool selection = false;
            //Zjišťování, zda uživatel najel kurzorem myši nad nějaké číslo a tím vybral číslo do buňky
            foreach (numberOffer offer in offers)
            {
                offer.logic(mouse);
                if (offer.mouseOver) selection = true;
            }
            if (!selection) selected = 0;
        }

        public void draw(Graphics g)
        {
            //Základní kruh
            g.DrawEllipse(new Pen(Color.FromArgb((int)transparency, C.CoronaDark), 12),
                pos.X - ((size * sizeOffset) / 2), pos.Y - ((size * sizeOffset) / 2), size * sizeOffset, size * sizeOffset);

            float numberSize = 30;  //Referenční velikost čísel

            foreach (numberOffer offer in offers)
            {
                font = new Font("Century Gothic", 16 * sizeOffset * offer.sizeOffset, FontStyle.Bold);

                //Zjištění stupňů pro vykreslení čísel
                float rad = ((((float)offer.value - 1f) * 40f) / 180f) * (float)Math.PI;
                PointF numPos = new PointF(
                    pos.X + ((float)Math.Sin(rad) * (size / 2) * sizeOffset),
                    pos.Y + ((float)-Math.Cos(rad) * (size / 2) * sizeOffset));
                //Pozadí čísel
                g.FillEllipse(new SolidBrush(Color.FromArgb((int)(transparency * offer.transparency), C.CoronaLight)),
                    numPos.X - (numberSize * sizeOffset * offer.sizeOffset / 2),
                    numPos.Y - (numberSize * sizeOffset * offer.sizeOffset / 2),
                    numberSize * sizeOffset * offer.sizeOffset,
                    numberSize * sizeOffset * offer.sizeOffset);
                //Zvýraznění
                g.FillEllipse(new SolidBrush(Color.FromArgb((int)(transparency * offer.highlight), C.CellMid)),
                    numPos.X - (numberSize * sizeOffset * offer.sizeOffset / 2),
                    numPos.Y - (numberSize * sizeOffset * offer.sizeOffset / 2),
                    numberSize * sizeOffset * offer.sizeOffset,
                    numberSize * sizeOffset * offer.sizeOffset);
                //Ohraničení
                g.DrawEllipse(new Pen(Color.FromArgb((int)transparency, C.CoronaDark), 4),
                    numPos.X - (numberSize * sizeOffset * offer.sizeOffset / 2),
                    numPos.Y - (numberSize * sizeOffset * offer.sizeOffset / 2),
                    numberSize * sizeOffset * offer.sizeOffset,
                    numberSize * sizeOffset * offer.sizeOffset);

                //Číslo samotné
                SizeF textSize = g.MeasureString(offer.value.ToString(), font);
                g.DrawString(offer.value.ToString(), font,
                    new SolidBrush(Color.FromArgb((int)transparency, C.CoronaDark)),
                    new PointF(
                        numPos.X - (textSize.Width / 2),
                        numPos.Y - (textSize.Height / 2)));
            }
        }

        public void refreshNumbers(cell[,] grid)
        {
            //Naplnění dočasného seznamu všemi čísly 1-9
            List<int> numbers = new List<int>();
            for (int i = 0; i < 9; i++) numbers.Add(i + 1);

            //Odebrání knofliktů v řádcích a sloupcích
            if (Form1.rowsColumns)
            {
                for (int i = 0; i < 9; i++)
                {
                    //Řádek
                    cell cell = grid[i, parent.index.Y];
                    if (cell.value != 0 && !cell.wrong) numbers.Remove(cell.value);
                    //Sloupec
                    cell = grid[parent.index.X, i];
                    if (cell.value != 0 && !cell.wrong) numbers.Remove(cell.value);
                }
            }

            //Odebrání konfliktů ve skupinách devíti buňek
            if (Form1.areas)
            {
                Point region = new Point((parent.index.X / 3) * 3, (parent.index.Y / 3) * 3);
                for (int i = 0; i < 9; i++)
                {
                    Point local = new Point(i % 3, i / 3);
                    cell cell = grid[region.X + local.X, region.Y + local.Y];
                    if (cell.value != 0 && !cell.wrong)
                        numbers.Remove(grid[region.X + local.X, region.Y + local.Y].value);
                }
            }

            //Vyčištění nabídky
            offers.Clear();

            //Plnění nabídky čísly z dočasného seznamu
            foreach (int number in numbers)
            {
                numberOffer offer = new numberOffer(this);
                offer.value = number;
                offers.Add(offer);
            }
        }
    }
    
    //
    //Číselná nabídka
    //Jedná se o jednotlivá čísla (1-9) v každé nabídce
    //

    class numberOffer
    {
        #region Globální proměnné

        public float sizeOffset;    //Relativní velikost
        public float transparency;  //Vlastní průhlednost
        public float highlight;     //Intenzita zvýraznění

        public int value;   //Hodnota čísla

        Selector parent;    //Rodičovská nabídka
        public bool mouseOver;  //Určuje, zda se kurzor nachází nad číslem

        #endregion

        public numberOffer(Selector parent)
        {
            //Inicializace proměnných
            transparency = 0f;
            sizeOffset = 0.5f;
            highlight = 0f;
            this.parent = parent;
        }

        public void logic(PointF mouse)
        {
            if (Func.vzdalenost(parent.pos, mouse) > 30)
            {
                //Výpočty související s určením toho, zda uživaatel vybírá dané číslo

                //Úhel v radiánech, který svírají střed rodičovské nabídky tohoto čísla a pozice kurzoru myši
                float angle = (float)Math.Atan2(mouse.X - parent.pos.X, parent.pos.Y - mouse.Y) + (float)Math.PI;

                //Mezní úhly ve kterých se může kurzor myši pohybovat, aby byla uživatelská volba platná
                float angle1 = (((((value) * 40f) + 160) / 180f) * (float)Math.PI) % (float)(Math.PI * 2);
                float angle2 = (((((value) * 40f) + 120) / 180f) * (float)Math.PI) % (float)(Math.PI * 2);

                //Podmínka pro prvních 8 čísel      OR      Výjimka pro případ posledního čísla kdy vypočítané úhly přesahují 2π a vracejí se k nule
                if ((angle < angle1 && angle > angle2) || (angle > angle2 && angle2 > angle1))
                {
                    mouseOver = true;
                    parent.selected = value;
                }
                else
                {
                    mouseOver = false;
                }
            }
            else
            {
                mouseOver = false;
                parent.selected = 0;
            }

            //Výpočty relativní velikosti a průhlednosti (Výchozí)
            sizeOffset = Func.exponential(sizeOffset, 1f, 3);
            transparency = Func.exponential(transparency, 1f, 3);

            if (mouseOver)
            {
                //Zvýraznění velikostí a barvou (Kurzor je nad číslem)
                sizeOffset = Func.exponential(sizeOffset, 1.5f, 4);
                highlight = Func.exponential(highlight, 1f, 8);
            }
            else
            {
                //Zvýraznění velikostí a barvou (Kurzor není nad číslem)
                sizeOffset = Func.exponential(sizeOffset, 1f, 4);
                highlight = Func.exponential(highlight, 0f, 8);
            }
        }
    }
}
