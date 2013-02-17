using System;

namespace Martin.SQLServer.Dts
{
    internal static class MessageStrings
    {
        #region Dynamic Messages
        private const string InvalidPropertyValuePattern2 = "Invalid value \"{1}\" for property {0}.";
        public static string InvalidPropertyValue(string propertyName, object propertyValue)
        {
            return string.Format(InvalidPropertyValuePattern2, propertyName, propertyValue);
        }

        private const string PropertyStringTooLongPattern = "The string \"{1}\" is too long for property {0}.";
        public static string PropertyStringTooLong(string propertyName, string propertyValue)
        {
            return string.Format(PropertyStringTooLongPattern, propertyName, propertyValue);
        }

        private const string PropertyDoesntDecodePattern = "The string \"{1}\" is not valid Base64 text for property {0}.";
        public static string PropertyDoesntDecode(string propertyName, string propertyValue)
        {
            return string.Format(PropertyDoesntDecodePattern, propertyName, propertyValue);
        }

        private const string APropertyIsMissingPattern = "There is a property missing from {0}.";
        public static string APropertyIsMissing(string missingFrom)
        {
            return string.Format(APropertyIsMissingPattern, missingFrom);
        }

        private const string TheOutputTypeIsInvalidPattern = "The output {0} has an invalid type of {1}.";
        public static string InvalidOutputType(string outputName, string outputType)
        {
            return String.Format(TheOutputTypeIsInvalidPattern, outputName, outputType);
        }

        private const string RowOverflowPattern = "Row #{0}: The number of parsed row columns ({1}) is greater than the number ({2}) of defined output columns in output ({3}).";
        public static string RowOverflow(Int64 rowNumber, int rowColumnCount, int outputColumnCount, string outputName)
        {
            return string.Format(RowOverflowPattern, rowNumber, rowColumnCount, outputColumnCount, outputName);
        }

        private const string FailedToAssignColumnValuePattern = "Row #{0}: Failed to assign the following value \"{1}\" to {2}.";
        public static string FailedToAssignColumnValue(Int64 rowNumber, string columnValue, string columnIdentification)
        {
            return string.Format(FailedToAssignColumnValuePattern, rowNumber, columnValue, columnIdentification);
        }

        private const string ParsingBufferOverflowPattern = "Row #{0}, column #{1}: Size of the parsed data exceeded maximum parsing buffer size ({2}).";
        public static string ParsingBufferOverflow(Int64 rowNumber, int columnNumber, int maxBufferSize)
        {
            return string.Format(ParsingBufferOverflowPattern, rowNumber, columnNumber, maxBufferSize);
        }

        private const string MaximumColumnNumberOverflowPattern = "Row #{0}: This row contains more columns than the maximum allowed number ({1}).";
        public static string MaximumColumnNumberOverflow(Int64 rowNumber, int maxNoColumns)
        {
            return string.Format(MaximumColumnNumberOverflowPattern, rowNumber, maxNoColumns);
        }

        private const string FileDoesNotExistPattern = "The following file {0} does not exist.";
        public static string FileDoesNotExist(string fileName)
        {
            return string.Format(FileDoesNotExistPattern, fileName);
        }

        private const string MasterRecordIDInvalidPattern = "The Master Record ID for output {0} is invalid.";
        public static string MasterRecordIDInvalid(string outputName)
        {
            return string.Format(MasterRecordIDInvalidPattern, outputName);
        }

        private const string ChildColumnInvalidPattern = "The Child Column {1} is not valid or is missing in output {0}.";
        public static string ChildColumnInvalid(string outputName, string columnName)
        {
            return string.Format(ChildColumnInvalidPattern, outputName, columnName);
        }

        private const string ErrorOutputHasInvalidColumnPattern = "The Error Output has an Invalid Column {0}.";
        public static string ErrorOutputHasInvalidColumn(string columName)
        {
            return String.Format(ErrorOutputHasInvalidColumnPattern, columName);
        }

        private const string OutputHasDuplicateColumnNamesPattern = "The Output {0} has duplicate column names of {1} which is not allowed.";
        public static string OutputHasDuplicateColumnNames(string outputName, string columnName)
        {
            return String.Format(OutputHasDuplicateColumnNamesPattern, outputName, columnName);
        }

        private const string MasterOutputHasRecordIDPattern = "The Output {0} is defined as Master but has MasterRecordID.";
        public static string MasterOutputHasRecordID(string outputName)
        {
            return String.Format(MasterOutputHasRecordIDPattern, outputName);
        }

        private const string DataOutputHasRecordIDPattern = "The Output {0} is defined as Data but has MasterRecordID.";
        public static string DataOutputHasRecordID(string outputName)
        {
            return String.Format(DataOutputHasRecordIDPattern, outputName);
        }

        private const string MasterColumnHasKeyOutputIDPattern = "The Column {0} is defined as Master on a Master Output {1} but has keyOutputID.";
        public static string MasterColumnHasKeyOutputID(string columnName, string outputName)
        {
            return String.Format(MasterColumnHasKeyOutputIDPattern, columnName, outputName);
        }

