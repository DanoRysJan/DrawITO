using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;
namespace Perceptron
{
    public partial class WindowRecognition : Form
    {
        Perceptron red;
        Bitmap target;
        SolidBrush color;
        bool paint = false;
        int histZero, histOne;
        int[][] pixels;
        int numberToDraw = 1;
        int posXRight, posYRigth, posXLeft, posYLeft, posXUp, posYUp, posXDown, posYDown;
        int posYU, posYR, posYD;
        int posXFinalA, posYFinalA, posXFinalD, posYFinalD;
        int widthFinal, heithFinal;
        List<String> draws = new List<string>();
        //Data to change. It´s segment of your data.
        int typeOutput = 4;
        int idDraw;

        public WindowRecognition(Perceptron p)
        {
            red = p;
            InitializeComponent();
            Training.Checked = true;
            guessDraw.Visible = false;
            drawList();
        }

        private void panelToDraw_MouseUp(object sender, MouseEventArgs e)
        {
            paint = false;
        }

        private void panelToDraw_MouseDown(object sender, MouseEventArgs e)
        {
            paint = true;
        }

        private void panelToDraw_MouseMove(object sender, MouseEventArgs e)
        {
            if (paint)
            {
                color = new SolidBrush(Color.Black);
                //Drawing graphics
                using (Graphics g = panelToDraw.CreateGraphics())
                {
                    //Fill ellipse with color which e is mouse.
                    g.FillEllipse(color, e.X, e.Y, 10, 10);
                }
            }
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            clearScreen();
        }

        private void clearScreen()
        {
            clearPanel();
        }

