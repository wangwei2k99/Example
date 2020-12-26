using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace SubsetSumAlgorithm
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CreatGird();
        }

        private async void Error()
        {
            this.TextBox.Text = "请输入数字!";
            this.TextBox.Background = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            await Task.Delay(1000);
            this.TextBox.Text = "";
            this.TextBox.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
        }

        private void CreatGird()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("单号", typeof(string));
            dt.Columns.Add("金额", typeof(double));
            DataRow row = dt.NewRow();
            row[0] = "点击此行按Ctrl+V粘贴数据";
            dt.Rows.Add(row);
            StaticVar.GridView = dt;
            Gird.ItemsSource = StaticVar.GridView.AsDataView();
            Gird.ColumnWidth = 200;
        }

        private List<DataSource> Algorithm(DataSource[] lds, double target)
        {
            int count = lds.Length;
            var factor = 3;
            int num = 0;
            int ntime = 0;
            int nmin = 0, nmax = 0;
            for (int i = count - 1; i >= 0; i--)
            {
                if (lds[i].Amount < target) { nmin += 1; target -= lds[i].Amount; } else { nmin += 1; break; }
            }
            target = StaticVar.TargetValue;
            for (int i = 0; i < count; i++)
            {
                if (lds[i].Amount < target) { target -= lds[i].Amount; } else { nmax = i; break; }
            }
            int cnt = 0;
            do
            {
                ntime++;
                num = ntime;
                if (num == 0)
                { num = count; }
                else if (ntime > 10)
                { num = count; }
                else if (num < nmin || num > nmax)
                { num = count; }
                target = StaticVar.TargetValue;
                Random rnd = new Random(DateTime.Now.Millisecond);
                for (int i = 1; i <= num; i++)
                {
                    var rn = (double)rnd.Next(100) / 100;
                    var r = (int)Math.Floor(rn * (count - i + 1)) + i;
                    if (num == count)
                    {
                        if (target < lds[r - 1].Amount)
                        {
                            num = i - 1;
                            break;
                        }
                    }
                    var temp = new DataSource();
                    temp = lds[r - 1];
                    lds[r - 1] = lds[i - 1];
                    lds[i - 1] = temp;
                    target = target - temp.Amount;
                }
                cnt = 0;
                while (((int)Math.Floor(target * Math.Pow(10, factor))) != 0)
                {
                    cnt++; if (cnt > count/6) break;
                    var rn = (double)rnd.Next(100) / 100;
                    var i = (int)Math.Floor(rn * (count - num)) + num + 1;
                    //var i = (int)Math.Floor(rn * (count - num)) + num;
                    rn = (double)rnd.Next(100) / 100;
                    var r = (int)Math.Floor(rn / 100 * num) + 1;
                    var s1 = target + lds[i - 1].Amount - lds[r - 1].Amount;
                    if (Math.Abs(s1) < Math.Abs(target))
                    {
                        var temp = new DataSource();
                        temp = lds[r - 1];
                        lds[r - 1] = lds[i - 1];
                        lds[i - 1] = temp;
                        target = target + temp.Amount - lds[r - 1].Amount;
                    }
                }
            } while ((int)Math.Floor(target * Math.Pow(10, factor)) != 0);
            var listView = lds.Take(num).ToList();
            listView = listView.OrderBy(o => o.Amount).ToList();
            return listView;
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Regex.IsMatch(this.TextBox.Text.Trim(), @"^(([^0][0-9]+|0)\.([0-9]{1,2})$)|^(([^0][0-9]+|0)$)|^(([1-9]+)\.([0-9]{1,2})$)|^(([1-9]+)$)"))
            {
                this.TextBox.Text = this.TextBox.Text.Trim();
                this.TextBox.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                StaticVar.TargetValue = Convert.ToDouble(this.TextBox.Text);
            }
            else
            {
                Error();
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (StaticVar.TargetValue == 0)
            {
                Error();
            }
            else
            {
                this.Btn1.Dispatcher.Invoke(new Action(delegate
                {
                    this.Result.Visibility = Visibility.Collapsed;
                    this.Btn1.Content = "计算中...";
                    this.Btn1.IsEnabled = false;
                }));
                DateTime startTime = DateTime.Now;

                var dt1 = StaticVar.GridView;
                var target = StaticVar.TargetValue;
                var datatemp = dt1.AsEnumerable().Where(P => Convert.ToDouble(P["金额"]) > target).ToList();
                datatemp.ForEach(P => dt1.Rows.Remove(P));
                int count = dt1.Rows.Count;
                List<DataSource> lds = new List<DataSource>();
                foreach (DataRow row in dt1.Rows)
                {
                    DataSource ds = new DataSource();
                    ds.OrderNumber = row["单号"].ToString();
                    ds.Amount = Convert.ToDouble(row["金额"]);
                    //ds.SerialNumber = Convert.ToInt32(row["排序"]);
                    lds.Add(ds);
                }
                DataSource[] arrds = lds.ToArray();
                int processes = 8;
                List<CancellationTokenSource> ctsList = new List<CancellationTokenSource>();
                Task<List<DataSource>>[] tasks = new Task<List<DataSource>>[processes];
                for (int i = 0; i < processes; i++)
                {
                    var cts = new CancellationTokenSource();
                    Task<List<DataSource>> task = Task.Run(() => { return Algorithm(arrds.Clone() as DataSource[], target); }, cts.Token);
                    ctsList.Add(cts);
                    tasks[i] = task;
                }
                var tfirst = await Task.WhenAny(tasks);
                var listView = tfirst.Result;
                foreach (var cts in ctsList)
                { cts.Cancel(); }
                this.Dispatcher.Invoke(() =>
                {
                    this.Gird.ItemsSource = listView;
                    this.Amount.Text = (from r in listView.AsEnumerable() select r.Amount).Sum().ToString();
                    this.ReCount.Text = listView.Count.ToString();
                    this.Time.Text = $"{ Math.Round((DateTime.Now - startTime).TotalSeconds, 2)}秒";
                    this.Time.Background = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                    bool copy = CopyListToClipBorad(listView);
                });
                this.Btn1.Content = "重新计算";
                this.Btn1.IsEnabled = true;
                this.Result.Visibility = Visibility.Visible;
            }
        }
        public virtual bool CopyDataTableToClipBorad(DataTable dataTable)
        {
            string str = null;
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                for (int j = 0; j < dataTable.Columns.Count; j++)
                {
                    if (j == 0)
                    {
                        str = str + dataTable.Rows[i][j].ToString();
                    }
                    else
                    {
                        str = str + "\t" + dataTable.Rows[i][j];
                    }
                }
                if (i != dataTable.Rows.Count - 1)
                {
                    str = str + "\n";
                }
            }
            if (!string.IsNullOrEmpty(str))
            {
                Clipboard.SetDataObject(str, true);
                return true;
            }
            return false;
        }
        public virtual bool CopyListToClipBorad<T>(List<T> list)
        {
            string str = null;
            for (int i = 0; i < list.Count; i++)
            {
                PropertyInfo[] infos = list[i].GetType().GetProperties();
                int ig = 0;
                foreach (PropertyInfo info in infos)
                {
                    if (ig == 0)
                    {
                        str = $"{str}{info.GetValue(list[i])}";
                    }
                    else
                    {
                        str = $"{str}\t{info.GetValue(list[i])}";
                    }
                    ig++;
                }
                if (i != list.Count - 1)
                {
                    str = str + "\n";
                }
            }
            if (!string.IsNullOrEmpty(str))
            {
                Clipboard.SetDataObject(str, true);
                return true;
            }
            return false;
        }
        private void grd_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.V)
            {
                StaticVar.GridView = null;
                CreatGird();
                try
                {
                    // 获取剪切板的内容，并按行分割   
                    string pasteText = Clipboard.GetText();
                    if (string.IsNullOrEmpty(pasteText))
                        return;
                    int tnum = 0;//剪贴板列数
                    int nnum = 0;//剪贴板行数
                                 //获得当前剪贴板内容的行、列数
                    for (int i = 0; i < pasteText.Length; i++)
                    {
                        if (pasteText.Substring(i, 1) == "\t")
                        {
                            tnum++;
                        }
                        if (pasteText.Substring(i, 1) == "\n")
                        {
                            nnum++;
                        }
                    }
                    string[,] arr;
                    //粘贴板上的数据来自于EXCEL时，每行末都有\n，在DATAGRIDVIEW内复制时，最后一行末没有\n
                    if (pasteText.Substring(pasteText.Length - 1, 1) == "\n")
                    {
                        nnum = nnum - 1;
                    }
                    tnum = tnum / (nnum + 1);
                    arr = new string[nnum + 1, tnum + 1];//定义一个二维数组
                    string rowstr;
                    rowstr = "";
                    //对数组赋值
                    for (int i = 0; i < (nnum + 1); i++)
                    {
                        for (int colIndex = 0; colIndex < (tnum + 1); colIndex++)
                        {
                            //一行中的最后一列
                            if (colIndex == tnum && pasteText.IndexOf("\r") != -1)
                            {
                                rowstr = pasteText.Substring(0, pasteText.IndexOf("\r"));
                            }
                            //最后一行的最后一列
                            if (colIndex == tnum && pasteText.IndexOf("\r") == -1)
                            {
                                rowstr = pasteText.Substring(0);
                            }
                            //其他行列
                            if (colIndex != tnum)
                            {
                                rowstr = pasteText.Substring(0, pasteText.IndexOf("\t"));
                                pasteText = pasteText.Substring(pasteText.IndexOf("\t") + 1);
                            }
                            arr[i, colIndex] = rowstr;
                        }
                        //截取下一行数据
                        pasteText = pasteText.Substring(pasteText.IndexOf("\n") + 1);
                    }
                    //获取获取当前选中单元格所在的行序号
                    int rowindex = this.Gird.SelectedIndex;
                    DataTable dt = StaticVar.GridView.Clone();
                    //List<List<string>> listBoxGriderModel = new List<List<string>>();
                    //dt.Columns.Add("排序", typeof(int));
                    for (int j = 0; j < (nnum + 1); j++)
                    {
                        DataRow row = dt.NewRow();
                        row["单号"] = arr[j, 0];
                        row["金额"] = Convert.ToDouble(arr[j, 1].Trim());
                        //row["排序"] = j + 1;
                        dt.Rows.Add(row);
                        //listBoxGriderModel.Add(new List<string>() {data[j, 0], data[j, 1]});
                    }
                    dt.DefaultView.Sort = "金额 ASC";
                    dt = dt.DefaultView.ToTable();
                    StaticVar.GridView = dt;
                    this.Gird.ItemsSource = dt.AsDataView();
                    int pasteNum = StaticVar.GridView.Rows.Count;
                    this.Num.Text = pasteNum.ToString();
                }
                catch
                {
                    MessageBox.Show("粘贴区域大小不一致");
                    return;
                }
            }
        }
    }
}
