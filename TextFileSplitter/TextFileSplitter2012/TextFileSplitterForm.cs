using Microsoft.SqlServer.Dts.Design;
using Microsoft.SqlServer.Dts.Pipeline.Design;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.SqlServer.Dts.Runtime.Design;
using Microsoft.SqlServer.Dts.Runtime.Wrapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FileHelpers.Dynamic;
using FileHelpers;
using FileHelpers.Detection;

namespace Martin.SQLServer.Dts
{
    public partial class TextFileSplitterForm : Form, IDtsComponentUI
    {

        private IDTSComponentMetaData100 _componentMetaData;
        private IDTSDesigntimeComponent100 designtimeComponent;
        private IServiceProvider serviceProvider;
        private IErrorCollectionService errorCollector;
        private Connections connections;
        private Variables variables;
        private Microsoft.SqlServer.Dts.Runtime.Design.IDtsConnectionService connService;
        private Boolean _isLoading;
        Dictionary<String, List<String>> sampleDatas = new Dictionary<string, List<string>>();

        public TextFileSplitterForm()
        {
            _isLoading = true;
            InitializeComponent();
            _isLoading = false;
        }

        #region IDtsComponentUI Members

        void IDtsComponentUI.Initialize(IDTSComponentMetaData100 dtsComponentMetadata, IServiceProvider serviceProvider)
        {
            Initialize(dtsComponentMetadata, serviceProvider);
        }

        protected void Initialize(IDTSComponentMetaData100 dtsComponentMetadata, IServiceProvider serviceProvider)
        {
            this._componentMetaData = dtsComponentMetadata;
            this.serviceProvider = serviceProvider;

            Debug.Assert(this.serviceProvider != null, "The service provider was null!");

            this.errorCollector = this.serviceProvider.GetService(
                typeof(IErrorCollectionService)) as IErrorCollectionService;
            Debug.Assert(this.errorCollector != null, "The errorCollector was null!");

            if (this.errorCollector == null)
            {
                Exception ex = new System.ApplicationException(Properties.Resources.NotAllEditingServicesAvailable);
                throw ex;
            }
            this.connService = this.serviceProvider.GetService(typeof(Microsoft.SqlServer.Dts.Runtime.Design.IDtsConnectionService)) as Microsoft.SqlServer.Dts.Runtime.Design.IDtsConnectionService;
        }


        bool IDtsComponentUI.Edit(IWin32Window parentWindow, Microsoft.SqlServer.Dts.Runtime.Variables variables, Microsoft.SqlServer.Dts.Runtime.Connections connections)
        {
            return Edit(parentWindow, variables, connections);
        }

        protected bool Edit(IWin32Window parentWindow, Microsoft.SqlServer.Dts.Runtime.Variables variables, Microsoft.SqlServer.Dts.Runtime.Connections connections)
        {
            this.ClearErrors();


            try
            {
                Debug.Assert(this._componentMetaData != null, "Original Component Metadata is not OK.");

                this.designtimeComponent = this._componentMetaData.Instantiate();

                Debug.Assert(this.designtimeComponent != null, "Design-time component object is not OK.");

                // Cache variables and connections.
                this.variables = variables;
                this.connections = connections;
                if (this.ShowDialog(parentWindow) == DialogResult.OK)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                this.ReportErrors(ex);
                return false;
            }
        }

        /// <summary>
        /// Called before adding the component to the diagram. 
        /// </summary>
        /// <param name="parentWindow">The calling window</param>
        void IDtsComponentUI.New(IWin32Window parentWindow)
        {
            New(parentWindow);
        }

        protected void New(IWin32Window parentWindow)
        {
        }

        /// <summary>
        /// Called before deleting the component from the diagram. 
        /// </summary>
        /// <param name="parentWindow">The calling window</param>
        void IDtsComponentUI.Delete(IWin32Window parentWindow)
        {
            Delete(parentWindow);
        }

        protected void Delete(IWin32Window parentWindow)
        {
        }

        /// <summary>
        /// Display the component help
        /// </summary>
        /// <param name="parentWindow">The calling window</param>
        void IDtsComponentUI.Help(IWin32Window parentWindow)
        {
            Help(parentWindow);
        }

        protected void Help(IWin32Window parentWindow)
        {
        }

        #endregion

        #region Handling errors
        /// <summary>
        /// Clear the collection of errors collected by handling the pipeline events.
        /// </summary>
        protected void ClearErrors()
        {
            this.errorCollector.ClearErrors();
        }

        /// <summary>
        /// Get the text of error message that consist of all errors captured from pipeline events (OnError and OnWarning). 
        /// </summary>
        /// <returns>The error message</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        protected string GetErrorMessage()
        {
            return this.errorCollector.GetErrorMessage();
        }

        /// <summary>
        /// Reports errors occurred in the components by retrieving 
        /// error messages reported through pipeline events
        /// </summary>
        /// <param name="ex">passes in the exception to display</param>
        protected void ReportErrors(Exception ex)
        {
            if (this.errorCollector.GetErrors().Count > 0)
            {
                MessageBox.Show(
                    this.errorCollector.GetErrorMessage(),
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1,
                    0);
            }
            else
            {
                if (ex != null)
                {
                    MessageBox.Show(
                        ex.Message + "\r\nSource: " + ex.Source + "\r\n" + ex.TargetSite + "\r\n" + ex.StackTrace,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error,
                        MessageBoxDefaultButton.Button1,
                        0);
                }
                else
                {
                    MessageBox.Show(
                        "Somehow we got an error without an exception",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error,
                        MessageBoxDefaultButton.Button1,
                        0);

                }
            }
        }

        #endregion

