using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dir_with_personal_methods
{
    public partial class Form1 : Form
    {
        int n, m;
        double Nmax, E, W;
        double a = 1.0, b = 2.0, c = 2.0, d = 3.0;

        double function(double x, double y)
        {
            return Math.Sin((3.14159265) * x * y);
        }

        //Прямоугольная область 
        void BoardAdRightForRectangle( double h, double k, int num1, int num2, double[,] v, double[,] f)
        {

            //Правая часть 
            for (int j1 = 1; j1 < num2; j1++)
            {
                for (int i1 = 1; i1 < num1; i1++)
                {
                    double xi = a + i1 * h;
                    double yj = c + j1 * k;
                    double fi = 0;
                    //тест
                    if (radioButton1.Checked == true)
                        //-f = u"xx + u"yy;
                        fi = (3.14159265) * (3.14159265) * (xi * xi + yj * yj) * (function(xi, yj));
                    //основная
                    if (radioButton1.Checked == false)
                        fi = Math.Exp((-1) * xi * Math.Pow(yj, 2));
                    f[i1, j1] = fi;
                    v[i1, j1] = 0;
                }
            }

            
            // Граничные условия 
            for (int i1 = 0; i1 < num1 + 1; i1++)
            {
                double xi = a + (double)i1 * h;
                if (radioButton1.Checked == true)
                {
                    v[i1, 0] = function(xi, c);
                    v[i1, num2] = function(xi, d);
                }
                if (radioButton2.Checked == true)
                {
                    v[i1, 0] = (xi - 1) * (xi - 2);
                    v[i1, num2] = xi * (xi - 1) * (xi - 2);
                }
            }

            for (int j1 = 0; j1 < num2 + 1; j1++)
            {
                double yj = c + (double)j1 * k;
                if (radioButton1.Checked == true)
                {
                    v[0, j1] = function(a, yj);
                    v[num1, j1] = function(b, yj);
                }
                if (radioButton2.Checked == true)
                {
                    v[0, j1] = (yj - 2) * (yj - 3);
                    v[num1, j1] = yj * (yj - 2) * (yj - 3);
                }
            }
        }

        void GetR(int num1, int num2, double h2, double k2, double a2, double [,] v, double [,] r, double [,] f)
        {
            for (int j = 1; j < num2; j++)
            {
                for (int i = 1; i < num1; i++)
                {
                    r[i, j] = (h2 * (v[i + 1, j] + v[i - 1, j]) + k2 * (v[i, j + 1] + v[i, j - 1])) + v[i, j] * a2 - f[i, j];

                }
            }
        }

        void Rectangular()
        {
            Nmax = int.Parse(textBox4.Text);
            E = double.Parse(textBox3.Text);
            n = int.Parse(textBox1.Text);
            m = int.Parse(textBox2.Text);


            if (n < 2)
            {
                textBox1.Text += " Число разбиений по x должно быть >= 2 ";
            }
            if (m < 2)
            {
                textBox1.Text += (" Число разбиений по y должно быть >= 2");
            }



            //Очистка строк и столбцов таблицы
            dataGridView1.Rows.Clear();

            dataGridView1.RowCount = m + 2;
            dataGridView1.ColumnCount = n + 2;



            int S = 0; // Число проведенных итераций 
            double Emax = 0; // Текущее значение прироста
            double Ecur = 0;  // Для подсчета значения текущего значения прироста

            double a2, k2, h2; // ненулевые элементы матрицы А кудрявая 


            double[,] v = new double[n + 1, m + 1];  // искомый вектор 
            double[,] f = new double[n + 1, m + 1];
            double Ts;  // параметр текущей невязки


            int i, j;
            double v_old;
            double v_new;


            double h = (b - a) / n;
            double k = (d - c) / m;
            h2 = (-1.0) * (n / (b - a)) * (n / (b - a));
            k2 = (-1.0) * (m / (d - c)) * (m / (d - c));
            a2 = (-2.0) * (h2 + k2);

            BoardAdRightForRectangle(h, k, n, m, v, f);

            bool flag = true;
            double[,] r = new double[n + 1, m + 1]; //  Невязка 
            S = 0;
            while (S < Nmax && flag)
            {
                Emax = 0;

                GetR(n, m, h2, k2, a2, v, r, f);
                double temps = 0.0, Ars = 0.0, doub_Ars = 0.0;

                for (j = 1; j < m; j++)
                {
                    for (i = 1; i < n; i++)
                    {
                        temps = (r[i, j] * a2 + h2 * (r[i + 1, j] + r[i - 1, j]) + k2 * (r[i, j + 1] + r[i, j - 1]));
                        Ars += temps * r[i, j];
                        doub_Ars += temps * temps;
                    }
                }
                Ts = Ars / doub_Ars;
                for (j = 1; j < m; j++)
                {
                    for (i = 1; i < n; i++)
                    {
                        v_old = v[i, j];
                        v_new = v_old - Ts * r[i, j];
                        Ecur = Math.Abs(v_old - v_new); // модуль 
                        if (Ecur > Emax)
                        {
                            Emax = Ecur;
                        }
                        v[i, j] = v_new;
                    }
                }


                S++;
                if (Emax < E)
                    flag = false;

            }
            label11.Text = Convert.ToString(Emax);
            label8.Text = Convert.ToString(S);

            //получим вектор с половинным шагом 
            double[,] vh = new double[2 * n + 1, 2 * m + 1];

            if (radioButton2.Checked == true)
            {
                double[,] fh = new double[2 * n + 1, 2 * m + 1];

                double hh = (b - a) / (2 * n);
                double kh = (d - c) / (2 * m);
                double hh2 = (-1.0) * (2 * n / (b - a)) * (2 * n / (b - a));
                double kh2 = -(1 / kh) * (1 / kh);
                double ah2 = (-2.0) * (hh2 + kh2);

                BoardAdRightForRectangle(hh, kh, 2 * n, 2 * m, vh, fh);

                bool flag1 = true;
                double[,] rh = new double[2 * n + 1, 2 * m + 1];
                S = 0;
                while (S < Nmax && flag1)
                {
                    Emax = 0;

                    GetR(2 * n, 2 * m, hh2, kh2, ah2, vh, rh, fh);
                    double temps = 0.0, Ars = 0.0, doub_Ars = 0.0;

                    for (j = 1; j < 2 * m; j++)
                    {
                        for (i = 1; i < 2 * n; i++)
                        {
                            temps = (rh[i, j] * ah2 + hh2 * (rh[i + 1, j] + rh[i - 1, j]) + kh2 * (rh[i, j + 1] + rh[i, j - 1]));
                            Ars += temps * rh[i, j];
                            doub_Ars += temps * temps;
                        }
                    }
                    Ts = Ars / doub_Ars;
                    for (j = 1; j < 2 * m; j++)
                    {
                        for (i = 1; i < 2 * n; i++)
                        {
                            v_old = vh[i, j];
                            v_new = v_old - Ts * rh[i, j];
                            Ecur = Math.Abs(v_old - v_new); // модуль 
                            if (Ecur > Emax)
                            {
                                Emax = Ecur;
                            }
                            vh[i, j] = v_new;
                        }
                    }


                    S++;
                    if (Emax < E)
                        flag1 = false;

                }
                label17.Text = Convert.ToString(Emax);
                label15.Text = Convert.ToString(S);

            }



            //Невязка 
            double r_evk = 0;
            for (int jj = 1; jj < m; jj++)
            {
                for (int ii = 1; ii < n; ii++)
                {
                    r[ii, jj] = (h2 * (v[ii + 1, jj] + v[ii - 1, jj]) + k2 * (v[ii, jj + 1] + v[ii, jj - 1])) + v[ii, jj] * a2 - f[ii, jj];
                    r_evk += r[ii, jj] * r[ii, jj];
                }
            }
            r_evk = Math.Sqrt(r_evk);
            label19.Text = Convert.ToString(r_evk);


            // Точность 
            double xi1 = 0.0, yj1 = 0.0;
            double maxE = 0;
            double curE = 0;
            for (j = 0; j < m + 1; j++)
            {
                for (i = 0; i < n + 1; i++)
                {

                    if (radioButton1.Checked == true)
                    {
                        xi1 = (double)i * h + a;
                        yj1 = (double)j * k + c;
                        curE = Math.Abs(function(xi1, yj1) - v[i, j]);
                    }
                    else
                        curE = Math.Abs(v[i, j] - vh[2 * i, 2 * j]);
                    if (curE >= maxE)
                    {
                        maxE = curE;
                    }
                }
            }
            label13.Text = Convert.ToString(maxE);

            //X
            dataGridView1.Rows[0].Cells[0].Value = "X / Y ";

            for (int i1 = 1; i1 < n + 2; i1++)
            {
                xi1 = (double)(i1 - 1.0) * h + a;

                dataGridView1.Rows[0].Cells[i1].Style.BackColor = System.Drawing.Color.OrangeRed;
                dataGridView1.Rows[0].Cells[i1].Value = xi1;
            }

            //Y

            int rr = 1;
            for (int j1 = m + 1; j1 > 0; j1--)
            {
                yj1 = (double)(rr - 1) * k + c;

                dataGridView1.Rows[j1].Cells[0].Style.BackColor = System.Drawing.Color.OrangeRed;
                dataGridView1.Rows[j1].Cells[0].Value = yj1;

                rr++;
            }


            //table
            for (int j1 = 1; j1 < m + 2; j1++)
            {
                for (int i1 = 1; i1 < n + 2; i1++)
                {
                    //dataGridView1.Rows[j1].Cells[i1].Style.BackColor = System.Drawing.Color.SlateGray;
                    dataGridView1.Rows[j1].Cells[i1].Value = v[i1 - 1, m + 1 - j1];
                }

            }
        }

        //Своя область 
        double a1 = 0.5, b1 = 0.75, c1 = 0.25, d1 = 0.75; //Мини квадратик 

        void BoardAdRightForRectangle2(double h, double k, int num1, int num2, double[,] v, double[,] f)
        {

            //Правая часть 
            for (int j1 = 1; j1 < num2; j1++)
            {
                for (int i1 = 1; i1 < num1; i1++)
                {
                    //Обходим вырезанный угол
                    if (i1 < num1 / 4 && j1 > 3 * num2 / 4)
                        continue;
                    //Обходим вырезанную область 
                    if (i1 > num1 / 2 && i1 < 3 * num1 / 4 && j1 > num2 / 4 && j1 < 3 * num2 / 4)
                        continue;

                    double xi = a + i1 * h;
                    double yj = c + j1 * k;
                    double fi = 0;
                    //тест
                    if (radioButton1.Checked == true)
                        //-f = u"xx + u"yy;
                        fi = (3.14159265) * (3.14159265) * (xi * xi + yj * yj) * (function(xi, yj));
                    //основная
                    if (radioButton1.Checked == false)
                        fi = Math.Exp((-1) * xi * Math.Pow(yj, 2));
                    f[i1, j1] = fi;
                    v[i1, j1] = 0;
                }
            }


            // Граничные условия 
            for (int i1 = 0; i1 < num1 + 1; i1++)
            {
                double xi = a + (double)i1 * h;
                if (radioButton1.Checked == true)
                {
                    v[i1, 0] = function(xi, c);

                    if (i1 >= 0.5 * num1 && i1 <= 0.75 * num1)
                    {
                        v[i1, num2/4] = function(xi, c1);
                        v[i1, 3 * num2 / 4] = function(xi,d1);
                    }
                    if (i1 <= 0.25 * num1)
                        v[i1, 3 * num2 / 4] = function(xi,d1);
                    else
                        v[i1,num2] = function(xi, d);
                    
                }
                if (radioButton2.Checked == true)
                {
                    v[i1, 0] = (xi - 1) * (xi - 2);
                    
                    if (i1 >= 0.5 * num1 && i1 <= 0.75 * num1)
                    {
                        v[i1, num2 / 4] = (xi - 1) * (xi - 2);
                        v[i1, 3 * num2 / 4] = xi * (xi - 1) * (xi - 2);
                    }
                    if (i1 <= 0.25 * num1)
                        v[i1, 3 * num2 / 4] = xi * (xi - 1) * (xi - 2);
                    else
                        v[i1, num2] = xi * (xi - 1) * (xi - 2);
                }
            }

            for (int j1 = 0; j1 < num2 + 1; j1++)
            {
                double yj = c + (double)j1 * k;
                if (radioButton1.Checked == true)
                {
                    v[num1, j1] = function(b, yj);

                    if (j1 <= 3*num2 / 4)
                    {
                        v[0, j1] = function(a, yj);
                        if (j1 >= num2/4)
                        {
                            v[num1/2,j1] = function(a1,yj);
                            v[3*num1/4,j1] = function(b1,yj);
                        }
                    }
                    else 
                        v[num1/4,j1] = function(d-d1,yj);
                }
                if (radioButton2.Checked == true)
                {
                    v[num1, j1] = yj * (yj - 2) * (yj - 3);

                    if (j1 <= 3 * num2 / 4)
                    {
                        v[0, j1] = (yj - 2) * (yj - 3);
                        if (j1 >= num2 / 4)
                        {
                            v[num1 / 2, j1] = (yj - 2) * (yj - 3);
                            v[3 * num1 / 4, j1] = yj * (yj - 2) * (yj - 3);
                        }
                    }
                    else
                        v[num1 / 4, j1] = (yj - 2) * (yj - 3);
                }
            }
        }

        void GetR2(int num1, int num2, double h2, double k2, double a2, double[,] v, double[,] r, double[,] f)
        {
            for (int j = 1; j < num2; j++)
            {
                for (int i = 1; i < num1; i++)
                {
                    //Обходим вырезанный угол
                    if (i < n / 4 && j > 3 * m / 4)
                        continue;
                    //Обходим вырезанную область 
                    if (i > n / 2 && i < 3 * n / 4 && j > m / 4 && j < 3 * m / 4)
                        continue;

                    r[i, j] = (h2 * (v[i + 1, j] + v[i - 1, j]) + k2 * (v[i, j + 1] + v[i, j - 1])) + v[i, j] * a2 - f[i, j];

                }
            }
        }

        void QwnArea()
        {
            Nmax = int.Parse(textBox4.Text);
            E = double.Parse(textBox3.Text);
            n = int.Parse(textBox1.Text);
            m = int.Parse(textBox2.Text);


            if (n < 4)
            {
                MessageBox.Show(" Число разбиений по x должно быть >= 4 ");
                return;
            }
            if (m < 4)
            {
                MessageBox.Show(" Число разбиений по y должно быть >= 4");
                return;
            }
            if (n % 4 != 0)
            {
                MessageBox.Show(" Число разбиений по x должно быть кратно 4 ");
                return;
            }
            if (m % 4 != 0)
            {
                MessageBox.Show(" Число разбиений по y должно быть кратно 4");
                return;
            }



            //Очистка строк и столбцов таблицы
            dataGridView1.Rows.Clear();

            dataGridView1.RowCount = m + 2;
            dataGridView1.ColumnCount = n + 2;



            int S = 0; // Число проведенных итераций 
            double Emax = 0; // Текущее значение прироста
            double Ecur = 0;  // Для подсчета значения текущего значения прироста

            double a2, k2, h2; // ненулевые элементы матрицы А кудрявая 


            double[,] v = new double[n + 1, m + 1];  // искомый вектор 
            double[,] f = new double[n + 1, m + 1];
            double Ts;  // параметр текущей невязки


            int i, j;
            double v_old;
            double v_new;


            double h = (b - a) / n;
            double k = (d - c) / m;
            h2 = (-1.0) * (n / (b - a)) * (n / (b - a));
            k2 = (-1.0) * (m / (d - c)) * (m / (d - c));
            a2 = (-2.0) * (h2 + k2);

            BoardAdRightForRectangle2(h, k, n, m, v, f);

            bool flag = true;
            double[,] r = new double[n + 1, m + 1]; //  Невязка 
            S = 0;
            while (S < Nmax && flag)
            {
                Emax = 0;

                GetR(n, m, h2, k2, a2, v, r, f);
                double temps = 0.0, Ars = 0.0, doub_Ars = 0.0;

                for (j = 1; j < m; j++)
                {
                    for (i = 1; i < n; i++)
                    {
                        //Обходим вырезанный угол
                        if (i < n / 4 && j > 3 * m / 4)
                            continue;
                        //Обходим вырезанную область 
                        if (i > n / 2 && i < 3 * n / 4 && j > m / 4 && j < 3 * m / 4)
                            continue;
                        temps = (r[i, j] * a2 + h2 * (r[i + 1, j] + r[i - 1, j]) + k2 * (r[i, j + 1] + r[i, j - 1]));
                        Ars += temps * r[i, j];
                        doub_Ars += temps * temps;
                    }
                }
                Ts = Ars / doub_Ars;
                for (j = 1; j < m; j++)
                {
                    for (i = 1; i < n; i++)
                    {
                        //Обходим вырезанный угол
                        if (i < n / 4 && j > 3 * m / 4)
                            continue;
                        //Обходим вырезанную область 
                        if (i > n / 2 && i < 3 * n / 4 && j > m / 4 && j < 3 * m / 4)
                            continue;

                        v_old = v[i, j];
                        v_new = v_old - Ts * r[i, j];
                        Ecur = Math.Abs(v_old - v_new); // модуль 
                        if (Ecur > Emax)
                        {
                            Emax = Ecur;
                        }
                        v[i, j] = v_new;
                    }
                }


                S++;
                if (Emax < E)
                    flag = false;

            }
            label11.Text = Convert.ToString(Emax);
            label8.Text = Convert.ToString(S);

            //получим вектор с половинным шагом 
            double[,] vh = new double[2 * n + 1, 2 * m + 1];

            if (radioButton2.Checked == true)
            {
                double[,] fh = new double[2 * n + 1, 2 * m + 1];

                double hh = (b - a) / (2 * n);
                double kh = (d - c) / (2 * m);
                double hh2 = (-1.0) * (2 * n / (b - a)) * (2 * n / (b - a));
                double kh2 = -(1 / kh) * (1 / kh);
                double ah2 = (-2.0) * (hh2 + kh2);

                BoardAdRightForRectangle(hh, kh, 2 * n, 2 * m, vh, fh);

                bool flag1 = true;
                double[,] rh = new double[2 * n + 1, 2 * m + 1];
                S = 0;
                while (S < Nmax && flag1)
                {
                    Emax = 0;

                    GetR(2 * n, 2 * m, hh2, kh2, ah2, vh, rh, fh);
                    double temps = 0.0, Ars = 0.0, doub_Ars = 0.0;

                    for (j = 1; j < 2 * m; j++)
                    {
                        for (i = 1; i < 2 * n; i++)
                        {
                            //Обходим вырезанный угол
                            if (i < n / 4 && j > 3 * m / 4)
                                continue;
                            //Обходим вырезанную область 
                            if (i > n / 2 && i < 3 * n / 4 && j > m / 4 && j < 3 * m / 4)
                                continue;
                            temps = (rh[i, j] * ah2 + hh2 * (rh[i + 1, j] + rh[i - 1, j]) + kh2 * (rh[i, j + 1] + rh[i, j - 1]));
                            Ars += temps * rh[i, j];
                            doub_Ars += temps * temps;
                        }
                    }
                    Ts = Ars / doub_Ars;
                    for (j = 1; j < 2 * m; j++)
                    {
                        for (i = 1; i < 2 * n; i++)
                        {
                            //Обходим вырезанный угол
                            if (i < n / 4 && j > 3 * m / 4)
                                continue;
                            //Обходим вырезанную область 
                            if (i > n / 2 && i < 3 * n / 4 && j > m / 4 && j < 3 * m / 4)
                                continue;

                            v_old = vh[i, j];
                            v_new = v_old - Ts * rh[i, j];
                            Ecur = Math.Abs(v_old - v_new); // модуль 
                            if (Ecur > Emax)
                            {
                                Emax = Ecur;
                            }
                            vh[i, j] = v_new;
                        }
                    }


                    S++;
                    if (Emax < E)
                        flag1 = false;

                }
                label17.Text = Convert.ToString(Emax);
                label15.Text = Convert.ToString(S);

            }



            //Невязка 
            double r_evk = 0;
            for (int jj = 1; jj < m; jj++)
            {
                for (int ii = 1; ii < n; ii++)
                {
                    //Обходим вырезанный угол
                    if (ii < n / 4 && jj > 3 * m / 4)
                        continue;
                    //Обходим вырезанную область 
                    if (ii > n / 2 && ii < 3 * n / 4 && jj > m / 4 && jj < 3 * m / 4)
                        continue;

                    r[ii, jj] = (h2 * (v[ii + 1, jj] + v[ii - 1, jj]) + k2 * (v[ii, jj + 1] + v[ii, jj - 1])) + v[ii, jj] * a2 - f[ii, jj];
                    r_evk += r[ii, jj] * r[ii, jj];
                }
            }
            r_evk = Math.Sqrt(r_evk);
            label19.Text = Convert.ToString(r_evk);


            // Точность 
            double xi1 = 0.0, yj1 = 0.0;
            double maxE = 0;
            double curE = 0;
            for (j = 0; j < m + 1; j++)
            {
                for (i = 0; i < n + 1; i++)
                {
                    //Обходим вырезанный угол
                    if (i < n / 4 && j > 3 * m / 4)
                        continue;
                    //Обходим вырезанную область 
                    if (i > n / 2 && i < 3 * n / 4 && j > m / 4 && j < 3 * m / 4)
                        continue;

                    if (radioButton1.Checked == true)
                    {
                        xi1 = (double)i * h + a;
                        yj1 = (double)j * k + c;
                        curE = Math.Abs(function(xi1, yj1) - v[i, j]);
                    }
                    else
                        curE = Math.Abs(v[i, j] - vh[2 * i, 2 * j]);
                    if (curE >= maxE)
                    {
                        maxE = curE;
                    }
                }
            }
            label13.Text = Convert.ToString(maxE);

            //X
            dataGridView1.Rows[0].Cells[0].Value = "X / Y ";

            for (int i1 = 1; i1 < n + 2; i1++)
            {
                xi1 = (double)(i1 - 1.0) * h + a;

                dataGridView1.Rows[0].Cells[i1].Style.BackColor = System.Drawing.Color.OrangeRed;
                dataGridView1.Rows[0].Cells[i1].Value = xi1;
            }

            //Y

            int rr = 1;
            for (int j1 = m + 1; j1 > 0; j1--)
            {
                yj1 = (double)(rr - 1) * k + c;

                dataGridView1.Rows[j1].Cells[0].Style.BackColor = System.Drawing.Color.OrangeRed;
                dataGridView1.Rows[j1].Cells[0].Value = yj1;

                rr++;
            }


            //table
            for (int j1 = 1; j1 < m + 2; j1++)
            {
                for (int i1 = 1; i1 < n + 2; i1++)
                {
                    //dataGridView1.Rows[j1].Cells[i1].Style.BackColor = System.Drawing.Color.SlateGray;
                    dataGridView1.Rows[j1].Cells[i1].Value = v[i1 - 1, m + 1 - j1];
                }

            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                Rectangular();
                return;
            }
            if (comboBox1.SelectedIndex == 2)
            {
                QwnArea();
                return;
            }
            
        }

        
        public Form1()
        {
            InitializeComponent();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

    }
}
