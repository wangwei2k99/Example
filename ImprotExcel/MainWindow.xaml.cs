using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
using ImprotExcel.Controller;
using ImprotExcel.Model;
using System.Data;

namespace ImprotExcel
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Btn1_Click(object sender, RoutedEventArgs e)
        {
            new SelectFile().SelectFileExcel();
            List<DataSet> list = new List<DataSet>();
            foreach (string fileName in PublicVariable.FileNames)
            {
                DataSet dataSet = Npoi.ExcelToDataSet(fileName);
                list.Add(dataSet);
            }
            this.DataGrid1.ItemsSource = list;
        }
    }
}