        private void TextFileSplitterForm_Load(object sender, EventArgs e)
        {
            try
            {
                _isLoading = true;
                this.Text = _componentMetaData.Name;
                cbOutputDisposition.DataSource = System.Enum.GetValues(typeof(DTSRowDisposition));
                cbPTErrorDisposition.DataSource = System.Enum.GetValues(typeof(DTSRowDisposition));
                cbOutputType.Items.Add(Enum.GetName(typeof(Utilities.typeOfOutputEnum), Utilities.typeOfOutputEnum.KeyRecords));
                cbOutputType.Items.Add(Enum.GetName(typeof(Utilities.typeOfOutputEnum), Utilities.typeOfOutputEnum.DataRecords));
                cbOutputType.Items.Add(Enum.GetName(typeof(Utilities.typeOfOutputEnum), Utilities.typeOfOutputEnum.MasterRecord));
                cbOutputType.Items.Add(Enum.GetName(typeof(Utilities.typeOfOutputEnum), Utilities.typeOfOutputEnum.ChildMasterRecord));
                cbOutputType.Items.Add(Enum.GetName(typeof(Utilities.typeOfOutputEnum), Utilities.typeOfOutputEnum.ChildRecord));

                foreach (ConnectionManager cm in this.connections)
                {
                    if (isDelimitedFileConnection(cm))
                    {
                        this.cbConnectionManager.Items.Add(cm.Name);
                    }
                }

                this.cbConnectionManager.Items.Add("New Connection");

                if (this._componentMetaData.RuntimeConnectionCollection.Count == 1)
                {
                    string cmID = this._componentMetaData.RuntimeConnectionCollection[0].ConnectionManagerID;
                    if (this.connections.Contains(cmID))
                    {
                        this.cbConnectionManager.SelectedItem = this.connections[cmID].Name;
                        this.cbConnectionManager.Enabled = false;
                    }
                }

                this.tbColumnDelimiter.Text = (String)ManageProperties.GetPropertyValue(this._componentMetaData.CustomPropertyCollection, ManageProperties.columnDelimiter);
                this.cbDelimitedText.Checked = (Boolean)ManageProperties.GetPropertyValue(this._componentMetaData.CustomPropertyCollection, ManageProperties.isTextDelmited);
                this.tbTextDelimiter.Text = (String)ManageProperties.GetPropertyValue(this._componentMetaData.CustomPropertyCollection, ManageProperties.textDelmiter);
                this.cbTreatNulls.Checked = (Boolean)ManageProperties.GetPropertyValue(this._componentMetaData.CustomPropertyCollection, ManageProperties.treatEmptyStringsAsNull);

                foreach (IDTSOutput100 output in this._componentMetaData.OutputCollection)
                {
                    switch ((Utilities.typeOfOutputEnum)ManageProperties.GetPropertyValue(output.CustomPropertyCollection, ManageProperties.typeOfOutput))
                    {
                        case Utilities.typeOfOutputEnum.ErrorRecords:
                            break;
                        case Utilities.typeOfOutputEnum.KeyRecords:
                            int selectedItem = lbOutputs.Items.Add(output.Name);
                            lbOutputs.SelectedIndex = selectedItem;
                            break;
                        case Utilities.typeOfOutputEnum.DataRecords:
                            lbOutputs.Items.Add(output.Name);
                            break;
                        case Utilities.typeOfOutputEnum.PassThrough:
                            this.cbPTErrorDisposition.SelectedItem = output.ErrorRowDisposition;
                            foreach (IDTSOutputColumn100 outputColumn in output.OutputColumnCollection)
                            {
                                int rowNumber = dgvPassThrough.Rows.Add(1);
                                dgvPassThrough.Rows[rowNumber].Cells[0].Value = outputColumn.Name;
                                dgvPassThrough.Rows[rowNumber].Cells[0].ReadOnly = true;

                                DataGridViewComboBoxCell usageList = dgvPassThrough.Rows[rowNumber].Cells[1] as DataGridViewComboBoxCell;
                                //usageList.DataSource = System.Enum.GetValues(typeof(Utilities.usageOfColumnEnum));
                                usageList.Items.Add(Enum.GetName(typeof(Utilities.usageOfColumnEnum), Utilities.usageOfColumnEnum.RowData));
                                usageList.Items.Add(Enum.GetName(typeof(Utilities.usageOfColumnEnum), Utilities.usageOfColumnEnum.RowType));
                                usageList.Items.Add(Enum.GetName(typeof(Utilities.usageOfColumnEnum), Utilities.usageOfColumnEnum.Passthrough));
                                usageList.Value = Enum.GetName(typeof(Utilities.usageOfColumnEnum), (Utilities.usageOfColumnEnum)ManageProperties.GetPropertyValue(outputColumn.CustomPropertyCollection, ManageProperties.usageOfColumn));

                                dgvPassThrough.Rows[rowNumber].Cells[2].Value = outputColumn.CodePage;
                                dgvPassThrough.Rows[rowNumber].Cells[2].ReadOnly = true;

                                DataGridViewComboBoxCell dataType = dgvPassThrough.Rows[rowNumber].Cells[3] as DataGridViewComboBoxCell;
                                dataType.DataSource = System.Enum.GetValues(typeof(DataType));
                                dataType.Value = outputColumn.DataType;
                                dataType.ReadOnly = true;

                                dgvPassThrough.Rows[rowNumber].Cells[4].Value = outputColumn.Length;
                                dgvPassThrough.Rows[rowNumber].Cells[4].ReadOnly = true;

                                dgvPassThrough.Rows[rowNumber].Cells[5].Value = outputColumn.Precision;
                                dgvPassThrough.Rows[rowNumber].Cells[5].ReadOnly = true;

                                dgvPassThrough.Rows[rowNumber].Cells[6].Value = outputColumn.Scale;
                                dgvPassThrough.Rows[rowNumber].Cells[6].ReadOnly = true;
                            }
                            break;
                        case Utilities.typeOfOutputEnum.MasterRecord:
                            lbOutputs.Items.Add(output.Name);
                            break;
                        case Utilities.typeOfOutputEnum.ChildMasterRecord:
                            lbOutputs.Items.Add(output.Name);
                            break;
                        case Utilities.typeOfOutputEnum.ChildRecord:
                            lbOutputs.Items.Add(output.Name);
                            break;
                        case Utilities.typeOfOutputEnum.RowsProcessed:
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("{0}\r\n{1}", ex.Message, ex.StackTrace), "Something went Really Wrong!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _isLoading = false;
            }
        }





        #region Connection Tab Events

        private void cbConnectionManager_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Special case: the <New...> item on the combo box causes us to try to create a new connection manager.
            if ((string)cbConnectionManager.SelectedItem == "New Connection")
            {
                // Fetch the IDtsConnectionService.  It provides facilities to present the user with 
                // a new connection dialog, so they don't need to exit the (modal) UI to create one.
                IDtsConnectionService connService =
                    (IDtsConnectionService)serviceProvider.GetService(typeof(IDtsConnectionService));
                System.Collections.ArrayList created = connService.CreateConnection("FLATFILE");

                // CreateConnection() returns back a list of connections that were created -- go ahead
                // and update our list with those new items.
                foreach (ConnectionManager cm in created)
                {
                    cbConnectionManager.Items.Insert(0, cm.Name);
                }

                // If we created an item, we select it in the combo box, otherwise, clear the selection entirely.
                if (created.Count > 0)
                {
                    cbConnectionManager.SelectedIndex = 0;
                }
                else
                {
                    cbConnectionManager.SelectedIndex = -1;
                }
            }



            // No matter what, we set the current connection manager to the chosen item if it's real.
            if (connections.Contains(cbConnectionManager.SelectedItem))
            {
                cbConnectionManager.Enabled = false;
                if (this._componentMetaData.RuntimeConnectionCollection[0].ConnectionManagerID != connections[cbConnectionManager.SelectedItem].ID)
                {

                    this._componentMetaData.RuntimeConnectionCollection[0].ConnectionManager = DtsConvert.GetExtendedInterface(connections[cbConnectionManager.SelectedItem]);
                    // Depreciated //    DtsConvert.ToConnectionManager90(connections[cbConnectionManager.SelectedItem]);

                    this._componentMetaData.RuntimeConnectionCollection[0].ConnectionManagerID =
                        connections[cbConnectionManager.SelectedItem].ID;

                    ConnectionManager cm = connections[cbConnectionManager.SelectedItem];
                    IDTSConnectionManagerFlatFile100 connectionFlatFile = cm.InnerObject as IDTSConnectionManagerFlatFile100;
                    if (!String.IsNullOrEmpty(cm.ConnectionString))
                    {
                        IDTSOutput100 passthroughOutput = null;
                        foreach (IDTSOutput100 output in this._componentMetaData.OutputCollection)
                        {
                            if ((Utilities.typeOfOutputEnum)ManageProperties.GetPropertyValue(output.CustomPropertyCollection, ManageProperties.typeOfOutput) == Utilities.typeOfOutputEnum.PassThrough)
                            {
                                passthroughOutput = output;
                                break;
                            }
                        }

                        if (passthroughOutput != null)
                        {
                            if (passthroughOutput.OutputColumnCollection.Count == 0)
                            {
                                foreach (IDTSConnectionManagerFlatFileColumn100 FFcolumn in connectionFlatFile.Columns)
                                {
                                    IDTSOutputColumn100 outColumn = passthroughOutput.OutputColumnCollection.New();
                                    ManageColumns.SetOutputColumnDefaults(outColumn, connectionFlatFile.CodePage);
                                    ManageProperties.AddOutputColumnProperties(outColumn.CustomPropertyCollection);
                                    outColumn.Name = ((IDTSName100)FFcolumn).Name;
                                    outColumn.SetDataTypeProperties(FFcolumn.DataType, FFcolumn.MaximumWidth, FFcolumn.DataPrecision, FFcolumn.DataScale, connectionFlatFile.CodePage);
                                    IDTSExternalMetadataColumn100 eColumn = this._componentMetaData.OutputCollection[0].ExternalMetadataColumnCollection.New();
                                    eColumn.Name = outColumn.Name;
                                    eColumn.DataType = outColumn.DataType;
                                    eColumn.Precision = outColumn.Precision;
                                    eColumn.Scale = outColumn.Scale;
                                    eColumn.Length = outColumn.Length;
                                    outColumn.ExternalMetadataColumnID = eColumn.ID;
                                }
                            }
                            this.cbPTErrorDisposition.SelectedItem = passthroughOutput.ErrorRowDisposition;
                            dgvPassThrough.Rows.Clear();
                            foreach (IDTSOutputColumn100 outputColumn in passthroughOutput.OutputColumnCollection)
                            {
                                int rowNumber = dgvPassThrough.Rows.Add(1);
                                dgvPassThrough.Rows[rowNumber].Cells[0].Value = outputColumn.Name;
                                dgvPassThrough.Rows[rowNumber].Cells[0].ReadOnly = true;

                                DataGridViewComboBoxCell usageList = dgvPassThrough.Rows[rowNumber].Cells[1] as DataGridViewComboBoxCell;
                                //usageList.DataSource = System.Enum.GetValues(typeof(Utilities.usageOfColumnEnum));
                                usageList.Items.Add(Enum.GetName(typeof(Utilities.usageOfColumnEnum), Utilities.usageOfColumnEnum.RowData));
                                usageList.Items.Add(Enum.GetName(typeof(Utilities.usageOfColumnEnum), Utilities.usageOfColumnEnum.RowType));
                                usageList.Items.Add(Enum.GetName(typeof(Utilities.usageOfColumnEnum), Utilities.usageOfColumnEnum.Passthrough));
                                usageList.Value = Enum.GetName(typeof(Utilities.usageOfColumnEnum), (Utilities.usageOfColumnEnum)ManageProperties.GetPropertyValue(outputColumn.CustomPropertyCollection, ManageProperties.usageOfColumn));

                                dgvPassThrough.Rows[rowNumber].Cells[2].Value = outputColumn.CodePage;
                                dgvPassThrough.Rows[rowNumber].Cells[2].ReadOnly = true;

                                DataGridViewComboBoxCell dataType = dgvPassThrough.Rows[rowNumber].Cells[3] as DataGridViewComboBoxCell;
                                dataType.DataSource = System.Enum.GetValues(typeof(DataType));
                                dataType.Value = outputColumn.DataType;
                                dataType.ReadOnly = true;

                                dgvPassThrough.Rows[rowNumber].Cells[4].Value = outputColumn.Length;
                                dgvPassThrough.Rows[rowNumber].Cells[4].ReadOnly = true;

                                dgvPassThrough.Rows[rowNumber].Cells[5].Value = outputColumn.Precision;
                                dgvPassThrough.Rows[rowNumber].Cells[5].ReadOnly = true;

                                dgvPassThrough.Rows[rowNumber].Cells[6].Value = outputColumn.Scale;
                                dgvPassThrough.Rows[rowNumber].Cells[6].ReadOnly = true;
                            }
                        }
                    }
                }
            }
        }

        private void tbColumnDelimiter_TextChanged(object sender, EventArgs e)
        {
            ManageProperties.SetPropertyValue(_componentMetaData.CustomPropertyCollection, ManageProperties.columnDelimiter, tbColumnDelimiter.Text);
        }

        private void cbDelimitedText_CheckedChanged(object sender, EventArgs e)
        {
            ManageProperties.SetPropertyValue(_componentMetaData.CustomPropertyCollection, ManageProperties.isTextDelmited, cbDelimitedText.Checked);
        }

        private void tbTextDelimiter_TextChanged(object sender, EventArgs e)
        {
            ManageProperties.SetPropertyValue(_componentMetaData.CustomPropertyCollection, ManageProperties.textDelmiter, tbTextDelimiter.Text);
        }

        private void cbTreatNulls_CheckedChanged(object sender, EventArgs e)
        {
            ManageProperties.SetPropertyValue(_componentMetaData.CustomPropertyCollection, ManageProperties.treatEmptyStringsAsNull, cbTreatNulls.Checked);
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                DrawingControl.SuspendDrawing(dgvConnectionPreview);
                dgvConnectionPreview.Columns.Clear();
                dgvConnectionPreview.Rows.Clear();
                IDTSConnectionManagerFlatFile100 connectionFlatFile = connections[cbConnectionManager.SelectedItem].InnerObject as IDTSConnectionManagerFlatFile100;
                string FileName = connections[cbConnectionManager.SelectedItem].ConnectionString;
                IDTSOutput100 passThoughIDTSOutput = null;

                foreach (IDTSOutput100 output in _componentMetaData.OutputCollection)
                {
                    if ((Utilities.typeOfOutputEnum)ManageProperties.GetPropertyValue(output.CustomPropertyCollection, ManageProperties.typeOfOutput) == Utilities.typeOfOutputEnum.PassThrough)
                    {
                        passThoughIDTSOutput = output;
                        break;
                    }
                }

                SSISOutput passThroughOutput = new SSISOutput(passThoughIDTSOutput, null);
                bool firstRowColumnNames = connectionFlatFile.ColumnNamesInFirstDataRow;
                String passThroughClassString = Utilities.DynamicClassStringFromOutput(passThroughOutput, firstRowColumnNames, connectionFlatFile.Columns[connectionFlatFile.Columns.Count - 1].ColumnDelimiter, connectionFlatFile.Columns[0].ColumnDelimiter);
                Type passThroughType = ClassBuilder.ClassFromString(passThroughClassString);
                foreach (SSISOutputColumn ssisColumn in passThroughOutput.OutputColumnCollection)
                {
                    ssisColumn.FileHelperField = passThroughType.GetField(ssisColumn.Name);
                    dgvConnectionPreview.Columns.Add(ssisColumn.Name, ssisColumn.Name);
                }
                System.Reflection.FieldInfo[] fieldList = passThroughType.GetFields();
                FileHelperAsyncEngine engine = new FileHelperAsyncEngine(passThroughType);
                engine.BeginReadFile(FileName);

                int RowCount = 0;

                while (engine.ReadNext() != null)
                {
                    int RowNumber = dgvConnectionPreview.Rows.Add(engine.LastRecordValues);
                    //foreach (SSISOutputColumn ssisColumn in passThroughOutput.OutputColumnCollection)
                    //{
                    //    dgvConnectionPreview.Rows[RowNumber].Cells[ssisColumn.Name].Value = (String)ssisColumn.FileHelperField.GetValue(engine.LastRecord);
                    //}
                    if (RowCount++ > tbNumberOfRecordsToPreview.Value)
                    {
                        break;
                    }
                }
                engine.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Something went Really Wrong!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                DrawingControl.ResumeDrawing(dgvConnectionPreview);
            }
        }

