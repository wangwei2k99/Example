using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Data;
using System.IO;
using System.Linq;
namespace ImprotExcel.Controller
{
    public static class Npoi
    {
        /// <summary>
        /// Excel文件转DataSet
        /// </summary>
        /// <param 传入Excel文件路径="filePath"></param>
        /// <returns DataSet></returns>
        public static DataSet ExcelToDataSet(string filePath)
        {
            IWorkbook workbook;
            DataSet dataSet = new DataSet();
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                string extension = Path.GetExtension(filePath).ToLower();
                if (extension == ".xlsx" | extension == ".xlsm")
                {
                    workbook = new XSSFWorkbook(fileStream);
                }
                else if (extension == ".xls")
                {
                    workbook = new HSSFWorkbook(fileStream);
                }
                else
                {
                    workbook = null;
                }
                if (workbook == null) { return null; }
            }
            foreach (ISheet sheet in workbook)
            {
                DataTable dataTable = new DataTable();
                int maxRow = 0, maxCol = 0, nullRows = 0, blankRows = 5;//nullRows实际连续空行,blankRows允许连续空行.
                for (int r = 0; r <= sheet.LastRowNum + blankRows; r++)
                {
                    bool rowisNull = true;
                    if (sheet.GetRow(r) != null)
                    {
                        bool rowMaxcol = false;
                        for (int c = sheet.GetRow(r).LastCellNum - 1; c >= 0; c--)
                        {
                            if (sheet.GetRow(r).GetCell(c) != null && !string.IsNullOrEmpty(sheet.GetRow(r).GetCell(c).ToString().Trim()))
                            {
                                rowisNull = false;
                                if (rowMaxcol == false) { if (c > maxCol) { maxCol = c; rowMaxcol = true; } }
                                break;
                            }
                        }
                    }
                    if (rowisNull == true) { nullRows++; } else if (rowisNull == false) { nullRows = 0; }
                    if (nullRows == blankRows) { maxRow = r - blankRows; break; }//maxRow最大有效行数,连续空行5以上,后面的会舍弃.
                }
                #region 标题
                IRow title = sheet.GetRow(sheet.FirstRowNum);
                for (int i = 0; i <= maxCol; i++)
                {
                    object obj = GetCellTypeValue(title.GetCell(i));
                    if (obj == null || string.IsNullOrEmpty(obj.ToString().Trim()) || dataTable.Columns.Contains(obj.ToString()))
                    {
                        dataTable.Columns.Add(new DataColumn($"C{i + 1}"));
                    }
                    else
                    {
                        dataTable.Columns.Add(new DataColumn(obj.ToString()));
                    }
                }
                #endregion
                #region 数据
                for (int r = sheet.FirstRowNum + 1; r <= maxRow; r++)
                {
                    DataRow dataRow = dataTable.NewRow();
                    if (sheet.GetRow(r) != null && sheet.GetRow(r).LastCellNum > 0)
                    {
                        for (int c = sheet.GetRow(r).FirstCellNum; c <= maxCol; c++)
                        {
                            dataRow[c] = GetCellTypeValue(sheet.GetRow(r).GetCell(c));
                        }
                    }
                    dataTable.Rows.Add(dataRow);
                }
                #endregion
                dataTable.TableName = sheet.SheetName.Trim();
                dataSet.Tables.Add(dataTable);
            }
            return dataSet;
        }
        /// <summary>
        /// 获取Excel单元格的类型及值
        /// </summary>
        /// <param 传入单元格对象ICell="cell"></param>
        /// <returns object></returns>
        public static object GetCellTypeValue(ICell cell)
        {
            if (cell == null)
            {
                return null;
            }
            switch (cell.CellType)
            {
                case CellType.Blank:
                    return null;
                case CellType.Boolean:
                    return cell.BooleanCellValue;
                case CellType.Numeric:
                    if (DateUtil.IsCellDateFormatted(cell))
                    {
                        return cell.DateCellValue;
                    }
                    else
                    {
                        return cell.NumericCellValue;
                    }
                case CellType.String:
                    return cell.StringCellValue.Trim();
                case CellType.Formula:
                //case CellType.Error:
                //case CellType.Unknown:
                default:
                    cell.SetCellType(CellType.String);
                    return cell.StringCellValue.Trim();
            }
        }
        /// <summary>
        /// DataSet导出成Excel
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="file"></param>
        public static void DataSetToExcel(DataSet dataSet, string file)
        {
            IWorkbook workbook;
            string fileExt = Path.GetExtension(file).ToLower();
            if (fileExt == ".xlsx") { workbook = new XSSFWorkbook(); } else if (fileExt == ".xls") { workbook = new HSSFWorkbook(); } else { workbook = null; }
            if (workbook == null) { return; }
            for (int i = 0; i < dataSet.Tables.Count; i++)
            {
                DataTable dt = dataSet.Tables[i];
                ISheet sheet = string.IsNullOrEmpty(dt.TableName) ? workbook.CreateSheet($"Table{i}") : workbook.CreateSheet(dt.TableName);
                //表头  
                IRow row0 = sheet.CreateRow(0);
                for (int c = 0; c < dt.Columns.Count; c++)
                {
                    ICell cell = row0.CreateCell(c);
                    cell.SetCellValue(dt.Columns[c].ColumnName);
                }
                //数据  
                for (int r = 0; r < dt.Rows.Count; r++)
                {
                    IRow row1 = sheet.CreateRow(r + 1);
                    for (int c = 0; c < dt.Columns.Count; c++)
                    {
                        ICell cell = row1.CreateCell(c);
                        Type columnsType = dt.Columns[c].DataType;
                        SetCellTypeValue(columnsType, dt.Rows[r][c].ToString(), ref cell);
                    }
                }
                AutoColumnWidth(sheet, sheet.GetRow(0).LastCellNum);
                //转为字节数组  
                MemoryStream stream = new MemoryStream();
                workbook.Write(stream);
                var buf = stream.ToArray();
                //保存为Excel文件  
                using (FileStream fs = new FileStream(file, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(buf, 0, buf.Length);
                    fs.Flush();
                }
            }
        }
        /// <summary>
        /// 写入Excel时设置单元格的类型及值
        /// </summary>
        /// <param 列数据类型="columnsType"></param>
        /// <param 字符串="str"></param>
        /// <param ICell="cell"></param>
        public static void SetCellTypeValue(Type columnsType, string str, ref ICell cell)
        {
            switch (columnsType.Name)
            {
                case nameof(String):
                    cell.SetCellValue(str);
                    break;
                case nameof(DateTime):
                    DateTime datevalue;
                    DateTime.TryParse(str, out datevalue);
                    cell.SetCellValue(datevalue);
                    break;
                case nameof(Boolean):
                    bool boolvalue = false;
                    bool.TryParse(str, out boolvalue);
                    break;
                case nameof(Int16):
                case nameof(Int32):
                case nameof(Int64):
                case nameof(Byte):
                    int intvalue = 0;
                    int.TryParse(str, out intvalue);
                    cell.SetCellValue(intvalue);
                    break;
                case nameof(Decimal):
                case nameof(Double):
                    double doublevalue = 0;
                    double.TryParse(str, out doublevalue);
                    cell.SetCellValue(doublevalue);
                    break;
                case nameof(DBNull):
                    cell.SetCellValue("");
                    break;
                default:
                    cell.SetCellValue(str);
                    break;
            }
        }
        /// <summary>
        /// 删除无用列
        /// </summary>
        /// <param DataTable="dataTable"></param>
        /// <param 从第几列开始="startCol">从1开始计数,同Excel</param>
        /// <param 第几列结束="endCol">从1开始计数,同Excel</param>
        /// <returns DataTable></returns>
        public static DataTable DeleteColumns(DataTable dataTable, int startCol, int endCol)
        {
            for (int i = dataTable.Columns.Count - 1; i >= 0; i--)
            {
                if (i < startCol - 1 || i >= endCol)
                {
                    dataTable.Columns.RemoveAt(i);
                }
            }
            return dataTable;
        }
        /// <summary>
        /// 流程卡设置页码
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="colName"></param>
        public static void SetPageNumber(ref DataRow[] dataRows, string colName)
        {
            for (int i = 0; i < dataRows.Length; i++)
            {
                dataRows[i].BeginEdit();
                dataRows[i][colName] = $"第{i + 1}页，共{dataRows.Length}页";
                dataRows[i].EndEdit();
            }
        }
        /// <summary>
        /// 设置流水序号
        /// </summary>
        /// <param 传入DataTable="dataTable"></param>
        /// <param 需要设置的列名称="ColName"></param>
        public static void SetSerialNumber(DataTable dataTable, string ColName)
        {
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                dataTable.Rows[i][ColName] = i + 1;
            }
        }
        /// <summary>
        /// DataTable转换指定的列为decimal
        /// </summary>
        /// <param 传入DataTable="dataTable"></param>
        /// <param 传入列标数组="cols"></param>
        /// <returns></returns>
        public static DataTable ColumnsToDecimal(DataTable dataTable, params int[] cols)
        {
            DataTable table = dataTable.Clone();
            for (int c = 0; c < dataTable.Columns.Count; c++)
            {
                if (cols.Contains(c))
                {
                    table.Columns[c].DataType = typeof(decimal);
                }
            }
            foreach (DataRow dataRow in dataTable.Rows)
            {
                table.ImportRow(dataRow);
            }
            return table;
        }
        /// <summary>
        /// 批量设置Col值[泛型]
        /// </summary>
        public static void ColumnSetValue<T>(ref DataTable dt, T t, string ColName)
        {
            foreach (DataRow row in dt.Rows)
            {
                row[ColName] = t;
            }
        }
        /// <summary>
        /// 批量设置Col宽
        /// </summary>
        public static void AutoColumnWidth(ISheet sheet, int Cols)
        {
            for (int col = 0; col <= Cols; col++)
            {
                sheet.AutoSizeColumn(col);//自适应宽度，但是其实还是比实际文本要宽
                //int columnWidth = sheet.GetColumnWidth(col) / 256;//获取当前列宽度
                //for (int rowIndex = 1; rowIndex <= sheet.LastRowNum; rowIndex++)
                //{
                //    IRow row = sheet.GetRow(rowIndex);
                //    ICell cell = row.GetCell(col);
                //    if (cell != null)
                //    {
                //        int contextLength = Encoding.UTF8.GetBytes(cell.ToString()).Length;//获取当前单元格的内容宽度
                //        columnWidth = columnWidth < contextLength ? contextLength : columnWidth;
                //    }
                //}
                //sheet.SetColumnWidth(col, columnWidth * 300);//
            }
        }
        //以下未优化
        /// <summary>
        /// 将DataTable写入模板
        /// </summary>
        /// <param name="dataTable">汇总</param>
        /// <param name="rowIndex">开始行</param>
        /// <param name="colIndex">结束行</param>
        /// <param name="shtName">要汇总的工作表名</param>
        /// <param name="path">输出路径</param>
        public static void DataTableToTemplate(DataTable dataTable, int rowIndex, int colIndex, string shtName, string path, params int[] nums)
        {
            IWorkbook wb;
            using (FileStream fs = new FileStream("Template.xlsx", FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                wb = new XSSFWorkbook(fs);
            }
            ISheet sht = wb.GetSheetAt(wb.GetSheetIndex(shtName));
            ICellStyle style = wb.CreateCellStyle();
            style.BorderTop = BorderStyle.Thin;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderBottom = BorderStyle.Thin;
            for (int r = 0; r < dataTable.Rows.Count; r++)
            {
                IRow row = sht.GetRow(rowIndex);
                int col = colIndex;
                for (int c = 0; c < dataTable.Columns.Count; c++)
                {
                    ICell cell = row.CreateCell(col);
                    cell.CellStyle = style;
                    if (nums.Contains(c))
                    {
                        double.TryParse(dataTable.Rows[r][c].ToString(), out double d);
                        cell.SetCellValue(d);
                    }
                    else
                    {
                        Type type = dataTable.Columns[c].DataType;
                        SetCellTypeValue(type, dataTable.Rows[r][c].ToString(), ref cell);
                    }
                    col++;
                }
                rowIndex++;
            }
            MemoryStream stream = new MemoryStream();
            wb.Write(stream);
            var buf = stream.ToArray();
            //保存为Excel文件
            using (FileStream fs1 = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                fs1.Write(buf, 0, buf.Length);
                fs1.Flush();
            }
        }
    }
}