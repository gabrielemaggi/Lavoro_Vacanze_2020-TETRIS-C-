using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    class forme
    {
        public int x;
        public int y;
        public int[,] griglia;
        public int[,] nextgriglia;
        public int sizegriglia;
        public int sizeprossimagriglia;

        public int[,] tetr1 = new int[4, 4]{      //genero il pezzo a |
            {0,0,1,0  },
            {0,0,1,0  },
            {0,0,1,0  },
            {0,0,1,0  },
        };

        public int[,] tetr2 = new int[3, 3]{  //genero pezzo a Z
            {0,2,0 },
            {0,2,2 },
            {0,0,2 },
        };

        public int[,] tetr3 = new int[3, 3]{  //genero pezzo a T
            {0,0,0 },
            {3,3,3 },
            {0,3,0 },
        };

        public int[,] tetr4 = new int[3, 3]{  //genero L
            { 4,0,0 },
            { 4,0,0 },
            { 4,4,0 },
        };

        public int[,] tetr5 = new int[2, 2]{ //genero cubo
            {5, 5},
            {5, 5},
        };
        public int[,] tetr6 = new int[3, 3]{  //genero pezzo a Z
            {0,6,0 },
            {6,6,0 },
            {6,0,0 },
        };
        public int[,] tetr7 = new int[3, 3]{  //genero L
            { 0,0,7 },
            { 0,0,7 },
            { 0,7,7 },
        };


        public forme(int _x, int _y)
        {
            x = _x;
            y = _y;
            griglia = generamatrice();
            sizegriglia = (int)Math.Sqrt(griglia.Length);
            nextgriglia = generamatrice();
            sizeprossimagriglia = (int)Math.Sqrt(nextgriglia.Length);
        }

        public void ResetForma(int _x, int _y)
        {
            x = _x;
            y = _y;
            griglia = nextgriglia;
            sizegriglia = (int)Math.Sqrt(griglia.Length);
            nextgriglia = generamatrice();
            sizeprossimagriglia = (int)Math.Sqrt(nextgriglia.Length);
        }
        public void ruotaforma()
        {
            int[,] temp = new int[sizegriglia, sizegriglia];

            for (int i = 0; i < sizegriglia; i++)
            {
                for (int j = 0; j < sizegriglia; j++)
                {
                    temp[i, j] = griglia[j, (sizegriglia - 1) - i];
                }
            }
            griglia = temp;
            int offset1 = (8 - (x + sizegriglia));
            if (offset1 < 0)
            {
                for (int i = 0; i < Math.Abs(offset1); i++)
                    Sinistra();
            }

            if (x < 0)
            {
                for (int i = 0; i < Math.Abs(x) + 1; i++)
                   Destra();
            }

        }
        public int[,] generamatrice()
        {
            int[,] matrix = tetr1;
            int r0;
          
            Random r = new Random(); // scegle random la prossima forma ma è un po' impreciso per c# perchè qualche volta escono numeri sempre uguali


            switch (r.Next(1, 8))
            {
                case 1:
                    matrix = tetr1;
                    break;
                case 2:
                    matrix = tetr2;
                    break;
                case 3:
                    matrix = tetr3;
                    break;
                case 4:
                    matrix = tetr4;
                    break;
                case 5:
                    matrix = tetr5;
                    break;
                case 6:
                    matrix = tetr6;
                    break;
                case 7:
                    matrix = tetr7;
                    break;
            }
            return matrix;
        }

        public void Giu()
        {
            y++;
        }
        public void Destra()
        {
            x++;
        }
        public void Sinistra()
        {
            x--;
        }
        
    }
}
