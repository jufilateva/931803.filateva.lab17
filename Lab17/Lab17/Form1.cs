using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab17
{
    public partial class Form1 : Form
    {
        int i = 0, Num;
        double lyam1, lyam2;
        double[] freq,  tau; double t0 = 0, t1 = 0.5;
        double[] p1, p2, aggr_p;
        double chi;

        Random r = new Random();
        int n;

        public Form1()
        {
            InitializeComponent();
            Num = 100000;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            lyam1 = (double)numericUpDown1.Value;
            lyam2 = (double)numericUpDown2.Value;
            p1 = new double[Num];
            p2 = new double[Num];
            p1[0] = t0;
            p2[0] = t1;

            for (i = 1; i< Num; i++)
            {
                p1[i] = p1[i - 1] + Math.Log(r.NextDouble()) / lyam1;
                p2[i] = p2[i - 1] + Math.Log(r.NextDouble()) / lyam2;
            }

            // слить их в один поток
            aggr_p = new double[2*Num];
            int M = 2 + Num;

            p1.CopyTo(aggr_p, 0);
            p2.CopyTo(aggr_p, Num);

            MergeSort(aggr_p, 0, M-1);

            tau = new double[M - 1];
            for( int i = 1, j = 0; i<M-1; i++, j++)
            {
                tau[j] = aggr_p[i] - aggr_p[i - 1];
            }

           
            n = (int)Math.Log((double)M) + 1; 

            double min = tau.Min();
            double h = (tau.Max() - tau.Min()) / (double)n; 
            freq = new double[n]; for (int i = 0; i < n; i++) freq[i] = 0;

            for (int i = 0; i < M-1; i++)
            {
                for (int j = 1; j <= n; j++)
                {
                    if (tau[i] < min + j * h) { freq[j - 1]++; break; }
                }
            }
            for (int i = 0; i < n; i++) freq[i] = freq[i] / (double)M; 

            chart1.Series[0].Points.Clear();
            for (int i = 0; i < n; i++) chart1.Series[0].Points.AddXY(min + (i+1) * h/(double)2, freq[i]);

            textBox2.Text = isChiSquared(n, M, freq, h, min).ToString();
        }

        double prob(double l, double a, double b)
        {
            return Math.Exp(-l * b) - Math.Exp(-l * a);
        }


        bool isChiSquared(int m, int N, double[] freq, double h, double min)
        {
            double square_hi = 19.675;
            chi = 0;
            double a = min;
            double b = min + h;
            for(int i = 0; i<m; i++)
            {
                chi += freq[i] * freq[i] / ((double)N* prob(lyam1 + lyam2, a, b));
                a = b; b += h;
            }
            chi -= (double)N;

            return (chi <= square_hi);
        }

        static void Merge(double[] array, int lowIndex, int middleIndex, int highIndex)
        {
            var left = lowIndex;
            var right = middleIndex + 1;
            var tempArray = new double[highIndex - lowIndex + 1];
            var index = 0;

            while ((left <= middleIndex) && (right <= highIndex))
            {
                if (array[left] < array[right])
                {
                    tempArray[index] = array[left];
                    left++;
                }
                else
                {
                    tempArray[index] = array[right];
                    right++;
                }

                index++;
            }

            for (var i = left; i <= middleIndex; i++)
            {
                tempArray[index] = array[i];
                index++;
            }

            for (var i = right; i <= highIndex; i++)
            {
                tempArray[index] = array[i];
                index++;
            }

            for (var i = 0; i < tempArray.Length; i++)
            {
                array[lowIndex + i] = tempArray[i];
            }
        }

        //сортировка слиянием
        static double[] MergeSort(double[] array, int lowIndex, int highIndex)
        {
            if (lowIndex < highIndex)
            {
                var middleIndex = (lowIndex + highIndex) / 2;
                MergeSort(array, lowIndex, middleIndex);
                MergeSort(array, middleIndex + 1, highIndex);
                Merge(array, lowIndex, middleIndex, highIndex);
            }

            return array;
        }
    }
}
