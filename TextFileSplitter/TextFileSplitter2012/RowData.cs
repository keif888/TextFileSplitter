using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Martin.SQLServer.Dts
{
    internal class RowData : IRowParsingContext
    {
        List<string> columnValues = new List<string>();
        StringBuilder rowText = new StringBuilder();

        public string RowText
        {
            get
            {
                return this.rowText.ToString();
            }
        }

        public void ResetRowData()
        {
            this.columnValues.Clear();
            rowText.Length = 0;
        }

        public void ResetColumnData()
        {
            this.columnValues.Clear();
        }

        public void AddColumnData(string value)
        {
            this.columnValues.Add(value);
        }

        public string GetColumnData(int columnIndex)
        {
            if (columnIndex < 0 || columnIndex >= this.columnValues.Count)
            {
                throw new System.ArgumentException("columnIndex");
            }

            return this.columnValues[columnIndex];
        }

        public void RebuildRowText(string columnDelimiter)
        {
            rowText.Clear();
            if (columnValues.Count > 0)
            {
                rowText.Append(columnValues[0]);
                for (int i = 1; i < columnValues.Count; i++)
                {
                    rowText.Append(columnDelimiter);
                    rowText.Append(columnValues[i]);
                }
            }
        }

        #region IRowParsingContext Members

        public int ColumnCount
        {
            get
            {
                return columnValues.Count;
            }
        }

        public void Append(char ch)
        {
            rowText.Append(ch);
        }

        #endregion
    }
}
