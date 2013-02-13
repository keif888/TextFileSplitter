#region Usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Dts.Pipeline;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime.Wrapper;
using System.Runtime.InteropServices;
using Microsoft.SqlServer.Dts.Runtime;
using FileHelpers.Dynamic;
using FileHelpers;

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

#endregion


namespace Martin.SQLServer.Dts
{
    [DtsPipelineComponent(DisplayName = "Text File Splitter Source",
        CurrentVersion = 3 // NB. Keep this in sync with ProvideCustomProperties and PerformUpgrade.
        , Description = "Extract many outputs from a single Text File"
        , IconResource = "Martin.SQLServer.Dts.Resources.TextFileSplitter.ico"
#if SQL2012
       , UITypeName = "Martin.SQLServer.Dts.TextFileSplitterForm, TextFileSplitter2012, Version=1.0.0.0, Culture=neutral, PublicKeyToken=cc8ffdd352b00674"
#endif
#if SQL2008
       , UITypeName = "Martin.SQLServer.Dts.TextFileSplitterForm, TextFileSplitter2008, Version=1.0.0.0, Culture=neutral, PublicKeyToken=cc8ffdd352b00674"
#endif
        , ComponentType = ComponentType.SourceAdapter)]
    public class TextFileSplitter : PipelineComponent
    {

        #region Globals
        ManageProperties propertyManager = new ManageProperties();
        const int E_FAIL = unchecked((int)0x80004005);
        private String fileName = String.Empty;
        private int codePage = 1252;
        private string columnDelimter = string.Empty;
        object ffConnection = null;

        public TextFileSplitter()
        {
            this.propertyManager.PostErrorEvent += new PostErrorDelegate(this.PostError);
        }

        #endregion

        #region Design Time

        #region Provide Component Poperties

        public override void ProvideComponentProperties()
        {
            this.RemoveAllInputsOutputsAndCustomProperties();
            this.ComponentMetaData.Version = 3;  // NB.  Always keep this in sync with the CurrentVersion!!!
            this.ComponentMetaData.UsesDispositions = true;
            this.ComponentMetaData.ContactInfo = "http://TextFileSplitter.codeplex.com/";
            ManageProperties.AddComponentProperties(this.ComponentMetaData.CustomPropertyCollection);

            // PassThrough Record Output
            IDTSOutput output = this.ComponentMetaData.OutputCollection.New();
            output.Name = MessageStrings.PassthroughOutputName;
            output.ErrorRowDisposition = DTSRowDisposition.RD_FailComponent;
            output.ErrorOrTruncationOperation = MessageStrings.RowLevelErrorOperation;
            output.SynchronousInputID = 0;
            ManageProperties.AddOutputProperties(output.CustomPropertyCollection);
            ManageProperties.SetPropertyValue(output.CustomPropertyCollection, ManageProperties.typeOfOutput, Utilities.typeOfOutputEnum.PassThrough);

            // Error Regular output.
            IDTSOutput errorOutput = this.ComponentMetaData.OutputCollection.New();
            errorOutput.IsErrorOut = true;
            errorOutput.Name = MessageStrings.ErrorOutputName;
            ManageProperties.AddOutputProperties(errorOutput.CustomPropertyCollection);
            ManageProperties.SetPropertyValue(errorOutput.CustomPropertyCollection, ManageProperties.typeOfOutput, Utilities.typeOfOutputEnum.ErrorRecords);
            ManageColumns.AddErrorOutputColumns(errorOutput);

            // Key Records output.
            IDTSOutput keyRecords = this.ComponentMetaData.OutputCollection.New();
            keyRecords.Name = MessageStrings.KeyRecordOutputName;
            keyRecords.ErrorRowDisposition = DTSRowDisposition.RD_FailComponent;
            keyRecords.ErrorOrTruncationOperation = MessageStrings.RowLevelErrorOperation;
            keyRecords.SynchronousInputID = 0;
            ManageProperties.AddOutputProperties(keyRecords.CustomPropertyCollection);
            ManageProperties.SetPropertyValue(keyRecords.CustomPropertyCollection, ManageProperties.typeOfOutput, Utilities.typeOfOutputEnum.KeyRecords);

            // Number of Rows output.
            IDTSOutput numberOfRows = this.ComponentMetaData.OutputCollection.New();
            numberOfRows.Name = MessageStrings.RowCountOutputName;
            numberOfRows.SynchronousInputID = 0;
            ManageProperties.AddOutputProperties(numberOfRows.CustomPropertyCollection);
            ManageProperties.SetPropertyValue(numberOfRows.CustomPropertyCollection, ManageProperties.typeOfOutput, Utilities.typeOfOutputEnum.RowsProcessed);
            ManageColumns.AddNumberOfRowsOutputColumns(numberOfRows);

            // Reserve space for the file connection.
            IDTSRuntimeConnection connectionSlot = this.ComponentMetaData.RuntimeConnectionCollection.New();
            connectionSlot.Name = MessageStrings.FileConnectionName;
            connectionSlot.Description = MessageStrings.FileConnectionDescription;
        }

        #endregion

        #region Perform Upgrade

        public override void PerformUpgrade(int pipelineVersion)
        {
            // Get the attributes for the executable
            DtsPipelineComponentAttribute componentAttribute = (DtsPipelineComponentAttribute)Attribute.GetCustomAttribute(this.GetType(), typeof(DtsPipelineComponentAttribute), false);
            int binaryVersion = componentAttribute.CurrentVersion;

            // Get the attributes for the SSIS Package
            int metadataVersion = ComponentMetaData.Version;

            if (binaryVersion > metadataVersion)
            {
                // Version 2 added a new Column property of isColumnOptional
                bool isColumnOptionalMissing = false; //(ManageProperties.GetPropertyValue(this.ComponentMetaData.OutputCollection[0].OutputColumnCollection[0].CustomPropertyCollection, ManageProperties.isColumnOptional) == null);
                bool isPropertyExpressionNotify = false;  // Version 3 removed Notify on Expressions

                foreach (IDTSOutput100 output in this.ComponentMetaData.OutputCollection)
                {
                    if ((Utilities.typeOfOutputEnum)ManageProperties.GetPropertyValue(output.CustomPropertyCollection, ManageProperties.typeOfOutput) == Utilities.typeOfOutputEnum.KeyRecords)
                    {
                        foreach (IDTSOutputColumn100 outputColumn in output.OutputColumnCollection)
                        {
                            isColumnOptionalMissing = (ManageProperties.GetPropertyValue(outputColumn.CustomPropertyCollection, ManageProperties.isColumnOptional) == null);
                            isPropertyExpressionNotify = (outputColumn.CustomPropertyCollection[0].ExpressionType == DTSCustomPropertyExpressionType.CPET_NOTIFY);
                            break;
                        }
                    }
                }

                if (isColumnOptionalMissing)
                {
                    foreach (IDTSOutput100 output in this.ComponentMetaData.OutputCollection)
                    {
                        switch ((Utilities.typeOfOutputEnum)ManageProperties.GetPropertyValue(output.CustomPropertyCollection, ManageProperties.typeOfOutput))
                        {
                            case Utilities.typeOfOutputEnum.KeyRecords:
                            case Utilities.typeOfOutputEnum.DataRecords:
                            case Utilities.typeOfOutputEnum.PassThrough:
                            case Utilities.typeOfOutputEnum.MasterRecord:
                            case Utilities.typeOfOutputEnum.ChildMasterRecord:
                            case Utilities.typeOfOutputEnum.ChildRecord:
                                foreach (IDTSOutputColumn100 outputColumn in output.OutputColumnCollection)
                                {
                                    ManageProperties.AddMissingOutputColumnProperties(outputColumn.CustomPropertyCollection);
                                }
                                break;
                            case Utilities.typeOfOutputEnum.ErrorRecords:
                            case Utilities.typeOfOutputEnum.RowsProcessed:
                                break;
                            default:
                                break;
                        }
                    }
                }

                // Remove the ability to set columnProperties via Expressions.
                if (isPropertyExpressionNotify)
                {
                    foreach (IDTSOutput100 output in this.ComponentMetaData.OutputCollection)
                    {
                        switch ((Utilities.typeOfOutputEnum)ManageProperties.GetPropertyValue(output.CustomPropertyCollection, ManageProperties.typeOfOutput))
                        {
                            case Utilities.typeOfOutputEnum.KeyRecords:
                            case Utilities.typeOfOutputEnum.DataRecords:
                            case Utilities.typeOfOutputEnum.PassThrough:
                            case Utilities.typeOfOutputEnum.MasterRecord:
                            case Utilities.typeOfOutputEnum.ChildMasterRecord:
                            case Utilities.typeOfOutputEnum.ChildRecord:
                                foreach (IDTSOutputColumn100 outputColumn in output.OutputColumnCollection)
                                {
                                    foreach (IDTSCustomProperty100 columnProperty in outputColumn.CustomPropertyCollection)
                                    {
                                        columnProperty.ExpressionType = DTSCustomPropertyExpressionType.CPET_NONE;
                                    }
                                }
                                foreach (IDTSCustomProperty100 outputProperty in output.CustomPropertyCollection)
                                {
                                    outputProperty.ExpressionType = DTSCustomPropertyExpressionType.CPET_NONE;
                                }
                                break;
                            case Utilities.typeOfOutputEnum.ErrorRecords:
                            case Utilities.typeOfOutputEnum.RowsProcessed:
                                break;
                            default:
                                break;
                        }
                    }
                }

            }
            this.ComponentMetaData.Version = 3;  // NB.  Always keep this in sync with the CurrentVersion!!!
        }

        #endregion

        #region Validate

        public override DTSValidationStatus Validate()
        {
            DTSValidationStatus status = DTSValidationStatus.VS_ISVALID;
            status = ValidateComponentProperties(status);
            status = ValidateOutputs(status);
            return status;
        }

        private DTSValidationStatus ValidateComponentProperties(DTSValidationStatus oldStatus)
        {
            DTSValidationStatus returnStatus = oldStatus;
            returnStatus = this.propertyManager.ValidateComponentProperties(this.ComponentMetaData.CustomPropertyCollection, oldStatus);
            returnStatus =  this.propertyManager.ValidateProperties(this.ComponentMetaData.CustomPropertyCollection, returnStatus);
            return returnStatus;
        }

        private DTSValidationStatus ValidateOutputs(DTSValidationStatus oldStatus)
        {
            DTSValidationStatus returnStatus = oldStatus;
            if (this.ComponentMetaData.OutputCollection.Count < 4)
            {
                this.PostError(MessageStrings.UnexpectedNumberOfOutputs);
                returnStatus = DTSValidationStatus.VS_ISCORRUPT;
            }
            else
            {
                IDTSOutputCollection outputCollection = this.ComponentMetaData.OutputCollection;
                // Ensure The required outputs exist and there is only one of each!
                int errorOutputs = 0;
                int passThroughOutputs = 0;
                int keyOutputs = 0;
                int numberOfRowsOutput = 0;
                foreach (IDTSOutput output in outputCollection)
                {
                    if (output.IsErrorOut)
                    {
                        returnStatus = ValidateErrorOutput(output, returnStatus);
                        errorOutputs++;
                    }
                    else
                    {
                        returnStatus = ValidateRegularOutput(output, returnStatus, ref passThroughOutputs, ref keyOutputs, ref numberOfRowsOutput);
                    }
                }

                if (errorOutputs != 1)
                {
                    returnStatus = Utilities.CompareValidationValues(returnStatus, DTSValidationStatus.VS_ISBROKEN);
                    this.PostError(MessageStrings.NoErrorOutput);
                }
                if (passThroughOutputs != 1)
                {
                    returnStatus = Utilities.CompareValidationValues(oldStatus, DTSValidationStatus.VS_ISBROKEN);
                    this.PostError(MessageStrings.InvalidPassThoughOutput);
                }
                if (keyOutputs != 1)
                {
                    returnStatus = Utilities.CompareValidationValues(oldStatus, DTSValidationStatus.VS_ISBROKEN);
                    this.PostError(MessageStrings.InvalidPassKeyOutput);
                }
                if (numberOfRowsOutput != 1)
                {
                    returnStatus = Utilities.CompareValidationValues(oldStatus, DTSValidationStatus.VS_ISBROKEN);
                    this.PostError(MessageStrings.InvalidNumberOfRowsOutput);
                }
            }

            return returnStatus;
        }

