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

namespace FacialPhoto
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Bitmap leftimageLoad;

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
        int xDown, yDown, rectW, rectH; //mouse clicked down positions for cropping and width and height of the crop
        bool startSelection = false;
        public Pen crpPen = new Pen(Color.White);
        bool drawing = false, drawingLine = false, ending = false;


        Circle[] circleArray = new Circle[10];
        Point[] locationArray = new Point[10];
     
        Line line;

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

        private void picBoxLeft_MouseEnter(object sender, EventArgs e)
        {

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



            picBoxRight.Image = (Image)crpImg;
            picBoxRight.SizeMode = PictureBoxSizeMode.StretchImage;

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

            circleArray[0] = new Circle(new Point(100, 100), picBoxLeft, Color.Red, 50, 1, "A");
            circleArray[1] = new Circle(new Point(150 + 100, 100 + 100), picBoxLeft, Color.Green, 50, 2, "B");

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