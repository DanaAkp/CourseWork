using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CourseWork2
{
    class Data
    {
        public static List<string> parametrs = new List<string>();
        public static int countData { get; set; }
        public static double[][] Array { get; set; }


        public static void GetCsv()
        {
            string[] vs = File.ReadAllLines(@"D:\Универ\ТВМС\таблицы\breast-cancer-wisconsin-data\data.csv");

            string[] str = vs[0].Split(',');
            countData = str.Length;

            foreach (string x in str) if (x !="") parametrs.Add(x);

            Array = new double[parametrs.Count][];
            
            for(int i = 0; i < parametrs.Count; i++)
            {
                double[] buf = new double[vs.Length - 1];
                for(int j = 1; j < vs.Length; j++)
                {
                    str = vs[j].Split(',');
                    if (str[i] == "M") buf[j-1] = 1;
                    else if (str[i] == "B") buf[j-1] = 0;
                    else
                        buf[j-1] = double.Parse(str[i].Replace('.', ','));
                }
                Array[i] = buf;
            }
        }

        public static string Output()
        {
            string s = "";
            for (int i = 0; i < parametrs.Count; i++) s += parametrs[i] + "\t";
            s += "\n";
            for (int i = 0; i < countData; i++)
            {
                for (int j = 0; j < parametrs.Count; j++)
                {
                    double[] buf = Array[j];
                    s += buf[i] + "\t";
                }
                s += "\n";
            }

            return s;
        }
    }
}