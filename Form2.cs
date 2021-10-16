using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using static System.Math;
namespace WinFormsApp5
{
    public partial class Form2 : Form
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
        public Form2()
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
                textBox3.Enabled = true;
            }
            else
            {
                button2.Enabled = false;
                textBox3.Enabled = false;
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
                listView1.Items.Add("point n." + ++point_count);
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
                    listView1.Items.Add("line n." + ++lines_count);
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
            textBox3.Text = "0";
            textBox4.Text = "0";
            textBox5.Text = "0";
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
            double[,] rotate = { { Cos(PI / 2), Sin(PI / 2), 0 }, { -Sin(PI / 2), Cos(PI / 2), 0 }, { 0, 0, 1 } };
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

        private void button2_Click(object sender, EventArgs e)
        {
            int dx, dy,fi;
            if (current_pol.Length == 0)
            {
                MessageBox.Show("Полигон не выбран");
                return;
            }
            if (current_point.Length == 0)
            {
                MessageBox.Show("Точка не выбрана");
                return;
            }
            try
            {
                fi = int.Parse(textBox3.Text);               
            }
            catch
            {
                MessageBox.Show("Данные неверные");
                return;
            }
            int point= int.Parse(current_point.Substring(8));
            int ind = int.Parse(current_pol.Substring(10));
            var currentl = polygons[ind - 1];
            //вокруг чего вращение
            dx = points[point - 1].X;
            dy = points[point - 1].Y;
            //матрица вращения 
            double[,] rotate = { { Cos(fi*PI / 180), Sin(fi*PI / 180), 0 }, { -Sin(fi*PI / 180), Cos(fi*PI / 180), 0 }, { 0, 0, 1 } };
            //матрица сдвига к начальным координатам
            double[,] mov_to_nach_coord = { { 1, 0, 0 }, { 0, 1, 0 }, { -dx, -dy, 1 } };
            //матрица возвращения обратно
            double[,] mov_to_izn = { { 1, 0, 0 }, { 0, 1, 0 }, { dx, dy, 1 } };
            for (int i = 0; i < currentl.Count; i++)
            {
                Point x = currentl[i];
                double[,] coord = { { x.X, x.Y, 1 } };
               
                double[,] rotated_point = multMatrix(coord, mov_to_nach_coord);
                rotated_point = multMatrix(rotated_point, rotate);
                rotated_point = multMatrix(rotated_point, mov_to_izn);
                x.X = (int)rotated_point[0, 0];
                x.Y = (int)rotated_point[0, 1];
                currentl[i] = x;
               /* Point y = currentl[i];
                double[,] coord2 = { { y.X, y.Y, 1 } };
                rotated_point = multMatrix(coord2, mov_to_nach_coord);
                rotated_point = multMatrix(rotated_point, rotate);
                rotated_point = multMatrix(rotated_point, mov_to_izn);
                y.X = (int)rotated_point[0, 0];
                y.Y = (int)rotated_point[0, 1];
                currentl[i] = y;*/
            }
            polygons[ind - 1] = currentl;
         
            graph.Clear(Color.White);
            DrawAll();
            pictureBox1.Invalidate();

        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton6.Checked)
            {
                button4.Enabled = true;
                textBox4.Enabled = true;
                textBox5.Enabled = true;
            }
            else
            {
                button4.Enabled = false;
                textBox4.Enabled = false;
                textBox5.Enabled = false;
            }
        }

        private void radioButton8_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton8.Checked)
            {
                button6.Enabled = true;
            }
            else
            {
                button6.Enabled = false;           
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            int dx1, dy1, dx2, dy2, dx3, dy3, dx4, dy4,fi;
            if (current_line1.Length == 0||current_line2.Length==0)
            {
                MessageBox.Show("Отрезки не выбраны");
                return;
            }
            int ind1 = int.Parse(current_line1.Substring(7));
            int ind2 = int.Parse(current_line2.Substring(7));
            var currentline1 = lines[ind1 - 1];
            var currentline2 = lines[ind2 - 1];
            dx1 = currentline1.Item1.X;
            dy1 = currentline1.Item1.Y;
            dx2 = currentline1.Item2.X;
            dy2 = currentline1.Item2.Y;
            dx3 = currentline2.Item1.X;
            dy3 = currentline2.Item1.Y;
            dx4 = currentline2.Item2.X;
            dy4 = currentline2.Item2.Y;
            if (FindCrossPoint(dx1, dy1,dx2, dy2, dx3, dy3, dx4, dy4) != null)
            {
                Point? p = FindCrossPoint(dx1, dy1, dx2, dy2, dx3, dy3, dx4, dy4);

                MessageBox.Show("X: "+p?.X+" Y: "+p?.Y);
                graph.DrawEllipse(rpen, (int)(p?.X), (int)(p?.Y),5,5);
                pictureBox1.Invalidate();
            }
            else
            {
                MessageBox.Show("Не пересекаются");
            }
        }
        Point? FindCrossPoint(int x1, int y1, int x2, int y2, int x3, int y3, int x4, int y4)
        {
            float k1, b1, k2, b2;
            k1 = (float)((float)(y2 - y1) / (float)(x2 - x1));
            b1 = y1 - k1 * x1;
            k2 = (float)((float)(y4 - y3) / (float)(x4 - x3));
            b2 = y3 - k2 * x3;
            int x, y;
            if (k1 == k2)
            return null; 
           
            x = (int)((b2 - b1) / (k1 - k2));
            y = (int)(k1 * x + b1);
            if ((x1 > x && x2 > x) || (x1 < x && x2 < x) || (x3 > x && x4 > x) || (x3 < x && x4 < x))
                return null;
            return new Point(x, y);
        }

        private void radioButton9_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton9.Checked)
            {
                button7.Enabled = true;
            }
            else
            {
                button7.Enabled = false;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (current_line2.Length == 0)
            {
                MessageBox.Show("Отрезок не выбран");
                return;
            }
            if (current_point.Length == 0)
            {
                MessageBox.Show("Точка не выбрана");
                return;
            }
            int point = int.Parse(current_point.Substring(8));
            int ind = int.Parse(current_line2.Substring(7));
            Point c = points[point - 1];
            Point b = lines[ind - 1].Item1;
            Point a = lines[ind - 1].Item2;
            if (((b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X)) > 0)
            {
                MessageBox.Show("Точка справа от отрезка");
            }
            else
                MessageBox.Show("Точка слева от отрезка");


            graph.Clear(Color.White);
            DrawAll();
            pictureBox1.Invalidate();
        }

        private void radioButton10_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton10.Checked)
            {
                button8.Enabled = true;
            }
            else
            {
                button8.Enabled = false;
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (current_pol.Length == 0)
            {
                MessageBox.Show("Полигон не выбран");
                return;
            }
            if (current_point.Length == 0)
            {
                MessageBox.Show("Точка не выбрана");
                return;
            }
            int point = int.Parse(current_point.Substring(8));
            int ind = int.Parse(current_pol.Substring(10));
            var currentl = polygons[ind - 1];
            int dx = points[point - 1].X;
            int dy = points[point - 1].Y;
            int border_x = pictureBox1.Width;
            int border_y = pictureBox1.Height;
            int cnt = 0;
            for (int i = 0; i < currentl.Count; i++)
            {
                Point? p;
                //graph.DrawLine(pen, currentl[i].X+10, currentl[i].Y, currentl[i + 1].X+10, currentl[i + 1].Y);
             //   pictureBox1.Invalidate();
                if (i == currentl.Count - 1)
                {
                   // graph.DrawLine(pen, currentl[0].X + 10, currentl[0].Y, currentl[i].X + 10, currentl[i].Y);
                  //  pictureBox1.Invalidate();
                    p = FindCrossPoint(dx, dy, border_x, dy, currentl[0].X, currentl[0].Y, currentl[i].X, currentl[i].Y);
                    if (p != null)
                    {
                        cnt++;
                        graph.DrawLine(pen, dx, dy, border_x, dy);
                        pictureBox1.Invalidate();
                    }
                }
                else
                {
                    p = FindCrossPoint(dx, dy, border_x, dy, currentl[i].X, currentl[i].Y, currentl[i + 1].X, currentl[i + 1].Y);
                    if (p != null)
                    {
                        cnt++;
                        graph.DrawLine(pen, dx, dy, border_x, dy);
                        pictureBox1.Invalidate();
                    }
                }
            }
            if (cnt%2!=0)
            {
                MessageBox.Show("Точка внутри");

            }
            else
            {
                MessageBox.Show("Точка Снаружи");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            int dx, dy, fi, ksi;
            if (current_pol.Length == 0)
            {
                MessageBox.Show("Полигон не выбран");
                return;
            }
            if (current_point.Length == 0)
            {
                MessageBox.Show("Точка не выбрана");
                return;
            }
            try
            {
                fi = int.Parse(textBox4.Text);
                ksi = int.Parse(textBox5.Text);
            }
            catch
            {
                MessageBox.Show("Данные неверные");
                return;
            }
            int point = int.Parse(current_point.Substring(8));
            int ind = int.Parse(current_pol.Substring(10));
            var currentl = polygons[ind - 1];
        
            dx = points[point - 1].X;
            dy = points[point - 1].Y;
            double[,] dilatation = { { fi, 0, 0 }, { 0, ksi, 0 }, { 0, 0, 1 } };
            //матрица сдвига к начальным координатам
            double[,] mov_to_nach_coord = { { 1, 0, 0 }, { 0, 1, 0 }, { -dx, -dy, 1 } };
            //матрица возвращения обратно
            double[,] mov_to_izn = { { 1, 0, 0 }, { 0, 1, 0 }, { dx, dy, 1 } };
            for (int i = 0; i < currentl.Count; i++)
            {
                Point x = currentl[i];
                double[,] coord = { { x.X, x.Y, 1 } };
                double[,] dil_point = multMatrix(coord, mov_to_nach_coord);
                dil_point = multMatrix(dil_point, dilatation);
               dil_point = multMatrix(dil_point, mov_to_izn);
                x.X = (int)dil_point[0, 0];
                x.Y = (int)dil_point[0, 1];
                currentl[i] = x;
             }
            polygons[ind - 1] = currentl;

            graph.Clear(Color.White);
            DrawAll();
            pictureBox1.Invalidate();
        }


    }
}
