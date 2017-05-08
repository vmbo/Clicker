using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Outline
{
    public partial class MainForm : Form
    {

        public MainForm()
        {
            InitializeComponent();
        }

        private void NewImage()
        {
            var bmp = new Bitmap(pBox.Width, pBox.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.FillRectangle(Brushes.White, 0, 0, bmp.Width, bmp.Height);
            }
            pBox.Image = bmp;
        }


        private int Conturing(Bitmap image, int step, Point startPoint)
        {
            var contur = new Contur((Bitmap)pBox.Image, step, startPoint);
            contur.PointsOnBorder();
            var ps = contur.MakeRegion();
            Graphics g = pBox.CreateGraphics();
            g.DrawPolygon(Pens.Red, ps.ToArray());
            return ps.Count();
        }

        bool isDrowing = false;
        Point p0;

        private void pBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int step = Convert.ToInt32(stepBox.Text);
                int n = Conturing((Bitmap)pBox.Image, step, e.Location);
                infoLabel.Text = $"Points = {n}";
            }
            else
            {
                isDrowing = true;
                p0 = e.Location;
            }
        }

        private void pBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrowing)
            {
                Pen pen = new Pen(Color.Gray, 1);
                Graphics g = pBox.CreateGraphics();
                Graphics g2 = Graphics.FromImage(pBox.Image);
                g.DrawLine(pen, p0, e.Location);
                g2.DrawLine(pen, p0, e.Location);

                p0 = e.Location;
            }
        }

        private void pBox_MouseUp(object sender, MouseEventArgs e)
        {
            isDrowing = false;
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewImage();
        }

        private void loadButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pBox.Image = Image.FromFile(openFileDialog1.FileName);
            }
        }
    }
}
