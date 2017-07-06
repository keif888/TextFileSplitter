using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Martin.SQLServer.Dts;
using Microsoft.SqlServer.Dts.Pipeline;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime.Wrapper;
using Microsoft.SqlServer.Dts.Runtime;
using System.Collections.Generic;

namespace UnitTestTextFileSplitter2012
{
    [TestClass]
    public class TestManageProperties
    {
        [TestMethod]
        public void TestValidateProperties_Valid()
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

            ManageProperties.AddComponentProperties(output.CustomPropertyCollection);
            ManageProperties.AddOutputProperties(output.CustomPropertyCollection);
            ManageProperties.AddOutputColumnProperties(output.CustomPropertyCollection);

            ManageProperties propertyManage = new ManageProperties();

            DTSValidationStatus expected = DTSValidationStatus.VS_ISVALID;
            DTSValidationStatus actual = propertyManage.ValidateProperties(output.CustomPropertyCollection, expected);

            Assert.AreEqual(expected, actual, "Validation Status Wrong");
            Assert.IsTrue(ManageProperties.SetContainsLineage(output.CustomPropertyCollection, ManageProperties.columnDelimiter, false), "Property columnDelimiter is missing");
            Assert.IsTrue(ManageProperties.SetContainsLineage(output.CustomPropertyCollection, ManageProperties.isTextDelmited, false), "Property isTextDelmited is missing");
            Assert.IsTrue(ManageProperties.SetContainsLineage(output.CustomPropertyCollection, ManageProperties.textDelmiter, false), "Property textDelmiter is missing");
            Assert.IsTrue(ManageProperties.SetContainsLineage(output.CustomPropertyCollection, ManageProperties.treatEmptyStringsAsNull, false), "Property treatEmptyStringsAsNull is missing");
            Assert.IsTrue(ManageProperties.SetContainsLineage(output.CustomPropertyCollection, ManageProperties.typeOfOutput, false), "Property typeOfOutput is missing");
            Assert.IsTrue(ManageProperties.SetContainsLineage(output.CustomPropertyCollection, ManageProperties.rowTypeValue, false), "Property rowTypeValue is missing");
            Assert.IsTrue(ManageProperties.SetContainsLineage(output.CustomPropertyCollection, ManageProperties.masterRecordID, false), "Property masterRecordID is missing");
            Assert.IsTrue(ManageProperties.SetContainsLineage(output.CustomPropertyCollection, ManageProperties.usageOfColumn, false), "Property usageOfColumn is missing");
            Assert.IsTrue(ManageProperties.SetContainsLineage(output.CustomPropertyCollection, ManageProperties.keyOutputColumnID, false), "Property keyOutputColumnID is missing");
            Assert.IsTrue(ManageProperties.SetContainsLineage(output.CustomPropertyCollection, ManageProperties.dotNetFormatString, false), "Property dotNetFormatString is missing");
            Assert.IsTrue(ManageProperties.SetContainsLineage(output.CustomPropertyCollection, ManageProperties.isColumnOptional, false), "Property isColumnOptional is missing");
        }

        #region RowType
        [TestMethod]
        public void TestValidateProperties_RowTypeToLong()
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

            ManageProperties.AddComponentProperties(output.CustomPropertyCollection);
            ManageProperties.AddOutputProperties(output.CustomPropertyCollection);
            ManageProperties.AddOutputColumnProperties(output.CustomPropertyCollection);

            string propertyString = "1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890";
            ManageProperties.SetPropertyValue(output.CustomPropertyCollection, ManageProperties.rowTypeValue, propertyString);

            ManageProperties propertyManager = new ManageProperties();
            propertyManager.PostErrorEvent += new PostErrorDelegate(this.PostError);
            errorMessages = new List<string>();

            DTSValidationStatus expected = DTSValidationStatus.VS_ISBROKEN;
            DTSValidationStatus actual = propertyManager.ValidateProperties(output.CustomPropertyCollection, DTSValidationStatus.VS_ISVALID);
            
