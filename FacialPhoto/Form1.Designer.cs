namespace FacialPhoto
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.picBoxLeft = new System.Windows.Forms.PictureBox();
            this.picBoxRight = new System.Windows.Forms.PictureBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnBrowseLeft = new System.Windows.Forms.Button();
            this.btnCropLeft = new System.Windows.Forms.Button();
            this.btnCropInitLeft = new System.Windows.Forms.Button();
            this.btnUndo = new System.Windows.Forms.Button();
            this.btnLoadPoints = new System.Windows.Forms.Button();
            this.btnDraw = new System.Windows.Forms.Button();
            this.btnLeftPrint = new System.Windows.Forms.Button();
            this.lblLeftAngle = new System.Windows.Forms.Label();
            this.btnLeftcalculate = new System.Windows.Forms.Button();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnBrowseRight = new System.Windows.Forms.Button();
            this.btnCropRight = new System.Windows.Forms.Button();
            this.btnCropInitRight = new System.Windows.Forms.Button();
            this.btnRightReset = new System.Windows.Forms.Button();
            this.btnRightLoad = new System.Windows.Forms.Button();
            this.btnRightDraw = new System.Windows.Forms.Button();
            this.btnRightPrint = new System.Windows.Forms.Button();
            this.lblRightAngle = new System.Windows.Forms.Label();
            this.btnRightCalculate = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBoxLeft)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBoxRight)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.picBoxLeft, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.picBoxRight, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel2, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.2807F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 87.7193F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1164, 561);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // picBoxLeft
            // 
            this.picBoxLeft.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picBoxLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picBoxLeft.Image = global::FacialPhoto.Properties.Resources.exampleSide;
            this.picBoxLeft.Location = new System.Drawing.Point(3, 71);
            this.picBoxLeft.Name = "picBoxLeft";
            this.picBoxLeft.Size = new System.Drawing.Size(576, 487);
            this.picBoxLeft.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picBoxLeft.TabIndex = 0;
            this.picBoxLeft.TabStop = false;
            this.picBoxLeft.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picBoxLeft_MouseDown);
            this.picBoxLeft.MouseMove += new System.Windows.Forms.MouseEventHandler(this.picBoxLeft_MouseMove);
            this.picBoxLeft.MouseUp += new System.Windows.Forms.MouseEventHandler(this.picBoxLeft_MouseUp);
            // 
            // picBoxRight
            // 
            this.picBoxRight.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picBoxRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picBoxRight.Image = ((System.Drawing.Image)(resources.GetObject("picBoxRight.Image")));
            this.picBoxRight.Location = new System.Drawing.Point(585, 71);
            this.picBoxRight.Name = "picBoxRight";
            this.picBoxRight.Size = new System.Drawing.Size(576, 487);
            this.picBoxRight.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picBoxRight.TabIndex = 1;
            this.picBoxRight.TabStop = false;
            this.picBoxRight.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picBoxRight_MouseDown);
            this.picBoxRight.MouseMove += new System.Windows.Forms.MouseEventHandler(this.picBoxRight_MouseMove);
            this.picBoxRight.MouseUp += new System.Windows.Forms.MouseEventHandler(this.picBoxRight_MouseUp);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.flowLayoutPanel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flowLayoutPanel1.Controls.Add(this.btnBrowseLeft);
            this.flowLayoutPanel1.Controls.Add(this.btnCropLeft);
            this.flowLayoutPanel1.Controls.Add(this.btnCropInitLeft);
            this.flowLayoutPanel1.Controls.Add(this.btnUndo);
            this.flowLayoutPanel1.Controls.Add(this.btnLoadPoints);
            this.flowLayoutPanel1.Controls.Add(this.btnDraw);
            this.flowLayoutPanel1.Controls.Add(this.btnLeftPrint);
            this.flowLayoutPanel1.Controls.Add(this.lblLeftAngle);
            this.flowLayoutPanel1.Controls.Add(this.btnLeftcalculate);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(576, 62);
            this.flowLayoutPanel1.TabIndex = 2;
            // 
            // btnBrowseLeft
            // 
            this.btnBrowseLeft.Location = new System.Drawing.Point(3, 3);
            this.btnBrowseLeft.Name = "btnBrowseLeft";
            this.btnBrowseLeft.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseLeft.TabIndex = 0;
            this.btnBrowseLeft.Text = "Browse";
            this.btnBrowseLeft.UseVisualStyleBackColor = true;
            this.btnBrowseLeft.Click += new System.EventHandler(this.btnBrowseLeft_Click);
            // 
            // btnCropLeft
            // 
            this.btnCropLeft.Location = new System.Drawing.Point(84, 3);
            this.btnCropLeft.Name = "btnCropLeft";
            this.btnCropLeft.Size = new System.Drawing.Size(75, 23);
            this.btnCropLeft.TabIndex = 1;
            this.btnCropLeft.Text = "Crop";
            this.btnCropLeft.UseVisualStyleBackColor = true;
            this.btnCropLeft.Click += new System.EventHandler(this.btnCropLeft_Click);
            // 
            // btnCropInitLeft
            // 
            this.btnCropInitLeft.Location = new System.Drawing.Point(165, 3);
            this.btnCropInitLeft.Name = "btnCropInitLeft";
            this.btnCropInitLeft.Size = new System.Drawing.Size(75, 23);
            this.btnCropInitLeft.TabIndex = 2;
            this.btnCropInitLeft.Text = "Select Crop";
            this.btnCropInitLeft.UseVisualStyleBackColor = true;
            this.btnCropInitLeft.Click += new System.EventHandler(this.btnCropInitLeft_Click);
            // 
            // btnUndo
            // 
            this.btnUndo.Location = new System.Drawing.Point(246, 3);
            this.btnUndo.Name = "btnUndo";
            this.btnUndo.Size = new System.Drawing.Size(104, 23);
            this.btnUndo.TabIndex = 3;
            this.btnUndo.Text = "Reset  Image";
            this.btnUndo.UseVisualStyleBackColor = true;
            this.btnUndo.Click += new System.EventHandler(this.btnUndo_Click);
            // 
            // btnLoadPoints
            // 
            this.btnLoadPoints.Location = new System.Drawing.Point(356, 3);
            this.btnLoadPoints.Name = "btnLoadPoints";
            this.btnLoadPoints.Size = new System.Drawing.Size(129, 23);
            this.btnLoadPoints.TabIndex = 4;
            this.btnLoadPoints.Text = "Load Points";
            this.btnLoadPoints.UseVisualStyleBackColor = true;
            this.btnLoadPoints.Click += new System.EventHandler(this.btnLoadPoints_Click);
            // 
            // btnDraw
            // 
            this.btnDraw.Location = new System.Drawing.Point(3, 32);
            this.btnDraw.Name = "btnDraw";
            this.btnDraw.Size = new System.Drawing.Size(156, 23);
            this.btnDraw.TabIndex = 5;
            this.btnDraw.Text = "Draw Line";
            this.btnDraw.UseVisualStyleBackColor = true;
            this.btnDraw.Click += new System.EventHandler(this.btnDraw_Click);
            // 
            // btnLeftPrint
            // 
            this.btnLeftPrint.Location = new System.Drawing.Point(165, 32);
            this.btnLeftPrint.Name = "btnLeftPrint";
            this.btnLeftPrint.Size = new System.Drawing.Size(75, 23);
            this.btnLeftPrint.TabIndex = 6;
            this.btnLeftPrint.Text = "Process";
            this.btnLeftPrint.UseVisualStyleBackColor = true;
            // 
            // lblLeftAngle
            // 
            this.lblLeftAngle.AutoSize = true;
            this.lblLeftAngle.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLeftAngle.Location = new System.Drawing.Point(246, 29);
            this.lblLeftAngle.Name = "lblLeftAngle";
            this.lblLeftAngle.Size = new System.Drawing.Size(65, 24);
            this.lblLeftAngle.TabIndex = 7;
            this.lblLeftAngle.Text = "Angle:";
            this.lblLeftAngle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnLeftcalculate
            // 
            this.btnLeftcalculate.Location = new System.Drawing.Point(317, 32);
            this.btnLeftcalculate.Name = "btnLeftcalculate";
            this.btnLeftcalculate.Size = new System.Drawing.Size(125, 23);
            this.btnLeftcalculate.TabIndex = 8;
            this.btnLeftcalculate.Text = "Calculate Angle";
            this.btnLeftcalculate.UseVisualStyleBackColor = true;
            this.btnLeftcalculate.Click += new System.EventHandler(this.btnLeftcalculate_Click);
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.flowLayoutPanel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flowLayoutPanel2.Controls.Add(this.btnBrowseRight);
            this.flowLayoutPanel2.Controls.Add(this.btnCropRight);
            this.flowLayoutPanel2.Controls.Add(this.btnCropInitRight);
            this.flowLayoutPanel2.Controls.Add(this.btnRightReset);
            this.flowLayoutPanel2.Controls.Add(this.btnRightLoad);
            this.flowLayoutPanel2.Controls.Add(this.btnRightDraw);
            this.flowLayoutPanel2.Controls.Add(this.btnRightPrint);
            this.flowLayoutPanel2.Controls.Add(this.lblRightAngle);
            this.flowLayoutPanel2.Controls.Add(this.btnRightCalculate);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(585, 3);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(576, 62);
            this.flowLayoutPanel2.TabIndex = 3;
            // 
            // btnBrowseRight
            // 
            this.btnBrowseRight.Location = new System.Drawing.Point(3, 3);
            this.btnBrowseRight.Name = "btnBrowseRight";
            this.btnBrowseRight.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseRight.TabIndex = 0;
            this.btnBrowseRight.Text = "Browse";
            this.btnBrowseRight.UseVisualStyleBackColor = true;
            this.btnBrowseRight.Click += new System.EventHandler(this.btnBrowseRight_Click);
            // 
            // btnCropRight
            // 
            this.btnCropRight.Location = new System.Drawing.Point(84, 3);
            this.btnCropRight.Name = "btnCropRight";
            this.btnCropRight.Size = new System.Drawing.Size(75, 23);
            this.btnCropRight.TabIndex = 1;
            this.btnCropRight.Text = "Crop";
            this.btnCropRight.UseVisualStyleBackColor = true;
            this.btnCropRight.Click += new System.EventHandler(this.btnCropRight_Click);
            // 
            // btnCropInitRight
            // 
            this.btnCropInitRight.Location = new System.Drawing.Point(165, 3);
            this.btnCropInitRight.Name = "btnCropInitRight";
            this.btnCropInitRight.Size = new System.Drawing.Size(75, 23);
            this.btnCropInitRight.TabIndex = 2;
            this.btnCropInitRight.Text = "Select Crop";
            this.btnCropInitRight.UseVisualStyleBackColor = true;
            this.btnCropInitRight.Click += new System.EventHandler(this.btnCropInitRight_Click);
            // 
            // btnRightReset
            // 
            this.btnRightReset.Location = new System.Drawing.Point(246, 3);
            this.btnRightReset.Name = "btnRightReset";
            this.btnRightReset.Size = new System.Drawing.Size(75, 23);
            this.btnRightReset.TabIndex = 3;
            this.btnRightReset.Text = "Reset image";
            this.btnRightReset.UseVisualStyleBackColor = true;
            this.btnRightReset.Click += new System.EventHandler(this.btnRightReset_Click);
            // 
            // btnRightLoad
            // 
            this.btnRightLoad.Location = new System.Drawing.Point(327, 3);
            this.btnRightLoad.Name = "btnRightLoad";
            this.btnRightLoad.Size = new System.Drawing.Size(117, 23);
            this.btnRightLoad.TabIndex = 4;
            this.btnRightLoad.Text = "Load Points";
            this.btnRightLoad.UseVisualStyleBackColor = true;
            this.btnRightLoad.Click += new System.EventHandler(this.btnRightLoad_Click);
            // 
            // btnRightDraw
            // 
            this.btnRightDraw.Location = new System.Drawing.Point(3, 32);
            this.btnRightDraw.Name = "btnRightDraw";
            this.btnRightDraw.Size = new System.Drawing.Size(124, 23);
            this.btnRightDraw.TabIndex = 5;
            this.btnRightDraw.Text = "Draw Line";
            this.btnRightDraw.UseVisualStyleBackColor = true;
            this.btnRightDraw.Click += new System.EventHandler(this.btnRightDraw_Click);
            // 
            // btnRightPrint
            // 
            this.btnRightPrint.Location = new System.Drawing.Point(133, 32);
            this.btnRightPrint.Name = "btnRightPrint";
            this.btnRightPrint.Size = new System.Drawing.Size(75, 23);
            this.btnRightPrint.TabIndex = 6;
            this.btnRightPrint.Text = "Process";
            this.btnRightPrint.UseVisualStyleBackColor = true;
            // 
            // lblRightAngle
            // 
            this.lblRightAngle.AutoSize = true;
            this.lblRightAngle.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRightAngle.Location = new System.Drawing.Point(214, 29);
            this.lblRightAngle.Name = "lblRightAngle";
            this.lblRightAngle.Size = new System.Drawing.Size(69, 25);
            this.lblRightAngle.TabIndex = 7;
            this.lblRightAngle.Text = "Angle:";
            // 
            // btnRightCalculate
            // 
            this.btnRightCalculate.Location = new System.Drawing.Point(289, 32);
            this.btnRightCalculate.Name = "btnRightCalculate";
            this.btnRightCalculate.Size = new System.Drawing.Size(134, 23);
            this.btnRightCalculate.TabIndex = 8;
            this.btnRightCalculate.Text = "Calculate Angle";
            this.btnRightCalculate.UseVisualStyleBackColor = true;
            this.btnRightCalculate.Click += new System.EventHandler(this.btnRightCalculate_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1164, 561);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Facial  Tag  App";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.MouseEnter += new System.EventHandler(this.Form1_MouseEnter);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picBoxLeft)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBoxRight)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.PictureBox picBoxRight;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button btnBrowseLeft;
        private System.Windows.Forms.Button btnCropLeft;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Button btnBrowseRight;
        private System.Windows.Forms.Button btnCropRight;
        private System.Windows.Forms.Button btnCropInitLeft;
        private System.Windows.Forms.Button btnCropInitRight;
        private System.Windows.Forms.Button btnUndo;
        private System.Windows.Forms.Button btnLoadPoints;
        private System.Windows.Forms.Button btnDraw;
        private System.Windows.Forms.Button btnLeftPrint;
        private System.Windows.Forms.Button btnRightReset;
        private System.Windows.Forms.Button btnRightLoad;
        private System.Windows.Forms.Button btnRightDraw;
        private System.Windows.Forms.Button btnRightPrint;
        private System.Windows.Forms.Label lblLeftAngle;
        private System.Windows.Forms.Label lblRightAngle;
        private System.Windows.Forms.PictureBox picBoxLeft;
        private System.Windows.Forms.Button btnLeftcalculate;
        private System.Windows.Forms.Button btnRightCalculate;
    }
}

