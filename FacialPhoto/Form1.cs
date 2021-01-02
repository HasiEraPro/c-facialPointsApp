using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using iText.Kernel.Pdf;
using iText.Layout;
using DrawingImage = System.Drawing.Image;
using Kaliko.ImageLibrary.Scaling;
using PdfImage = iText.Layout.Element.Image;
using iText.Layout.Properties;
using iText.Layout.Element;
using System.IO;
using iText.Kernel.Geom;
using Rectangle = System.Drawing.Rectangle;
using PdfPoint = iText.Kernel.Geom.Point;
using Point = System.Drawing.Point;
using iText.Kernel.Pdf.Canvas.Draw;
using Emgu.CV;
using Emgu.CV.Face;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace FacialPhoto
{
    public partial class Form1 : Form
    {
        public static string answerAssym = "";
        public static string answerSupport = "";
        public static double mmtoPixelRatioL;
        public static double mmtoPixelRatioR;
        public static int xDown, yDown, rectW, rectH; //left pic box mouse clicked down positions for cropping and width and height of the crop
        public static int xRDown, yRDown, rectRW, rectRH; //right picbox mouse click
        private const int FRAMEWIDTH = 952;
        private const int FRAMEHEIGHT = 885;

        bool startSelectionLeft = false; //left pic box crop selection started
        bool startSelectionRight = false; //right picBox crop selection started
      
        public Pen crpPen = new Pen(Color.Blue); //using the same pen for both pic boxes

        bool drawing = false, drawingLine = false, ending = false; //left picbox drawing line
        bool drawingR = false, drawingLineR = false, endingR = false; //right picbox drawing line

        Circle[] circleArray = new Circle[36]; //left picbox drawn circles array
        Point[] locationArray = new Point[36];


        Circle[] circleArrayR = new Circle[36]; //right picbox drawn circles array
        Point[] locationArrayR = new Point[36];

        Line lineL; //left picbox drawn line
        Line lineR;//right pic box drawn line

        //list of the names of the points

        List<String> namesR = new List<string>() //name list for the Right image points

        {
            "V'","Tr","Eu'R","Eu'L","FtR","Ft'L","G'","ExR","PsR","PiR","EnR","N'","Rh'","EnL","PsL","PiL",
            "ExL","ZyR","OrR","Prn","AIR","AI","Or'L","Zy'L","SpI","Ls","CHR",
            "Sti","Sts","Li","SbI","ChL","Go'L","GoR","Pog'","Me'"


        };
        List<String> names = new List<string>() //name list for the left image points

        {
            "V'","Tr","Op'","Tr'","Op'","Go'","G'","Ex","Or'","Ac","Ch","Me'","G'","N'","Rh'","Prn",
            "C'","Sn","Ls","Li","Sbl","Pog'","Gn'"


        };
        double gonialAngleLeft = 0.0;
        double angleRight = 0.0; //use to calculate an angle on the right side of image, don't know what yet

        Point golCircle, gorCircle;  //both points are measured for distance,in left image
        Point golCircleR, gorCircleR;//both points are measured for distance,in right image


        public Form1()
        {
            InitializeComponent();
        }
        Bitmap leftimageLoad; //left pic box loaded image for later undo the crop changes made
        Bitmap rightimageLoad;//right pic box loaded image for later to undo the crop changes made


        private static Bitmap cropAtRect(Bitmap b, Rectangle r)
        {
            Bitmap nb = new Bitmap(r.Width, r.Height);
            using (Graphics g = Graphics.FromImage(nb))
            {
                g.DrawImage(b, -r.X, -r.Y);
                return nb;
            }
        }

        public static Bitmap CropImage(System.Drawing.Image source, int x, int y, int width, int height)
        {
            Rectangle crop = new Rectangle(x, y, width, height);

            var bmp = new Bitmap(crop.Width, crop.Height);
            using (var gr = Graphics.FromImage(bmp))
            {
                gr.DrawImage(source, new Rectangle(0, 0, bmp.Width, bmp.Height), crop, GraphicsUnit.Pixel);
            }
            return bmp;
        }

        private void btnBrowseLeft_Click(object sender, EventArgs e)
        {
            //open file dialog box object
            OpenFileDialog open = new OpenFileDialog();
            //filter picture formats
            open.Filter = "Image Files (*.jpg;*.jpeg;*.gif;*.bmp)|*.jpg;*.jpeg;*.gif;*.bmp";

            if (open.ShowDialog() == DialogResult.OK)
            {
                //display image on the picture box left
                leftimageLoad = new Bitmap(open.FileName);
                picBoxLeft.Image = leftimageLoad;

            }


        }

        private void btnBrowseRight_Click(object sender, EventArgs e)
        {
            //open file dialog box object
            OpenFileDialog open = new OpenFileDialog();
            //filter picture formats
            open.Filter = "Image Files (*.jpg;*.jpeg;*.gif;*.bmp)|*.jpg;*.jpeg;*.gif;*.bmp";

            if (open.ShowDialog() == DialogResult.OK)
            {
                //display image on the picture box right
                rightimageLoad = new Bitmap(open.FileName);
                picBoxRight.Image = rightimageLoad;

            }
        }

       

        private void picBoxLeft_MouseUp(object sender, MouseEventArgs e)
        {

            if (!ending && !startSelectionLeft)
            {
                foreach (Circle item in circleArray)
                {
                    if (item != null)
                    {
                        item._selected = false;
                        item.drawMe();
                    }

                }
            }
        }


        private void picBoxLeft_MouseDown(object sender, MouseEventArgs e)
        {
            if (ending) ending = false;
            if (drawingLine)
            {

                lineL._endLoc = e.Location;



                //line.drawMe();


                btnDraw.BackColor = Color.LightGray;
                String content = Interaction.InputBox("Distance in millimeters", "Distance Between", "0", 500, 500);

                lineL._text = content;

                Form1.mmtoPixelRatioL = Distance(lineL._startLoc, lineL._endLoc, Convert.ToInt32(content));
                Console.WriteLine("left distance ratio" + mmtoPixelRatioL);
                //line.drawText();
                picBoxLeft.Refresh();
                drawingLine = false;
                drawing = false;
                ending = true;

            }
            if ((e.Button == MouseButtons.Left) && drawing)
            {

                //lineStart = e.Location;
                lineL = new Line(picBoxLeft);
                lineL._startLoc = e.Location;
                drawingLine = true;

            }
            if (!ending && !startSelectionLeft)
            {
                foreach (Circle item in circleArray)
                {

                    if (item != null && insideCircle(item._location, e.Location, item._radius))
                    {
                        item._selected = true;
                    }



                }

            }


            if ((e.Button == MouseButtons.Left) && startSelectionLeft)
            {

                crpPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                xDown = e.X;
                yDown = e.Y;
            }



        }

        private void picBoxRight_MouseDown(object sender, MouseEventArgs e)
        {
            if (endingR) endingR = false;

            if (drawingLineR)
            {

                lineR._endLoc = e.Location;
                //lineR.drawMe();


                btnRightDraw.BackColor = Color.LightGray;
                String content = Interaction.InputBox("Distance in millimeters", "Distance Between", "0", 500, 500);

                Form1.mmtoPixelRatioR = Distance(lineR._startLoc, lineR._endLoc, Convert.ToInt32(content));
                Console.WriteLine("right distance ratio" + mmtoPixelRatioR);

                //lineR._text = content;
                //lineR.drawText();

                picBoxRight.Refresh();

                drawingLineR = false;
                drawingR = false;
                endingR = true;

            }
            if ((e.Button == MouseButtons.Left) && drawingR)
            {

                //lineStart = e.Location;
                lineR = new Line(picBoxRight);
                lineR._startLoc = e.Location;
                drawingLineR = true;

            }
            if (!endingR && !startSelectionRight)
            {
                foreach (Circle item in circleArrayR)
                {

                    if (item != null && insideCircle(item._location, e.Location, item._radius))
                    {
                        item._selected = true;
                    }



                }

            }

            if ((e.Button == MouseButtons.Left) && startSelectionRight)
            {

                crpPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                xRDown = e.X;
                yRDown = e.Y;
            }

        }

        private void picBoxRight_MouseMove(object sender, MouseEventArgs e)
        {
            base.OnMouseMove(e);
           
            if (drawingLineR)
            {


                picBoxRight.Refresh();
                lineR._endLoc = e.Location;
                lineR.drawMe();

            }

            foreach (Circle item in circleArrayR)
            {
                if (item != null && item._selected)
                {
                    picBoxRight.Refresh();

                    item._location = new Point(e.X, e.Y);
                    if (String.Equals(item._text, "Go'L")) //Find a circle thats label saved as this and save that location
                    {

                        golCircleR = item._location;

                    }
                    if (String.Equals(item._text, "GoR"))//Find a circle thats label saved as this and save that location
                    {

                        gorCircleR = item._location;

                    }

                }

               

                if (item != null) item.drawMe();
                //if (lineR != null)
                //{
                //    lineR.drawMe();
                //    if (lineR._text != null) lineR.drawText();
                //}
            }

            if ((e.Button == MouseButtons.Left) && startSelectionRight)
            {

                picBoxRight.Refresh();
                //set width and height of the rectangle to crop

                rectRW = e.X - xRDown;
                rectRH = e.Y - yRDown;

                Graphics g = picBoxRight.CreateGraphics();
                g.DrawRectangle(crpPen, xRDown, yRDown, rectRW, rectRH);
                g.Dispose();

            }

        }

        private void picBoxRight_MouseUp(object sender, MouseEventArgs e)
        {
            if (!endingR && !startSelectionRight)
            {
                foreach (Circle item in circleArrayR)
                {
                    if (item != null)
                    {
                        item._selected = false;
                        item.drawMe();
                    }

                }
            }

        }


        static System.Drawing.Image FixedSize(System.Drawing.Image imgPhoto, int Width, int Height)
        {
            /* cropping doesn't work
            Bitmap nBMP = (Bitmap)imgPhoto;
            Rectangle n = new Rectangle(xDown, yDown, rectW, rectH);
            Bitmap bmpCrop = cropAtRect(nBMP, n);
            imgPhoto = (System.Drawing.Image)bmpCrop;
            */

            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;


            nPercentW = ((float)Width / (float)sourceWidth);
            nPercentH = ((float)Height / (float)sourceHeight);
            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = System.Convert.ToInt16((Width -
                              (sourceWidth * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = System.Convert.ToInt16((Height -
                              (sourceHeight * nPercent)) / 2);
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap bmPhoto = new Bitmap(Width, Height,
                              PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
                             imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.Clear(Color.White);
            grPhoto.InterpolationMode =
                    InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();
            return bmPhoto;
        }

        private void btnCropRight_Click(object sender, EventArgs e)
        {
            startSelectionRight = false;//make the flag false selection is over

            Bitmap bmp2 = new Bitmap(picBoxRight.Width, picBoxRight.Height);
            picBoxRight.DrawToBitmap(bmp2, picBoxRight.ClientRectangle);
            Bitmap crpImg = new Bitmap(rectRW, rectRH);

           // Bitmap bmp3 = fillPicBox(picBoxRight); // create a bitmap using picbox measurements colored 

            for (int i = 0; i < rectRW; i++)
            {
                for (int y = 0; y < rectRH; y++)
                {
                    Color pxlclr = bmp2.GetPixel(xRDown + i, yRDown + y);
                    crpImg.SetPixel(i, y, pxlclr);
                }
            }
            //copy the pixels from the croped one to the bmp3 we created 
            //for (int i = 0; i < rectRW; i++) 
           // {
              //  for (int y = 0; y < rectRH; y++)
              //  {
               ///     Color pxlclr = crpImg.GetPixel(i, y);
               //     bmp3.SetPixel(xRDown + i, yRDown + y, pxlclr);
              //  }
           // }

            Bitmap sbitMap = ScaleImage(crpImg, picBoxRight.Width, picBoxRight.Height);
            Bitmap bmp3 = fillPicBox(picBoxRight);
            Graphics g = Graphics.FromImage(bmp3);
            g.DrawImage(sbitMap, new PointF((picBoxRight.Width - sbitMap.Width) / 2, (picBoxRight.Height - sbitMap.Height) / 2));
            //load that image into the picbox,this image will clear the location difference between image and picbox
            picBoxRight.Image = (DrawingImage)bmp3;
            picBoxRight.SizeMode = PictureBoxSizeMode.Zoom;

            picBoxRight.Refresh();//remove the selection rectangle

            btnCropRight.Enabled = false;
            // picBoxRight.Image = FixedSize(picBoxRight.Image, FRAMEWIDTH, FRAMEHEIGHT);
            // picBoxRight.SizeMode = PictureBoxSizeMode.Zoom;
            // picBoxRight.Refresh();

        }

        private void btnRightReset_Click(object sender, EventArgs e)
        {
            picBoxRight.Image = rightimageLoad;
        }

        private void btnRightLoad_Click(object sender, EventArgs e)
        {
            //int countX = 0;
            //int countY = 50;



            //for (int i = 0; i < namesR.Count; i++)
            //{


            //    countX += 50;
            //    if (countX > picBoxLeft.Width - 50) { countX = 10; countY += 100; }

            //    circleArrayR[i] = new Circle(new Point(countX, countY), picBoxRight, Color.Red, 15, i, namesR[i]);

            //}

            //foreach (Circle item in circleArrayR)
            //{
            //    if (item != null) item.drawMe();


            //}
            {
                try
                {
                    if (picBoxRight.Image == null)
                    {
                        return;
                    }

                    string rootDirectory = System.IO.Path.GetFullPath(@"..\..\");
                    string lbpfacepath = rootDirectory + "resources/haarcascade_frontalface_default.xml";
                    string modelpath = rootDirectory + "resources/lbfmodel.yaml.txt";
                    CascadeClassifier classifier = new CascadeClassifier(lbpfacepath);
                    FacemarkLBFParams facemarkLBF = new FacemarkLBFParams();
                   // var thisimg = fillPictureBox(picBoxRight, new Bitmap(picBoxRight.Image));
                    FacemarkLBF facemark = new FacemarkLBF(facemarkLBF);
                    Size s = picBoxRight.Size;
                    
                    var thisimg = resizeImage(picBoxRight.Image, s);
                    var img = new Bitmap(thisimg).ToImage<Bgr, byte>();
                    var imgGray = img.Convert<Gray, byte>();
                    int faceheight;
                    int facewidth;

                    var faces = classifier.DetectMultiScale(imgGray);
                    foreach (var face in faces)
                    {
                        facewidth = face.Width/20;
                        faceheight = face.Height/20;
                    
                    facemark.LoadModel(modelpath);
                    VectorOfVectorOfPointF landmarks = new VectorOfVectorOfPointF();
                    VectorOfRect rects = new VectorOfRect(faces);
                    bool result = facemark.Fit(imgGray, rects, landmarks);
                        if (result)
                        {
                            for (int i = 0; i < faces.Length; i++)
                            {
                                //To View Landmark Points You can uncomment next line!!!!
                                //FaceInvoke.DrawFacemarks(img, landmarks[i], new MCvScalar(0, 255, 0));
                                //state line
                                var p1 = landmarks[i][27];
                                var p2 = landmarks[i][28];
                                var p3 = landmarks[i][29];
                                var p4 = landmarks[i][30];
                                var p5 = landmarks[i][33];
                                var p6 = landmarks[i][51];
                                var p7 = landmarks[i][62];
                                var p8 = landmarks[i][66];
                                var p9 = landmarks[i][57];
                                var p10 = landmarks[i][8];
                                //right eye
                                var p11 = landmarks[i][42];
                                var p12 = landmarks[i][45];
                                var p13 = landmarks[i][43];
                                var p14 = landmarks[i][47];
                                //left eye
                                var p15 = landmarks[i][36];
                                var p16 = landmarks[i][39];
                                var p17 = landmarks[i][37];
                                var p18 = landmarks[i][41];
                                //eyebrows
                                var p19 = landmarks[i][17];
                                var p20 = landmarks[i][26];
                                //nose
                                var p21 = landmarks[i][31];
                                var p22 = landmarks[i][35];
                                //lips
                                var p23 = landmarks[i][48];
                                var p24 = landmarks[i][54];

                                var p25 = landmarks[i][5];
                                var p26 = landmarks[i][11];

                                var p27 = landmarks[i][1];
                                var p28 = landmarks[i][15];
                                var p29 = landmarks[i][0];
                                var p30 = landmarks[i][16];



                                circleArrayR[0] = new Circle(new Point((int)p1.X, (int)p1.Y - faceheight*12), picBoxRight, Color.Red, 15, 0, namesR[0]);
                                circleArrayR[1] = new Circle(new Point((int)p1.X, (int)p1.Y - faceheight * 8), picBoxRight, Color.Red, 15, 1, namesR[1]);
                                circleArrayR[2] = new Circle(new Point((int)p29.X-facewidth*2, (int)p29.Y -faceheight*4), picBoxRight, Color.Red, 15, 2, namesR[2]);
                                circleArrayR[3] = new Circle(new Point((int)p30.X + facewidth*2, (int)p30.Y -faceheight *4), picBoxRight, Color.Red, 15, 3, namesR[3]);
                                circleArrayR[4] = new Circle(new Point((int)p19.X, (int)p19.Y-faceheight*2), picBoxRight, Color.Red, 15, 4, namesR[4]);
                                circleArrayR[5] = new Circle(new Point((int)p20.X - facewidth, (int)p20.Y-faceheight*2), picBoxRight, Color.Red, 15, 5, namesR[5]);
                                circleArrayR[6] = new Circle(new Point((int)p1.X, (int)p1.Y-faceheight*2), picBoxRight, Color.Red, 15, 6, namesR[6]);
                                circleArrayR[7] = new Circle(new Point((int)p15.X-facewidth, (int)p15.Y), picBoxRight, Color.Red, 15, 7, namesR[7]);
                                circleArrayR[8] = new Circle(new Point((int)p17.X, (int)p17.Y-10), picBoxRight, Color.Red, 15, 8, namesR[8]);
                                circleArrayR[9] = new Circle(new Point((int)p18.X, (int)p18.Y), picBoxRight, Color.Red, 15, 9, namesR[9]);
                                circleArrayR[10] = new Circle(new Point((int)p16.X, (int)p16.Y), picBoxRight, Color.Red, 15, 10, namesR[10]);
                                circleArrayR[11] = new Circle(new Point((int)p1.X, (int)p1.Y), picBoxRight, Color.Red, 15, 11, namesR[11]);
                                circleArrayR[12] = new Circle(new Point((int)p2.X, (int)p2.Y), picBoxRight, Color.Red, 15, 12, namesR[12]);
                                circleArrayR[13] = new Circle(new Point((int)p11.X-facewidth, (int)p11.Y), picBoxRight, Color.Red, 15, 13, namesR[13]);
                                circleArrayR[14] = new Circle(new Point((int)p13.X+5, (int)p13.Y-10), picBoxRight, Color.Red, 15, 14, namesR[14]);
                                circleArrayR[15] = new Circle(new Point((int)p14.X+5, (int)p14.Y), picBoxRight, Color.Red, 15, 15, namesR[15]);
                                circleArrayR[16] = new Circle(new Point((int)p12.X+5, (int)p12.Y), picBoxRight, Color.Red, 15, 16, namesR[16]);

                                circleArrayR[17] = new Circle(new Point((int)p27.X, (int)p27.Y), picBoxRight, Color.Red, 15, 17, namesR[17]);
                                circleArrayR[18] = new Circle(new Point((int)p27.X+facewidth*4, (int)p27.Y), picBoxRight, Color.Red, 15, 18, namesR[18]);
                                circleArrayR[19] = new Circle(new Point((int)p4.X, (int)p4.Y), picBoxRight, Color.Red, 15, 19, namesR[19]);
                                circleArrayR[20] = new Circle(new Point((int)p22.X+5, (int)p22.Y-faceheight), picBoxRight, Color.Red, 15, 20, namesR[20]);
                                circleArrayR[21] = new Circle(new Point((int)p21.X-5, (int)p21.Y-faceheight), picBoxRight, Color.Red, 15, 21, namesR[21]);
                                circleArrayR[22] = new Circle(new Point((int)p28.X-facewidth*4, (int)p28.Y), picBoxRight, Color.Red, 15, 22, namesR[22]);
                                circleArrayR[23] = new Circle(new Point((int)p28.X, (int)p28.Y), picBoxRight, Color.Red, 15, 23, namesR[23]);
                                circleArrayR[24] = new Circle(new Point((int)p5.X, (int)p5.Y-10), picBoxRight, Color.Red, 15, 24, namesR[24]);
                                circleArrayR[25] = new Circle(new Point((int)p6.X, (int)p6.Y-faceheight), picBoxRight, Color.Red, 15, 25, namesR[25]);

                                circleArrayR[26] = new Circle(new Point((int)p23.X, (int)p23.Y), picBoxRight, Color.Red, 15, 26, namesR[26]);
                                circleArrayR[27] = new Circle(new Point((int)p8.X, (int)p8.Y), picBoxRight, Color.Red, 15, 27, namesR[27]);
                                circleArrayR[28] = new Circle(new Point((int)p7.X, (int)p7.Y), picBoxRight, Color.Red, 15, 28, namesR[28]);
                                circleArrayR[29] = new Circle(new Point((int)p9.X, (int)p9.Y), picBoxRight, Color.Red, 15, 29, namesR[29]);
                                circleArrayR[30] = new Circle(new Point((int)p9.X, (int)p9.Y+faceheight), picBoxRight, Color.Red, 15, 30, namesR[30]);
                                circleArrayR[31] = new Circle(new Point((int)p24.X, (int)p24.Y), picBoxRight, Color.Red, 15, 31, namesR[31]);
                                circleArrayR[32] = new Circle(new Point((int)p26.X, (int)p26.Y), picBoxRight, Color.Red, 15, 32, namesR[32]);
                                circleArrayR[33] = new Circle(new Point((int)p25.X, (int)p25.Y), picBoxRight, Color.Red, 15, 33, namesR[33]);
                                circleArrayR[34] = new Circle(new Point((int)p10.X, (int)p10.Y-faceheight), picBoxRight, Color.Red, 15, 34, namesR[34]);
                                circleArrayR[35] = new Circle(new Point((int)p10.X, (int)p10.Y), picBoxRight, Color.Red, 15, 35, namesR[35]);

                                foreach (Circle item in circleArrayR)
                                {
                                    if (item != null) item.drawMe();

                                }


                            }
                        }
                    }

                    picBoxRight.Image = img.ToBitmap();



                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }


        }

        private void btnRightDraw_Click(object sender, EventArgs e)
        {

            if (drawingR)
            {
                btnRightDraw.BackColor = Color.LightGray;
                drawingR = false;
                drawingLineR = false;
            }
            else
            {
                drawingR = true;
                btnRightDraw.BackColor = Color.Green;

            }
        }

        private void btnDrawLeft_Click(object sender, EventArgs e)
        {

            if (drawing)
            {
                btnDraw.BackColor = Color.LightGray;
                drawing = false;
                drawingLine = false;
            }
            else
            {
                drawing = true;
                btnDraw.BackColor = Color.Green;

            }



        }

        private void btnLeftcalculate_Click(object sender, EventArgs e)
        {


            double answer = calculateAngle(circleArray);
            answer = Math.Round(answer, 2, MidpointRounding.ToEven);
            gonialAngleLeft = answer;

            Console.WriteLine("Left Angle:" + answer);

            //lblLeftAngle.Text = "Angle:" + answer + "\u00B0";
        }

        private void btnRightCalculate_Click(object sender, EventArgs e)
        {
            double p2X = 0.0, p2Y = 0.0, p1X = 0.0, p1Y = 0.0, p3X = 0.0, p3Y = 0.0;

            foreach (Circle item in circleArrayR)
            {

                if (item != null)
                {
                    if (String.Equals(item._text, "Tr"))
                    {
                        p2X = item._location.X;
                        p2Y = item._location.Y;
                    }

                    else if (item._text == "Go'")
                    {
                        p1X = item._location.X;
                        p1Y = item._location.Y;
                    }
                    else if (item._text == "Gn'")
                    {
                        p3X = item._location.X;
                        p3Y = item._location.Y;
                    }


                }


            }

            double answer = calculateAngle(p1X, p1Y, p2X, p2Y, p3X, p3Y);
            answer = Math.Round(answer, 2, MidpointRounding.ToEven);

            Console.WriteLine("Right Angle:" + answer);
            //lblRightAngle.Text = "Angle:" + answer + "\u00B0";
        }
        private static double Distance(Point pt1, Point pt2, int mmValue)
        {
            var temp1 = Math.Pow((pt1.X - pt2.X), 2);
            var temp2 = Math.Pow((pt1.Y - pt2.Y), 2);
            var result = Math.Sqrt(temp1 + temp2);


            return (mmValue / result);


        }

        private static double Distance(Point pt1, Point pt2)
        {
            var temp1 = Math.Pow((pt1.X - pt2.X), 2);
            var temp2 = Math.Pow((pt1.Y - pt2.Y), 2);
            var result = Math.Sqrt(temp1 + temp2);


            return (result);


        }
        double calcDistanceSide(Point p1, Point p2)
        {
            double distanceInMm = Math.Round(Distance(p1, p2) * mmtoPixelRatioL, 2, MidpointRounding.ToEven);
            Console.WriteLine("distance in mm:" + distanceInMm);

            return distanceInMm;
        }
        double calcDistanceFront(Point p1, Point p2)
        {
            double distanceInMm = Math.Round(Distance(p1, p2) * mmtoPixelRatioR, 2, MidpointRounding.ToEven);
            Console.WriteLine("distance in mm:" + distanceInMm);

            return distanceInMm;
        }
       
        public static DrawingImage resizeImage(DrawingImage imgToResize, Size size)
        {
            return imgToResize;
            //return (DrawingImage)(new Bitmap(imgToResize, size));
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {

            MeasurementEngine.bigonialWidth = calcDistanceFront(golCircleR, gorCircleR);
            Console.WriteLine("distance in mm:" + MeasurementEngine.bigonialWidth);

            msg f2 = new msg();
            f2.ShowDialog();//this way the form1 will hold untill the form 2(msg form ) is closed

            string savePdfFilePath = "";
            SaveFileDialog sf = new SaveFileDialog();
            sf.InitialDirectory = @"C:\Users\Andrew\Documents";
            sf.Title = "Save Pdf in";
            sf.Filter = "Pdf files (*.pdf)|*.pdf|All files (*.*)|*.*";
            if (sf.ShowDialog() == DialogResult.OK)
            {
                savePdfFilePath = System.IO.Path.GetFullPath(sf.FileName);
            }

            PdfWriter writer = new PdfWriter(savePdfFilePath);
            PdfDocument pdf = new PdfDocument(writer);

            float a4Width = PageSize.A4.GetWidth();
            float a4Height = PageSize.A4.GetHeight();
            PageSize pagesize = new PageSize(a4Width, a4Height);
            pdf.SetDefaultPageSize(pagesize);


            Document document = new Document(pdf);



            Pen p = new Pen(Color.Red);
            SolidBrush b = new SolidBrush(Color.Red);
            //Bitmap bmap = new Bitmap(picBoxLeft.Width, picBoxLeft.Height);

            Bitmap bmap = new Bitmap(picBoxLeft.Image, picBoxLeft.Width, picBoxLeft.Height);
            Bitmap bmapRight = new Bitmap(picBoxRight.Image, picBoxRight.Width, picBoxRight.Height);


            bmap.MakeTransparent();
            bmapRight.MakeTransparent();

            Graphics g = Graphics.FromImage(bmap);
            Graphics gr = Graphics.FromImage(bmapRight);

            foreach (Circle item in circleArray)
            {
                if (item != null)
                {

                    g.FillEllipse(b, item._location.X, item._location.Y, item._radius, item._radius);
                    g.DrawString(item._text, new Font(FontFamily.GenericSansSerif, 16, FontStyle.Bold), b, new Point(item._location.X, item._location.Y + item._radius - 70));
                    //g.Dispose();

                }

            }
            foreach (Circle item in circleArrayR)
            {
                if (item != null)
                {

                    gr.FillEllipse(b, item._location.X, item._location.Y, item._radius, item._radius);
                    gr.DrawString(item._text, new Font(FontFamily.GenericSansSerif, 16, FontStyle.Bold), b, new Point(item._location.X, item._location.Y + item._radius - 70));
                    //g.Dispose();

                }

            }

            



            MemoryStream ms = new MemoryStream();
            MemoryStream msR = new MemoryStream();
            bmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            bmapRight.Save(msR, System.Drawing.Imaging.ImageFormat.Jpeg);

            byte[] buff = ms.GetBuffer();
            byte[] buffR = msR.GetBuffer();

            Paragraph header = new Paragraph("Created From Facial Mark App")
               .SetTextAlignment(TextAlignment.CENTER)
               .SetFontSize(20).SetMarginBottom(5);



            PdfImage img = new PdfImage(iText.IO.Image.ImageDataFactory
                                    .Create(buff)).SetTextAlignment(TextAlignment.LEFT);
            PdfImage imgR = new PdfImage(iText.IO.Image.ImageDataFactory
                                    .Create(buffR)).SetTextAlignment(TextAlignment.RIGHT);

            Table table = new Table(1).SetTextAlignment(TextAlignment.CENTER);
            Paragraph p1 = new Paragraph();
             //img.Scale(0.3f, 0.3f);
             //imgR.Scale(0.3f, 0.3f);
            //double AspectratioLeft = (picBoxLeft.Image.Width+0.0) / (picBoxLeft.Image.Height+0.0);
            //double AspectratioRight = picBoxRight.Image.Width / picBoxRight.Image.Height;
           // Console.WriteLine(picBoxLeft.Image.Width); Console.WriteLine(picBoxLeft.Image.Height);
          
           // Console.WriteLine(AspectratioLeft);
            //double newLeftPicHeight = (200 / AspectratioLeft);
            
            //double newLeftPicWidth =(200 * AspectratioLeft);
           

            //double newRightPicHeight = (200/ AspectratioRight);
           // double newRightPicWidth = (200 * AspectratioRight);

            //Console.WriteLine(newRightPicHeight); Console.WriteLine(newRightPicWidth);

            img.ScaleToFit(200f, 200f);
            imgR.ScaleToFit(200f, 200f);
            
            p1.Add(img);
            p1.Add(imgR);

            table.AddCell(p1);

            // Line separator
            LineSeparator ls = new LineSeparator(new SolidLine()).SetMarginBottom(10); ;
            Paragraph subheader = new Paragraph("Answers")
                .SetTextAlignment(TextAlignment.LEFT)
                .SetFontSize(15).SetMarginBottom(10);



            Paragraph answerAssym = new Paragraph("Degree of asymmetry:-" + Form1.answerAssym)
               .SetTextAlignment(TextAlignment.LEFT)
               .SetFontSize(10);

            Paragraph answerSupport = new Paragraph("Degree of undereye support :-" + Form1.answerSupport)
               .SetTextAlignment(TextAlignment.LEFT)
               .SetFontSize(10);


            //"Distance(Side)" + Math.Round(distanceInMm / 10,2,MidpointRounding.ToEven) + "cm" + "                    
            Paragraph distancePara = new Paragraph("Distance(Frontal)" + Math.Round(MeasurementEngine.bigonialWidth /10,2,MidpointRounding.ToEven) + "cm")
              .SetTextAlignment(TextAlignment.LEFT)
              .SetFontSize(10);

            Paragraph anglePara = new Paragraph("The angle created by the line from “Tr to Go” and “Go to Gn’”")
               .SetTextAlignment(TextAlignment.LEFT)
               .SetFontSize(12).SetBold().SetMarginBottom(15);
            if (gonialAngleLeft <= 0.0)
            {
                double answer = calculateAngle(circleArray);
                answer = Math.Round(answer, 2, MidpointRounding.ToEven);
                gonialAngleLeft = answer;

            }

            Paragraph angleParaLeft = new Paragraph("Angle(Side):" + gonialAngleLeft + "\u00B0")
               .SetTextAlignment(TextAlignment.LEFT)
               .SetFontSize(10).SetMarginBottom(10);

            //Paragraph angleParaRight = new Paragraph("Angle(Front):" + angleRight + "\u00B0")
            //   .SetTextAlignment(TextAlignment.LEFT)
            //   .SetFontSize(10).SetMarginBottom(10);

            document.Add(header);
            document.Add(ls);
            document.Add(table);
            document.Add(subheader);
            document.Add(answerAssym);
            document.Add(answerSupport);
            document.Add(distancePara);
            document.Add(anglePara);
            document.Add(angleParaLeft);
            //document.Add(angleParaRight);
            //document.Add(img);
            //document.Add(imgR);
            document.Close();

            MessageBox.Show("Successfully Saved Pdf");

        }

        private void btnRightCalculate_Click_1(object sender, EventArgs e)
        {


            double answer = calculateAngle(circleArrayR);
            answer = Math.Round(answer, 2, MidpointRounding.ToEven);
            angleRight = answer;
            Console.WriteLine("Right Angle:" + answer);
            //lblRightAngle.Text = "Angle:" + answer + "\u00B0";
        }

        private void Form1_MouseEnter(object sender, EventArgs e)
        {

        }

        
        private void picBoxLeft_MouseMove(object sender, MouseEventArgs e)
        {
            base.OnMouseMove(e);
            

            if (drawingLine)
            {


                picBoxLeft.Refresh();
                lineL._endLoc = e.Location;
                lineL.drawMe();

            }

            foreach (Circle item in circleArray)
            {
                if (item != null && item._selected)
                {
                    picBoxLeft.Refresh();

                    item._location = new Point(e.X, e.Y);

                    if (String.Equals(item._text, "GoR") )
                    {

                        golCircle = item._location;    
                            
                    }
                    if (String.Equals(item._text, "Go'L"))
                    {

                        gorCircle = item._location;

                    }

                }



                if (item != null) item.drawMe();
                //if (line != null)
                //{
                //    line.drawMe();
                //    if (line._text != null) line.drawText();
                //}
            }
            if ((e.Button == MouseButtons.Left) && startSelectionLeft)
            {

                picBoxLeft.Refresh();
                //set width and height of the rectangle to crop

                rectW = e.X - xDown;
                rectH = e.Y - yDown;

                Graphics g = picBoxLeft.CreateGraphics();
                g.DrawRectangle(crpPen, xDown, yDown, rectW, rectH);
                g.Dispose();

            }

        }
        private void btnCropInitLeft_Click(object sender, EventArgs e)
        {

            btnCropLeft.Enabled = true;

            startSelectionLeft = true;



        }

        private void btnCropInitRight_Click(object sender, EventArgs e)
        {

            startSelectionRight = true;
            btnCropRight.Enabled = true;
        }

        private void btnCropLeft_Click(object sender, EventArgs e)
        {
            startSelectionLeft = false;//make the flag false selection is over

           //Bitmap bmp2 = fillPicBox(picBoxLeft);
           Bitmap bmp2 = new Bitmap(picBoxLeft.Width, picBoxLeft.Height);
           picBoxLeft.DrawToBitmap(bmp2, picBoxLeft.ClientRectangle);
            Bitmap crpImg = new Bitmap(rectW, rectH);

            

            for (int i = 0; i < rectW; i++)
            {
                for (int y = 0; y < rectH; y++)
                {
                    Color pxlclr = bmp2.GetPixel(xDown + i, yDown + y);
                    crpImg.SetPixel(i, y, pxlclr);
                }
            }





            // for (int i = 0; i < rectW; i++)
            // {
            //   for (int y = 0; y < rectH; y++)
            //  {
            //    Color pxlclr = crpImg.GetPixel(i, y);
            //     bmp3.SetPixel(xDown + i, yDown + y, pxlclr);
            ///  }
            //   }
            Bitmap sbitMap = ScaleImage(crpImg, picBoxLeft.Width, picBoxLeft.Height);
            Bitmap bmp3 = fillPicBox(picBoxLeft);
             Graphics g = Graphics.FromImage(bmp3);
             g.DrawImage(sbitMap, new PointF((picBoxLeft.Width - sbitMap.Width)/2,(picBoxLeft.Height - sbitMap.Height)/2));

            picBoxLeft.Image = (DrawingImage)bmp3;
            picBoxLeft.SizeMode = PictureBoxSizeMode.Zoom;
            picBoxLeft.Refresh();//remove the selection rectangle
            btnCropLeft.Enabled = false;
            //picBoxLeft.Image = FixedSize(picBoxLeft.Image, FRAMEWIDTH, FRAMEHEIGHT);
            //picBoxLeft.SizeMode = PictureBoxSizeMode.Zoom;
            // picBoxLeft.Refresh();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            picBoxLeft.SizeMode = PictureBoxSizeMode.Zoom;
            picBoxRight.SizeMode = PictureBoxSizeMode.Zoom;
            return;
        }
        private void btnUndo_Click(object sender, EventArgs e)
        {
            picBoxLeft.Image = leftimageLoad;
        }



        private void btnLoadPoints_Click(object sender, EventArgs e)
        {

            //int countX = 0;
            //int countY = 50;



            //for (int i = 0; i < names.Count; i++)
            //{


            //    countX += 50;
            //    if (countX > picBoxLeft.Width - 50) { countX = 10; countY += 100; }

            //    circleArray[i] = new Circle(new Point(countX, countY), picBoxLeft, Color.Red, 15, i, names[i]);

            //}

            //foreach (Circle item in circleArray)
            //{
            //    if (item != null) item.drawMe();


            //}
            Size s = picBoxLeft.Size;

            //var thisimg = resizeImage(picBoxLeft.Image, s);
            var image = new Bitmap(picBoxLeft.Image).ToImage<Bgr, byte>();
            if(image.Height> picBoxLeft.Height+picBoxLeft.Height/5 && image.Width > picBoxLeft.Width+picBoxLeft.Width/5)
            {
                picBoxLeft.SizeMode = PictureBoxSizeMode.Zoom;
            }
            else
            {
                picBoxLeft.SizeMode = PictureBoxSizeMode.Zoom;
            }
            


            var grayimg = image.Convert<Gray, Byte>().Clone();
            string rootDirectory = System.IO.Path.GetFullPath(@"..\..\");
            string haarCascadeFilePath = rootDirectory + @"Resources\new_lbpcascade_profileface.xml";

            CascadeClassifier classifier = new CascadeClassifier(haarCascadeFilePath);

            System.Drawing.Rectangle[] facesDetected = classifier.DetectMultiScale(grayimg, 1.1, 3);




            var faces = facesDetected.Length;
            if (faces != 0)
            {
                foreach (var face in facesDetected)
                {
                    Point p = face.Location;
                    int x = face.Width / 20;
                    int y = face.Height / 20;
                    int wid = face.Width;
                    int hei = face.Height;

                    circleArray[0] = new Circle(new Point(p.X + wid + x * 6, p.Y - y * 8), picBoxLeft, Color.Red, 10, 0, names[0]);

                    circleArray[1] = new Circle(new Point(p.X + x * 4, p.Y - y * 4), picBoxLeft, Color.Red, 10, 1, names[1]);

                    circleArray[2] = new Circle(new Point(p.X + wid, p.Y + y * 18), picBoxLeft, Color.Red, 10, 2, names[2]);

                    circleArray[3] = new Circle(new Point(p.X + wid + x * 4, p.Y + y * 6), picBoxLeft, Color.Red, 10, 3, names[3]);

                    //circleArray[4] = new Circle(new Point(p.X, p.Y), picBoxLeft, Color.Red, 10, 4, names[4]);

                    circleArray[5] = new Circle(new Point(p.X + wid + x * 4, p.Y + y * 14), picBoxLeft, Color.Red, 10, 5, names[5]);

                    circleArray[6] = new Circle(new Point(p.X + x, p.Y + y * 4), picBoxLeft, Color.Red, 10, 6, names[6]);

                    circleArray[7] = new Circle(new Point(p.X + x * 10, p.Y + y * 5), picBoxLeft, Color.Red, 10, 7, names[7]);

                    circleArray[8] = new Circle(new Point(p.X + x * 9, p.Y + y * 8), picBoxLeft, Color.Red, 10, 8, names[8]);

                    circleArray[9] = new Circle(new Point(p.X + x * 7, p.Y + y * 12), picBoxLeft, Color.Red, 10, 9, names[9]);

                    circleArray[10] = new Circle(new Point(p.X + x * 9, p.Y + y * 14), picBoxLeft, Color.Red, 10, 10, names[10]);

                    circleArray[11] = new Circle(new Point(p.X + x * 15, p.Y + y * 19), picBoxLeft, Color.Red, 10, 11, names[11]);

                    circleArray[12] = new Circle(new Point(p.X + x * 11, p.Y + hei), picBoxLeft, Color.Red, 10, 12, names[12]);

                    circleArray[13] = new Circle(new Point(p.X + x * 3, p.Y + y * 7), picBoxLeft, Color.Red, 10, 13, names[13]);

                    circleArray[14] = new Circle(new Point(p.X + x * 2, p.Y + y * 9), picBoxLeft, Color.Red, 10, 14, names[14]);

                    circleArray[15] = new Circle(new Point(p.X + x * 1, p.Y + y * 10), picBoxLeft, Color.Red, 10, 15, names[15]);

                    circleArray[16] = new Circle(new Point(p.X + x * 2, p.Y + y * 12), picBoxLeft, Color.Red, 10, 16, names[16]);

                    circleArray[17] = new Circle(new Point(p.X + x * 3, p.Y + y * 13), picBoxLeft, Color.Red, 10, 17, names[17]);

                    circleArray[18] = new Circle(new Point(p.X + x * 5, p.Y + y * 14), picBoxLeft, Color.Red, 10, 18, names[18]);

                    circleArray[19] = new Circle(new Point(p.X + x * 6, p.Y + y * 16), picBoxLeft, Color.Red, 10, 19, names[19]);

                    circleArray[20] = new Circle(new Point(p.X + x * 8, p.Y + y * 17), picBoxLeft, Color.Red, 10, 20, names[20]);

                    circleArray[21] = new Circle(new Point(p.X + x * 9, p.Y + y * 19), picBoxLeft, Color.Red, 10, 21, names[21]);


                    foreach (Circle item in circleArray)
                    {
                        if (item != null) item.drawMe();

                    }

                }
                picBoxLeft.Image = image.ToBitmap();
                picBoxLeft.Refresh();

            }
            else
            {
                MessageBox.Show("face Not detected!");
            }

        }


        private bool insideCircle(Point circle, Point mouse, int radius)
        {
            int dx = circle.X - mouse.X;
            int dy = circle.Y - mouse.Y;


            return (dx * dx + dy * dy <= radius * radius);

        }

        double calculateAngle(double P1X, double P1Y, double P2X, double P2Y,
            double P3X, double P3Y)
        {

            double numerator = P2Y * (P1X - P3X) + P1Y * (P3X - P2X) + P3Y * (P2X - P1X);
            double denominator = (P2X - P1X) * (P1X - P3X) + (P2Y - P1Y) * (P1Y - P3Y);
            double ratio = numerator / denominator;

            double angleRad = Math.Atan(ratio);
            double angleDeg = (angleRad * 180) / Math.PI;

            if (angleDeg < 0)
            {
                angleDeg = 180 + angleDeg;
            }

            return angleDeg;
        }

        double calculateAngle(Circle[] circleArray)
        {
            double P2X = 0.0, P2Y = 0.0, P1X = 0.0, P1Y = 0.0, P3X = 0.0, P3Y = 0.0;

            foreach (Circle item in circleArray)
            {

                if (item != null)
                {
                    if (String.Equals(item._text, "Tr"))
                    {
                        P2X = item._location.X;
                        P2Y = item._location.Y;
                    }

                    else if (item._text == "Go'")
                    {
                        P1X = item._location.X;
                        P1Y = item._location.Y;
                    }
                    else if (item._text == "Gn'")
                    {
                        P3X = item._location.X;
                        P3Y = item._location.Y;
                    }


                }


            }

            double numerator = P2Y * (P1X - P3X) + P1Y * (P3X - P2X) + P3Y * (P2X - P1X);
            double denominator = (P2X - P1X) * (P1X - P3X) + (P2Y - P1Y) * (P1Y - P3Y);
            double ratio = numerator / denominator;

            double angleRad = Math.Atan(ratio);
            double angleDeg = (angleRad * 180) / Math.PI;

            if (angleDeg < 0)
            {
                angleDeg = 180 + angleDeg;
            }

            return angleDeg;
        }


        public Bitmap fillPicBox(PictureBox picbox) 
        {


            Bitmap Bmp = new Bitmap(picbox.Width, picbox.Height);
            using (Graphics gfx = Graphics.FromImage(Bmp))
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(255, 0, 0)))
            {
                gfx.FillRectangle(brush, 0, 0, picbox.Width, picbox.Height);
            }

            return Bmp;




        }
        public static Bitmap ScaleImage(Bitmap bmp, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / bmp.Width;
            var ratioY = (double)maxHeight / bmp.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(bmp.Width * ratio);
            var newHeight = (int)(bmp.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);

            using (var graphics = Graphics.FromImage(newImage))
                graphics.DrawImage(bmp, 0, 0, newWidth, newHeight);

            return newImage;
        }
    }
}

class Circle : UserControl
{
    public Point _location { get; set; } //location of the circle
    public PictureBox _picBox { get; set; } //what picture box circle needs to draw
    public Color _color { get; set; } //color 

    public int _radius { get; set; }//radius of the circle created
    public int _id { get; set; } //the id of the circle created
    public bool _selected; //this circle is selected to move or not
    public String _text;
    public Circle(Point location, PictureBox picBox, Color color, int radius, int id, String text)
    {

        this._location = location;
        this._picBox = picBox;
        this._color = color;
        this._radius = radius;
        this._id = id;
        this._text = text;
    }

    public void drawMe()
    {

        Pen p = new Pen(Color.Red);
        SolidBrush b = new SolidBrush(_color);

        Graphics g = this._picBox.CreateGraphics();

        g.FillEllipse(b, _location.X, _location.Y, this._radius, this._radius);
        g.DrawString(this._text, new Font(FontFamily.GenericSansSerif, 10, FontStyle.Bold), b, new Point(this._location.X+ this._radius + 2, this._location.Y));
        g.Dispose();


    }




}


class Line
{
    public String _text { get; set; }
    public int _width { get; set; } = 5;
    public PictureBox _picBox { get; set; }
    public Point _startLoc { get; set; }
    public Point _endLoc { get; set; }

    public Line(PictureBox picBox)
    {

        this._picBox = picBox;

    }

    public void drawMe()
    {


        Pen linePen = new Pen(Color.Blue);
        linePen.Width = this._width;
        Graphics lineG = this._picBox.CreateGraphics();
        lineG.DrawLine(linePen, this._startLoc, this._endLoc);

        lineG.Dispose();

    }


    public Point midpoint(Point pt1, Point pt2)

    {
        return (new Point((pt1.X + pt2.X) / 2, (pt1.Y + pt2.Y) / 2));


    }

    public void drawText()
    {
        Point mid = midpoint(this._startLoc, this._endLoc);
        Pen linePen = new Pen(Color.Blue);
        linePen.Width = this._width;
        Graphics lineG = this._picBox.CreateGraphics();

        lineG.DrawString(this._text + "cm",
                    new Font(FontFamily.GenericSansSerif, 16, FontStyle.Bold),
                   new SolidBrush(Color.Red),
                    new Point(mid.X, mid.Y - 10));
        lineG.Dispose();

    }


}