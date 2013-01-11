using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.SqlServer.Dts.Pipeline;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
#if SQL2012
using IDTSOutput = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSOutput100;
using IDTSCustomProperty = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSCustomProperty100;
using IDTSOutputCollection = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSOutputCollection100;
using IDTSOutputColumnCollection = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSOutputColumnCollection100;
using IDTSOutputColumn = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSOutputColumn100;
using IDTSInput = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSInput100;
using IDTSInputColumn = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSInputColumn100;
using IDTSVirtualInputColumn = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSVirtualInputColumn100;
using IDTSVirtualInput = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSVirtualInput100;
using IDTSInputColumnCollection = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSInputColumnCollection100;
using IDTSComponentMetaData = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSComponentMetaData100;
using IDTSExternalMetadataColumn = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSExternalMetadataColumn100;
using IDTSRuntimeConnection = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSRuntimeConnection100;
using IDTSConnectionManagerFlatFile = Microsoft.SqlServer.Dts.Runtime.Wrapper.IDTSConnectionManagerFlatFile100;
using IDTSConnectionManagerFlatFileColumn = Microsoft.SqlServer.Dts.Runtime.Wrapper.IDTSConnectionManagerFlatFileColumn100;
#endif
#if SQL2008
    using IDTSOutput = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSOutput100;
    using IDTSCustomProperty = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSCustomProperty100;
    using IDTSOutputCollection = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSOutputCollection100;
    using IDTSOutputColumnCollection = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSOutputColumnCollection100;
    using IDTSOutputColumn = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSOutputColumn100;
    using IDTSInput = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSInput100;
    using IDTSInputColumn = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSInputColumn100;
    using IDTSVirtualInputColumn = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSVirtualInputColumn100;
    using IDTSVirtualInput = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSVirtualInput100;
    using IDTSInputColumnCollection = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSInputColumnCollection100;
    using IDTSComponentMetaData = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSComponentMetaData100;
    using IDTSExternalMetadataColumn = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSExternalMetadataColumn100;
    using IDTSRuntimeConnection = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSRuntimeConnection100;
    using IDTSConnectionManagerFlatFile = Microsoft.SqlServer.Dts.Runtime.Wrapper.IDTSConnectionManagerFlatFile100;
    using IDTSConnectionManagerFlatFileColumn = Microsoft.SqlServer.Dts.Runtime.Wrapper.IDTSConnectionManagerFlatFileColumn100;
#endif

namespace Martin.SQLServer.Dts
{
    internal class BufferSink
    {
        IComponentBufferService bufferService = null;

        IDTSOutput output = null;
        Int64 currentRowCount = 0;

        bool treatEmptyStringAsNull = true;
        private ComponentBufferService bufferService1;
        private IDTSOutput passThroughOutput;
        private bool treatNulls;
        private String columnDelimter;
        private bool concatenateIfToManyColumns;

        public BufferSink(IComponentBufferService bufferService, IDTSOutput output, bool treatEmptyStringAsNull)
        {
            ArgumentVerifier.CheckObjectArgument(bufferService, "bufferService");
            ArgumentVerifier.CheckObjectArgument(output, "output");

            this.bufferService = bufferService;
            this.output = output;
            this.treatEmptyStringAsNull = treatEmptyStringAsNull;
            this.columnDelimter = string.Empty;
            this.concatenateIfToManyColumns = false;
        }


        public BufferSink(IComponentBufferService bufferService, IDTSOutput output, bool treatEmptyStringAsNull, String columnDelimter, bool concatenateIfToManyColumns)
        {
            ArgumentVerifier.CheckObjectArgument(bufferService, "bufferService");
            ArgumentVerifier.CheckObjectArgument(output, "output");

            this.bufferService = bufferService;
            this.output = output;
            this.treatEmptyStringAsNull = treatEmptyStringAsNull;
            this.columnDelimter = columnDelimter;
            this.concatenateIfToManyColumns = concatenateIfToManyColumns;
        }

        public bool ConcatenateIfToManyColumns
        {
            get { return concatenateIfToManyColumns; }
            set { concatenateIfToManyColumns = value; }
        }

        public String ColumnDelimiter
        {
            get { return columnDelimter; }
            set { columnDelimter = value; }
        }

        public Int64 CurrentRowCount
        {
            get { return currentRowCount; }
            set { currentRowCount = value; }
        }

        public void AddRow(RowData rowData)
        {
            this.currentRowCount++;
            if (concatenateIfToManyColumns)
            {
                this.ConcatenateRowOverflow(ref rowData);
                this.bufferService.AddRow();
                AddColumns(rowData);
            }
            else
            {
                if (!this.CheckRowOverflow(rowData))
                {
                    this.bufferService.AddRow();
                    AddColumns(rowData);
                }
            }
        }

