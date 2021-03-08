using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Sudoku_3
{
    public partial class Form1
    {
        //
        //Oddíl design
        //Ruční definování všech prvků a jejich parametrů
        //Velmi podobné strojově generovaným kódům Formů "Designer"
        //

        #region Globální deklarace

        menuButton U1;
        menuButton U2;
        menuButton U3;
        menuButton U4;

        menuButton check;
        menuButton set;
        menuButton mini;
        menuButton exit;

        GraphicsPath indicatorLeft, indicatorRight, checkMark;

        #endregion

        void design()
        {
            #region Hrací pole

            //Velikost hracího pole, původně proměnná, algoritmy pro Sudoku však vyžadují 
            //velikost 9x9 
            gridSize = new Size(9, 9);

            //Pole buněk (třída cell)
            grid = new cell[gridSize.Width, gridSize.Height];

            //Plnění pole buněk
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                {
                    cell cell = new cell(); //Vytvoření nové instance třídy buňky
                    cell.index = new Point(i, j);   //Přiřazení indexu buňky
                    cell.pos = new Point(   //Nastavení vykreslovací pozice buňky
                        (i * cellSize) + (cellSize / 2) + ((i / 3) * 2) + borders.X,
                        (j * cellSize) + (cellSize / 2) + ((j / 3) * 2) + borders.Y + menuSize);
                    cell.targetPos = cell.pos;  //Nastavení pozice ke které se buňka snaží přiblížit
                    cell.size = new SizeF(cellSize, cellSize);  //Velikost buňky
                    cell.MouseDown += new Prvek_Zaklad.MouseEventHandler(cell_MouseDown);   //Přiřazení nového event handleru
                    grid[i, j] = cell;  //Přidání buňky do pole buněk
                    cell.draw_layer = 0;

                    prvky.Add(cell);    //Přidání do seznamu prvků
                }

            uroven = 2; //Obtížnost Sudoku
            explode(10000); //Efekt rozlétnutí buněk
            generateNumbers();  //Vygenerování nových platných čísel

            #endregion

            #region Tlačítka

            //Tlačítka úrovní

            U1 = new menuButton();
            U1.pos = new PointF(menuSize, 0);
            U1.size = new SizeF(menuSize, menuSize);
            U1.hint = "Lehké";
            U1.draw_layer = 1;
            U1.MouseDown += new Prvek_Zaklad.MouseEventHandler(U1_MouseDown);

            U2 = new menuButton();
            U2.pos = new PointF((2 * menuSize), 0);
            U2.size = new SizeF(menuSize, menuSize);
            U2.hint = "Střední";
            U2.draw_layer = 1;
            U2.MouseDown += new Prvek_Zaklad.MouseEventHandler(U2_MouseDown);

            U3 = new menuButton();
            U3.pos = new PointF((3 * menuSize), 0);
            U3.size = new SizeF(menuSize, menuSize);
            U3.hint = "Těžké";
            U3.draw_layer = 1;
            U3.MouseDown += new Prvek_Zaklad.MouseEventHandler(U3_MouseDown);

            U4 = new menuButton();
            U4.pos = new PointF((4 * menuSize), 0);
            U4.size = new SizeF(menuSize, menuSize);
            U4.hint = "Velmi těžké";
            U4.draw_layer = 1;
            U4.MouseDown += new Prvek_Zaklad.MouseEventHandler(U4_MouseDown);

            //Kontrola
            check = new menuButton();
            check.pos = new PointF(0, 0);
            check.size = new SizeF(menuSize, menuSize);
            //check.text = "✓";
            check.hint = "Zkontrolovat";
            check.draw_layer = 1;
            check.MouseDown += new Prvek_Zaklad.MouseEventHandler(check_MouseDown);

            //Nápověda
            set = new menuButton();
            set.pos = new PointF(this.Size.Width - (menuSize * 3), 0);
            set.size = new SizeF(menuSize, menuSize);
            set.text = "­?";
            set.hint = "Nápověda";
            set.draw_layer = 1;
            set.MouseDown += new Prvek_Zaklad.MouseEventHandler(set_MouseDown);

            //Minimalizace
            mini = new menuButton();
            mini.pos = new PointF(this.Size.Width - (menuSize * 2), 0);
            mini.size = new SizeF(menuSize, menuSize);
            mini.text = "_";
            mini.hint = "Minimalizovat";
            mini.draw_layer = 1;
            mini.MouseDown += new Prvek_Zaklad.MouseEventHandler(mini_MouseDown);

            //Konec
            exit = new menuButton();
            exit.pos = new PointF(this.Size.Width - menuSize, 0);
            exit.size = new SizeF(menuSize, menuSize);
            exit.text = "x";
            exit.hint = "Ukončit";
            exit.draw_layer = 1;
            exit.MouseDown += new Prvek_Zaklad.MouseEventHandler(exit_MouseDown);

            //Přidání do seznamu prvků
            prvky.Add(U1);
            prvky.Add(U2);
            prvky.Add(U3);
            prvky.Add(U4);
            prvky.Add(check);
            prvky.Add(set);
            prvky.Add(mini);
            prvky.Add(exit);

            #endregion

            #region Grafické tvary

            //Znak kontroly - checkmark
            checkMark = new GraphicsPath();
            checkMark.AddLine(1, 3, 4, 6);
            checkMark.AddLine(4,6,8,0);
            checkMark.AddLine(8,0,10,1);
            checkMark.AddLine(10,1,4,9);
            checkMark.AddLine(4,9,0,5);
            checkMark.AddLine(0,5,1,3);
            checkMark.CloseFigure();

            //Určení zvětšovací transformace
            Matrix scale = new Matrix();
            scale.Scale(1.8f, 1.8f);

            checkMark.Transform(scale);

            //Znak úrovně Lehké
            indicatorLeft = new GraphicsPath();
            indicatorLeft.AddArc(0, 0, 12, 16, 90, 180);
            indicatorLeft.AddLine(12, 0, 24, 0);
            indicatorLeft.AddLine(24, 0, 24, 16);
            indicatorLeft.AddLine(24, 16, 10, 16);
            indicatorLeft.CloseFigure();

            //Určení překlápěcí transformace
            Matrix mirror = new Matrix();
            mirror.Scale(-1, 1);
            mirror.Translate(-24, 0);

            //Znak úrovně Velmi těžké
            indicatorRight = (GraphicsPath)indicatorLeft.Clone();
            indicatorRight.Transform(mirror);

            #endregion
        }

        //
        //Metody tlačítek a buněk
        //

        void cell_MouseDown(IPrvek p, MouseEventArgs e)
        {
            cell cell = p as cell;

            if (e.Button == MouseButtons.Left && cell.editable && !selecting)
            {
                selecting = true;   //Proměnná říkající, že uživatel vybírá číslo z nabídky

                Selector sel = new Selector(cell);    //Vytvoření nové nabídky čísel
                sel.refreshNumbers(grid);   //Načtení vhodných čísel k zobrazení
                selectors.Insert(0, sel);   //Vložení nabídky do seznamu nabídek
            }
            else if (e.Button == MouseButtons.Right && !selecting && cell.editable)
            {
                cell.value = 0; //Odebrání hodnoty z buňky
                cell.sizeOffset = 0.8f; //Zvětšovací efekt při odebrání hodnoty buňky
                cell.wrong = false; //Buňky již nenabývá žádné hodnoty, tudíž nemůže být špatně vyplněná
            }
        }

        void U1_MouseDown(IPrvek p, MouseEventArgs e)
        {
            uroven = 1;
            explode(1);
            generateNumbers();
        }

        void U2_MouseDown(IPrvek p, MouseEventArgs e)
        {
            uroven = 2;
            explode(6);
            generateNumbers();
        }

        void U3_MouseDown(IPrvek p, MouseEventArgs e)
        {
            uroven = 3;
            explode(18);
            generateNumbers();
        }

        void U4_MouseDown(IPrvek p, MouseEventArgs e)
        {
            uroven = 4;
            explode(50);
            generateNumbers();
        }

        void check_MouseDown(IPrvek p, MouseEventArgs e)
        {
            checkIndex = 0;
            checking = true;
        }

        void set_MouseDown(IPrvek p, MouseEventArgs e)
        {
            Cursor.Show();
            Settings set = new Settings();
            set.ShowDialog(this);
            Cursor.Hide();
        }

        void mini_MouseDown(IPrvek p, MouseEventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        void exit_MouseDown(IPrvek p, MouseEventArgs e)
        {
            this.Close();
        }

    }
}
