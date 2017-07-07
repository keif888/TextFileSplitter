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

namespace UnitTestTextFileSplitter
{
    [TestClass]
    public class TestKeyRecordOutput
    {
        #region Delete Output (Exception)

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

        #endregion

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
                instance.InsertOutputColumnAt(textFileSplitter.OutputCollection[2].ID, 1, "New Error Message", String.Empty);
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
            IDTSOutputColumn100 newColumn =  instance.InsertOutputColumnAt(textFileSplitter.OutputCollection[2].ID, 1, "New Error Message", String.Empty);
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
        public void TestChangeKeyOutputSetKeyToRowType()
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
            IDTSOutputColumn100 newColumn = instance.InsertOutputColumnAt(textFileSplitter.OutputCollection[2].ID, 1, "New Error Message", String.Empty);
            try
            {
                instance.SetOutputColumnProperty(textFileSplitter.OutputCollection[2].ID, textFileSplitter.OutputCollection[2].OutputColumnCollection[0].ID, ManageProperties.usageOfColumn, Utilities.usageOfColumnEnum.RowType);
            }
            catch (Exception ex)
            {
                exceptionThrown = true;
                Assert.AreEqual(MessageStrings.InvalidPropertyValue(textFileSplitter.OutputCollection[2].Name, textFileSplitter.OutputCollection[2].OutputColumnCollection[0].Name, ManageProperties.usageOfColumn, System.Enum.GetName(typeof(Utilities.usageOfColumnEnum), Utilities.usageOfColumnEnum.RowType)), ex.Message, "Exception Message Wrong");
            }

            Assert.IsTrue(exceptionThrown, "Exception Thrown");
        }

        [TestMethod]
        public void TestChangeKeyOutputSetKeyToRowData()
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
            IDTSOutputColumn100 newColumn = instance.InsertOutputColumnAt(textFileSplitter.OutputCollection[2].ID, 1, "New Error Message", String.Empty);
            try
            {
                instance.SetOutputColumnProperty(textFileSplitter.OutputCollection[2].ID, textFileSplitter.OutputCollection[2].OutputColumnCollection[0].ID, ManageProperties.usageOfColumn, Utilities.usageOfColumnEnum.RowData);
            }
            catch (Exception ex)
            {
                exceptionThrown = true;
                Assert.AreEqual(MessageStrings.InvalidPropertyValue(textFileSplitter.OutputCollection[2].Name, textFileSplitter.OutputCollection[2].OutputColumnCollection[0].Name, ManageProperties.usageOfColumn, System.Enum.GetName(typeof(Utilities.usageOfColumnEnum), Utilities.usageOfColumnEnum.RowData)), ex.Message, "Exception Message Wrong");
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
            IDTSOutputColumn100 newColumn = instance.InsertOutputColumnAt(textFileSplitter.OutputCollection[2].ID, 1, "New Error Message", String.Empty);
            try
            {
                instance.SetOutputColumnProperty(textFileSplitter.OutputCollection[2].ID, textFileSplitter.OutputCollection[2].OutputColumnCollection[0].ID, ManageProperties.usageOfColumn, Utilities.usageOfColumnEnum.Passthrough);
            }
            catch (Exception ex)
            {
                exceptionThrown = true;
                Assert.AreEqual(MessageStrings.InvalidPropertyValue(textFileSplitter.OutputCollection[2].Name, textFileSplitter.OutputCollection[2].OutputColumnCollection[0].Name, ManageProperties.usageOfColumn, System.Enum.GetName(typeof(Utilities.usageOfColumnEnum), Utilities.usageOfColumnEnum.Passthrough)), ex.Message, "Exception Message Wrong");
            }

            Assert.IsTrue(exceptionThrown, "Exception Thrown");
        }

        [TestMethod]
        public void TestChangeKeyOutputSetKeyToKey()
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
            IDTSOutputColumn100 newColumn = instance.InsertOutputColumnAt(textFileSplitter.OutputCollection[2].ID, 1, "New Error Message", String.Empty);
            try
            {
                instance.SetOutputColumnProperty(textFileSplitter.OutputCollection[2].ID, textFileSplitter.OutputCollection[2].OutputColumnCollection[0].ID, ManageProperties.usageOfColumn, Utilities.usageOfColumnEnum.Key);
            }
            catch (Exception ex)
            {
                exceptionThrown = true;
                Assert.AreEqual(MessageStrings.InvalidPropertyValue(textFileSplitter.OutputCollection[2].Name, textFileSplitter.OutputCollection[2].OutputColumnCollection[0].Name, ManageProperties.usageOfColumn, System.Enum.GetName(typeof(Utilities.usageOfColumnEnum), Utilities.usageOfColumnEnum.Key)), ex.Message, "Exception Message Wrong");
            }