        private void ConcatenateRowOverflow(ref RowData rowData)
        {
            if ((rowData.ColumnCount > this.bufferService.ColumnCount) && (this.bufferService.ColumnCount > 0))
            {
                String overflowData = String.Empty;
                for (int i = this.bufferService.ColumnCount - 1; i < rowData.ColumnCount; i++)
                {
                    overflowData += (overflowData.Length == 0 ? String.Empty : this.columnDelimter) + rowData.GetColumnData(i);
                }
                List<string> columnValues = new List<string>();
                for (int i = 0; i < this.bufferService.ColumnCount - 1; i++)
                {
                    columnValues.Add(rowData.GetColumnData(i));
                }
                columnValues.Add(overflowData);
                rowData.ResetColumnData();
                for (int i = 0; i < columnValues.Count; i++)
                {
                    rowData.AddColumnData(columnValues[i]);
                }
            }
        }

        private bool CheckRowOverflow(RowData rowData)
        {
            bool rowHandled = false;
            if (rowData.ColumnCount > this.bufferService.ColumnCount)
            {
                string errorMessage = MessageStrings.RowOverflow(this.currentRowCount, rowData.ColumnCount, this.bufferService.ColumnCount, this.output.Name);
                if (this.bufferService.ErrorOutputUsed)
                {
                    if (this.output.TruncationRowDisposition == DTSRowDisposition.RD_RedirectRow)
                    {
                        this.bufferService.AddErrorRow(-1071611003, 0, errorMessage, string.Empty, rowData.RowText);
                        rowHandled = true;
                    }
                    else if (this.output.TruncationRowDisposition == DTSRowDisposition.RD_IgnoreFailure)
                    {
                        rowHandled = true;
                    }
                }

                if (!rowHandled)
                {
                    throw new BufferSinkException(errorMessage);
                }
            }
            // If the row is handled here it will be ignored in the main output.
            return rowHandled;
        }

        private void AddColumns(RowData rowData)
        {
            for (int i = 0; i < bufferService.ColumnCount; i++)
            {
                if (i < rowData.ColumnCount)
                {
                    string columnData = rowData.GetColumnData(i);
                    if (this.treatEmptyStringAsNull && string.IsNullOrEmpty(columnData))
                    {
                        this.bufferService.SetNull(i);
                    }
                    else
                    {
                        try
                        {
                            this.bufferService.SetColumnData(i, columnData);
                        }
                        catch (Exception ex)
                        {
                            IDTSOutputColumn outputColumn = this.output.OutputColumnCollection[i];
                            if (ex is DoesNotFitBufferException ||
                                ex is OverflowException ||
                                ex is System.Data.SqlTypes.SqlTruncateException)
                            {
                                this.HandleColumnErrorDistribution(outputColumn.TruncationRowDisposition, outputColumn.LineageID, outputColumn.IdentificationString, columnData, rowData.RowText, ex);
                            }
                            else
                            {
                                this.HandleColumnErrorDistribution(outputColumn.ErrorRowDisposition, outputColumn.LineageID, outputColumn.IdentificationString, columnData, rowData.RowText, ex);
                            }
                            // If we get this far, it means the error row is redirected or ignored. Stop the loop.
                            break;
                        }
                    }
                }
                else
                {
                    this.bufferService.SetNull(i);
                }
            }
        }

        private void HandleColumnErrorDistribution(DTSRowDisposition rowDisposition, int columnLineage, string columnIdString, string columnData, string rowData, Exception ex)
        {
            bool rowHandled = false;

            string errorMessage = MessageStrings.FailedToAssignColumnValue(this.currentRowCount, columnData, columnIdString);
            if (this.bufferService.ErrorOutputUsed)
            {
                if (rowDisposition == DTSRowDisposition.RD_RedirectRow)
                {
                    this.bufferService.AddErrorRow(System.Runtime.InteropServices.Marshal.GetHRForException(ex), columnLineage, errorMessage, columnData, rowData);
                    rowHandled = true;
                }
                else if (rowDisposition == DTSRowDisposition.RD_IgnoreFailure)
                {
                    rowHandled = true;
                }
            }

            this.bufferService.RemoveRow();

            if (!rowHandled)
            {
                throw new BufferSinkException(errorMessage, ex);
            }
        }
    }

    internal class BufferSinkException : Exception
    {
        public BufferSinkException(string message)
            : base(message)
        {
        }

        public BufferSinkException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
