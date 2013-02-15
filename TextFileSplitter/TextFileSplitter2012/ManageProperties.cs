using System;
using System.Collections.Generic;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;

namespace Martin.SQLServer.Dts
{
    public class ManageProperties
    {
        public event PostErrorDelegate PostErrorEvent = null;

        // Component Properties
        public const string columnDelimiter = "columnDelimiter";
        public const string isTextDelmited = "isTextDelmited";
        public const string textDelmiter = "textDelmiter";
        public const string treatEmptyStringsAsNull = "treatEmptyStringsAsNull";

        // Output Properties
        public const string typeOfOutput = "typeOfOutput";
        public const string rowTypeValue = "rowTypeValue";
        public const string masterRecordID = "masterRecordID";

        // Output Column Properties
        public const string usageOfColumn = "usageOfColumn";
        public const string keyOutputColumnID = "keyOutputColumnID";
        public const string dotNetFormatString = "dotNetFormatString";
        public const string isColumnOptional = "isColumnOptional";

        // Defaults
        const string DefaultDelimiter = ",";
        const int MaxDelimLength = 20;
        const int MaxRowTypeLength = 128;

        private Dictionary<string, ValidateProperty> propertyValidationTable = new Dictionary<string, ValidateProperty>();

        public ManageProperties()
        {
            this.SetPropertyValidationTable();
        }