            Assert.IsTrue(exceptionThrown, "Exception Thrown");
        }

        [TestMethod]
        public void TestChangeKeyOutputSetKeyToIgnore()
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
            IDTSOutputColumn100 newColumn = instance.InsertOutputColumnAt(textFileSplitter.OutputCollection[2].ID, 1, "New Error Message", String.Empty);
            try
            {
                instance.SetOutputColumnProperty(textFileSplitter.OutputCollection[2].ID, textFileSplitter.OutputCollection[2].OutputColumnCollection[0].ID, ManageProperties.usageOfColumn, Utilities.usageOfColumnEnum.Ignore);
            }
            catch (Exception ex)
            {
                exceptionThrown = true;
                Assert.AreEqual(MessageStrings.InvalidPropertyValue(textFileSplitter.OutputCollection[2].Name, textFileSplitter.OutputCollection[2].OutputColumnCollection[0].Name, ManageProperties.usageOfColumn, System.Enum.GetName(typeof(Utilities.usageOfColumnEnum), Utilities.usageOfColumnEnum.Ignore)), ex.Message, "Exception Message Wrong");
            }

            Assert.IsTrue(exceptionThrown, "Exception Thrown");
        }

        [TestMethod]
        public void TestChangeKeyOutputSetKeyToMasterValue()
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
            IDTSOutputColumn100 newColumn = instance.InsertOutputColumnAt(textFileSplitter.OutputCollection[2].ID, 1, "New Error Message", String.Empty);
            try
            {
                instance.SetOutputColumnProperty(textFileSplitter.OutputCollection[2].ID, textFileSplitter.OutputCollection[2].OutputColumnCollection[0].ID, ManageProperties.usageOfColumn, Utilities.usageOfColumnEnum.MasterValue);
            }
            catch (Exception ex)
            {
                exceptionThrown = true;
                Assert.AreEqual(MessageStrings.InvalidPropertyValue(textFileSplitter.OutputCollection[2].Name, textFileSplitter.OutputCollection[2].OutputColumnCollection[0].Name, ManageProperties.usageOfColumn, System.Enum.GetName(typeof(Utilities.usageOfColumnEnum), Utilities.usageOfColumnEnum.MasterValue)), ex.Message, "Exception Message Wrong");
            }

            Assert.IsTrue(exceptionThrown, "Exception Thrown");
        }
        
        #endregion

        #region Set UsageOfColumn to any value From Passthrough (Exception)

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
            IDTSOutputColumn100 newColumn = instance.InsertOutputColumnAt(textFileSplitter.OutputCollection[2].ID, 1, "New Error Message", String.Empty);
            try
            {
                instance.SetOutputColumnProperty(textFileSplitter.OutputCollection[2].ID, newColumn.ID, ManageProperties.usageOfColumn, Utilities.usageOfColumnEnum.Key);
            }
            catch (Exception ex)
            {
                exceptionThrown = true;
                Assert.AreEqual(MessageStrings.InvalidPropertyValue(textFileSplitter.OutputCollection[2].Name, newColumn.Name, ManageProperties.usageOfColumn, System.Enum.GetName(typeof(Utilities.usageOfColumnEnum), Utilities.usageOfColumnEnum.Key)), ex.Message, "Exception Message Wrong");
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
            IDTSOutputColumn100 newColumn = instance.InsertOutputColumnAt(textFileSplitter.OutputCollection[2].ID, 1, "New Error Message", String.Empty);
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
            IDTSOutputColumn100 newColumn = instance.InsertOutputColumnAt(textFileSplitter.OutputCollection[2].ID, 1, "New Error Message", String.Empty);
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
            IDTSOutputColumn100 newColumn = instance.InsertOutputColumnAt(textFileSplitter.OutputCollection[2].ID, 1, "New Error Message", String.Empty);
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
            IDTSOutputColumn100 newColumn = instance.InsertOutputColumnAt(textFileSplitter.OutputCollection[2].ID, 1, "New Error Message", String.Empty);
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

        #region Set keyOutputColumnID to any value (Exception)

        [TestMethod]
        public void TestChangeKeyOutputSetkeyOutputColumnIDOnPassthrough()
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
            IDTSOutputColumn100 newColumn = instance.InsertOutputColumnAt(textFileSplitter.OutputCollection[2].ID, 1, "New Error Message", String.Empty);
            try
            {
                instance.SetOutputColumnProperty(textFileSplitter.OutputCollection[2].ID, newColumn.ID, ManageProperties.keyOutputColumnID, 10);
            }
            catch (Exception ex)
            {
                exceptionThrown = true;
                Assert.AreEqual(MessageStrings.InvalidPropertyValue(textFileSplitter.OutputCollection[2].Name, newColumn.Name, ManageProperties.keyOutputColumnID, "10"), ex.Message, "Exception Message Wrong");
            }

            Assert.IsTrue(exceptionThrown, "Exception Thrown");
        }

        [TestMethod]
        public void TestChangeKeyOutputSetkeyOutputColumnIDOnKey()
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
                instance.SetOutputColumnProperty(textFileSplitter.OutputCollection[2].ID, textFileSplitter.OutputCollection[2].OutputColumnCollection[0].ID, ManageProperties.keyOutputColumnID, 10);
            }
            catch (Exception ex)
            {
                exceptionThrown = true;
                Assert.AreEqual(MessageStrings.InvalidPropertyValue(textFileSplitter.OutputCollection[2].Name, textFileSplitter.OutputCollection[2].OutputColumnCollection[0].Name, ManageProperties.keyOutputColumnID, "10"), ex.Message, "Exception Message Wrong");
            }

            Assert.IsTrue(exceptionThrown, "Exception Thrown");
        }
        #endregion

        #region Set dotNetFormatString on column with usageOfColumn = Key (Exception)
        [TestMethod]
        public void TestChangeKeyOutputSetdotNetFormatStringOnKey()
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
                instance.SetOutputColumnProperty(textFileSplitter.OutputCollection[2].ID, textFileSplitter.OutputCollection[2].OutputColumnCollection[0].ID, ManageProperties.dotNetFormatString, "yyyyMMdd");
            }
            catch (Exception ex)
            {
                exceptionThrown = true;
                Assert.AreEqual(MessageStrings.InvalidPropertyValue(textFileSplitter.OutputCollection[2].Name, textFileSplitter.OutputCollection[2].OutputColumnCollection[0].Name, ManageProperties.dotNetFormatString, "yyyyMMdd"), ex.Message, "Exception Message Wrong");
            }

            Assert.IsTrue(exceptionThrown, "Exception Not Thrown");
        }
        #endregion

        #region Set dotNetFormatString on column with usageOfColumn = passThrough (Succeed)

        [TestMethod]
        public void TestChangeKeyOutputSetdotNetFormatStringOnPassThrough()
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
            IDTSOutputColumn100 newColumn = instance.InsertOutputColumnAt(textFileSplitter.OutputCollection[2].ID, 1, "New Error Message", String.Empty);
            try
            {
                instance.SetOutputColumnProperty(textFileSplitter.OutputCollection[2].ID, newColumn.ID, ManageProperties.dotNetFormatString, "yyyyMMdd");
            }
            catch
            {
                exceptionThrown = true;
            }

            Assert.IsFalse(exceptionThrown, "Exception Thrown");
            Assert.AreEqual("yyyyMMdd", (String)ManageProperties.GetPropertyValue(newColumn.CustomPropertyCollection, ManageProperties.dotNetFormatString), "Format NOT set");
        }

        #endregion

        #region Set isColumnOptional on column with usageOfColumn = Key (Exception)

        [TestMethod]
        public void TestChangeKeyOutputSetisColumnOptionalOnKey()
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
                instance.SetOutputColumnProperty(textFileSplitter.OutputCollection[2].ID, textFileSplitter.OutputCollection[2].OutputColumnCollection[0].ID, ManageProperties.isColumnOptional, true);
            }
            catch (Exception ex)
            {
                exceptionThrown = true;
                Assert.AreEqual(MessageStrings.InvalidPropertyValue(textFileSplitter.OutputCollection[2].Name, textFileSplitter.OutputCollection[2].OutputColumnCollection[0].Name, ManageProperties.isColumnOptional, true.ToString()), ex.Message, "Exception Message Wrong");
            }

            Assert.IsTrue(exceptionThrown, "Exception NOT Thrown");
        }

        #endregion

        #region Set isColumnOptional on column with usageOfColumn = passThrough and NOT last non optional column (Exception)

        [TestMethod]
        public void TestChangeKeyOutputSetisColumnOptionalOnPassThroughNotLastNonOptional()
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
            IDTSOutputColumn100 newColumn1 = instance.InsertOutputColumnAt(textFileSplitter.OutputCollection[2].ID, 1, "New Error Message", String.Empty);
            IDTSOutputColumn100 newColumn2 = instance.InsertOutputColumnAt(textFileSplitter.OutputCollection[2].ID, 2, "New Error Message", String.Empty);
            try
            {
                instance.SetOutputColumnProperty(textFileSplitter.OutputCollection[2].ID, newColumn1.ID, ManageProperties.isColumnOptional, true);
            }
            catch (Exception ex)
            {
                exceptionThrown = true;
                Assert.AreEqual(MessageStrings.CanOnlySetOptionalOnLastNonOptionalColumn, ex.Message, "Exception Message Wrong");
            }

            Assert.IsTrue(exceptionThrown, "Exception NOT Thrown");
        }

        #endregion

        #region Set isColumnOptional on column with usageOfColumn = passThrough and last column (Succeed)

        [TestMethod]
        public void TestChangeKeyOutputSetisColumnOptionalOnPassThroughLastColumn()
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
            IDTSOutputColumn100 newColumn = instance.InsertOutputColumnAt(textFileSplitter.OutputCollection[2].ID, 1, "New Error Message", String.Empty);
            try
            {
                instance.SetOutputColumnProperty(textFileSplitter.OutputCollection[2].ID, newColumn.ID, ManageProperties.isColumnOptional, true);
            }
            catch
            {
                exceptionThrown = true;
            }

            Assert.IsFalse(exceptionThrown, "Exception Thrown");
            Assert.AreEqual(true, (bool)ManageProperties.GetPropertyValue(newColumn.CustomPropertyCollection, ManageProperties.isColumnOptional), "isOptional NOT set");
        }

        #endregion

        #region Set isColumnOptional on column with usageOfColumn = passThrough and last non optional column (2nd last column) (Succeed)

        [TestMethod]
        public void TestChangeKeyOutputSetisColumnOptionalOnPassThroughLastNonOptionalOK()
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
            IDTSOutputColumn100 newColumn1 = instance.InsertOutputColumnAt(textFileSplitter.OutputCollection[2].ID, 1, "New Error Message", String.Empty);
            IDTSOutputColumn100 newColumn2 = instance.InsertOutputColumnAt(textFileSplitter.OutputCollection[2].ID, 2, "New Error Message", String.Empty);
            instance.SetOutputColumnProperty(textFileSplitter.OutputCollection[2].ID, newColumn2.ID, ManageProperties.isColumnOptional, true);
            try
            {
                instance.SetOutputColumnProperty(textFileSplitter.OutputCollection[2].ID, newColumn1.ID, ManageProperties.isColumnOptional, true);
            }
            catch
            {
                exceptionThrown = true;
            }

            Assert.IsFalse(exceptionThrown, "Exception Thrown");
            Assert.AreEqual(true, (bool)ManageProperties.GetPropertyValue(newColumn1.CustomPropertyCollection, ManageProperties.isColumnOptional), "isOptional NOT set");
        }

        #endregion

        #region Set isColumnOptional OFF on column with usageOfColumn = passThrough and last 2 columns optional (last column) (Exception)

        [TestMethod]
        public void TestChangeKeyOutputSetisColumnOptionalOFFOnPassThroughLastNonOptional()
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
            IDTSOutputColumn100 newColumn1 = instance.InsertOutputColumnAt(textFileSplitter.OutputCollection[2].ID, 1, "New Error Message", String.Empty);
            IDTSOutputColumn100 newColumn2 = instance.InsertOutputColumnAt(textFileSplitter.OutputCollection[2].ID, 2, "New Error Message", String.Empty);
            instance.SetOutputColumnProperty(textFileSplitter.OutputCollection[2].ID, newColumn2.ID, ManageProperties.isColumnOptional, true);
            instance.SetOutputColumnProperty(textFileSplitter.OutputCollection[2].ID, newColumn1.ID, ManageProperties.isColumnOptional, true);
            try
            {
                instance.SetOutputColumnProperty(textFileSplitter.OutputCollection[2].ID, newColumn2.ID, ManageProperties.isColumnOptional, false);
            }
            catch (Exception ex)
            {
                exceptionThrown = true;
                Assert.AreEqual(MessageStrings.CanOnlySetNonOptionalOnLastOptionalColumn, ex.Message, "Exception Message Wrong");
            }

            Assert.IsTrue(exceptionThrown, "Exception NOT Thrown");
        }

        #endregion

        #region Set isColumnOptional on column with usageOfColumn = passThrough and last non optional column (2nd last column) (Succeed)

        [TestMethod]
        public void TestChangeKeyOutputSetisColumnOptionalOFFOnPassThroughLastNonOptionalOK()
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
            IDTSOutputColumn100 newColumn1 = instance.InsertOutputColumnAt(textFileSplitter.OutputCollection[2].ID, 1, "New Error Message", String.Empty);
            IDTSOutputColumn100 newColumn2 = instance.InsertOutputColumnAt(textFileSplitter.OutputCollection[2].ID, 2, "New Error Message", String.Empty);
            instance.SetOutputColumnProperty(textFileSplitter.OutputCollection[2].ID, newColumn2.ID, ManageProperties.isColumnOptional, true);
            instance.SetOutputColumnProperty(textFileSplitter.OutputCollection[2].ID, newColumn1.ID, ManageProperties.isColumnOptional, true);
            try
            {
                instance.SetOutputColumnProperty(textFileSplitter.OutputCollection[2].ID, newColumn1.ID, ManageProperties.isColumnOptional, false);
            }
            catch
            {
                exceptionThrown = true;
            }

            Assert.IsFalse(exceptionThrown, "Exception Thrown");
            Assert.AreEqual(false, (bool)ManageProperties.GetPropertyValue(newColumn1.CustomPropertyCollection, ManageProperties.isColumnOptional), "isOptional NOT set");
        }

        #endregion

        #region Set Key Column Data Types (Exception)

        [TestMethod]
        public void TestChangeKeyOutputSetColumnDataTypeKey()
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

        #endregion

        #region Set Passthrough Column Data Types (Succeed)

        [TestMethod]
        public void TestChangeKeyOutputSetColumnDataTypePassthrough()
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
            IDTSOutputColumn100 newColumn = instance.InsertOutputColumnAt(textFileSplitter.OutputCollection[2].ID, 1, "New Error Message", String.Empty);

            try
            {
                instance.SetOutputColumnDataTypeProperties(textFileSplitter.OutputCollection[2].ID, newColumn.ID, DataType.DT_DBDATE, 0, 0, 0, 0);
            }
            catch
            {
                exceptionThrown = true;
            }

            Assert.IsFalse(exceptionThrown, "Exception Thrown");
            Assert.AreEqual(DataType.DT_DBDATE, textFileSplitter.OutputCollection[2].OutputColumnCollection[1].DataType, "Incorrect Column DataType");
        }

        #endregion

        #region Set Invalid Property for a Column (Exception)

        [TestMethod]
        public void TestChangeKeyOutputSetInvalidProperty()
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
            IDTSOutputColumn100 newColumn = instance.InsertOutputColumnAt(textFileSplitter.OutputCollection[2].ID, 1, "New Error Message", String.Empty);

            try
            {
                instance.SetOutputColumnProperty(textFileSplitter.OutputCollection[2].ID, newColumn.ID, ManageProperties.masterRecordID, 10);
            }
            catch (Exception ex)
            {
                exceptionThrown = true;
                Assert.AreEqual(MessageStrings.YouCanNotSetThatPropertyOnAColumn(textFileSplitter.OutputCollection[2].Name, newColumn.Name, ManageProperties.masterRecordID), ex.Message, "Exception Message Wrong");
            }

            Assert.IsTrue(exceptionThrown, "Exception NOT Thrown");
        }

        #endregion

    }
}