        private DTSValidationStatus ValidateRegularOutput(IDTSOutput output, DTSValidationStatus oldStatus, ref int passThoughOutputs, ref int keyOutputs, ref int numberOfRowsOutput)
        {
            DTSValidationStatus returnStatus = oldStatus;

            IDTSOutputColumnCollection outputColumnCollection = output.OutputColumnCollection;

            if (output.SynchronousInputID != 0)
            {
                this.PostError(MessageStrings.OutputIsSyncronous(output.Name));
                returnStatus = Utilities.CompareValidationValues(returnStatus, DTSValidationStatus.VS_ISBROKEN);
            }
            oldStatus = returnStatus;
            returnStatus = this.propertyManager.ValidateOutputProperties(output.CustomPropertyCollection, returnStatus);
            if (returnStatus != oldStatus)
            {
                this.PostError(MessageStrings.APropertyIsMissing(output.Name));
                return returnStatus;
            }
            else
            {
                List<String> outputColumnNames = new List<string>();
                foreach (IDTSOutputColumn100 outputColumn in output.OutputColumnCollection)
                {
                    if (outputColumnNames.Contains(outputColumn.Name))
                    {
                        this.PostError(MessageStrings.OutputHasDuplicateColumnNames(output.Name, outputColumn.Name));
                        returnStatus = Utilities.CompareValidationValues(returnStatus, DTSValidationStatus.VS_ISBROKEN);
                    }
                    else
                    {
                        outputColumnNames.Add(outputColumn.Name);
                    }
                }
                switch ((Utilities.typeOfOutputEnum)ManageProperties.GetPropertyValue(output.CustomPropertyCollection, ManageProperties.typeOfOutput))
                {
                    case Utilities.typeOfOutputEnum.ErrorRecords:
                        this.PostError(MessageStrings.InvalidOutputType(output.Name, "ErrorRecords"));
                        returnStatus  =  Utilities.CompareValidationValues(returnStatus, DTSValidationStatus.VS_ISBROKEN);
                        break;
                    case Utilities.typeOfOutputEnum.KeyRecords:
                        keyOutputs++;
                        break;
                    case Utilities.typeOfOutputEnum.DataRecords:
                        returnStatus = ValidateDataOutput(output, returnStatus);
                        break;
                    case Utilities.typeOfOutputEnum.PassThrough:
                        passThoughOutputs++;
                        returnStatus = ValidateExternalMetaData(output, returnStatus);
                        break;
                    case Utilities.typeOfOutputEnum.MasterRecord:
                        returnStatus = ValidateMasterOutput(output, returnStatus);
                        break;
                    case Utilities.typeOfOutputEnum.ChildMasterRecord:
                        returnStatus = ValidateChildMasterOutput(output, returnStatus);
                        returnStatus = ValidateChildOutput(output, returnStatus);
                        break;
                    case Utilities.typeOfOutputEnum.ChildRecord:
                        returnStatus = ValidateChildOutput(output, returnStatus);
                        break;
                    case Utilities.typeOfOutputEnum.RowsProcessed:
                        numberOfRowsOutput++;
                        return ValidateRowsProcessedOutput(output, returnStatus);
                    default:
                        this.PostError(MessageStrings.InvalidOutputType(output.Name, "Unknown"));
                        returnStatus  =  Utilities.CompareValidationValues(returnStatus, DTSValidationStatus.VS_ISBROKEN);
                        break;
                }
            }

            if ((outputColumnCollection.Count == 0) && output.IsAttached)  // Must have output columns if we are attached!
            {
                this.PostError(MessageStrings.NoOutputColumns);
                returnStatus = Utilities.CompareValidationValues(returnStatus, DTSValidationStatus.VS_ISBROKEN);
            }
            else
            {
                returnStatus = ValidateOutputColumns(outputColumnCollection, returnStatus);
            }

            return returnStatus;
        }

        private DTSValidationStatus ValidateRowsProcessedOutput(IDTSOutput rowCountOutput, DTSValidationStatus oldStatus)
        {
            DTSValidationStatus returnStatus = oldStatus;
            if (rowCountOutput.OutputColumnCollection.Count != 3)
            {
                this.PostError(MessageStrings.RowCountOutputInvalid);
                returnStatus = Utilities.CompareValidationValues(returnStatus, DTSValidationStatus.VS_ISBROKEN);
            }
            if (rowCountOutput.OutputColumnCollection[0].DataType != DataType.DT_STR)
            {
                this.PostError(MessageStrings.RowCountOutputInvalid);
                returnStatus = Utilities.CompareValidationValues(returnStatus, DTSValidationStatus.VS_ISBROKEN);
            }
            if (rowCountOutput.OutputColumnCollection[1].DataType != DataType.DT_I8)
            {
                this.PostError(MessageStrings.RowCountOutputInvalid);
                returnStatus = Utilities.CompareValidationValues(returnStatus, DTSValidationStatus.VS_ISBROKEN);
            }
            if (rowCountOutput.OutputColumnCollection[2].DataType != DataType.DT_STR)
            {
                this.PostError(MessageStrings.RowCountOutputInvalid);
                returnStatus = Utilities.CompareValidationValues(returnStatus, DTSValidationStatus.VS_ISBROKEN);
            }
            return returnStatus;
        }

        private DTSValidationStatus ValidateExternalMetaData(IDTSOutput passThroughOutput, DTSValidationStatus oldStatus)
        {
            DTSValidationStatus returnStatus = oldStatus;

            if (ComponentMetaData.RuntimeConnectionCollection[0].ConnectionManager != null)
            {

                ConnectionManager cm = Microsoft.SqlServer.Dts.Runtime.DtsConvert.GetWrapper(ComponentMetaData.RuntimeConnectionCollection[0].ConnectionManager);
                if (String.IsNullOrEmpty(cm.ConnectionString))
                {
                    PostWarning(MessageStrings.ConnectionManagerNotSet);
                    return Utilities.CompareValidationValues(returnStatus, DTSValidationStatus.VS_NEEDSNEWMETADATA);
                }
                IDTSConnectionManagerFlatFile connectionFlatFile = cm.InnerObject as IDTSConnectionManagerFlatFile;
                if (passThroughOutput.ExternalMetadataColumnCollection.Count != connectionFlatFile.Columns.Count)
                {
                    PostWarning(MessageStrings.ExternalMetaDataOutOfSync);
                    return Utilities.CompareValidationValues(returnStatus, DTSValidationStatus.VS_NEEDSNEWMETADATA);
                }
                else
                {
                    // Check that the External Meta Data is the same!
                    for (int i = 0; i < connectionFlatFile.Columns.Count; i++)
                    {
                        IDTSOutputColumn outColumn = passThroughOutput.OutputColumnCollection[i];
                        IDTSConnectionManagerFlatFileColumn FFcolumn = connectionFlatFile.Columns[i];
                        if ((FFcolumn.MaximumWidth != outColumn.Length)
                          || (FFcolumn.DataType != outColumn.DataType)
                          || (FFcolumn.DataPrecision != outColumn.Precision)
                          || (FFcolumn.DataScale != outColumn.Scale)
                          || (connectionFlatFile.CodePage != outColumn.CodePage)
                          || (outColumn.Name != ((IDTSName100)FFcolumn).Name))
                        {
                            PostWarning(MessageStrings.ExternalMetaDataOutOfSync);
                            return Utilities.CompareValidationValues(returnStatus, DTSValidationStatus.VS_NEEDSNEWMETADATA);
                        }
                    }
                }
            }
            else
            {
                PostWarning(MessageStrings.ConnectionManagerNotSet);
                return Utilities.CompareValidationValues(returnStatus, DTSValidationStatus.VS_NEEDSNEWMETADATA);
            }
            return returnStatus;
        }

        private DTSValidationStatus ValidateErrorOutput(IDTSOutput errorOutput, DTSValidationStatus oldStatus)
        {
            DTSValidationStatus returnStatus = oldStatus;

            IDTSOutputColumnCollection outputColumnCollection = errorOutput.OutputColumnCollection;

            if (outputColumnCollection.Count < 5)
            {
                this.PostError(MessageStrings.ErrorOutputColumnsAreMissing);
                returnStatus = Utilities.CompareValidationValues(returnStatus, DTSValidationStatus.VS_ISCORRUPT);
            }
            else
            {
                foreach (IDTSOutputColumn outputColumn in outputColumnCollection)
                {
                    if (ManageProperties.GetPropertyValue(outputColumn.CustomPropertyCollection, ManageProperties.usageOfColumn) != null)
                    {
                        if ((Utilities.usageOfColumnEnum)ManageProperties.GetPropertyValue(outputColumn.CustomPropertyCollection, ManageProperties.usageOfColumn) != Utilities.usageOfColumnEnum.Key)
                        {
                            this.PostError(MessageStrings.ErrorOutputHasInvalidColumn(outputColumn.Name));
                            returnStatus = Utilities.CompareValidationValues(returnStatus, DTSValidationStatus.VS_ISCORRUPT);
                        }
                    }
                }
            }

            return returnStatus;
        }

        private DTSValidationStatus ValidateOutputColumns(IDTSOutputColumnCollection outputColumnCollection, DTSValidationStatus oldStatus)
        {
            DTSValidationStatus returnStatus = oldStatus;

            foreach (IDTSOutputColumn outputColumn in outputColumnCollection)
            {
                returnStatus = ValidateOutputColumn(outputColumn, returnStatus);
            }

            return returnStatus;
        }

        private DTSValidationStatus ValidateOutputColumn(IDTSOutputColumn outputColumn, DTSValidationStatus oldStatus)
        {
            oldStatus = propertyManager.ValidateProperties(outputColumn.CustomPropertyCollection, oldStatus);
            DTSValidationStatus returnStatus = oldStatus;
            returnStatus = this.propertyManager.ValidateOutputColumnProperties(outputColumn.CustomPropertyCollection, returnStatus);
            if (returnStatus != oldStatus)
            {
                this.PostError(MessageStrings.APropertyIsMissing(outputColumn.Name));
                return returnStatus;
            }
            return Utilities.CompareValidationValues(returnStatus, ValidateSupportedDataTypes(outputColumn.DataType));
        }

        private DTSValidationStatus ValidateSupportedDataTypes(DataType dataType)
        {
            if (dataType == DataType.DT_BYTES ||
                dataType == DataType.DT_IMAGE)
            {
                this.PostError(MessageStrings.UnsupportedDataType(dataType.ToString()));
                return DTSValidationStatus.VS_ISBROKEN;
            }
            else
            {
                return DTSValidationStatus.VS_ISVALID;
            }
        }

        private DTSValidationStatus ValidateDataOutput(IDTSOutput output, DTSValidationStatus oldStatus)
        {
            if ((int)ManageProperties.GetPropertyValue(output.CustomPropertyCollection, ManageProperties.masterRecordID) != -1)
            {
                this.PostError(MessageStrings.DataOutputHasRecordID(output.Name));
                return Utilities.CompareValidationValues(oldStatus, DTSValidationStatus.VS_ISBROKEN);
            }
            foreach (IDTSOutputColumn100 outputColumn in output.OutputColumnCollection)
            {
                switch ((Utilities.usageOfColumnEnum)ManageProperties.GetPropertyValue(outputColumn.CustomPropertyCollection, ManageProperties.usageOfColumn))
                {
                    case Utilities.usageOfColumnEnum.RowType:
                        this.PostError(MessageStrings.CannotSetPropertyToRowType);
                        return Utilities.CompareValidationValues(oldStatus, DTSValidationStatus.VS_ISBROKEN);
                    case Utilities.usageOfColumnEnum.RowData:
                        this.PostError(MessageStrings.CannotSetPropertyToRowData);
                        return Utilities.CompareValidationValues(oldStatus, DTSValidationStatus.VS_ISBROKEN);
                    case Utilities.usageOfColumnEnum.Passthrough:
                        break;
                    case Utilities.usageOfColumnEnum.Key:
                        break;
                    case Utilities.usageOfColumnEnum.Ignore:
                        break;
                    case Utilities.usageOfColumnEnum.MasterValue:
                        this.PostError(MessageStrings.CannotSetPropertyToMasterValue);
                        return Utilities.CompareValidationValues(oldStatus, DTSValidationStatus.VS_ISBROKEN);
                    default:
                        break;
                }
            }
            return oldStatus;
        }

        private DTSValidationStatus ValidateMasterOutput(IDTSOutput output, DTSValidationStatus oldStatus)
        {
            if ((int)ManageProperties.GetPropertyValue(output.CustomPropertyCollection, ManageProperties.masterRecordID) != -1)
            {
                this.PostError(MessageStrings.MasterOutputHasRecordID(output.Name));
                return Utilities.CompareValidationValues(oldStatus, DTSValidationStatus.VS_ISBROKEN);
            }
            foreach (IDTSOutputColumn100 outputColumn in output.OutputColumnCollection)
            {
                switch ((Utilities.usageOfColumnEnum)ManageProperties.GetPropertyValue(outputColumn.CustomPropertyCollection, ManageProperties.usageOfColumn))
                {
                    case Utilities.usageOfColumnEnum.RowType:
                        this.PostError(MessageStrings.CannotSetPropertyToRowType);
                        return Utilities.CompareValidationValues(oldStatus, DTSValidationStatus.VS_ISBROKEN);
                    case Utilities.usageOfColumnEnum.RowData:
                        this.PostError(MessageStrings.CannotSetPropertyToRowData);
                        return Utilities.CompareValidationValues(oldStatus, DTSValidationStatus.VS_ISBROKEN);
                    case Utilities.usageOfColumnEnum.Passthrough:
                        break;
                    case Utilities.usageOfColumnEnum.Key:
                        break;
                    case Utilities.usageOfColumnEnum.Ignore:
                        break;
                    case Utilities.usageOfColumnEnum.MasterValue:
                        if ((int)ManageProperties.GetPropertyValue(outputColumn.CustomPropertyCollection, ManageProperties.keyOutputColumnID) != -1)
                        {
                            this.PostError(MessageStrings.MasterColumnHasKeyOutputID(outputColumn.Name, output.Name));
                            return Utilities.CompareValidationValues(oldStatus, DTSValidationStatus.VS_ISBROKEN);
                        }
                        break;
                    default:
                        break;
                }
            }
            return oldStatus;
        }

        private DTSValidationStatus ValidateChildMasterOutput(IDTSOutput output, DTSValidationStatus oldStatus)
        {
            if ((int)ManageProperties.GetPropertyValue(output.CustomPropertyCollection, ManageProperties.masterRecordID) == -1)
            {
                this.PostError(MessageStrings.MasterRecordIDInvalid(output.Name));
                return Utilities.CompareValidationValues(oldStatus, DTSValidationStatus.VS_ISBROKEN);
            }
            return oldStatus;
        }


