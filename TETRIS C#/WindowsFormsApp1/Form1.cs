/*
 * TETRIS_c# .net coded with ❤ By Gabriele Maggi 4ID
 */


using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using NAudio.Wave;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private WaveOutEvent outputDevice;
        private AudioFileReader audioFile;

        forme nuovaforma;
        int size;
        int[,] map = new int[16, 8];   //mappa generata
        int ds = 40;// serve per posizione griglia non da cambiare pls
        int lineRimosse;  // contatore per linee rimosse
        int score;         //contatore per score
        int timerintervallo; //timer per l'intervallo della difficoltà
        int b = 1; 
        int pause = 1;  //mette in pausa il gioco
        public Form1()
        {
            InitializeComponent();
            this.KeyUp += new KeyEventHandler(iniTastiera);  //inizializzo tastiera
            Init();  //inzializo il programma
        }

        public void Init()
        {
            size = 25;
            score = 0;
            lineRimosse = 0;
            nuovaforma = new forme(3, 0);

            playsong();



            //inizializzo il timer senza possibilità di cambiare velocità
            timerintervallo = 500;
            timer1.Interval = timerintervallo;
            timer1.Tick += new EventHandler(aggiorna);

            if (b == 0)
            {
                resetgriglia();
                timer1.Start();
               
            }
            
            Invalidate();
        }

        public void playsong()
        {
            if (outputDevice == null)
            {
                outputDevice = new WaveOutEvent();
                //   outputDevice.PlaybackStopped += OnPlaybackStopped;
            }
            if (audioFile == null)
            {

                audioFile = new AudioFileReader(@".\st.m4a"); // se non funziona formato cambiare con mp4 m4a
                outputDevice.Init(audioFile);
            }
            outputDevice.Play();
        }





        private void iniTastiera(object sender, KeyEventArgs e) // inizializzo i tasti
        {

            switch (e.KeyCode)
            {
                case Keys.Up:    //ruota
                    if (!inters())
                    {
                        resetgriglia();    // refresh della griglia
                        nuovaforma.ruotaforma();    // ruota la forma
                        merge();
                        Invalidate();
                    }
                    break;
                case Keys.Down://scatta in basso
                    score = score + 10;
                    timer1.Interval = 1;
                    break;
                case Keys.Right: //va verso destra finche non rileva bordi
                    if (!bordi(1))
                    {
                        resetgriglia();
                        nuovaforma.Destra();
                        merge();
                        Invalidate();
                    }
                    break;
                case Keys.Left:
                    if (!bordi(-1))
                    {
                        resetgriglia();
                        nuovaforma.Sinistra();
                        merge();
                        Invalidate();
                    }
                    break;
                case Keys.P://mette in pausa
                    pausa();
                    break;


            }

        }

        public bool collisione()  //rileva collisioni
        {
            for (int i = nuovaforma.y; i < nuovaforma.y + nuovaforma.sizegriglia; i++)
            {
                for (int j = nuovaforma.x; j < nuovaforma.x + nuovaforma.sizegriglia; j++)
                {
                    if (j >= 0 && j <= 7)
                    {

                        if (nuovaforma.griglia[i - nuovaforma.y, j - nuovaforma.x] != 0)
                        {
                            if (i + 1 == 16)
                                
                               return true;
                            //label4.Text = "collisione";  // per debug
                            if (map[i + 1, j] != 0)
                            {
                           
                                //label4.Text = "collisione";  // per debug
                                return true;

                            }
                        }
                    }
                }
            }
            return false;
        }

        public bool bordi(int dir) //collide con i bordi di anche gli altri ogg
        {
            for (int c = nuovaforma.y; c < nuovaforma.y + nuovaforma.sizegriglia; c++)
            {
                for (int j = nuovaforma.x; j < nuovaforma.x + nuovaforma.sizegriglia; j++)
                {
                    if (nuovaforma.griglia[c - nuovaforma.y, j - nuovaforma.x] != 0)
                    {
                        if (j + 1 * dir > 7 || j + 1 * dir < 0)
                        {
                            return true;
                        }
                        if (map[c, j + 1 * dir] != 0)
                        {
                            int t = (1 * dir);
                            int k = (j - nuovaforma.x + t);
                            if (k >= nuovaforma.sizegriglia && k < 0)
                            {
                                return true;
                            }

                            if (map[c, j + 1 * dir] != 0)
                            {
                                if (j - nuovaforma.x + 1 * dir >= nuovaforma.sizegriglia || j - nuovaforma.x + 1 * dir < 0)
                                {
                                    return true;
                                }
                                if (nuovaforma.griglia[(c - nuovaforma.y), (j - nuovaforma.x + 1 * dir)] == 0)
                                {
                                    return true;
                                }
                            }




                        }
                    }
                }
            }
            return false;
        }
        public bool inters()
        {
            for (int i = nuovaforma.y; i < nuovaforma.y + nuovaforma.sizegriglia; i++)
            {
                for (int j = nuovaforma.x; j < nuovaforma.x + nuovaforma.sizegriglia; j++)
                {
                    if (j >= 0 && j <= 7)
                    {
                        if (map[i, j] != 0 && nuovaforma.griglia[i - nuovaforma.y, j - nuovaforma.x] == 0)
                            
                            return true;
                    }
                }
            }
            return false;
        }

        public void merge()
        {
            for (int i = nuovaforma.y; i < nuovaforma.y + nuovaforma.sizegriglia; i++)
            {
                for (int j = nuovaforma.x; j < nuovaforma.x + nuovaforma.sizegriglia; j++)
                {
                    if (nuovaforma.griglia[i - nuovaforma.y, j - nuovaforma.x] != 0)
                        map[i, j] = nuovaforma.griglia[i - nuovaforma.y, j - nuovaforma.x];
                }
            }

        }

        private void aggiorna(object sender, EventArgs e) /// aggiorna al tick
        {
            punteggi(); // refresh puteggi
            resetgriglia();  // rimette a 0 la griglia
            if (!collisione())   //muove la forma in giù
            {
                nuovaforma.Giu();
            }
            else
            {
                merge();
                checklinee();  //guardo se ci sono linee da rimuovere
                timer1.Interval = timerintervallo;
                nuovaforma.ResetForma(3, 0);
                if (collisione()) //qunado c'è una collisione resetta la map a 0
                {
                    for (int i = 0; i < 16; i++)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            map[i, j] = 0;
                        }
                    }
                    timer1.Tick -= new EventHandler(aggiorna);
                    timer1.Stop();
                    playsong();
                    Init();
                }
            }
            merge();
            Invalidate();
        }

        public void resetgriglia()
        {  //resetto tutti i cubi della griglia a 0 per evitare collisioni a caso
            for (int i = nuovaforma.y; i < nuovaforma.y + nuovaforma.sizegriglia; i++)
            {
                for (int j = nuovaforma.x; j < nuovaforma.x + nuovaforma.sizegriglia; j++)
                {
                    if (i >= 0 && j >= 0 && i < 16 && j < 8)
                    {
                        if (nuovaforma.griglia[i - nuovaforma.y, j - nuovaforma.x] != 0)
                        {
                            map[i, j] = 0;
                        }
                    }
                }
            }
        }

        public void checklinee() // contorlla le linee da rimuovere            
        {
            int linee = 0;
            int c = 0;
            for (int i = 0; i < 16; i++)
            {
                c = 0;
                for (int t = 0; t < 8; t++)
                {
                    //  label4.Text = "" + c;
                    if (map[i, t] != 0)
                    {
                        c++;
                        if (c == 8)
                        {
                            linee++;
                            score = score + 100;
                            c = 0;

                            for (int j = i; j >= 1; j--)
                            {
                                for (int z = 0; z < 8; z++)
                                {
                                    //riscrivo la linea con quella sopra
                                    map[j, z] = map[j - 1, z];
                                }
                            }

                        }
                    }
                }


            }
            lineRimosse = lineRimosse + linee;
        }
        public void punteggi() // stampa i puinteggi con formato 000000000
        {

            int leng;
            int s;
            int lcount = 9;
            int scount = 9;

            for (int t = 10; t < 100000000;)
            {
                if (lineRimosse >= t)
                {
                    lcount--;
                }
                t = t * 10;
            }
            for (int t = 10; t < 100000000;)
            {
                if (score >= t)
                {
                    scount--;
                }
                t = t * 10;
            }


            leng = lineRimosse.ToString("D").Length + lcount;
            s = score.ToString("D").Length + scount;





            //stampa a video
            label1.Text = score.ToString("D" + s.ToString());
            label2.Text = lineRimosse.ToString("D" + leng.ToString());



        }
        public void disegna_tabella(Graphics g) // disegna la griglia che non si vede perchè è in nero per vederla cambiare pens.black in pens.White
        {
            for (int i = 0; i <= 16; i++)  //disegna righe orizzontali
            {
                g.DrawLine(Pens.Black, new Point(ds, ds + i * size), new Point(ds + 8 * size, ds + i * size));
            }
            for (int i = 0; i <= 8; i++)  //disegna righe verticali
            {
                g.DrawLine(Pens.Black, new Point(ds + i * size, ds), new Point(ds + i * size, ds + 16 * size));
            }

        }

        public void disegnaForma(Graphics e) // disegna la forma all'interno della griglia
        {
            int ds = 40;
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    int sj = (j * size);
                    if (map[i, j] == 1)
                    {
                        e.FillRectangle(Brushes.Red, new Rectangle(ds + sj + 1, ds + i * (size) + 1, size - 1, size - 1));
                    }
                    if (map[i, j] == 2)
                    {
                        e.FillRectangle(Brushes.Fuchsia, new Rectangle(ds + sj + 1, ds + i * (size) + 1, size - 1, size - 1));
                    }
                    if (map[i, j] == 3)
                    {
                        e.FillRectangle(Brushes.AliceBlue, new Rectangle(ds + sj + 1, ds + i * (size) + 1, size - 1, size - 1));
                    }
                    if (map[i, j] == 4)
                    {
                        e.FillRectangle(Brushes.Yellow, new Rectangle(ds + sj + 1, ds + i * (size) + 1, size - 1, size - 1));
                    }
                    if (map[i, j] == 5)
                    {
                        e.FillRectangle(Brushes.Purple, new Rectangle(ds + sj + 1, ds + i * (size) + 1, size - 1, size - 1));
                    }
                    if (map[i, j] == 6)
                    {
                        e.FillRectangle(Brushes.Blue, new Rectangle(ds + sj + 1, ds + i * (size) + 1, size - 1, size - 1));
                    }
                    if (map[i, j] == 7)
                    {
                        e.FillRectangle(Brushes.Orange, new Rectangle(ds + sj + 1, ds + i * (size) + 1, size - 1, size - 1));
                    }
                }
            }
        }

        public void disegnaprossimaforma(Graphics e) // disegna la forma successiva 
        {
            int x = 300, y = 60; //si utilizzano per spostare la posizione della prossima forma
            for (int i = 0; i < nuovaforma.sizeprossimagriglia; i++)
            {
                for (int j = 0; j < nuovaforma.sizeprossimagriglia; j++)
                {
                    int sj = (j * size);
                    //stampo come in disegnaforma solo che la stampo come nextgriglia
                    if (nuovaforma.nextgriglia[i, j] == 1)
                    {
                        e.FillRectangle(Brushes.Red, new Rectangle(x + sj + 1, y + i * (size) + 1, size - 1, size - 1));
                    }
                    if (nuovaforma.nextgriglia[i, j] == 2)
                    {
                        e.FillRectangle(Brushes.Fuchsia, new Rectangle(x + sj + 1, y + i * (size) + 1, size - 1, size - 1));
                    }
                    if (nuovaforma.nextgriglia[i, j] == 3)
                    {
                        e.FillRectangle(Brushes.AliceBlue, new Rectangle(x + sj + 1, y + i * (size) + 1, size - 1, size - 1));
                    }
                    if (nuovaforma.nextgriglia[i, j] == 4)
                    {
                        e.FillRectangle(Brushes.Yellow, new Rectangle(x + sj + 1, y + i * (size) + 1, size - 1, size - 1));
                    }
                    if (nuovaforma.nextgriglia[i, j] == 5)
                    {
                        e.FillRectangle(Brushes.Purple, new Rectangle(x + sj + 1, y + i * (size) + 1, size - 1, size - 1));
                    }
                    if (nuovaforma.nextgriglia[i, j] == 6)
                    {
                        e.FillRectangle(Brushes.Blue, new Rectangle(  x + sj + 1,  y + i * (size) + 1, size - 1, size - 1));
                    }
                    if (nuovaforma.nextgriglia[i, j] == 7)
                    {
                        e.FillRectangle(Brushes.Orange, new Rectangle(x + sj + 1, y + i * (size) + 1, size - 1, size - 1));
                    }
                }
            }
        }

        private void disegna(object sender, PaintEventArgs e) // funzione che raccoglie tutte le funzioni che disegnano sulla form
        {  //in questa funzione racchiudo tutte le funzioni per disegnare le varie forme

            disegna_tabella(e.Graphics);   //disegna tabella
            disegnaForma(e.Graphics);
            disegnaprossimaforma(e.Graphics);//disegna prossima mesh
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }


        private void label3_Click(object sender, EventArgs e) // START
        {
           
                b = 0;
                Init();
            
            
        }

        public void pausa() // funzione per la pausa
        {
            if (pause == 1)
            {
                timer1.Stop();
                pause = 0;
            }
            else if (pause == 0)
            {
                timer1.Start();
                pause = 1;
            }
        }
        private void label5_Click(object sender, EventArgs e) // mette in pausa il gioco
        {
            pausa();
        }

    }
}
