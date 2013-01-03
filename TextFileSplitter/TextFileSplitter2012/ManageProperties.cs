using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using System.Xml;
using System.IO;

#if SQL2012
    using IDTSCustomProperty = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSCustomProperty100;
    using IDTSCustomPropertyCollection = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSCustomPropertyCollection100;
#endif
#if SQL2008
    using IDTSCustomProperty = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSCustomProperty100;
    using IDTSCustomPropertyCollection = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSCustomPropertyCollection100;
#endif
#if SQL2005
    using IDTSCustomProperty = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSCustomProperty90;
    using IDTSCustomPropertyCollection = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSCustomPropertyCollection90;
#endif


namespace Martin.SQLServer.Dts
{
    internal class ManageProperties
    {
        public event PostErrorDelegate PostErrorEvent = null;

        // Component Properties
        internal const string inputKeyColumnID = "inputKeyColumnID";
        internal const string inputDataColumnID = "inputDataColumnID";
        internal const string columnDelimiter = "columnDelimiter";
        internal const string isTextDelmited = "isTextDelmited";
        internal const string textDelmiter = "textDelmiter";

        // Input Column Properties
        internal const string splitFieldType = "splitFieldType";

        // Output Properties
        internal const string typeOfOutput = "typeOfOutput";
        internal const string rowTypeValue = "rowTypeValue";

        // Output Column Properties
        internal const string usageOfColumn = "usageOfColumn";
        internal const string keyOutputColumnID = "keyOutputColumnID";

        // Defaults
        const string DefaultDelimiter = ",";
        const int MaxDelimLength = 20;
        const int MaxRowTypeLength = 20;

        private Dictionary<string, ValidateProperty> propertyValidationTable = new Dictionary<string, ValidateProperty>();

        public ManageProperties()
        {
            this.SetPropertyValidationTable();
        }

        #region Validators
        #region Private
        private void SetPropertyValidationTable()
        {
            this.propertyValidationTable.Add(isTextDelmited, new ValidateProperty(ValidateBooleanProperty));
            this.propertyValidationTable.Add(typeOfOutput, new ValidateProperty(ValidateTypeOfOutputProperty));
            
            this.propertyValidationTable.Add(columnDelimiter, new ValidateProperty(ValidateDelimiterProperty));
            this.propertyValidationTable.Add(textDelmiter, new ValidateProperty(ValidateDelimiterProperty));

            this.propertyValidationTable.Add(splitFieldType, new ValidateProperty(ValidateSplitEnumProperty));
            this.propertyValidationTable.Add(rowTypeValue, new ValidateProperty(ValidateRowTypeProperty));

            this.propertyValidationTable.Add(usageOfColumn, new ValidateProperty(ValidateUsageOfColumnProperty));
            this.propertyValidationTable.Add(keyOutputColumnID, new ValidateProperty(ValidateIntegerProperty));

            this.propertyValidationTable.Add(inputKeyColumnID, new ValidateProperty(ValidateIntegerProperty));
            this.propertyValidationTable.Add(inputDataColumnID, new ValidateProperty(ValidateIntegerProperty));
        }

        private DTSValidationStatus ValidateRowTypeProperty(string propertyName, object propertyValue)
        {
            if (propertyValue is string)
            {
                string value = (string)propertyValue;
                if (value.Length <= MaxRowTypeLength)
                {
                    return DTSValidationStatus.VS_ISVALID;
                }
                else
                {
                    this.PostError(MessageStrings.PropertyStringTooLong(propertyName, propertyValue.ToString()));
                    return DTSValidationStatus.VS_ISBROKEN;
                }
            }
            else
            {
                this.PostError(MessageStrings.InvalidPropertyValue(propertyName, propertyValue));
                return DTSValidationStatus.VS_ISCORRUPT;
            }
        }

        private DTSValidationStatus ValidateSplitEnumProperty(string propertyName, object propertyValue)
        {
            if (Enum.IsDefined(typeof(Utilities.splitFieldTypeEnum), propertyValue))
            {
                return DTSValidationStatus.VS_ISVALID;
            }
            else
            {
                this.PostError(MessageStrings.InvalidPropertyValue(propertyName, propertyValue));
                return DTSValidationStatus.VS_ISCORRUPT;
            }
        }

