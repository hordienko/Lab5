using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;


namespace matrixgame
{
    class Program
    {
        static double[,] filedata;

        static void Main(string[] args)
        {
            Console.WriteLine("Теорiя iгор");
            GetData();
            int index_min;
            int index_max;

            double max = Line(out index_max);
            double min = Column(out index_min);
            if (max == min)
            {
                Console.WriteLine($"Ciдлова точка = {max} \nТип вирiшення: Чиста стратегiя!");
            }
            else
            {
                Console.WriteLine("Сiдлової точки немає - переходимо до пошуку рiшень в змiшаних стратегiях!");

                Console.WriteLine("Початкова матриця:");
                double[,] table = Strategy_zm();

                double[] result = new double[2];
                double[,] table_result;
                Simplex S = new Simplex(table);
                table_result = S.Calculate(result);

                Console.WriteLine("Симплекс-таблиця:");
                for (int i = 0; i < table_result.GetLength(0); i++)
                {
                    for (int j = 0; j < table_result.GetLength(1); j++)
                        Console.Write(table_result[i, j] + " ");
                    Console.WriteLine();
                }

                Console.WriteLine();
                Console.WriteLine("Оптимальний план :");
                Console.WriteLine(" у[1] = " + result[0]);
                Console.WriteLine(" у[2] = " + result[1]);
                Console.WriteLine(" F(Y) = " + 1 * (result[0] + result[1]));

                double[][] A = new double[][] { new double[] {14,9},
                                               new double[]{12,14}, };
                double[] vector = new double[2] { 1, 1 };


                double[][] inv = MatrixInverse(A);
               Console.WriteLine("\nОбернена матриця: ");
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        Console.Write(inv[i][j] + " ");
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();

                Console.WriteLine("Оптимальний план для двоїстої задачi:");

                double[] VectorRESULT = new double[2];
                for (int i = 0; i < inv.Length; i++)
                {

                    VectorRESULT[i] = Math.Abs(vector[0] * inv[0][i] + vector[1] * inv[1][i]);
                }
                double Fx = VectorRESULT[0] + VectorRESULT[1]; // F(х) = 0,0795454545454545
                Console.WriteLine($" x[1]={ VectorRESULT[0]} \n x[2]={ VectorRESULT[1]} \n F(x)={Fx}");

                double p1 = Math.Abs(Fx * vector[0]);
                double p2 = Math.Abs(Fx * vector[1]);
                Console.WriteLine("\nРiшення для першого гравця:");
                Console.WriteLine($" p1={p1} \n p2={p2} ");


                double q1 = Math.Abs(Fx * result[0]);
                double q2 = Math.Abs(Fx * result[1]);
                Console.WriteLine("\nРiшення для другого гравця:");
                Console.WriteLine($" q1={q1} \n q2={q2} ");
                Console.WriteLine($"\nЦiна гри= {1/Fx}");

                Console.ReadLine();
            }

        }
        public static double[,] GetData()
        {
            string filePath = Path.GetFullPath("1.txt");
            int y = 0;
            int i = 0;
            using var fileReader = new StreamReader(filePath);
            string line;

            filedata = new double[5, 5];
            while ((line = fileReader.ReadLine()) != null)
            {
                string[] lines = line.Split(' ');


                for (i = 0; i < lines.Length; i++)
                {
                    filedata[y, i] = Convert.ToDouble(lines[i]);
                    //Console.Write(filedata[y, i] + " ");
                }
                y++;

            }
            return filedata;
        }

        //---------------------------------------пошук сідлової точки--------------------------------------------------
        static double Line(out int index_max)
        {
            GetData();
            double x;
            double max;
            double[] mas;
            mas = new double[5];
            double[] mass;
            mass = new double[5];

            for (int y = 0; y < 5; y++)
            {
                for (int i = 0; i < 5; i++)
                {
                    mas[i] = filedata[y, i];
                }
                x = mas.Min();
                mass[y] = x;
                //  Console.WriteLine(x);
            }
            max = mass.Max();
            index_max = Array.IndexOf(mass, max);
            //Console.WriteLine(index + 1);
            //Console.WriteLine(max);
            return max;
        }

