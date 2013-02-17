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
    public class TestKeyRecordOutput
    {
        [TestMethod]
        public void TestDeleteKeyOutput()
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
                instance.DeleteOutput(textFileSplitter.OutputCollection[2].ID);
            }
            catch (COMException ex)
            {
                Assert.AreEqual(MessageStrings.CannotDeleteKeyOutput, ex.Message, "Exception Message Wrong");
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "Exception Not Thrown");
        }

        #region Change typeOfOutput to anything else (Exception)
        [TestMethod]
        public void TestChangeKeyOutputTypeToKey()
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
                instance.SetOutputProperty(textFileSplitter.OutputCollection[2].ID, ManageProperties.typeOfOutput, Utilities.typeOfOutputEnum.KeyRecords);
            }
            catch (COMException ex)
            {
                Assert.AreEqual(MessageStrings.CantChangeOutputProperties("Key"), ex.Message, "Exception Message Wrong");
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "Exception Not Thrown");
        }

        [TestMethod]
        public void TestChangeKeyOutputTypeToData()
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
                instance.SetOutputProperty(textFileSplitter.OutputCollection[2].ID, ManageProperties.typeOfOutput, Utilities.typeOfOutputEnum.DataRecords);
            }
            catch (COMException ex)
            {
                Assert.AreEqual(MessageStrings.CantChangeOutputProperties("Key"), ex.Message, "Exception Message Wrong");
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "Exception Not Thrown");
        }

        [TestMethod]
        public void TestChangeKeyOutputTypeToPassThrough()
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
                instance.SetOutputProperty(textFileSplitter.OutputCollection[2].ID, ManageProperties.typeOfOutput, Utilities.typeOfOutputEnum.PassThrough);
            }
            catch (COMException ex)
            {
                Assert.AreEqual(MessageStrings.CantChangeOutputProperties("Key"), ex.Message, "Exception Message Wrong");
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "Exception Not Thrown");
        }

        [TestMethod]
        public void TestChangeKeyOutputTypeToMaster()
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
                instance.SetOutputProperty(textFileSplitter.OutputCollection[2].ID, ManageProperties.typeOfOutput, Utilities.typeOfOutputEnum.MasterRecord);
            }
            catch (COMException ex)
            {
                Assert.AreEqual(MessageStrings.CantChangeOutputProperties("Key"), ex.Message, "Exception Message Wrong");
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "Exception Not Thrown");
        }

        [TestMethod]
        public void TestChangeKeyOutputTypeToChildMaster()
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
                instance.SetOutputProperty(textFileSplitter.OutputCollection[2].ID, ManageProperties.typeOfOutput, Utilities.typeOfOutputEnum.ChildMasterRecord);
            }
            catch (COMException ex)
            {
                Assert.AreEqual(MessageStrings.CantChangeOutputProperties("Key"), ex.Message, "Exception Message Wrong");
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "Exception Not Thrown");
        }

        [TestMethod]
        public void TestChangeKeyOutputTypeToChild()
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
                instance.SetOutputProperty(textFileSplitter.OutputCollection[2].ID, ManageProperties.typeOfOutput, Utilities.typeOfOutputEnum.ChildRecord);
            }
            catch (COMException ex)
            {
                Assert.AreEqual(MessageStrings.CantChangeOutputProperties("Key"), ex.Message, "Exception Message Wrong");
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "Exception Not Thrown");
        }

        [TestMethod]
        public void TestChangeKeyOutputTypeToRows()
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
                instance.SetOutputProperty(textFileSplitter.OutputCollection[2].ID, ManageProperties.typeOfOutput, Utilities.typeOfOutputEnum.RowsProcessed);
            }
            catch (COMException ex)
            {
                Assert.AreEqual(MessageStrings.CantChangeOutputProperties("Key"), ex.Message, "Exception Message Wrong");
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "Exception Not Thrown");
        }

        [TestMethod]
        public void TestChangeKeyOutputTypeToError()
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
                instance.SetOutputProperty(textFileSplitter.OutputCollection[2].ID, ManageProperties.typeOfOutput, Utilities.typeOfOutputEnum.ErrorRecords);
            }
            catch (COMException ex)
            {
                Assert.AreEqual(MessageStrings.CantChangeOutputProperties("Key"), ex.Message, "Exception Message Wrong");
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "Exception Not Thrown");
        }

        #endregion

        #region Change masterRecordID (Exception)
        [TestMethod]
        public void TestChangeKeyOutputMasterRecordID()
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
                instance.SetOutputProperty(textFileSplitter.OutputCollection[2].ID, ManageProperties.masterRecordID, 5);
            }
            catch (COMException ex)
            {
                Assert.AreEqual(MessageStrings.CantChangeOutputProperties("Key"), ex.Message, "Exception Message Wrong");
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "Exception Not Thrown");
        }
        #endregion

        #region Change rowTypeValue (Succeed)
        [TestMethod]
        public void TestChangeKeyOutputrowTypeValue()
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
                instance.SetOutputProperty(textFileSplitter.OutputCollection[2].ID, ManageProperties.rowTypeValue, "ERROR");
            }
            catch
            {
                exceptionThrown = true;
            }

            Assert.IsFalse(exceptionThrown, "Exception Thrown");
            Assert.AreEqual("ERROR", ManageProperties.GetPropertyValue(textFileSplitter.OutputCollection[2].CustomPropertyCollection, ManageProperties.rowTypeValue), "Property Value");
        }
        #endregion

        #region Add Column (Succeed)
        [TestMethod]
        public void TestChangeKeyOutputAddColumn()
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
                instance.InsertOutputColumnAt(textFileSplitter.OutputCollection[2].ID, 0, "New Error Message", String.Empty);
            }
            catch
            {
                exceptionThrown = true;
            }

            Assert.IsFalse(exceptionThrown, "Exception Thrown");
            Assert.AreEqual(2, textFileSplitter.OutputCollection[2].OutputColumnCollection.Count, "Incorrect Column Count");
        }
        #endregion

        #region Delete Passthrough Column (Succeed)
        [TestMethod]
        public void TestChangeKeyOutputDeletePassThoughColumn()
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
            IDTSOutputColumn100 newColumn =  instance.InsertOutputColumnAt(textFileSplitter.OutputCollection[2].ID, 0, "New Error Message", String.Empty);
            try
            {
                instance.DeleteOutputColumn(textFileSplitter.OutputCollection[2].ID, newColumn.ID);
            }
            catch
            {
                exceptionThrown = true;
            }

            Assert.IsFalse(exceptionThrown, "Exception Thrown");
            Assert.AreEqual(1, textFileSplitter.OutputCollection[2].OutputColumnCollection.Count, "Incorrect Column Count");
        }
        #endregion

        #region Delete Key Column (Exception)
        [TestMethod]
        public void TestChangeKeyOutputDeleteKeyColumn()
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
                instance.DeleteOutputColumn(textFileSplitter.OutputCollection[2].ID, textFileSplitter.OutputCollection[2].OutputColumnCollection[0].ID);
            }
            catch (COMException ex)
            {
                Assert.AreEqual(MessageStrings.CantDeleteKeyColumn, ex.Message, "Exception Message Wrong");
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "Exception Not Thrown");
        }
        #endregion

        #region Set UsageOfColumn to any value From Key (Exception)

        [TestMethod]
        public void TestChangeKeyOutputSetKeyToPassthrough()
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

            IDTSOutput100 keyOutput = textFileSplitter.OutputCollection[2];
            ManageProperties.AddMissingOutputColumnProperties(keyOutput.OutputColumnCollection[0].CustomPropertyCollection);

            Boolean exceptionThrown = false;
            IDTSOutputColumn100 newColumn = instance.InsertOutputColumnAt(textFileSplitter.OutputCollection[2].ID, 0, "New Error Message", String.Empty);
            try
            {
                instance.SetOutputColumnProperty(textFileSplitter.OutputCollection[2].ID, textFileSplitter.OutputCollection[2].OutputColumnCollection[0].ID, ManageProperties.usageOfColumn, Utilities.usageOfColumnEnum.Passthrough);
            }
            catch (Exception ex)
            {
                exceptionThrown = true;
                Assert.AreEqual(MessageStrings.CantUnSetKeyColumn, ex.Message, "Exception Message Wrong");
            }

            Assert.IsTrue(exceptionThrown, "Exception Thrown");
        }

        [TestMethod]
        public void TestChangeKeyOutputSetKeyToPassthrough()
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

            IDTSOutput100 keyOutput = textFileSplitter.OutputCollection[2];
            ManageProperties.AddMissingOutputColumnProperties(keyOutput.OutputColumnCollection[0].CustomPropertyCollection);

            Boolean exceptionThrown = false;
            IDTSOutputColumn100 newColumn = instance.InsertOutputColumnAt(textFileSplitter.OutputCollection[2].ID, 0, "New Error Message", String.Empty);
            try
            {
                instance.SetOutputColumnProperty(textFileSplitter.OutputCollection[2].ID, textFileSplitter.OutputCollection[2].OutputColumnCollection[0].ID, ManageProperties.usageOfColumn, Utilities.usageOfColumnEnum.Passthrough);
            }
            catch (Exception ex)
            {
                exceptionThrown = true;
                Assert.AreEqual(MessageStrings.CantUnSetKeyColumn, ex.Message, "Exception Message Wrong");
            }

            Assert.IsTrue(exceptionThrown, "Exception Thrown");
        }

        [TestMethod]
        public void TestChangeKeyOutputSetKeyToPassthrough()
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

            IDTSOutput100 keyOutput = textFileSplitter.OutputCollection[2];
            ManageProperties.AddMissingOutputColumnProperties(keyOutput.OutputColumnCollection[0].CustomPropertyCollection);

            Boolean exceptionThrown = false;
            IDTSOutputColumn100 newColumn = instance.InsertOutputColumnAt(textFileSplitter.OutputCollection[2].ID, 0, "New Error Message", String.Empty);
            try
            {
                instance.SetOutputColumnProperty(textFileSplitter.OutputCollection[2].ID, textFileSplitter.OutputCollection[2].OutputColumnCollection[0].ID, ManageProperties.usageOfColumn, Utilities.usageOfColumnEnum.Passthrough);
            }
            catch (Exception ex)
            {
                exceptionThrown = true;
                Assert.AreEqual(MessageStrings.CantUnSetKeyColumn, ex.Message, "Exception Message Wrong");
            }

            Assert.IsTrue(exceptionThrown, "Exception Thrown");
        }

        [TestMethod]
        public void TestChangeKeyOutputSetKeyToPassthrough()
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

            IDTSOutput100 keyOutput = textFileSplitter.OutputCollection[2];
            ManageProperties.AddMissingOutputColumnProperties(keyOutput.OutputColumnCollection[0].CustomPropertyCollection);

            Boolean exceptionThrown = false;
            IDTSOutputColumn100 newColumn = instance.InsertOutputColumnAt(textFileSplitter.OutputCollection[2].ID, 0, "New Error Message", String.Empty);
            try
            {
                instance.SetOutputColumnProperty(textFileSplitter.OutputCollection[2].ID, textFileSplitter.OutputCollection[2].OutputColumnCollection[0].ID, ManageProperties.usageOfColumn, Utilities.usageOfColumnEnum.Passthrough);
            }
            catch (Exception ex)
            {
                exceptionThrown = true;
                Assert.AreEqual(MessageStrings.CantUnSetKeyColumn, ex.Message, "Exception Message Wrong");
            }

            Assert.IsTrue(exceptionThrown, "Exception Thrown");
        }

        [TestMethod]
        public void TestChangeKeyOutputSetKeyToPassthrough()
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

            IDTSOutput100 keyOutput = textFileSplitter.OutputCollection[2];
            ManageProperties.AddMissingOutputColumnProperties(keyOutput.OutputColumnCollection[0].CustomPropertyCollection);

            Boolean exceptionThrown = false;
            IDTSOutputColumn100 newColumn = instance.InsertOutputColumnAt(textFileSplitter.OutputCollection[2].ID, 0, "New Error Message", String.Empty);
            try
            {
                instance.SetOutputColumnProperty(textFileSplitter.OutputCollection[2].ID, textFileSplitter.OutputCollection[2].OutputColumnCollection[0].ID, ManageProperties.usageOfColumn, Utilities.usageOfColumnEnum.Passthrough);
            }
            catch (Exception ex)
            {
                exceptionThrown = true;
                Assert.AreEqual(MessageStrings.CantUnSetKeyColumn, ex.Message, "Exception Message Wrong");
            }

            Assert.IsTrue(exceptionThrown, "Exception Thrown");
        }
        
        [TestMethod]
        public void TestChangeKeyOutputSetKeyToPassthrough()
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

            IDTSOutput100 keyOutput = textFileSplitter.OutputCollection[2];
            ManageProperties.AddMissingOutputColumnProperties(keyOutput.OutputColumnCollection[0].CustomPropertyCollection);

            Boolean exceptionThrown = false;
            IDTSOutputColumn100 newColumn = instance.InsertOutputColumnAt(textFileSplitter.OutputCollection[2].ID, 0, "New Error Message", String.Empty);
            try
            {
                instance.SetOutputColumnProperty(textFileSplitter.OutputCollection[2].ID, textFileSplitter.OutputCollection[2].OutputColumnCollection[0].ID, ManageProperties.usageOfColumn, Utilities.usageOfColumnEnum.Passthrough);
            }
            catch (Exception ex)
            {
                exceptionThrown = true;
                Assert.AreEqual(MessageStrings.CantUnSetKeyColumn, ex.Message, "Exception Message Wrong");
            }

            Assert.IsTrue(exceptionThrown, "Exception Thrown");
        }
        
        #endregion




        #region Set Passthrough

        [TestMethod]
        public void TestChangeKeyOutputSetPassthroughToKey()
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

            IDTSOutput100 keyOutput = textFileSplitter.OutputCollection[2];
            ManageProperties.AddMissingOutputColumnProperties(keyOutput.OutputColumnCollection[0].CustomPropertyCollection);

            Boolean exceptionThrown = false;
            IDTSOutputColumn100 newColumn = instance.InsertOutputColumnAt(textFileSplitter.OutputCollection[2].ID, 0, "New Error Message", String.Empty);
            try
            {
                instance.SetOutputColumnProperty(textFileSplitter.OutputCollection[2].ID, newColumn.ID, ManageProperties.usageOfColumn, Utilities.usageOfColumnEnum.Key);
            }
            catch (Exception ex)
            {
                exceptionThrown = true;
                Assert.AreEqual(MessageStrings.CantSetKeyColumn, ex.Message, "Exception Message Wrong");
            }

            Assert.IsTrue(exceptionThrown, "Exception Thrown");
        }

        [TestMethod]
        public void TestChangeKeyOutputSetPassthroughToIgnore()
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

            IDTSOutput100 keyOutput = textFileSplitter.OutputCollection[2];
            ManageProperties.AddMissingOutputColumnProperties(keyOutput.OutputColumnCollection[0].CustomPropertyCollection);

            Boolean exceptionThrown = false;
            IDTSOutputColumn100 newColumn = instance.InsertOutputColumnAt(textFileSplitter.OutputCollection[2].ID, 0, "New Error Message", String.Empty);
            try
            {
                instance.SetOutputColumnProperty(textFileSplitter.OutputCollection[2].ID, newColumn.ID, ManageProperties.usageOfColumn, Utilities.usageOfColumnEnum.Ignore);
            }
            catch (Exception ex)
            {
                exceptionThrown = true;
                Assert.AreEqual(MessageStrings.InvalidPropertyValue(textFileSplitter.OutputCollection[2].Name, newColumn.Name, ManageProperties.usageOfColumn, System.Enum.GetName(typeof(Utilities.usageOfColumnEnum), Utilities.usageOfColumnEnum.Ignore)), ex.Message, "Exception Message Wrong");
            }

            Assert.IsTrue(exceptionThrown, "Exception Thrown");
        }

        [TestMethod]
        public void TestChangeKeyOutputSetPassthroughToMasterValue()
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

            IDTSOutput100 keyOutput = textFileSplitter.OutputCollection[2];
            ManageProperties.AddMissingOutputColumnProperties(keyOutput.OutputColumnCollection[0].CustomPropertyCollection);

            Boolean exceptionThrown = false;
            IDTSOutputColumn100 newColumn = instance.InsertOutputColumnAt(textFileSplitter.OutputCollection[2].ID, 0, "New Error Message", String.Empty);
            try
            {
                instance.SetOutputColumnProperty(textFileSplitter.OutputCollection[2].ID, newColumn.ID, ManageProperties.usageOfColumn, Utilities.usageOfColumnEnum.MasterValue);
            }
            catch (Exception ex)
            {
                exceptionThrown = true;
                Assert.AreEqual(MessageStrings.InvalidPropertyValue(textFileSplitter.OutputCollection[2].Name, newColumn.Name, ManageProperties.usageOfColumn, System.Enum.GetName(typeof(Utilities.usageOfColumnEnum), Utilities.usageOfColumnEnum.MasterValue)), ex.Message, "Exception Message Wrong");
            }

            Assert.IsTrue(exceptionThrown, "Exception Thrown");
        }

        [TestMethod]
        public void TestChangeKeyOutputSetPassthroughToRowData()
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

            IDTSOutput100 keyOutput = textFileSplitter.OutputCollection[2];
            ManageProperties.AddMissingOutputColumnProperties(keyOutput.OutputColumnCollection[0].CustomPropertyCollection);

            Boolean exceptionThrown = false;
            IDTSOutputColumn100 newColumn = instance.InsertOutputColumnAt(textFileSplitter.OutputCollection[2].ID, 0, "New Error Message", String.Empty);
            try
            {
                instance.SetOutputColumnProperty(textFileSplitter.OutputCollection[2].ID, newColumn.ID, ManageProperties.usageOfColumn, Utilities.usageOfColumnEnum.RowData);
            }
            catch (Exception ex)
            {
                exceptionThrown = true;
                Assert.AreEqual(MessageStrings.InvalidPropertyValue(textFileSplitter.OutputCollection[2].Name, newColumn.Name, ManageProperties.usageOfColumn, System.Enum.GetName(typeof(Utilities.usageOfColumnEnum), Utilities.usageOfColumnEnum.RowData)), ex.Message, "Exception Message Wrong");
            }

            Assert.IsTrue(exceptionThrown, "Exception Thrown");
        }

        [TestMethod]
        public void TestChangeKeyOutputSetPassthroughToRowType()
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

            IDTSOutput100 keyOutput = textFileSplitter.OutputCollection[2];
            ManageProperties.AddMissingOutputColumnProperties(keyOutput.OutputColumnCollection[0].CustomPropertyCollection);

            Boolean exceptionThrown = false;
            IDTSOutputColumn100 newColumn = instance.InsertOutputColumnAt(textFileSplitter.OutputCollection[2].ID, 0, "New Error Message", String.Empty);
            try
            {
                instance.SetOutputColumnProperty(textFileSplitter.OutputCollection[2].ID, newColumn.ID, ManageProperties.usageOfColumn, Utilities.usageOfColumnEnum.RowType);
            }
            catch (Exception ex)
            {
                exceptionThrown = true;
                Assert.AreEqual(MessageStrings.InvalidPropertyValue(textFileSplitter.OutputCollection[2].Name, newColumn.Name, ManageProperties.usageOfColumn, System.Enum.GetName(typeof(Utilities.usageOfColumnEnum), Utilities.usageOfColumnEnum.RowType)), ex.Message, "Exception Message Wrong");
            }

            Assert.IsTrue(exceptionThrown, "Exception Thrown");
        }

        #endregion

        [TestMethod]
        public void TestChangeKeyOutputSetColumnProperty()
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

            IDTSOutput100 keyOutput = textFileSplitter.OutputCollection[2];
            ManageProperties.AddMissingOutputColumnProperties(keyOutput.OutputColumnCollection[0].CustomPropertyCollection);

            Boolean exceptionThrown = false;
            try
            {
                instance.SetOutputColumnProperty(keyOutput.ID, keyOutput.OutputColumnCollection[0].ID, ManageProperties.dotNetFormatString, String.Empty);
            }
            catch (COMException ex)
            {
                Assert.AreEqual(MessageStrings.CantChangeOutputProperties("Key"), ex.Message, "Exception Message Wrong");
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "Exception Not Thrown");
        }

        [TestMethod]
        public void TestChangeKeyOutputSetColumnDataType()
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

            IDTSOutput100 keyOutput = textFileSplitter.OutputCollection[2];

            Boolean exceptionThrown = false;
            try
            {
                instance.SetOutputColumnDataTypeProperties(keyOutput.ID, keyOutput.OutputColumnCollection[0].ID, DataType.DT_STR, 512, 0, 0, 1252);
            }
            catch (COMException ex)
            {
                Assert.AreEqual(MessageStrings.CantChangeOutputProperties("Key"), ex.Message, "Exception Message Wrong");
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "Exception Not Thrown");
        }
    }
}
