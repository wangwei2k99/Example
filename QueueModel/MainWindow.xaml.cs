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
using System.Reflection;
using System.Text.RegularExpressions;

namespace QueueModel
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private Queue<UserInfo> ts;
        private Dictionary<int, Desk> ds;
        public MainWindow()
        {
            InitializeComponent();
            ts = new Queue<UserInfo>();
            ds = new Dictionary<int, Desk>();
            this.desknum.Text = "1";
            EnterDesks.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            de.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }

        private void en_Click(object sender, RoutedEventArgs e)
        {
            UserInfo user = new UserInfo()
            {
                QueueID = Queue.Qid,
                Status = false,
                Number = 1
            };
            ts.Enqueue(user);
            this.tb1.Text = $"最大叫号为{user.QueueID}";
            this.tb2.Text = $"{ts.Count}位顾客在等待";
        }

        private void de_Click(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.Invoke(new Action(async () =>
            {
                while (true)
                {
                    this.de.IsEnabled = false;
                    foreach (var desk in ds.Values)
                    {
                        if (desk.Status == false && ts.Count > 0)
                        {
                            desk.Status = true;
                            var current = ts.Dequeue();
                            this.rtb.AppendText($"请{current.QueueID}号顾客到{desk.DeskID}号桌就餐!");
                            this.rtb.AppendText(Environment.NewLine);
                            this.tb2.Text = $"还有{ts.Count}个号码在等待!";
                        }
                        Button button = SPanel1.FindName($"desk{desk.DeskID}") as Button;
                        //Button button = (Button)this.GetType().GetField($"desk{desk.DeskID}", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase).GetValue(this);
                        if (desk.Status == false)
                        {
                            button.Content = $"{desk.DeskID}号桌空";
                            button.IsEnabled = false;
                        }
                        else
                        {
                            button.Content = $"{desk.DeskID}号桌满";
                            button.IsEnabled = true;
                        }
                    }
                    await Task.Delay(1000);
                }
            }));
        }

        private void UserLeft(string btnName)
        {
            //string btnName = Regex.Match(methodName, "[desk]+[0-9]+").Groups[0].ToString();
            foreach (var desk in ds.Values)
            {
                if (btnName == $"desk{ desk.DeskID}")
                {
                    Button button = SPanel1.FindName($"desk{desk.DeskID}") as Button;
                    //Button button = (Button)this.GetType().GetField(btnName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase).GetValue(this);
                    desk.Status = false;
                    this.rtb.AppendText($"第{desk.DeskID}号桌顾客已离场!");
                    this.rtb.AppendText(Environment.NewLine);
                    button.Content = $"{desk.DeskID}号桌空";
                    button.IsEnabled = false;
                }
            }
        }

        private void rtb_TextChanged(object sender, TextChangedEventArgs e)
        {
            rtb.ScrollToEnd();
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            UserLeft(btn.Name);
        }
        private void Enter_Click(object sender, RoutedEventArgs e)
        {
            if (Regex.IsMatch(this.desknum.Text, "[0-9]+"))
            {
                int dsCount = ds.Count;
                int dsTarget = Convert.ToInt32(this.desknum.Text);
                if (dsTarget > dsCount)
                {
                    for (int i = 1; i <= dsTarget - dsCount; i++)
                    {
                        if (ds.Keys.Contains(i))
                        {
                            dsTarget++;
                        }
                        else if (!ds.Keys.Contains(i))
                        {
                            Desk desk = new Desk()
                            {
                                DeskID = i,
                                Status = false,
                            };
                            ds.Add(desk.DeskID, desk);
                            Button btn = new Button()
                            {
                                Name = $"desk{i}",
                                Content = $"{i}号桌空",
                                Visibility = Visibility.Visible,
                                IsEnabled = false,
                                Height = 30,
                                HorizontalAlignment = HorizontalAlignment.Stretch
                            };
                            btn.Click += new RoutedEventHandler(Btn_Click);
                            this.SPanel1.Children.Add(btn);
                            this.SPanel1.RegisterName(btn.Name, btn);
                        }
                    }
                }
                else if (dsTarget < dsCount)
                {
                    List<int> tmp = new List<int>();
                    foreach (var i in ds.Keys)
                    {
                        Button btn = SPanel1.FindName($"desk{i}") as Button;
                        if (btn.Content.ToString() == $"{i}号桌空")
                        {
                            this.SPanel1.UnregisterName($"desk{i}");
                            this.SPanel1.Children.Remove(btn);
                            tmp.Add(i);
                        }
                    }
                    foreach (var i in tmp)
                    {
                        ds.Remove(i);
                    }
                }
            }
        }
    }

    class UserInfo
    {
        public int QueueID { get; set; }
        public bool Status { get; set; }
        public int Number { get; set; }
    }

    public class Desk
    {
        public bool Status { get; set; }
        private int deskid;
        public int DeskID
        {
            get { return deskid; }
            set { deskid = value; }
        }
    }

    public static class Queue
    {
        private static int qid;
        public static int Qid
        {
            get
            {
                qid++;
                return qid;
            }
        }
    }
}
