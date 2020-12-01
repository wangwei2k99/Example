using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReverseAlgorithm
{
    public static class StaticVar
    {
        private static double targetValue;

        public static double TargetValue
        {
            get { return targetValue; }
            set { targetValue = value; }
        }

        private static DataTable gridView;

        public static DataTable GridView
        {
            get { return gridView; }
            set { gridView = value; }
        }

    }
    public class DataSource
    {
        public string Odd { get; set; }
    }
}
