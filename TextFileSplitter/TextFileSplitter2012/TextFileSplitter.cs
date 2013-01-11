﻿#region Usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Dts.Pipeline;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime.Wrapper;
using System.Runtime.InteropServices;
using Microsoft.SqlServer.Dts.Runtime;

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
#endregion


namespace Martin.SQLServer.Dts
{
    [DtsPipelineComponent(DisplayName = "Text File Splitter Source",
        CurrentVersion = 1 // NB. Keep this in sync with ProvideCustomProperties and PerformUpgrade.
        , Description = "Extract many outputs from a single Text File"
        , IconResource = "Martin.SQLServer.Dts.Resources.TextFileSplitter.ico"
        , ComponentType = ComponentType.SourceAdapter)]
    public class TextFileSplitter : PipelineComponent
    {

        #region Globals
        ManageProperties propertyManager = new ManageProperties();
        const int E_FAIL = unchecked((int)0x80004005);
        private String fileName = String.Empty;
        private int passthroughOutputID = -1;
        private int errorOutputID = -1;
        private int keyRecordOutputID = -1;
        private Dictionary<String, int> dataOutputIDs = new Dictionary<string, int>();
        private string columnDelimter = string.Empty;
        #endregion

        #region Design Time

        #region Provide Component Poperties

        public override void ProvideComponentProperties()
        {
            this.RemoveAllInputsOutputsAndCustomProperties();
            this.ComponentMetaData.Version = 1;  // NB.  Always keep this in sync with the CurrentVersion!!!
            this.ComponentMetaData.UsesDispositions = true;
            this.ComponentMetaData.ContactInfo = "http://TextFileSplitter.codeplex.com/";
            ManageProperties.AddComponentProperties(this.ComponentMetaData.CustomPropertyCollection);

            // PassThrough Record Output
            IDTSOutput output = this.ComponentMetaData.OutputCollection.New();
            output.Name = MessageStrings.PassthroughOutputName;
            output.TruncationRowDisposition = DTSRowDisposition.RD_FailComponent;
            output.ErrorOrTruncationOperation = MessageStrings.RowLevelTruncationOperation;
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
            keyRecords.TruncationRowDisposition = DTSRowDisposition.RD_FailComponent;
            keyRecords.ErrorOrTruncationOperation = MessageStrings.RowLevelTruncationOperation;
            keyRecords.SynchronousInputID = 0;
            ManageProperties.AddOutputProperties(keyRecords.CustomPropertyCollection);
            ManageProperties.SetPropertyValue(keyRecords.CustomPropertyCollection, ManageProperties.typeOfOutput, Utilities.typeOfOutputEnum.KeyRecords);

            // Reserve space for the file connection.
            IDTSRuntimeConnection connectionSlot = this.ComponentMetaData.RuntimeConnectionCollection.New();
            connectionSlot.Name = MessageStrings.FileConnectionName;
            connectionSlot.Description = MessageStrings.FileConnectionDescription;
        }

        #endregion

        #region Perform Upgrade

        public override void PerformUpgrade(int pipelineVersion)
        {
            this.ComponentMetaData.Version = 1;  // NB.  Always keep this in sync with the CurrentVersion!!!
            base.PerformUpgrade(pipelineVersion);
        }

        #endregion

        #region Validate

        public override DTSValidationStatus Validate()
        {
            DTSValidationStatus status = DTSValidationStatus.VS_ISVALID;
            findOutputIDs();
            status = ValidateComponentProperties(status);
            status = ValidateOutputs(status);
            return status;
        }

        private DTSValidationStatus ValidateComponentProperties(DTSValidationStatus oldStatus)
        {
            DTSValidationStatus returnStatus = oldStatus;
            returnStatus = ManageProperties.ValidateComponentProperties(this.ComponentMetaData.CustomPropertyCollection, oldStatus);
            returnStatus =  this.propertyManager.ValidateProperties(this.ComponentMetaData.CustomPropertyCollection, returnStatus);
            return returnStatus;
        }

