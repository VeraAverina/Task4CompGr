using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using static System.Math;
//я здесь что-то сломала
namespace WinFormsApp5
{
    public partial class Form1 : Form
    {
        Bitmap pic;
        Graphics graph;
        List<Point> points = new List<Point>();
        List<(Point, Point)> lines = new List<(Point, Point)>();
        List<List<Point>> polygons = new List<List<Point>>();
        int polygon_count = 0;
        int lines_count = 0;
        int point_count = 0;
        Pen pen = new Pen(Color.Black, 1), rpen = new Pen(Color.Red, 5);
        bool first_of_line = false;
        bool current_polygon = false;
        (int, int) first_coord_of_line;
        //выбранные в списке фигуры
        string current_pol = "";
        string current_point = "";
        string current_line1 = "";
        string current_line2 = "";
        public Form1()
        {
            InitializeComponent();
            pic = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = pic;
            graph = Graphics.FromImage(pictureBox1.Image);
            graph.Clear(Color.White);
        }



        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton4.Checked)
            {
                button2.Enabled = true;
            }
            else
            {
                button2.Enabled = false;
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            first_of_line = false;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
                polygons.Add(new List<Point>());
            else
                if (polygons[polygons.Count - 1].Count == 0)
            {
                polygons.RemoveAt(polygons.Count - 1);
            }
            else
                current_polygon = false;
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (radioButton1.Checked)
            {
                //рисуем точку, добавляем в список
                points.Add(new Point(e.X, e.Y));
                listView1.Items.Add("point n."+ ++point_count);
            }
            else if (radioButton2.Checked)
            {
                if (first_of_line != true) //если точка в отрезке первая
                {
                    first_coord_of_line = (e.X, e.Y);//запоминаем
                    first_of_line = true;
                }
                else
                {
                    //рисуем линию из запомненной точки в поставленную, добавляем в список
                    lines.Add((new Point(first_coord_of_line.Item1, first_coord_of_line.Item2), new Point(e.X, e.Y)));
                    listView1.Items.Add("line n."+ ++lines_count);
                    first_of_line = false;
                }
            }
            else if (radioButton3.Checked)
            {
                polygons[polygons.Count - 1].Add(new Point(e.X, e.Y));
                if (!current_polygon)
                {
                    listView1.Items.Add("polygon n." + ++polygon_count);
                    current_polygon = true;
                }
            }
            DrawAll();
        }
        void DrawAll()
        {
            graph.Clear(Color.White);
            foreach (var x in points)
                pic.SetPixel(x.X, x.Y, Color.Black);
          
            foreach (var x in lines)
                graph.DrawLine(pen, x.Item1.X, x.Item1.Y, x.Item2.X, x.Item2.Y);
          
            foreach (var x in polygons)
            {
                graph.DrawLine(pen, x[0], x[x.Count - 1]);
                for (int i = 0; i < x.Count - 1; i++)
                    graph.DrawLine(pen, x[i], x[i + 1]);
            }

            pictureBox1.Invalidate();
        }