        private DTSValidationStatus ValidateChildOutput(IDTSOutput output, DTSValidationStatus oldStatus)
        {
            // Make sure that the Master Record ID is pointing at a Master Output
            int masterRecordID = (int) ManageProperties.GetPropertyValue(output.CustomPropertyCollection, ManageProperties.masterRecordID);
            if (masterRecordID == -1)
            {
                this.PostError(MessageStrings.MasterRecordIDInvalid(output.Name));
                return Utilities.CompareValidationValues(oldStatus, DTSValidationStatus.VS_ISBROKEN);
            }
            IDTSOutput masterOutput = null;
            try
            {
                masterOutput = this.ComponentMetaData.OutputCollection.FindObjectByID(masterRecordID);
                if (masterOutput == null)
                {
                    this.PostError(MessageStrings.MasterRecordIDInvalid(output.Name));
                    return Utilities.CompareValidationValues(oldStatus, DTSValidationStatus.VS_ISBROKEN);
                }
            }
            catch
            {
                this.PostError(MessageStrings.MasterRecordIDInvalid(output.Name));
                return Utilities.CompareValidationValues(oldStatus, DTSValidationStatus.VS_ISBROKEN);
            }

            // Loop though all the output columns on the Master, and make sure that the Child has them, and they are of the correct data types.
            foreach (IDTSOutputColumn masterOutputColumn in masterOutput.OutputColumnCollection)
            {
                if ((Utilities.usageOfColumnEnum)ManageProperties.GetPropertyValue(masterOutputColumn.CustomPropertyCollection, ManageProperties.usageOfColumn) == Utilities.usageOfColumnEnum.MasterValue)
                {
                    Boolean foundColumn = false;
                    foreach (IDTSOutputColumn childOutputColumn in output.OutputColumnCollection)
                    {
                        if ((Utilities.usageOfColumnEnum)ManageProperties.GetPropertyValue(childOutputColumn.CustomPropertyCollection, ManageProperties.usageOfColumn) == Utilities.usageOfColumnEnum.MasterValue)
                        {
                            if ((int)ManageProperties.GetPropertyValue(childOutputColumn.CustomPropertyCollection, ManageProperties.keyOutputColumnID) != -1)
                            {
                                if (((int)ManageProperties.GetPropertyValue(childOutputColumn.CustomPropertyCollection, ManageProperties.keyOutputColumnID) == masterOutputColumn.LineageID)
                                    || ((int)ManageProperties.GetPropertyValue(childOutputColumn.CustomPropertyCollection, ManageProperties.keyOutputColumnID) == (int)ManageProperties.GetPropertyValue(masterOutputColumn.CustomPropertyCollection, ManageProperties.keyOutputColumnID)))
                                {
                                    foundColumn = true;
                                    oldStatus = ValidateOutputAndKeyColumn(masterOutputColumn, childOutputColumn, output.Name, oldStatus);
                                }
                            }
                        }
                    }
                    if (!foundColumn)
                    {
                        this.PostError(MessageStrings.ChildColumnInvalid(output.Name, masterOutputColumn.Name));
                        return Utilities.CompareValidationValues(oldStatus, DTSValidationStatus.VS_ISBROKEN);
                    }
                }
            }

            return oldStatus;
        }

        private DTSValidationStatus ValidateOutputAndKeyColumn(IDTSOutputColumn keyColumn, IDTSOutputColumn dataColumn, String dataOutputName, DTSValidationStatus oldStatus)
        {
            if ((keyColumn.CodePage != dataColumn.CodePage)
             || (keyColumn.DataType != dataColumn.DataType)
             || (keyColumn.Length != dataColumn.Length)
             || (keyColumn.Precision != dataColumn.Precision)
             || (keyColumn.Scale != dataColumn.Scale))
            {
                this.PostError(MessageStrings.ChildColumnInvalid(dataOutputName, dataColumn.Name));
                return Utilities.CompareValidationValues(oldStatus, DTSValidationStatus.VS_ISBROKEN);
            }
            return oldStatus;
        }

        #endregion

        #region ReInitialise Meta Data

        public override void ReinitializeMetaData()
        {
            if (this.ComponentMetaData.OutputCollection[0].OutputColumnCollection.Count == 0)
            {
                if (!String.IsNullOrEmpty(fileName))
                {
                    ConnectionManager cm = Microsoft.SqlServer.Dts.Runtime.DtsConvert.GetWrapper(ComponentMetaData.RuntimeConnectionCollection[0].ConnectionManager);
                    IDTSConnectionManagerFlatFile connectionFlatFile = cm.InnerObject as IDTSConnectionManagerFlatFile;
                    foreach (IDTSConnectionManagerFlatFileColumn FFcolumn in connectionFlatFile.Columns)
                    {
                        IDTSOutputColumn outColumn = this.ComponentMetaData.OutputCollection[0].OutputColumnCollection.New();
                        ManageColumns.SetOutputColumnDefaults(outColumn, connectionFlatFile.CodePage);
                        ManageProperties.AddOutputColumnProperties(outColumn.CustomPropertyCollection);
                        outColumn.Name = ((IDTSName100)FFcolumn).Name;
                        outColumn.SetDataTypeProperties(FFcolumn.DataType, FFcolumn.MaximumWidth, FFcolumn.DataPrecision, FFcolumn.DataScale, connectionFlatFile.CodePage);
                        IDTSExternalMetadataColumn eColumn = this.ComponentMetaData.OutputCollection[0].ExternalMetadataColumnCollection.New();
                        eColumn.Name = outColumn.Name;
                        eColumn.DataType = outColumn.DataType;
                        eColumn.Precision = outColumn.Precision;
                        eColumn.Scale = outColumn.Scale;
                        eColumn.Length = outColumn.Length;
                        outColumn.ExternalMetadataColumnID = eColumn.ID;
                    }
                }
            }
        }

        #endregion

        #region Acquire Connection
        public override void AcquireConnections(object transaction)
        {
            if (this.ComponentMetaData.RuntimeConnectionCollection[0].ConnectionManager != null)
            {
                ConnectionManager cm = Microsoft.SqlServer.Dts.Runtime.DtsConvert.GetWrapper(ComponentMetaData.RuntimeConnectionCollection[0].ConnectionManager);
                ConnectionManagerFlatFile cmFlatFile = cm.InnerObject as ConnectionManagerFlatFile;
                IDTSConnectionManagerFlatFile connectionFlatFile = cm.InnerObject as IDTSConnectionManagerFlatFile;

                if (cmFlatFile == null)
                {
                    PostErrorAndThrow(MessageStrings.MustBeFlatFileConnection);
                }
                else
                {
                    // Have to acquire the connection to get any Expressions to fire...
                    this.ffConnection = cm.AcquireConnection(transaction);
                    fileName = cmFlatFile.ConnectionString;
                    this.codePage = connectionFlatFile.CodePage;
                }
            }
        }
        #endregion

        #region Release Connections
        public override void ReleaseConnections()
        {
            if (!String.IsNullOrEmpty(fileName))
            {
                fileName = String.Empty;
                if (this.ffConnection != null)
                {

                }
            }
        }
        #endregion

        #region Set Component Property
        public override IDTSCustomProperty SetComponentProperty(string propertyName, object propertyValue)
        {
            if (this.propertyManager.ValidatePropertyValue(propertyName, propertyValue, DTSValidationStatus.VS_ISVALID) == DTSValidationStatus.VS_ISVALID)
            {
                return base.SetComponentProperty(propertyName, propertyValue);
            }
            else
            {
                throw new COMException(MessageStrings.InvalidPropertyValue(propertyName, propertyValue), E_FAIL);
            }
        }
        #endregion

        #region Insert Input

        /// <summary>
        /// Prevents the addition of Input's to this component
        /// </summary>
        /// <param name="insertPlacement"></param>
        /// <param name="inputID"></param>
        /// <returns>null</returns>
        public override IDTSInput InsertInput(DTSInsertPlacement insertPlacement, int inputID)
        {
            this.PostErrorAndThrow(MessageStrings.CantAddInput);
            return null;
        }

        #endregion

        #region InsertOutput
        /// <summary>
        /// Add the Output at the selected location, and ensures that all the required MetaData is associated with the output.
        /// This will also Propogate Columns marked as Key's from the "KeyRecords" output.
        /// </summary>
        /// <param name="insertPlacement">passed from SSIS</param>
        /// <param name="outputID">passed from SSIS</param>
        /// <returns>the new output</returns>
        public override IDTSOutput InsertOutput(DTSInsertPlacement insertPlacement, int outputID)
        {
            IDTSOutput thisOutput = base.InsertOutput(insertPlacement, outputID);
            ManageProperties.AddOutputProperties(thisOutput.CustomPropertyCollection);
            thisOutput.ExternalMetadataColumnCollection.IsUsed = false;
            thisOutput.SynchronousInputID = 0;
            thisOutput.ErrorRowDisposition = DTSRowDisposition.RD_FailComponent;
            thisOutput.ErrorOrTruncationOperation = MessageStrings.RowLevelErrorOperation;

            // Add any keys that have already been defined!
            foreach (IDTSOutput keyOutput in this.ComponentMetaData.OutputCollection)
            {
                if ((Utilities.typeOfOutputEnum)ManageProperties.GetPropertyValue(keyOutput.CustomPropertyCollection, ManageProperties.typeOfOutput) == Utilities.typeOfOutputEnum.KeyRecords)
                {
                    // Find the Key columns, and add them to this output.
                    foreach (IDTSOutputColumn keyColumn in keyOutput.OutputColumnCollection)
                    {
                        if ((Utilities.usageOfColumnEnum)ManageProperties.GetPropertyValue(keyColumn.CustomPropertyCollection, ManageProperties.usageOfColumn) == Utilities.usageOfColumnEnum.Key)
                        {
                            IDTSOutputColumn outputColumn = thisOutput.OutputColumnCollection.New();
                            outputColumn.Name = keyColumn.Name;
                            outputColumn.Description = MessageStrings.KeyColumnDescription;
                            ManageProperties.AddOutputColumnProperties(outputColumn.CustomPropertyCollection);
                            ManageProperties.SetPropertyValue(outputColumn.CustomPropertyCollection, ManageProperties.usageOfColumn, Utilities.usageOfColumnEnum.Key);
                            ManageProperties.SetPropertyValue(outputColumn.CustomPropertyCollection, ManageProperties.keyOutputColumnID, keyColumn.LineageID);
                            outputColumn.SetDataTypeProperties(keyColumn.DataType, keyColumn.Length, keyColumn.Precision, keyColumn.Scale, keyColumn.CodePage);
                        }
                    }
                    // There can only be ONE Key Output, so stop iterating.
                    break;
                }
            }

            return thisOutput;
        }

        #endregion

        #region InsertOutputColumnAt (Add the properties to Output Columns)
        public override IDTSOutputColumn InsertOutputColumnAt(int outputID, int outputColumnIndex, string name, string description)
        {
            if (!this.ComponentMetaData.OutputCollection.GetObjectByID(outputID).IsErrorOut)
            {
                Utilities.typeOfOutputEnum outputUsage = (Utilities.typeOfOutputEnum)ManageProperties.GetPropertyValue(this.ComponentMetaData.OutputCollection.GetObjectByID(outputID).CustomPropertyCollection, ManageProperties.typeOfOutput);
                switch (outputUsage)
                {
                    case Utilities.typeOfOutputEnum.ErrorRecords:
                        this.PostErrorAndThrow(MessageStrings.CantChangeOutputProperties("Error"));
                        return null;
                    case Utilities.typeOfOutputEnum.RowsProcessed:
                        this.PostErrorAndThrow(MessageStrings.CantChangeOutputProperties("RowsProcessed"));
                        return null;
                    case Utilities.typeOfOutputEnum.PassThrough:
                        this.PostErrorAndThrow(MessageStrings.CantChangeOutputProperties("PassThrough"));
                        return null;
                    case Utilities.typeOfOutputEnum.KeyRecords:
                    case Utilities.typeOfOutputEnum.DataRecords:
                    case Utilities.typeOfOutputEnum.MasterRecord:
                    case Utilities.typeOfOutputEnum.ChildMasterRecord:
                    case Utilities.typeOfOutputEnum.ChildRecord:
                    default:
                        IDTSOutputColumn col = base.InsertOutputColumnAt(outputID, outputColumnIndex, name, description);
                        ManageProperties.AddOutputColumnProperties(col.CustomPropertyCollection);
                        col.ErrorOrTruncationOperation = MessageStrings.ColumnLevelErrorTruncationOperation;
                        ManageColumns.SetOutputColumnDefaults(col, codePage);
                        return col;
                }
            }
            else
            {
                this.PostErrorAndThrow(MessageStrings.CantChangeOutputProperties("Error"));
                return null;
            }
        }
        #endregion