        private DTSValidationStatus ValidateOutputs(DTSValidationStatus oldStatus)
        {
            DTSValidationStatus returnStatus = oldStatus;
            if (this.ComponentMetaData.OutputCollection.Count < 3)
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
                foreach (IDTSOutput output in outputCollection)
                {
                    if (output.IsErrorOut)
                    {
                        returnStatus = ValidateErrorOutput(output, returnStatus);
                        errorOutputs++;
                    }
                    else
                    {
                        returnStatus = ValidateRegularOutput(output, returnStatus, ref passThroughOutputs, ref keyOutputs);
                    }
                }

                if (errorOutputs != 1)
                {
                    this.PostError(MessageStrings.NoErrorOutput);
                    returnStatus = Utilities.CompareValidationValues(returnStatus, DTSValidationStatus.VS_ISBROKEN);
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
            }

            return returnStatus;
        }

        private DTSValidationStatus ValidateRegularOutput(IDTSOutput output, DTSValidationStatus oldStatus, ref int passThoughOutputs, ref int keyOutputs)
        {
            DTSValidationStatus returnStatus = oldStatus;

            IDTSOutputColumnCollection outputColumnCollection = output.OutputColumnCollection;

            returnStatus = ManageProperties.ValidateOutputProperties(output.CustomPropertyCollection, returnStatus);
            if (returnStatus != oldStatus)
            {
                this.PostError(MessageStrings.APropertyIsMissing(output.Name));
                return returnStatus;
            }
            else
            {
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
                        break;
                    case Utilities.typeOfOutputEnum.PassThrough:
                        passThoughOutputs++;
                        returnStatus = ValidateExternalMetaData(output, returnStatus);
                        break;
                    default:
                        this.PostError(MessageStrings.InvalidOutputType(output.Name, "Unknown"));
                        returnStatus  =  Utilities.CompareValidationValues(returnStatus, DTSValidationStatus.VS_ISBROKEN);
                        break;
                }
            }

