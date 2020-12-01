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
using LinqOracle.Data;

namespace LinqOracle
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private Entities Ent { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            Ent = new Entities();
        }
        private void Btn1_Click(object sender, RoutedEventArgs e)
        {
            string 项目编码 = Tb1.Text.Trim();
            string 设计单号 = Tb2.Text.Trim();
            var query = Ent.JK_PM_MES_CTD.Where(P => P.ITEM_SETUP_NO.Contains(项目编码)&&P.SJD_NO.Contains(设计单号)).ToList();
            Dg1.DataContext = query;
        }
        private void Btn2_Click(object sender, RoutedEventArgs e)
        {
            Ent.SaveChangesAsync();
        }
    }
}