        #endregion

        #region Pass Through Tab Events
        private void btnPTPreview_Click(object sender, EventArgs e)
        {
            try
            {
                DrawingControl.SuspendDrawing(dgvPassThroughPreview);
                dgvPassThroughPreview.Columns.Clear();
                dgvPassThroughPreview.Rows.Clear();
                IDTSConnectionManagerFlatFile100 connectionFlatFile = connections[cbConnectionManager.SelectedItem].InnerObject as IDTSConnectionManagerFlatFile100;
                string FileName = connections[cbConnectionManager.SelectedItem].ConnectionString;
                IDTSOutput100 passThoughIDTSOutput = null;

                foreach (IDTSOutput100 output in _componentMetaData.OutputCollection)
                {
                    if ((Utilities.typeOfOutputEnum)ManageProperties.GetPropertyValue(output.CustomPropertyCollection, ManageProperties.typeOfOutput) == Utilities.typeOfOutputEnum.PassThrough)
                    {
                        passThoughIDTSOutput = output;
                        break;
                    }
                }

                SSISOutput passThroughOutput = new SSISOutput(passThoughIDTSOutput, null);
                bool firstRowColumnNames = connectionFlatFile.ColumnNamesInFirstDataRow;
                String passThroughClassString = Utilities.DynamicClassStringFromOutput(passThroughOutput, firstRowColumnNames, connectionFlatFile.Columns[connectionFlatFile.Columns.Count - 1].ColumnDelimiter, connectionFlatFile.Columns[0].ColumnDelimiter);
                Type passThroughType = ClassBuilder.ClassFromString(passThroughClassString);
                foreach (SSISOutputColumn ssisColumn in passThroughOutput.OutputColumnCollection)
                {
                    ssisColumn.FileHelperField = passThroughType.GetField(ssisColumn.Name);
                    dgvPassThroughPreview.Columns.Add(ssisColumn.Name, ssisColumn.Name);
                }
                System.Reflection.FieldInfo[] fieldList = passThroughType.GetFields();
                FileHelperAsyncEngine engine = new FileHelperAsyncEngine(passThroughType);
                engine.BeginReadFile(FileName);

                int RowCount = 0;

                while (engine.ReadNext() != null)
                {
                    int RowNumber = dgvPassThroughPreview.Rows.Add(engine.LastRecordValues);
                    //foreach (SSISOutputColumn ssisColumn in passThroughOutput.OutputColumnCollection)
                    //{
                    //    dgvConnectionPreview.Rows[RowNumber].Cells[ssisColumn.Name].Value = (String)ssisColumn.FileHelperField.GetValue(engine.LastRecord);
                    //}
                    if (RowCount++ > tbPTNumberOfRecordsToPreview.Value)
                    {
                        break;
                    }
                }
                engine.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Something went Really Wrong!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                DrawingControl.ResumeDrawing(dgvPassThroughPreview);
            }
        }

