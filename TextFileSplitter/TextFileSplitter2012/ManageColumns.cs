using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime.Wrapper;
using System;

namespace Martin.SQLServer.Dts
{
    class ManageColumns
    {
        private const int DefaultStringColumnSize = 255;

        /// <summary>
        /// Add the columns required to the Error Output.
        /// </summary>
        /// <param name="errorOutput">The actual error output</param>
        public static void AddErrorOutputColumns(IDTSOutput100 errorOutput)
        {
            IDTSOutputColumnCollection100 outputColumnCollection = errorOutput.OutputColumnCollection;
            IDTSOutputColumn100 outputColumn = outputColumnCollection.New();
            outputColumn.Name = MessageStrings.ErrorMessageColumnName;
            outputColumn.SetDataTypeProperties(DataType.DT_WSTR, 4000, 0, 0, 0);
            outputColumn = outputColumnCollection.New();
            outputColumn.Name = MessageStrings.ColumnDataColumnName;
            outputColumn.SetDataTypeProperties(DataType.DT_WSTR, 4000, 0, 0, 0);
            outputColumn = outputColumnCollection.New();
            outputColumn.Name = MessageStrings.RowDataColumnName;
            outputColumn.SetDataTypeProperties(DataType.DT_WSTR, 4000, 0, 0, 0);
        }

        /// <summary>
        /// Set a columns default properties (regardless of the column).
        /// </summary>
        /// <param name="outputColumn">The column that is to be setup</param>
        /// <param name="CodePage">The code page that is currently being used.</param>
        public static void SetOutputColumnDefaults(IDTSOutputColumn100 outputColumn, int CodePage)
        {
            outputColumn.SetDataTypeProperties(DataType.DT_STR, DefaultStringColumnSize, 0, 0, CodePage);
        }

        /// <summary>
        /// Adds the columns that are required to the Number Of Rows output
        /// </summary>
        /// <param name="numberOfRows">the number of rows output</param>
        internal static void AddNumberOfRowsOutputColumns(IDTSOutput100 numberOfRows)
        {
            IDTSOutputColumnCollection100 outputColumnCollection = numberOfRows.OutputColumnCollection;
            IDTSOutputColumn100 outputColumn = outputColumnCollection.New();
            outputColumn.Name = MessageStrings.KeyValueColumnName;
            outputColumn.SetDataTypeProperties(DataType.DT_STR, DefaultStringColumnSize, 0, 0, 1252);
            outputColumn.Description = MessageStrings.KeyValueColumnDescription;
            outputColumn = outputColumnCollection.New();
            outputColumn.Name = MessageStrings.NumberOfRowsColumnName;
            outputColumn.SetDataTypeProperties(DataType.DT_I8, 0, 0, 0, 0);
            outputColumn.Description = MessageStrings.NumberOfRowsColumnDescription;
            outputColumn = outputColumnCollection.New();
            outputColumn.Name = MessageStrings.KeyValueStatusColumnName;
            outputColumn.SetDataTypeProperties(DataType.DT_STR, DefaultStringColumnSize, 0, 0, 1252);
            outputColumn.Description = MessageStrings.KeyValueStatusColumnDescription;
        }
    }
}