        private void clearPanel()
        {
            Graphics g1 = panelToDraw.CreateGraphics();
            g1.Clear(panelToDraw.BackColor);
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Save_Click(object sender, EventArgs e)
        {
            handWritingScalar(handwritingRecognition());
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (Training.Checked == true)
            {
                panelToDraw.Width = 255;
                panelToDraw.Height = 255;
                labelNumberToDraw.Visible = true;
                drawToPaint.Visible = false;
                guessDraw.Visible = false;
                richTextBoxShowData.Visible = true;
                IDnum.Visible = true;
                drawToRecognition.Visible = false;
            }
            else
            {
                panelToDraw.Width = 600;
                panelToDraw.Height = 350;
                labelNumberToDraw.Visible = false;
                drawToPaint.Visible = true;
                guessDraw.Visible = true;
                richTextBoxShowData.Visible = false;
                IDnum.Visible = false;
                drawToRecognition.Visible = true;
                Random r = new Random();
                int random = r.Next(0, 3);
                textDraw(random);

            }
        }

        public Bitmap handwritingRecognition()
        {
            //Create bitmap with width and height are equal to panel1
            Bitmap bmp = new Bitmap(panelToDraw.Width, panelToDraw.Height);
            //Use using like dispose function to clean up all resources
            using (Graphics g = Graphics.FromImage(bmp))
            {
                //Fix sixe and location of panel1 in screen
                Rectangle rect = panelToDraw.RectangleToScreen(panelToDraw.ClientRectangle);
                //Copy panel1 from screen
                g.CopyFromScreen(rect.Location, Point.Empty, panelToDraw.Size);
            }

            // Instance of next pixel
            posYU = bmp.Width;
            posYR = 0;
            posYD = bmp.Height;

            //Realice the mapping
            mapping(bmp);

            //Final result of mapping
            posXFinalA = posXLeft;
            posYFinalA = posYUp;
            posXFinalD = posXDown;
            posYFinalD = posYRigth;
            widthFinal = posXFinalD - posYFinalA;
            heithFinal = posYFinalD - posXFinalA;

            //Put value in each pixel into array and not show value of pixels == 0, show value of pixels == 1 
            string[] ids = pixels.Select(a => String.Join("", a.Select(b => b == 0 ? " " : "1"))).ToArray();
            richTextBoxShowData.Lines = ids;

            //Change
            saveBmp(scaleBmp(cutBmp(bmp, posXLeft, posYUp, posXDown, posYRigth), 150, 150));

            histZero = 0;
            histOne = 0;
            posYU = bmp.Width;
            posYR = 0;
            posYD = bmp.Height;
            return scaleBmp(cutBmp(bmp, posXLeft, posYUp, posXDown, posYRigth), 150, 150); // to here
        }

        public void saveBmp(Bitmap bmp)
        {
            string Path = "D:/Dano/Escritorio/DataSet/" + numberToDraw + ".Bmp";
            bool Imagexis = File.Exists(Path);
            while (Imagexis == true)
            {
                numberToDraw += 1;
                Path = "D:/Dano/Escritorio/DataSet/" + numberToDraw + ".Bmp";
                Imagexis = File.Exists(Path);
            }

            bmp.Save("D:/Dano/Escritorio/DataSet/" + numberToDraw + ".Bmp");
            numberToDraw = 1;
        }

        public Bitmap cutBmp(Bitmap img, int x1, int y1, int x2, int y2)
        {
            int y3;
            int aux;
            int x3 = x2 - x1;
            if (y2 < y1)
            {
                aux = y2;
                y3 = y1 - y2;
            }
            else
            {
                aux = y1;
                y3 = y2 - y1;
            }
            if (y1 < y2)
            {
                Rectangle cropRect = new Rectangle(x1, y1, x3, y3);
                Bitmap src = img;
                target = new Bitmap(cropRect.Width, cropRect.Height);
                using (Graphics g = Graphics.FromImage(target))
                {
                    g.DrawImage(src, new Rectangle(0, 0, target.Width, target.Height), cropRect, GraphicsUnit.Pixel);
                }
                return target;
            }
            else if (y2 <= y1)
            {
                Rectangle cropRect = new Rectangle(x1, y3, x3, y3);
                Bitmap src = img;
                target = new Bitmap(cropRect.Width, cropRect.Height);
                using (Graphics g = Graphics.FromImage(target))
                {
                    g.DrawImage(src, new Rectangle(0, 0, target.Width, target.Height), cropRect, GraphicsUnit.Pixel);
                }
                return target;
            }
            return target;
        }

        public Bitmap scaleBmp(Bitmap original, int whidth, int height)
        {
            var rad = Math.Max((double)whidth / original.Width, (double)height / original.Height);
            var newWidth = (int)(original.Width * rad);
            var newHeigth = height;
            var scaleImage = new Bitmap(newWidth, newHeigth);
            Graphics.FromImage(scaleImage).DrawImage(original, 0, 0, newWidth, newWidth);
            Bitmap finalImage = new Bitmap(scaleImage);
            return finalImage;
        }

        public void handWritingScalar(Bitmap bmp)
        {
            // Instance of next pixel
            posYU = bmp.Width;
            posYR = 0;
            posYD = bmp.Height;

            mapping(bmp);
            //Put value in each pixel into array and not show value of pixels == 0, show value of pixels == 1 
            string[] ids = pixels.Select(a => String.Join("", a.Select(b => b == 0 ? " " : "1"))).ToArray();
            string rutaCompleta = "D:/Dano/Escritorio/Dataset-Draw" + "_his.txt";
            using (StreamWriter file = new StreamWriter(rutaCompleta, true))
            {
                file.WriteLine(histZero + "," + histOne + "," + typeOutput + "," + widthFinal + "," + heithFinal);
                file.Close();
            }
            richTextBoxShowData.Lines = ids;

            clearScreen();

            histZero = 0;
            histOne = 0;
            posYU = bmp.Width;
            posYR = 0;
            posYD = bmp.Height;
        }

        private void mapping(Bitmap bmp)
        {
            //Get value of each pixel
            pixels = new int[bmp.Height][];
            for (int y = 0; y < bmp.Height; y++)
            {
                pixels[y] = new int[bmp.Width];
            }
            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    if (bmp.GetPixel(x, y).GetBrightness() < 0.5)
                    {
                        //One refers to black
                        pixels[y][x] = 1;
                        histOne++;

                        //Get the pixel Left
                        if (histOne == 1)
                        {
                            posXLeft = x;
                            posYLeft = y;
                        }
                        //Get the pixel of the Up
                        if (y < posYU)
                        {
                            posYU = y;
                            posXUp = x;
                            posYUp = posYU;
                        }
                        //Get the pixel of the Rigth
                        if (y > posYD)
                        {
                            posYD = y;
                            posXDown = x;
                            posYDown = posYD;
                        }
                        if (y < posYD)
                        {
                            posYD = x;
                            posXDown = posYD;
                            posYDown = y;
                        }
                        //Get the pixel of the Down
                        if (y > posYR)
                        {
                            posYR = y;
                            posXRight = x;
                            posYRigth = posYR;
                        }
                    }
                    else
                    {
                        //Zero refers to white
                        pixels[y][x] = 0;
                        histZero++;
                    }
                }
            }
        }

        public Bitmap handwritingRecognitionRecognition()
        {
            //Create bitmap with width and height are equal to panel1
            Bitmap bmp = new Bitmap(panelToDraw.Width, panelToDraw.Height);
            //Use using like dispose function to clean up all resources
            using (Graphics g = Graphics.FromImage(bmp))
            {
                //Fix sixe and location of panel1 in screen
                Rectangle rect = panelToDraw.RectangleToScreen(panelToDraw.ClientRectangle);
                //Copy panel1 from screen
                g.CopyFromScreen(rect.Location, Point.Empty, panelToDraw.Size);
            }

            // Instance of next pixel
            posYU = bmp.Width;
            posYR = 0;
            posYD = bmp.Height;

            //Realice the mapping
            mapping(bmp);

            //Final result of mapping
            posXFinalA = posXLeft;
            posYFinalA = posYUp;
            posXFinalD = posXDown;
            posYFinalD = posYRigth;
            widthFinal = posXFinalD - posYFinalA;
            heithFinal = posYFinalD - posXFinalA;

            //Put value in each pixel into array and not show value of pixels == 0, show value of pixels == 1 
            string[] ids = pixels.Select(a => String.Join("", a.Select(b => b == 0 ? " " : "1"))).ToArray();
            richTextBoxShowData.Lines = ids;

            //Change
            //saveBmp(scaleBmp(cutBmp(bmp, posXLeft, posYUp, posXDown, posYRigth), 150, 150));

            histZero = 0;
            histOne = 0;
            posYU = bmp.Width;
            posYR = 0;
            posYD = bmp.Height;
            return scaleBmp(cutBmp(bmp, posXLeft, posYUp, posXDown, posYRigth), 150, 150); // to here
        }

