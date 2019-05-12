using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MathNet;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra.Double;
using Microsoft.Win32;

namespace CourseWork2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        double[][] columArray;
        double[][] columArraySource;
        DenseMatrix koeffPair;
        DenseMatrix koeffPrivate;
        int colum = 10;
        string s = "";
        double[] sampleSource;
        double[] sampleRationing;
        public MainWindow()
        {
            InitializeComponent();
        }
        #region Нормирование
        //private void BtnOpenFile_Click(object sender, RoutedEventArgs e)
        //{
        //    OpenFileDialog FileOT = new OpenFileDialog();
        //    FileOT.Filter = "All files (*.*)|*.*|TXT text (*.txt)|*.txt";
        //    if (FileOT.ShowDialog() == true)
        //    {
        //        Stream ms = new FileStream(FileOT.FileName, FileMode.Open);
        //        lblNameFile.Content = FileOT.FileName;
        //        byte[] array = new byte[ms.Length];
        //        ms.Read(array, 0, array.Length);
        //        string buf = Encoding.Default.GetString(array);
        //        s = buf.ToLower();
        //    }
        //    sampleSource = DiscriptiveStatistics.GetSample(s);
        //    tbDiscrStat_source.Text = DiscriptiveStatistics.Output_descriptive_statistics(sampleSource);
        //    tblSourceSample.Text = DiscriptiveStatistics.Output(sampleSource);
        //}
        //private void BtnRationing_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        sampleRationing = DiscriptiveStatistics.Rationing_MaxMin(sampleSource);
        //        tblRationning.Text = DiscriptiveStatistics.Output(sampleRationing);
        //        tbDiscrStat_Ration.Text = DiscriptiveStatistics.Output_descriptive_statistics(sampleRationing);
        //    }
        //    catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка"); }
        //}
        //private void BtnSaveFile_Click(object sender, RoutedEventArgs e)
        //{
        //    SaveFileDialog sd = new SaveFileDialog();
        //    sd.Filter = "All files (*.*)|*.*|TXT text (*.txt)|*.txt";
        //    if (sd.ShowDialog() == true)
        //    {
        //        File.WriteAllText(sd.FileName, DiscriptiveStatistics.Save(sampleRationing));
        //    }
        //}

        #endregion

        #region Распределение
        private double[] Get_theretical_frequency_Laplas(double[] arr, double[][] intervalArray)
        {
            double X_aver = DiscriptiveStatistics.Average(arr);
            double disp = Math.Sqrt(DiscriptiveStatistics.Dispersion(arr));
            double[] P = new double[intervalArray.Length];
            for (int i = 0; i < P.Length; i++)
            {
                double[] buf = intervalArray[i];
                P[i] = DiscriptiveStatistics.GetValueLaplasFunction(Math.Round(Math.Abs((buf[0] - X_aver) / disp), 2)) * DiscriptiveStatistics.GetValueLaplasFunction(Math.Round(Math.Abs((buf[buf.Length - 1] - X_aver) / disp), 2));
                P[i] *= buf.Length;
            }
            return P;
        }
        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double k = 43.2;
                int interval = 31;
                
                double[][] points = new double[colum][];
                double[][] xn = new double[colum][];
                double[][] m_e = new double[colum][];
                double[][] m_t = new double[colum][];
                double[] x2_arr = new double[colum];

                for (int i = 0; i < colum; i++)
                {
                    points[i] = point(columArray[i], interval);
                    double[] me_buf = m_e[i] = Get_m_e(columArray[i], points[i], interval);
                    xn[i] = Get_xn(columArray[i], points[i], interval);
                    double[] mt_buf = m_t[i] = theoretical_freq(columArray[i], m_e[i].Sum(), xn[i], interval);

                    for (int j = 0; j < mt_buf.Length; j++)
                    {
                        x2_arr[i] += Math.Pow(me_buf[j] - mt_buf[j], 2) / mt_buf[j];
                    }
                }
                string s = "\n", t = "\n", w = "\n";
                for (int i = 0; i < colum; i++)
                {
                    if (x2_arr[i] > k) { s += "Не нормальное\n\n"; t += string.Format("{0:F2}", x2_arr[i])+ "\n\n"; }
                    else { s += "Нормальное\n\n"; t += string.Format("{0:F2}", x2_arr[i]) + "\n\n"; }
                    w += Data.parametrs[i] + "\n\n";
                }
                
                stpDistr.Children.Add(new TextBlock { Text = s, HorizontalAlignment=HorizontalAlignment.Center});
                stpDistrX_2.Children.Add(new TextBlock { Text = t , HorizontalAlignment = HorizontalAlignment.Center });
                stpDistrParams.Children.Add(new TextBlock { Text = w });
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка"); }
        }
        private double[] theoretical_freq(double[] arr, double sum, double[] xn, int CountInterval)
        {
            double X_aver = DiscriptiveStatistics.Average(arr);
            double disp = Math.Sqrt(DiscriptiveStatistics.Dispersion(arr));
            double[] m_t = new double[CountInterval];
            double h = (DiscriptiveStatistics.Max(arr) - DiscriptiveStatistics.Min(arr)) / CountInterval;
            for (int i = 0; i < m_t.Length; i++)
            {
                double u = (xn[i] - X_aver) / disp;
                double f_u = 1 / (Math.Sqrt(2 * Math.PI) * Math.Pow(Math.E, (u * u) / 2));
                m_t[i] = sum * h / disp * f_u;
            }
            return m_t;
        }

        private double[] point(double[] arr, int CountInterval)
        {
            double max = DiscriptiveStatistics.Max(arr);
            double min = DiscriptiveStatistics.Min(arr);
            double[] points = new double[CountInterval + 1];
            for (int i = 0; i < points.Length; i++)
            {
                points[i] = max - i * (max - min) / CountInterval;
            }

            return points.OrderBy(x => x).ToArray();
        }
        private double[] Get_m_e(double[] arr, double[] points, int CountInterval)
        {
            double[] m_e = new double[CountInterval];
            for (int i = 0; i < points.Length; i++)
            {
                foreach (double x in arr)
                {
                    if (points[i] < x && x < points[i + 1]) m_e[i]++;
                }
            }
            return m_e;

        }
        private double[] Get_xn(double[] arr, double[] points, int CountInterval)
        {
            double[] xn = new double[CountInterval];
            for (int i = 0; i < points.Length - 1; i++)
            {
                xn[i] = points[i + 1] - (points[i + 1] - points[i]) / 2;
            }
            return xn;
        }
        #endregion

        #region Корреляционный анализ
        private void Btn_Pair_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                columArray = new double[colum][];
                koeffPair = new DenseMatrix(colum, colum);
                for (int i = 0; i < colum; i++)
                    columArray[i] = DiscriptiveStatistics.GetSample(File.ReadAllText(@"C:\Users\Данагуль\source\repos\CourseWork2\Нормированные значения\" + (i + 2).ToString() + ".txt"));

                for (int i = 0; i < colum; i++)
                    for (int j = i; j < colum; j++)
                        koeffPair[i, j] = koeffPair[j, i] = DiscriptiveStatistics.PairKoeff(columArray[i], columArray[j]);

                DenseMatrix t_Matrix = T_Matrix_Koeff(koeffPair);
                // DenseMatrix D_Matr = D_Matrix(koeffPair);

                tbMatrix.Text = Output_R(koeffPair);
                tbMatrix.Text += "Коэффициент значим при t > 1.96\n";
                tbMatrix.Text += Output_R(t_Matrix);
                //tbMatrix.Text += "Коэффициенты детерминации\n";
                //tbMatrix.Text += Output_R(D_Matr);


                var X_Y = new List<KeyValuePair<double, double>>()
                {
                new KeyValuePair<double,double>( 0,150),
                new KeyValuePair<double,double>( 17,255),

                new KeyValuePair<double,double>( 80,281 ),
                new KeyValuePair<double,double>( 180,281 ),

                new KeyValuePair<double, double>( 250,255 ),
                new KeyValuePair<double, double>( 300-2,150 ),

               new KeyValuePair<double, double>( 250,43 ),
               new KeyValuePair<double, double>( 180,17 ),

               new KeyValuePair<double, double>( 80,17 ),
               new KeyValuePair<double, double>( 40,34 )
                };
                foreach (KeyValuePair<double, double> x in X_Y)
                {
                    Ellipse l = new Ellipse();
                    l.Width = l.Height = 5;
                    l.Fill = Brushes.Red;
                    Canvas.SetTop(l, x.Key);
                    Canvas.SetLeft(l, x.Value);
                    cnvMain.Children.Add(l);
                }

                //for (int i = 0; i < colum; i++)
                //{
                //    for (int j = i + 1; j < colum; j++)
                //    {
                //        if (i != j && koeffPair[i, j] >= 0.3 && koeffPair[i, j] < 0.5)
                //        {
                //            Line l = new Line();
                //            l.X1 = X_Y[i].Key;
                //            l.X2 = X_Y[j].Key;
                //            l.Y1 = X_Y[i].Value;
                //            l.Y2 = X_Y[j].Value;
                //            l.Stroke = Brushes.Red;
                //            l.StrokeThickness = 1;
                //            cnvMain.Children.Add(l);
                //        }
                //        if (i != j && koeffPair[i, j] >= 0.5 && koeffPair[i, j] < 0.7)
                //        {
                //            Line l = new Line();
                //            l.X1 = X_Y[i].Key;
                //            l.X2 = X_Y[j].Key;
                //            l.Y1 = X_Y[i].Value;
                //            l.Y2 = X_Y[j].Value;
                //            l.Stroke = Brushes.Yellow;
                //            l.StrokeThickness = 1;
                //            cnvMain.Children.Add(l);
                //        }
                //        if (i != j && koeffPair[i, j] >= 0.7 && koeffPair[i, j] < 1)
                //        {
                //            Line l = new Line();
                //            l.X1 = X_Y[i].Key;
                //            l.X2 = X_Y[j].Key;
                //            l.Y1 = X_Y[i].Value;
                //            l.Y2 = X_Y[j].Value;
                //            l.Stroke = Brushes.Green;
                //            l.StrokeThickness = 1;
                //            cnvMain.Children.Add(l);
                //        }
                //    }
                //}
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка"); }
        }
        private void Btn2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                tbMatrix2.Text = "";
                koeffPrivate = new DenseMatrix(colum, colum);
                if (koeffPair == null) throw new Exception("Вычислите парные коэффициенты!");
                for (int i = 0; i < colum; i++)
                {
                    for (int j = i + 1; j < colum; j++)
                        koeffPrivate[i, j] = koeffPrivate[j, i] = Get_AlgebralAddition(koeffPair, i, j) / Math.Sqrt(Get_AlgebralAddition(koeffPair, i, i) * Get_AlgebralAddition(koeffPair, j, j));
                    koeffPrivate[i, i] = koeffPair[i, i];
                }
                #region Окружность
                //    var X_Y = new List<KeyValuePair<double, double>>()
                //{
                //    new KeyValuePair<double,double>( 150-1, 0 ),
                //    new KeyValuePair<double,double>( 225-1, 150*(2-Math.Sqrt(3))/2-1 ),
                //    new KeyValuePair<double, double>(300-150*(2-Math.Sqrt(3))/2-1, 75-1 ),
                //   new KeyValuePair<double, double>( 300-1, 150-1 ),
                //   new KeyValuePair<double, double>(300-150*(2-Math.Sqrt(3))/2-1, 225-1 ),
                //    new KeyValuePair<double, double>(225-1, 300-150*(2-Math.Sqrt(3))/2-1 ),
                //    new KeyValuePair<double, double>(150-1, 300-1 ),
                //   new KeyValuePair<double, double>( 75-1, 300-150*(2-Math.Sqrt(3))/2 -1),
                //    new KeyValuePair<double, double>( 150*(2-Math.Sqrt(3))/2-1, 225-1 ),
                //    new KeyValuePair<double, double>(0, 150-1 ),
                //    new KeyValuePair<double, double>( 150*(2-Math.Sqrt(3))/2-1, 75 -1),
                //   new KeyValuePair<double, double>( 75-1, 150*(2-Math.Sqrt(3))/2-1 ) };
                //    //foreach (KeyValuePair<double, double> x in X_Y)
                //    //{
                //    //    Ellipse l = new Ellipse();
                //    //    l.Width = l.Height = 5;
                //    //    l.Fill = Brushes.Red;
                //    //    Canvas.SetTop(l, x.Key);
                //    //    Canvas.SetLeft(l, x.Value);
                //    //    cnvMainPrivate.Children.Add(l);
                //    //}
                //    for (int i = 0; i < colum; i++)
                //    {
                //        for (int j = i + 1; j < colum; j++)
                //        {
                //            if (i != j && koeffPair[i, j] >= 0.3 && koeffPair[i, j] < 0.5)
                //            {
                //                Line l = new Line();
                //                l.X1 = X_Y[i].Key;
                //                l.X2 = X_Y[j].Key;
                //                l.Y1 = X_Y[i].Value;
                //                l.Y2 = X_Y[j].Value;
                //                l.Stroke = Brushes.Yellow;
                //                l.StrokeThickness = 1;
                //                cnvMainPrivate.Children.Add(l);
                //            }
                //            if (i != j && koeffPair[i, j] >= 0.5 && koeffPair[i, j] < 0.7)
                //            {
                //                Line l = new Line();
                //                l.X1 = X_Y[i].Key;
                //                l.X2 = X_Y[j].Key;
                //                l.Y1 = X_Y[i].Value;
                //                l.Y2 = X_Y[j].Value;
                //                l.Stroke = Brushes.Green;
                //                l.StrokeThickness = 1;
                //                cnvMainPrivate.Children.Add(l);
                //            }
                //            if (i != j && koeffPair[i, j] >= 0.7 && koeffPair[i, j] < 1)
                //            {
                //                Line l = new Line();
                //                l.X1 = X_Y[i].Key;
                //                l.X2 = X_Y[j].Key;
                //                l.Y1 = X_Y[i].Value;
                //                l.Y2 = X_Y[j].Value;
                //                l.Stroke = Brushes.Red;
                //                l.StrokeThickness = 1;
                //                cnvMainPrivate.Children.Add(l);
                //            }
                //        }
                //    }
                #endregion

                #region Палки

                for(int i = 0; i < colum; i++)
                {
                    Canvas cnv = new Canvas();
                    cnv.Height = colum * 30;
                    cnv.Width = 160;

                    for (int j = 0; j < colum; j++)
                    {
                        if (j != i)
                        {
                           
                        }
                    }
                    lvCNV.Items.Add(cnv);
                }

                #endregion

                tbMatrix2.Text = Output_R(koeffPrivate);
                for (int i = 0; i < colum; i++)
                {
                    for (int j = i + 1; j < colum; j++)
                    {
                        if (Math.Abs(koeffPair[i, j]) < Math.Abs(koeffPrivate[i, j])) tbMatrix2.Text += "\nКоэффициент r" + (i + 1).ToString() + "," + (j + 1).ToString() + " увеличился";
                        else tbMatrix2.Text += "\nКоэффициент r" + (i + 1).ToString() + "," + (j + 1).ToString() + " уменьшился";

                        if (koeffPrivate[i, j] * koeffPair[i, j] > 0) tbMatrix2.Text += " и изменил знак";
                        else tbMatrix2.Text += " и не изменил знак";
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка"); }
        }
        private void Btn3_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (koeffPair == null) throw new Exception("Вычислите парные коэффициенты!");
                double det_R = koeffPair.Determinant();
                DenseMatrix R = new DenseMatrix(colum);
                double[] r_m = new double[colum];
                tbMatrix3.Text = "Индексы детерминации для множественных коэффициентов:\n";
                for (int i = 0; i < colum; i++) tbMatrix3.Text += (i + 1).ToString() + "\t";
                tbMatrix3.Text += "\n";

                for (int i = 0; i < colum; i++)
                {
                    r_m[i] = Math.Sqrt(1 - (det_R / Get_AlgebralAddition(koeffPair, i, i)));
                    tbMatrix3.Text += string.Format("{0:F2}\t", r_m[i] * r_m[i]);
                }
                tbMatrix3.Text += "\nЗначимость коэффициентов:\n";
                double[] Fr_matrix = new double[colum];
                for (int i = 0; i < colum; i++)
                {
                    Fr_matrix[i] = r_m[i] * r_m[i] * 8 / (1 - r_m[i] * r_m[i]);
                    tbMatrix3.Text += string.Format("{0:F2}\n", Fr_matrix[i]);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка"); }
        }
        #region Методы
        public DenseMatrix D_Matrix(DenseMatrix Matrix)
        {
            DenseMatrix M = new DenseMatrix(colum);
            for (int i = 0; i < colum; i++)
                for (int j = 0; j < colum; j++)
                    M[i, j] = Matrix[i, j] * Matrix[i, j];
            return M;
        }
        public string Output_R(DenseMatrix Matrix)
        {
            string s = "\t";
            for (int i = 0; i < colum; i++) s += (i + 1).ToString() + "\t";
            s += "\n\n";
            for (int i = 0; i < colum; i++)
            {
                s += (i + 1).ToString() + "\t";
                for (int j = 0; j < colum; j++)
                    s += string.Format("{0:F2}\t", Matrix[i, j]);
                s += "\n\n";
            }
            s += "\n\n";
            return s;
        }
        public DenseMatrix T_Matrix_Koeff(DenseMatrix Matrix_Koeff)
        {
            DenseMatrix t_Matrix = new DenseMatrix(colum);
            for (int i = 0; i < colum; i++)
            {
                for (int j = 0; j < colum; j++)
                {
                    t_Matrix[i, j] = Math.Abs(Matrix_Koeff[i, j]) * Math.Sqrt((colum - 2) / (1 - Matrix_Koeff[i, j]));
                }
            }
            return t_Matrix;
        }
        public static DenseMatrix GetMinor(DenseMatrix A, int n, int i, int j)
        {
            DenseMatrix M = new DenseMatrix(n - 1, n - 1);

            for (int k = 0; k < n; k++)
            {
                for (int m = 0; m < n; m++)
                {
                    if (k < i && m < j) M[k, m] = A[k, m];
                    if (k > i && m > j) M[k - 1, m - 1] = A[k, m];
                    if (k > i && m < j) M[k - 1, m] = A[k, m];
                    if (k < i && m > j) M[k, m - 1] = A[k, m];
                }
            }

            return M;
        }
        private double Get_AlgebralAddition(DenseMatrix A, int i, int j)
        {
            return Math.Pow(-1, i + j) * GetMinor(A, colum, i, j).Determinant();
        }
        public DenseMatrix GetDense(double[][] A)
        {
            double[] buf = A[0];
            DenseMatrix res = new DenseMatrix(colum, buf.Length);
            for (int i = 0; i < colum; i++)
            {
                buf = A[i];
                for (int j = 0; j < buf.Length; j++) res[i, j] = buf[j];
            }
            return res;
        }
        #endregion

        #endregion

        #region Регрессионный анализ

        private void BtnRegress_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (columArray == null) throw new Exception("Проведите корреляционный анализ!");
                DenseMatrix X_1 = new DenseMatrix(colum, columArray[0].Length);
                DenseMatrix Y = new DenseMatrix(columArray[0].Length, 1);
                Y[0, 0] = 1;

                for (int i = 1; i < colum; i++)
                {
                    double[] buf = columArray[i];
                    for (int j = 0; j < columArray[0].Length; j++)
                    {
                        X_1[0,j] = 1;
                        Y[j, 0] = columArray[0][j];
                        X_1[i, j] = buf[j];
                    }
                }

                DenseMatrix X_T = (DenseMatrix)X_1.Transpose();
                DenseMatrix MulMatr = (DenseMatrix)X_1.Multiply(X_T);
                DenseMatrix inverseM = (DenseMatrix)MulMatr.Inverse();
                DenseMatrix mul = (DenseMatrix)inverseM.Multiply(X_1);
                DenseMatrix A = (DenseMatrix)mul.Multiply(Y);
                tbRegress.Text = "";
                for (int i = 0; i < A.RowCount; i++)
                {
                    tbRegress.Text += string.Format("{0}\t{1}\n", (i ).ToString(),A[i, 0] );
                }

                #region Значимость
                DenseMatrix NewY = (DenseMatrix)X_T.Multiply(A);
                DenseMatrix Qr = (DenseMatrix)NewY.TransposeThisAndMultiply(NewY);
                double Qos = 0;
                for(int i = 0; i < columArray[0].Length; i++)
                {
                    Qos += Math.Pow(Y[i, 0] - NewY[i, 0], 2);
                }
                double t_tabl = 1.96;//2.306;
                double S_2 = Qr[0, 0] / (columArray[0].Length - colum - 1);
                DenseMatrix S_b = inverseM;

                for (int i = 0; i < S_b.RowCount; i++)
                {
                    S_b[i, i] *= S_2;
                    if (S_b[i, i] > t_tabl) tbRegress.Text += "a" + i.ToString() + " значим\n";
                    else tbRegress.Text += "a" + i.ToString() + " не значим\n";
                }
                double F_tabl = 4.459;
                //double F = (Qr[0, 0] * (569-10 - 1)) / ((10+1) * Qos);
                double F = (Qr[0, 0] * (10- 1 - 1)) / ((1+1) * Qos);
                tbRegress.Text += "\n Значимость равна " + F.ToString();
                if(F>F_tabl) tbRegress.Text += "\n Уравнение регрессии значимо "  + "\nY\tY^\n ";
                else tbRegress.Text += "\n Уравнение регрессии не значимо " + "\nY\tY^\n ";
                for (int i = 0; i < columArray.Length; i++)
                {
                    tbRegress.Text += string.Format("{0}\t{1}\n", Y[i, 0], NewY[i, 0]);
                }

                double Prognoz = 0;
                double S_ = Math.Sqrt(S_2);
                DenseMatrix X0 = new DenseMatrix(1, 10);
                for(int i = 0; i < colum; i++)
                {
                    Prognoz += columArray[i][0] * A[i, 0];
                }

                for(int i = 0; i < colum; i++)
                {
                    X0[0, i] = columArraySource[i][0];
                }
                DenseMatrix delta = (DenseMatrix)X0.Multiply(inverseM);
                X0 = (DenseMatrix)X0.Transpose();
              DenseMatrix m = (DenseMatrix) delta.Multiply(X0);
                //tbRegress.Text += "\n" + Prognoz + "+-" + t_tabl * S_2 * Math.Sqrt(Math.Abs(m[0, 0]) + 1);
                #endregion

                #region Прогноз
                for (int j = 1; j < A.RowCount; j++)
                    A[0,0] += X0[j - 1,0] * A[j, 0];
                for (int j = 1; j < delta.ColumnCount; j++)
                    delta[0,0] += X0[ j - 1,0] * delta[0, j];
                double del = t_tabl * S_ * Math.Sqrt(Math.Abs(delta[0,0]) - 1);

                tbRegress.Text += "Интервал предсказания: " + Math.Round(A[0,0] - del, 3) + " <= y~ <= " +
                Math.Round(A[0,0] + del, 3); 
                #endregion
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка"); }
        }

        #endregion

        #region Данные
        private void BtnDATA_Click(object sender, RoutedEventArgs e)
        {
            Data.GetCsv();
            columArraySource = Data.Array;
            columArray = new double[Data.parametrs.Count][];
            colum = Data.parametrs.Count;
            for (int i = 0; i < Data.parametrs.Count; i++)
            {
                columArray[i] = DiscriptiveStatistics.Rationing_MaxMin(columArraySource[i]);
            }
            StackPanel stp = new StackPanel();
            stp.Orientation = Orientation.Horizontal;
            for (int i = 0; i < Data.Array.Length; i++)
            {
                double[] buf = Data.Array[i];
                TextBlock lv = new TextBlock();
                lv.Width = 100;
                lv.Height = Data.Array[0].Length * 18;
                lv.Text += Data.parametrs[i] + "\n";
                for (int j = 0; j < buf.Length; j++)
                    lv.Text += buf[j] + "\n";
                stp.Children.Add(lv);
            }
            scv.Content = stp;
        }
        #endregion

        #region Описательная статистика
        private void BtnDiscrStat_Click(object sender, RoutedEventArgs e)
        {
            stpDiscrStat.Children.Clear();
            try
            {
                if (rbNorm.IsChecked == true)
                {
                    output(columArray);
                    tblDiscrStat.Text = "\n\nСреднее\n\nСумма\n\nСтандартная ошибка\n\nМедиана\n\nМода\n\nСтандартное отклонение\n\nДисперсия\n\nЭксцесс\n\nАсимметричность\n\nИнтервал\n\nМинимум\n\nМаксимум\n\nСчет";
                }
                else if (rbNotNorm.IsChecked == true)
                {
                    output(columArraySource);
                    tblDiscrStat.Text = "\n\nСреднее\n\nСумма\n\nСтандартная ошибка\n\nМедиана\n\nМода\n\nСтандартное отклонение\n\nДисперсия\n\nЭксцесс\n\nАсимметричность\n\nИнтервал\n\nМинимум\n\nМаксимум\n\nСчет";
                }
                else throw new Exception("Выберите вид данных!");
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка"); }
        }

        private void output(double[][] arr)
        {
            for (int i = 0; i < colum; i++)
            {
                string s = "";
                string t = "";
                s += (i + 1).ToString() + "\n\n";
                t += "\t\n\n\n";
                s += string.Format("{0:F2}", DiscriptiveStatistics.Average(arr[i])) + "\n\n";
                t += "\t\n\n\n";
                s += string.Format("{0:F2}", DiscriptiveStatistics.Amount(arr[i])) + "\n\n";
                t += "\t\n\n\n";
                s += string.Format("{0:F2}", DiscriptiveStatistics.StandartError(arr[i])) + "\n\n";
                t += "\t\n\n\n";
                s += string.Format("{0:F2}", DiscriptiveStatistics.Median(arr[i])) + "\n\n";
                t += "\t\n\n\n";
                s += string.Format("{0:F2}", DiscriptiveStatistics.Fashion(arr[i])) + "\n\n";
                t += "\t\n\n\n";
                s += string.Format("{0:F2}", DiscriptiveStatistics.StandartDeviation(arr[i])) + "\n\n";
                t += "\t\n\n\n";
                s += string.Format("{0:F2}", DiscriptiveStatistics.Dispersion(arr[i])) + "\n\n";
                t += "\t\n\n\n";
                s += string.Format("{0:F2}", DiscriptiveStatistics.Excess(arr[i])) + "\n\n";
                t += "\t\n\n\n";
                s += string.Format("{0:F2}", DiscriptiveStatistics.Asymmetry(arr[i])) + "\n\n";
                t += "\t\n\n\n";
                s += string.Format("{0:F2}", DiscriptiveStatistics.Interval(arr[i])) + "\n\n";
                t += "\t\n\n\n";
                s += string.Format("{0:F2}", DiscriptiveStatistics.Min(arr[i])) + "\n\n";
                t += "\t\n\n\n";
                s += string.Format("{0:F2}", DiscriptiveStatistics.Max(arr[i])) + "\n\n";
                t += "\t\n\n\n";
                s += string.Format("{0:F2}", arr[i].Length) + "\n\n";
                t += "\t\n\n\n";
                stpDiscrStat.Children.Add(new TextBlock { Text = s });
                stpDiscrStat.Children.Add(new TextBlock { Text = t });
            }
        }
        #endregion

       

    }
}