        #region SetOutputColumnProperty
        /// <summary>
        /// Enables the setting of custom properties.  If the Property is Key, (and on the appropriate output), then the column will be propogated to all Data outputs.
        /// </summary>
        /// <param name="outputID"></param>
        /// <param name="outputColumnID"></param>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        /// <returns></returns>
        public override IDTSCustomProperty SetOutputColumnProperty(int outputID, int outputColumnID, string propertyName, object propertyValue)
        {
            if (propertyName == ManageProperties.keyOutputColumnID)
            {
                throw new COMException(MessageStrings.CannotSetProperty, E_FAIL);
            }
            if (this.propertyManager.ValidatePropertyValue(propertyName, propertyValue, DTSValidationStatus.VS_ISVALID) == DTSValidationStatus.VS_ISVALID)
            {
                IDTSOutput thisOutput = this.ComponentMetaData.OutputCollection.GetObjectByID(outputID);
                IDTSOutputColumn thisColumn = thisOutput.OutputColumnCollection.GetObjectByID(outputColumnID);
                Utilities.typeOfOutputEnum thisOutputType = (Utilities.typeOfOutputEnum)ManageProperties.GetPropertyValue(thisOutput.CustomPropertyCollection, ManageProperties.typeOfOutput);

                switch (thisOutputType)
                {
                    case Utilities.typeOfOutputEnum.ErrorRecords:
                        throw new COMException(MessageStrings.CantChangeOutputProperties("Error"), E_FAIL);
                    case Utilities.typeOfOutputEnum.RowsProcessed:
                        throw new COMException(MessageStrings.CantChangeOutputProperties("RowsProcessed"), E_FAIL);
                    default:
                        break;
                }

                // We need to add/remove the key columns to the outputs...
                if (propertyName == ManageProperties.usageOfColumn)
                {
                    Utilities.usageOfColumnEnum oldValue = (Utilities.usageOfColumnEnum)ManageProperties.GetPropertyValue(thisColumn.CustomPropertyCollection, ManageProperties.usageOfColumn);
                    switch ((Utilities.typeOfOutputEnum)ManageProperties.GetPropertyValue(thisOutput.CustomPropertyCollection, ManageProperties.typeOfOutput))
                    {
                        case Utilities.typeOfOutputEnum.ErrorRecords:
                            break;
                        case Utilities.typeOfOutputEnum.KeyRecords:
                            #region KeyRecords
                            // The Key value can be used here, BUT the column must be added to the other outputs!
                            oldValue = (Utilities.usageOfColumnEnum)ManageProperties.GetPropertyValue(thisColumn.CustomPropertyCollection, ManageProperties.usageOfColumn);
                            if ((oldValue != (Utilities.usageOfColumnEnum)propertyValue) && ((oldValue == Utilities.usageOfColumnEnum.Key) || ((Utilities.usageOfColumnEnum)propertyValue == Utilities.usageOfColumnEnum.Key)))
                            {
                                if ((Utilities.usageOfColumnEnum)propertyValue == Utilities.usageOfColumnEnum.Key)
                                {
                                    // Determine the correct position for this record in the output.
                                    int keyPosition = 0;
                                    for (int i = 0; i < thisOutput.OutputColumnCollection.Count; i++)
                                    {
                                        if (thisOutput.OutputColumnCollection[i].ID == outputColumnID)
                                        {
                                            break;
                                        }
                                        else
                                        {
                                            if ((Utilities.usageOfColumnEnum)ManageProperties.GetPropertyValue(thisOutput.OutputColumnCollection[i].CustomPropertyCollection, ManageProperties.usageOfColumn) == Utilities.usageOfColumnEnum.Key)
                                            {
                                                keyPosition++;
                                            }
                                        }
                                    }
                                    // Add column to the other Outputs.
                                    foreach (IDTSOutput output in this.ComponentMetaData.OutputCollection)
                                    {
                                        Utilities.typeOfOutputEnum outputType = (Utilities.typeOfOutputEnum)ManageProperties.GetPropertyValue(output.CustomPropertyCollection, ManageProperties.typeOfOutput);
                                        if ((outputType == Utilities.typeOfOutputEnum.DataRecords)
                                            || (outputType == Utilities.typeOfOutputEnum.MasterRecord)
                                            || (outputType == Utilities.typeOfOutputEnum.ChildRecord)
                                            || (outputType == Utilities.typeOfOutputEnum.ChildMasterRecord))
                                        {
                                            IDTSOutputColumn outputColumn = output.OutputColumnCollection.NewAt(keyPosition);
                                            outputColumn.Name = thisColumn.Name;
                                            outputColumn.Description = MessageStrings.KeyColumnDescription;
                                            ManageProperties.AddOutputColumnProperties(outputColumn.CustomPropertyCollection);
                                            ManageProperties.SetPropertyValue(outputColumn.CustomPropertyCollection, ManageProperties.usageOfColumn, Utilities.usageOfColumnEnum.Key);
                                            ManageProperties.SetPropertyValue(outputColumn.CustomPropertyCollection, ManageProperties.keyOutputColumnID, thisColumn.LineageID);
                                            outputColumn.SetDataTypeProperties(thisColumn.DataType, thisColumn.Length, thisColumn.Precision, thisColumn.Scale, thisColumn.CodePage);
                                        }
                                        else if (outputType == Utilities.typeOfOutputEnum.ErrorRecords)
                                        {
                                            IDTSOutputColumn outputColumn = output.OutputColumnCollection.New();
                                            outputColumn.Name = thisColumn.Name;
                                            outputColumn.Description = MessageStrings.KeyColumnDescription;
                                            ManageProperties.AddOutputColumnProperties(outputColumn.CustomPropertyCollection);
                                            ManageProperties.SetPropertyValue(outputColumn.CustomPropertyCollection, ManageProperties.usageOfColumn, Utilities.usageOfColumnEnum.Key);
                                            ManageProperties.SetPropertyValue(outputColumn.CustomPropertyCollection, ManageProperties.keyOutputColumnID, thisColumn.LineageID);
                                            outputColumn.SetDataTypeProperties(thisColumn.DataType, thisColumn.Length, thisColumn.Precision, thisColumn.Scale, thisColumn.CodePage);
                                        }
                                    }
                                }
                                else
                                {
                                    // Remove this column from the outputs
                                    RemoveLinkedColumnFromOutputs(thisColumn);
                                }
                            } 
                            #endregion
                            break;
                        case Utilities.typeOfOutputEnum.DataRecords:
                        case Utilities.typeOfOutputEnum.ChildRecord:
                            if ((oldValue == Utilities.usageOfColumnEnum.Key) || (oldValue == Utilities.usageOfColumnEnum.MasterValue))
                            {
                                throw new COMException(MessageStrings.InvalidPropertyValue(ManageProperties.usageOfColumn, System.Enum.GetName(typeof(Utilities.usageOfColumnEnum), propertyValue)));
                            }

                            switch ((Utilities.usageOfColumnEnum) propertyValue)
	                        {
                                case Utilities.usageOfColumnEnum.RowType:
                                    throw new COMException(MessageStrings.CannotSetPropertyToRowType, E_FAIL);
                                case Utilities.usageOfColumnEnum.RowData:
                                    throw new COMException(MessageStrings.CannotSetPropertyToRowData, E_FAIL);
                                case Utilities.usageOfColumnEnum.Key:
                                    throw new COMException(MessageStrings.CannotSetPropertyToKey, E_FAIL);
                                case Utilities.usageOfColumnEnum.MasterValue:
                                    throw new COMException(MessageStrings.CannotSetPropertyToMasterValue, E_FAIL);
                                default:
                                    break;
	                        }
                            break;
                        case Utilities.typeOfOutputEnum.PassThrough:
                            switch ((Utilities.usageOfColumnEnum) propertyValue)
	                        {
                                case Utilities.usageOfColumnEnum.RowType:
                                    int CountOfRowTypes = 0;
                                    foreach (IDTSOutputColumn outputColumn in thisOutput.OutputColumnCollection)
                                    {
                                        if ((Utilities.usageOfColumnEnum)ManageProperties.GetPropertyValue(outputColumn.CustomPropertyCollection, ManageProperties.usageOfColumn) == Utilities.usageOfColumnEnum.RowType)
                                        {
                                            CountOfRowTypes++;
                                        }
                                    }
                                    if (CountOfRowTypes > 0)
                                    {
                                        throw new COMException(MessageStrings.ThereCanOnlyBeOneRowTypeColumn, E_FAIL);
                                    }
                                    break;
                                case Utilities.usageOfColumnEnum.RowData:
                                    int CountOfRowDatas = 0;
                                    foreach (IDTSOutputColumn outputColumn in thisOutput.OutputColumnCollection)
                                    {
                                        if ((Utilities.usageOfColumnEnum)ManageProperties.GetPropertyValue(outputColumn.CustomPropertyCollection, ManageProperties.usageOfColumn) == Utilities.usageOfColumnEnum.RowData)
                                        {
                                            CountOfRowDatas++;
                                        }
                                    }
                                    if (CountOfRowDatas > 0)
                                    {
                                        throw new COMException(MessageStrings.ThereCanOnlyBeOneRowDataColumn, E_FAIL);
                                    }
                                    break;
                                case Utilities.usageOfColumnEnum.Key:
                                    throw new COMException(MessageStrings.CannotSetPropertyToKey, E_FAIL);
                                case Utilities.usageOfColumnEnum.MasterValue:
                                    throw new COMException(MessageStrings.CannotSetPropertyToMasterValue, E_FAIL);
                                case Utilities.usageOfColumnEnum.Passthrough:
                                case Utilities.usageOfColumnEnum.Ignore:
                                    break;
                                default:
                                    break;
	                        }
                            break;
                        case Utilities.typeOfOutputEnum.MasterRecord:
                        case Utilities.typeOfOutputEnum.ChildMasterRecord:
                            switch ((Utilities.usageOfColumnEnum)propertyValue)
	                            {
                                    case Utilities.usageOfColumnEnum.RowType:
                                        throw new COMException(MessageStrings.CannotSetPropertyToRowType, E_FAIL);
                                    case Utilities.usageOfColumnEnum.RowData:
                                        throw new COMException(MessageStrings.CannotSetPropertyToRowData, E_FAIL);
                                    case Utilities.usageOfColumnEnum.Key:
                                        throw new COMException(MessageStrings.CannotSetPropertyToKey, E_FAIL);
                                    case Utilities.usageOfColumnEnum.Passthrough:
                                    case Utilities.usageOfColumnEnum.Ignore:
                                    case Utilities.usageOfColumnEnum.MasterValue:
                                        if ((oldValue != (Utilities.usageOfColumnEnum)propertyValue) && (oldValue != Utilities.usageOfColumnEnum.MasterValue))
                                        {
                                            // Determine the correct position for this record in the output.
                                            PushMasterValueColumnsToChildren(thisOutput, thisColumn.ID, thisColumn);
                                        }
                                        else
                                        {
                                            if ((oldValue != (Utilities.usageOfColumnEnum)propertyValue) && (oldValue == Utilities.usageOfColumnEnum.MasterValue))
                                            {
                                                // Remove this column from the other outputs
                                                RemoveLinkedColumnFromOutputs(thisColumn);
                                            }
                                        }
                                        //ToDo: Apply Value down the Chain!
                                        break;
                                    default:
                                        throw new COMException(MessageStrings.CannotSetProperty, E_FAIL);
                                }
                            break;
                        default:
                            break;
                    }
                }
                return base.SetOutputColumnProperty(outputID, outputColumnID, propertyName, propertyValue);
            }
            else
            {
                throw new COMException(string.Empty, E_FAIL);
            }
        }


        
        #endregion