        #region Validators
        #region Private
        private void SetPropertyValidationTable()
        {
            this.propertyValidationTable.Add(columnDelimiter, new ValidateProperty(ValidateDelimiterProperty));
            this.propertyValidationTable.Add(isTextDelmited, new ValidateProperty(ValidateBooleanProperty));
            this.propertyValidationTable.Add(textDelmiter, new ValidateProperty(ValidateDelimiterProperty));
            this.propertyValidationTable.Add(treatEmptyStringsAsNull, new ValidateProperty(ValidateBooleanProperty));

            this.propertyValidationTable.Add(typeOfOutput, new ValidateProperty(ValidateTypeOfOutputProperty));
            this.propertyValidationTable.Add(rowTypeValue, new ValidateProperty(ValidateRowTypeProperty));
            this.propertyValidationTable.Add(masterRecordID, new ValidateProperty(ValidateIntegerProperty));

            this.propertyValidationTable.Add(usageOfColumn, new ValidateProperty(ValidateUsageOfColumnProperty));
            this.propertyValidationTable.Add(keyOutputColumnID, new ValidateProperty(ValidateIntegerProperty));
            this.propertyValidationTable.Add(dotNetFormatString, new ValidateProperty(ValidateStringProperty));
            this.propertyValidationTable.Add(isColumnOptional, new ValidateProperty(ValidateBooleanProperty));
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

        private DTSValidationStatus ValidateStringProperty(string propertyName, object propertyValue)
        {
            if (propertyValue is string)
            {
                return DTSValidationStatus.VS_ISVALID;
            }
            else
            {
                this.PostError(MessageStrings.InvalidPropertyValue(propertyName, propertyValue));
                return DTSValidationStatus.VS_ISCORRUPT;
            }
        }

        private DTSValidationStatus ValidatePropertyExists(IDTSCustomPropertyCollection100 customPropertyCollection, string propertyName, DTSValidationStatus oldStatus)
        {

            foreach (IDTSCustomProperty100 property in customPropertyCollection)
            {
                if (property.Name == propertyName)
                {
                    return Utilities.CompareValidationValues(oldStatus, DTSValidationStatus.VS_ISVALID);
                }
            }
            this.PostError(MessageStrings.MissingProperty(propertyName));
            return Utilities.CompareValidationValues(oldStatus, DTSValidationStatus.VS_ISCORRUPT);
        }
        #endregion

        #region Public Validations
        public DTSValidationStatus ValidateProperties(IDTSCustomPropertyCollection100 customPropertyCollection, DTSValidationStatus oldStatus)
        {
            DTSValidationStatus resultStatus = oldStatus;
            foreach (IDTSCustomProperty100 property in customPropertyCollection)
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

        public DTSValidationStatus ValidateComponentProperties(IDTSCustomPropertyCollection100 customPropertyCollection, DTSValidationStatus oldStatus)
        {
            DTSValidationStatus resultStatus = oldStatus;
            resultStatus = ValidatePropertyExists(customPropertyCollection, isTextDelmited, resultStatus);
            resultStatus = ValidatePropertyExists(customPropertyCollection, textDelmiter, resultStatus);
            resultStatus = ValidatePropertyExists(customPropertyCollection, columnDelimiter, resultStatus);
            resultStatus = ValidatePropertyExists(customPropertyCollection, treatEmptyStringsAsNull, resultStatus);
            return resultStatus;
        }


        public DTSValidationStatus ValidateOutputProperties(IDTSCustomPropertyCollection100 customPropertyCollection, DTSValidationStatus oldStatus)
        {
            DTSValidationStatus resultStatus = oldStatus;
            resultStatus = ValidatePropertyExists(customPropertyCollection, rowTypeValue, resultStatus);
            resultStatus = ValidatePropertyExists(customPropertyCollection, typeOfOutput, resultStatus);
            resultStatus = ValidatePropertyExists(customPropertyCollection, masterRecordID, resultStatus);
            return resultStatus;
        }

        public DTSValidationStatus ValidateOutputColumnProperties(IDTSCustomPropertyCollection100 customPropertyCollection, DTSValidationStatus oldStatus)
        {
            DTSValidationStatus resultStatus = oldStatus;
            resultStatus = ValidatePropertyExists(customPropertyCollection, usageOfColumn, resultStatus);
            resultStatus = ValidatePropertyExists(customPropertyCollection, keyOutputColumnID, resultStatus);
            resultStatus = ValidatePropertyExists(customPropertyCollection, dotNetFormatString, resultStatus);
            resultStatus = ValidatePropertyExists(customPropertyCollection, isColumnOptional, resultStatus);
            return resultStatus;
        }

        #endregion
        #endregion

        #region MaintainProperties

        #region Add Properties

        private static void AddCustomProperty(IDTSCustomPropertyCollection100 propertyCollection, string name, string description, object defaultValue)
        {
            AddCustomProperty(propertyCollection, name, description, defaultValue, string.Empty, false);
        }

        private static void AddCustomProperty(IDTSCustomPropertyCollection100 propertyCollection, string name, string description, object defaultValue, Boolean valueContainsID)
        {
            AddCustomProperty(propertyCollection, name, description, defaultValue, string.Empty, valueContainsID);
        }

        private static void AddCustomProperty(IDTSCustomPropertyCollection100 propertyCollection, string name, string description, object defaultValue, string typeConverter)
        {
            AddCustomProperty(propertyCollection, name, description, defaultValue, typeConverter, false);
        }

        private static void AddCustomProperty(IDTSCustomPropertyCollection100 propertyCollection, string name, string description, object defaultValue, string typeConverter, Boolean valueContainsID)
        {
            IDTSCustomProperty100 property = propertyCollection.New();
            property.Name = name;
            property.Description = description;
            property.Value = defaultValue;
            property.ExpressionType = DTSCustomPropertyExpressionType.CPET_NONE; // Don't make this Notify as it causes MAJOR performance issues. (and why would you want them to be expressioned anyway)?
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

        public static void AddComponentProperties(IDTSCustomPropertyCollection100 propertyCollection)
        {
            AddCustomProperty(propertyCollection, isTextDelmited, MessageStrings.IsTextDelmitedPropDescription, true);
            AddCustomProperty(propertyCollection, textDelmiter, MessageStrings.TextDelmiterPropDescription, "\"");
            AddCustomProperty(propertyCollection, columnDelimiter, MessageStrings.ColumnDelimiterPropDescription, ",");
            AddCustomProperty(propertyCollection, treatEmptyStringsAsNull, MessageStrings.TreatEmptyStringsAsNullPropDescription, true);
        }

        public static void AddOutputProperties(IDTSCustomPropertyCollection100 propertyCollection)
        {
            AddCustomProperty(propertyCollection, typeOfOutput, MessageStrings.TypeOfOutputPropDescription, Utilities.typeOfOutputEnum.DataRecords, typeof(Utilities.typeOfOutputEnum).AssemblyQualifiedName);
            AddCustomProperty(propertyCollection, rowTypeValue, MessageStrings.RowTypeValuePropDescription, String.Empty);
            AddCustomProperty(propertyCollection, masterRecordID, MessageStrings.MasterRecordIDPropDescription, -1, true);
        }

        
        public static void AddOutputColumnProperties(IDTSCustomPropertyCollection100 propertyCollection)
        {
            AddCustomProperty(propertyCollection, usageOfColumn, MessageStrings.UsageOfColumnPropDescription, Utilities.usageOfColumnEnum.Passthrough, typeof(Utilities.usageOfColumnEnum).AssemblyQualifiedName);
            AddCustomProperty(propertyCollection, keyOutputColumnID, MessageStrings.KeyOutputColumnIDPropDescription, -1, true);
            AddCustomProperty(propertyCollection, dotNetFormatString, MessageStrings.DotNetFormatStringPropDescription, String.Empty);
            AddCustomProperty(propertyCollection, isColumnOptional, MessageStrings.IsColumnOptionalPropDescription, false);
        }

        public static void AddIgnorableColumnProperties(IDTSCustomPropertyCollection100 propertyCollection)
        {
            AddCustomProperty(propertyCollection, usageOfColumn, MessageStrings.UsageOfColumnPropDescription, Utilities.usageOfColumnEnum.Ignore, typeof(Utilities.usageOfColumnEnum).AssemblyQualifiedName);
        }

        public static void AddMissingOutputColumnProperties(IDTSCustomPropertyCollection100 propertyCollection)
        {
            if (GetPropertyValue(propertyCollection, usageOfColumn) == null)
            {
                AddCustomProperty(propertyCollection, usageOfColumn, MessageStrings.UsageOfColumnPropDescription, Utilities.usageOfColumnEnum.Passthrough, typeof(Utilities.usageOfColumnEnum).AssemblyQualifiedName);
            }
            if (GetPropertyValue(propertyCollection, keyOutputColumnID) == null)
            {
                AddCustomProperty(propertyCollection, keyOutputColumnID, MessageStrings.KeyOutputColumnIDPropDescription, -1, true);
            }
            if (GetPropertyValue(propertyCollection, dotNetFormatString) == null)
            {
                AddCustomProperty(propertyCollection, dotNetFormatString, MessageStrings.DotNetFormatStringPropDescription, String.Empty);
            }
            if (GetPropertyValue(propertyCollection, isColumnOptional) == null)
            {
                AddCustomProperty(propertyCollection, isColumnOptional, MessageStrings.IsColumnOptionalPropDescription, false);
            }
        }

        #endregion

        #region Get Properties

        public static object GetPropertyValue(IDTSCustomPropertyCollection100 propertyCollection, string name)
        {
            for (int i = 0; i < propertyCollection.Count; i++)
            {
                IDTSCustomProperty100 property = propertyCollection[i];
                if (property.Name.Equals(name))
                {
                    return property.Value;
                }
            }

            return null;
        }

        public static object GetPropertyValue(Dictionary<String, SSISProperty> propertyCollection, string name)
        {
            SSISProperty value = null;
            if (propertyCollection.TryGetValue(name, out value))
            {
                return value.Value;
            }
            else
                return null;
        }

        #endregion

        #region Set Properties

        public static Boolean SetPropertyValue(IDTSCustomPropertyCollection100 propertyCollection, string name, object value)
        {
            for (int i = 0; i < propertyCollection.Count; i++)
            {
                IDTSCustomProperty100 property = propertyCollection[i];
                if (property.Name.Equals(name))
                {
                    property.Value = value;
                    return true;
                }
            }

            return false;
        }

        public static Boolean SetContainsLineage(IDTSCustomPropertyCollection100 propertyCollection, string name, Boolean value)
        {
            for (int i = 0; i < propertyCollection.Count; i++)
            {
                IDTSCustomProperty100 property = propertyCollection[i];
                if (property.Name.Equals(name))
                {
                    property.ContainsID = value;
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region Contains

        public static Boolean Contains(IDTSCustomPropertyCollection100 properties,  String propertyName)
        {
            Boolean found = false;

            foreach (IDTSCustomProperty100 property in properties)
            {
                if (String.Compare(property.Name, propertyName, false) == 0)  // Case Sensitive!
                {
                    found = true;
                    break;
                }
            }

            return found;
        }

        public static Boolean Contains(Dictionary<String, SSISProperty> properties, String propertyName)
        {
            return properties.ContainsKey(propertyName);
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

    public delegate void PostErrorDelegate(string errorMessage);
}