        private void dgvPassThrough_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!_isLoading)
            {
                if (e.ColumnIndex == 1)
                {
                    IDTSOutput100 passThoughIDTSOutput = null;

                    foreach (IDTSOutput100 output in _componentMetaData.OutputCollection)
                    {
                        if ((Utilities.typeOfOutputEnum)ManageProperties.GetPropertyValue(output.CustomPropertyCollection, ManageProperties.typeOfOutput) == Utilities.typeOfOutputEnum.PassThrough)
                        {
                            passThoughIDTSOutput = output;
                            break;
                        }
                    }

                    foreach (IDTSOutputColumn100 outputColumn in passThoughIDTSOutput.OutputColumnCollection)
                    {
                        if (outputColumn.Name == (String)dgvPassThrough.Rows[e.RowIndex].Cells[0].Value)
                        {
                            ManageProperties.SetPropertyValue(outputColumn.CustomPropertyCollection, ManageProperties.usageOfColumn, ConvertFromStringToUsageOfColumn((String)dgvPassThrough.Rows[e.RowIndex].Cells[e.ColumnIndex].Value));
                            break;
                        }
                    }
                }
            }
        }

        private void cbPTErrorDisposition_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_isLoading)
            {
                foreach (IDTSOutput100 output in _componentMetaData.OutputCollection)
                {
                    if ((Utilities.typeOfOutputEnum)ManageProperties.GetPropertyValue(output.CustomPropertyCollection, ManageProperties.typeOfOutput) == Utilities.typeOfOutputEnum.PassThrough)
                    {
                        output.ErrorRowDisposition = (DTSRowDisposition)cbPTErrorDisposition.SelectedItem;
                        break;
                    }
                }
            }
        }


        #endregion

        #region Helpers

        private Utilities.typeOfOutputEnum ConvertFromString(String comboBoxValue)
        {
            foreach (var value in Enum.GetValues(typeof(Utilities.typeOfOutputEnum)))
            {
                if (comboBoxValue == Enum.GetName(typeof(Utilities.typeOfOutputEnum), value))
                {
                    return (Utilities.typeOfOutputEnum)value;
                }
            }
            return Utilities.typeOfOutputEnum.DataRecords;
        }

        private Utilities.usageOfColumnEnum ConvertFromStringToUsageOfColumn(String comboBoxValue)
        {
            foreach (var value in Enum.GetValues(typeof(Utilities.usageOfColumnEnum)))
            {
                if (comboBoxValue == Enum.GetName(typeof(Utilities.usageOfColumnEnum), value))
                {
                    return (Utilities.usageOfColumnEnum)value;
                }
            }
            return Utilities.usageOfColumnEnum.Passthrough;
        }

        private bool isDelimitedFileConnection(ConnectionManager cm)
        {
            if (cm != null)
            {
                ConnectionManagerFlatFile cmFlatFile = cm.InnerObject as ConnectionManagerFlatFile;
                if (cmFlatFile != null)
                {
                    IDTSConnectionManagerFlatFile100 connectionFlatFile = cm.InnerObject as IDTSConnectionManagerFlatFile100;
                    if (connectionFlatFile != null)
                    {
                        if (connectionFlatFile.Format.Contains("Delimited"))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private void SetDataTypeDefaults(DataGridViewRow row)
        {
            Boolean oldIsLoading = _isLoading;
            _isLoading = true;
            DataGridViewComboBoxCell dataType = row.Cells[4] as DataGridViewComboBoxCell;
            switch ((DataType)dataType.Value)
            {
                case DataType.DT_BOOL:
                case DataType.DT_BYREF_BOOL:
                case DataType.DT_BYREF_CY:
                case DataType.DT_BYREF_DATE:
                case DataType.DT_BYREF_DBDATE:
                case DataType.DT_BYREF_DBTIME:
                case DataType.DT_BYREF_FILETIME:
                case DataType.DT_BYREF_GUID:
                case DataType.DT_BYREF_I1:
                case DataType.DT_BYREF_I2:
                case DataType.DT_BYREF_I4:
                case DataType.DT_BYREF_I8:
                case DataType.DT_BYREF_R4:
                case DataType.DT_BYREF_R8:
                case DataType.DT_BYREF_UI1:
                case DataType.DT_BYREF_UI2:
                case DataType.DT_BYREF_UI4:
                case DataType.DT_BYREF_UI8:
                case DataType.DT_CY:
                case DataType.DT_DATE:
                case DataType.DT_DBDATE:
                case DataType.DT_DBTIME:
                case DataType.DT_EMPTY:
                case DataType.DT_FILETIME:
                case DataType.DT_GUID:
                case DataType.DT_I1:
                case DataType.DT_I2:
                case DataType.DT_I4:
                case DataType.DT_I8:
                case DataType.DT_NULL:
                case DataType.DT_R4:
                case DataType.DT_R8:
                case DataType.DT_UI1:
                case DataType.DT_UI2:
                case DataType.DT_UI4:
                case DataType.DT_UI8:
                    row.Cells[3].Value = 0; // CodePage
                    row.Cells[5].Value = 0;  // Length
                    row.Cells[6].Value = 0;  // Precision
                    row.Cells[7].Value = 0;  // Scale
                    break;
                case DataType.DT_BYREF_DBTIMESTAMP:
                case DataType.DT_DBTIMESTAMP:
                    row.Cells[3].Value = 0;  // CodePage
                    row.Cells[5].Value = 0;  // Length
                    row.Cells[6].Value = 0;  // Precision
                    row.Cells[7].Value = 3;  // Scale
                    break;
                case DataType.DT_BYREF_DBTIME2:
                case DataType.DT_BYREF_DBTIMESTAMP2:
                case DataType.DT_BYREF_DBTIMESTAMPOFFSET:
                case DataType.DT_DBTIME2:
                case DataType.DT_DBTIMESTAMP2:
                case DataType.DT_DBTIMESTAMPOFFSET:
                    row.Cells[3].Value = 0;  // CodePage
                    row.Cells[5].Value = 0;  // Length
                    row.Cells[6].Value = 0;  // Precision
                    row.Cells[7].Value = 7;  // Scale
                    break;
                case DataType.DT_BYREF_DECIMAL:
                case DataType.DT_BYREF_NUMERIC:
                case DataType.DT_DECIMAL:
                case DataType.DT_NUMERIC:
                    row.Cells[3].Value = 0; // CodePage
                    row.Cells[5].Value = 0; // Length
                    row.Cells[6].Value = 18; // Precision
                    row.Cells[7].Value = 2; // Scale
                    break;
                case DataType.DT_BYTES:
                case DataType.DT_IMAGE:
                case DataType.DT_NTEXT:
                case DataType.DT_WSTR:
                    row.Cells[3].Value = 0; // CodePage
                    row.Cells[5].Value = 255; // Length
                    row.Cells[6].Value = 0; // Precision
                    row.Cells[7].Value = 0; // Scale
                    break;
                case DataType.DT_STR:
                case DataType.DT_TEXT:
                    row.Cells[3].Value = 1252; // CodePage
                    row.Cells[5].Value = 255; // Length
                    row.Cells[6].Value = 0; // Precision
                    row.Cells[7].Value = 0; // Scale
                    break;
                default:
                    row.Cells[3].Value = 0; // CodePage
                    row.Cells[5].Value = 0; // Length
                    row.Cells[6].Value = 0; // Precision
                    row.Cells[7].Value = 0; // Scale
                    break;
            }
            _isLoading = oldIsLoading;
        }

        private void SetOutputColumnsReadWrite(DataGridViewRow row)
        {
            row.Cells[3].ReadOnly = false; // CodePage
            row.Cells[5].ReadOnly = false; // Length
            row.Cells[6].ReadOnly = false; // Precision
            row.Cells[7].ReadOnly = false; // Scale
        }

        private void SetOutputColumnsReadOnly(DataGridViewRow row)
        {
            DataGridViewComboBoxCell dataType = row.Cells[4] as DataGridViewComboBoxCell;
            switch ((DataType)dataType.Value)
            {
                case DataType.DT_BOOL:
                case DataType.DT_BYREF_BOOL:
                case DataType.DT_BYREF_CY:
                case DataType.DT_BYREF_DATE:
                case DataType.DT_BYREF_DBDATE:
                case DataType.DT_BYREF_DBTIME:
                case DataType.DT_BYREF_FILETIME:
                case DataType.DT_BYREF_GUID:
                case DataType.DT_BYREF_I1:
                case DataType.DT_BYREF_I2:
                case DataType.DT_BYREF_I4:
                case DataType.DT_BYREF_I8:
                case DataType.DT_BYREF_R4:
                case DataType.DT_BYREF_R8:
                case DataType.DT_BYREF_UI1:
                case DataType.DT_BYREF_UI2:
                case DataType.DT_BYREF_UI4:
                case DataType.DT_BYREF_UI8:
                case DataType.DT_CY:
                case DataType.DT_DATE:
                case DataType.DT_DBDATE:
                case DataType.DT_DBTIME:
                case DataType.DT_EMPTY:
                case DataType.DT_FILETIME:
                case DataType.DT_GUID:
                case DataType.DT_I1:
                case DataType.DT_I2:
                case DataType.DT_I4:
                case DataType.DT_I8:
                case DataType.DT_NULL:
                case DataType.DT_R4:
                case DataType.DT_R8:
                case DataType.DT_UI1:
                case DataType.DT_UI2:
                case DataType.DT_UI4:
                case DataType.DT_UI8:
                    row.Cells[3].ReadOnly = true; // CodePage
                    row.Cells[5].ReadOnly = true; // Length
                    row.Cells[6].ReadOnly = true; // Precision
                    row.Cells[7].ReadOnly = true; // Scale
                    break;
                case DataType.DT_BYREF_DBTIME2:
                case DataType.DT_BYREF_DBTIMESTAMP:
                case DataType.DT_BYREF_DBTIMESTAMP2:
                case DataType.DT_BYREF_DBTIMESTAMPOFFSET:
                case DataType.DT_DBTIME2:
                case DataType.DT_DBTIMESTAMP:
                case DataType.DT_DBTIMESTAMP2:
                case DataType.DT_DBTIMESTAMPOFFSET:
                    row.Cells[3].ReadOnly = true; // CodePage
                    row.Cells[5].ReadOnly = true; // Length
                    row.Cells[6].ReadOnly = true; // Precision
                    row.Cells[7].ReadOnly = false; // Scale
                    break;
                case DataType.DT_BYREF_DECIMAL:
                case DataType.DT_BYREF_NUMERIC:
                case DataType.DT_DECIMAL:
                case DataType.DT_NUMERIC:
                    row.Cells[3].ReadOnly = true; // CodePage
                    row.Cells[5].ReadOnly = true; // Length
                    row.Cells[6].ReadOnly = false; // Precision
                    row.Cells[7].ReadOnly = false; // Scale
                    break;
                case DataType.DT_BYTES:
                case DataType.DT_IMAGE:
                case DataType.DT_NTEXT:
                case DataType.DT_WSTR:
                    row.Cells[3].ReadOnly = true; // CodePage
                    row.Cells[5].ReadOnly = false; // Length
                    row.Cells[6].ReadOnly = true; // Precision
                    row.Cells[7].ReadOnly = true; // Scale
                    break;
                case DataType.DT_STR:
                case DataType.DT_TEXT:
                    row.Cells[3].ReadOnly = false; // CodePage
                    row.Cells[5].ReadOnly = false; // Length
                    row.Cells[6].ReadOnly = true; // Precision
                    row.Cells[7].ReadOnly = true; // Scale
                    break;
                default:
                    row.Cells[3].ReadOnly = true; // CodePage
                    row.Cells[5].ReadOnly = true; // Length
                    row.Cells[6].ReadOnly = true; // Precision
                    row.Cells[7].ReadOnly = true; // Scale
                    break;
            }
        }

        #endregion

        #region Outputs Tab Events
        private void lbOutputs_SelectedIndexChanged(object sender, EventArgs e)
        {
            _isLoading = true;
            DrawingControl.SuspendDrawing(dgvOutputColumns);
            DrawingControl.SuspendDrawing(dgvOutputPreview);
            dgvOutputPreview.Columns.Clear();
            dgvOutputPreview.Rows.Clear();
            dgvOutputColumns.SuspendLayout();
            dgvOutputColumns.Rows.Clear();
            tbOutputName.Text = String.Empty;
            tbRowTypeValue.Text = String.Empty;
            foreach (IDTSOutput100 output in this._componentMetaData.OutputCollection)
            {
                if (output.Name == (String)lbOutputs.SelectedItem)
                {
                    tbOutputName.Text = output.Name;
                    tbOutputName.Tag = output.ID;
                    tbRowTypeValue.Text = (String)ManageProperties.GetPropertyValue(output.CustomPropertyCollection, ManageProperties.rowTypeValue);
                    cbOutputDisposition.SelectedItem = output.ErrorRowDisposition;
                    cbOutputType.SelectedItem = Enum.GetName(typeof(Utilities.typeOfOutputEnum), (Utilities.typeOfOutputEnum)ManageProperties.GetPropertyValue(output.CustomPropertyCollection, ManageProperties.typeOfOutput));

                    // This is how to get the value back out!
                    // Utilities.typeOfOutputEnum testThis = ConvertFromString((String)cbOutputType.SelectedItem);

                    switch ((Utilities.typeOfOutputEnum)ManageProperties.GetPropertyValue(output.CustomPropertyCollection, ManageProperties.typeOfOutput))
                    {
                        case Utilities.typeOfOutputEnum.KeyRecords:
                            foreach (IDTSOutputColumn100 outputColumn in output.OutputColumnCollection)
                            {
                                int rowNumber = dgvOutputColumns.Rows.Add(1);
                                dgvOutputColumns.Rows[rowNumber].Tag = output.ID;
                                dgvOutputColumns.Rows[rowNumber].Cells[0].Value = outputColumn.Name;
                                dgvOutputColumns.Rows[rowNumber].Cells[0].Tag = outputColumn.ID;

                                DataGridViewComboBoxCell usageList = dgvOutputColumns.Rows[rowNumber].Cells[1] as DataGridViewComboBoxCell;
                                usageList.Items.Add(Enum.GetName(typeof(Utilities.usageOfColumnEnum), Utilities.usageOfColumnEnum.Key));
                                usageList.Items.Add(Enum.GetName(typeof(Utilities.usageOfColumnEnum), Utilities.usageOfColumnEnum.Passthrough));
                                usageList.Value = Enum.GetName(typeof(Utilities.usageOfColumnEnum), (Utilities.usageOfColumnEnum)ManageProperties.GetPropertyValue(outputColumn.CustomPropertyCollection, ManageProperties.usageOfColumn));

                                dgvOutputColumns.Rows[rowNumber].Cells[2].Value = (String)ManageProperties.GetPropertyValue(outputColumn.CustomPropertyCollection, ManageProperties.dotNetFormatString);

                                dgvOutputColumns.Rows[rowNumber].Cells[3].Value = outputColumn.CodePage;

                                DataGridViewComboBoxCell dataType = dgvOutputColumns.Rows[rowNumber].Cells[4] as DataGridViewComboBoxCell;
                                dataType.DataSource = System.Enum.GetValues(typeof(DataType));
                                dataType.ValueType = typeof(DataType);
                                dataType.Value = outputColumn.DataType;

                                dgvOutputColumns.Rows[rowNumber].Cells[5].Value = outputColumn.Length;

                                dgvOutputColumns.Rows[rowNumber].Cells[6].Value = outputColumn.Precision;

                                dgvOutputColumns.Rows[rowNumber].Cells[7].Value = outputColumn.Scale;
                                SetOutputColumnsReadOnly(dgvOutputColumns.Rows[rowNumber]);
                            }
                            break;
                        case Utilities.typeOfOutputEnum.DataRecords:
                        case Utilities.typeOfOutputEnum.MasterRecord:
                        case Utilities.typeOfOutputEnum.ChildMasterRecord:
                        case Utilities.typeOfOutputEnum.ChildRecord:
                            if (ManageProperties.GetPropertyValue(output.CustomPropertyCollection, ManageProperties.masterRecordID) != null)
                            {
                                int MasterID = (int)ManageProperties.GetPropertyValue(output.CustomPropertyCollection, ManageProperties.masterRecordID);
                                if (MasterID > 0)
                                {
                                    try
                                    {
                                        IDTSOutput100 masterOutput = _componentMetaData.OutputCollection.GetObjectByID(MasterID);
                                        if (masterOutput != null)
                                        {
                                            tbMaster.Text = masterOutput.Name;
                                        }
                                    }
                                    catch
                                    {
                                        // Something Bad, so reset the type and values!!!
                                        tbMaster.Text = String.Empty;
                                        designtimeComponent.SetOutputProperty(output.ID, ManageProperties.masterRecordID, -1);
                                        designtimeComponent.SetOutputProperty(output.ID, ManageProperties.typeOfOutput, Utilities.typeOfOutputEnum.PassThrough);
                                    }
                                }
                                else
                                {
                                    tbMaster.Text = String.Empty;
                                }
                            }
                            else
                            {
                                tbMaster.Text = String.Empty;
                            }

                            foreach (IDTSOutputColumn100 outputColumn in output.OutputColumnCollection)
                            {
                                int rowNumber = dgvOutputColumns.Rows.Add(1);
                                dgvOutputColumns.Rows[rowNumber].Tag = output.ID;
                                dgvOutputColumns.Rows[rowNumber].Cells[0].Value = outputColumn.Name;
                                dgvOutputColumns.Rows[rowNumber].Cells[0].Tag = outputColumn.ID;

                                DataGridViewComboBoxCell usageList = dgvOutputColumns.Rows[rowNumber].Cells[1] as DataGridViewComboBoxCell;
                                usageList.Items.Add(Enum.GetName(typeof(Utilities.usageOfColumnEnum), Utilities.usageOfColumnEnum.Key));
                                usageList.Items.Add(Enum.GetName(typeof(Utilities.usageOfColumnEnum), Utilities.usageOfColumnEnum.Passthrough));
                                usageList.Items.Add(Enum.GetName(typeof(Utilities.usageOfColumnEnum), Utilities.usageOfColumnEnum.MasterValue));
                                usageList.Value = Enum.GetName(typeof(Utilities.usageOfColumnEnum), (Utilities.usageOfColumnEnum)ManageProperties.GetPropertyValue(outputColumn.CustomPropertyCollection, ManageProperties.usageOfColumn));

                                dgvOutputColumns.Rows[rowNumber].Cells[2].Value = (String)ManageProperties.GetPropertyValue(outputColumn.CustomPropertyCollection, ManageProperties.dotNetFormatString);

                                dgvOutputColumns.Rows[rowNumber].Cells[3].Value = outputColumn.CodePage;

                                DataGridViewComboBoxCell dataType = dgvOutputColumns.Rows[rowNumber].Cells[4] as DataGridViewComboBoxCell;
                                dataType.DataSource = System.Enum.GetValues(typeof(DataType));
                                dataType.ValueType = typeof(DataType);
                                dataType.Value = outputColumn.DataType;

                                dgvOutputColumns.Rows[rowNumber].Cells[5].Value = outputColumn.Length;

                                dgvOutputColumns.Rows[rowNumber].Cells[6].Value = outputColumn.Precision;

                                dgvOutputColumns.Rows[rowNumber].Cells[7].Value = outputColumn.Scale;
                                SetOutputColumnsReadOnly(dgvOutputColumns.Rows[rowNumber]);
                            }
                            break;
                        case Utilities.typeOfOutputEnum.ErrorRecords:
                        case Utilities.typeOfOutputEnum.PassThrough:
                        case Utilities.typeOfOutputEnum.RowsProcessed:
                        default:
                            break;
                    }
                    break;
                }
            }
            _isLoading = false;
            dgvOutputColumns.ResumeLayout();
            DrawingControl.ResumeDrawing(dgvOutputPreview);
            DrawingControl.ResumeDrawing(dgvOutputColumns);
        }

        
        private void dgvOutputColumns_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // For some reason we are getting a failure on the Data Type column.
            // No Idea why..

            e.Cancel = true;
        }

        private void btnGenerateOutputs_Click(object sender, EventArgs e)
        {
            Dictionary<String, IDTSOutput100> validOutputs = new Dictionary<string, IDTSOutput100>();
            sampleDatas = new Dictionary<string, List<string>>();
            Dictionary<String, Boolean> newOutputs = new Dictionary<string,bool>();
            IDTSOutput100 passThoughIDTSOutput = null;
            int lastOutput = 0;
            Boolean isBroken = false;

            foreach (IDTSOutput100 output in _componentMetaData.OutputCollection)
            {
                String typeValue = (String)ManageProperties.GetPropertyValue(output.CustomPropertyCollection, ManageProperties.rowTypeValue);
                switch ((Utilities.typeOfOutputEnum)ManageProperties.GetPropertyValue(output.CustomPropertyCollection, ManageProperties.typeOfOutput))
                {
                    case Utilities.typeOfOutputEnum.ErrorRecords:
                        break;
                    case Utilities.typeOfOutputEnum.PassThrough:
                        passThoughIDTSOutput = output;
                        break;
                    case Utilities.typeOfOutputEnum.KeyRecords:
                        if (String.IsNullOrEmpty((String)ManageProperties.GetPropertyValue(output.CustomPropertyCollection, ManageProperties.rowTypeValue)))
                        {
                            isBroken = true;
                            break;
                        }
                        validOutputs.Add(typeValue, output);
                        sampleDatas.Add(typeValue, new List<string>());
                        newOutputs.Add(typeValue, false);
                        break;
                    case Utilities.typeOfOutputEnum.DataRecords:
                    case Utilities.typeOfOutputEnum.MasterRecord:
                    case Utilities.typeOfOutputEnum.ChildMasterRecord:
                    case Utilities.typeOfOutputEnum.ChildRecord:
                        validOutputs.Add(typeValue, output);
                        sampleDatas.Add(typeValue, new List<string>());
                        newOutputs.Add(typeValue, false);
                        break;
                    case Utilities.typeOfOutputEnum.RowsProcessed:
                        break;
                    default:
                        break;
                }
                lastOutput = output.ID;
            }
            if (isBroken)
            {
                MessageBox.Show("You MUST set the Selector Value for the Key Output.", "Selector Not Set", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            try
            {
                IDTSConnectionManagerFlatFile100 connectionFlatFile = connections[cbConnectionManager.SelectedItem].InnerObject as IDTSConnectionManagerFlatFile100;
                string FileName = connections[cbConnectionManager.SelectedItem].ConnectionString;

                SSISOutputColumn typeColumn = null;
                SSISOutputColumn valueColumn = null;

                SSISOutput passThroughOutput = new SSISOutput(passThoughIDTSOutput, null);
                bool firstRowColumnNames = connectionFlatFile.ColumnNamesInFirstDataRow;
                String passThroughClassString = Utilities.DynamicClassStringFromOutput(passThroughOutput, firstRowColumnNames, connectionFlatFile.Columns[connectionFlatFile.Columns.Count - 1].ColumnDelimiter, connectionFlatFile.Columns[0].ColumnDelimiter);
                Type passThroughType = ClassBuilder.ClassFromString(passThroughClassString);
                foreach (SSISOutputColumn ssisColumn in passThroughOutput.OutputColumnCollection)
                {
                    ssisColumn.FileHelperField = passThroughType.GetField(ssisColumn.Name);
                    if ((Utilities.usageOfColumnEnum)ManageProperties.GetPropertyValue(ssisColumn.CustomPropertyCollection, ManageProperties.usageOfColumn) == Utilities.usageOfColumnEnum.RowType)
                    {
                        typeColumn = ssisColumn;
                    }
                    else if ((Utilities.usageOfColumnEnum)ManageProperties.GetPropertyValue(ssisColumn.CustomPropertyCollection, ManageProperties.usageOfColumn) == Utilities.usageOfColumnEnum.RowData)
                    {
                        valueColumn = ssisColumn;
                    }
                }
                System.Reflection.FieldInfo[] fieldList = passThroughType.GetFields();
                FileHelperAsyncEngine engine = new FileHelperAsyncEngine(passThroughType);
                engine.BeginReadFile(FileName);

                int RowCount = 0;

                while (engine.ReadNext() != null)
                {
                    //int RowNumber = dgvPassThroughPreview.Rows.Add(engine.LastRecordValues);
                    //foreach (SSISOutputColumn ssisColumn in passThroughOutput.OutputColumnCollection)
                    //{
                    //    dgvConnectionPreview.Rows[RowNumber].Cells[ssisColumn.Name].Value = (String)ssisColumn.FileHelperField.GetValue(engine.LastRecord);
                    //}
                    String typeValue = (String)typeColumn.FileHelperField.GetValue(engine.LastRecord);
                    String valueValue = (String)valueColumn.FileHelperField.GetValue(engine.LastRecord);

                    if (!validOutputs.ContainsKey(typeValue))
                    {
                        IDTSOutput100 newOutput = designtimeComponent.InsertOutput(DTSInsertPlacement.IP_AFTER, lastOutput);
                        newOutput.Name = typeValue;
                        lastOutput = newOutput.ID;
                        ManageProperties.SetPropertyValue(newOutput.CustomPropertyCollection, ManageProperties.rowTypeValue, typeValue);

                        validOutputs.Add(typeValue, newOutput);
                        sampleDatas.Add(typeValue, new List<string>());
                        sampleDatas[typeValue].Add(valueValue);
                        newOutputs.Add(typeValue, true);
                        lbOutputs.Items.Add(typeValue);
                    }
                    else
                    {
                        if (sampleDatas[typeValue].Count < tbOutputNumberOfRecordsToPreview.Value)
                        {
                            sampleDatas[typeValue].Add(valueValue);
                        }
                    }
                    if (++RowCount >= tbGenerateOutputsNumber.Value)
                    {
                        break;
                    }
                }
                engine.Close();
                SmartFormatDetector dataDetector = new SmartFormatDetector();
                dataDetector.MaxSampleLines = (int)tbOutputNumberOfRecordsToPreview.Value;
                if (tbColumnDelimiter.Text == ",")
                    dataDetector.FormatHint = FormatHint.DelimitedByComma;
                else if (tbColumnDelimiter.Text == "\t")
                    dataDetector.FormatHint = FormatHint.DelimitedByTab;
                else if (tbColumnDelimiter.Text == ";")
                    dataDetector.FormatHint = FormatHint.DelimitedBySemicolon;
                else if (tbColumnDelimiter.Text == "~")
                    dataDetector.FormatHint = FormatHint.DelimitedByTilde;
                else
                    dataDetector.FormatHint = FormatHint.Delimited;

                foreach (KeyValuePair<String, Boolean> valuePair in newOutputs)
                {
                    
                    if ((valuePair.Value) || (validOutputs[valuePair.Key].OutputColumnCollection.Count == 0))
                    {
                        // Ok, we have a NEW output, or one that needs to be updated...!!!
                        RecordFormatInfo[] delimitedColumns = dataDetector.DetectStringFormat(sampleDatas[valuePair.Key]);
                        if (delimitedColumns.Length == 1)
                        {
                            if (delimitedColumns[0].ClassBuilderAsDelimited != null)
                            {
                                IDTSOutput100 currentOutput = validOutputs[valuePair.Key];
                                int columnPosition = currentOutput.OutputColumnCollection.Count;
                                // Something was found...
                                foreach (DelimitedFieldBuilder field in delimitedColumns[0].ClassBuilderAsDelimited.Fields)
                                {
                                    IDTSOutputColumn100 newColumn = designtimeComponent.InsertOutputColumnAt(currentOutput.ID, columnPosition++, field.FieldName, String.Empty);
                                }
                            }
                            else
                            {
                                MessageBox.Show(String.Format("Unable to understand data for output {0}.\r\nWas not detected as Delimited.\r\nPlease configure manually.", valuePair.Key), "Problems", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                        else if (delimitedColumns.Length > 0)
                        {
                            MessageBox.Show(String.Format("Unable to understand data for output {0}.\r\nPlease configure manually.", valuePair.Key), "Problems", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }

                //dataDetector.DetectStringFormat();
                MessageBox.Show(String.Format("Parsed {0} rows to determine outputs.", RowCount), "Done");
                lbOutputs_SelectedIndexChanged(sender, new EventArgs());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Something went Really Wrong!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvOutputColumns_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!_isLoading)
            {
                int outputID = (int)dgvOutputColumns.Rows[e.RowIndex].Tag;
                int outputColumnID = (int)dgvOutputColumns.Rows[e.RowIndex].Cells[0].Tag;
                IDTSOutput100 output = _componentMetaData.OutputCollection.GetObjectByID(outputID);
                IDTSOutputColumn100 outputColumn = output.OutputColumnCollection.GetObjectByID(outputColumnID);
                int Length = 0;
                int Scale = 0;
                int Precision = 0;
                int CodePage = 0;
                try
                {
                    switch (e.ColumnIndex)
                    {
                        case 0: // Column Name
                            outputColumn.Name = (String)dgvOutputColumns.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                            lbOutputs.SelectedItem = (String)dgvOutputColumns.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                            break;
                        case 1: // Usage
                            designtimeComponent.SetOutputColumnProperty(outputID, outputColumnID, ManageProperties.usageOfColumn, ConvertFromStringToUsageOfColumn((String)dgvOutputColumns.Rows[e.RowIndex].Cells[e.ColumnIndex].Value));
                            break;
                        case 2: // Format String
                            designtimeComponent.SetOutputColumnProperty(outputID, outputColumnID, ManageProperties.dotNetFormatString, (String)dgvOutputColumns.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
                            break;
                        case 4: // Data Type
                            if (outputColumn.DataType != (DataType)dgvOutputColumns.Rows[e.RowIndex].Cells[e.ColumnIndex].Value)
                            {
                                SetOutputColumnsReadWrite(dgvOutputColumns.Rows[e.RowIndex]);
                                SetDataTypeDefaults(dgvOutputColumns.Rows[e.RowIndex]);
                                Length = int.Parse(dgvOutputColumns.Rows[e.RowIndex].Cells[5].Value.ToString());
                                Scale = int.Parse(dgvOutputColumns.Rows[e.RowIndex].Cells[6].Value.ToString());
                                Precision = int.Parse(dgvOutputColumns.Rows[e.RowIndex].Cells[7].Value.ToString());
                                CodePage = int.Parse(dgvOutputColumns.Rows[e.RowIndex].Cells[3].Value.ToString());
                                SetOutputColumnsReadOnly(dgvOutputColumns.Rows[e.RowIndex]);
                                designtimeComponent.SetOutputColumnDataTypeProperties(outputID, outputColumnID, (DataType)dgvOutputColumns.Rows[e.RowIndex].Cells[4].Value, Length, Scale, Precision, CodePage);
                            }
                            break;
                        case 3: // Code Page
                        case 5: // Length
                        case 6: // Precision
                        case 7: // Scale
                                Length = int.Parse(dgvOutputColumns.Rows[e.RowIndex].Cells[5].Value.ToString());
                                Scale = int.Parse(dgvOutputColumns.Rows[e.RowIndex].Cells[6].Value.ToString());
                                Precision = int.Parse(dgvOutputColumns.Rows[e.RowIndex].Cells[7].Value.ToString());
                                CodePage = int.Parse(dgvOutputColumns.Rows[e.RowIndex].Cells[3].Value.ToString());
                                designtimeComponent.SetOutputColumnDataTypeProperties(outputID, outputColumnID, (DataType)dgvOutputColumns.Rows[e.RowIndex].Cells[4].Value, Length, Scale, Precision, CodePage);
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Not Applicable!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    lbOutputs_SelectedIndexChanged(sender, new EventArgs());
                }
            }
        }

        private void cbOutputType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_isLoading)
            {
                int outputID = (int)tbOutputName.Tag;
                try
                {
                    designtimeComponent.SetOutputProperty(outputID, ManageProperties.typeOfOutput, ConvertFromString((String)cbOutputType.SelectedItem));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Not Applicable!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    lbOutputs_SelectedIndexChanged(sender, new EventArgs());
                    return;
                }
                if ((ConvertFromString((String)cbOutputType.SelectedItem) == Utilities.typeOfOutputEnum.ChildRecord)
                    || (ConvertFromString((String)cbOutputType.SelectedItem) == Utilities.typeOfOutputEnum.ChildMasterRecord))
                {
                    List<String> masterNameList = new List<string>();
                    foreach (IDTSOutput100 output in _componentMetaData.OutputCollection)
                    {
                        Utilities.typeOfOutputEnum outputType = (Utilities.typeOfOutputEnum)ManageProperties.GetPropertyValue(output.CustomPropertyCollection, ManageProperties.typeOfOutput);
                        if ((outputType == Utilities.typeOfOutputEnum.MasterRecord) || (outputType == Utilities.typeOfOutputEnum.ChildMasterRecord))
                        {
                            masterNameList.Add(output.Name);
                        }
                    }
                    if (masterNameList.Count == 1)
                    {
                        tbMaster.Text = masterNameList[0];
                    }
                    else
                    {
                        if (masterNameList.Count > 1)
                        {
                            MasterSelection masterDialog = new MasterSelection();
                            masterDialog.initMasterList(masterNameList);
                            if (masterDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                tbMaster.Text = masterDialog.SelectedMaster;
                            }
                            else
                            {
                                _isLoading = true;
                                cbOutputType.SelectedItem = Enum.GetName(typeof(Utilities.typeOfOutputEnum), Utilities.typeOfOutputEnum.DataRecords);
                                _isLoading = false;
                                designtimeComponent.SetOutputProperty(outputID, ManageProperties.typeOfOutput, Utilities.typeOfOutputEnum.DataRecords);
                                lbOutputs_SelectedIndexChanged(sender, new EventArgs());
                                return;
                            }
                        }
                        else
                        {
                            designtimeComponent.SetOutputProperty(outputID, ManageProperties.typeOfOutput, Utilities.typeOfOutputEnum.PassThrough);
                            lbOutputs_SelectedIndexChanged(sender, new EventArgs());
                            return;
                        }
                    }
                    foreach (IDTSOutput100 output in _componentMetaData.OutputCollection)
                    {
                        Utilities.typeOfOutputEnum outputType = (Utilities.typeOfOutputEnum)ManageProperties.GetPropertyValue(output.CustomPropertyCollection, ManageProperties.typeOfOutput);
                        if ((outputType == Utilities.typeOfOutputEnum.MasterRecord) || (outputType == Utilities.typeOfOutputEnum.ChildMasterRecord))
                        {
                            if (output.Name == tbMaster.Text)
                            {
                                try
                                {
                                    designtimeComponent.SetOutputProperty(outputID, ManageProperties.masterRecordID, output.ID);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, "Not Applicable!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    tbMaster.Text = String.Empty;
                                    designtimeComponent.SetOutputProperty(outputID, ManageProperties.typeOfOutput, Utilities.typeOfOutputEnum.PassThrough);
                                    lbOutputs_SelectedIndexChanged(sender, new EventArgs());
                                    return;
                                }
                                break;
                            }
                        }
                    }
                }
                lbOutputs_SelectedIndexChanged(sender, new EventArgs());
            }
        }

        private void tbOutputName_Leave(object sender, EventArgs e)
        {
            if (!_isLoading)
            {
                try
                {
                    int outputID = (int)tbOutputName.Tag;
                    IDTSOutput100 output = _componentMetaData.OutputCollection.GetObjectByID(outputID);
                    if (output.Name != tbOutputName.Text)
                    {
                        output.Name = tbOutputName.Text;
                        lbOutputs.Items[lbOutputs.SelectedIndex] = tbOutputName.Text;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Not Applicable!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    lbOutputs_SelectedIndexChanged(sender, new EventArgs());
                }
            }
        }

        private void tbRowTypeValue_TextChanged(object sender, EventArgs e)
        {
            if (!_isLoading)
            {
                try
                {
                    int outputID = (int)tbOutputName.Tag;
                    designtimeComponent.SetOutputProperty(outputID, ManageProperties.rowTypeValue, tbRowTypeValue.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Not Applicable!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    lbOutputs_SelectedIndexChanged(sender, new EventArgs());
                }
            }
        }

        private void cbOutputDisposition_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_isLoading)
            {
                try
                {
                    int outputID = (int)tbOutputName.Tag;
                    IDTSOutput100 output = _componentMetaData.OutputCollection.GetObjectByID(outputID);
                    output.ErrorRowDisposition = (DTSRowDisposition)cbOutputDisposition.SelectedItem;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Not Applicable!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    lbOutputs_SelectedIndexChanged(sender, new EventArgs());
                }
            }
        }

        private void btnOutputPreview_Click(object sender, EventArgs e)
        {
            IDTSOutput100 passThoughIDTSOutput = null;
            int lastOutput = 0;

            if (!sampleDatas.ContainsKey(tbRowTypeValue.Text))
            {

                foreach (IDTSOutput100 output in _componentMetaData.OutputCollection)
                {
                    String typeValue = (String)ManageProperties.GetPropertyValue(output.CustomPropertyCollection, ManageProperties.rowTypeValue);
                    switch ((Utilities.typeOfOutputEnum)ManageProperties.GetPropertyValue(output.CustomPropertyCollection, ManageProperties.typeOfOutput))
                    {
                        case Utilities.typeOfOutputEnum.ErrorRecords:
                            break;
                        case Utilities.typeOfOutputEnum.PassThrough:
                            passThoughIDTSOutput = output;
                            break;
                        case Utilities.typeOfOutputEnum.KeyRecords:
                        case Utilities.typeOfOutputEnum.DataRecords:
                        case Utilities.typeOfOutputEnum.MasterRecord:
                        case Utilities.typeOfOutputEnum.ChildMasterRecord:
                        case Utilities.typeOfOutputEnum.ChildRecord:
                            sampleDatas.Add(typeValue, new List<string>());
                            break;
                        case Utilities.typeOfOutputEnum.RowsProcessed:
                            break;
                        default:
                            break;
                    }
                    lastOutput = output.ID;
                }

                try
                {
                    IDTSConnectionManagerFlatFile100 connectionFlatFile = connections[cbConnectionManager.SelectedItem].InnerObject as IDTSConnectionManagerFlatFile100;
                    string FileName = connections[cbConnectionManager.SelectedItem].ConnectionString;

                    SSISOutputColumn typeColumn = null;
                    SSISOutputColumn valueColumn = null;

                    SSISOutput passThroughOutput = new SSISOutput(passThoughIDTSOutput, null);
                    bool firstRowColumnNames = connectionFlatFile.ColumnNamesInFirstDataRow;
                    String passThroughClassString = Utilities.DynamicClassStringFromOutput(passThroughOutput, firstRowColumnNames, connectionFlatFile.Columns[connectionFlatFile.Columns.Count - 1].ColumnDelimiter, connectionFlatFile.Columns[0].ColumnDelimiter);
                    Type passThroughType = ClassBuilder.ClassFromString(passThroughClassString);
                    foreach (SSISOutputColumn ssisColumn in passThroughOutput.OutputColumnCollection)
                    {
                        ssisColumn.FileHelperField = passThroughType.GetField(ssisColumn.Name);
                        if ((Utilities.usageOfColumnEnum)ManageProperties.GetPropertyValue(ssisColumn.CustomPropertyCollection, ManageProperties.usageOfColumn) == Utilities.usageOfColumnEnum.RowType)
                        {
                            typeColumn = ssisColumn;
                        }
                        else if ((Utilities.usageOfColumnEnum)ManageProperties.GetPropertyValue(ssisColumn.CustomPropertyCollection, ManageProperties.usageOfColumn) == Utilities.usageOfColumnEnum.RowData)
                        {
                            valueColumn = ssisColumn;
                        }
                    }
                    System.Reflection.FieldInfo[] fieldList = passThroughType.GetFields();
                    FileHelperAsyncEngine engine = new FileHelperAsyncEngine(passThroughType);
                    engine.BeginReadFile(FileName);

                    int RowCount = 0;

                    while (engine.ReadNext() != null)
                    {
                        String typeValue = (String)typeColumn.FileHelperField.GetValue(engine.LastRecord);
                        String valueValue = (String)valueColumn.FileHelperField.GetValue(engine.LastRecord);

                        if (!sampleDatas.ContainsKey(typeValue))
                        {
                            sampleDatas.Add(typeValue, new List<string>());
                            sampleDatas[typeValue].Add(valueValue);
                        }
                        else
                        {
                            if (sampleDatas[typeValue].Count < tbOutputNumberOfRecordsToPreview.Value)
                            {
                                sampleDatas[typeValue].Add(valueValue);
                            }
                        }
                        if (++RowCount >= tbGenerateOutputsNumber.Value)
                        {
                            break;
                        }
                    }
                    engine.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Something went Really Wrong!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            if (sampleDatas.ContainsKey(tbRowTypeValue.Text))
            {
                try
                {
                    DrawingControl.SuspendDrawing(dgvOutputPreview);
                    int outputID = (int)tbOutputName.Tag;
                    IDTSOutput100 output = _componentMetaData.OutputCollection.GetObjectByID(outputID);
                    SSISOutput ssisOutput = new SSISOutput(output, null);
                    dgvOutputPreview.Columns.Clear();
                    dgvOutputPreview.Rows.Clear();
                    string classString = Utilities.DynamicClassStringFromOutput(ssisOutput, false, String.Empty, tbColumnDelimiter.Text);
                    Type classBuffer = ClassBuilder.ClassFromString(classString);
                    foreach (SSISOutputColumn ssisColumn in ssisOutput.OutputColumnCollection)
                    {
                        ssisColumn.FileHelperField = classBuffer.GetField(ssisColumn.Name);
                        if (!ssisColumn.IsDerived)
                        {
                            dgvOutputPreview.Columns.Add(ssisColumn.Name, ssisColumn.Name);
                        }
                    }
                    FileHelperEngine engine = new FileHelperEngine(classBuffer);
                    engine.ErrorManager.ErrorMode = ErrorMode.SaveAndContinue;
                    if (sampleDatas[tbRowTypeValue.Text].Count == 0)
                    {
                        int currentGridRow = dgvOutputPreview.Rows.Add();
                        dgvOutputPreview.Rows[currentGridRow].Cells[0].Value = String.Format("There were no records found in the first {0} lines of the file.  Change the value under the Generate Outputs button.", tbGenerateOutputsNumber.Value);
                    }
                    foreach (String RowDataValue in sampleDatas[tbRowTypeValue.Text])
                    {
                        List<Object> parseResults = engine.ReadStringAsList(RowDataValue);
                        if (engine.ErrorManager.HasErrors)
                        {
                            foreach(ErrorInfo errorInfo in engine.ErrorManager.Errors)
                            {
                                int currentGridRow = dgvOutputPreview.Rows.Add();
                                dgvOutputPreview.Rows[currentGridRow].Cells[0].Value = errorInfo.ExceptionInfo.Message;
                                dgvOutputPreview.Rows[currentGridRow].Cells[1].Value = RowDataValue;
                            }
                        }
                        foreach (Object result in parseResults)
                        {
                            int currentGridRow = dgvOutputPreview.Rows.Add();
                            int currentCell = 0;
                            foreach (SSISOutputColumn currentOutputColumn in ssisOutput.OutputColumnCollection)
                            {
                                if (!currentOutputColumn.IsDerived)
                                {
                                    Object currentValue = null;
                                    try
                                    {
                                        currentValue = currentOutputColumn.FileHelperField.GetValue(result);
                                        if (currentValue != null)
                                        {
                                            if (currentValue is String)
                                            {
                                                if (!String.IsNullOrEmpty((String)currentValue))
                                                {
                                                    dgvOutputPreview.Rows[currentGridRow].Cells[currentCell].Value = currentValue;
                                                }
                                            }
                                            else
                                            {
                                                dgvOutputPreview.Rows[currentGridRow].Cells[currentCell].Value = currentValue;
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show(ex.Message, "Parsing to Grid Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                    currentCell++;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Not Applicable!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    lbOutputs_SelectedIndexChanged(sender, new EventArgs());
                }
                finally
                {
                    DrawingControl.ResumeDrawing(dgvOutputPreview);
                }
            }
            else
            {
                MessageBox.Show(String.Format("The value {0} was not found as a Row Type.",tbRowTypeValue.Text), "Not Applicable!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tbGenerateOutputsNumber_ValueChanged(object sender, EventArgs e)
        {
            sampleDatas = new Dictionary<string, List<string>>();
        }

        private void btnAddColumn_Click(object sender, EventArgs e)
        {
            if (lbOutputs.SelectedItem != null)
            {
                try
                {
                    int lOutputID = (int)tbOutputName.Tag;
                    IDTSOutput100 currentOutput = this._componentMetaData.OutputCollection.GetObjectByID(lOutputID);
                    int lOutputColumnIndex = currentOutput.OutputColumnCollection.Count;
                    IDTSOutputColumn100 newColumn = this.designtimeComponent.InsertOutputColumnAt(lOutputID, lOutputColumnIndex, String.Format("ColumnName{0}", lOutputColumnIndex + 1), String.Empty);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "New Output Column Create Failed.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                lbOutputs_SelectedIndexChanged(sender, new EventArgs());
            }
        }

        private void btnRemoveColumn_Click(object sender, EventArgs e)
        {
            if ((dgvOutputColumns.CurrentRow != null) && (lbOutputs.SelectedItem != null))
            {
                try
                {
                    int lOutputID = (int)tbOutputName.Tag;
                    int lOutputColumnID = (int)dgvOutputColumns.CurrentRow.Cells[0].Tag;
                    this.designtimeComponent.DeleteOutputColumn(lOutputID, lOutputColumnID);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Not Applicable!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                lbOutputs_SelectedIndexChanged(sender, new EventArgs());
            }
        }

        private void btnAddOutput_Click(object sender, EventArgs e)
        {
            try
            {
                int lOutputID = 0;
                foreach (IDTSOutput100 output in this._componentMetaData.OutputCollection)
                {
                    if (output.ID > lOutputID)
                        lOutputID = output.ID;
                }
                IDTSOutput100 newOutput = this.designtimeComponent.InsertOutput(DTSInsertPlacement.IP_AFTER, lOutputID);
                newOutput.Name = String.Format("NewOutput{0}", newOutput.ID);
                int newItem = lbOutputs.Items.Add(newOutput.Name);
                lbOutputs.SelectedIndex = newItem;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "New Output Create Failed.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lbOutputs_SelectedIndexChanged(sender, new EventArgs());
            }
        }

        private void btnRemoveOutput_Click(object sender, EventArgs e)
        {
            Boolean deletedOutput = false;
            if (lbOutputs.SelectedItem != null)
            {
                try
                {
                    int lOutputID = (int)tbOutputName.Tag;
                    this.designtimeComponent.DeleteOutput(lOutputID);
                    deletedOutput = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Not Applicable!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    lbOutputs_SelectedIndexChanged(sender, new EventArgs());
                }
                if (deletedOutput)
                {
                    int selectedIndex = lbOutputs.SelectedIndex;
                    lbOutputs.Items.RemoveAt(selectedIndex);
                    lbOutputs.SelectedIndex = 0;
                }
            }
        }
        #endregion

    }
}
