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
    public class TestRowsProcessedOutput
    {
        [TestMethod]
        public void TestDeleteRowsProcessedOutput()
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
                instance.DeleteOutput(textFileSplitter.OutputCollection[3].ID);
            }
            catch (COMException ex)
            {
                Assert.AreEqual(MessageStrings.CannotDeleteRowsProcessedOutput, ex.Message, "Exception Message Wrong");
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "Exception Not Thrown");
        }

        #region TestChangeRowsProcessedOutputType
        [TestMethod]
        public void TestChangeRowsProcessedOutputTypeToKey()
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
                instance.SetOutputProperty(textFileSplitter.OutputCollection[3].ID, ManageProperties.typeOfOutput, Utilities.typeOfOutputEnum.KeyRecords);
            }
            catch (COMException ex)
            {
                Assert.AreEqual(MessageStrings.CantChangeOutputProperties("RowsProcessed"), ex.Message, "Exception Message Wrong");
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "Exception Not Thrown");
        }

        [TestMethod]
        public void TestChangeRowsProcessedOutputTypeToData()
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
                instance.SetOutputProperty(textFileSplitter.OutputCollection[3].ID, ManageProperties.typeOfOutput, Utilities.typeOfOutputEnum.DataRecords);
            }
            catch (COMException ex)
            {
                Assert.AreEqual(MessageStrings.CantChangeOutputProperties("RowsProcessed"), ex.Message, "Exception Message Wrong");
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "Exception Not Thrown");
        }

        [TestMethod]
        public void TestChangeRowsProcessedOutputTypeToPassThrough()
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
                instance.SetOutputProperty(textFileSplitter.OutputCollection[3].ID, ManageProperties.typeOfOutput, Utilities.typeOfOutputEnum.PassThrough);
            }
            catch (COMException ex)
            {
                Assert.AreEqual(MessageStrings.CantChangeOutputProperties("RowsProcessed"), ex.Message, "Exception Message Wrong");
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "Exception Not Thrown");
        }

        [TestMethod]
        public void TestChangeRowsProcessedOutputTypeToMaster()
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
                instance.SetOutputProperty(textFileSplitter.OutputCollection[3].ID, ManageProperties.typeOfOutput, Utilities.typeOfOutputEnum.MasterRecord);
            }
            catch (COMException ex)
            {
                Assert.AreEqual(MessageStrings.CantChangeOutputProperties("RowsProcessed"), ex.Message, "Exception Message Wrong");
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "Exception Not Thrown");
        }

        [TestMethod]
        public void TestChangeRowsProcessedOutputTypeToChildMaster()
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
                instance.SetOutputProperty(textFileSplitter.OutputCollection[3].ID, ManageProperties.typeOfOutput, Utilities.typeOfOutputEnum.ChildMasterRecord);
            }
            catch (COMException ex)
            {
                Assert.AreEqual(MessageStrings.CantChangeOutputProperties("RowsProcessed"), ex.Message, "Exception Message Wrong");
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "Exception Not Thrown");
        }

        [TestMethod]
        public void TestChangeRowsProcessedOutputTypeToChild()
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
                instance.SetOutputProperty(textFileSplitter.OutputCollection[3].ID, ManageProperties.typeOfOutput, Utilities.typeOfOutputEnum.ChildRecord);
            }
            catch (COMException ex)
            {
                Assert.AreEqual(MessageStrings.CantChangeOutputProperties("RowsProcessed"), ex.Message, "Exception Message Wrong");
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "Exception Not Thrown");
        }

        [TestMethod]
        public void TestChangeRowsProcessedOutputTypeToRows()
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
                instance.SetOutputProperty(textFileSplitter.OutputCollection[3].ID, ManageProperties.typeOfOutput, Utilities.typeOfOutputEnum.RowsProcessed);
            }
            catch (COMException ex)
            {
                Assert.AreEqual(MessageStrings.CantChangeOutputProperties("RowsProcessed"), ex.Message, "Exception Message Wrong");
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "Exception Not Thrown");
        }

        [TestMethod]
        public void TestChangeRowsProcessedOutputTypeToError()
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
                instance.SetOutputProperty(textFileSplitter.OutputCollection[3].ID, ManageProperties.typeOfOutput, Utilities.typeOfOutputEnum.ErrorRecords);
            }
            catch (COMException ex)
            {
                Assert.AreEqual(MessageStrings.CantChangeOutputProperties("RowsProcessed"), ex.Message, "Exception Message Wrong");
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "Exception Not Thrown");
        }

        #endregion

        [TestMethod]
        public void TestChangeRowsProcessedOutputMasterRecordID()
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
                instance.SetOutputProperty(textFileSplitter.OutputCollection[3].ID, ManageProperties.masterRecordID, 5);
            }
            catch (COMException ex)
            {
                Assert.AreEqual(MessageStrings.CantChangeOutputProperties("RowsProcessed"), ex.Message, "Exception Message Wrong");
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "Exception Not Thrown");
        }

        [TestMethod]
        public void TestChangeRowsProcessedOutputrowTypeValue()
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
                instance.SetOutputProperty(textFileSplitter.OutputCollection[3].ID, ManageProperties.rowTypeValue, "ERROR");
            }
            catch (COMException ex)
            {
                Assert.AreEqual(MessageStrings.CantChangeOutputProperties("RowsProcessed"), ex.Message, "Exception Message Wrong");
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "Exception Not Thrown");
        }

        [TestMethod]
        public void TestChangeRowsProcessedOutputAddColumn()
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
                instance.InsertOutputColumnAt(textFileSplitter.OutputCollection[3].ID, 0, "New Error Message", String.Empty);
            }
            catch (COMException ex)
            {
                Assert.AreEqual(MessageStrings.CantChangeOutputProperties("RowsProcessed"), ex.Message, "Exception Message Wrong");
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "Exception Not Thrown");
        }

        [TestMethod]
        public void TestChangeRowsProcessedOutputDeleteColumn()
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
                instance.DeleteOutputColumn(textFileSplitter.OutputCollection[3].ID, textFileSplitter.OutputCollection[3].OutputColumnCollection[0].ID);
            }
            catch (COMException ex)
            {
                Assert.AreEqual(MessageStrings.CantChangeOutputProperties("RowsProcessed"), ex.Message, "Exception Message Wrong");
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "Exception Not Thrown");
        }

        [TestMethod]
        public void TestChangeRowsProcessedOutputSetColumnProperty()
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

            IDTSOutput100 errorOutput = textFileSplitter.OutputCollection[3];
            ManageProperties.AddMissingOutputColumnProperties(errorOutput.OutputColumnCollection[0].CustomPropertyCollection);

            Boolean exceptionThrown = false;
            try
            {
                instance.SetOutputColumnProperty(errorOutput.ID, errorOutput.OutputColumnCollection[0].ID, ManageProperties.dotNetFormatString, String.Empty);
            }
            catch (COMException ex)
            {
                Assert.AreEqual(MessageStrings.CantChangeOutputProperties("RowsProcessed"), ex.Message, "Exception Message Wrong");
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "Exception Not Thrown");
        }
    }
}
