using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Martin.SQLServer.Dts
{
    internal class StringParser
    {
        public const int MaxColumnNumber = 1024;

        FieldParser fieldParser = null;
        bool singleColumn = false;

        public StringParser(string columnDelimiter, string qualifier)
        {
            ArgumentVerifier.CheckStringArgument(columnDelimiter, "columnDelimiter");
            if (string.IsNullOrEmpty(qualifier))
            {
                this.fieldParser = FieldParser.BuildParserWithSingleDelimiter(columnDelimiter);
            }
            else
            {
                this.fieldParser = FieldParser.BuildParserWithSingleDelimiterAndQualifier(columnDelimiter, qualifier);
            }
        }

        public void ParseRow(StringAsRowReader reader, StringData rowData)
        {
            ArgumentVerifier.CheckObjectArgument(reader, "reader");

            if (rowData != null)
            {
                rowData.ResetRowData();
            }
            this.fieldParser.ResetParsingState();

            if (this.singleColumn)
            {
                fieldParser.ParseNext(reader, rowData);
                if (rowData != null)
                {
                    string columnData = fieldParser.CurrentText;
                    if (!reader.IsEOF || !string.IsNullOrEmpty(columnData))
                    {
                        rowData.AddColumnData(fieldParser.CurrentText);
                    }
                }
            }
            else
            {
                while (!reader.IsEOF && !this.fieldParser.RowDelimiterMatch)
                {
                    this.fieldParser.ParseNext(reader, rowData);

                    if (rowData != null)
                    {
                        string columnData = fieldParser.CurrentText;
                        if (!reader.IsEOF || rowData.ColumnCount > 0 || !string.IsNullOrEmpty(columnData))
                        {
                            if (MaxColumnNumber == rowData.ColumnCount)
                            {
                                throw new RowColumnNumberOverflow();
                            }
                            // Add data if this is not the last and empty row.
                            rowData.AddColumnData(fieldParser.CurrentText);
                        }
                    }
                }
            }
        }
    }
}
