using Microsoft.SqlServer.Dts.Design;
using Microsoft.SqlServer.Dts.Pipeline.Design;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Martin.SQLServer.Dts
{
    public abstract class DataFlowComponentUI : IDtsComponentUI
    {
        private IDTSComponentMetaData100 componentMetaData;
        private IDTSDesigntimeComponent100 designtimeComponent;
        private IDTSVirtualInput100 virtualInput;
        private IServiceProvider serviceProvider;
        private IErrorCollectionService errorCollector;
        private Connections connections;
        private Variables variables;

        #region Properties

        /// <summary>
        /// Gets the components metadata
        /// </summary>
        protected IDTSComponentMetaData100 ComponentMetadata
        {
            get
            {
                return this.componentMetaData;
            }
        }

        /// <summary>
        /// Gets the design time component
        /// </summary>
        protected IDTSDesigntimeComponent100 DesigntimeComponent
        {
            get
            {
                return this.designtimeComponent;
            }
        }

        /// <summary>
        /// Gets the virtual input
        /// </summary>
        protected IDTSVirtualInput100 VirtualInput
        {
            get
            {
                return this.virtualInput;
            }
        }

        /// <summary>
        /// Gets the service provider
        /// </summary>
        protected IServiceProvider ServiceProvider
        {
            get
            {
                return this.serviceProvider;
            }
        }

        /// <summary>
        /// Gets the connections
        /// </summary>
        protected Connections Connections
        {
            get
            {
                return this.connections;
            }
        }

        /// <summary>
        /// Gets the variables
        /// </summary>
        protected Variables Variables
        {
            get
            {
                return this.variables;
            }
        }

        #endregion

        #region IDtsComponentUI Members

        /// <summary>
        /// Called before Edit, New and Delete to pass in the necessary parameters.  
        /// </summary>
        /// <param name="dtsComponentMetadata">The components metadata</param>
        /// <param name="serviceProvider">The SSIS service provider</param>
        void IDtsComponentUI.Initialize(IDTSComponentMetaData100 dtsComponentMetadata, IServiceProvider serviceProvider)
        {
            Initialize(dtsComponentMetadata, serviceProvider);
        }

        protected void Initialize(IDTSComponentMetaData100 dtsComponentMetadata, IServiceProvider serviceProvider)
        {
            this.componentMetaData = dtsComponentMetadata;
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
        }

        /// <summary>
        /// Called to invoke the UI. 
        /// </summary>
        /// <param name="parentWindow">The calling window</param>
        /// <param name="variables">The SSIS variables</param>
        /// <param name="connections">The SSIS connections</param>
        /// <returns>True all works</returns>
        bool IDtsComponentUI.Edit(IWin32Window parentWindow, Microsoft.SqlServer.Dts.Runtime.Variables variables, Microsoft.SqlServer.Dts.Runtime.Connections connections)
        {
            return Edit(parentWindow, variables, connections);
        }

        protected bool Edit(IWin32Window parentWindow, Microsoft.SqlServer.Dts.Runtime.Variables variables, Microsoft.SqlServer.Dts.Runtime.Connections connections)
        {
            this.ClearErrors();

            try
            {
                Debug.Assert(this.componentMetaData != null, "Original Component Metadata is not OK.");

                this.designtimeComponent = this.componentMetaData.Instantiate();

                Debug.Assert(this.designtimeComponent != null, "Design-time component object is not OK.");

                // Cache the virtual input so the available columns are easily accessible.
                this.LoadVirtualInput();

                // Cache variables and connections.
                this.variables = variables;
                this.connections = connections;

                // Here comes the UI that will be invoked in EditImpl virtual method.
                return this.EditImpl(parentWindow);
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

        #region Virtual methods

        /// <summary>
        /// Bring up the form by implementing this method in subclasses. 
        /// </summary>
        /// <param name="parentControl">The caller's window id</param>
        /// <returns>True if all ok.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Impl")]
        protected abstract bool EditImpl(IWin32Window parentControl);

        #endregion

        #region Handling virtual inputs

        /// <summary>
        /// Loads all virtual inputs and makes their columns easily accessible.
        /// </summary>
        protected void LoadVirtualInput()
        {
            Debug.Assert(this.componentMetaData != null, "The passed in component metadata was null!");

            IDTSInputCollection100 inputCollection = this.componentMetaData.InputCollection;

            if (inputCollection.Count > 0)
            {
                IDTSInput100 input = inputCollection[0];
                this.virtualInput = input.GetVirtualInput();
            }
        }

        #endregion
    }
}