        private const string ThereAreChildRecordsForMasterPattern = "The Output {0} has a child.  You can not change it's type.";
        public static string ThereAreChildRecordsForMaster(string outputName)
        {
            return String.Format(ThereAreChildRecordsForMasterPattern, outputName);
        }

        private const string MissingPropertyPattern = "The custom property {0} is missing.";
        public static string MissingProperty(string propertyName)
        {
            return String.Format(MissingPropertyPattern, propertyName);
        }

        private const string CantChangeOutputPropertiesPattern = "You are not allowed to change {0} Output properties.";
        public static string CantChangeOutputProperties(string outputType)
        {
            return String.Format(CantChangeOutputPropertiesPattern, outputType);
        }

        private const string OutputIsSyncronousPattern = "Output {0} is incorrectly defined as Syncronous";
        public static string OutputIsSyncronous(string outputName)
        {
            return String.Format(OutputIsSyncronousPattern, outputName);
        }

        private const string MasterHasChildPattern = "Output {0} has a child {1} which is preventing it being changed.";
        public static string MasterHasChild(string masterOutputName, string childOutputName)
        {
            return String.Format(MasterHasChildPattern, masterOutputName, childOutputName);
        }

        private const string KeyRecordColumnBadValuePattern = "Column {0} in the key output has the property {1} set incorrectly.";
        public static string KeyRecordColumnBadValue(string columnName, string propertyName)
        {
            return String.Format(KeyRecordColumnBadValuePattern, columnName, propertyName);
        }

        private const string AKeyColumnIsMissingFromOutputPattern = "The Key Column is missing from output {0}.";
        public static string AKeyColumnIsMissingFromOutput(string outputName)
        {
            return String.Format(AKeyColumnIsMissingFromOutputPattern, outputName);
        }

        private const string InvalidPropertyValuePattern4 = "You can not set the property {2} on column {1} on output {0} to the value {3}.";
        public static string InvalidPropertyValue(string outputName, string columnName, string propertyName, string propertyValue)
        {
            return String.Format(InvalidPropertyValuePattern4, outputName, columnName, propertyName, propertyValue);
        }

        private const string YouCanNotSetThatPropertyOnAColumnPattern = "You can not set the property {2} on column {1} on output {0}.";
        public static string YouCanNotSetThatPropertyOnAColumn(string outputName, string columnName, string propertyName)
        {
            return String.Format(YouCanNotSetThatPropertyOnAColumnPattern, outputName, columnName, propertyName);
        }

        #endregion

        #region Name Descriptions
        public const string ColumnDelimiterPropDescription = "Stores the column delimiter that is to be used for all columns.";
        public const string IsTextDelmitedPropDescription = "Stores a True/False that text is delimited.";
        public const string TextDelmiterPropDescription = "Stores a string up to 20 characters long that is the text delimiter";
        public const string KeyRecordFormatPropDescription = "Stores an encoded XML String that has the details of the Key Record";
        public const string TypeOfOutputPropDescription = "Stores the type of output that this is.";
        public const string RowTypeValuePropDescription = "Stores the key string value that indicates that the source record should go to this output.\r\nIt MUST match a value from the column flaged with splitFieldType = Row Type.";
        public const string SplitFieldTypePropDescription = "Stores the type of column that this is.  Row Type indicates that this column switches the output the data must go down...";
        public const string UsageOfColumnPropDescription = "Stores what to do with this column.  Key indicates that it will be propogated to ALL data records IF this is a KeyRecords output.";
        public const string KeyOutputColumnIDPropDescription = "Stores the key column ID so that activity on it will be synced to this column.\r\nDO NOT EDIT MANUALLY!!!";
        public const string FileConnectionDescription = "This is the Text File connection, which should be a standard Text File, with 2 columns defined!";
        public const string InputKeyColumnIDPropDescription = "Stores the Lineage ID of the Key column";
        public const string InputDataColumnIDPropDescription = "Stores the Lineage ID of the Data column";
        public const string TreatEmptyStringsAsNullPropDescription = "If a string is empty then return it as a Null?";
        public const string MasterRecordIDPropDescription = "Stores the Lineage ID of the output that contains the Master Record for this Child Record.";
        public const string DotNetFormatStringPropDescription = "Stores the Dot Net Format String to describe this data conversion from String.  eg. yyyy/MM/dd";
        public const string KeyValueColumnDescription = "The key value that was used to determine what output this row went to.";
        public const string NumberOfRowsColumnDescription = "The number of rows that were sent down this associated output.";
        public const string KeyValueStatusColumnDescription = "The status that this output held when being processed.";
        public const string IsColumnOptionalPropDescription = "Stores a True/False that shows if this column is optional within the delimited text.";
        public const string KeyRecordKeyColumnDescription = "The GUID value that uniquely identifies this Key record.";
        #endregion

        #region Key Value Status Values
        public const string ConnectedAndProcessed = "Connected and Processed";
        public const string Disconnected = "Disconnected";
        public const string NotRecognised = "Not configured";
        #endregion

