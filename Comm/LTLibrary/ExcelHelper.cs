using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTLibrary
{
    //导出Excel数据格式
    public class ExcelHelper
    { 
        /// <summary>
        /// 将DataTable数据导入到excel中
        /// </summary>
        /// <param name="data">要导入的数据</param>
        /// <param name="isColumnWritten">DataTable的列名是否要导入</param>
        /// <param name="sheetName">要导入的excel的sheet的名称</param>
        /// <returns>导入数据行数(包含列名那一行)</returns>
        public static int DataTableToExcel(string fileName, DataTable data, string sheetName = "Sheet1", bool isColumnWritten = true)
        {
            if (data == null)
                return 0;
            //excel数据总行数
            //检查文件夹是否存在，不存在创建
            string DirUrl=fileName.Substring(0, fileName.LastIndexOf("/")+1);
            if(!System.IO.Directory.Exists(DirUrl))
            {
                System.IO.Directory.CreateDirectory(DirUrl);
            }
            int count = 0;
            using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                IWorkbook workbook = null;
                ISheet sheet = null;
                if (fileName.IndexOf(".xlsx") > 0) // 2007版本
                {
                    workbook = new XSSFWorkbook();
                }
                else if (fileName.IndexOf(".xls") > 0) // 2003版本
                {
                    workbook = new HSSFWorkbook();
                }
                try
                {
                    if (workbook != null)
                    {
                        sheet = workbook.CreateSheet(sheetName);
                    }
                    else
                    {
                        return -1;
                    }

                    if (isColumnWritten == true) //写入DataTable的列名
                    {
                        IRow row = sheet.CreateRow(0);
                        for (int j = 0; j < data.Columns.Count; ++j)
                        {
                            row.CreateCell(j).SetCellValue(data.Columns[j].Caption);
                        }
                        count = 1;
                    }
                    else
                    {
                        count = 0;
                    }

                    for (int i = 0; i < data.Rows.Count; ++i)
                    {
                        IRow row = sheet.CreateRow(count);
                        for (int j = 0; j < data.Columns.Count; ++j)
                        {
                            row.CreateCell(j).SetCellValue(data.Rows[i][j].ToString());
                        }
                        ++count;
                    }
                    workbook.Write(fs); //写入到excel 
                    return count;
                }
                catch (Exception ex)
                {
                    throw ex;
                    // Console.WriteLine("Exception: " + ex.Message); 
                }
                finally
                { 
                   // fs.Flush();
                    fs.Close();
                    workbook.Close();
                }
            }
            // return -1;
        }
        /// <summary>
        /// 将excel中的数据导入到DataTable中
        /// </summary>
        /// <param name="sheetName">excel工作薄sheet的名称</param>
        /// <param name="isFirstRowColumn">第一行是否是DataTable的列名</param>
        /// <returns>返回的DataTable</returns>
        public static DataTable ExcelToDataTable(string fileName, string sheetName = "Sheet1", bool isFirstRowColumn = true)
        {
            DataTable data = new DataTable();
            int startRow = 0;
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                ISheet sheet = null;
                IWorkbook workbook = null;
                try
                {
                    if (fileName.IndexOf(".xlsx") > 0) // 2007版本
                    {
                        workbook = new XSSFWorkbook(fs);
                    }
                    else if (fileName.IndexOf(".xls") > 0) // 2003版本
                    {
                        workbook = new HSSFWorkbook(fs);
                    }
                    if (sheetName != null)
                    {
                        sheet = workbook.GetSheet(sheetName);
                        if (sheet == null) //如果没有找到指定的sheetName对应的sheet，则尝试获取第一个sheet
                        {
                            sheet = workbook.GetSheetAt(0);
                        }
                    }
                    else
                    {
                        sheet = workbook.GetSheetAt(0);
                    }
                    if (sheet != null)
                    {
                        IRow firstRow = sheet.GetRow(0);
                        int cellCount = firstRow.LastCellNum; //一行最后一个cell的编号 即总的列数

                        if (isFirstRowColumn)
                        {
                            for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                            {
                                ICell cell = firstRow.GetCell(i);
                                if (cell != null)
                                {
                                    string cellValue = cell.StringCellValue;
                                    if (cellValue != null)
                                    {
                                        DataColumn column = new DataColumn(cellValue);
                                        data.Columns.Add(column);
                                    }
                                }
                            }
                            startRow = sheet.FirstRowNum + 1;
                        }
                        else
                        {
                            startRow = sheet.FirstRowNum;
                        }

                        //最后一列的标号
                        int rowCount = sheet.LastRowNum;
                        for (int i = startRow; i <= rowCount; ++i)
                        {
                            IRow row = sheet.GetRow(i);
                            if (row == null) continue; //没有数据的行默认是null　　　　　　　

                            DataRow dataRow = data.NewRow();
                            for (int j = row.FirstCellNum; j < cellCount; ++j)
                            {
                                if (row.GetCell(j) != null) //同理，没有数据的单元格都默认是null
                                    dataRow[j] = row.GetCell(j).ToString();
                            }
                            data.Rows.Add(dataRow);
                        }
                    }
                    return data;
                }
                catch (Exception ex)
                {
                    throw ex;
                    //Console.WriteLine("Exception: " + ex.Message);
                }
                finally
                {
                    
                   // fs.Flush();
                    fs.Close();
                    workbook.Close();
                }
            }
            //return null; 
        }



    }
}
