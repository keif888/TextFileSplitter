using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Martin.SQLServer.Dts;
using Microsoft.SqlServer.Dts.Pipeline;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime.Wrapper;
using Microsoft.SqlServer.Dts.Runtime;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace UnitTestTextFileSplitter2012
{
    [TestClass]
    public class TestErrorRecordOutput
    {
        [TestMethod]
        public void TestDeleteErrorOutput()
        {
            Microsoft.SqlServer.Dts.Runtime.Package package = new Microsoft.SqlServer.Dts.Runtime.Package();
            Executable exec = package.Executables.Add("STOCK:PipelineTask");
            Microsoft.SqlServer.Dts.Runtime.TaskHost thMainPipe = exec as Microsoft.SqlServer.Dts.Runtime.TaskHost;
            MainPipe dataFlowTask = thMainPipe.InnerObject as MainPipe;
            ComponentEventHandler events = new ComponentEventHandler();
            dataFlowTask.Events = DtsConvert.GetExtendedInterface(events as IDTSComponentEvents);

            IDTSComponentMetaData100 textFileSplitter = dataFlowTask.ComponentMetaDataCollection.New();
            textFileSplitter.Name = "Row Splitter Test";
            textFileSplitter.ComponentClassID = typeof(Martin.SQLServer.Dts.TextFileSplitter).AssemblyQualifiedName;
            CManagedComponentWrapper instance = textFileSplitter.Instantiate();

            instance.ProvideComponentProperties();

            Boolean exceptionThrown = false;
            try
            {
                instance.DeleteOutput(textFileSplitter.OutputCollection[1].ID);
            }
            catch (COMException ex)
            {
                Assert.AreEqual(MessageStrings.CannotDeleteErrorOutput, ex.Message, "Exception Message Wrong");
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "Exception Not Thrown");
        }

        #region TestChangeErrorOutputType
        [TestMethod]
        public void TestChangeErrorOutputTypeToKey()
        {
            Microsoft.SqlServer.Dts.Runtime.Package package = new Microsoft.SqlServer.Dts.Runtime.Package();
            Executable exec = package.Executables.Add("STOCK:PipelineTask");
            Microsoft.SqlServer.Dts.Runtime.TaskHost thMainPipe = exec as Microsoft.SqlServer.Dts.Runtime.TaskHost;
            MainPipe dataFlowTask = thMainPipe.InnerObject as MainPipe;
            ComponentEventHandler events = new ComponentEventHandler();
            dataFlowTask.Events = DtsConvert.GetExtendedInterface(events as IDTSComponentEvents);

            IDTSComponentMetaData100 textFileSplitter = dataFlowTask.ComponentMetaDataCollection.New();
            textFileSplitter.Name = "Row Splitter Test";
            textFileSplitter.ComponentClassID = typeof(Martin.SQLServer.Dts.TextFileSplitter).AssemblyQualifiedName;
            CManagedComponentWrapper instance = textFileSplitter.Instantiate();

            instance.ProvideComponentProperties();

            Boolean exceptionThrown = false;
            try
            {
                instance.SetOutputProperty(textFileSplitter.OutputCollection[1].ID, ManageProperties.typeOfOutput, Utilities.typeOfOutputEnum.KeyRecords);
            }
            catch (COMException ex)
            {
                Assert.AreEqual(MessageStrings.CantChangeOutputProperties("Error"), ex.Message, "Exception Message Wrong");
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "Exception Not Thrown");
        }

        [TestMethod]
        public void TestChangeErrorOutputTypeToData()
        {
            Microsoft.SqlServer.Dts.Runtime.Package package = new Microsoft.SqlServer.Dts.Runtime.Package();
            Executable exec = package.Executables.Add("STOCK:PipelineTask");
            Microsoft.SqlServer.Dts.Runtime.TaskHost thMainPipe = exec as Microsoft.SqlServer.Dts.Runtime.TaskHost;
            MainPipe dataFlowTask = thMainPipe.InnerObject as MainPipe;
            ComponentEventHandler events = new ComponentEventHandler();
            dataFlowTask.Events = DtsConvert.GetExtendedInterface(events as IDTSComponentEvents);

            IDTSComponentMetaData100 textFileSplitter = dataFlowTask.ComponentMetaDataCollection.New();
            textFileSplitter.Name = "Row Splitter Test";
            textFileSplitter.ComponentClassID = typeof(Martin.SQLServer.Dts.TextFileSplitter).AssemblyQualifiedName;
            CManagedComponentWrapper instance = textFileSplitter.Instantiate();

            instance.ProvideComponentProperties();

            Boolean exceptionThrown = false;
            try
            {
                instance.SetOutputProperty(textFileSplitter.OutputCollection[1].ID, ManageProperties.typeOfOutput, Utilities.typeOfOutputEnum.DataRecords);
            }
            catch (COMException ex)
            {
                Assert.AreEqual(MessageStrings.CantChangeOutputProperties("Error"), ex.Message, "Exception Message Wrong");
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "Exception Not Thrown");
        }

        [TestMethod]
        public void TestChangeErrorOutputTypeToPassThrough()
        {
            Microsoft.SqlServer.Dts.Runtime.Package package = new Microsoft.SqlServer.Dts.Runtime.Package();
            Executable exec = package.Executables.Add("STOCK:PipelineTask");
            Microsoft.SqlServer.Dts.Runtime.TaskHost thMainPipe = exec as Microsoft.SqlServer.Dts.Runtime.TaskHost;
            MainPipe dataFlowTask = thMainPipe.InnerObject as MainPipe;
            ComponentEventHandler events = new ComponentEventHandler();
            dataFlowTask.Events = DtsConvert.GetExtendedInterface(events as IDTSComponentEvents);

            IDTSComponentMetaData100 textFileSplitter = dataFlowTask.ComponentMetaDataCollection.New();
            textFileSplitter.Name = "Row Splitter Test";
            textFileSplitter.ComponentClassID = typeof(Martin.SQLServer.Dts.TextFileSplitter).AssemblyQualifiedName;
            CManagedComponentWrapper instance = textFileSplitter.Instantiate();

            instance.ProvideComponentProperties();

            Boolean exceptionThrown = false;
            try
            {
                instance.SetOutputProperty(textFileSplitter.OutputCollection[1].ID, ManageProperties.typeOfOutput, Utilities.typeOfOutputEnum.PassThrough);
            }
            catch (COMException ex)
            {
                Assert.AreEqual(MessageStrings.CantChangeOutputProperties("Error"), ex.Message, "Exception Message Wrong");
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "Exception Not Thrown");
        }

        [TestMethod]
        public void TestChangeErrorOutputTypeToMaster()
        {
            Microsoft.SqlServer.Dts.Runtime.Package package = new Microsoft.SqlServer.Dts.Runtime.Package();
            Executable exec = package.Executables.Add("STOCK:PipelineTask");
            Microsoft.SqlServer.Dts.Runtime.TaskHost thMainPipe = exec as Microsoft.SqlServer.Dts.Runtime.TaskHost;
            MainPipe dataFlowTask = thMainPipe.InnerObject as MainPipe;
            ComponentEventHandler events = new ComponentEventHandler();
            dataFlowTask.Events = DtsConvert.GetExtendedInterface(events as IDTSComponentEvents);

            IDTSComponentMetaData100 textFileSplitter = dataFlowTask.ComponentMetaDataCollection.New();
            textFileSplitter.Name = "Row Splitter Test";
            textFileSplitter.ComponentClassID = typeof(Martin.SQLServer.Dts.TextFileSplitter).AssemblyQualifiedName;
            CManagedComponentWrapper instance = textFileSplitter.Instantiate();

            instance.ProvideComponentProperties();

            Boolean exceptionThrown = false;
            try
            {
                instance.SetOutputProperty(textFileSplitter.OutputCollection[1].ID, ManageProperties.typeOfOutput, Utilities.typeOfOutputEnum.MasterRecord);
            }
            catch (COMException ex)
            {
                Assert.AreEqual(MessageStrings.CantChangeOutputProperties("Error"), ex.Message, "Exception Message Wrong");
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "Exception Not Thrown");
        }

        [TestMethod]
        public void TestChangeErrorOutputTypeToChildMaster()
        {
            Microsoft.SqlServer.Dts.Runtime.Package package = new Microsoft.SqlServer.Dts.Runtime.Package();
            Executable exec = package.Executables.Add("STOCK:PipelineTask");
            Microsoft.SqlServer.Dts.Runtime.TaskHost thMainPipe = exec as Microsoft.SqlServer.Dts.Runtime.TaskHost;
            MainPipe dataFlowTask = thMainPipe.InnerObject as MainPipe;
            ComponentEventHandler events = new ComponentEventHandler();
            dataFlowTask.Events = DtsConvert.GetExtendedInterface(events as IDTSComponentEvents);

            IDTSComponentMetaData100 textFileSplitter = dataFlowTask.ComponentMetaDataCollection.New();
            textFileSplitter.Name = "Row Splitter Test";
            textFileSplitter.ComponentClassID = typeof(Martin.SQLServer.Dts.TextFileSplitter).AssemblyQualifiedName;
            CManagedComponentWrapper instance = textFileSplitter.Instantiate();

            instance.ProvideComponentProperties();

            Boolean exceptionThrown = false;
            try
            {
                instance.SetOutputProperty(textFileSplitter.OutputCollection[1].ID, ManageProperties.typeOfOutput, Utilities.typeOfOutputEnum.ChildMasterRecord);
            }
            catch (COMException ex)
            {
                Assert.AreEqual(MessageStrings.CantChangeOutputProperties("Error"), ex.Message, "Exception Message Wrong");
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "Exception Not Thrown");
        }

        [TestMethod]
        public void TestChangeErrorOutputTypeToChild()
        {
            Microsoft.SqlServer.Dts.Runtime.Package package = new Microsoft.SqlServer.Dts.Runtime.Package();
            Executable exec = package.Executables.Add("STOCK:PipelineTask");
            Microsoft.SqlServer.Dts.Runtime.TaskHost thMainPipe = exec as Microsoft.SqlServer.Dts.Runtime.TaskHost;
            MainPipe dataFlowTask = thMainPipe.InnerObject as MainPipe;
            ComponentEventHandler events = new ComponentEventHandler();
            dataFlowTask.Events = DtsConvert.GetExtendedInterface(events as IDTSComponentEvents);

            IDTSComponentMetaData100 textFileSplitter = dataFlowTask.ComponentMetaDataCollection.New();
            textFileSplitter.Name = "Row Splitter Test";
            textFileSplitter.ComponentClassID = typeof(Martin.SQLServer.Dts.TextFileSplitter).AssemblyQualifiedName;
            CManagedComponentWrapper instance = textFileSplitter.Instantiate();

            instance.ProvideComponentProperties();

            Boolean exceptionThrown = false;
            try
            {
                instance.SetOutputProperty(textFileSplitter.OutputCollection[1].ID, ManageProperties.typeOfOutput, Utilities.typeOfOutputEnum.ChildRecord);
            }
            catch (COMException ex)
            {
                Assert.AreEqual(MessageStrings.CantChangeOutputProperties("Error"), ex.Message, "Exception Message Wrong");
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "Exception Not Thrown");
        }

        [TestMethod]
        public void TestChangeErrorOutputTypeToRows()
        {
            Microsoft.SqlServer.Dts.Runtime.Package package = new Microsoft.SqlServer.Dts.Runtime.Package();
            Executable exec = package.Executables.Add("STOCK:PipelineTask");
            Microsoft.SqlServer.Dts.Runtime.TaskHost thMainPipe = exec as Microsoft.SqlServer.Dts.Runtime.TaskHost;
            MainPipe dataFlowTask = thMainPipe.InnerObject as MainPipe;
            ComponentEventHandler events = new ComponentEventHandler();
            dataFlowTask.Events = DtsConvert.GetExtendedInterface(events as IDTSComponentEvents);

            IDTSComponentMetaData100 textFileSplitter = dataFlowTask.ComponentMetaDataCollection.New();
            textFileSplitter.Name = "Row Splitter Test";
            textFileSplitter.ComponentClassID = typeof(Martin.SQLServer.Dts.TextFileSplitter).AssemblyQualifiedName;
            CManagedComponentWrapper instance = textFileSplitter.Instantiate();

            instance.ProvideComponentProperties();

            Boolean exceptionThrown = false;
            try
            {
                instance.SetOutputProperty(textFileSplitter.OutputCollection[1].ID, ManageProperties.typeOfOutput, Utilities.typeOfOutputEnum.RowsProcessed);
            }
            catch (COMException ex)
            {
                Assert.AreEqual(MessageStrings.CantChangeOutputProperties("Error"), ex.Message, "Exception Message Wrong");
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "Exception Not Thrown");
        }

        [TestMethod]
        public void TestChangeErrorOutputTypeToError()
        {
            Microsoft.SqlServer.Dts.Runtime.Package package = new Microsoft.SqlServer.Dts.Runtime.Package();
            Executable exec = package.Executables.Add("STOCK:PipelineTask");
            Microsoft.SqlServer.Dts.Runtime.TaskHost thMainPipe = exec as Microsoft.SqlServer.Dts.Runtime.TaskHost;
            MainPipe dataFlowTask = thMainPipe.InnerObject as MainPipe;
            ComponentEventHandler events = new ComponentEventHandler();
            dataFlowTask.Events = DtsConvert.GetExtendedInterface(events as IDTSComponentEvents);

            IDTSComponentMetaData100 textFileSplitter = dataFlowTask.ComponentMetaDataCollection.New();
            textFileSplitter.Name = "Row Splitter Test";
            textFileSplitter.ComponentClassID = typeof(Martin.SQLServer.Dts.TextFileSplitter).AssemblyQualifiedName;
            CManagedComponentWrapper instance = textFileSplitter.Instantiate();

            instance.ProvideComponentProperties();

            Boolean exceptionThrown = false;
            try
            {
                instance.SetOutputProperty(textFileSplitter.OutputCollection[1].ID, ManageProperties.typeOfOutput, Utilities.typeOfOutputEnum.ErrorRecords);
            }
            catch (COMException ex)
            {
                Assert.AreEqual(MessageStrings.CantChangeOutputProperties("Error"), ex.Message, "Exception Message Wrong");
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "Exception Not Thrown");
        }

        #endregion

        [TestMethod]
        public void TestChangeErrorOutputMasterRecordID()
        {
            Microsoft.SqlServer.Dts.Runtime.Package package = new Microsoft.SqlServer.Dts.Runtime.Package();
            Executable exec = package.Executables.Add("STOCK:PipelineTask");
            Microsoft.SqlServer.Dts.Runtime.TaskHost thMainPipe = exec as Microsoft.SqlServer.Dts.Runtime.TaskHost;
            MainPipe dataFlowTask = thMainPipe.InnerObject as MainPipe;
            ComponentEventHandler events = new ComponentEventHandler();
            dataFlowTask.Events = DtsConvert.GetExtendedInterface(events as IDTSComponentEvents);

            IDTSComponentMetaData100 textFileSplitter = dataFlowTask.ComponentMetaDataCollection.New();
            textFileSplitter.Name = "Row Splitter Test";
            textFileSplitter.ComponentClassID = typeof(Martin.SQLServer.Dts.TextFileSplitter).AssemblyQualifiedName;
            CManagedComponentWrapper instance = textFileSplitter.Instantiate();

            instance.ProvideComponentProperties();

            Boolean exceptionThrown = false;
            try
            {
                instance.SetOutputProperty(textFileSplitter.OutputCollection[1].ID, ManageProperties.masterRecordID, 5);
            }
            catch (COMException ex)
            {
                Assert.AreEqual(MessageStrings.CantChangeOutputProperties("Error"), ex.Message, "Exception Message Wrong");
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "Exception Not Thrown");
        }

        [TestMethod]
        public void TestChangeErrorOutputrowTypeValue()
        {
            Microsoft.SqlServer.Dts.Runtime.Package package = new Microsoft.SqlServer.Dts.Runtime.Package();
            Executable exec = package.Executables.Add("STOCK:PipelineTask");
            Microsoft.SqlServer.Dts.Runtime.TaskHost thMainPipe = exec as Microsoft.SqlServer.Dts.Runtime.TaskHost;
            MainPipe dataFlowTask = thMainPipe.InnerObject as MainPipe;
            ComponentEventHandler events = new ComponentEventHandler();
            dataFlowTask.Events = DtsConvert.GetExtendedInterface(events as IDTSComponentEvents);

            IDTSComponentMetaData100 textFileSplitter = dataFlowTask.ComponentMetaDataCollection.New();
            textFileSplitter.Name = "Row Splitter Test";
            textFileSplitter.ComponentClassID = typeof(Martin.SQLServer.Dts.TextFileSplitter).AssemblyQualifiedName;
            CManagedComponentWrapper instance = textFileSplitter.Instantiate();

            instance.ProvideComponentProperties();

            Boolean exceptionThrown = false;
            try
            {
                instance.SetOutputProperty(textFileSplitter.OutputCollection[1].ID, ManageProperties.rowTypeValue, "ERROR");
            }
            catch (COMException ex)
            {
                Assert.AreEqual(MessageStrings.CantChangeOutputProperties("Error"), ex.Message, "Exception Message Wrong");
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "Exception Not Thrown");
        }

        [TestMethod]
        public void TestChangeErrorOutputAddColumn()
        {
            Microsoft.SqlServer.Dts.Runtime.Package package = new Microsoft.SqlServer.Dts.Runtime.Package();
            Executable exec = package.Executables.Add("STOCK:PipelineTask");
            Microsoft.SqlServer.Dts.Runtime.TaskHost thMainPipe = exec as Microsoft.SqlServer.Dts.Runtime.TaskHost;
            MainPipe dataFlowTask = thMainPipe.InnerObject as MainPipe;
            ComponentEventHandler events = new ComponentEventHandler();
            dataFlowTask.Events = DtsConvert.GetExtendedInterface(events as IDTSComponentEvents);

            IDTSComponentMetaData100 textFileSplitter = dataFlowTask.ComponentMetaDataCollection.New();
            textFileSplitter.Name = "Row Splitter Test";
            textFileSplitter.ComponentClassID = typeof(Martin.SQLServer.Dts.TextFileSplitter).AssemblyQualifiedName;
            CManagedComponentWrapper instance = textFileSplitter.Instantiate();

            instance.ProvideComponentProperties();

            Boolean exceptionThrown = false;
            try
            {
                instance.InsertOutputColumnAt(textFileSplitter.OutputCollection[1].ID, 0, "New Error Message", String.Empty);
            }
            catch (COMException ex)
            {
                Assert.AreEqual(MessageStrings.CantChangeOutputProperties("Error"), ex.Message, "Exception Message Wrong");
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "Exception Not Thrown");
        }

        [TestMethod]
        public void TestChangeErrorOutputDeleteColumn()
        {
            Microsoft.SqlServer.Dts.Runtime.Package package = new Microsoft.SqlServer.Dts.Runtime.Package();
            Executable exec = package.Executables.Add("STOCK:PipelineTask");
            Microsoft.SqlServer.Dts.Runtime.TaskHost thMainPipe = exec as Microsoft.SqlServer.Dts.Runtime.TaskHost;
            MainPipe dataFlowTask = thMainPipe.InnerObject as MainPipe;
            ComponentEventHandler events = new ComponentEventHandler();
            dataFlowTask.Events = DtsConvert.GetExtendedInterface(events as IDTSComponentEvents);

            IDTSComponentMetaData100 textFileSplitter = dataFlowTask.ComponentMetaDataCollection.New();
            textFileSplitter.Name = "Row Splitter Test";
            textFileSplitter.ComponentClassID = typeof(Martin.SQLServer.Dts.TextFileSplitter).AssemblyQualifiedName;
            CManagedComponentWrapper instance = textFileSplitter.Instantiate();

            instance.ProvideComponentProperties();

            Boolean exceptionThrown = false;
            try
            {
                instance.DeleteOutputColumn(textFileSplitter.OutputCollection[1].ID, textFileSplitter.OutputCollection[1].OutputColumnCollection[0].ID);
            }
            catch (COMException ex)
            {
                Assert.AreEqual(MessageStrings.CantChangeOutputProperties("Error"), ex.Message, "Exception Message Wrong");
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "Exception Not Thrown");
        }

        [TestMethod]
        public void TestChangeErrorOutputSetColumnProperty()
        {
            Microsoft.SqlServer.Dts.Runtime.Package package = new Microsoft.SqlServer.Dts.Runtime.Package();
            Executable exec = package.Executables.Add("STOCK:PipelineTask");
            Microsoft.SqlServer.Dts.Runtime.TaskHost thMainPipe = exec as Microsoft.SqlServer.Dts.Runtime.TaskHost;
            MainPipe dataFlowTask = thMainPipe.InnerObject as MainPipe;
            ComponentEventHandler events = new ComponentEventHandler();
            dataFlowTask.Events = DtsConvert.GetExtendedInterface(events as IDTSComponentEvents);

            IDTSComponentMetaData100 textFileSplitter = dataFlowTask.ComponentMetaDataCollection.New();
            textFileSplitter.Name = "Row Splitter Test";
            textFileSplitter.ComponentClassID = typeof(Martin.SQLServer.Dts.TextFileSplitter).AssemblyQualifiedName;
            CManagedComponentWrapper instance = textFileSplitter.Instantiate();

            instance.ProvideComponentProperties();

            IDTSOutput100 errorOutput = textFileSplitter.OutputCollection[1];
            ManageProperties.AddMissingOutputColumnProperties(errorOutput.OutputColumnCollection[0].CustomPropertyCollection);

            Boolean exceptionThrown = false;
            try
            {
                instance.SetOutputColumnProperty(errorOutput.ID, errorOutput.OutputColumnCollection[0].ID, ManageProperties.dotNetFormatString, String.Empty);
            }
            catch (COMException ex)
            {
                Assert.AreEqual(MessageStrings.CantChangeOutputProperties("Error"), ex.Message, "Exception Message Wrong");
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "Exception Not Thrown");
        }

        [TestMethod]
        public void TestChangeErrorOutputSetColumnDataType()
        {
            Microsoft.SqlServer.Dts.Runtime.Package package = new Microsoft.SqlServer.Dts.Runtime.Package();
            Executable exec = package.Executables.Add("STOCK:PipelineTask");
            Microsoft.SqlServer.Dts.Runtime.TaskHost thMainPipe = exec as Microsoft.SqlServer.Dts.Runtime.TaskHost;
            MainPipe dataFlowTask = thMainPipe.InnerObject as MainPipe;
            ComponentEventHandler events = new ComponentEventHandler();
            dataFlowTask.Events = DtsConvert.GetExtendedInterface(events as IDTSComponentEvents);

            IDTSComponentMetaData100 textFileSplitter = dataFlowTask.ComponentMetaDataCollection.New();
            textFileSplitter.Name = "Row Splitter Test";
            textFileSplitter.ComponentClassID = typeof(Martin.SQLServer.Dts.TextFileSplitter).AssemblyQualifiedName;
            CManagedComponentWrapper instance = textFileSplitter.Instantiate();

            instance.ProvideComponentProperties();

            IDTSOutput100 errorOutput = textFileSplitter.OutputCollection[1];

            Boolean exceptionThrown = false;
            try
            {
                instance.SetOutputColumnDataTypeProperties(errorOutput.ID, errorOutput.OutputColumnCollection[0].ID, DataType.DT_STR, 512, 0, 0, 1252);
            }
            catch (COMException ex)
            {
                Assert.AreEqual(MessageStrings.CantChangeOutputProperties("Error"), ex.Message, "Exception Message Wrong");
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "Exception Not Thrown");
        }
    }
}
