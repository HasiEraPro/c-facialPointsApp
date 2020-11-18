using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using DrawingImage = System.Drawing.Image;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using PdfImage = iText.Layout.Element.Image;
using iText.Layout.Properties;
using System.IO;

namespace FacialPhoto
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Bitmap leftimageLoad; //left pic box loaded image for later undo the crop changes made
        Bitmap rightimageLoad;//right pic box loaded image for later to undo the crop changes made

        Graphics leftGraphic;
        Graphics rightGraphic;

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
               // leftGraphic = Graphics.FromImage(leftimageLoad);
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
                picBoxRight.Image = new Bitmap(open.FileName);


            }
        }

        int xDown, yDown, rectW, rectH; //left pic box mouse clicked down positions for cropping and width and height of the crop
        int xRDown, yRDown, rectRW, rectRH; //right picbox mouse click

        bool startSelection = false; //left pic box selection started
        bool startSelectionRight = false; //right picBox selection started

        public Pen crpPen = new Pen(Color.White); //using the same pen for both pic boxes

        bool drawing = false, drawingLine = false, ending = false; //left picbox drawing line
        bool drawingR = false, drawingLineR = false, endingR = false; //right picbox drawing line

        Circle[] circleArray = new Circle[35]; //left picbox drawn circles array
        Point[] locationArray = new Point[35];


        Circle[] circleArrayR = new Circle[35]; //right picbox drawn circles array
        Point[] locationArrayR = new Point[35];

        Line line; //left picbox drawn line
        Line lineR;//right pic box drawn line

        //list of the names of the points
        
        List<String> names = new List<string>()

        {
            "V'","Tr'","Eu'","G'","Op'","Ft","Na'","Ps","Ex","En","Pi","Or'","Rh'","Zy'","Tr","Prn",
            "C'","Al","Ac","Sn","Spl","Ls","Sts","St","Ch","Sti","Li","Sbl","Pog'","Gn'","Me'","Go'","C"


        };

        private void picBoxLeft_MouseUp(object sender, MouseEventArgs e)
        {

            if (!ending && !startSelection) { 
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
        
        private void picBoxRight_MouseDown(object sender, MouseEventArgs e)
        {
            if (endingR) endingR = false;
            if (drawingLineR)
            {

                lineR._endLoc = e.Location;
                lineR.drawMe();


               btnRightDraw.BackColor = Color.LightGray;
                String content = Interaction.InputBox("Enter Your Value from cm", "Distance Between", "10", 500, 500);

                lineR._text = content;
                lineR.drawText();

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


                Refresh();
                lineR._endLoc = e.Location;
                lineR.drawMe();

            }

            foreach (Circle item in circleArrayR)
            {
                if (item != null && item._selected)
                {
                    Refresh();

                    item._location = new Point(e.X, e.Y);


                }

                if (item != null) item.drawMe();
                if (lineR != null)
                {
                    lineR.drawMe();
                    if (lineR._text != null) lineR.drawText();
                }
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

        private void btnCropRight_Click(object sender, EventArgs e)
        {
            startSelectionRight = false;//make the flag false selection is over


            Bitmap bmp2 = new Bitmap(picBoxRight.Width, picBoxRight.Height);
            picBoxRight.DrawToBitmap(bmp2, picBoxRight.ClientRectangle);
            Bitmap crpImg = new Bitmap(rectRW, rectRH);


            for (int i = 0; i < rectRW; i++)
            {
                for (int y = 0; y < rectRH; y++)
                {
                    Color pxlclr = bmp2.GetPixel(xRDown + i, yRDown + y);
                    crpImg.SetPixel(i, y, pxlclr);
                }
            }



            picBoxRight.Image = (DrawingImage)crpImg;
            picBoxRight.SizeMode = PictureBoxSizeMode.StretchImage;

            picBoxRight.Refresh();//remove the selection rectangle
        }

        private void btnRightReset_Click(object sender, EventArgs e)
        {
            picBoxRight.Image = rightimageLoad;
        }

        private void btnRightLoad_Click(object sender, EventArgs e)
        {
            int countX = 0;
            int countY = 50;



            for (int i = 0; i < names.Count; i++)
            {


                countX += 50;
                if (countX > picBoxLeft.Width - 50) { countX = 10; countY += 100; }

                circleArrayR[i] = new Circle(new Point(countX, countY), rightGraphic, Color.Red, 25, i, names[i]);

            }

            foreach (Circle item in circleArrayR)
            {
                if (item != null) item.drawMe();


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

        private void picBoxLeft_MouseDown(object sender, MouseEventArgs e)
        {
            if (ending) ending = false;
            if (drawingLine)
            {

                line._endLoc = e.Location;
                line.drawMe();


                btnDraw.BackColor = Color.LightGray;
                String content = Interaction.InputBox("Enter Your Value from cm", "Distance Between", "10", 500, 500);

                line._text = content;
                line.drawText();

                drawingLine = false;
                drawing = false;
                ending = true;
                
            }
            if ((e.Button == MouseButtons.Left) && drawing)
            {

                //lineStart = e.Location;
                line = new Line(picBoxLeft);
                line._startLoc = e.Location;
                drawingLine = true;

            }
            if (!ending && !startSelection)
            {
                foreach (Circle item in circleArray)
                {

                    if (item != null && insideCircle(item._location, e.Location, item._radius))
                    {
                        item._selected = true;
                    }



                }

            }


            if ((e.Button == MouseButtons.Left) && startSelection)
            {

                crpPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                xDown = e.X;
                yDown = e.Y;
            }

        }

        private void btnLeftcalculate_Click(object sender, EventArgs e)
        {
            double p2X = 0.0, p2Y=0.0, p1X=0.0, p1Y=0.0, p3X=0.0, p3Y=0.0;

            foreach (Circle item in circleArray)
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

            double answer = calculateAngle(p1X,p1Y,p2X,p2Y,p3X,p3Y);
            answer = Math.Round(answer, 2, MidpointRounding.ToEven);

            lblLeftAngle.Text = "Angle:" + answer+ "\u00B0";
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

            lblRightAngle.Text = "Angle:" + answer + "\u00B0";
        }

        private void Form1_MouseEnter(object sender, EventArgs e)
        {

        }

        private void btnLeftPrint_Click(object sender, EventArgs e)
        {
            pdfWrite();
        }

        private void picBoxLeft_MouseMove(object sender, MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (drawingLine)
            {

           
                Refresh();
                line._endLoc = e.Location;
                line.drawMe();

            }
            
            foreach (Circle item in circleArray)
            {
                if (item != null && item._selected)
                {
                    Refresh();

                    item._location = new Point(e.X, e.Y);


                }

                if (item != null) item.drawMe();
                if (line != null) {
                    line.drawMe();
                    if (line._text != null) line.drawText();
                } 
            }

            if ((e.Button == MouseButtons.Left) && startSelection)
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

            startSelection = true;
        }

        private void btnCropInitRight_Click(object sender, EventArgs e)
        {
            startSelectionRight = true;

        }

        private void btnCropLeft_Click(object sender, EventArgs e)
        {
            startSelection = false;//make the flag false selection is over


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



            picBoxLeft.Image = (DrawingImage)crpImg;
            picBoxLeft.SizeMode = PictureBoxSizeMode.StretchImage;

            picBoxLeft.Refresh();//remove the selection rectangle
        }

        private void btnUndo_Click(object sender, EventArgs e)
        {
            picBoxLeft.Image = leftimageLoad;
        }
        
        private void btnDraw_Click(object sender, EventArgs e)
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

        private void btnLoadPoints_Click(object sender, EventArgs e)
        {

            int countX = 0;
            int countY = 50;

         

            for (int i = 0; i < names.Count; i++)
            {

               
                    countX +=50;
                    if (countX > picBoxLeft.Width-50) { countX = 10; countY += 100; }
                  
                    circleArray[i] = new Circle(new Point(countX, countY),leftGraphic, Color.Red, 25, i, names[i]);
               
            }

            foreach (Circle item in circleArray)
            {
                if (item != null) item.drawMe();


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

        public void pdfWrite()
        {

            MemoryStream ms = new MemoryStream();
            picBoxLeft.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            byte[] buff = ms.GetBuffer();

            PdfWriter writer = new PdfWriter("D:\\demo.pdf");
            PdfDocument pdf = new PdfDocument(writer);
            Document document = new Document(pdf);
            Paragraph header = new Paragraph("HEADER")
               .SetTextAlignment(TextAlignment.CENTER)
               .SetFontSize(20);
            PdfImage img = new PdfImage(iText.IO.Image.ImageDataFactory.Create(buff))
                            .SetTextAlignment(TextAlignment.CENTER);
            document.Add(img);
            document.Add(header);
            document.Close();

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
    public Graphics _graphicObj { get; set; }
    public Circle(Point location,Graphics g, Color color, int radius, int id, String text)
    {

        this._location = location;
        // this._picBox = picBox;
        this._graphicObj = g;
        this._color = color;
        this._radius = radius;
        this._id = id;
        this._text = text;
    }

    public void drawMe()
    {

        Pen p = new Pen(Color.Red);
        SolidBrush b = new SolidBrush(_color);

        //Graphics g = this._picBox.CreateGraphics();

       this. _graphicObj.FillEllipse(b, _location.X, _location.Y, this._radius, this._radius);
        this._graphicObj.DrawString(this._text, new Font(FontFamily.GenericSansSerif, 16, FontStyle.Bold), b, new Point(this._location.X, this._location.Y + this._radius - 70));
        this._graphicObj.Dispose();

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


    private Point midpoint(Point pt1, Point pt2)

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