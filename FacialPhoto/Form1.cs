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

using PdfImage = iText.Layout.Element.Image;
using iText.Layout.Properties;
using iText.Layout.Element;
using System.IO;
using iText.Kernel.Geom;

using PdfPoint = iText.Kernel.Geom.Point;
using Point = System.Drawing.Point;
using iText.Kernel.Pdf.Canvas.Draw;

namespace FacialPhoto
{
    public partial class Form1 : Form
    {
        public  static string answerAssym = "";
        public  static string answerSupport = "";
        
        public Form1()
        {
            InitializeComponent();
        }
        Bitmap leftimageLoad; //left pic box loaded image for later undo the crop changes made
        Bitmap rightimageLoad;//right pic box loaded image for later to undo the crop changes made

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

            if (!ending && !startSelection)
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

                circleArrayR[i] = new Circle(new Point(countX, countY), picBoxRight, Color.Red, 25, i, names[i]);

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
            double p2X = 0.0, p2Y = 0.0, p1X = 0.0, p1Y = 0.0, p3X = 0.0, p3Y = 0.0;

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

            double answer = calculateAngle(p1X, p1Y, p2X, p2Y, p3X, p3Y);
            answer = Math.Round(answer, 2, MidpointRounding.ToEven);

            lblLeftAngle.Text = "Angle:" + answer + "\u00B0";
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

        private void btnLeftPrint_Click(object sender, EventArgs e)
        {
            msg f2 = new msg();
            f2.ShowDialog();//this way the form1 will hold untill the form 2(msg form ) is closed

            string savePdfFilePath = "";
            SaveFileDialog sf = new SaveFileDialog();
            sf.InitialDirectory = @"C:\";
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

                Pen linePen = new Pen(Color.Blue);
                if (line != null)
                {
                    linePen.Width = (line == null) ? 10 : line._width;
                    g.DrawLine(linePen, line._startLoc, line._endLoc);
                    gr.DrawLine(linePen, lineR._startLoc, lineR._endLoc);
                    Point mid = line.midpoint(line._startLoc, line._endLoc);
                    Point midR = line.midpoint(lineR._startLoc, lineR._endLoc);
                    g.DrawString(line._text + "cm",
                       new Font(FontFamily.GenericSansSerif, 16, FontStyle.Bold),
                      new SolidBrush(Color.Red),
                       new Point(mid.X, mid.Y - 10));

                    gr.DrawString(lineR._text + "cm",
                          new Font(FontFamily.GenericSansSerif, 16, FontStyle.Bold),
                         new SolidBrush(Color.Red),
                          new Point(midR.X, midR.Y - 10));
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
                img.Scale(0.3f, 0.3f);
                imgR.Scale(0.3f, 0.3f);
                p1.Add(img);
                p1.Add(imgR);

                table.AddCell(p1);

            // Line separator
            LineSeparator ls = new LineSeparator(new SolidLine()).SetMarginBottom(10); ;
            Paragraph subheader = new Paragraph("Answers")
                .SetTextAlignment(TextAlignment.LEFT)
                .SetFontSize(15).SetMarginBottom(20);

          

            Paragraph answerAssym = new Paragraph("Degree of asymmetry:-"+Form1.answerAssym)
               .SetTextAlignment(TextAlignment.LEFT)
               .SetFontSize(10);

            Paragraph answerSupport = new Paragraph("Degree of undereye support :-" + Form1.answerSupport)
               .SetTextAlignment(TextAlignment.LEFT)
               .SetFontSize(10);
            string distnaceLeft="NULL";
            string distanceRight = "NULL";

            if (line != null)
            {

                distnaceLeft = line._text;
            }
            if (lineR != null)
            {

                distanceRight = lineR._text;

            }
            Paragraph distancePara = new Paragraph("Distance(Side)" + distnaceLeft+"cm"+ "Distance(Frontal)" + distanceRight + "cm")
              .SetTextAlignment(TextAlignment.LEFT)
              .SetFontSize(10);

            Paragraph anglePara = new Paragraph("The angle created by the line from “Tr to Go” and “Go to Gn’”")
               .SetTextAlignment(TextAlignment.LEFT)
               .SetFontSize(12).SetBold().SetMarginBottom(15);

            Paragraph angleParaLeft = new Paragraph("Angle(Side):")
               .SetTextAlignment(TextAlignment.LEFT)
               .SetFontSize(10).SetMarginBottom(10);

            Paragraph angleParaRight = new Paragraph("Angle(Front):")
               .SetTextAlignment(TextAlignment.LEFT)
               .SetFontSize(10).SetMarginBottom(10);

            document.Add(header);
            document.Add(ls);
            document.Add(table);
            document.Add(subheader);
            document.Add(answerAssym);
            document.Add(answerSupport);
            document.Add(distancePara);
            document.Add(anglePara);
            document.Add(angleParaLeft);
            document.Add(angleParaRight);
            //document.Add(img);
            //document.Add(imgR);
            document.Close();

            
            
        }

        private void btnRightCalculate_Click_1(object sender, EventArgs e)
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
                if (line != null)
                {
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


                countX += 50;
                if (countX > picBoxLeft.Width - 50) { countX = 10; countY += 100; }

                circleArray[i] = new Circle(new Point(countX, countY), picBoxLeft, Color.Red, 25, i, names[i]);

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
        g.DrawString(this._text, new Font(FontFamily.GenericSansSerif, 16, FontStyle.Bold), b, new Point(this._location.X, this._location.Y + this._radius - 70));
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