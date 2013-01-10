using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Martin.SQLServer.Dts
{
    internal static class MessageStrings
    {
        #region Name Strings
        const string InvalidPropertyValuePattern = "Invalid value \"{1}\" for property {0}.";
        public static string InvalidPropertyValue(string propertyName, object propertyValue)
        {
            return string.Format(InvalidPropertyValuePattern, propertyName, propertyValue);
        }

        const string PropertyStringTooLongPattern = "The string \"{1}\" is too long for property {0}.";
        public static string PropertyStringTooLong(string propertyName, string propertyValue)
        {
            return string.Format(PropertyStringTooLongPattern, propertyName, propertyValue);
        }

        const string PropertyDoesntDecodePattern = "The string \"{1}\" is not valid Base64 text for property {0}.";
        public static string PropertyDoesntDecode(string propertyName, string propertyValue)
        {
            return string.Format(PropertyDoesntDecodePattern, propertyName, propertyValue);
        }

        const string APropertyIsMissingPattern = "There is a property missing from {0}.";
        public static string APropertyIsMissing(string missingFrom)
        {
            return string.Format(APropertyIsMissingPattern, missingFrom);
        }

        const string TheOutputTypeIsInvalidPattern = "The output {0} has an invalid type of {1}.";
        public static string InvalidOutputType(string outputName, string outputType)
        {
            return String.Format(TheOutputTypeIsInvalidPattern, outputName, outputType);
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
        #endregion

        #region Input/Output Names
        public const string KeyRecordOutputName = "TestFileSplitter KeyRecords Output";
        public const string PassthroughOutputName = "TestFileSplitter PassThrough Output";
        public const string ErrorOutputName = "TestFileSplitter Error Output";
        public const string FileConnectionName = "FileConnection";
        #endregion

        #region Messages
        public const string RowLevelTruncationOperation = "Truncation - too many columns in a parsed row.";
        public const string ColumnLevelErrorTruncationOperation = "Error or truncation while assigning column values.";
        public const string NotExpectedInputs = "The number of inputs is NOT one.";
        public const string UnexpectedNumberOfOutputs = "The number of outputs is less than two.";
        public const string NoOutputColumns = "No output columns have been defined on a regular output.";
        public const string ErrorOutputColumnsAreMissing = "One or More of the required columns on the error output have been removed.";
        public const string NoErrorOutput = "The Error Output is MISSING!";
        public const string MissingInputProperties = "At least one input property is missing.";
        public const string CantChangeErrorOutputProperties = "You are not allowed to change Error Output properties.";
        public const string CantAddInput = "You are not allowed to add another input.";
        public const string ReadWriteNotSupported = "This component does not support READWRITE.";
        public const string KeyColumnDescription = "This is a propogated name because it was chosen as a key name";
        public const string CannotSetPropertyToKey = "You can only set Key on the Key Output";
        public const string CannotSetProperty = "You are not allowed to change this property.";
        public const string CannotDeleteKeyOutput = "You can NOT delete the Key Output!";
        public const string CannotDeleteErrorOutput = "You can NOT delete the error output!";
        public const string CannotDeletePassThroughOutput = "You can NOT delete the PassThrough output!";
        public const string OutputIsSyncronous = "At least one output is incorrectly defined as Syncronous";
        public const string OnlyStringDataTypesSupported = "You can only use String data types with this component";
        public const string BadParsingGraphError = "WTF...  We have a Bad Parsing state from parsing the string!";
        public const string MustBeFlatFileConnection = "The connection manager MUST be a Flat File connection mamager";
        public const string InvalidPassThoughOutput = "There MUST be One and Only One PassThrough output!";
        public const string ExternalMetaDataOutOfSync = "The External Metadata is out of sync.";
        public const string ConnectionManagerNotSet = "The Connection Manager Needs to be Setup.";
        public const string InvalidPassKeyOutput = "There MUST be One and Only One KeyRecord output!";
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