        private DTSValidationStatus ValidateTypeOfOutputProperty(string propertyName, object propertyValue)
        {
            if (Enum.IsDefined(typeof(Utilities.typeOfOutputEnum), propertyValue))
            {
                return DTSValidationStatus.VS_ISVALID;
            }
            else
            {
                this.PostError(MessageStrings.InvalidPropertyValue(propertyName, propertyValue));
                return DTSValidationStatus.VS_ISCORRUPT;
            }
        }


        private DTSValidationStatus ValidateUsageOfColumnProperty(string propertyName, object propertyValue)
        {
            if (Enum.IsDefined(typeof(Utilities.usageOfColumnEnum), propertyValue))
            {
                return DTSValidationStatus.VS_ISVALID;
            }
            else
            {
                this.PostError(MessageStrings.InvalidPropertyValue(propertyName, propertyValue));
                return DTSValidationStatus.VS_ISCORRUPT;
            }
        }

        // No Longer Used
        /*
        private DTSValidationStatus ValidateKeyRecordProperty(string propertyName, object propertyValue)
        {
            if (propertyValue is string)
            {
                string value = (string)propertyValue;
                try
                {
                    String xmlData = Utilities.XmlDecodeFromString(value);
                    KeyRecordFormat keyRecords = (KeyRecordFormat) Utilities.XmlDeserializeFromString(xmlData, typeof(KeyRecordFormat));

                    return DTSValidationStatus.VS_ISVALID;
                }
                catch
                {
                    this.PostError(MessageStrings.PropertyDoesntDecode(propertyName, propertyValue.ToString()));
                    return DTSValidationStatus.VS_ISBROKEN;
                }
            }
            else
            {
                this.PostError(MessageStrings.InvalidPropertyValue(propertyName, propertyValue));
                return DTSValidationStatus.VS_ISCORRUPT;
            }
        }
        */

        private DTSValidationStatus ValidateBooleanProperty(string propertyName, object propertyValue)
        {
            if (propertyValue is bool)
            {
                return DTSValidationStatus.VS_ISVALID;
            }
            else
            {
                this.PostError(MessageStrings.InvalidPropertyValue(propertyName, propertyValue));
                return DTSValidationStatus.VS_ISCORRUPT;
            }
        }

        private DTSValidationStatus ValidateIntegerProperty(string propertyName, object propertyValue)
        {
            if (propertyValue is int)
            {
                return DTSValidationStatus.VS_ISVALID;
            }
            else
            {
                this.PostError(MessageStrings.InvalidPropertyValue(propertyName, propertyValue));
                return DTSValidationStatus.VS_ISCORRUPT;
            }
        }
        
        private DTSValidationStatus ValidateDelimiterProperty(string propertyName, object propertyValue)
        {
            if (propertyValue is string)
            {
                string value = (string)propertyValue;
                if (value.Length <= MaxDelimLength)
                {
                    return DTSValidationStatus.VS_ISVALID;
                }
                else
                {
                    this.PostError(MessageStrings.PropertyStringTooLong(propertyName, propertyValue.ToString()));
                    return DTSValidationStatus.VS_ISBROKEN;
                }
            }
            else
            {
                this.PostError(MessageStrings.InvalidPropertyValue(propertyName, propertyValue));
                return DTSValidationStatus.VS_ISCORRUPT;
            }
        }

        private static DTSValidationStatus ValidatePropertyExists(IDTSCustomPropertyCollection customPropertyCollection, string propertyName, DTSValidationStatus oldStatus)
        {

            foreach (IDTSCustomProperty property in customPropertyCollection)
            {
                if (property.Name == propertyName)
                {
                    return Utilities.CompareValidationValues(oldStatus, DTSValidationStatus.VS_ISVALID);
                }
            }
            return Utilities.CompareValidationValues(oldStatus, DTSValidationStatus.VS_ISCORRUPT);
        }
        #endregion

