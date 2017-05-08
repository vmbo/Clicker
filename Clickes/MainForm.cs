using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Clicker
{
    public partial class MainForm : Form
    {
        static int M = 1;

        static string imageFileName = @"UkraineMap.png";

        Points points;

        public MainForm()
        {
            InitializeComponent();
            InitImage();
        }

        void InitImage()
        {
            try
            {
                Image img = Image.FromFile(imageFileName);
                imagePanel.Width = img.Width * M;
                imagePanel.Height = img.Height * M;
                imagePanel.SizeMode = PictureBoxSizeMode.StretchImage;
                imagePanel.Image = img;
                points = new Points(imageFileName + ".txt");
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("", "Image file not found.");
            }
            catch (OutOfMemoryException)
            {
                MessageBox.Show("", "Bad Image.");
            }

        }

        private void imagePanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen redPen = new Pen(Color.Red, M);
            Pen blackPen = new Pen(Color.Black, M);

            // red current curve
            Point[] ps = Points.VerTextToPointArray(pointsBox.Text, M);
                
            if (ps != null && ps.Length > 1)
            {
                g.DrawLines(redPen, ps);
            }

            // black saved curves
            foreach (var k in points.Curves.Keys)
            {
                // draw curve
                var dots = points.GetScaledCurve(k, M);
                g.DrawLines(blackPen, dots);
                // red curve cap 
                g.DrawLine(redPen, dots[dots.Length - 1], dots[dots.Length - 2]);

                // draw key
                Point p1 = dots.First(), p2 = dots.Last();
                var pos = new PointF
                {
                    X = p1.X * 0.5f + p2.X * 0.5f,
                    Y = p1.Y * 0.5f + p2.Y * 0.5f
                };
                g.DrawString(k, outerPanel.Font, Brushes.Black, pos);
            }

        }

        private void imagePanel_MouseDown(object sender, MouseEventArgs e)
        {
            pointsBox.Text += $"\r\n {e.X / M}, {e.Y / M},";
            imagePanel.Refresh();
        }

        private void pointsBox_TextChanged(object sender, EventArgs e)
        {
            imagePanel.Refresh();
        }

        
        // Zoom in
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            M *= 2;
            InitImage();
            imagePanel.Refresh();
            toolStripButton2.Enabled = M > 1;
        }

        // Zoom out
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            M /= 2;
            InitImage();
            imagePanel.Refresh();
        }

        // Load new image
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                imageFileName = openFileDialog1.FileName;
                InitImage();
            }
        }

        // Save points to file
        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            var key = toolStripTextBox1.Text;
            if (points.Curves.ContainsKey(key))
            {
                if (DialogResult.Cancel == MessageBox.Show("Want to replace?", "", MessageBoxButtons.OKCancel))
                    return;
            }
            points.SaveCurve(key, pointsBox.Text);
            pointsBox.Text = "";
            imagePanel.Refresh();
        }

        // Delete the last point
        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                int n = pointsBox.Text.LastIndexOf('\r');
                if (n != -1)
                    pointsBox.Text = pointsBox.Text.Substring(0, n);
                
            } 
        }

        #region Hilight regions

        private string currentRegionName = null;

        private void imagePanel_MouseMove(object sender, MouseEventArgs e)
        {
            toolStripLabel1.Text = $" M = {M}:  {e.X / M}, {e.Y / M}";

            HiLightRegion(new Point(e.X / M, e.Y / M));
        }

        void HiLightRegion(Point p)
        {
            string selected = points.GetPoligonePointInto(p);
            if (selected != currentRegionName)
            {
                Graphics g = imagePanel.CreateGraphics();
                //if (currentRegionName != null)
                //{
                //    var ps = points.GetScaledPoligone(currentRegionName, M);
                //    g.DrawLines(new Pen(Color.Black, M), ps);
                //}
                imagePanel.Refresh();
                if (selected != null)
                {
                    var ps = points.GetScaledPoligone(selected, M);
                    g.DrawLines(new Pen(Color.Red, M), ps);
                }
                currentRegionName = selected;
            }

        }

        #endregion
    }
}