        #region SetOutputColumnDataTypeProperties
        public override void SetOutputColumnDataTypeProperties(int iOutputID, int iOutputColumnID, DataType eDataType, int iLength, int iPrecision, int iScale, int iCodePage)
        {
            IDTSOutputCollection100 thisOutputColl = ComponentMetaData.OutputCollection;
            IDTSOutput100 thisOutput = thisOutputColl.GetObjectByID(iOutputID);
            if (thisOutput != null)
            {
                if (thisOutput.IsErrorOut)
                {
                    this.PostErrorAndThrow(MessageStrings.CantChangeOutputProperties("Error"));
                }
                else
                {
                    IDTSOutputColumnCollection100 thisColumnColl = thisOutput.OutputColumnCollection;
                    IDTSOutputColumn100 thisColumn = thisColumnColl.GetObjectByID(iOutputColumnID);
                    if (thisColumn != null)
                    {
                        if (((Utilities.usageOfColumnEnum)ManageProperties.GetPropertyValue(thisColumn.CustomPropertyCollection, ManageProperties.usageOfColumn) == Utilities.usageOfColumnEnum.Key)
                           & ((Utilities.typeOfOutputEnum)ManageProperties.GetPropertyValue(thisOutput.CustomPropertyCollection, ManageProperties.typeOfOutput) != Utilities.typeOfOutputEnum.KeyRecords))
                        {
                            throw new COMException(MessageStrings.CannotSetProperty, E_FAIL);
                        }
                        if (((Utilities.usageOfColumnEnum)ManageProperties.GetPropertyValue(thisColumn.CustomPropertyCollection, ManageProperties.usageOfColumn) == Utilities.usageOfColumnEnum.MasterValue)
                           & ((int)ManageProperties.GetPropertyValue(thisColumn.CustomPropertyCollection, ManageProperties.keyOutputColumnID) != -1))
                        {
                            throw new COMException(MessageStrings.CannotSetProperty, E_FAIL);
                        }

                        if (ValidateSupportedDataTypes(eDataType) == DTSValidationStatus.VS_ISVALID)
                        {
                            thisColumn.SetDataTypeProperties(eDataType, iLength, iPrecision, iScale, iCodePage);
                            if (((Utilities.usageOfColumnEnum)ManageProperties.GetPropertyValue(thisColumn.CustomPropertyCollection, ManageProperties.usageOfColumn) == Utilities.usageOfColumnEnum.Key)
                            ||(((Utilities.usageOfColumnEnum)ManageProperties.GetPropertyValue(thisColumn.CustomPropertyCollection, ManageProperties.usageOfColumn) == Utilities.usageOfColumnEnum.MasterValue)
                           & ((int)ManageProperties.GetPropertyValue(thisColumn.CustomPropertyCollection, ManageProperties.keyOutputColumnID) == -1)))
                            {
                                // Need to set the "children"!
                                foreach (IDTSOutput output in this.ComponentMetaData.OutputCollection)
                                {
                                    if (output.CustomPropertyCollection.Count > 0)
                                    {
                                        switch ((Utilities.typeOfOutputEnum)ManageProperties.GetPropertyValue(output.CustomPropertyCollection, ManageProperties.typeOfOutput))
                                        {
                                            case Utilities.typeOfOutputEnum.ErrorRecords:
                                            case Utilities.typeOfOutputEnum.KeyRecords:
                                            case Utilities.typeOfOutputEnum.PassThrough:
                                            case Utilities.typeOfOutputEnum.RowsProcessed:
                                                break;
                                            case Utilities.typeOfOutputEnum.DataRecords:
                                            case Utilities.typeOfOutputEnum.MasterRecord:
                                            case Utilities.typeOfOutputEnum.ChildMasterRecord:
                                            case Utilities.typeOfOutputEnum.ChildRecord:
                                            foreach (IDTSOutputColumn outputColumn in output.OutputColumnCollection)
                                            {
                                                if (thisColumn.LineageID == (int)ManageProperties.GetPropertyValue(outputColumn.CustomPropertyCollection, ManageProperties.keyOutputColumnID))
                                                {
                                                    outputColumn.SetDataTypeProperties(eDataType, iLength, iPrecision, iScale, iCodePage);
                                                }
                                            }
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            throw new COMException(string.Empty, E_FAIL);
                        }
                    }
                }
            }
        }
        #endregion

        #region DeleteOutputColumn
        /// <summary>
        /// Delete columns from outputs where the output is NOT and error output.
        /// Also ensure that if this is a KeyRecord output, and the column is a Key, remove it from all other Data outputs
        /// Also prevent deletion of Key columns from NON KeyRecord outputs.
        /// </summary>
        /// <param name="outputID"></param>
        /// <param name="outputColumnID"></param>
        public override void DeleteOutputColumn(int outputID, int outputColumnID)
        {
            IDTSOutput100 thisOutput = this.ComponentMetaData.OutputCollection.GetObjectByID(outputID);
            IDTSOutputColumn thisColumn = thisOutput.OutputColumnCollection.GetObjectByID(outputColumnID);
            Utilities.typeOfOutputEnum thisOutputType = (Utilities.typeOfOutputEnum)ManageProperties.GetPropertyValue(thisOutput.CustomPropertyCollection, ManageProperties.typeOfOutput);

            switch (thisOutputType)
            {
                case Utilities.typeOfOutputEnum.ErrorRecords:
                    throw new COMException(MessageStrings.CantChangeOutputProperties("Error"), E_FAIL);
                case Utilities.typeOfOutputEnum.RowsProcessed:
                    throw new COMException(MessageStrings.CantChangeOutputProperties("RowsProcessed"), E_FAIL);
                default:
                    break;
            }

            if ((Utilities.usageOfColumnEnum)ManageProperties.GetPropertyValue(thisColumn.CustomPropertyCollection, ManageProperties.usageOfColumn) == Utilities.usageOfColumnEnum.Key)
            {
                if ((Utilities.typeOfOutputEnum)ManageProperties.GetPropertyValue(thisOutput.CustomPropertyCollection, ManageProperties.typeOfOutput) != Utilities.typeOfOutputEnum.KeyRecords)
                {
                    throw new COMException(MessageStrings.CannotSetProperty, E_FAIL);
                }
                else
                {
                    // Need to delete the "children"!
                    foreach (IDTSOutput output in this.ComponentMetaData.OutputCollection)
                    {
                        if (!output.IsErrorOut)
                        {
                            if ((Utilities.typeOfOutputEnum)ManageProperties.GetPropertyValue(output.CustomPropertyCollection, ManageProperties.typeOfOutput) == Utilities.typeOfOutputEnum.DataRecords)
                            {
                                int columnIDToDelete = -1;
                                foreach (IDTSOutputColumn outputColumn in output.OutputColumnCollection)
                                    if (thisColumn.LineageID == (int)ManageProperties.GetPropertyValue(outputColumn.CustomPropertyCollection, ManageProperties.keyOutputColumnID))
                                    {
                                        columnIDToDelete = outputColumn.ID;
                                        break;
                                    }
                                if (columnIDToDelete != -1)
                                {
                                    output.OutputColumnCollection.RemoveObjectByID(columnIDToDelete);
                                }
                            }
                        }
                    }
                }
            }
            else if ((Utilities.usageOfColumnEnum)ManageProperties.GetPropertyValue(thisColumn.CustomPropertyCollection, ManageProperties.usageOfColumn) == Utilities.usageOfColumnEnum.MasterValue)
            {
                switch ((Utilities.typeOfOutputEnum)ManageProperties.GetPropertyValue(thisOutput.CustomPropertyCollection, ManageProperties.typeOfOutput))
                {
                    case Utilities.typeOfOutputEnum.ErrorRecords:
                    case Utilities.typeOfOutputEnum.KeyRecords:
                    case Utilities.typeOfOutputEnum.DataRecords:
                    case Utilities.typeOfOutputEnum.PassThrough:
                    case Utilities.typeOfOutputEnum.ChildRecord:
                    case Utilities.typeOfOutputEnum.RowsProcessed:
                        break;
                    case Utilities.typeOfOutputEnum.MasterRecord:
                    case Utilities.typeOfOutputEnum.ChildMasterRecord:
                        if ((int)ManageProperties.GetPropertyValue(thisColumn.CustomPropertyCollection, ManageProperties.keyOutputColumnID) <= 0)
                        {
                            // Need to delete the "children"!
                            RemoveLinkedColumnFromOutputs(thisColumn);
                        }
                        else
                        {
                            throw new COMException(MessageStrings.CannotSetProperty, E_FAIL);
                        }
                        break;
                    default:
                        break;
                }
            }
            base.DeleteOutputColumn(outputID, outputColumnID);
        }
        #endregion

        #region SetOutputProperty
        /// <summary>
        /// Called when a custom property of an output object is set.
        /// Ensure that only valid values can be set.  Prevent Multiple Outputs of a specific type being created.
        /// </summary>
        /// <param name="outputID">The ID of the output.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="propertyValue">The value to assign to the property.</param>
        /// <returns>The custom property.</returns>
        public override IDTSCustomProperty SetOutputProperty(int outputID, string propertyName, object propertyValue)
        {
            IDTSOutput currentOutput = null;
            // Make sure that the the property is valid...
            if (this.propertyManager.ValidatePropertyValue(propertyName, propertyValue, DTSValidationStatus.VS_ISVALID) != DTSValidationStatus.VS_ISVALID)
            {
                throw new COMException(MessageStrings.InvalidPropertyValue(propertyName, propertyValue), E_FAIL);
            }
            else
            {
                currentOutput = this.ComponentMetaData.OutputCollection.FindObjectByID(outputID);
                Utilities.typeOfOutputEnum oldOutputType = (Utilities.typeOfOutputEnum)ManageProperties.GetPropertyValue(currentOutput.CustomPropertyCollection, ManageProperties.typeOfOutput);
                int oldMasterID = (int)ManageProperties.GetPropertyValue(currentOutput.CustomPropertyCollection, ManageProperties.masterRecordID);
                switch (oldOutputType)
                {
                    case Utilities.typeOfOutputEnum.ErrorRecords:
                        throw new COMException(MessageStrings.CantChangeOutputProperties("Error"), E_FAIL);
                    case Utilities.typeOfOutputEnum.PassThrough:
                        throw new COMException(MessageStrings.CantChangeOutputProperties("PassThrough"), E_FAIL);
                    case Utilities.typeOfOutputEnum.RowsProcessed:
                        throw new COMException(MessageStrings.CantChangeOutputProperties("RowsProcessed"), E_FAIL);
                    default:
                        break;
                }
                // If we are setting the masterRecordID, which points at the output that is the Master of this Child/ChildMaster...
                switch (propertyName)
                {
                    case ManageProperties.masterRecordID:
                    #region masterRecordID
                    switch (oldOutputType)
                    {
                        case Utilities.typeOfOutputEnum.KeyRecords:
                        case Utilities.typeOfOutputEnum.DataRecords:
                        case Utilities.typeOfOutputEnum.MasterRecord:
                            // You can NOT set the masterRecordID for the output types above...
                            throw new COMException(MessageStrings.CannotSetProperty, E_FAIL);
                        case Utilities.typeOfOutputEnum.ChildMasterRecord:
                        case Utilities.typeOfOutputEnum.ChildRecord:
                            if (oldMasterID != (int)propertyValue)
                            {
                                if (oldMasterID != -1)
                                {
                                    // Remove all the Master columns that are not -1's
                                    List<int> columnsToRemove = new List<int>();
                                    foreach (IDTSOutputColumn100 childColumn in currentOutput.OutputColumnCollection)
                                    {
                                        if (((int)ManageProperties.GetPropertyValue(childColumn.CustomPropertyCollection, ManageProperties.keyOutputColumnID) != -1)
                                        && ((Utilities.usageOfColumnEnum)ManageProperties.GetPropertyValue(childColumn.CustomPropertyCollection, ManageProperties.usageOfColumn) == Utilities.usageOfColumnEnum.MasterValue))
                                        {
                                            columnsToRemove.Add(childColumn.ID);
                                        }
                                    }
                                    foreach (int columnToRemoveID in columnsToRemove)
                                    {
                                        currentOutput.OutputColumnCollection.RemoveObjectByID(columnToRemoveID);
                                    }
                                }
                                // Add the "MasterValue" columns into this record
                                IDTSOutput masterOutput = null;
                                try
                                {
                                    masterOutput = this.ComponentMetaData.OutputCollection.FindObjectByID((int)propertyValue);
                                }
                                catch
                                {
                                    // If we couldn't find the output in the masterRecordID, then it's obviously wrong, so error out.
                                    throw new COMException(MessageStrings.InvalidPropertyValue(propertyName, propertyValue), E_FAIL);
                                }
                                if (masterOutput == null)
                                {
                                    // If we couldn't find the output in the masterRecordID, then it's obviously wrong, so error out.
                                    throw new COMException(MessageStrings.InvalidPropertyValue(propertyName, propertyValue), E_FAIL);
                                }
                                Utilities.typeOfOutputEnum masterType = (Utilities.typeOfOutputEnum)ManageProperties.GetPropertyValue(masterOutput.CustomPropertyCollection, ManageProperties.typeOfOutput);
                                if ((masterType == Utilities.typeOfOutputEnum.ChildMasterRecord) || (masterType == Utilities.typeOfOutputEnum.MasterRecord))
                                {
                                    int columnPosition = 0;
                                    foreach (IDTSOutputColumn outputColumn in masterOutput.OutputColumnCollection)
                                    {
                                        // increment the column position to avoid Key properties.
                                        if ((Utilities.usageOfColumnEnum)ManageProperties.GetPropertyValue(outputColumn.CustomPropertyCollection, ManageProperties.usageOfColumn) == Utilities.usageOfColumnEnum.Key)
                                        {
                                            columnPosition++;
                                        }
                                        else
                                        {
                                            // If it's a MasterValue, then it has to be propogated into this child.
                                            if ((Utilities.usageOfColumnEnum)ManageProperties.GetPropertyValue(outputColumn.CustomPropertyCollection, ManageProperties.usageOfColumn) == Utilities.usageOfColumnEnum.MasterValue)
                                            {
                                                IDTSOutputColumn newOutputColumn = currentOutput.OutputColumnCollection.NewAt(columnPosition);
                                                newOutputColumn.Name = outputColumn.Name;
                                                newOutputColumn.Description = MessageStrings.KeyColumnDescription;
                                                ManageProperties.AddOutputColumnProperties(newOutputColumn.CustomPropertyCollection);
                                                ManageProperties.SetPropertyValue(newOutputColumn.CustomPropertyCollection, ManageProperties.usageOfColumn, Utilities.usageOfColumnEnum.MasterValue);
                                                // Make sure that we point at the ORIGINAL column ID...
                                                if ((int)ManageProperties.GetPropertyValue(outputColumn.CustomPropertyCollection, ManageProperties.keyOutputColumnID) > 0)
                                                {
                                                    ManageProperties.SetPropertyValue(newOutputColumn.CustomPropertyCollection, ManageProperties.keyOutputColumnID, (int)ManageProperties.GetPropertyValue(outputColumn.CustomPropertyCollection, ManageProperties.keyOutputColumnID));
                                                }
                                                else
                                                {
                                                    ManageProperties.SetPropertyValue(newOutputColumn.CustomPropertyCollection, ManageProperties.keyOutputColumnID, outputColumn.LineageID);
                                                }
                                                newOutputColumn.SetDataTypeProperties(outputColumn.DataType, outputColumn.Length, outputColumn.Precision, outputColumn.Scale, outputColumn.CodePage);
                                                columnPosition++;
                                                // If WE are a ChildMaster, then we need to propogate the new columns to OUR children.
                                                // The UI Doesn't support this, but the "Advanced Editor" does...
                                                if ((Utilities.typeOfOutputEnum)ManageProperties.GetPropertyValue(currentOutput.CustomPropertyCollection, ManageProperties.typeOfOutput) == Utilities.typeOfOutputEnum.ChildMasterRecord)
                                                {
                                                    PushMasterValueColumnsToChildren(currentOutput, outputColumn.ID, outputColumn);
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    // You can only set the masterRecordID to a Master or MasterChild output...
                                    throw new COMException(MessageStrings.InvalidPropertyValue(propertyName, propertyValue), E_FAIL);
                                }
                            }
                            break;
                        default:
                            break;
                    }
                    #endregion
                        break;
                    case ManageProperties.typeOfOutput:
                    #region typeOfOutput
                    switch (oldOutputType)
                    {
                        case Utilities.typeOfOutputEnum.KeyRecords:
                            throw new COMException(MessageStrings.CantChangeOutputProperties("Key"), E_FAIL);
                        default:
                            break;
                    }
                    switch ((Utilities.typeOfOutputEnum)propertyValue)
                    {
                        case Utilities.typeOfOutputEnum.ErrorRecords:
                        case Utilities.typeOfOutputEnum.KeyRecords:
                        case Utilities.typeOfOutputEnum.PassThrough:
                        case Utilities.typeOfOutputEnum.RowsProcessed:
                            throw new COMException(MessageStrings.InvalidPropertyValue(propertyName, System.Enum.GetName(typeof(Utilities.typeOfOutputEnum), propertyValue)));
                        default:
                            break;
                    }
                    List<int> columnsNoLongerMaster = new List<int>();
                    List<int> columnsToDelete = new List<int>();
                    switch (oldOutputType)
                    {
                        case Utilities.typeOfOutputEnum.MasterRecord:
                            #region MasterRecord
                            switch ((Utilities.typeOfOutputEnum)propertyValue)
                            {
                                case Utilities.typeOfOutputEnum.MasterRecord:
                                case Utilities.typeOfOutputEnum.ChildMasterRecord:
                                    // Nothing to do in these cases.
                                    // The ChildMaster case will be caught when the masterRecordID is set.
                                    break;
                                case Utilities.typeOfOutputEnum.DataRecords:
                                case Utilities.typeOfOutputEnum.ChildRecord:
                                    // Have to Prevent if there are already Child Records!
                                    Boolean childRecords = false;
                                        foreach (IDTSOutput100 output in this.ComponentMetaData.OutputCollection)
                                        {
                                            if ((int)ManageProperties.GetPropertyValue(output.CustomPropertyCollection, ManageProperties.masterRecordID) == outputID)
                                            {
                                                childRecords = true;
                                                this.PostError(MessageStrings.MasterHasChild(currentOutput.Name, output.Name));
                                            }
                                        }
                                        if (childRecords)
                                        {
                                            throw new COMException(MessageStrings.ThereAreChildRecordsForMaster(currentOutput.Name), E_FAIL);
                                        }
                                    // Ok, so there weren't any children (Throw!)
                                    // Set any Master Columns to Passthrough
                                        foreach (IDTSOutputColumn100 outputColumn in currentOutput.OutputColumnCollection)
                                        {
                                            if ((Utilities.usageOfColumnEnum)ManageProperties.GetPropertyValue(outputColumn.CustomPropertyCollection, ManageProperties.usageOfColumn) == Utilities.usageOfColumnEnum.MasterValue)
                                            {
                                                columnsNoLongerMaster.Add(outputColumn.ID);
                                            }
                                        }
                                        foreach (int columnToChangeID in columnsNoLongerMaster)
                                        {
                                            this.SetOutputColumnProperty(outputID, columnToChangeID, ManageProperties.usageOfColumn, Utilities.usageOfColumnEnum.Passthrough);
                                        }
                                    break;
                                default:
                                    break;
                            }
                            #endregion
                            break;
                        case Utilities.typeOfOutputEnum.DataRecords:
                            // Nothing to do for this case!
                            break;
                        case Utilities.typeOfOutputEnum.ChildMasterRecord:
                            #region ChildMasterRecord
                            switch ((Utilities.typeOfOutputEnum)propertyValue)
                            {
                                case Utilities.typeOfOutputEnum.MasterRecord:
                                    // Need to remove the old Master's columns.
                                    // Set any Master Columns to Passthrough, or delete if they came from a Master up the tree.
                                    foreach (IDTSOutputColumn100 outputColumn in currentOutput.OutputColumnCollection)
                                    {
                                        if ((Utilities.usageOfColumnEnum)ManageProperties.GetPropertyValue(outputColumn.CustomPropertyCollection, ManageProperties.usageOfColumn) == Utilities.usageOfColumnEnum.MasterValue)
                                        {
                                            if ((int)ManageProperties.GetPropertyValue(outputColumn.CustomPropertyCollection, ManageProperties.keyOutputColumnID) != -1)
                                            {
                                                columnsToDelete.Add(outputColumn.ID);
                                            }
                                        }
                                    }
                                    foreach (int columnToDeleteID in columnsToDelete)
                                    {
                                        int keyValueToRemoveID = (int)ManageProperties.GetPropertyValue(currentOutput.OutputColumnCollection.GetObjectByID(columnToDeleteID).CustomPropertyCollection, ManageProperties.keyOutputColumnID);
                                        this.RemoveLinkedColumnFromChildOutputs(currentOutput.ID, keyValueToRemoveID);
                                        ManageProperties.SetPropertyValue(currentOutput.OutputColumnCollection.GetObjectByID(columnToDeleteID).CustomPropertyCollection, ManageProperties.usageOfColumn, Utilities.usageOfColumnEnum.Passthrough);
                                        this.DeleteOutputColumn(outputID, columnToDeleteID);
                                    } 
                                    ManageProperties.SetPropertyValue(currentOutput.CustomPropertyCollection, ManageProperties.masterRecordID, -1);
                                    break;
                                case Utilities.typeOfOutputEnum.ChildMasterRecord:
                                    // Nothing to do in this case.
                                    break;
                                case Utilities.typeOfOutputEnum.DataRecords:
                                    // Have to Prevent if there are already Child Records!
                                    foreach (IDTSOutput100 output in this.ComponentMetaData.OutputCollection)
                                    {
                                        if ((int)ManageProperties.GetPropertyValue(output.CustomPropertyCollection, ManageProperties.masterRecordID) == outputID)
                                        {
                                            throw new COMException(MessageStrings.ThereAreChildRecordsForMaster(output.Name), E_FAIL);
                                        }
                                    }
                                    // Ok, so there weren't any children (Throw!)
                                    // Set any Master Columns to Passthrough, or delete if they came from a Master up the tree.
                                    foreach (IDTSOutputColumn100 outputColumn in currentOutput.OutputColumnCollection)
                                    {
                                        if ((Utilities.usageOfColumnEnum)ManageProperties.GetPropertyValue(outputColumn.CustomPropertyCollection, ManageProperties.usageOfColumn) == Utilities.usageOfColumnEnum.MasterValue)
                                        {
                                            if ((int)ManageProperties.GetPropertyValue(outputColumn.CustomPropertyCollection, ManageProperties.keyOutputColumnID) == -1)
                                            {
                                                columnsNoLongerMaster.Add(outputColumn.ID);
                                            }
                                            else
                                            {
                                                columnsToDelete.Add(outputColumn.ID);
                                            }
                                        }
                                    }
                                    foreach (int columnToChangeID in columnsNoLongerMaster)
                                    {
                                        this.SetOutputColumnProperty(outputID, columnToChangeID, ManageProperties.usageOfColumn, Utilities.usageOfColumnEnum.Passthrough);
                                    }
                                    foreach (int columnToDeleteID in columnsToDelete)
                                    {
                                        RemoveLinkedColumnFromChildOutputs(currentOutput.ID, columnToDeleteID);
                                        ManageProperties.SetPropertyValue(currentOutput.OutputColumnCollection.GetObjectByID(columnToDeleteID).CustomPropertyCollection, ManageProperties.usageOfColumn, Utilities.usageOfColumnEnum.Passthrough);
                                        this.DeleteOutputColumn(outputID, columnToDeleteID);
                                    }
                                    ManageProperties.SetPropertyValue(currentOutput.CustomPropertyCollection, ManageProperties.masterRecordID, -1);
                                    break;
                                case Utilities.typeOfOutputEnum.ChildRecord:
                                    // Have to Prevent if there are already Child Records!
                                    foreach (IDTSOutput100 output in this.ComponentMetaData.OutputCollection)
                                    {
                                        if ((int)ManageProperties.GetPropertyValue(output.CustomPropertyCollection, ManageProperties.masterRecordID) == outputID)
                                        {
                                            throw new COMException(MessageStrings.ThereAreChildRecordsForMaster(output.Name), E_FAIL);
                                        }
                                    }
                                    // Ok, so there weren't any children (Throw!)
                                    // Set any Master Columns to Passthrough
                                    foreach (IDTSOutputColumn100 outputColumn in currentOutput.OutputColumnCollection)
                                    {
                                        if ((Utilities.usageOfColumnEnum)ManageProperties.GetPropertyValue(outputColumn.CustomPropertyCollection, ManageProperties.usageOfColumn) == Utilities.usageOfColumnEnum.MasterValue)
                                        {
                                            if ((int)ManageProperties.GetPropertyValue(outputColumn.CustomPropertyCollection, ManageProperties.keyOutputColumnID) == -1)
                                            {
                                                columnsNoLongerMaster.Add(outputColumn.ID);
                                            }
                                        }
                                    }
                                    foreach (int columnToChangeID in columnsNoLongerMaster)
                                    {
                                        this.SetOutputColumnProperty(outputID, columnToChangeID, ManageProperties.usageOfColumn, Utilities.usageOfColumnEnum.Passthrough);
                                    }
                                    break;
                                default:
                                    break;
                            }
                            #endregion
                            break;
                        case Utilities.typeOfOutputEnum.ChildRecord:
                            #region ChildRecord
                            switch ((Utilities.typeOfOutputEnum)propertyValue)
                            {
                                case Utilities.typeOfOutputEnum.MasterRecord:
                                case Utilities.typeOfOutputEnum.DataRecords:
                                    // Need to remove the MasterValues
                                    List<int> columnsToRemove = new List<int>();
                                    foreach (IDTSOutputColumn100 outputColumn in currentOutput.OutputColumnCollection)
                                    {
                                        Utilities.usageOfColumnEnum columnUsage = (Utilities.usageOfColumnEnum)ManageProperties.GetPropertyValue(outputColumn.CustomPropertyCollection, ManageProperties.usageOfColumn);
                                        if (columnUsage == Utilities.usageOfColumnEnum.MasterValue)
                                        {
                                            columnsToRemove.Add(outputColumn.ID);
                                        }
                                    }
                                    foreach (int columnID in columnsToRemove)
                                    {
                                        DeleteOutputColumn(currentOutput.ID, columnID);
                                    }
                                    ManageProperties.SetPropertyValue(currentOutput.CustomPropertyCollection, ManageProperties.masterRecordID, -1);
                                    break;
                                case Utilities.typeOfOutputEnum.ChildMasterRecord:
                                case Utilities.typeOfOutputEnum.ChildRecord:
                                    // Nothing to do for this case!
                                    break;
                                default:
                                    throw new COMException(MessageStrings.InvalidPropertyValue(propertyName, propertyValue));
                            }
                            #endregion
                            break;
                        default:
                            throw new COMException(MessageStrings.CannotSetProperty, E_FAIL);
                    }
                    #endregion
                        break;
                    case ManageProperties.rowTypeValue:
                        #region rowTypeValue
                        // You are allowed to set this!
                        // The places you can't have already thrown!
                        #endregion
                        break;
                    default:
                        throw new COMException(MessageStrings.CannotSetProperty, E_FAIL);
                }
                return base.SetOutputProperty(outputID, propertyName, propertyValue);
            }
        }
        #endregion

        #region DeleteOutput

        public override void DeleteOutput(int outputID)
        {
            IDTSOutput thisOutput = this.ComponentMetaData.OutputCollection.GetObjectByID(outputID);
            if (!thisOutput.IsErrorOut)
            {
                switch ((Utilities.typeOfOutputEnum)ManageProperties.GetPropertyValue(thisOutput.CustomPropertyCollection, ManageProperties.typeOfOutput))
                {
                    case Utilities.typeOfOutputEnum.ErrorRecords:
                        this.PostErrorAndThrow(MessageStrings.CannotDeleteErrorOutput);
                        break;
                    case Utilities.typeOfOutputEnum.KeyRecords:
                        this.PostErrorAndThrow(MessageStrings.CannotDeleteKeyOutput);
                        break;
                    case Utilities.typeOfOutputEnum.RowsProcessed:
                        this.PostErrorAndThrow(MessageStrings.CannotDeleteRowsProcessedOutput);
                        break;
                    case Utilities.typeOfOutputEnum.ChildRecord:
                    case Utilities.typeOfOutputEnum.DataRecords:
                        base.DeleteOutput(outputID);
                        break;
                    case Utilities.typeOfOutputEnum.PassThrough:
                        this.PostErrorAndThrow(MessageStrings.CannotDeletePassThroughOutput);
                        break;
                    case Utilities.typeOfOutputEnum.ChildMasterRecord:
                    case Utilities.typeOfOutputEnum.MasterRecord:
                        // Find all the Master Columns (owned by this output) in this output, and force them to be deleted from the children.
                        IDTSOutput100 currentOutput = this.ComponentMetaData.OutputCollection.GetObjectByID(outputID);
                        foreach(IDTSOutputColumn100 outputColumn in currentOutput.OutputColumnCollection)
                        {
                            if (((Utilities.usageOfColumnEnum)ManageProperties.GetPropertyValue(outputColumn.CustomPropertyCollection, ManageProperties.usageOfColumn) == Utilities.usageOfColumnEnum.MasterValue)
                             && ((int)ManageProperties.GetPropertyValue(outputColumn.CustomPropertyCollection, ManageProperties.keyOutputColumnID) == -1))
                                RemoveLinkedColumnFromOutputs(outputColumn);
                        }

                        // Walk the tree down and change Children to Data, and ChildMasters to Masters (or ChildMasters)...
                        List<int> outputsToEdit = new List<int>();
                        Boolean isChildMaster = ((Utilities.typeOfOutputEnum)ManageProperties.GetPropertyValue(thisOutput.CustomPropertyCollection, ManageProperties.typeOfOutput) == Utilities.typeOfOutputEnum.ChildMasterRecord);
                        int oldMasterID = (int)ManageProperties.GetPropertyValue(thisOutput.CustomPropertyCollection, ManageProperties.masterRecordID);
                        foreach (IDTSOutput100 output in this.ComponentMetaData.OutputCollection)
                        {
                            if ((int)ManageProperties.GetPropertyValue(output.CustomPropertyCollection, ManageProperties.masterRecordID) == outputID)
                            {
                                // This output points at the output to be deleted.
                                outputsToEdit.Add(output.ID);
                            }
                        }
                        foreach (int outputToChangeID in outputsToEdit)
                        {
                            IDTSOutput100 outputToChange = this.ComponentMetaData.OutputCollection.GetObjectByID(outputToChangeID);
                            if (isChildMaster)
                            {
                                ManageProperties.SetPropertyValue(outputToChange.CustomPropertyCollection, ManageProperties.masterRecordID, oldMasterID);
                            }
                            else
                            {
                                if ((Utilities.typeOfOutputEnum)ManageProperties.GetPropertyValue(outputToChange.CustomPropertyCollection, ManageProperties.typeOfOutput) == Utilities.typeOfOutputEnum.ChildMasterRecord)
                                {
                                    this.SetOutputProperty(outputToChangeID, ManageProperties.typeOfOutput, Utilities.typeOfOutputEnum.MasterRecord);
                                }
                                else
                                {
                                    this.SetOutputProperty(outputToChangeID, ManageProperties.typeOfOutput, Utilities.typeOfOutputEnum.DataRecords);
                                }
                            }
                        }
                        base.DeleteOutput(outputID);
                        break;
                    default:
                        break;
                }
            }
            else
                this.PostErrorAndThrow(MessageStrings.CannotDeleteErrorOutput);
        }

        #endregion

        #endregion

        #region RunTime

        #region Prime Ouputs
        public override void PrimeOutput(int outputs, int[] outputIDs, PipelineBuffer[] buffers)
        {
            PipelineBuffer errorBuffer = null;
            SSISOutput errorOutput = null;
            PipelineBuffer rowCountBuffer = null;
            PipelineBuffer passThroughBuffer = null;
            SSISOutput passThroughOutput = null;
            Boolean passThroughConnected = false;
            DTSRowDisposition passThoughDisposition = DTSRowDisposition.RD_NotUsed;
            Dictionary<int, Object> keyMasterValues = new Dictionary<int, object>();
            Dictionary<String, int> badRecords = new Dictionary<string, int>();
            Dictionary<String, SplitOutput> splitOutputs = new Dictionary<string, SplitOutput>();
            SplitOutput currentSplitOutput = null;
            String RowDataValue = string.Empty;
            String RowTypeValue = string.Empty;
            Boolean pbFireAgain = true;
            Boolean pbCancel = false;
            Int64 recordsRead = 0;
            Boolean rowCountConnected = false;
            Boolean keyRecordFailure = false;

            columnDelimter = (String)ManageProperties.GetPropertyValue(this.ComponentMetaData.CustomPropertyCollection, ManageProperties.columnDelimiter);

            foreach (IDTSOutput output in this.ComponentMetaData.OutputCollection)
            {
                switch ((Utilities.typeOfOutputEnum)ManageProperties.GetPropertyValue(output.CustomPropertyCollection, ManageProperties.typeOfOutput))
                {
                    case Utilities.typeOfOutputEnum.ErrorRecords:
                        for (int i = 0; i < outputs; i++)
                        {
                            if (outputIDs[i] == output.ID)
                            {
                                errorBuffer = buffers[i];
                                errorOutput = new SSISOutput(output, BufferManager);
                                break;
                            }
                        }
                        break;
                    case Utilities.typeOfOutputEnum.KeyRecords:
                        currentSplitOutput = new SplitOutput();
                        for (int i = 0; i < outputs; i++)
                        {
                            if (outputIDs[i] == output.ID)
                            {
                                currentSplitOutput.DataBuffer = buffers[i];
                                SSISOutput dataOutput = new SSISOutput(output, BufferManager);
                                currentSplitOutput.DataOutput = dataOutput;
                                foreach (SSISOutputColumn outputColumn in dataOutput.OutputColumnCollection)
                                {
                                    if (outputColumn.IsMasterOrKey)
                                    {
                                        keyMasterValues.Add(outputColumn.LineageID, null);
                                    }
                                }
                                SetupFileHelper(output, dataOutput, out currentSplitOutput.ClassBuffer, out currentSplitOutput.Engine);
                                break;
                            }
                        }
                        splitOutputs.Add((String)ManageProperties.GetPropertyValue(output.CustomPropertyCollection, ManageProperties.rowTypeValue), currentSplitOutput);
                        break;
                    case Utilities.typeOfOutputEnum.DataRecords:
                    case Utilities.typeOfOutputEnum.ChildRecord:
                        currentSplitOutput = new SplitOutput();
                        for (int i = 0; i < outputs; i++)
                        {
                            if (outputIDs[i] == output.ID)
                            {
                                currentSplitOutput.DataBuffer = buffers[i];
                                SSISOutput dataOutput = new SSISOutput(output, BufferManager);
                                currentSplitOutput.DataOutput = dataOutput;
                                SetupFileHelper(output, dataOutput, out currentSplitOutput.ClassBuffer, out currentSplitOutput.Engine);
                                break;
                            }
                        }
                        splitOutputs.Add((String)ManageProperties.GetPropertyValue(output.CustomPropertyCollection, ManageProperties.rowTypeValue), currentSplitOutput);
                        break;
                    case Utilities.typeOfOutputEnum.ChildMasterRecord:
                    case Utilities.typeOfOutputEnum.MasterRecord:
                        currentSplitOutput = new SplitOutput();
                        for (int i = 0; i < outputs; i++)
                        {
                            if (outputIDs[i] == output.ID)
                            {
                                currentSplitOutput.DataBuffer = buffers[i];
                                SSISOutput dataOutput = new SSISOutput(output, BufferManager);
                                currentSplitOutput.DataOutput = dataOutput;
                                foreach (SSISOutputColumn outputColumn in dataOutput.OutputColumnCollection)
                                {
                                    if (outputColumn.IsMasterOrKey)
                                    {
                                        keyMasterValues.Add(outputColumn.LineageID, null);
                                    }
                                }
                                SetupFileHelper(output, dataOutput, out currentSplitOutput.ClassBuffer, out currentSplitOutput.Engine);
                                break;
                            }
                        }
                        splitOutputs.Add((String)ManageProperties.GetPropertyValue(output.CustomPropertyCollection, ManageProperties.rowTypeValue), currentSplitOutput);
                        break;
                    case Utilities.typeOfOutputEnum.PassThrough:
                        for (int i = 0; i < outputs; i++)
                        {
                            if (outputIDs[i] == output.ID)
                            {
                                passThroughBuffer = buffers[i];
                                passThroughOutput = new SSISOutput(output, BufferManager);
                                passThroughConnected = true;
                                break;
                            }
                        }
                        passThoughDisposition = output.ErrorRowDisposition;
                        if (!passThroughConnected)
                        {
                            passThroughOutput = new SSISOutput(output, null);
                        }
                        break;
                    case Utilities.typeOfOutputEnum.RowsProcessed:
                        for (int i = 0; i < outputs; i++)
                        {
                            if (outputIDs[i] == output.ID)
                            {
                                rowCountBuffer = buffers[i];
                                rowCountConnected = true;
                                break;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

            ConnectionManager cm = Microsoft.SqlServer.Dts.Runtime.DtsConvert.GetWrapper(ComponentMetaData.RuntimeConnectionCollection[0].ConnectionManager);
            IDTSConnectionManagerFlatFile connectionFlatFile = cm.InnerObject as IDTSConnectionManagerFlatFile;

            bool firstRowColumnNames = connectionFlatFile.ColumnNamesInFirstDataRow;
            bool treatNulls = (bool)ManageProperties.GetPropertyValue(this.ComponentMetaData.CustomPropertyCollection, ManageProperties.treatEmptyStringsAsNull);
            columnDelimter = connectionFlatFile.Columns[0].ColumnDelimiter;

            String passThroughClassString = string.Empty;
            passThroughClassString = Utilities.DynamicClassStringFromOutput(passThroughOutput, firstRowColumnNames, connectionFlatFile.Columns[connectionFlatFile.Columns.Count - 1].ColumnDelimiter, connectionFlatFile.Columns[0].ColumnDelimiter);

            Type passThroughType = ClassBuilder.ClassFromString(passThroughClassString);
            foreach(SSISOutputColumn ssisColumn in passThroughOutput.OutputColumnCollection)
            {
                ssisColumn.FileHelperField = passThroughType.GetField(ssisColumn.Name);
            }
            System.Reflection.FieldInfo[] fieldList = passThroughType.GetFields();
            FileHelperAsyncEngine engine = new FileHelperAsyncEngine(passThroughType);
            engine.BeginReadFile(fileName);
            while (engine.ReadNext() != null)
            {
                recordsRead++;
                if (passThroughConnected)
                {
                    passThroughBuffer.AddRow();
                }
                foreach (SSISOutputColumn ssisColumn in passThroughOutput.OutputColumnCollection)
                {
                    if (passThroughConnected)
                    {
                        passThroughBuffer[ssisColumn.OutputBufferID] = ssisColumn.FileHelperField.GetValue(engine.LastRecord);
                    }
                    if (ssisColumn.IsRowData)
                    {
                        RowDataValue = (String)ssisColumn.FileHelperField.GetValue(engine.LastRecord);
                    }
                    else if (ssisColumn.IsRowType)
                    {
                        RowTypeValue = (String)ssisColumn.FileHelperField.GetValue(engine.LastRecord);
                    }
                }
                PipelineBuffer currentBuffer = null;
                if (splitOutputs.TryGetValue(RowTypeValue, out currentSplitOutput))
                {
                    if (currentSplitOutput.FoundInBuffers)
                    {
                        currentBuffer = currentSplitOutput.DataBuffer;
                        FileHelperEngine currentEngine = currentSplitOutput.Engine;
                        List<Object> parseResults = currentEngine.ReadStringAsList(RowDataValue);
                        SSISOutput currentOutput = currentSplitOutput.DataOutput;
                        Utilities.typeOfOutputEnum typeOfOutput = (Utilities.typeOfOutputEnum)ManageProperties.GetPropertyValue(currentOutput.CustomPropertyCollection, ManageProperties.typeOfOutput);
                        if (typeOfOutput == Utilities.typeOfOutputEnum.KeyRecords)
                        {
                            keyRecordFailure = false;
                            // Zap all the Saved Values they are now redundant.
                            foreach (int keyID in keyMasterValues.Keys.ToList())
                            {
                                keyMasterValues[keyID] = null;
                            }
                        }
                        if (currentEngine.ErrorManager.HasErrors)
                        {
                            if (typeOfOutput == Utilities.typeOfOutputEnum.KeyRecords)
                            {
                                keyRecordFailure = true;
                            }
                            switch (currentOutput.ErrorRowDisposition)
                            {
                                case DTSRowDisposition.RD_FailComponent:
                                    foreach (ErrorInfo err in currentEngine.ErrorManager.Errors)
                                    {
                                        this.ComponentMetaData.FireError(999, this.ComponentMetaData.Name, err.ExceptionInfo.Message, string.Empty, 0, out pbCancel);
                                    }
                                    throw new COMException(String.Format("Parsing Exception raised on or around input file line {0}.", recordsRead), E_FAIL);
                                case DTSRowDisposition.RD_IgnoreFailure:
                                    foreach (ErrorInfo err in currentEngine.ErrorManager.Errors)
                                    {
                                        this.ComponentMetaData.FireInformation(999, this.ComponentMetaData.Name, err.ExceptionInfo.Message, string.Empty, 0, ref pbFireAgain);
                                    }
                                    currentEngine.ErrorManager.ClearErrors();
                                    break;
                                case DTSRowDisposition.RD_NotUsed:
                                    currentEngine.ErrorManager.ClearErrors();
                                    break;
                                case DTSRowDisposition.RD_RedirectRow:
                                    foreach (ErrorInfo err in currentEngine.ErrorManager.Errors)
                                    {
                                        errorBuffer.AddRow();
                                        errorBuffer[2] = TruncateStringTo4000(String.Format("Output {0} had error {1}", currentOutput.Name, err.ExceptionInfo.Message));
                                        errorBuffer[4] = TruncateStringTo4000(err.RecordString);
                                        Object currentValue = null;
                                        foreach (SSISOutputColumn currentOutputColumn in errorOutput.OutputColumnCollection)
                                        {
                                            if (currentOutputColumn.CustomPropertyCollection.Count > 0)
                                            {
                                                if (keyMasterValues.TryGetValue((int)ManageProperties.GetPropertyValue(currentOutputColumn.CustomPropertyCollection, ManageProperties.keyOutputColumnID), out currentValue))
                                                {
                                                    errorBuffer[currentOutputColumn.OutputBufferID] = currentValue;
                                                }
                                            }
                                        }
                                    }
                                    currentEngine.ErrorManager.ClearErrors();
                                    break;
                                default:
                                    currentEngine.ErrorManager.ClearErrors();
                                    break;
                            }
                        }
                        else
                        {
                            if (keyRecordFailure)
                            {
                                foreach (Object result in parseResults)
                                {
                                    switch (currentOutput.ErrorRowDisposition)
                                    {
                                        case DTSRowDisposition.RD_FailComponent:
                                            this.ComponentMetaData.FireError(999, this.ComponentMetaData.Name, String.Format("Key Record Failed Before Record {0}.", recordsRead), string.Empty, 0, out pbCancel);
                                            throw new COMException(String.Format("Key Record Failed on or around input file line {0}.", recordsRead), E_FAIL);
                                        case DTSRowDisposition.RD_IgnoreFailure:
                                            this.ComponentMetaData.FireInformation(999, this.ComponentMetaData.Name, String.Format("Key Record Failed Before Record {0}.", recordsRead), string.Empty, 0, ref pbFireAgain);
                                            break;
                                        case DTSRowDisposition.RD_NotUsed:
                                            break;
                                        case DTSRowDisposition.RD_RedirectRow:
                                            errorBuffer.AddRow();
                                            errorBuffer[2] = TruncateStringTo4000(String.Format("Key Record Failed Before Record {0}.", recordsRead));
                                            errorBuffer[4] = TruncateStringTo4000(RowDataValue);
                                            break;
                                        default:
                                            break;
                                    }
                                    currentSplitOutput.IncrementRowCount();
                                }
                            }
                            else
                            {
                                foreach (Object result in parseResults)
                                {
                                    currentBuffer.AddRow();
                                    foreach (SSISOutputColumn currentOutputColumn in currentOutput.OutputColumnCollection)
                                    {
                                        Boolean errorFound = false;
                                        try
                                        {
                                            Object currentValue = null;
                                            if (currentOutputColumn.IsDerived)
                                            {
                                                if (keyMasterValues.TryGetValue((int)ManageProperties.GetPropertyValue(currentOutputColumn.CustomPropertyCollection, ManageProperties.keyOutputColumnID), out currentValue))
                                                {
                                                    currentBuffer[currentOutputColumn.OutputBufferID] = currentValue;
                                                }
                                            }
                                            else
                                            {
                                                currentValue = currentOutputColumn.FileHelperField.GetValue(result);
                                                if (currentValue != null)
                                                {
                                                    if (currentValue is String)
                                                    {
                                                        if (String.IsNullOrEmpty((String)currentValue))
                                                        {
                                                            if (!treatNulls)
                                                            {
                                                                currentBuffer[currentOutputColumn.OutputBufferID] = currentValue;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            currentBuffer[currentOutputColumn.OutputBufferID] = currentValue;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        currentBuffer[currentOutputColumn.OutputBufferID] = currentValue;
                                                    }
                                                }
                                                if (currentOutputColumn.IsMasterOrKey)
                                                {
                                                    keyMasterValues[currentOutputColumn.LineageID] = currentValue;
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            switch (currentOutput.ErrorRowDisposition)
                                            {
                                                case DTSRowDisposition.RD_FailComponent:
                                                    this.ComponentMetaData.FireError(999, this.ComponentMetaData.Name, String.Format("Exception {0} thrown on Record {1} for field {2}.", ex.Message, recordsRead, currentOutputColumn.Name), string.Empty, 0, out pbCancel);
                                                    throw new COMException(String.Format("Parsing Exception raised on or around input file line {0}.", recordsRead), E_FAIL);
                                                case DTSRowDisposition.RD_IgnoreFailure:
                                                    this.ComponentMetaData.FireInformation(999, this.ComponentMetaData.Name, String.Format("Exception {0} thrown on Record {1} for field {2}.", ex.Message, recordsRead, currentOutputColumn.Name), string.Empty, 0, ref pbFireAgain);
                                                    break;
                                                case DTSRowDisposition.RD_NotUsed:
                                                    break;
                                                case DTSRowDisposition.RD_RedirectRow:
                                                    errorBuffer.AddRow();
                                                    errorBuffer[2] = TruncateStringTo4000(String.Format("Exception {0} thrown on Record {1} for field {2}.", ex.Message, recordsRead, currentOutputColumn.Name));
                                                    errorBuffer[3] = TruncateStringTo4000(currentOutputColumn.FileHelperField.GetValue(result).ToString());
                                                    errorBuffer[4] = TruncateStringTo4000(RowDataValue);
                                                    Object currentValue = null;
                                                    foreach (SSISOutputColumn errorOutputColumn in errorOutput.OutputColumnCollection)
                                                    {
                                                        if (errorOutputColumn.CustomPropertyCollection.Count > 0)
                                                        {
                                                            if (keyMasterValues.TryGetValue((int)ManageProperties.GetPropertyValue(errorOutputColumn.CustomPropertyCollection, ManageProperties.keyOutputColumnID), out currentValue))
                                                            {
                                                                errorBuffer[errorOutputColumn.OutputBufferID] = currentValue;
                                                            }
                                                        }
                                                    }
                                                    currentBuffer.RemoveRow();
                                                    errorFound = true;
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                        if (errorFound)
                                            break;
                                    }
                                    currentSplitOutput.IncrementRowCount();
                                }
                            }
                        }
                    }
                    else
                    {
                        currentSplitOutput.IncrementRowCount();
                    }
                }
                else
                {
                    switch (passThoughDisposition)
                    {
                        case DTSRowDisposition.RD_FailComponent:
                            this.ComponentMetaData.FireError(999, this.ComponentMetaData.Name, String.Format("Unexpected Row Type Value {0} found on Record {1}.", RowTypeValue, recordsRead), string.Empty, 0, out pbCancel);
                            throw new COMException(String.Format("Parsing Exception raised on or around input file line {0}.", recordsRead), E_FAIL);
                        case DTSRowDisposition.RD_RedirectRow:
                            errorBuffer.AddRow();
                            errorBuffer[2] = TruncateStringTo4000(String.Format("Unexpected Row Type Value {0} found on Record {1}.", RowTypeValue, recordsRead));
                            errorBuffer[4] = TruncateStringTo4000(String.Format("{0}{1}{2}", RowTypeValue, columnDelimter, RowDataValue));
                            Object currentValue = null;
                            foreach (SSISOutputColumn currentOutputColumn in errorOutput.OutputColumnCollection)
                            {
                                if (currentOutputColumn.CustomPropertyCollection.Count > 0)
                                {
                                    if (keyMasterValues.TryGetValue((int)ManageProperties.GetPropertyValue(currentOutputColumn.CustomPropertyCollection, ManageProperties.keyOutputColumnID), out currentValue))
                                    {
                                        errorBuffer[currentOutputColumn.OutputBufferID] = currentValue;
                                    }
                                }
                            }
                            break;
                        case DTSRowDisposition.RD_IgnoreFailure:
                        case DTSRowDisposition.RD_NotUsed:
                        default:
                            break;
                    }
                    if (badRecords.ContainsKey(RowTypeValue))
                    {
                        badRecords[RowTypeValue]++;
                    }
                    else
                    {
                        int numberOfColumns = RowDataValue.Split(new String[] { (String)ManageProperties.GetPropertyValue(this.ComponentMetaData.CustomPropertyCollection, ManageProperties.columnDelimiter) }, StringSplitOptions.None).Count();
                        this.ComponentMetaData.FireWarning(0, this.ComponentMetaData.Name, String.Format("The RowType value of {0} was not expected and as {1} columns!", RowTypeValue, numberOfColumns), string.Empty, 0);
                        badRecords.Add(RowTypeValue, 1);
                    }
                }
            }

            if (rowCountConnected)
            {
                rowCountBuffer.AddRow();
                rowCountBuffer[0] = "Total Records";
                rowCountBuffer[1] = recordsRead;
                if (passThroughConnected)
                    rowCountBuffer[2] = MessageStrings.ConnectedAndProcessed;
                else
                    rowCountBuffer[2] = MessageStrings.Disconnected;

                foreach (KeyValuePair<String, SplitOutput> validRecords in splitOutputs)
                {
                    rowCountBuffer.AddRow();
                    rowCountBuffer[0] = validRecords.Key;
                    rowCountBuffer[1] = validRecords.Value.NumberOfRecords;
                    if (validRecords.Value.FoundInBuffers)
                    {
                        rowCountBuffer[2] = MessageStrings.ConnectedAndProcessed;
                    }
                    else
                    {
                        rowCountBuffer[2] = MessageStrings.Disconnected;
                    }
                }
                foreach (KeyValuePair<String, int> invalidRecords in badRecords)
                {
                    rowCountBuffer.AddRow();
                    rowCountBuffer[0] = invalidRecords.Key;
                    rowCountBuffer[1] = invalidRecords.Value;
                    rowCountBuffer[2] = MessageStrings.NotRecognised;
                }
            }

            foreach (PipelineBuffer buffer in buffers)
            {
                buffer.SetEndOfRowset();
            }

            foreach (KeyValuePair<String, int> badRecord in badRecords)
            {
                this.ComponentMetaData.FireInformation(0, this.ComponentMetaData.Name, String.Format("The Unexpected RowType value of {0} was found {1} times.", badRecord.Key, badRecord.Value), string.Empty, 0, ref pbFireAgain);
            }

            this.ComponentMetaData.FireInformation(0, this.ComponentMetaData.Name, String.Format("Total Number of records read is {0}.", recordsRead), string.Empty, 0, ref pbFireAgain);
        }
        #endregion

        #endregion

        #region Helpers

        private String TruncateStringTo4000(String inputData)
        {
            if (inputData.Length > 4000)
            {
                return inputData.Substring(0, 4000);
            }
            else
            {
                return inputData;
            }
        }

        private void SetupFileHelper(IDTSOutput output, SSISOutput dataOutput, out Type classBuffer, out FileHelperEngine engine)
        {
            String classString = Utilities.DynamicClassStringFromOutput(dataOutput, false, string.Empty, (String)ManageProperties.GetPropertyValue(this.ComponentMetaData.CustomPropertyCollection, ManageProperties.columnDelimiter));
            classBuffer = ClassBuilder.ClassFromString(classString);
            foreach (SSISOutputColumn ssisColumn in dataOutput.OutputColumnCollection)
            {
                ssisColumn.FileHelperField = classBuffer.GetField(ssisColumn.Name);
            }
            engine = new FileHelperEngine(classBuffer);
            engine.ErrorManager.ErrorMode = ErrorMode.SaveAndContinue;
        }

        private String ReplaceEscapes(String stringToCleanse)
        {
            String workString = stringToCleanse.Replace("\\", @"\");
            return workString.Replace("\n", @"\n").Replace("\a", @"\a").Replace("\b", @"\b").Replace("\f", @"\f").Replace("\r", @"\r").Replace("\t", @"\t").Replace("\v", @"\v").Replace("\'", @"\'");
        }

        private void PushMasterValueColumnsToChildren(IDTSOutput masterOutput, int outputColumnID, IDTSOutputColumn thisColumn)//int masterOutputID, IDTSOutputColumn masterOutputColumn)
        {
            int keyPosition = 0;
            for (int i = 0; i < masterOutput.OutputColumnCollection.Count; i++)
            {
                if (masterOutput.OutputColumnCollection[i].ID == outputColumnID)
                {
                    break;
                }
                else
                {
                    if (((Utilities.usageOfColumnEnum)ManageProperties.GetPropertyValue(masterOutput.OutputColumnCollection[i].CustomPropertyCollection, ManageProperties.usageOfColumn) == Utilities.usageOfColumnEnum.Key)
                    || ((Utilities.usageOfColumnEnum)ManageProperties.GetPropertyValue(masterOutput.OutputColumnCollection[i].CustomPropertyCollection, ManageProperties.usageOfColumn) == Utilities.usageOfColumnEnum.MasterValue))
                    {
                        keyPosition++;
                    }
                }
            }
            // Add column to the Child Outputs.
            int MasterOutputID = masterOutput.ID;
            foreach (IDTSOutput output in this.ComponentMetaData.OutputCollection)
            {
                // Only add to outputs that aren't me, or errors
                if ((output.ID != MasterOutputID) && (!output.IsErrorOut))
                {
                    IDTSOutputColumn outputColumn = null;
                    if ((((Utilities.typeOfOutputEnum)ManageProperties.GetPropertyValue(output.CustomPropertyCollection, ManageProperties.typeOfOutput) == Utilities.typeOfOutputEnum.ChildRecord)
                     || ((Utilities.typeOfOutputEnum)ManageProperties.GetPropertyValue(output.CustomPropertyCollection, ManageProperties.typeOfOutput) == Utilities.typeOfOutputEnum.ChildMasterRecord))
                    && ((int)ManageProperties.GetPropertyValue(output.CustomPropertyCollection, ManageProperties.masterRecordID) == MasterOutputID))
                    {
                        outputColumn = output.OutputColumnCollection.NewAt(keyPosition);
                        outputColumn.Name = thisColumn.Name;
                        outputColumn.Description = MessageStrings.KeyColumnDescription;
                        ManageProperties.AddOutputColumnProperties(outputColumn.CustomPropertyCollection);
                        ManageProperties.SetPropertyValue(outputColumn.CustomPropertyCollection, ManageProperties.usageOfColumn, Utilities.usageOfColumnEnum.MasterValue);
                        // Make sure that we point at the ORIGINAL column ID...
                        if ((int)ManageProperties.GetPropertyValue(outputColumn.CustomPropertyCollection, ManageProperties.keyOutputColumnID) > 0)
                        {
                            ManageProperties.SetPropertyValue(outputColumn.CustomPropertyCollection, ManageProperties.keyOutputColumnID, (int)ManageProperties.GetPropertyValue(thisColumn.CustomPropertyCollection, ManageProperties.keyOutputColumnID));
                        }
                        else
                        {
                            ManageProperties.SetPropertyValue(outputColumn.CustomPropertyCollection, ManageProperties.keyOutputColumnID, thisColumn.LineageID);
                        }
                        outputColumn.SetDataTypeProperties(thisColumn.DataType, thisColumn.Length, thisColumn.Precision, thisColumn.Scale, thisColumn.CodePage);
                        if ((Utilities.typeOfOutputEnum)ManageProperties.GetPropertyValue(output.CustomPropertyCollection, ManageProperties.typeOfOutput) == Utilities.typeOfOutputEnum.ChildMasterRecord)
                        {
                            PushMasterValueColumnsToChildren(output, outputColumn.ID, outputColumn);
                        }
                    }
                }
            }
        }

        private void RemoveLinkedColumnFromOutputs(IDTSOutputColumn thisColumn)
        {
            foreach (IDTSOutput output in this.ComponentMetaData.OutputCollection)
            {
                int IDToDelete = -1;
                foreach (IDTSOutputColumn outputColumn in output.OutputColumnCollection)
                {
                    if (outputColumn.CustomPropertyCollection.Count > 0)
                    {
                        if (thisColumn.LineageID == (int)ManageProperties.GetPropertyValue(outputColumn.CustomPropertyCollection, ManageProperties.keyOutputColumnID))
                        {
                            IDToDelete = outputColumn.ID;
                            if (((Utilities.typeOfOutputEnum)ManageProperties.GetPropertyValue(output.CustomPropertyCollection, ManageProperties.typeOfOutput) == Utilities.typeOfOutputEnum.MasterRecord)
                                || ((Utilities.typeOfOutputEnum)ManageProperties.GetPropertyValue(output.CustomPropertyCollection, ManageProperties.typeOfOutput) == Utilities.typeOfOutputEnum.ChildMasterRecord))
                            {
                                RemoveLinkedColumnFromOutputs(outputColumn);
                            }
                            break;
                        }
                    }
                }
                if (IDToDelete != -1)
                {
                    output.OutputColumnCollection.RemoveObjectByID(IDToDelete);
                }
            }
        }


        /// <summary>
        /// Removes the child columns that are below the passed in parentOutputID
        /// </summary>
        /// <param name="parentOutputID"></param>
        /// <param name="keyColumnID"></param>
        private void RemoveLinkedColumnFromChildOutputs(int parentOutputID, int keyColumnID)
        {
            foreach (IDTSOutput100 output in this.ComponentMetaData.OutputCollection)
            {
                int masterID = (int)ManageProperties.GetPropertyValue(output.CustomPropertyCollection, ManageProperties.masterRecordID);
                if (masterID == parentOutputID)
                {
                    if ((Utilities.typeOfOutputEnum)ManageProperties.GetPropertyValue(output.CustomPropertyCollection, ManageProperties.typeOfOutput) == Utilities.typeOfOutputEnum.ChildMasterRecord)
                    {
                        RemoveLinkedColumnFromChildOutputs(output.ID, keyColumnID);
                    }
                    int columnToRemove = -1;
                    foreach (IDTSOutputColumn100 outputColumn in output.OutputColumnCollection)
                    {
                        if ((int)ManageProperties.GetPropertyValue(outputColumn.CustomPropertyCollection, ManageProperties.keyOutputColumnID) == keyColumnID)
                        {
                            columnToRemove = outputColumn.ID;
                            break;
                        }
                    }
                    if (columnToRemove != -1)
                    {
                        output.OutputColumnCollection.RemoveObjectByID(columnToRemove);
                    }
                }
            }
        }

        private System.Text.Encoding GetEncoding()
        {
            ConnectionManager cm = Microsoft.SqlServer.Dts.Runtime.DtsConvert.GetWrapper(ComponentMetaData.RuntimeConnectionCollection[0].ConnectionManager);
            IDTSConnectionManagerFlatFile connectionFlatFile = cm.InnerObject as IDTSConnectionManagerFlatFile;

            bool unicode = connectionFlatFile.Unicode;
            int codePage = connectionFlatFile.CodePage;

            System.Text.Encoding encoding = unicode ? System.Text.Encoding.Unicode : System.Text.Encoding.ASCII;

            if (!unicode && codePage > 0)
            {
                encoding = System.Text.Encoding.GetEncoding(codePage);
            }

            return encoding;
        }

        #endregion

        #region Handlers
        private void PostWarning(string warningMessage)
        {
            ComponentMetaData.FireWarning(0, ComponentMetaData.Name, warningMessage, string.Empty, 0);
        }

        private void PostError(string errorMessage)
        {
            bool cancelled;
            ComponentMetaData.FireError(E_FAIL, ComponentMetaData.Name, errorMessage, string.Empty, 0, out cancelled);
        }

        private void PostErrorAndThrow(string errorMessage)
        {
            PostError(errorMessage);
            throw new System.Runtime.InteropServices.COMException(errorMessage, E_FAIL);
        }
        #endregion    
    }
}
