using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Martin.SQLServer.Dts;
using Microsoft.SqlServer.Dts.Pipeline;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime.Wrapper;
using Microsoft.SqlServer.Dts.Runtime;

namespace UnitTestTextFileSplitter2012
{
    [TestClass]
    public class TestManageColumns
    {
        [TestMethod]
        public void TestAddErrorOutputColumns()
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

            IDTSOutput100 actual = textFileSplitter.OutputCollection.New();
            ManageColumns.AddErrorOutputColumns(actual);

            Assert.AreEqual(3, actual.OutputColumnCollection.Count, "Number of Columns is wrong");
            Assert.AreEqual(MessageStrings.ErrorMessageColumnName, actual.OutputColumnCollection[0].Name, "Column Name is wrong");
            Assert.AreEqual(DataType.DT_WSTR, actual.OutputColumnCollection[0].DataType, "DataType is wrong");
            Assert.AreEqual(4000, actual.OutputColumnCollection[0].Length, "Length is wrong");
            Assert.AreEqual(MessageStrings.ColumnDataColumnName, actual.OutputColumnCollection[1].Name, "Column Name is wrong");
            Assert.AreEqual(DataType.DT_WSTR, actual.OutputColumnCollection[1].DataType, "DataType is wrong");
            Assert.AreEqual(4000, actual.OutputColumnCollection[1].Length, "Length is wrong");
            Assert.AreEqual(MessageStrings.RowDataColumnName, actual.OutputColumnCollection[2].Name, "Column Name is wrong");
            Assert.AreEqual(DataType.DT_WSTR, actual.OutputColumnCollection[2].DataType, "DataType is wrong");
            Assert.AreEqual(4000, actual.OutputColumnCollection[2].Length, "Length is wrong");
        }

        [TestMethod]
        public void TestSetOutputColumnDefaults()
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
            IDTSOutputColumn100 actual = output.OutputColumnCollection.New();
            ManageColumns.SetOutputColumnDefaults(actual, 1252);

            Assert.AreEqual(DataType.DT_STR, actual.DataType, "DataType is wrong");
            Assert.AreEqual(255, actual.Length, "Length is wrong");
            Assert.AreEqual(DTSRowDisposition.RD_NotUsed, actual.ErrorRowDisposition, "Row Disposition is wrong");
            Assert.AreEqual(DTSRowDisposition.RD_NotUsed, actual.TruncationRowDisposition, "Truncate Disposition is wrong");
        }

        [TestMethod]
        public void TestAddNumberOfRowsOutputColumns()
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
            IDTSOutput100 actual = textFileSplitter.OutputCollection.New();
            ManageColumns.AddNumberOfRowsOutputColumns(actual);

            Assert.AreEqual(3, actual.OutputColumnCollection.Count, "Number of Columns is wrong");
            Assert.AreEqual(MessageStrings.KeyValueColumnName, actual.OutputColumnCollection[0].Name, "Column Name is wrong");
            Assert.AreEqual(DataType.DT_STR, actual.OutputColumnCollection[0].DataType, "DataType is wrong");
            Assert.AreEqual(255, actual.OutputColumnCollection[0].Length, "Length is wrong");
            Assert.AreEqual(MessageStrings.KeyValueColumnDescription, actual.OutputColumnCollection[0].Description, "Description is wrong");
            Assert.AreEqual(MessageStrings.NumberOfRowsColumnName, actual.OutputColumnCollection[1].Name, "Column Name is wrong");
            Assert.AreEqual(DataType.DT_I8, actual.OutputColumnCollection[1].DataType, "DataType is wrong");
            Assert.AreEqual(0, actual.OutputColumnCollection[1].Length, "Length is wrong");
            Assert.AreEqual(MessageStrings.NumberOfRowsColumnDescription, actual.OutputColumnCollection[1].Description, "Description is wrong");
            Assert.AreEqual(MessageStrings.KeyValueStatusColumnName, actual.OutputColumnCollection[2].Name, "Column Name is wrong");
            Assert.AreEqual(DataType.DT_STR, actual.OutputColumnCollection[2].DataType, "DataType is wrong");
            Assert.AreEqual(255, actual.OutputColumnCollection[2].Length, "Length is wrong");
            Assert.AreEqual(MessageStrings.KeyValueStatusColumnDescription, actual.OutputColumnCollection[2].Description, "Description is wrong");
        }
    }
}
