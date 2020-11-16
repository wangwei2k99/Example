using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace Event
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()

        {
            Event1.Eve1 += Event1_Eve1;
            InitializeComponent();
        }

        private async void Event1_Eve1(object sender, SolidColorBrush brush)
        {
            this.Btn1.Background = brush;
            await InvokTextAsync();
        }

        private void Btn1_Click(object sender, RoutedEventArgs e)
        {
            Window2 window2 = new Window2();
            window2.Owner = this;
            window2.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window2.Show();
        }

        private async Task InvokTextAsync()
        {
           await Task.Delay(1000);
            this.Btn1.Background = new SolidColorBrush(Color.FromArgb(100, 221, 221, 221));
        }
    }
}