            Assert.AreEqual(expected, actual, "Validation Status Wrong");
            Assert.AreEqual(1, errorMessages.Count, "Number of error messages wrong");
            Assert.AreEqual(String.Format("The string \"{1}\" is too long for property {0}.", ManageProperties.rowTypeValue, propertyString), errorMessages[0], "Error message is wrong");
        }

        [TestMethod]
        public void TestValidateProperties_RowTypeWrongType()
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

            ManageProperties.AddComponentProperties(output.CustomPropertyCollection);
            ManageProperties.AddOutputProperties(output.CustomPropertyCollection);
            ManageProperties.AddOutputColumnProperties(output.CustomPropertyCollection);

            int propertyValue = 12345;
            ManageProperties.SetPropertyValue(output.CustomPropertyCollection, ManageProperties.rowTypeValue, propertyValue);

            ManageProperties propertyManager = new ManageProperties();
            propertyManager.PostErrorEvent += new PostErrorDelegate(this.PostError);
            errorMessages = new List<string>();

            DTSValidationStatus expected = DTSValidationStatus.VS_ISCORRUPT;
            DTSValidationStatus actual = propertyManager.ValidateProperties(output.CustomPropertyCollection, DTSValidationStatus.VS_ISVALID);

            Assert.AreEqual(expected, actual, "Validation Status Wrong");
            Assert.AreEqual(1, errorMessages.Count, "Number of error messages wrong");
            Assert.AreEqual(MessageStrings.InvalidPropertyValue(ManageProperties.rowTypeValue, propertyValue), errorMessages[0], "Error message is wrong");
        }
        #endregion

        #region TypeOfOutput
        [TestMethod]
        public void TestValidateProperties_TypeOfOutputCorrupt()
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

            ManageProperties.AddComponentProperties(output.CustomPropertyCollection);
            ManageProperties.AddOutputProperties(output.CustomPropertyCollection);
            ManageProperties.AddOutputColumnProperties(output.CustomPropertyCollection);

            int propertyValue = 50;
            ManageProperties.SetPropertyValue(output.CustomPropertyCollection, ManageProperties.typeOfOutput, propertyValue);

            ManageProperties propertyManager = new ManageProperties();
            propertyManager.PostErrorEvent += new PostErrorDelegate(this.PostError);
            errorMessages = new List<string>();

            DTSValidationStatus expected = DTSValidationStatus.VS_ISCORRUPT;
            DTSValidationStatus actual = propertyManager.ValidateProperties(output.CustomPropertyCollection, DTSValidationStatus.VS_ISVALID);

            Assert.AreEqual(expected, actual, "Validation Status Wrong");
            Assert.AreEqual(1, errorMessages.Count, "Number of error messages wrong");
            Assert.AreEqual(MessageStrings.InvalidPropertyValue( ManageProperties.typeOfOutput, propertyValue), errorMessages[0], "Error message is wrong");
        }
       
        #endregion

        #region usageOfColumnEnum
        [TestMethod]
        public void TestValidateProperties_usageOfColumnEnumCorrupt()
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

            ManageProperties.AddComponentProperties(output.CustomPropertyCollection);
            ManageProperties.AddOutputProperties(output.CustomPropertyCollection);
            ManageProperties.AddOutputColumnProperties(output.CustomPropertyCollection);

            int propertyValue = 50;
            ManageProperties.SetPropertyValue(output.CustomPropertyCollection, ManageProperties.usageOfColumn, propertyValue);

            ManageProperties propertyManager = new ManageProperties();
            propertyManager.PostErrorEvent += new PostErrorDelegate(this.PostError);
            errorMessages = new List<string>();

            DTSValidationStatus expected = DTSValidationStatus.VS_ISCORRUPT;
            DTSValidationStatus actual = propertyManager.ValidateProperties(output.CustomPropertyCollection, DTSValidationStatus.VS_ISVALID);

            Assert.AreEqual(expected, actual, "Validation Status Wrong");
            Assert.AreEqual(1, errorMessages.Count, "Number of error messages wrong");
            Assert.AreEqual(MessageStrings.InvalidPropertyValue(ManageProperties.usageOfColumn, propertyValue), errorMessages[0], "Error message is wrong");
        }

        #endregion

        #region Boolean Property
        [TestMethod]
        public void TestValidateProperties_BooleanCorrupt()
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

            ManageProperties.AddComponentProperties(output.CustomPropertyCollection);
            ManageProperties.AddOutputProperties(output.CustomPropertyCollection);
            ManageProperties.AddOutputColumnProperties(output.CustomPropertyCollection);

            int propertyValue = 50;
            ManageProperties.SetPropertyValue(output.CustomPropertyCollection, ManageProperties.treatEmptyStringsAsNull, propertyValue);

            ManageProperties propertyManager = new ManageProperties();
            propertyManager.PostErrorEvent += new PostErrorDelegate(this.PostError);
            errorMessages = new List<string>();

            DTSValidationStatus expected = DTSValidationStatus.VS_ISCORRUPT;
            DTSValidationStatus actual = propertyManager.ValidateProperties(output.CustomPropertyCollection, DTSValidationStatus.VS_ISVALID);

            Assert.AreEqual(expected, actual, "Validation Status Wrong");
            Assert.AreEqual(1, errorMessages.Count, "Number of error messages wrong");
            Assert.AreEqual(MessageStrings.InvalidPropertyValue(ManageProperties.treatEmptyStringsAsNull, propertyValue), errorMessages[0], "Error message is wrong");
        }

        #endregion

        #region Integer Property
        [TestMethod]
        public void TestValidateProperties_IntegerCorrupt()
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

            ManageProperties.AddComponentProperties(output.CustomPropertyCollection);
            ManageProperties.AddOutputProperties(output.CustomPropertyCollection);
            ManageProperties.AddOutputColumnProperties(output.CustomPropertyCollection);

            String propertyValue = "50";
            ManageProperties.SetPropertyValue(output.CustomPropertyCollection, ManageProperties.masterRecordID, propertyValue);

            ManageProperties propertyManager = new ManageProperties();
            propertyManager.PostErrorEvent += new PostErrorDelegate(this.PostError);
            errorMessages = new List<string>();

            DTSValidationStatus expected = DTSValidationStatus.VS_ISCORRUPT;
            DTSValidationStatus actual = propertyManager.ValidateProperties(output.CustomPropertyCollection, DTSValidationStatus.VS_ISVALID);

            Assert.AreEqual(expected, actual, "Validation Status Wrong");
            Assert.AreEqual(1, errorMessages.Count, "Number of error messages wrong");
            Assert.AreEqual(MessageStrings.InvalidPropertyValue(ManageProperties.masterRecordID, propertyValue), errorMessages[0], "Error message is wrong");
        }

        #endregion

        #region Delimiter Property
        [TestMethod]
        public void TestValidateProperties_DelimiterToLong()
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

            ManageProperties.AddComponentProperties(output.CustomPropertyCollection);
            ManageProperties.AddOutputProperties(output.CustomPropertyCollection);
            ManageProperties.AddOutputColumnProperties(output.CustomPropertyCollection);

            string propertyString = "1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890";
            ManageProperties.SetPropertyValue(output.CustomPropertyCollection, ManageProperties.columnDelimiter, propertyString);

            ManageProperties propertyManager = new ManageProperties();
            propertyManager.PostErrorEvent += new PostErrorDelegate(this.PostError);
            errorMessages = new List<string>();

            DTSValidationStatus expected = DTSValidationStatus.VS_ISBROKEN;
            DTSValidationStatus actual = propertyManager.ValidateProperties(output.CustomPropertyCollection, DTSValidationStatus.VS_ISVALID);

            Assert.AreEqual(expected, actual, "Validation Status Wrong");
            Assert.AreEqual(1, errorMessages.Count, "Number of error messages wrong");
            Assert.AreEqual(String.Format("The string \"{1}\" is too long for property {0}.", ManageProperties.columnDelimiter, propertyString), errorMessages[0], "Error message is wrong");
        }

        [TestMethod]
        public void TestValidateProperties_DelimiterWrongType()
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

            ManageProperties.AddComponentProperties(output.CustomPropertyCollection);
            ManageProperties.AddOutputProperties(output.CustomPropertyCollection);
            ManageProperties.AddOutputColumnProperties(output.CustomPropertyCollection);

            int propertyValue = 12345;
            ManageProperties.SetPropertyValue(output.CustomPropertyCollection, ManageProperties.columnDelimiter, propertyValue);

            ManageProperties propertyManager = new ManageProperties();
            propertyManager.PostErrorEvent += new PostErrorDelegate(this.PostError);
            errorMessages = new List<string>();

            DTSValidationStatus expected = DTSValidationStatus.VS_ISCORRUPT;
            DTSValidationStatus actual = propertyManager.ValidateProperties(output.CustomPropertyCollection, DTSValidationStatus.VS_ISVALID);

            Assert.AreEqual(expected, actual, "Validation Status Wrong");
            Assert.AreEqual(1, errorMessages.Count, "Number of error messages wrong");
            Assert.AreEqual(MessageStrings.InvalidPropertyValue(ManageProperties.columnDelimiter, propertyValue), errorMessages[0], "Error message is wrong");
        }
        #endregion

        #region String Property
        [TestMethod]
        public void TestValidateProperties_StringCorrupt()
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

            ManageProperties.AddComponentProperties(output.CustomPropertyCollection);
            ManageProperties.AddOutputProperties(output.CustomPropertyCollection);
            ManageProperties.AddOutputColumnProperties(output.CustomPropertyCollection);

            int propertyValue = 50;
            ManageProperties.SetPropertyValue(output.CustomPropertyCollection, ManageProperties.dotNetFormatString, propertyValue);

            ManageProperties propertyManager = new ManageProperties();
            propertyManager.PostErrorEvent += new PostErrorDelegate(this.PostError);
            errorMessages = new List<string>();

            DTSValidationStatus expected = DTSValidationStatus.VS_ISCORRUPT;
            DTSValidationStatus actual = propertyManager.ValidateProperties(output.CustomPropertyCollection, DTSValidationStatus.VS_ISVALID);

            Assert.AreEqual(expected, actual, "Validation Status Wrong");
            Assert.AreEqual(1, errorMessages.Count, "Number of error messages wrong");
            Assert.AreEqual(MessageStrings.InvalidPropertyValue(ManageProperties.dotNetFormatString, propertyValue), errorMessages[0], "Error message is wrong");
        }

        #endregion

        #region ValidateOutputProperties

        [TestMethod]
        public void TestValidateOutputProperties_Valid()
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

            ManageProperties.AddComponentProperties(output.CustomPropertyCollection);
            ManageProperties.AddOutputProperties(output.CustomPropertyCollection);
            ManageProperties.AddOutputColumnProperties(output.CustomPropertyCollection);

            ManageProperties propertyManager = new ManageProperties();
            propertyManager.PostErrorEvent += new PostErrorDelegate(this.PostError);
            errorMessages = new List<string>();

            DTSValidationStatus expected = DTSValidationStatus.VS_ISVALID;
            DTSValidationStatus actual = propertyManager.ValidateOutputProperties(output.CustomPropertyCollection, DTSValidationStatus.VS_ISVALID);

            Assert.AreEqual(expected, actual, "Validation Status Wrong");
            Assert.AreEqual(0, errorMessages.Count, "Number of error messages wrong");
        }


        [TestMethod]
        public void TestValidateOutputProperties_MissingProperty()
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

            ManageProperties.AddComponentProperties(output.CustomPropertyCollection);
            //ManageProperties.AddOutputProperties(output.CustomPropertyCollection);
            ManageProperties.AddOutputColumnProperties(output.CustomPropertyCollection);

            ManageProperties propertyManager = new ManageProperties();
            propertyManager.PostErrorEvent += new PostErrorDelegate(this.PostError);
            errorMessages = new List<string>();

            DTSValidationStatus expected = DTSValidationStatus.VS_ISCORRUPT;
            DTSValidationStatus actual = propertyManager.ValidateOutputProperties(output.CustomPropertyCollection, DTSValidationStatus.VS_ISVALID);

            Assert.AreEqual(expected, actual, "Validation Status Wrong");
            Assert.AreEqual(3, errorMessages.Count, "Number of error messages wrong");
            Assert.IsTrue(errorMessages.Contains(MessageStrings.MissingProperty(ManageProperties.rowTypeValue)), "Error message is wrong");
            Assert.IsTrue(errorMessages.Contains(MessageStrings.MissingProperty(ManageProperties.typeOfOutput)), "Error message is wrong");
            Assert.IsTrue(errorMessages.Contains(MessageStrings.MissingProperty(ManageProperties.masterRecordID)), "Error message is wrong");
        }

        #endregion

        #region ValidateComponentProperties

        [TestMethod]
        public void TestValidateComponentProperties_Valid()
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

            ManageProperties.AddComponentProperties(output.CustomPropertyCollection);
            ManageProperties.AddOutputProperties(output.CustomPropertyCollection);
            ManageProperties.AddOutputColumnProperties(output.CustomPropertyCollection);

            ManageProperties propertyManager = new ManageProperties();
            propertyManager.PostErrorEvent += new PostErrorDelegate(this.PostError);
            errorMessages = new List<string>();

            DTSValidationStatus expected = DTSValidationStatus.VS_ISVALID;
            DTSValidationStatus actual = propertyManager.ValidateComponentProperties(output.CustomPropertyCollection, DTSValidationStatus.VS_ISVALID);

            Assert.AreEqual(expected, actual, "Validation Status Wrong");
            Assert.AreEqual(0, errorMessages.Count, "Number of error messages wrong");
        }


        [TestMethod]
        public void TestValidateComponentProperties_MissingProperty()
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

            //ManageProperties.AddComponentProperties(output.CustomPropertyCollection);
            ManageProperties.AddOutputProperties(output.CustomPropertyCollection);
            ManageProperties.AddOutputColumnProperties(output.CustomPropertyCollection);

            ManageProperties propertyManager = new ManageProperties();
            propertyManager.PostErrorEvent += new PostErrorDelegate(this.PostError);
            errorMessages = new List<string>();

            DTSValidationStatus expected = DTSValidationStatus.VS_ISCORRUPT;
            DTSValidationStatus actual = propertyManager.ValidateComponentProperties(output.CustomPropertyCollection, DTSValidationStatus.VS_ISVALID);

            Assert.AreEqual(expected, actual, "Validation Status Wrong");
            Assert.AreEqual(4, errorMessages.Count, "Number of error messages wrong");
            Assert.IsTrue(errorMessages.Contains(MessageStrings.MissingProperty(ManageProperties.isTextDelmited)), "Error message is wrong");
            Assert.IsTrue(errorMessages.Contains(MessageStrings.MissingProperty(ManageProperties.textDelmiter)), "Error message is wrong");
            Assert.IsTrue(errorMessages.Contains(MessageStrings.MissingProperty(ManageProperties.columnDelimiter)), "Error message is wrong");
            Assert.IsTrue(errorMessages.Contains(MessageStrings.MissingProperty(ManageProperties.treatEmptyStringsAsNull)), "Error message is wrong");
        }

        #endregion

        #region ValidateOutputColumnProperties

        [TestMethod]
        public void TestValidateOutputColumnProperties_Valid()
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

            ManageProperties.AddComponentProperties(output.CustomPropertyCollection);
            ManageProperties.AddOutputProperties(output.CustomPropertyCollection);
            ManageProperties.AddOutputColumnProperties(output.CustomPropertyCollection);

            ManageProperties propertyManager = new ManageProperties();
            propertyManager.PostErrorEvent += new PostErrorDelegate(this.PostError);
            errorMessages = new List<string>();

            DTSValidationStatus expected = DTSValidationStatus.VS_ISVALID;
            DTSValidationStatus actual = propertyManager.ValidateOutputColumnProperties(output.CustomPropertyCollection, DTSValidationStatus.VS_ISVALID);

            Assert.AreEqual(expected, actual, "Validation Status Wrong");
            Assert.AreEqual(0, errorMessages.Count, "Number of error messages wrong");
        }


        [TestMethod]
        public void TestValidateOutputColumnProperties_MissingProperty()
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

            ManageProperties.AddComponentProperties(output.CustomPropertyCollection);
            ManageProperties.AddOutputProperties(output.CustomPropertyCollection);
            //ManageProperties.AddOutputColumnProperties(output.CustomPropertyCollection);

            ManageProperties propertyManager = new ManageProperties();
            propertyManager.PostErrorEvent += new PostErrorDelegate(this.PostError);
            errorMessages = new List<string>();

            DTSValidationStatus expected = DTSValidationStatus.VS_ISCORRUPT;
            DTSValidationStatus actual = propertyManager.ValidateOutputColumnProperties(output.CustomPropertyCollection, DTSValidationStatus.VS_ISVALID);

            Assert.AreEqual(expected, actual, "Validation Status Wrong");
            Assert.AreEqual(5, errorMessages.Count, "Number of error messages wrong");
            Assert.IsTrue(errorMessages.Contains(MessageStrings.MissingProperty(ManageProperties.usageOfColumn)), "Error message is wrong");
            Assert.IsTrue(errorMessages.Contains(MessageStrings.MissingProperty(ManageProperties.keyOutputColumnID)), "Error message is wrong");
            Assert.IsTrue(errorMessages.Contains(MessageStrings.MissingProperty(ManageProperties.dotNetFormatString)), "Error message is wrong");
            Assert.IsTrue(errorMessages.Contains(MessageStrings.MissingProperty(ManageProperties.isColumnOptional)), "Error message is wrong");
            Assert.IsTrue(errorMessages.Contains(MessageStrings.MissingProperty(ManageProperties.nullResultOnConversionError)), "Error message is wrong");
        }

        #endregion

        #region SetPropertyValue

        [TestMethod]
        public void TestSetPropertyValue_Valid()
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

            ManageProperties.AddComponentProperties(output.CustomPropertyCollection);
            ManageProperties.AddOutputProperties(output.CustomPropertyCollection);
            ManageProperties.AddOutputColumnProperties(output.CustomPropertyCollection);

            ManageProperties propertyManager = new ManageProperties();
            propertyManager.PostErrorEvent += new PostErrorDelegate(this.PostError);
            errorMessages = new List<string>();
            Boolean expected = true;
            Boolean actual = ManageProperties.SetPropertyValue(output.CustomPropertyCollection, ManageProperties.isColumnOptional, true);

            Assert.AreEqual(expected, actual, "SetPropertyValue Return is wrong");
            Assert.AreEqual(0, errorMessages.Count, "Number of error messages wrong");
        }


        [TestMethod]
        public void TestSetPropertyValue_InValid()
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

            ManageProperties.AddComponentProperties(output.CustomPropertyCollection);
            ManageProperties.AddOutputProperties(output.CustomPropertyCollection);
            //ManageProperties.AddOutputColumnProperties(output.CustomPropertyCollection);

            ManageProperties propertyManager = new ManageProperties();
            propertyManager.PostErrorEvent += new PostErrorDelegate(this.PostError);
            errorMessages = new List<string>();
            Boolean expected = false;
            Boolean actual = ManageProperties.SetPropertyValue(output.CustomPropertyCollection, ManageProperties.isColumnOptional, true);

            Assert.AreEqual(expected, actual, "SetPropertyValue Return is wrong");
            Assert.AreEqual(0, errorMessages.Count, "Number of error messages wrong");
        }
        #endregion

        #region SetContainsLineage

        [TestMethod]
        public void TestSetContainsLineage_Valid()
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

            ManageProperties.AddComponentProperties(output.CustomPropertyCollection);
            ManageProperties.AddOutputProperties(output.CustomPropertyCollection);
            ManageProperties.AddOutputColumnProperties(output.CustomPropertyCollection);

            ManageProperties propertyManager = new ManageProperties();
            propertyManager.PostErrorEvent += new PostErrorDelegate(this.PostError);
            errorMessages = new List<string>();
            Boolean expected = true;
            Boolean actual = ManageProperties.SetContainsLineage(output.CustomPropertyCollection, ManageProperties.isColumnOptional, false);

            Assert.AreEqual(expected, actual, "SetContainsLineage Return is wrong");
            Assert.AreEqual(0, errorMessages.Count, "Number of error messages wrong");
        }


        [TestMethod]
        public void TestSetContainsLineage_InValid()
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

            ManageProperties.AddComponentProperties(output.CustomPropertyCollection);
            ManageProperties.AddOutputProperties(output.CustomPropertyCollection);
            //ManageProperties.AddOutputColumnProperties(output.CustomPropertyCollection);

            ManageProperties propertyManager = new ManageProperties();
            propertyManager.PostErrorEvent += new PostErrorDelegate(this.PostError);
            errorMessages = new List<string>();
            Boolean expected = false;
            Boolean actual = ManageProperties.SetContainsLineage(output.CustomPropertyCollection, ManageProperties.isColumnOptional, false);

            Assert.AreEqual(expected, actual, "SetContainsLineage Return is wrong");
            Assert.AreEqual(0, errorMessages.Count, "Number of error messages wrong");
        }
        #endregion

        #region AddMissingOutputColumnProperties
        [TestMethod]
        public void TestAddMissingOutputColumnProperties()
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

            ManageProperties.AddMissingOutputColumnProperties(output.CustomPropertyCollection);

            Assert.AreEqual(5, output.CustomPropertyCollection.Count, "CustomPropertyCollection.Count is wrong");
        }       
        #endregion

        #region GetPropertyValue
        [TestMethod]
        public void TestGetPropertyValue_Valid()
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

            ManageProperties.AddComponentProperties(output.CustomPropertyCollection);
            ManageProperties.AddOutputProperties(output.CustomPropertyCollection);
            ManageProperties.AddOutputColumnProperties(output.CustomPropertyCollection);

            ManageProperties propertyManager = new ManageProperties();
            propertyManager.PostErrorEvent += new PostErrorDelegate(this.PostError);
            errorMessages = new List<string>();
            Object expected = false;
            Object actual = ManageProperties.GetPropertyValue(output.CustomPropertyCollection, ManageProperties.isColumnOptional);

            Assert.AreEqual(expected, actual, "GetPropertyValue Return is wrong");
            Assert.AreEqual(0, errorMessages.Count, "Number of error messages wrong");
        }


        [TestMethod]
        public void TestGetPropertyValue_InValid()
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

            ManageProperties.AddComponentProperties(output.CustomPropertyCollection);
            ManageProperties.AddOutputProperties(output.CustomPropertyCollection);
            //ManageProperties.AddOutputColumnProperties(output.CustomPropertyCollection);

            ManageProperties propertyManager = new ManageProperties();
            propertyManager.PostErrorEvent += new PostErrorDelegate(this.PostError);
            errorMessages = new List<string>();
            Object expected = null;
            Object actual = ManageProperties.GetPropertyValue(output.CustomPropertyCollection, ManageProperties.isColumnOptional);

            Assert.AreEqual(expected, actual, "GetPropertyValue Return is wrong");
            Assert.AreEqual(0, errorMessages.Count, "Number of error messages wrong");
        }

        [TestMethod]
        public void TestGetPropertyValueSSISProperty_Valid()
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

            ManageProperties.AddComponentProperties(output.CustomPropertyCollection);
            ManageProperties.AddOutputProperties(output.CustomPropertyCollection);
            ManageProperties.AddOutputColumnProperties(output.CustomPropertyCollection);

            ManageProperties propertyManager = new ManageProperties();
            propertyManager.PostErrorEvent += new PostErrorDelegate(this.PostError);
            errorMessages = new List<string>();

            SSISOutput ssisOutput = new SSISOutput(output, null);

            Object expected = false;
            Object actual = ManageProperties.GetPropertyValue(ssisOutput.CustomPropertyCollection, ManageProperties.isColumnOptional);

            Assert.AreEqual(expected, actual, "GetPropertyValue Return is wrong");
            Assert.AreEqual(0, errorMessages.Count, "Number of error messages wrong");
        }


        [TestMethod]
        public void TestGetPropertyValueSSISProperty_InValid()
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

            ManageProperties.AddComponentProperties(output.CustomPropertyCollection);
            ManageProperties.AddOutputProperties(output.CustomPropertyCollection);
            //ManageProperties.AddOutputColumnProperties(output.CustomPropertyCollection);

            ManageProperties propertyManager = new ManageProperties();
            propertyManager.PostErrorEvent += new PostErrorDelegate(this.PostError);
            errorMessages = new List<string>();

            SSISOutput ssisOutput = new SSISOutput(output, null);

            Object expected = null;
            Object actual = ManageProperties.GetPropertyValue(ssisOutput.CustomPropertyCollection, ManageProperties.isColumnOptional);

            Assert.AreEqual(expected, actual, "GetPropertyValue Return is wrong");
            Assert.AreEqual(0, errorMessages.Count, "Number of error messages wrong");
        }        
        #endregion

        #region Error Delegate

        private List<String> errorMessages;

        void PostError(string errorMessage)
        {
            errorMessages.Add(errorMessage);
        }
        #endregion
    }
}