        #region Public Validations
        public DTSValidationStatus ValidateProperties(IDTSCustomPropertyCollection customPropertyCollection, DTSValidationStatus oldStatus)
        {
            DTSValidationStatus resultStatus = oldStatus;
            foreach (IDTSCustomProperty property in customPropertyCollection)
            {
                resultStatus = ValidatePropertyValue(property.Name, property.Value, resultStatus);
            }

            return resultStatus;
        }

        public DTSValidationStatus ValidatePropertyValue(string propertyName, object propertyValue, DTSValidationStatus oldStatus)
        {
            DTSValidationStatus resultStatus = oldStatus;
            if (this.propertyValidationTable.ContainsKey(propertyName))
            {
                resultStatus = Utilities.CompareValidationValues(resultStatus, this.propertyValidationTable[propertyName](propertyName, propertyValue));
            }
            return resultStatus;
        }

        public static DTSValidationStatus ValidateComponentProperties(IDTSCustomPropertyCollection customPropertyCollection, DTSValidationStatus oldStatus)
        {
            DTSValidationStatus resultStatus = oldStatus;
            resultStatus = ValidatePropertyExists(customPropertyCollection, isTextDelmited, resultStatus);
            resultStatus = ValidatePropertyExists(customPropertyCollection, textDelmiter, resultStatus);
            resultStatus = ValidatePropertyExists(customPropertyCollection, columnDelimiter, resultStatus);
            resultStatus = ValidatePropertyExists(customPropertyCollection, inputKeyColumnID, resultStatus);
            resultStatus = ValidatePropertyExists(customPropertyCollection, inputDataColumnID, resultStatus);
            return resultStatus;
        }


        public static DTSValidationStatus ValidateInputColumnProperties(IDTSCustomPropertyCollection customPropertyCollection, DTSValidationStatus oldStatus)
        {
            DTSValidationStatus resultStatus = oldStatus;
            resultStatus = ValidatePropertyExists(customPropertyCollection, splitFieldType, resultStatus);
            return resultStatus;
        }

        public static DTSValidationStatus ValidateOutputProperties(IDTSCustomPropertyCollection customPropertyCollection, DTSValidationStatus oldStatus)
        {
            DTSValidationStatus resultStatus = oldStatus;
            resultStatus = ValidatePropertyExists(customPropertyCollection, rowTypeValue, resultStatus);
            resultStatus = ValidatePropertyExists(customPropertyCollection, typeOfOutput, resultStatus);
            return resultStatus;
        }

        public static DTSValidationStatus ValidateOutputColumnProperties(IDTSCustomPropertyCollection customPropertyCollection, DTSValidationStatus oldStatus)
        {
            DTSValidationStatus resultStatus = oldStatus;
            resultStatus = ValidatePropertyExists(customPropertyCollection, usageOfColumn, resultStatus);
            resultStatus = ValidatePropertyExists(customPropertyCollection, keyOutputColumnID, resultStatus);
            return resultStatus;
        }

        #endregion
        #endregion

        #region MaintainProperties

        #region Add Properties

        private static void AddCustomProperty(IDTSCustomPropertyCollection propertyCollection, string name, string description, object defaultValue)
        {
            AddCustomProperty(propertyCollection, name, description, defaultValue, string.Empty, false);
        }

        // Uncomment the following if you ever need it.
        // It's not covered by Unit tests at this point, so it's commented out.
        //private static void AddCustomProperty(IDTSCustomPropertyCollection propertyCollection, string name, string description, object defaultValue, Boolean valueContainsID)
        //{
        //    AddCustomProperty(propertyCollection, name, description, defaultValue, string.Empty, valueContainsID);
        //}

        private static void AddCustomProperty(IDTSCustomPropertyCollection propertyCollection, string name, string description, object defaultValue, string typeConverter)
        {
            AddCustomProperty(propertyCollection, name, description, defaultValue, typeConverter, false);
        }

