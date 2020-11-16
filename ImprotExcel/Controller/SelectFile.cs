using ImprotExcel.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImprotExcel.Controller
{
    class SelectFile
    {
        public void SelectFileExcel()
        {
            OpenFileDialog fileDialog = new OpenFileDialog()
            {
                Multiselect = true,
                Filter = "Excel文件|*.xls*",
                RestoreDirectory = true
            };
            if (fileDialog.ShowDialog()==true&&fileDialog.FileNames!=null)
            {
                PublicVariable.FileNames = fileDialog.FileNames;
            }
            else
            {
                return;
            }

        }
    }
}
