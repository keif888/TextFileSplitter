﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime.Wrapper;

using IDTSOutput = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSOutput100;
using IDTSCustomProperty = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSCustomProperty100;
using IDTSOutputColumn = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSOutputColumn100;
using IDTSOutputColumnCollection = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSOutputColumnCollection100;
using IDTSInput = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSInput100;
using IDTSInputColumn = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSInputColumn100;
using IDTSVirtualInputColumn = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSVirtualInputColumn100;
using IDTSVirtualInput = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSVirtualInput100;
using IDTSInputColumnCollection = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSInputColumnCollection100;
using IDTSComponentMetaData = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSComponentMetaData100;



namespace Martin.SQLServer.Dts
{
    class ManageColumns
    {
        public const String ErrorMessageColumnName = "ErrorMessage";
        public const string ColumnDataColumnName = "ColumnData";
        public const string RowDataColumnName = "RowData";
        public const int DefaultStringColumnSize = 255;

        public static void AddErrorOutputColumns(IDTSOutput errorOutput)
        {
            IDTSOutputColumnCollection outputColumnCollection = errorOutput.OutputColumnCollection;
            IDTSOutputColumn outputColumn = outputColumnCollection.New();
            outputColumn.Name = ErrorMessageColumnName;
            outputColumn.SetDataTypeProperties(DataType.DT_WSTR, 4000, 0, 0, 0);
            outputColumn = outputColumnCollection.New();
            outputColumn.Name = ColumnDataColumnName;
            outputColumn.SetDataTypeProperties(DataType.DT_WSTR, 4000, 0, 0, 0);
            outputColumn = outputColumnCollection.New();
            outputColumn.Name = RowDataColumnName;
            outputColumn.SetDataTypeProperties(DataType.DT_WSTR, 4000, 0, 0, 0);
        }

        public static void SetOutputColumnDefaults(IDTSOutputColumn outputColumn, int CodePage)
        {
            // If any column has a failure, use the Output level dispositions for redirection.
            //outputColumn.ErrorRowDisposition = DTSRowDisposition.RD_FailComponent;
            //outputColumn.TruncationRowDisposition = DTSRowDisposition.RD_FailComponent;
            //outputColumn.ErrorOrTruncationOperation = MessageStrings.ColumnLevelErrorTruncationOperation;
            outputColumn.SetDataTypeProperties(DataType.DT_STR, DefaultStringColumnSize, 0, 0, CodePage);
        }

        internal static void AddNumberOfRowsOutputColumns(IDTSOutput numberOfRows)
        {
            IDTSOutputColumnCollection outputColumnCollection = numberOfRows.OutputColumnCollection;
            IDTSOutputColumn outputColumn = outputColumnCollection.New();
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
