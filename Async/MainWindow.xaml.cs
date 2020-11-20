using System;
using System.Collections.Generic;
using System.Data;
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

namespace Async
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private DataTable dataTable { get; set; }
        public MainWindow()
        {
            Create();
            InitializeComponent();
        }

        private void Create()
        {
            dataTable = new DataTable();
            dataTable.Columns.Add("任务进度信息", typeof(string));
        }
        private void BTN1_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                this.Dispatcher.Invoke(new Action(async delegate
                {
                    this.BTN1.IsEnabled = false;
                    DataRow row1 = dataTable.NewRow();
                    row1["任务进度信息"] = "任务1开始..";
                    dataTable.Rows.Add(row1);
                    this.GRD1.ItemsSource = dataTable.AsDataView();
                    await Task.Delay(3000);
                    DataRow row2 = dataTable.NewRow();
                    row2["任务进度信息"] = "任务1结束..";
                    dataTable.Rows.Add(row2);
                    this.GRD1.ItemsSource = dataTable.AsDataView();
                    this.BTN1.IsEnabled = true;

                }));
            });

        }

        private void BTN2_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                this.Dispatcher.Invoke(new Action(async delegate
                {
                    this.BTN2.IsEnabled = false;
                    DataRow row1 = dataTable.NewRow();
                    row1["任务进度信息"] = "任务2开始..";
                    dataTable.Rows.Add(row1);
                    this.GRD1.ItemsSource = dataTable.AsDataView();
                    await Task.Delay(3000);
                    DataRow row2 = dataTable.NewRow();
                    row2["任务进度信息"] = "任务2结束..";
                    dataTable.Rows.Add(row2);
                    this.GRD1.ItemsSource = dataTable.AsDataView();
                    this.BTN2.IsEnabled = true;

                }));
            });
        }

        private void BTN3_Click(object sender, RoutedEventArgs e)
        {
            this.BTN3.IsEnabled = false;
            DataRow row1 = dataTable.NewRow();
            row1["任务进度信息"] = "任务3开始..";
            dataTable.Rows.Add(row1);
            this.GRD1.ItemsSource = dataTable.AsDataView();
            DataRow row2 = dataTable.NewRow();
            row2["任务进度信息"] = "任务3结束..";
            dataTable.Rows.Add(row2);
            this.GRD1.ItemsSource = dataTable.AsDataView();
            this.BTN3.IsEnabled = true;
        }
    }
}
