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

namespace DingTalkQRcodeLoginWpf
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DingTalkQRcodeLogin : Window
    {
        public DingTalkQRcodeLogin()
        {
            CefSharp.Wpf.CefSettings cefSettings = new CefSharp.Wpf.CefSettings();
            cefSettings.AcceptLanguageList = "zh-CN";
            CefSharp.Cef.Initialize(cefSettings, true);
            string str = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

            InitializeComponent();
            this.browser.Address = $@"{str}Views\DingTalkQRcodeLogin.html";
        }
    }
}
