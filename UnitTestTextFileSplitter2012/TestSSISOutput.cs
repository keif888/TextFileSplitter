using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Martin.SQLServer.Dts;
using Microsoft.SqlServer.Dts.Pipeline;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime.Wrapper;
using Microsoft.SqlServer.Dts.Runtime;
using System.Collections.Generic;

namespace UnitTestTextFileSplitter
{
    [TestClass]
    public class TestSSISOutput
    {

        [TestMethod]
        public void TestIDTSOutputCreator()
        {
            Microsoft.SqlServer.Dts.Runtime.Package package = new Microsoft.SqlServer.Dts.Runtime.Package();
            Executable exec = package.Executables.Add("STOCK:PipelineTask");
            Microsoft.SqlServer.Dts.Runtime.TaskHost thMainPipe = exec as Microsoft.SqlServer.Dts.Runtime.TaskHost;
            MainPipe dataFlowTask = thMainPipe.InnerObject as MainPipe;

            IDTSComponentMetaData100 textFileSplitter = dataFlowTask.ComponentMetaDataCollection.New();
            ComponentEventHandler events = new ComponentEventHandler();
            dataFlowTask.Events = DtsConvert.GetExtendedInterface(events as IDTSComponentEvents);
            textFileSplitter.Name = "Row Splitter Test";
            textFileSplitter.ComponentClassID = typeof(Martin.SQLServer.Dts.TextFileSplitter).AssemblyQualifiedName;
            CManagedComponentWrapper instance = textFileSplitter.Instantiate();

            instance.ProvideComponentProperties();

            IDTSOutput100 output = textFileSplitter.OutputCollection.New();
            output.Name = "New # Output";
            output.ErrorRowDisposition = DTSRowDisposition.RD_NotUsed;

            ManageProperties.AddOutputProperties(output.CustomPropertyCollection);

            ManageColumns.AddNumberOfRowsOutputColumns(output);

            ManageProperties propertyManager = new ManageProperties();
            propertyManager.PostErrorEvent += new PostErrorDelegate(this.PostError);
            errorMessages = new List<string>();

            SSISOutput actual = new SSISOutput(output, null);

            Assert.IsNotNull(actual, "SSISOutput is null");
            Assert.IsNotNull(actual.CustomPropertyCollection, "Custom Property Collection is null");
            Assert.IsNotNull(actual.OutputColumnCollection, "Column Collection is null");
            Assert.AreEqual(3, actual.CustomPropertyCollection.Count, "Custom Property Collection Count is Wrong");
            Assert.AreEqual(3, actual.OutputColumnCollection.Count, "Output Column Collection Count is Wrong");
            Assert.IsNotNull(ManageProperties.GetPropertyValue(actual.CustomPropertyCollection, ManageProperties.typeOfOutput));
            Assert.IsNotNull(ManageProperties.GetPropertyValue(actual.CustomPropertyCollection, ManageProperties.rowTypeValue));
            Assert.IsNotNull(ManageProperties.GetPropertyValue(actual.CustomPropertyCollection, ManageProperties.masterRecordID));
            Assert.AreEqual("_NewOutput", actual.Name, "Name is incorrect");
            Assert.AreEqual(DTSRowDisposition.RD_NotUsed, actual.ErrorRowDisposition, "Error Row Disposition is incorrect");
        }




        #region Error Delegate

        private List<String> errorMessages;

        void PostError(string errorMessage)
        {
            errorMessages.Add(errorMessage);
        }
        #endregion
    }
}
