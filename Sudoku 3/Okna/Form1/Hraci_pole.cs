using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Sudoku_3
{
    public partial class Form1
    {
        //Generování hracího pole. Vlastní implementace algoritmu s použitím "backtrackingu".
        //Zdroj: http://www.codeproject.com/Articles/23206/Sudoku-Algorithm-Generates-a-Valid-Sudoku-in
        //Funguje pouze pro pole 9x9 na hraní sudoku. Jiné velikosti polí nebudou fungovat!
        void generateNumbers()
        {
            int i = 0;  //Index současné buňky, pohybujeme se zleva doprava a shora dolu
            //int[,] numbers = new int[9, 9]; //Pole s číselnými hodnotami jednotlivých buňek
            List<int>[,] available = new List<int>[9, 9];   //Pole káždé buňky udávající čísla, která je možno do dné buňky dosadit

            //Random rnd = new Random();

            //Vyčištění pole před generováním nových hodnot
            foreach (cell cell in grid)
            {
                cell.value = 0;
                cell.editable = false;
                cell.wrong = false;
            }

            while (i != 81) //Cyklus pro každou buňku
            {
                int x = i % 9;  //Převod indexu na souřadnici X
                int y = i / 9;  //Převod indexu na souřadnici Y

                //Pokud pole s možnými kombinacemi není inicializováno, v předchozím cyklu jsme
                //inkrementovali index a pohnuli jsme se vpřed, kterékoliv předchozí možná čísla
                //již nejsou platná, a je nutno si je zjistit znovu
                if (available[x, y] == null)
                    getAvailable(x, y, ref available[x, y]);

                //Pokud nejsou na doplnění k dispozici žádná čísla, musíme zahájit backtracking.
                //Současnou buňku obereme jak o číselnou hodnotu, tak o její seznam všech možných
                //čísel k doplnění, takže až se k ní příště vrátíme, bude se chovat stejně, jako
                //by jsme jí nikdy žádné změny neprováděli.
                if (available[x, y].Count == 0)
                {
                    grid[x, y].value = 0;
                    available[x, y] = null;

                    //Nakonec dekrementujeme index, tj. vrátíme se o buňku (pokud bude i více) zpět,
                    // a začneme hledat vhodnější, která budou mít řešení až do konce vyplňování.
                    i--;
                }

                //Když máme k dispozici čísla na doplnění
                else
                {
                    //Zvolíme náhodné číslo z dostupných čísel
                    int random = Func.rnd.Next(available[x, y].Count);
                    int number = available[x, y][random];

                    //Přiřadíme toto číslo k buňce k buňce
                    grid[x, y].value = number;

                    //Odebereme dané číslo z možných čísel této buňky
                    //Pokud se někdy při postupném plnění hracího pole vrátíme až sem,
                    //kombinace s tímto číslem na této pozici není možná, a tak budeme
                    //nuceni na tuto pozici vybrat číslo jiné
                    available[x, y].Remove(number);

                    //Inkrementujeme index, posouváme se vpřed.
                    i++;
                }
            }

            //V této proměnné se uchovají původní vygenerovaná čísla
            //Při kontrole správnosti se porovnávají čísla zadaná do buněk uživatelem s čísly z tohoto pole
            puvodniCisla = new int[9,9];
            foreach (cell cell in grid)
            {
                puvodniCisla[cell.index.X, cell.index.Y] = cell.value;
            }

            List<int[]> zbyvajici = new List<int[]>();
            for (int j = 0; j < 9; j++)
                for (int k = 0; k < 9; k++)
                {
                    zbyvajici.Add(new int[2] { j, k });
                }

            //Odebrání čísel
            for (int j = 0; j < 20 + (uroven * 16); j++)
            {
                if (zbyvajici.Count <= 0) break;

                int nahoda = Func.rnd.Next(zbyvajici.Count);
                int[] random = zbyvajici[nahoda];
                grid[random[0], random[1]].value = 0;
                grid[random[0], random[1]].editable = true;
                grid[random[0], random[1]].highlight = 255;

                //zbyvajici.RemoveAt(nahoda);
            }
        }

        //Seznamu dostupných čísel k doplnění nejprve naplní čísly 1-9
        //A poté čísla, které se již vyskytují 
        void getAvailable(int x, int y, ref List<int> list)
        {
            //Inicializace seznamu
            list = new List<int>();

            //Naplnění počátečními hodnotami 1-9
            for (int i = 1; i <= 9; i++)
                list.Add(i);

            //Odebrání čísel, která se již vyskytují ve stejném
            //sloupci, řádku nebo ve stejné 3x3 oblasti
            for (int i = 0; i < 9; i++)
            {
                if (grid[i, y].value != 0) list.Remove(grid[i, y].value); //Řádek
                if (grid[x, i].value != 0) list.Remove(grid[x, i].value); //Sloupec
            }

            //Skupiny devíti čísel
            Point region = new Point((x / 3) * 3, (y / 3) * 3);
            for (int i = 0; i < 9; i++)
            {
                Point local = new Point(i % 3, i / 3);
                if (grid[region.X + local.X, region.Y + local.Y].value != 0)
                    list.Remove(grid[region.X + local.X, region.Y + local.Y].value);
            }
        }

        int checkIndex;

        //Kontrola čísel
        void checkNumbers()
        {
            int x = checkIndex % 9;
            int y = checkIndex / 9;

            if (grid[x, y].sizeOffset <= 1.1f)
            {
                if (grid[x, y].value != puvodniCisla[x, y])
                {
                    if(grid[x, y].value != 0) grid[x, y].wrong = true;
                    grid[x, y].editable = true;
                }
                else
                {
                    grid[x, y].wrong = false;
                    grid[x, y].editable = false;
                }

                checkIndex++;
            }
            if (grid[x, y].sizeOffset <= 1.05f)
                grid[x, y].sizeOffset = 1.5f;

            if (checkIndex >= 81) checking = false;
        }

        //Animace buněk
        void explode(int force)
        {
            foreach (cell cell in grid)
            {
                cell.pos.X += Func.rnd.Next(-force * 5, force * 5);
                cell.pos.Y += Func.rnd.Next(-force * 5, force * 5);

                cell.velocity = new PointF(Func.rnd.Next(-force, force), Func.rnd.Next(-force, force));
                cell.sizeVelocity = (float)Func.rnd.Next((-force / 3), (force/2)) / 100;

                cell.mass = Func.rnd.Next(10, 30);
                cell.damp = (float)Func.rnd.Next(70, 80) / 100;
            }
        }
    }
}