        //-------------------------------------------Пошук елемента по стовпцях------------------------------------------------------------------------
        static double Column(out int index_min)
        {
            GetData();
            double x;
            double min;
            double[] mas;
            mas = new double[5];
            double[] mass;
            mass = new double[5];

            for (int i = 0; i < 5; i++)
            {
                for (int y = 0; y < 5; y++)
                {
                    mas[y] = filedata[y, i];
                }
                x = mas.Max();
                mass[i] = x;
                // Console.WriteLine(x);
            }
            min = mass.Min();
            index_min = Array.IndexOf(mass, min);
            //Console.WriteLine(index+1);
            //Console.WriteLine(min);
            return min;
        }

        static double[,] Strategy_zm()
        {
            GetData();
            Kernel beforeChange, kernel = new Kernel(filedata);
            kernel.Print();
            do
            {
                beforeChange = kernel;
                for (int i = 0; i < filedata.GetLength(0); i++)
                {
                    for (int j = 0; j < filedata.GetLength(0); j++)
                    {
                        if (i == j)
                        {
                            continue;
                        }
                        if (kernel.StrategyMatrix[i] <= kernel.StrategyMatrix[j])
                        {
                            kernel.RemoveStrategy(i);
                        }
                    }
                }
                kernel = Kernel.Transpose(kernel);
                for (int i = 0; i < filedata.GetLength(1); i++)
                {
                    for (int j = 0; j < filedata.GetLength(1); j++)
                    {
                        if (i == j)
                        {
                            continue;
                        }

                        if (kernel.StrategyMatrix[i] >= kernel.StrategyMatrix[j])
                        {
                            kernel.RemoveStrategy(i);
                        }
                    }
                }
                kernel = Kernel.Transpose(kernel);
            } while (kernel != beforeChange);

            Console.WriteLine();

            Console.WriteLine();




            List<List<double>> numbers = new List<List<double>>() { };

            for (int i = 0; i < kernel.StrategyMatrix.Count; i++)
            {
                if (hasAppropriateNumber(kernel.StrategyMatrix[i].strategy))
                {
                    numbers.Add(new List<double>());
                }
                for (int j = 0; j < kernel.StrategyMatrix[i].strategy.Count; j++)
                {
                    if (kernel.StrategyMatrix[i].strategy[j] < 1000)
                    {
                        numbers[numbers.Count - 1].Add(kernel.StrategyMatrix[i].strategy[j]);
                        //Console.Write(numbers[numbers.Count - 1][numbers[numbers.Count - 1].Count - 1]);
                    }

                }
                //Console.WriteLine();
            }

            double[,] matr = new double[numbers.Count + 1, numbers[0].Count + 1];
            Console.WriteLine("Матриця пiсля видалення домiнюючих рядкiв та стовпцiв,для використання в симплекс методi:");
            for (int i = 0; i < (numbers.Count) + 1; i++)
            {
                if (i == 2) { matr[i, 0] = 0; }
                else { matr[i, 0] = 1; }

                Console.Write(matr[i, 0] + " ");

                for (int j = 1; j < (numbers.Count) + 1; j++)
                {
                    if (i == 2) { matr[i, j] = -1; }
                    else { matr[i, j] = numbers[i][j - 1]; }


                    Console.Write(matr[i, j] + " ");
                }
                Console.WriteLine();

            }
            Console.WriteLine();
            return matr;
        }
        public static bool hasAppropriateNumber(List<double> arr)
        {
            for (int i = 0; i < arr.Count; i++)
            {
                if (arr[i] < 1000)
                {
                    return true;
                }
            }
            return false;
        }

        static double[][] MatrixInverse(double[][] matrix)
        {
            int n = matrix.Length;
            double[][] result = MatrixDuplicate(matrix);

            int[] perm;
            int toggle;
            double[][] lum = MatrixDecompose(matrix, out perm,
              out toggle);
            if (lum == null)
                throw new Exception("Unable to compute inverse");

            double[] b = new double[n];
            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < n; ++j)
                {
                    if (i == perm[j])
                        b[j] = 1.0;
                    else
                        b[j] = 0.0;
                }

                double[] x = HelperSolve(lum, b);

