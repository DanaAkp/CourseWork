using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CourseWork2
{
    class Matrix
    {
        public static double determinant;
        public static int[] countX;
        public static int CountTransposition = 0;
        #region Методы
        /// <summary>
        /// Инициализация вектора индексов неизвестного Х
        /// </summary>
        /// <param name="cX"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static int[] InitialX(int[] cX, int n)
        {
            int[] countX = new int[n];
            for (int i = 0; i < n; i++)
                countX[i] = i;
            return countX;
        }
        /// <summary>
        /// умножение матриц
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="n"></param>
        /// <param name="m"></param>
        /// <param name="k"></param>
        public static double[,] MulMatrix(double[,] A, double[,] B, int n, int m, int k)
        {
            double[,] C = new double[n, k];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < k; j++)
                {
                    C[i, j] = 0;
                    for (int c = 0; c < m; c++)
                    {
                        C[i, j] += A[i, c] * B[c, j];
                    }
                }
            }
            Output(C, n, k);
            return C;
        }
        /// <summary>
        /// Умножение матрицы на вектор
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="n"></param>
        /// <param name="m"></param>
        /// <param name="k"></param>
        public static double[] MulMatrix(double[,] A, double[] B, int n, int m)
        {
            double[] C = new double[n];
            for (int i = 0; i < n; i++)
            {
                for (int c = 0; c < m; c++)
                {
                    C[i] += A[i, c] * B[c];
                }
            }
            Output(C, n);
            return C;
        }
        /// <summary>
        /// Вывод матрицы
        /// </summary>
        /// <param name="A"></param>
        /// <param name="n"></param>
        /// <param name="m"></param>
        public static void Output(double[,] A, int n, int m)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                    Console.Write(string.Format("{0:F2} ", A[i, j]));
                Console.WriteLine();
            }
            Console.WriteLine("------------------------------------------------------------------");
        }
        /// <summary>
        /// Вывод вектора
        /// </summary>
        /// <param name="A"></param>
        /// <param name="n"></param>
        /// <param name="m"></param>
        public static void Output(double[] A, int n)
        {
            for (int i = 0; i < n; i++)
            {
                Console.WriteLine(string.Format("{0:F2} ", A[i]));
            }
            Console.WriteLine("------------------------------------------------------------------");
        }
        /// <summary>
        /// Ошибка матриц
        /// </summary>
        /// <param name="A"></param>
        /// <param name="n"></param>
        /// <param name="b"></param>
        public static void Error(double[,] A, double[,] A_1, int n)
        {
            double[,] sub = new double[n, n];
            for (int i = 0; i < n; i++) for (int j = 0; j < n; j++) sub[i, j] = Math.Abs(A[i, j] - A_1[i, j]);

            double myB = average(sub, n);
            double b = average(A, n);
            Console.WriteLine("inccuracy = " + myB / b);
            Console.WriteLine("------------------------------------------------------------------");
        }
        /// <summary>
        /// Ошибка вектора
        /// </summary>
        /// <param name="B"></param>
        /// <param name="B_1"></param>
        /// <param name="n"></param>
        public static void Error(double[] B, double[] B_1, int n)
        {
            double[] sub = new double[B.Length];
            for (int i = 0; i < B.Length; i++)
                sub[i] = Math.Abs(B[i] - B_1[i]);

            double b = average(sub, n);
            double new_b = average(B, n);
            double incur = b / new_b;
            Console.WriteLine("inccuracy = " + incur);
        }
        /// <summary>
        /// Среднее квадратичное вектора
        /// </summary>
        /// <param name="B"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static double average(double[] B, int n)
        {
            double delta = 0;
            for (int i = 0; i < n; i++)
                delta += B[i] * B[i];
            return delta = Math.Sqrt(delta);
        }
        /// <summary>
        /// Среднее квадратичное матрицы
        /// </summary>
        /// <param name="A"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static double average(double[,] A, int n)
        {
            double delta = 0;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                    delta += A[i, j] * A[i, j];
            }

            delta = Math.Sqrt(delta);
            return delta;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="A"></param>
        /// <param name="n"></param>
        /// <param name="a">Индекс строки, которую нужно заменить на строку с индекс К</param>
        /// <param name="k">Индекс строки, которую нужно заменить на строку индекс А</param>
        /// <returns></returns>
        public static double[,] Colum(double[,] A, int n, int a, int k)
        {
            for (int i = 0; i < n + 1; i++)
            {
                double buf = A[a, i];
                A[a, i] = A[k, i];
                A[k, i] = buf;
            }
            return A;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="A"></param>
        /// <param name="n"></param>
        /// <param name="a">Индекс столбца</param>
        /// <param name="k">Индекс столбца</param>
        /// <returns></returns>
        public static double[,] Row(double[,] A, int n, int a, int k)
        {
            for (int i = 0; i < n; i++)
            {
                double buf = A[i, a];
                A[i, a] = A[i, k];
                A[i, k] = buf;
            }
            int c = countX[a];
            countX[a] = countX[k];
            countX[k] = c;
            return A;
        }
        /// <summary>
        /// Из квадратной матрицы А получаем прямоугольную путем добавления столбца В
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static double[,] ExtendedMatrix(double[,] A, double[] B, int n)
        {
            double[,] newA = new double[n, n + 1];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n + 1; j++)
                {
                    if (j == n) newA[i, j] = B[i];
                    else newA[i, j] = A[i, j];
                }
            }
            return newA;
        }
        /// <summary>
        /// Получение столбца В из расширенной матрицы
        /// </summary>
        /// <param name="A"></param>
        /// <param name="n"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        public static double[] GetBFromExtendedMatrix(double[,] A, int n, int m)
        {
            double[] B = new double[n];
            for (int i = 0; i < n; i++)
                B[i] = A[i, m - 1];
            return B;
        }
        #endregion

        #region Гаусс
        public static double[,] gauss(double[,] A, int n)
        {
            determinant = 1;
            for (int i = 0; i < n; i++)
            {
                double buf = A[i, i];
                determinant *= buf;
                for (int j = i; j < n + 1; j++) A[i, j] /= buf;

                for (int k = i + 1; k < n; k++)
                {
                    buf = A[k, i];
                    for (int j = i; j < n + 1; j++)
                        A[k, j] = buf * A[i, j] - A[k, j];
                }
            }
            Output(A, n, n + 1);
            Console.WriteLine("Determinant = " + determinant);
            return A;
        }
        public static double[] obr(double[,] A, int n)
        {
            InitialX(countX, n);
            double[] x = new double[n + 1];
            // int c = n-1;
            for (int i = n - 1; i >= 0; i--)
            {
                double buf = 0;
                for (int k = i; k < n; k++)
                {
                    //if(i!=0)
                    buf += A[i, k] * x[k];
                }
                x[i] = A[i, n] - buf;
                Console.Write(string.Format("X{0} = {1:F2}\n", i, x[i]));
                //c--;
            }
            return x;
        }
        #endregion

        #region 1 Модификация
        public static double[,] gaussModif1(double[,] A, int n)
        {
            determinant = 1;
            countX = InitialX(countX, n);
            for (int i = 0; i < n; i++)
            {
                double max = A[i, i];
                int index = i;
                for (int c = i; c < n; c++)
                    if (Math.Abs(max) < Math.Abs(A[i, c]))
                    { max = A[i, c]; index = c; }
                if (i != index)
                {
                    A = Row(A, n, i, index); CountTransposition += 1;
                }
                double buf = A[i, i]; determinant *= buf;
                for (int j = i; j < n + 1; j++) A[i, j] /= buf;

                for (int k = i + 1; k < n; k++)
                {
                    buf = A[k, i];
                    for (int j = i; j < n + 1; j++)
                        A[k, j] = buf * A[i, j] - A[k, j];
                }
            }
            Console.WriteLine("Determinant = " + determinant * Math.Pow(-1, CountTransposition));
            Output(A, n, n + 1);
            return A;
        }
        public static double[] obrModif1(double[,] A, int n)
        {
            double[] x = new double[n];
            double[] newX = new double[n];

            for (int i = n - 1; i >= 0; i--)
            {
                double buf = 0;
                for (int k = i; k < n; k++)
                {
                    buf += A[i, k] * x[k];
                }
                x[i] = A[i, n] - buf;
            }
            for (int i = 0; i < n; i++)
            {
                newX[i] = x[countX[i]];
                Console.Write(string.Format("X{0} = {1:F2}\n", i, newX[i]));
            }
            return newX;
        }
        #endregion

        #region 2 Модификация
        public static double[,] gaussModif2(double[,] A, int n)
        {
            determinant = 1;
            countX = InitialX(countX, n);
            for (int i = 0; i < n; i++)
            {
                double max = A[i, i];
                int index = i;
                for (int c = i; c < n; c++)
                    if (Math.Abs(max) < Math.Abs(A[c, i]))
                    { max = A[c, i]; index = c; }
                if (i != index)
                {
                    CountTransposition += 1;
                    A = Colum(A, n, i, index);
                }
                double buf = A[i, i];
                determinant *= buf;
                for (int j = i; j < n + 1; j++) A[i, j] /= buf;

                for (int k = i + 1; k < n; k++)
                {
                    buf = A[k, i];
                    for (int j = i; j < n + 1; j++)
                        A[k, j] = buf * A[i, j] - A[k, j];
                }
            }
            Output(A, n, n + 1);
            Console.WriteLine("Determinant = " + determinant * Math.Pow(-1, CountTransposition));
            return A;
        }
        public static double[] obrModif2(double[,] A, int n)
        {
            double[] x = new double[n];
            for (int i = n - 1; i >= 0; i--)
            {
                double buf = 0;
                for (int k = i; k < n; k++)
                {
                    buf += A[i, k] * x[k];
                }
                x[i] = A[i, n] - buf;
                Console.Write(string.Format("X{0} = {1:F2}\n", i, x[i]));
            }
            return x;
        }
        #endregion

        #region 3 Модификация
        public static double[,] gaussModif3(double[,] A, int n)
        {
            determinant = 1;
            countX = InitialX(countX, n);
            for (int i = 0; i < n; i++)
            {
                double max = A[i, i];
                int row = i;
                int col = i;
                for (int c = i; c < n; c++)
                {
                    for (int j = i; j < n; j++)
                        if (Math.Abs(max) < Math.Abs(A[c, j]))
                        {
                            max = A[c, j];
                            row = c;
                            col = j;
                        }
                }
                if (row != i) { A = Colum(A, n, row, i); CountTransposition += 1; }
                if (col != i) { A = Row(A, n, col, i); CountTransposition += 1; }
                double buf = A[i, i];
                determinant *= A[i, i];
                for (int j = i; j < n + 1; j++) A[i, j] /= buf;

                for (int k = i + 1; k < n; k++)
                {
                    buf = A[k, i];
                    for (int j = i; j < n + 1; j++)
                        A[k, j] = buf * A[i, j] - A[k, j];
                }
            }
            Output(A, n, n + 1);
            Console.WriteLine("Determinant = " + determinant * Math.Pow(-1, CountTransposition));
            return A;
        }
        public static double[] obrModif3(double[,] A, int n)
        {
            double[] x = new double[n];
            double[] newX = new double[n];

            for (int i = n - 1; i >= 0; i--)
            {
                double buf = 0;
                for (int k = i; k < n; k++)
                {
                    buf += A[i, k] * x[k];
                }
                x[i] = A[i, n] - buf;
            }
            for (int i = 0; i < n; i++)
            {
                newX[countX[i]] = x[i];
                Console.Write(string.Format("X{0} = {1:F2}\n", i, newX[countX[i]]));
            }
            return newX;
        }
        #endregion

        #region LU-методы
        public static double[] LU(double[,] A, double[] B, int n)
        {
            double[,] L = new double[n, n];
            double[,] U = new double[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    double buf = 0;
                    if (i <= j)
                    {
                        for (int k = 0; k <= i - 1; k++)
                        {
                            buf += L[i, k] * U[k, j];
                        }
                        U[i, j] = A[i, j] - buf;
                    }
                    buf = 0;
                    if (i > j)
                    {
                        for (int k = 0; k <= j - 1; k++)
                        {
                            buf += L[i, k] * U[k, j];
                        }
                        L[i, j] = (A[i, j] - buf) / U[j, j];
                    }
                    if (i == j) L[i, j] = 1;
                }
            }
            Console.WriteLine("--------------Матрица L");
            Output(L, n, n);
            Console.WriteLine("--------------Матрица U");
            Output(U, n, n);
            determinant = 1;
            for (int i = 0; i < n; i++)
            {
                determinant *= U[i, i];
            }
            Console.WriteLine("determ U = " + determinant);
            return GetX_LU(U, GetY_LU(L, B));
        }
        public static double[] GetY_LU(double[,] L, double[] B)
        {
            double[] Y = new double[B.Length];
            for (int i = 0; i < Y.Length; i++)
            {
                double sum = 0;
                for (int j = 0; j < i; j++) sum += L[i, j] * Y[j];
                Y[i] = B[i] - sum;
            }
            return Y;
        }
        public static double[] GetX_LU(double[,] U, double[] Y)
        {
            double[] X = new double[Y.Length];
            for (int i = X.Length - 1; i >= 0; i--)
            {
                double sum = 0;
                for (int j = i + 1; j < X.Length; j++) sum += U[i, j] * X[j];
                X[i] = (Y[i] - sum) / U[i, i];
            }
            return X;
        }
        #endregion

        #region Обратная матрица
        public static double[,] InverseMatrix(double[,] A, int n)
        {
            double[,] invrseMatrix = new double[n, n];
            for (int i = 0; i < n; i++)
            {
                double[,] A_buf = A;
                double[] x = obrModif1(gaussModif1(ExtendedMatrix(A_buf, GetB_E(n, i), n), n), n);
                for (int j = 0; j < n; j++)
                {
                    invrseMatrix[j, i] = x[j];
                }
            }
            Console.WriteLine("----------------Обратная матрица-------------------");
            Output(invrseMatrix, n, n);
            return invrseMatrix;
        }
        public static double[] GetB_E(int n, int i)
        {
            double[] E = new double[n];
            E[i] = 1;
            return E;
        }
        #endregion
    }
}