        public void handWritingScalarRecognition(Bitmap bmp)
        {
            int zeroNumber;
            int oneNumber;
            int index;
            // Instance of next pixel
            posYU = bmp.Width;
            posYR = 0;
            posYD = bmp.Height;

            mapping(bmp);
            //Put value in each pixel into array and not show value of pixels == 0, show value of pixels == 1 
            string[] ids = pixels.Select(a => String.Join("", a.Select(b => b == 0 ? " " : "1"))).ToArray();
           // string rutaCompleta = "D:/Dano/Escritorio/Dataset-Draw" + "_his.txt";
            //using (StreamWriter file = new StreamWriter(rutaCompleta, true))
            //{
               // file.WriteLine(histZero + "," + histOne + "," + typeOutput + "," + widthFinal + "," + heithFinal);
                //file.Close();
            zeroNumber = histZero;
            oneNumber = histOne;
            index = idDraw;
            //}
            //String[] ids = pixels.Select(a => String.Join("", a).ToArray();
            richTextBoxShowData.Lines = ids;

           // clearScreen();

            histZero = 0;
            histOne = 0;
            posYU = bmp.Width;
            posYR = 0;
            posYD = bmp.Height;
        }

        public void drawNeuronalRecognition(double error)
        {
            
        }

        private void btnRecognition_Click(object sender, EventArgs e)
        {
            Bitmap bmp = handwritingRecognition();
            posYU = bmp.Width;
            posYR = 0;
            posYD = bmp.Height;
            Program p = new Program();
            mapping(bmp);
            //Put value in each pixel into array and not show value of pixels == 0, show value of pixels == 1 
            string[] ids = pixels.Select(a => String.Join("", a.Select(b => b == 0 ? " " : "1"))).ToArray();
            //string rutaCompleta = "D:/Dano/Escritorio/Dataset-Draw" + "_his.txt";
            Console.WriteLine(histOne);
            Console.WriteLine(idDraw);
            p.peticionsalida(red, histOne, idDraw);
            /* using (StreamWriter file = new StreamWriter(rutaCompleta, true))
             {
                 file.WriteLine(histZero + "," + histOne + "," + typeOutput + "," + widthFinal + "," + heithFinal);
                 file.Close();
             }*/
            //String[] ids = pixels.Select(a => String.Join("", a).ToArray();
            richTextBoxShowData.Lines = ids;

            //Console.WriteLine(p.peticionsalida(red, histOne, 1));
            //clearScreen();

            if (p.peticionsalida(red, histOne, idDraw) > 0.0 && p.peticionsalida(red, histOne, idDraw) < 0.9)
            {
                drawToRecognition.Text = "Es una Puerta";
            }
            else
            {
                drawToRecognition.Text = "Es una Cara";
            }

           //Add more conditions . Depend of your neuronal red.
            /**if (p.peticionsalida(red, histOne, idDraw) > 10000 && p.peticionsalida(red, histOne, idDraw) < 200000 )
            {
                drawToRecognition.Text = "Es un Uno";
            }else{
                drawToRecognition.Text = "No puedo reconocer tu dibujo";
            }*/
            
            
            histZero = 0;
            histOne = 0;
            posYU = bmp.Width;
            posYR = 0;
            posYD = bmp.Height;
            Random r = new Random();
            int random = r.Next(0, 3);
            textDraw(random);


            
        }

        private void drawList()
        {
            //Add more draws. Depend of your neuronal.
            draws.Add("Una cara");
            draws.Add("Número Uno");
            draws.Add("Una Puerta");
            draws.Add("Un Oso");
        }

        public void textDraw(int r)
        {
            
            if (r == 0)
            {
                drawToPaint.Text = ""+draws[0];
                idDraw = 1;
            }
            if (r == 1)
            {
                drawToPaint.Text = "" + draws[1];
                idDraw = 10000;
            }
            if (r == 2)
            {
                drawToPaint.Text = "" + draws[2];
                idDraw = 20000;
            }
            if (r == 4)
            {
                drawToPaint.Text = "" + draws[3];
                idDraw = 20000;
            }

        }

    }
}