                for (int j = 0; j < n; ++j)
                    result[j][i] = x[j];
            }
            return result;
        }

        static double[][] MatrixDuplicate(double[][] matrix)
        {
            // allocates/creates a duplicate of a matrix.
            double[][] result = matrix;
            for (int i = 0; i < matrix.Length; ++i) // copy the values
                for (int j = 0; j < matrix[i].Length; ++j)
                    result[i][j] = matrix[i][j];
            return result;
        }

        static double[] HelperSolve(double[][] luMatrix, double[] b)
        {
            // before calling this helper, permute b using the perm array
            // from MatrixDecompose that generated luMatrix
            int n = luMatrix.Length;
            double[] x = new double[n];
            b.CopyTo(x, 0);

            for (int i = 1; i < n; ++i)
            {
                double sum = x[i];
                for (int j = 0; j < i; ++j)
                    sum -= luMatrix[i][j] * x[j];
                x[i] = sum;
            }

            x[n - 1] /= luMatrix[n - 1][n - 1];
            for (int i = n - 2; i >= 0; --i)
            {
                double sum = x[i];
                for (int j = i + 1; j < n; ++j)
                    sum -= luMatrix[i][j] * x[j];
                x[i] = sum / luMatrix[i][i];
            }

            return x;
        }

        static double[][] MatrixDecompose(double[][] matrix, out int[] perm, out int toggle)
        {
            // Doolittle LUP decomposition with partial pivoting.
            // rerturns: result is L (with 1s on diagonal) and U;
            // perm holds row permutations; toggle is +1 or -1 (even or odd)
            int rows = matrix.Length;
            int cols = matrix[0].Length; // assume square
            if (rows != cols)
                throw new Exception("Attempt to decompose a non-square m");

            int n = rows; // convenience

            double[][] result = MatrixDuplicate(matrix);

            perm = new int[n]; // set up row permutation result
            for (int i = 0; i < n; ++i) { perm[i] = i; }

            toggle = 1; // toggle tracks row swaps.
                        // +1 -greater-than even, -1 -greater-than odd. used by MatrixDeterminant

            for (int j = 0; j < n - 1; ++j) // each column
            {
                double colMax = Math.Abs(result[j][j]); // find largest val in col
                int pRow = j;
                //for (int i = j + 1; i less-than n; ++i)
                //{
                //  if (result[i][j] greater-than colMax)
                //  {
                //    colMax = result[i][j];
                //    pRow = i;
                //  }
                //}

                // reader Matt V needed this:
                for (int i = j + 1; i < n; ++i)
                {
                    if (Math.Abs(result[i][j]) > colMax)
                    {
                        colMax = Math.Abs(result[i][j]);
                        pRow = i;
                    }
                }
                // Not sure if this approach is needed always, or not.

                if (pRow != j) // if largest value not on pivot, swap rows
                {
                    double[] rowPtr = result[pRow];
                    result[pRow] = result[j];
                    result[j] = rowPtr;

                    int tmp = perm[pRow]; // and swap perm info
                    perm[pRow] = perm[j];
                    perm[j] = tmp;

                    toggle = -toggle; // adjust the row-swap toggle
                }

                // --------------------------------------------------
                // This part added later (not in original)
                // and replaces the 'return null' below.
                // if there is a 0 on the diagonal, find a good row
                // from i = j+1 down that doesn't have
                // a 0 in column j, and swap that good row with row j
                // --------------------------------------------------

                if (result[j][j] == 0.0)
                {
                    // find a good row to swap
                    int goodRow = -1;
                    for (int row = j + 1; row < n; ++row)
                    {
                        if (result[row][j] != 0.0)
                            goodRow = row;
                    }

                    if (goodRow == -1)
                        throw new Exception("Cannot use Doolittle's method");

                    // swap rows so 0.0 no longer on diagonal
                    double[] rowPtr = result[goodRow];
                    result[goodRow] = result[j];
                    result[j] = rowPtr;

                    int tmp = perm[goodRow]; // and swap perm info
                    perm[goodRow] = perm[j];
                    perm[j] = tmp;

                    toggle = -toggle; // adjust the row-swap toggle
                }
                // --------------------------------------------------
                // if diagonal after swap is zero . .
                //if (Math.Abs(result[j][j]) less-than 1.0E-20) 
                //  return null; // consider a throw

                for (int i = j + 1; i < n; ++i)
                {
                    result[i][j] /= result[j][j];
                    for (int k = j + 1; k < n; ++k)
                    {
                        result[i][k] -= result[i][j] * result[j][k];
                    }
                }
            }
            return result;
        }
    }
        

}


