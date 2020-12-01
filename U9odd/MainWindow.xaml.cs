using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Data;

namespace ReverseAlgorithm
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
        private void CreatGird()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("单号", typeof(string));
            DataRow row = dt.NewRow();
            row[0] = "点击此处按Ctrl+V粘贴数据";
            dt.Rows.Add(row);
            StaticVar.GridView = dt;
            grd.ItemsSource = StaticVar.GridView.AsDataView();
            grd.ColumnWidth = 200;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await this.Dispatcher.InvokeAsync(new Action(delegate
             {
                 this.result.Visibility = Visibility.Collapsed;
                 this.Btn1.Content = "计算中...";
                 this.Btn1.IsEnabled = false;
                 CopyDataTableToClipBorad(StaticVar.GridView);
             }));
            this.result.Visibility = Visibility.Visible;
            this.Btn1.Content = "再来一遍";
            this.Btn1.IsEnabled = true;
        }
        public virtual bool CopyDataTableToClipBorad(DataTable dataTable)
        {
            string str = null;
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                str += $"\"{dataTable.Rows[i][0]}\"";
                if (i != dataTable.Rows.Count - 1)
                {
                    str += ",";
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
                        nnum -= 1;
                    }
                    tnum /= (nnum + 1);
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
                    int rowindex = this.grd.SelectedIndex;
                    DataTable dt = StaticVar.GridView.Clone();
                    dt.Columns.Add("排序", typeof(int));
                    for (int j = 0; j < (nnum + 1); j++)
                    {
                        DataRow row = dt.NewRow();
                        row["单号"] = arr[j, 0];
                        dt.Rows.Add(row);
                    }
                    var list = dt.DefaultView.ToTable().AsEnumerable().Select(P=>P["单号"]).Distinct().ToList();
                    for (int i = dt.Rows.Count - 1; i >= 0; i--)
                    {
                        dt.Rows[i].Delete();
                    }
                    dt.AcceptChanges();
                    list.ForEach(P => dt.Rows.Add(P));
                    StaticVar.GridView = dt;
                    this.grd.ItemsSource = dt.AsDataView();
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
