using System.Data;

namespace SubsetSumAlgorithm
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
        public string OrderNumber { get; set; }
        public double Amount { get; set; }
        //public int SerialNumber { get; set; }
    }
}