        private static void AddCustomProperty(IDTSCustomPropertyCollection propertyCollection, string name, string description, object defaultValue, string typeConverter, Boolean valueContainsID)
        {
            IDTSCustomProperty property = propertyCollection.New();
            property.Name = name;
            property.Description = description;
            property.Value = defaultValue;
            property.ExpressionType = DTSCustomPropertyExpressionType.CPET_NOTIFY;
            property.ContainsID = valueContainsID;
            if (defaultValue is string)
            {
                property.State = DTSPersistState.PS_PERSISTASHEX;
            }
            if (!string.IsNullOrEmpty(typeConverter))
            {
                property.TypeConverter = typeConverter;
            }
        }
        #endregion

        #region Add To SSIS Stuff

        public static void AddComponentProperties(IDTSCustomPropertyCollection propertyCollection)
        {
            AddCustomProperty(propertyCollection, isTextDelmited, MessageStrings.IsTextDelmitedPropDescription, true);
            AddCustomProperty(propertyCollection, textDelmiter, MessageStrings.TextDelmiterPropDescription, "\"");
            AddCustomProperty(propertyCollection, columnDelimiter, MessageStrings.ColumnDelimiterPropDescription, ",");
            AddCustomProperty(propertyCollection, inputKeyColumnID, MessageStrings.InputKeyColumnIDPropDescription, -1);
            AddCustomProperty(propertyCollection, inputDataColumnID, MessageStrings.InputDataColumnIDPropDescription, -1);
        }

        public static void AddInputColumnProperties(IDTSCustomPropertyCollection propertyCollection)
        {
            AddCustomProperty(propertyCollection, splitFieldType, MessageStrings.SplitFieldTypePropDescription, Utilities.splitFieldTypeEnum.Ignore, typeof(Utilities.splitFieldTypeEnum).AssemblyQualifiedName);
        }

        public static void AddOutputProperties(IDTSCustomPropertyCollection propertyCollection)
        {
            AddCustomProperty(propertyCollection, typeOfOutput, MessageStrings.TypeOfOutputPropDescription, Utilities.typeOfOutputEnum.DataRecords, typeof(Utilities.typeOfOutputEnum).AssemblyQualifiedName);
            AddCustomProperty(propertyCollection, rowTypeValue, MessageStrings.RowTypeValuePropDescription, String.Empty);
        }

        
        public static void AddOutputColumnProperties(IDTSCustomPropertyCollection propertyCollection)
        {
            AddCustomProperty(propertyCollection, usageOfColumn, MessageStrings.UsageOfColumnPropDescription, Utilities.usageOfColumnEnum.Passthrough, typeof(Utilities.usageOfColumnEnum).AssemblyQualifiedName);
            AddCustomProperty(propertyCollection, keyOutputColumnID, MessageStrings.KeyOutputColumnIDPropDescription, -1);
        }

        #endregion

        #region Get Properties

        public static object GetPropertyValue(IDTSCustomPropertyCollection propertyCollection, string name)
        {
            for (int i = 0; i < propertyCollection.Count; i++)
            {
                IDTSCustomProperty property = propertyCollection[i];
                if (property.Name.Equals(name))
                {
                    return property.Value;
                }
            }

            return null;
        }

        #endregion

        #region Set Properties

        public static Boolean SetPropertyValue(IDTSCustomPropertyCollection propertyCollection, string name, object value)
        {
            for (int i = 0; i < propertyCollection.Count; i++)
            {
                IDTSCustomProperty property = propertyCollection[i];
                if (property.Name.Equals(name))
                {
                    property.Value = value;
                    return true;
                }
            }

            return false;
        }

        public static Boolean SetContainsLineage(IDTSCustomPropertyCollection propertyCollection, string name, Boolean value)
        {
            for (int i = 0; i < propertyCollection.Count; i++)
            {
                IDTSCustomProperty property = propertyCollection[i];
                if (property.Name.Equals(name))
                {
                    property.ContainsID = value;
                    return true;
                }
            }

            return false;
        }
        #endregion

        #endregion

        private void PostError(string errorMessage)
        {
            if (this.PostErrorEvent != null)
            {
                this.PostErrorEvent(errorMessage);
            }
        }


        delegate DTSValidationStatus ValidateProperty(string propertyName, object propertyValue);

    }

    internal delegate void PostErrorDelegate(string errorMessage);
}