            if (output.SynchronousInputID != 0)
            {
                this.PostError(MessageStrings.OutputIsSyncronous);
                returnStatus = Utilities.CompareValidationValues(returnStatus, DTSValidationStatus.VS_ISBROKEN);
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

            if (outputColumnCollection.Count != 5)
            {
                this.PostError(MessageStrings.ErrorOutputColumnsAreMissing);
                returnStatus = Utilities.CompareValidationValues(returnStatus, DTSValidationStatus.VS_ISCORRUPT);
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
            returnStatus = ManageProperties.ValidateOutputColumnProperties(outputColumn.CustomPropertyCollection, returnStatus);
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
                return DTSValidationStatus.VS_ISCORRUPT;
            }
            else
            {
                return DTSValidationStatus.VS_ISVALID;
            }
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
                        ManageColumns.SetOutputColumnDefaults(outColumn);
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
                    fileName = cmFlatFile.ConnectionString;
                }
            }
        }
        #endregion

        #region Release Connections
        public override void ReleaseConnections()
        {
            fileName = String.Empty;
        }
        #endregion

        #region Set Component Property
        public override IDTSCustomProperty SetComponentProperty(string propertyName, object propertyValue)
        {
            return base.SetComponentProperty(propertyName, propertyValue);
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
            thisOutput.TruncationRowDisposition = DTSRowDisposition.RD_FailComponent;
            thisOutput.ErrorRowDisposition = DTSRowDisposition.RD_FailComponent;
            thisOutput.ErrorOrTruncationOperation = MessageStrings.RowLevelTruncationOperation;

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
                            ManageProperties.SetContainsLineage(outputColumn.CustomPropertyCollection, ManageProperties.keyOutputColumnID, true);
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
                IDTSOutputColumn col = base.InsertOutputColumnAt(outputID, outputColumnIndex, name, description);
                ManageProperties.AddOutputColumnProperties(col.CustomPropertyCollection);
                col.ErrorOrTruncationOperation = MessageStrings.ColumnLevelErrorTruncationOperation;
                col.ErrorRowDisposition = DTSRowDisposition.RD_FailComponent;
                col.TruncationRowDisposition = DTSRowDisposition.RD_FailComponent;
                col.SetDataTypeProperties(DataType.DT_STR, 50, 0, 0, 1252);
                return col;
            }
            else
            {
                this.PostErrorAndThrow(MessageStrings.CantChangeErrorOutputProperties);
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
                // We need to add/remove the key columns to the outputs...
                if (propertyName == ManageProperties.usageOfColumn)
                {
                    IDTSOutput thisOutput = this.ComponentMetaData.OutputCollection.GetObjectByID(outputID);
                    IDTSOutputColumn thisColumn = thisOutput.OutputColumnCollection.GetObjectByID(outputColumnID);
                    switch ((Utilities.typeOfOutputEnum)ManageProperties.GetPropertyValue(thisOutput.CustomPropertyCollection, ManageProperties.typeOfOutput))
                    {
                        case Utilities.typeOfOutputEnum.ErrorRecords:
                            break;
                        case Utilities.typeOfOutputEnum.KeyRecords:
                            #region KeyRecords
                            // The Key value can be used here, BUT the column must be added to the other outputs!
                            Utilities.usageOfColumnEnum oldValue = (Utilities.usageOfColumnEnum)ManageProperties.GetPropertyValue(thisColumn.CustomPropertyCollection, ManageProperties.usageOfColumn);
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
                                    // Add column to the other Non Error Outputs.
                                    foreach (IDTSOutput output in this.ComponentMetaData.OutputCollection)
                                    {
                                        if (!output.IsErrorOut)
                                        {
                                            if ((Utilities.typeOfOutputEnum)ManageProperties.GetPropertyValue(output.CustomPropertyCollection, ManageProperties.typeOfOutput) == Utilities.typeOfOutputEnum.DataRecords)
                                            {
                                                IDTSOutputColumn outputColumn = output.OutputColumnCollection.NewAt(keyPosition);
                                                outputColumn.Name = thisColumn.Name;
                                                outputColumn.Description = MessageStrings.KeyColumnDescription;
                                                ManageProperties.AddOutputColumnProperties(outputColumn.CustomPropertyCollection);
                                                ManageProperties.SetPropertyValue(outputColumn.CustomPropertyCollection, ManageProperties.usageOfColumn, Utilities.usageOfColumnEnum.Key);
                                                ManageProperties.SetPropertyValue(outputColumn.CustomPropertyCollection, ManageProperties.keyOutputColumnID, thisColumn.LineageID);
                                                ManageProperties.SetContainsLineage(outputColumn.CustomPropertyCollection, ManageProperties.keyOutputColumnID, true);
                                                outputColumn.SetDataTypeProperties(thisColumn.DataType, thisColumn.Length, thisColumn.Precision, thisColumn.Scale, thisColumn.CodePage);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    // Remove this column from the outputs
                                    foreach (IDTSOutput output in this.ComponentMetaData.OutputCollection)
                                    {
                                        if (!output.IsErrorOut)
                                        {
                                            if ((Utilities.typeOfOutputEnum)ManageProperties.GetPropertyValue(output.CustomPropertyCollection, ManageProperties.typeOfOutput) == Utilities.typeOfOutputEnum.DataRecords)
                                            {
                                                int IDToDelete = -1;
                                                foreach (IDTSOutputColumn outputColumn in output.OutputColumnCollection)
                                                {
                                                    if (thisColumn.LineageID == (int)ManageProperties.GetPropertyValue(outputColumn.CustomPropertyCollection, ManageProperties.keyOutputColumnID))
                                                    {
                                                        IDToDelete = outputColumn.ID;
                                                    }
                                                }
                                                if (IDToDelete != -1)
                                                {
                                                    output.OutputColumnCollection.RemoveObjectByID(IDToDelete);
                                                }
                                            }
                                        }
                                    }
                                }
                            } 
                            #endregion
                            break;
                        case Utilities.typeOfOutputEnum.DataRecords:
                            switch ((Utilities.usageOfColumnEnum) propertyValue)
	                        {
                                case Utilities.usageOfColumnEnum.RowType:
                                    throw new COMException(MessageStrings.CannotSetPropertyToKey, E_FAIL);
                                case Utilities.usageOfColumnEnum.RowData:
                                    throw new COMException(MessageStrings.CannotSetPropertyToKey, E_FAIL);
                                case Utilities.usageOfColumnEnum.Passthrough:
                                    break;
                                case Utilities.usageOfColumnEnum.Key:
                                    throw new COMException(MessageStrings.CannotSetPropertyToKey, E_FAIL);
                                case Utilities.usageOfColumnEnum.Ignore:
                                    break;
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
                                    break;
                                case Utilities.usageOfColumnEnum.Passthrough:
                                    throw new COMException(MessageStrings.CannotSetPropertyToKey, E_FAIL);
                                case Utilities.usageOfColumnEnum.Key:
                                    throw new COMException(MessageStrings.CannotSetPropertyToKey, E_FAIL);
                                case Utilities.usageOfColumnEnum.Ignore:
                                    break;
                                default:
                                    break;
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
                    this.PostErrorAndThrow(MessageStrings.CantChangeErrorOutputProperties);
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
                        if (ValidateSupportedDataTypes(eDataType) == DTSValidationStatus.VS_ISVALID)
                        {
                            thisColumn.SetDataTypeProperties(eDataType, iLength, iPrecision, iScale, iCodePage);
                            if ((Utilities.usageOfColumnEnum)ManageProperties.GetPropertyValue(thisColumn.CustomPropertyCollection, ManageProperties.usageOfColumn) == Utilities.usageOfColumnEnum.Key)
                            {
                                // Need to set the "children"!
                                foreach (IDTSOutput output in this.ComponentMetaData.OutputCollection)
                                {
                                    if (!output.IsErrorOut)
                                    {
                                        if ((Utilities.typeOfOutputEnum)ManageProperties.GetPropertyValue(output.CustomPropertyCollection, ManageProperties.typeOfOutput) == Utilities.typeOfOutputEnum.DataRecords)
                                        {
                                            foreach (IDTSOutputColumn outputColumn in output.OutputColumnCollection)
                                            {
                                                if (thisColumn.LineageID == (int)ManageProperties.GetPropertyValue(outputColumn.CustomPropertyCollection, ManageProperties.keyOutputColumnID))
                                                {
                                                    outputColumn.SetDataTypeProperties(eDataType, iLength, iPrecision, iScale, iCodePage);
                                                }
                                            }
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
            if (thisOutput != null)
            {
                if (thisOutput.IsErrorOut)
                    this.PostErrorAndThrow(MessageStrings.CantChangeErrorOutputProperties);
                else
                {
                    IDTSOutputColumn100 thisColumn = thisOutput.OutputColumnCollection.GetObjectByID(outputColumnID);
                    if (thisColumn != null)
                    {
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
                    }
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
            if (propertyName == ManageProperties.typeOfOutput)
            {
                // Do not allow the typeOfOutput to be changed.
                throw new COMException(MessageStrings.CannotSetProperty, E_FAIL);
                //switch ((Utilities.typeOfOutputEnum)propertyValue)
                //{
                //    case Utilities.typeOfOutputEnum.ErrorRecords:
                //        if (!this.ComponentMetaData.OutputCollection.GetObjectByID(outputID).IsErrorOut)
                //        {
                //            throw new COMException(MessageStrings.CannotSetProperty, E_FAIL);
                //        }
                //        break;
                //    case Utilities.typeOfOutputEnum.KeyRecords:
                //        int keyOutputCount = 0;
                //        foreach (IDTSOutput output in this.ComponentMetaData.OutputCollection)
                //        {
                //            if ((Utilities.typeOfOutputEnum)ManageProperties.GetPropertyValue(output.CustomPropertyCollection, ManageProperties.typeOfOutput) == Utilities.typeOfOutputEnum.KeyRecords)
                //            { keyOutputCount++; }
                //        }
                //        if (keyOutputCount > 0)
                //        {
                //            throw new COMException(MessageStrings.CannotSetProperty, E_FAIL);
                //        }
                //        break;
                //    case Utilities.typeOfOutputEnum.DataRecords:
                //        break;
                //    case Utilities.typeOfOutputEnum.PassThrough:
                //        int passthroughOutputCount = 0;
                //        foreach (IDTSOutput output in this.ComponentMetaData.OutputCollection)
                //        {
                //            if ((Utilities.typeOfOutputEnum)ManageProperties.GetPropertyValue(output.CustomPropertyCollection, ManageProperties.typeOfOutput) == Utilities.typeOfOutputEnum.PassThrough)
                //            { passthroughOutputCount++; }
                //        }
                //        if (passthroughOutputCount > 0)
                //        {
                //            throw new COMException(MessageStrings.CannotSetProperty, E_FAIL);
                //        }
                //        break;
                //    default:
                //        break;
                //}
            }
            return base.SetOutputProperty(outputID, propertyName, propertyValue);
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
                    case Utilities.typeOfOutputEnum.DataRecords:
                        base.DeleteOutput(outputID);
                        break;
                    case Utilities.typeOfOutputEnum.PassThrough:
                        this.PostErrorAndThrow(MessageStrings.CannotDeletePassThroughOutput);
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

        private StringData stringData;
        private StringParser stringParser;
        private Dictionary<int, String> keyValues;


        #region Pre Execute
        public override void PreExecute()
        {
            base.PreExecute();
            Boolean isTextDelimited = (Boolean)ManageProperties.GetPropertyValue(this.ComponentMetaData.CustomPropertyCollection, ManageProperties.isTextDelmited);
            String textDelimiter = (String)ManageProperties.GetPropertyValue(this.ComponentMetaData.CustomPropertyCollection, ManageProperties.textDelmiter);
            String columnDelimiter = (String)ManageProperties.GetPropertyValue(this.ComponentMetaData.CustomPropertyCollection, ManageProperties.columnDelimiter);
            findOutputIDs();
            stringData = new StringData();
            stringParser = null;
            if (isTextDelimited)
            {
                stringParser = new StringParser(columnDelimiter, textDelimiter);
            }
            else
            {
                stringParser = new StringParser(columnDelimiter, string.Empty);
            }
            keyValues = new Dictionary<int, string>();
        }
        #endregion

        #region Prime Ouputs
        public override void PrimeOutput(int outputs, int[] outputIDs, PipelineBuffer[] buffers)
        {
            //base.PrimeOutput(outputs, outputIDs, buffers);
            PipelineBuffer errorBuffer = null;
            PipelineBuffer keyRecordBuffer = null;
            PipelineBuffer passThroughBuffer = null;
            Dictionary<String, PipelineBuffer> dataBuffers = new Dictionary<string, PipelineBuffer>();
            Dictionary<String, IDTSOutput> dataOutputs = new Dictionary<string, IDTSOutput>();
            int rowTypeColumnID = -1;
            int rowDataColumnID = -1;
            String keyValue = string.Empty;
            IDTSOutput keyOutput = null;

            IDTSOutput passThroughOutput = null;

            int errorOutID = 0;
            int errorOutIndex = 0;
            GetErrorOutputInfo(ref errorOutID, ref errorOutIndex);

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
                                break;
                            }
                        }
                        break;
                    case Utilities.typeOfOutputEnum.KeyRecords:
                        for (int i = 0; i < outputs; i++)
                        {
                            if (outputIDs[i] == output.ID)
                            {
                                keyRecordBuffer = buffers[i];
                                keyValue = (String)ManageProperties.GetPropertyValue(output.CustomPropertyCollection, ManageProperties.rowTypeValue);
                                keyOutput = output;
                                break;
                            }
                        }
                        break;
                    case Utilities.typeOfOutputEnum.DataRecords:
                        for (int i = 0; i < outputs; i++)
                        {
                            if (outputIDs[i] == output.ID)
                            {
                                dataBuffers.Add((String)ManageProperties.GetPropertyValue(output.CustomPropertyCollection, ManageProperties.rowTypeValue), buffers[i]);
                                dataOutputs.Add((String)ManageProperties.GetPropertyValue(output.CustomPropertyCollection, ManageProperties.rowTypeValue), output);
                                break;
                            }
                        }
                        
                        break;
                    case Utilities.typeOfOutputEnum.PassThrough:
                        for (int i = 0; i < outputs; i++)
                        {
                            if (outputIDs[i] == output.ID)
                            {
                                passThroughBuffer = buffers[i];
                                passThroughOutput = output;
                                break;
                            }
                        }
                        // Locate the RowType Column.
                        for (int i = 0; i < output.OutputColumnCollection.Count; i++)
                        {
                            switch ((Utilities.usageOfColumnEnum)ManageProperties.GetPropertyValue(output.OutputColumnCollection[i].CustomPropertyCollection, ManageProperties.usageOfColumn))
                            {
                                case Utilities.usageOfColumnEnum.RowType:
                                    rowTypeColumnID = i;
                                    break;
                                case Utilities.usageOfColumnEnum.RowData:
                                    rowDataColumnID = i;
                                    break;
                                case Utilities.usageOfColumnEnum.Passthrough:
                                    break;
                                case Utilities.usageOfColumnEnum.Key:
                                    break;
                                case Utilities.usageOfColumnEnum.Ignore:
                                    break;
                                default:
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

            bool firstRowColumnNames = connectionFlatFile.ColumnNamesInFirstDataRow; //(bool)this.GetComponentPropertyValue(ManageProperties.ColumnNamesInFirstRowPropName);
            bool treatNulls = (bool)ManageProperties.GetPropertyValue(this.ComponentMetaData.CustomPropertyCollection, ManageProperties.treatEmptyStringsAsNull);
            columnDelimter = connectionFlatFile.Columns[0].ColumnDelimiter;

            FileReader reader = new FileReader(this.fileName, this.GetEncoding());
            DelimitedFileParser parser = this.CreateParser();
            ComponentBufferService passThroughBufferService = new ComponentBufferService(passThroughBuffer, errorBuffer);
            BufferSink passThroughBufferSink = new BufferSink(passThroughBufferService, passThroughOutput, treatNulls, parser.ColumnDelimiter, true);

            ComponentBufferService keyBufferService = new ComponentBufferService(keyRecordBuffer, errorBuffer);
            BufferSink keyBufferSink = new BufferSink(keyBufferService, keyOutput, treatNulls, parser.ColumnDelimiter, true);

            Dictionary<String, ComponentBufferService> dataBufferServices = new Dictionary<string, ComponentBufferService>();
            Dictionary<String, BufferSink> dataBufferSinks = new Dictionary<string, BufferSink>();

            foreach (string key in dataBuffers.Keys)
            {
                PipelineBuffer workingBuffer = null;
                IDTSOutput output = null;
                dataBuffers.TryGetValue(key, out workingBuffer);
                dataOutputs.TryGetValue(key, out output);
                ComponentBufferService dataCBS = new ComponentBufferService(workingBuffer, errorBuffer);
                BufferSink dataBS = new BufferSink(dataCBS, output, treatNulls, (string)ManageProperties.GetPropertyValue(this.ComponentMetaData.CustomPropertyCollection, ManageProperties.columnDelimiter), false);
                dataBufferServices.Add(key, dataCBS);
                dataBufferSinks.Add(key, dataBS);
            }

            passThroughBufferSink.CurrentRowCount = parser.HeaderRowsToSkip + parser.DataRowsToSkip + (firstRowColumnNames ? 1 : 0);
            int currentRowCount = parser.HeaderRowsToSkip + parser.DataRowsToSkip + (firstRowColumnNames ? 1 : 0);
            try
            {
                parser.SkipInitialRows(reader);

                RowData rowData = new RowData();
                while (!reader.IsEOF)
                {
                    parser.ParseNextRow(reader, rowData);
                    if (rowData.ColumnCount == 0)
                    {
                        // Last row with no data will be ignored.
                        break;
                    }
                    ConcatenateRowOverflow(ref rowData, passThroughBuffer.ColumnCount);
                    // Add record to PassThrough
                    passThroughBufferSink.AddRow(rowData);

                    if (rowData.GetColumnData(rowTypeColumnID) == keyValue)
                    {
                        keyValues.Clear();

                        StringData columnData = new StringData();
                        StringAsRowReader columnReader = new StringAsRowReader(rowData.GetColumnData(rowDataColumnID));
                        stringParser.ParseRow(columnReader, columnData);
                        RowData dataRowData = new RowData();

                        for (int i = 0; i < columnData.ColumnCount; i++)
                        {
                            dataRowData.AddColumnData(columnData.GetColumnData(i));
                            keyValues.Add(keyOutput.OutputColumnCollection[i].LineageID, columnData.GetColumnData(i));
                        }
                        keyBufferSink.AddRow(dataRowData);
                    }
                    else
                    {
                        if (dataBuffers.ContainsKey(rowData.GetColumnData(rowTypeColumnID)))
                        {

                            StringData columnData = new StringData();
                            StringAsRowReader columnReader = new StringAsRowReader(rowData.GetColumnData(rowDataColumnID));
                            stringParser.ParseRow(columnReader, columnData);
                            RowData dataRowData = new RowData();
                            IDTSOutput output = null;
                            dataOutputs.TryGetValue(rowData.GetColumnData(rowTypeColumnID), out output);

                            for (int i = 0; i < output.OutputColumnCollection.Count; i++)
                            {
                                if ((Utilities.usageOfColumnEnum)ManageProperties.GetPropertyValue(output.OutputColumnCollection[i].CustomPropertyCollection, ManageProperties.usageOfColumn) == Utilities.usageOfColumnEnum.Key)
                                {
                                    String keyValuesvalue = String.Empty;
                                    keyValues.TryGetValue((int)ManageProperties.GetPropertyValue(output.OutputColumnCollection[i].CustomPropertyCollection, ManageProperties.keyOutputColumnID), out keyValuesvalue);
                                    dataRowData.AddColumnData(keyValuesvalue);
                                }
                            }

                            //foreach (string keyValuesvalue in keyValues.Values)
                            //{
                            //    dataRowData.AddColumnData(keyValuesvalue);
                            //}
                            
                            for (int i = 0; i < columnData.ColumnCount; i++)
                            {
                                dataRowData.AddColumnData(columnData.GetColumnData(i));
                            }

                            dataRowData.RebuildRowText(columnDelimter);
                            BufferSink dataSink = null;
                            dataBufferSinks.TryGetValue(rowData.GetColumnData(rowTypeColumnID), out dataSink);
                            dataSink.AddRow(dataRowData);
                        }
                        else
                        {
                            this.ComponentMetaData.FireWarning(0, this.ComponentMetaData.Name, String.Format("The RowType value of {0} was not expected!", rowData.GetColumnData(rowTypeColumnID)), string.Empty, 0);
                            // Send a record to the Error output somehow.
                        }
                    }
                }
            }
            catch (ParsingBufferOverflowException ex)
            {
                this.PostErrorAndThrow(MessageStrings.ParsingBufferOverflow(currentRowCount + 1, ex.ColumnIndex + 1, FieldParser.ParsingBufferMaxSize));
            }
            catch (RowColumnNumberOverflow)
            {
                this.PostErrorAndThrow(MessageStrings.MaximumColumnNumberOverflow(currentRowCount + 1, RowParser.MaxColumnNumber));
            }
            finally
            {
                reader.Close();
            }

            foreach (PipelineBuffer buffer in buffers)
            {
                buffer.SetEndOfRowset();
            }
        }
        #endregion

        #region Post Execute
        public override void PostExecute()
        {
            base.PostExecute();
        }
        #endregion

        #endregion

        #region Helpers
        /// <summary>
        /// Parse through all the outputs and find the three "special" outputs.
        /// </summary>
        /// <returns></returns>
        private void findOutputIDs()
        {
            dataOutputIDs = new Dictionary<string, int>();
            foreach (IDTSOutput output in this.ComponentMetaData.OutputCollection)
            {
                switch ((Utilities.typeOfOutputEnum)ManageProperties.GetPropertyValue(output.CustomPropertyCollection, ManageProperties.typeOfOutput))
                {
                    case Utilities.typeOfOutputEnum.ErrorRecords:
                        errorOutputID = output.ID;
                        break;
                    case Utilities.typeOfOutputEnum.KeyRecords:
                        keyRecordOutputID = output.ID;
                        break;
                    case Utilities.typeOfOutputEnum.DataRecords:
                        dataOutputIDs.Add((String)ManageProperties.GetPropertyValue(output.CustomPropertyCollection, ManageProperties.rowTypeValue), output.ID);
                        break;
                    case Utilities.typeOfOutputEnum.PassThrough:
                        passthroughOutputID = output.ID;
                        break;
                    default:
                        break;
                }
            }
        }

        private DelimitedFileParser CreateParser()
        {
            ConnectionManager cm = Microsoft.SqlServer.Dts.Runtime.DtsConvert.GetWrapper(ComponentMetaData.RuntimeConnectionCollection[0].ConnectionManager);
            IDTSConnectionManagerFlatFile connectionFlatFile = cm.InnerObject as IDTSConnectionManagerFlatFile;
            string headerRowDelimiter = connectionFlatFile.HeaderRowDelimiter;
            int headerRowsToSkip = connectionFlatFile.HeaderRowsToSkip;
            int dataRowsToSkip = connectionFlatFile.DataRowsToSkip;
            bool columnNamesInFirstRow = connectionFlatFile.ColumnNamesInFirstDataRow;
            string textQualifier = connectionFlatFile.TextQualifier;
            string columnDelimiter = connectionFlatFile.Columns[0].ColumnDelimiter;
            string rowDelimiter = connectionFlatFile.Columns[connectionFlatFile.Columns.Count - 1].ColumnDelimiter;

            DelimitedFileParser parser = new DelimitedFileParser(columnDelimiter, rowDelimiter);
            parser.HeaderRowDelimiter = headerRowDelimiter;
            parser.HeaderRowsToSkip = headerRowsToSkip;
            parser.DataRowsToSkip = dataRowsToSkip;
            parser.TextQualifier = textQualifier;
            parser.ColumnNameInFirstRow = columnNamesInFirstRow;

            return parser;
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


        private void ConcatenateRowOverflow(ref RowData rowData, int columnCount)
        {
            if ((rowData.ColumnCount > columnCount) && (columnCount > 0))
            {
                String overflowData = String.Empty;
                for (int i = columnCount - 1; i < rowData.ColumnCount; i++)
                {
                    overflowData += (overflowData.Length == 0 ? String.Empty : this.columnDelimter) + rowData.GetColumnData(i);
                }
                List<string> columnValues = new List<string>();
                for (int i = 0; i < columnCount - 1; i++)
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
