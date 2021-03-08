using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Sudoku_3
{
    public partial class Form1 : Form
    {
        #region Globální proměnné

        //Programový časovač, náhrada komponenty Timer
        Timer timer = new Timer();

        //Pole buněk
        cell[,] grid;
        //Pole všech čísel, která se vygenerují při zavolání metody generateNumbers()
        //Při kontrole správnosti se porovnávají čísla zadaná do buněk uživatelem s čísly tohoto pole
        int[,] puvodniCisla;

        Size gridSize;  //Velikost pole buněk
        int uroven; //Obtížnost hry Sudoku
        public static int cellSize = 40;    //Velikost jedné buňky
        public static int menuSize = 35;    //Velikost menu lišty

        public static bool rowsColumns; //Určuje, zda se mají v nabízených číslech zobrazovat
                                        //konfliktní čísla ze skupin devíti buněk
        public static bool areas;//Určuje, zda se mají v nabízených číslech zobrazovat
                                 //konfliktní čísla ze stejných sloupců a řádek

        Point borders;  //Určuje odsazení hracího pole od okrajů okna
        List<float> layers; //Vrstvy, podle kterých se určuje pořadí vykreslování všech komponent
        int draw_layers;

        PointF mouse;   //Lokální pozice kurzoru
        bool dragging;  //Určuje, zda uživatel hýbe s oknem
        bool selecting; //Určuje, zda uživatel vybírá do buňky číslo z nabídky čísel
        bool checking;  //Určuje, zda právě probíhá kontrola správnosti

        string hint; //Text v menu
        
        Point dragPos;  //Pozice používaná při přesunu okna

        Font font = new Font("Century Gothic", 14); //Font menu

        List<IPrvek> prvky = new List<IPrvek>();    //Seznam všech prvků ve formu
        List<Selector> selectors = new List<Selector>();    //Seznam právě zobrazovaných nabídek čísel

        #endregion

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            checking = false;
            selecting = false;
            dragging = false;

            rowsColumns = false;
            areas = true;

            hint = "";

            borders = new Point(50, 50);
            draw_layers = 2;
            this.Size = new Size((2 * borders.X) + (9 * cellSize) + 8, (2 * borders.Y) + menuSize + (9 * cellSize) + 8);
            design();

            //Kurzor
            Cursor.Hide();
        }

        //Náhrada defaultní Timer komponenty, obnovovací cyklus celého programu
        //Zdroj: http://www.dreamincode.net/forums/topic/99860-creating-games-in-c%23-part-i/
        public void ProgramLoop()
        {
            while (this.Created)
            {
                timer.Reset();
                Logic();    //Logika programu
                this.Invalidate();  //Vykreslování
                Application.DoEvents();
                while (timer.GetTicks() < 10) ; //Ovládání obnovovací frekvence vykreslování
            }
        }

        private void Logic()
        {
            //Pohyb okna
            if (dragging)
            {
                this.Location = new Point(Cursor.Position.X - dragPos.X, Cursor.Position.Y - dragPos.Y);
            }

            //Systém vrstev
            //Nejdříve zjistíme relativní velikost každého prvku
            //Tyto velikost vložíme do seznamu, a každé dvě sousední hodnoty zprůměrujeme
            //Při vykreslování potom kontrolujeme vrstvu po vrstvě od nejnižší po nejvyšší
            //a porovnáváme, zda se velikost prku vejde do daného rozsahu velikostí v seznamu
            List<float> sizes = new List<float>();  //Velikosti prvků
            layers = new List<float>(); //Vrstvy (zprůměrované sousední hodnoty velikostí)

            foreach (cell cell in grid)
            {
                //Přidání velikostí všech prvků do seznamuu velikostí
                //Pokud se již v seznamu velikost vyskytuje, není nutné ji přidávat
                if (!sizes.Contains(cell.sizeOffset)) sizes.Add(cell.sizeOffset);
            }

            //Seřazení velikostí od nejmenší po největší
            sizes.Sort();

            //Na začátek seznamu vložíme záporné nekonečno
            //To zajistí korektní rozsah nulté vrstvy
            layers.Add(float.NegativeInfinity);

            //Zprůměrování sousedních hodnot
            for (int i = 0; i < sizes.Count - 1; i++)
            {
                layers.Add(Func.prumer(sizes[i], sizes[i+1]));
            }

            //Přidání kladného nekonečna na konec seznamu
            layers.Add(float.PositiveInfinity);

            //Vykonání logiky pro všechny prvky
            bool hintChanged = false;
            foreach (IPrvek p in prvky)
            {
                p.logic(mouse, selecting, checking);
                if (p.mouseOver)
                {
                    hint = p.hint;
                    hintChanged = true;
                }
            }
            if (!hintChanged) hint = "";

            //Vykonání logiky pro všechny nabídky čísel
            foreach (Selector sel in selectors)
            {
                sel.logic(mouse);
            }

            //Odstranění nepotřebných nabídek čísel
            //Tj. když jejich průhlednost dosáhne nuly
            for (int i = 0; i < selectors.Count; )
            {
                if ((int)selectors[i].transparency == 0)
                {
                    selectors.Remove(selectors[i]);
                }
                else i++;
            }

            //Kontrola správnosti čísel
            if (checking) checkNumbers();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            //Vyhlazování
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            //Volba vykreslovací vrstvy
            for(int layer = 0; layer < draw_layers; layer++)
            {
                //Menu lišta
                if(layer == 1)
                e.Graphics.FillRectangle(new SolidBrush(C.MenuDark), 0, 0, this.Size.Width, menuSize);

                //Vykreslení prvků
                for (int i = 0; i < layers.Count - 1; i++)
                    foreach (IPrvek p in prvky)
                    {
                        if(p.draw_layer == layer)   //Pokud je prvek na správné vrstvě
                            if (p.sizeOffset >= layers[i] && p.sizeOffset < layers[i + 1])  //Pokud se prvek vejde do výškové vrstvy
                                p.draw(e.Graphics); //Vykreslit prvek
                    }
            }

            //Vykreslení nabídek čísel
            foreach (Selector sel in selectors)
            {
                sel.draw(e.Graphics);
            }

            //Úrovně obtížnosti
            e.Graphics.TranslateTransform(10, 11);
            e.Graphics.FillPath(Brushes.White, checkMark);
            e.Graphics.TranslateTransform(30, 0);
            e.Graphics.DrawPath(new Pen(Color.White, 2), indicatorLeft);
            e.Graphics.FillPath(Brushes.White, indicatorLeft);
            e.Graphics.TranslateTransform(menuSize, 0);
            e.Graphics.DrawRectangle(new Pen(Color.White, 2), 0, 0, 24, 16);
            if (uroven > 1) e.Graphics.FillRectangle(Brushes.White, 0, 0, 24, 16);
            e.Graphics.TranslateTransform(menuSize, 0);
            e.Graphics.DrawRectangle(new Pen(Color.White, 2), 0, 0, 24, 16);
            if (uroven > 2) e.Graphics.FillRectangle(Brushes.White, 0, 0, 24, 16);
            e.Graphics.TranslateTransform(menuSize, 0);
            e.Graphics.DrawPath(new Pen(Color.White, 2), indicatorRight);
            if (uroven > 3) e.Graphics.FillPath(Brushes.White, indicatorRight);
            e.Graphics.TranslateTransform(30, 0);

            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            e.Graphics.DrawString(hint, font, Brushes.White, new PointF(4, -2));
            e.Graphics.ResetTransform();

            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            //Hranice okna
            e.Graphics.DrawRectangle(new Pen(C.MenuMid, 8), 0, 0, this.Size.Width, this.Size.Height);

            //Kurzor
            if (!selecting || selecting && selectors[0].selected == 0)
            {
                e.Graphics.FillEllipse(new SolidBrush(C.CellLight), mouse.X - 5, mouse.Y - 5, 10, 10);
                e.Graphics.DrawEllipse(new Pen(C.CellDark, 2), mouse.X - 5, mouse.Y - 5, 10, 10);
            }
        }

        //Získá aktuální text pro obtížnost hry
        string getUroven()
        {
            switch (uroven)
            {
                case 1: return "Lehké";
                case 2: return "Střední";
                case 3: return "Těžké";
                case 4: return "Velmi těžké";
            }
            return "chyba";
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            bool prvek = false;

            foreach (IPrvek p in prvky)
            {
                //Pokud je kurzor nad prvkem, spustíme metody pro rakci na kliknutí na prvek
                if (p.mouseOver)
                {
                    p.mouseDown(e);
                    prvek = true;
                }
            }

            //Pokud jsme se nestrefili do žádného prvku
            //Začneme pohybovat oknem
            if (!prvek)
            {
                dragPos = e.Location;
                dragging = true;
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            //Neustálé zapisování lokálních souřadnic kurzoru do globální statické proměnné
            mouse = e.Location;
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (dragging) dragging = false; //Ukončení pohybování okna


            //Ukončení nabídky čísel
            //Případné vybrání čísla a jeho vložení do buňky
            if (selecting && e.Button == MouseButtons.Left)
            {
                selecting = false;

                if (selectors[0].selected != 0)
                {
                    selectors[0].parent.value = selectors[0].selected;
                    selectors[0].parent.wrong = false;
                }

                //Současná nabídka čísel
                selectors[0].obsolete = true;

            }

        }
    }
}