        #region Input/Output Names
        public const string KeyRecordOutputName = "TextFileSplitter KeyRecords Output";
        public const string PassthroughOutputName = "TextFileSplitter PassThrough Output";
        public const string ErrorOutputName = "TextFileSplitter Error Output";
        public const string RowCountOutputName = "TextFileSplitter RowCount Output";
        public const string FileConnectionName = "FileConnection";
        public const string KeyValueColumnName = "KeyValue";
        public const string NumberOfRowsColumnName = "NumberOfRows";
        public const string KeyValueStatusColumnName = "KeyValueStatus";
        public const string ErrorMessageColumnName = "ErrorMessage";
        public const string ColumnDataColumnName = "ColumnData";
        public const string RowDataColumnName = "RowData";
        public const string KeyRecordKeyColumnName = "TextFileSplitter KeyRecord KeyColumn";
        #endregion

        #region Messages
        public const string RowLevelTruncationOperation = "Error or truncation whilst parsing row values.";
        public const string RowLevelErrorOperation = "Error whilst parsing row values.";
        public const string ColumnLevelErrorTruncationOperation = "Error or truncation while assigning column values.";
        public const string NotExpectedInputs = "The number of inputs is NOT one.";
        public const string UnexpectedNumberOfOutputs = "The number of outputs is less than two.";
        public const string NoOutputColumns = "No output columns have been defined on a regular output.";
        public const string ErrorOutputColumnsAreMissing = "One or More of the required columns on the error output have been removed.";
        public const string NoErrorOutput = "The Error Output is MISSING!";
        public const string MissingInputProperties = "At least one input property is missing.";
        public const string CantAddInput = "You are not allowed to add another input.";
        public const string ReadWriteNotSupported = "This component does not support READWRITE.";
        public const string KeyColumnDescription = "This is a propogated name because it was chosen as a key name";
        public const string CannotSetPropertyToKey = "You can only set Key on the Key Output";
        public const string CannotSetPropertyToMasterValue = "You can only set MasterValue on a Master or ChildMaster Output";
        public const string CannotSetPropertyToRowData = "You can only set RowData on the PassThrough Output";
        public const string CannotSetPropertyToRowType = "You can only set RowType on the PassThrough Output";
        public const string CannotSetPropertyToPassThrough = "You can't set PassThrough on this Output";
        public const string CannotSetPropertyToIgnore = "You can't set Ignore on this Output";
        public const string CannotSetProperty = "You are not allowed to change this property.";
        public const string CannotDeleteKeyOutput = "You can NOT delete the Key Output!";
        public const string CannotDeleteErrorOutput = "You can NOT delete the error output!";
        public const string CannotDeletePassThroughOutput = "You can NOT delete the PassThrough output!";
        public const string CannotDeleteRowsProcessedOutput = "You can NOT delete the Rows Processed output!";
        public const string OnlyStringDataTypesSupported = "You can only use String data types with this component";
        public const string BadParsingGraphError = "WTF...  We have a Bad Parsing state from parsing the string!";
        public const string MustBeFlatFileConnection = "The connection manager MUST be a Flat File connection mamager";
        public const string InvalidPassThoughOutput = "There MUST be One and Only One PassThrough output!";
        public const string ExternalMetaDataOutOfSync = "The External Metadata is out of sync.";
        public const string ConnectionManagerNotSet = "The Connection Manager Needs to be Setup.";
        public const string InvalidPassKeyOutput = "There MUST be One and Only One KeyRecord output!";
        public const string ThereCanOnlyBeOneRowTypeColumn = "There can only be one RowType column in the PassThrough output.";
        public const string CannotSetMasterRecordID = "The Master Record ID can only be set on ChildRecords or ChildMasterRecords.";
        public const string ThereCanOnlyBeOneRowDataColumn = "There can only be one RowData column in the PassThrough output.";
        public const string InvalidNumberOfRowsOutput = "There MUST be One and Only One RowsProcessed output!";
        public const string RowCountOutputInvalid = "The RowsProcessed Output is invalid!";
        public const string KeyOutputColumnsAreMissing = "The Key Output GUID Column is missing.";
        public const string KeyRecordIncorrectNumberOfKeyColumns = "There is either more than one, or less than one Key Output GUID column.";
        public const string CantDeleteKeyColumn = "You can NOT delete the Key column.";
        public const string CantSetKeyColumn = "You can NOT set any properties on a Key column.";
        public const string CantUnSetKeyColumn = "You can NOT change a Key column to anything else.";
        public const string CanOnlySetOptionalOnLastNonOptionalColumn = "You MUST ensure that all the columns after the one you are changing are already optional.";
        public const string CanOnlySetNonOptionalOnLastOptionalColumn = "You MUST ensure that all the columns before the one you are changing are NON optional.";
        #endregion

        #region Validation Strings
        const string UnsupportedDataTypePattern = "The type {0} is not supported.";
        public static string UnsupportedDataType(string dataTypeName)
        {
            return string.Format(UnsupportedDataTypePattern, dataTypeName);
        }
        #endregion

    }
}
