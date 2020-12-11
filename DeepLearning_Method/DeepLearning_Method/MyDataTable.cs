/*     MyDataTable
*
* Version: 1.0
* Author:  Siyu Yu
* Email:   siyuyu @yangtzeu.edu.cn(or 573315294@qq.com)
* Date:    2018
* 
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace DeepLearning_Method
{
    public enum MySplitTypeEnum
    {
        点号,//"."
        逗号,//","
        冒号,//":"
        分号,//";"
        空格,//" "
        分隔符,//"-"
    }

    /// <summary>
    /// Class MyDataTable.
    /// 我的数据表，由多个数据列构成
    /// 增加:
    /// 1.可以在构造函数时，创建MyDataTable的框架。创建后能够添加数据(类似于DataTable)
    /// 2.首先创建一个新实例
    /// 3.创建实例后，添加列名，不能重复
    /// 4.可以根据列名获取列号，同理可以根据列号获取列名
    /// 5.设计的越简单越好
    /// </summary>
    public class MyDataTable
    {
        #region 公共属性

        /// <summary>
        /// 列名(不能重复哦)
        /// </summary>
        /// <value>The column names.</value>
        public List<string> ColumnNames
        {
            get
            {
                List<string> temp = new List<string>();
                foreach (var item in m_buffer)
                {
                    temp.Add(item.Name);
                }
                return temp;//返回一个复制版本，即使不小心修改也没事
            }
        }

        /// <summary>
        /// 行数
        /// </summary>
        public int RowCount
        {
            get
            {
                if (m_buffer.Count == 0) return 0;
                else
                {
                    MyColumn myColumn = m_buffer[0];
                    return m_buffer[0].Count;
                }
            }
        }
        /// <summary>
        /// 列数
        /// </summary>
        public int ColumnCount
        {
            get
            {
                return ColumnNames.Count;
            }
        }

        #endregion

        #region 数据缓冲区

        protected List<MyColumn> m_buffer = null;

        #endregion

        #region 构造方法

        public MyDataTable()
        {
            m_buffer = new List<MyColumn>();
        }
        public MyDataTable(DataTable source)
        {
            m_buffer = new List<MyColumn>();
            m_buffer = ReadFromDataTable(source).m_buffer;//Bug，这行不起加载的作用，回头再改
        }

        #endregion

        #region 方法

        #region 索引器

        /// <summary>
        /// 索引器(获取某行某列的value)
        /// </summary>
        /// <param name="RowIndex">Index of the row.</param>
        /// <param name="ColumnIndex">Index of the column.</param>
        /// <returns>System.Object.</returns>
        public object this[int RowIndex, int ColumnIndex]
        {
            get
            {
                return m_buffer[ColumnIndex][RowIndex];
            }
            set
            {
                m_buffer[ColumnIndex][RowIndex] = value;
            }
        }
        /// <summary>
        /// 索引器(获取某行某列的value)
        /// </summary>
        public object this[int RowIndex, string ColumnName]
        {
            get
            {
                return m_buffer[IndexOfColumn(ColumnName)][RowIndex];
            }
        }
        /// <summary>
        /// 获取某列(列名)，与GetColumn相同
        /// </summary>
        /// <param name="ColumnName">Name of the column.</param>
        /// <returns>MyColumn.</returns>
        public MyColumn this[string ColumnName]
        {
            get
            {
                return GetColumn(ColumnName);
            }
        }

        #endregion

        /// <summary>
        /// 根据列名获取对应的索引
        /// </summary>
        /// <param name="ColumnName">Name of the column.</param>
        /// <returns>System.Int32.</returns>
        public int IndexOfColumn(string ColumnName)
        {
            return ColumnNames.IndexOf(ColumnName);
        }

        #region 获取Row或者Column

        /// <summary>
        /// 获取某行(行索引)
        /// </summary>
        /// <param name="RowIndex">Index of the row.</param>
        /// <returns>MyRow.</returns>
        public MyRow GetRow(int RowIndex)
        {
            MyRow row = new MyRow();
            //提取某行的所有列，构成MyRow
            foreach (var ColumnName in ColumnNames)
            {
                row.Add(ColumnName, this[RowIndex, ColumnName]);
            }
            return row;
        }
        /// <summary>
        /// 获取某列(列索引)
        /// </summary>
        /// <param name="ColumnIndex">Index of the column.</param>
        /// <returns>MyColumn.</returns>
        public MyColumn GetColumn(int ColumnIndex)
        {
            return m_buffer[ColumnIndex];
        }
        /// <summary>
        /// 获取某列(列名)
        /// </summary>
        /// <param name="ColumnName">Name of the column.</param>
        /// <returns>MyColumn.</returns>
        public MyColumn GetColumn(string ColumnName)
        {
            int index = IndexOfColumn(ColumnName);
            if (index == -1) return null;
            return m_buffer[index];
        }

        #endregion

        /// <summary>
        /// 添加列对象，如果有重复，返回false
        /// </summary>
        /// <param name="ColumnName">Name of the column.</param>
        public bool AddColumn(string ColumnName)
        {
            if (ColumnName == string.Empty) return false;
            if (ColumnNames.Contains(ColumnName)) return false;

            m_buffer.Add(new MyColumn(ColumnName));

            return true;
        }
        /// <summary>
        /// (基于MyDataTable的列结构)创建1个空行
        /// </summary>
        public MyRow NewRow()
        {
            MyRow row = new MyRow();
            foreach (var ColumnName in ColumnNames)
            {
                row.Add(ColumnName, null);
            }
            return row;
        }
        /// <summary>
        /// 增加MyRow
        /// </summary>
        /// <param name="row">The row.</param>
        public void AddRow(MyRow row)
        {
            foreach (var item in row)
            {
                string ColumnName = item.Key;
                object value = item.Value;
                MyColumn myColumn = this[ColumnName];
                myColumn.Add(value);
            }
        }

        public double[,] ConvertToArray(double ValueOfNAN = -99.99)
        {
            double[,] array = new double[RowCount, ColumnCount];

            for (int r = 0; r < RowCount; r++)//遍历数据的所有行
            {
                for (int c = 0; c < ColumnCount; c++)//赋值row
                {
                    double value = ValueOfNAN;
                    bool b = double.TryParse(this[r, c].ToString(), out value);
                    array[r, c] = value;
                }
            }

            return array;
        }

        #region 读写方法

        #region 基于DataTable数据源的读写

        /// <summary>
        /// 从DataTable读取
        /// </summary>
        /// <param name="dt">The dt.</param>
        public static MyDataTable ReadFromDataTable(DataTable dt)
        {
            MyDataTable myDt = new MyDataTable();
            foreach (DataColumn Column in dt.Columns)
            {
                myDt.AddColumn(Column.ColumnName);
            }

            long N = dt.Rows.Count;//计算总量
            int counter = 0;//计数器
            using (var pb = new ConsoleProgressBar.ProgressBar())
            {
                System.Console.WriteLine("从DataTable读取");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    MyRow myRow = myDt.NewRow();//新建一行
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        string ColumnName = myDt.ColumnNames[j];
                        myRow[ColumnName] = dt.Rows[i][j];
                    }
                    myDt.AddRow(myRow);

                    counter++;
                    double progress = counter / (double)N;
                    pb.Progress.Report(progress, "正在加载数据");//更新UI
                }
            }
            return myDt;
        }
        /// <summary>
        /// 向DataTable写出
        /// </summary>
        /// <returns>DataTable.</returns>
        public static DataTable WriteToDataTable(MyDataTable myDt)
        {
            DataTable dt = new DataTable();

            #region 为datatable添加列名

            //添加数据表的列对象
            foreach (MyColumn myColumn in myDt.m_buffer)
            {
                dt.Columns.Add(myColumn.Name);
            }

            #endregion

            for (int r = 0; r < myDt.RowCount; r++)//遍历数据的所有行
            {
                DataRow row = dt.NewRow();//新建row
                for (int c = 0; c < myDt.ColumnNames.Count; c++)//赋值row
                {
                    row[c] = myDt[r, c];
                }
                dt.Rows.Add(row);//把row添加到DataTable
            }

            return dt;
        }

        #endregion

        #region Txt

        public void ReadFromTxt(string FileName)
        {

        }
        public void WriteToTxt(string FileName, string mySplit, bool Header = true)
        {
            using (StreamWriter sw = new StreamWriter(FileName, false, Encoding.UTF8))
            {
                if (Header)
                {
                    string first_row = string.Empty;
                    for (int i = 0; i < ColumnNames.Count; i++)
                    {
                        first_row += ColumnNames[i];
                        if (i == ColumnNames.Count - 1)
                            continue;
                        else
                            first_row += mySplit;
                    }
                    sw.WriteLine(first_row);
                }
                for (int r = 0; r < RowCount; r++)//遍历数据的所有行
                {
                    string row = string.Empty;
                    for (int c = 0; c < ColumnNames.Count; c++)//赋值row
                    {
                        row += this[r, c];
                        if (c == ColumnNames.Count - 1)
                            continue;
                        else
                            row += mySplit;
                    }
                    sw.WriteLine(row);
                }
            }
        }

        //暂时未使用
        string SplitText(MySplitTypeEnum mySplitType)
        {
            switch (mySplitType)
            {
                case MySplitTypeEnum.点号:
                    return ".";
                case MySplitTypeEnum.逗号:
                    return ",";
                case MySplitTypeEnum.冒号:
                    return ":";
                case MySplitTypeEnum.分号:
                    return ";";
                case MySplitTypeEnum.空格:
                    return " ";
                case MySplitTypeEnum.分隔符:
                    return "-";
                default:
                    return string.Empty;
            }
        }

        #endregion

        #endregion

        #endregion
    }

    public class MyRow : Dictionary<string, object>
    {
    }

    public class MyColumn
    {
        private string m_Name = string.Empty;
        /// <summary>
        /// 列名
        /// </summary>
        /// <value>The name.</value>
        public string Name { get { return m_Name; } }

        /// <summary>
        /// 数据缓冲区
        /// </summary>
        /// <value>The data.</value>
        protected List<object> m_Data = null;

        public int Count
        {
            get
            {
                return m_Data.Count;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="Name">The name.</param>
        public MyColumn(string Name)
        {
            m_Name = Name;
            m_Data = new List<object>();
        }

        public new string ToString()
        {
            return m_Name;
        }

        public void Add(object value)
        {
            m_Data.Add(value);
        }

        public object this[int i]
        {
            get
            {
                return m_Data[i];
            }
            set
            {
                m_Data[i] = value;
            }
        }
    }
}