        void PicClear()
        {
            graph.Clear(Color.White);
            radioButton1.Checked = true;
            polygons.Clear();
            points.Clear();
            lines.Clear();
            polygon_count = 0; point_count = 0; lines_count = 0;
            listView1.Clear();
            pictureBox1.Invalidate();
            current_pol = "";
            current_point = "";
            current_line1 = "";
            current_line2 = "";
            textBox1.Text = "0";
            textBox2.Text = "0";
            label1.Text = "Polygon:";
            label2.Text = "Выбр.точка: ";
            label3.Text = "Выбр.отрезок: ";
            label4.Text = "Выбр.полигон: ";
            label5.Text = "Line";

        }
        private void button1_Click(object sender, EventArgs e)
        {
            PicClear();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void radioButton7_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton7.Checked)
            {
                button5.Enabled = true;
            }
            else
            {
                button5.Enabled = false;
            }
        }
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count != 0)
            {
                for (int i = 0; i < listView1.SelectedItems.Count; i++)
                {
                    if (listView1.SelectedItems[i].Text.StartsWith("polygon"))
                    {
                        current_pol = listView1.SelectedItems[i].Text;
                        label4.Text = "Выбр.полигон: " + current_pol;
                    }
                    if (listView1.SelectedItems[i].Text.StartsWith("point"))
                    {
                        current_point = listView1.SelectedItems[i].Text;
                        label2.Text = "Выбр.точка: " + current_point;
                    }
                    if (listView1.SelectedItems[i].Text.StartsWith("line"))
                    {
                        if (current_line2.Length == 0)
                        {
                            current_line2 = listView1.SelectedItems[i].Text;
                        }
                        else
                        {
                            current_line1 = current_line2;
                            current_line2 = listView1.SelectedItems[i].Text;
                        }
                        label3.Text = "Выбр.отрезок: " + current_line2;
                    }
                }
            }
        }
        private void button5_Click(object sender, EventArgs e)
        {
            if (current_line2.Length == 0)
            {
                MessageBox.Show("Ребро не выбрано");
                return;
            }
            //номер выбранной фигуры
            int ind = int.Parse(current_line2.Substring(7));
            var currentline = lines[ind - 1];
            //середина, т.е точка, вокргу которой вращаем
            int dx = (currentline.Item1.X + currentline.Item2.X) / 2;
            int dy = (currentline.Item1.Y + currentline.Item2.Y) / 2;
             //матрица вращения 
            double[,] rotate = { { Cos( PI / 2), Sin( PI / 2), 0 }, { -Sin( PI / 2), Cos(PI / 2), 0 }, { 0, 0, 1 } };
            //матрица сдвига к начальным координатам
            double[,] mov_to_nach_coord = { { 1, 0, 0 }, { 0, 1, 0 }, { -dx, -dy, 1 } };
            //матрица возвращения обратно
            double[,] mov_to_izn = { { 1, 0, 0 }, { 0, 1, 0 }, { dx, dy, 1 } };
            Point x = currentline.Item1;
            double[,] coord = { { x.X, x.Y, 1 } };
            double[,] rotated_point = multMatrix(coord, mov_to_nach_coord);
            rotated_point = multMatrix(rotated_point, rotate);
            rotated_point = multMatrix(rotated_point, mov_to_izn);
            x.X = (int)rotated_point[0, 0];
            x.Y = (int)rotated_point[0, 1];
            currentline.Item1 = x;
            Point y = currentline.Item2;
            double[,] coord2 = { { y.X, y.Y, 1 } };
            rotated_point = multMatrix(coord2, mov_to_nach_coord);
            rotated_point = multMatrix(rotated_point, rotate);
            rotated_point = multMatrix(rotated_point, mov_to_izn);
            y.X = (int)rotated_point[0, 0];
            y.Y = (int)rotated_point[0, 1];
            currentline.Item2 = y;
            lines[ind - 1] = currentline;
            graph.Clear(Color.White);
            DrawAll();
            pictureBox1.Invalidate();
        }
        double[,] multMatrix(double[,] a, double[,] b)
        {
            double[,] res = new double[a.GetLength(0), b.GetLength(1)];
            for (int i = 0; i < a.GetLength(0); i++)
                for (int j = 0; j < b.GetLength(1); j++)
                    for (int z = 0; z < b.GetLength(0); z++)
                        res[i, j] += a[i, z] * b[z, j];
            return res;
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton5.Checked)
            {
                textBox1.Enabled = true;
                textBox2.Enabled = true;
                button3.Enabled = true;
            }
            else
            {
                textBox1.Enabled = false;
                textBox2.Enabled = false;
                button3.Enabled = false;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int dx, dy;
            if (current_pol.Length == 0)
            {
                MessageBox.Show("Полигон не выбран");
                return;
            }
            try
            {
                dx = int.Parse(textBox1.Text);
                dy = int.Parse(textBox2.Text);
            }
            catch
            {
                MessageBox.Show("Данные неверные");
                return;
            }
           //матрица переноса
                double[,] translation = { { 1, 0, 0 }, { 0, 1, 0 }, { dx, dy, 1 } };
                int ind = int.Parse(current_pol.Substring(10));
                var currentl = polygons[ind - 1];
                for (int i = 0; i < currentl.Count; i++)
                {
                    Point x = currentl[i];
                    double[,] coord = { { x.X, x.Y, 1 } };
                    double[,] translated = multMatrix(coord, translation);
                    x.X = (int)translated[0, 0];
                    x.Y = (int)translated[0, 1];
                    currentl[i] = x;
                }
                polygons[ind - 1] = currentl;
            
            graph.Clear(Color.White);
            DrawAll();
            pictureBox1.Invalidate();
        
    }

        private void button4_Click(object sender, EventArgs e)
        {

        }

      
    }
}
