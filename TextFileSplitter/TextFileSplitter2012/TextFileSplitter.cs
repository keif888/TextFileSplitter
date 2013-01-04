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
            ManageColumns.AddErrorOutputColumns(errorOutput);

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

            status = ValidateComponentProperties(status);
            status = ValidateOutputs(status);
            //return status;
            return DTSValidationStatus.VS_NEEDSNEWMETADATA;
        }

        private DTSValidationStatus ValidateComponentProperties(DTSValidationStatus oldStatus)
        {
            return this.propertyManager.ValidateProperties(this.ComponentMetaData.CustomPropertyCollection, oldStatus);
        }

        private DTSValidationStatus ValidateOutputs(DTSValidationStatus oldStatus)
        {
            DTSValidationStatus returnStatus = oldStatus;
            if (this.ComponentMetaData.OutputCollection.Count < 2)
            {
                this.PostError(MessageStrings.UnexpectedNumberOfOutputs);
                returnStatus = DTSValidationStatus.VS_ISCORRUPT;
            }
            else
            {
                IDTSOutputCollection outputCollection = this.ComponentMetaData.OutputCollection;
                // Ensure only one isErrorOutput!
                int errorOutputs = 0;
                int passThroughOutputs = 0;
                foreach (IDTSOutput output in outputCollection)
                {
                    if (output.IsErrorOut)
                    {
                        returnStatus = ValidateErrorOutput(output, returnStatus);
                        errorOutputs++;
                    }
                    else
                    {
                        returnStatus = ValidateRegularOutput(output, returnStatus, ref passThroughOutputs);
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
            }

            return returnStatus;
        }

        private DTSValidationStatus ValidateRegularOutput(IDTSOutput output, DTSValidationStatus oldStatus, ref int passThoughOutputs)
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
                if (((Utilities.typeOfOutputEnum)ManageProperties.GetPropertyValue(output.CustomPropertyCollection, ManageProperties.typeOfOutput)) == Utilities.typeOfOutputEnum.PassThrough)
                {
                    passThoughOutputs++;
                    returnStatus = ValidateExternalMetaData(output, returnStatus);
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



        //#region Set Component Property
        //public override IDTSCustomProperty SetComponentProperty(string propertyName, object propertyValue)
        //{
        //    return base.SetComponentProperty(propertyName, propertyValue);
        //}
        //#endregion

        #region Insert Input

        public override IDTSInput InsertInput(DTSInsertPlacement insertPlacement, int inputID)
        {
            this.PostErrorAndThrow(MessageStrings.CantAddInput);
            return null;
        }

        #endregion

        #region InsertOutput
        public override IDTSOutput InsertOutput(DTSInsertPlacement insertPlacement, int outputID)
        {
            IDTSOutput thisOutput = base.InsertOutput(insertPlacement, outputID);
            ManageProperties.AddOutputProperties(thisOutput.CustomPropertyCollection);
            thisOutput.ExternalMetadataColumnCollection.IsUsed = false;
            thisOutput.SynchronousInputID = 0;

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
                            IDTSOutputColumn outputColumn = thisOutput.OutputColumnCollection.NewAt(0);
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
                    if ((Utilities.typeOfOutputEnum)ManageProperties.GetPropertyValue(thisOutput.CustomPropertyCollection, ManageProperties.typeOfOutput) == Utilities.typeOfOutputEnum.KeyRecords)
                    {
                        // The Key value can be used here, BUT the column must be added to the other outputs!
                        Utilities.usageOfColumnEnum oldValue = (Utilities.usageOfColumnEnum)ManageProperties.GetPropertyValue(thisColumn.CustomPropertyCollection, ManageProperties.usageOfColumn);
                        if ((oldValue != (Utilities.usageOfColumnEnum)propertyValue) && ((oldValue == Utilities.usageOfColumnEnum.Key) || ((Utilities.usageOfColumnEnum)propertyValue == Utilities.usageOfColumnEnum.Key)))
                        {
                            if ((Utilities.usageOfColumnEnum)propertyValue == Utilities.usageOfColumnEnum.Key)
                            {
                                // Add column to the other Non Error Outputs.
                                foreach (IDTSOutput output in this.ComponentMetaData.OutputCollection)
                                {
                                    if (!output.IsErrorOut)
                                    {
                                        if ((Utilities.typeOfOutputEnum)ManageProperties.GetPropertyValue(output.CustomPropertyCollection, ManageProperties.typeOfOutput) == Utilities.typeOfOutputEnum.DataRecords)
                                        {
                                            IDTSOutputColumn outputColumn = output.OutputColumnCollection.NewAt(0);
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
                    }
                    else
                    {
                        if (((Utilities.usageOfColumnEnum)propertyValue == Utilities.usageOfColumnEnum.Key) || ((Utilities.usageOfColumnEnum)ManageProperties.GetPropertyValue(thisColumn.CustomPropertyCollection, ManageProperties.usageOfColumn) == Utilities.usageOfColumnEnum.Key))
                        {
                            // You are not allowed to set this with the GUI!
                            throw new COMException(MessageStrings.CannotSetPropertyToKey, E_FAIL);
                        }
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
        public override void DeleteOutputColumn(int outputID, int outputColumnID)
        {
            IDTSOutputCollection100 thisOutputColl = ComponentMetaData.OutputCollection;
            IDTSOutput100 thisOutput = thisOutputColl.GetObjectByID(outputID);
            if (thisOutput != null)
            {
                if (thisOutput.IsErrorOut)
                    this.PostErrorAndThrow(MessageStrings.CantChangeErrorOutputProperties);
                else
                {
                    IDTSOutputColumnCollection100 thisColumnColl = thisOutput.OutputColumnCollection;
                    IDTSOutputColumn100 thisColumn = thisColumnColl.GetObjectByID(outputColumnID);
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
        /// </summary>
        /// <param name="outputID">The ID of the output.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="propertyValue">The value to assign to the property.</param>
        /// <returns>The custom property.</returns>
        public override IDTSCustomProperty SetOutputProperty(int outputID, string propertyName, object propertyValue)
        {
            return base.SetOutputProperty(outputID, propertyName, propertyValue);
        }
        #endregion

        #region DeleteOutput

        public override void DeleteOutput(int outputID)
        {
            IDTSOutput thisOutput = this.ComponentMetaData.OutputCollection.GetObjectByID(outputID);
            if (!thisOutput.IsErrorOut)
            {
                if ((Utilities.typeOfOutputEnum)ManageProperties.GetPropertyValue(thisOutput.CustomPropertyCollection, ManageProperties.typeOfOutput) == Utilities.typeOfOutputEnum.KeyRecords)
                    this.PostErrorAndThrow(MessageStrings.CannotDeleteKeyOutput);
                else
                {
                    base.DeleteOutput(outputID);
                }
            }
            else
                this.PostErrorAndThrow(MessageStrings.CannotDeleteErrorOutput);
        }

        #endregion

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
