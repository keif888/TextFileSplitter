using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Martin.SQLServer.Dts;
using Microsoft.SqlServer.Dts.Pipeline;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime.Wrapper;
using Microsoft.SqlServer.Dts.Runtime;
using System.Diagnostics;
using System.Collections.Generic;


namespace UnitTestTextFileSplitter2012
{
    [TestClass]
    public class TestTextFileSplitter
    {
        #region Provide Component Properties
        [TestMethod]
        public void TestProvideComponentProperties()
        {
            Microsoft.SqlServer.Dts.Runtime.Package package = new Microsoft.SqlServer.Dts.Runtime.Package();
            Executable exec = package.Executables.Add("STOCK:PipelineTask");
            Microsoft.SqlServer.Dts.Runtime.TaskHost thMainPipe = exec as Microsoft.SqlServer.Dts.Runtime.TaskHost;
            MainPipe dataFlowTask = thMainPipe.InnerObject as MainPipe;

            IDTSComponentMetaData100 textFileSplitter = dataFlowTask.ComponentMetaDataCollection.New();
            textFileSplitter.Name = "Row Splitter Test";
            textFileSplitter.ComponentClassID = typeof(Martin.SQLServer.Dts.TextFileSplitter).AssemblyQualifiedName;
            CManagedComponentWrapper instance = textFileSplitter.Instantiate();

            instance.ProvideComponentProperties();

            Assert.AreEqual(5, textFileSplitter.Version);
            Assert.AreEqual(true, textFileSplitter.UsesDispositions);
            Assert.AreEqual("http://TextFileSplitter.codeplex.com/", textFileSplitter.ContactInfo);
            Assert.AreEqual(true, (Boolean)textFileSplitter.CustomPropertyCollection[ManageProperties.isTextDelmited].Value);
            Assert.AreEqual("\"", (String)textFileSplitter.CustomPropertyCollection[ManageProperties.textDelmiter].Value);
            Assert.AreEqual(",", (String)textFileSplitter.CustomPropertyCollection[ManageProperties.columnDelimiter].Value);
            Assert.AreEqual(true, (Boolean)textFileSplitter.CustomPropertyCollection[ManageProperties.treatEmptyStringsAsNull].Value);

            Assert.AreEqual(4, textFileSplitter.OutputCollection.Count);
            //ToDo: Add Asserts on all the Columns...

        }
        #endregion

        #region Perform Upgrade
        [TestMethod]
        public void TestPerformUpgrade()
        {
            Microsoft.SqlServer.Dts.Runtime.Package package = new Microsoft.SqlServer.Dts.Runtime.Package();
            Executable exec = package.Executables.Add("STOCK:PipelineTask");
            Microsoft.SqlServer.Dts.Runtime.TaskHost thMainPipe = exec as Microsoft.SqlServer.Dts.Runtime.TaskHost;
            MainPipe dataFlowTask = thMainPipe.InnerObject as MainPipe;

            IDTSComponentMetaData100 textFileSplitter = dataFlowTask.ComponentMetaDataCollection.New();
            textFileSplitter.Name = "Row Splitter Test";
            textFileSplitter.ComponentClassID = typeof(Martin.SQLServer.Dts.TextFileSplitter).AssemblyQualifiedName;
            CManagedComponentWrapper instance = textFileSplitter.Instantiate();

            instance.ProvideComponentProperties();
            instance.ReinitializeMetaData();

            foreach (IDTSOutput100 output in textFileSplitter.OutputCollection)
            {
                if ((Utilities.typeOfOutputEnum)ManageProperties.GetPropertyValue(output.CustomPropertyCollection, ManageProperties.typeOfOutput) == Utilities.typeOfOutputEnum.KeyRecords)
                {
                    IDTSOutputColumn100 newColumn = instance.InsertOutputColumnAt(output.ID, 0, "New Column", String.Empty);
                    int isColumnID = newColumn.CustomPropertyCollection[ManageProperties.isColumnOptional].ID;
                    newColumn.CustomPropertyCollection.RemoveObjectByID(isColumnID);
                    newColumn.CustomPropertyCollection[0].ExpressionType = DTSCustomPropertyExpressionType.CPET_NOTIFY;
                    break;
                }
            }

            Assert.AreEqual(5, textFileSplitter.Version);
            textFileSplitter.Version = 0;

            string packageXML, package2XML;
            package.SaveToXML(out packageXML, null);
            package = new Microsoft.SqlServer.Dts.Runtime.Package();
            // Force the Upgrade to run the 1st time.
            package.LoadFromXML(packageXML, null);
            // We have to save it, so that the meta data will be shown as changed.  Don't ask, it's an SSIS thing.
            package.SaveToXML(out package2XML, null);
            package = new Microsoft.SqlServer.Dts.Runtime.Package();
            // Load again, so that we can check that the version number is now updated!
            package.LoadFromXML(package2XML, null);

            exec = package.Executables[0];
            thMainPipe = exec as Microsoft.SqlServer.Dts.Runtime.TaskHost;
            dataFlowTask = thMainPipe.InnerObject as MainPipe;
            textFileSplitter = dataFlowTask.ComponentMetaDataCollection[0];
            Assert.AreEqual(5, textFileSplitter.Version, "Version failed to match on reload");
            foreach (IDTSOutput100 output in textFileSplitter.OutputCollection)
            {
                if ((Utilities.typeOfOutputEnum)ManageProperties.GetPropertyValue(output.CustomPropertyCollection, ManageProperties.typeOfOutput) == Utilities.typeOfOutputEnum.KeyRecords)
                {
                    Assert.AreEqual(false, ManageProperties.GetPropertyValue(output.OutputColumnCollection[0].CustomPropertyCollection, ManageProperties.isColumnOptional), "isColumnOptional Property not added");
                    Assert.AreEqual(DTSCustomPropertyExpressionType.CPET_NONE, output.OutputColumnCollection[0].CustomPropertyCollection[0].ExpressionType, "Expression Type hasn't been reset.");
                    break;
                }
            }

        }

        #endregion

        #region Validate
        [TestMethod]
        public void TestValidate_ValidateComponentProperties_VS_NEEDSNEWMETADATA()
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

            Assert.AreEqual(DTSValidationStatus.VS_NEEDSNEWMETADATA, instance.Validate());
            Assert.AreEqual("[Warning] Text File Splitter Source: The Connection Manager Needs to be Setup.", events.errorMessages[0]);
        }

        [TestMethod]
        public void TestValidate_ValidateComponentProperties_VS_ISVALID()
        {
            Microsoft.SqlServer.Dts.Runtime.Package package = new Microsoft.SqlServer.Dts.Runtime.Package();
            Executable exec = package.Executables.Add("STOCK:PipelineTask");
            Microsoft.SqlServer.Dts.Runtime.TaskHost thMainPipe = exec as Microsoft.SqlServer.Dts.Runtime.TaskHost;
            MainPipe dataFlowTask = thMainPipe.InnerObject as MainPipe;

            // Create a flat file source
            ConnectionManager flatFileConnectionManager = package.Connections.Add("FLATFILE");
            flatFileConnectionManager.Properties["Format"].SetValue(flatFileConnectionManager, "Delimited");
            flatFileConnectionManager.Properties["Name"].SetValue(flatFileConnectionManager, "Flat File Connection");
            flatFileConnectionManager.Properties["ConnectionString"].SetValue(flatFileConnectionManager, @".\SampleSourceFile.txt");
            flatFileConnectionManager.Properties["ColumnNamesInFirstDataRow"].SetValue(flatFileConnectionManager, false);
            flatFileConnectionManager.Properties["HeaderRowDelimiter"].SetValue(flatFileConnectionManager, "\r\n");
            flatFileConnectionManager.Properties["TextQualifier"].SetValue(flatFileConnectionManager, "\"");
            flatFileConnectionManager.Properties["DataRowsToSkip"].SetValue(flatFileConnectionManager, 0);
            flatFileConnectionManager.Properties["Unicode"].SetValue(flatFileConnectionManager, false);
            flatFileConnectionManager.Properties["CodePage"].SetValue(flatFileConnectionManager, 1252);

            // Create the columns in the flat file
            IDTSConnectionManagerFlatFile100 flatFileConnection = flatFileConnectionManager.InnerObject as IDTSConnectionManagerFlatFile100;
            IDTSConnectionManagerFlatFileColumn100 rowTypeColumn = flatFileConnection.Columns.Add();
            rowTypeColumn.ColumnDelimiter = "|";
            rowTypeColumn.ColumnType = "Delimited";
            rowTypeColumn.DataType = DataType.DT_STR;
            rowTypeColumn.DataPrecision = 0;
            rowTypeColumn.DataScale = 0;
            rowTypeColumn.MaximumWidth = 10;
            ((IDTSName100)rowTypeColumn).Name = "rowType";
            IDTSConnectionManagerFlatFileColumn100 dataColumn = flatFileConnection.Columns.Add();
            dataColumn.ColumnDelimiter = "\r\n";
            dataColumn.ColumnType = "Delimited";
            dataColumn.DataType = DataType.DT_TEXT;
            dataColumn.DataPrecision = 0;
            dataColumn.DataScale = 0;
            ((IDTSName100)dataColumn).Name = "Data";


            IDTSComponentMetaData100 textFileSplitter = dataFlowTask.ComponentMetaDataCollection.New();
            textFileSplitter.Name = "Row Splitter Test";
            textFileSplitter.ComponentClassID = typeof(Martin.SQLServer.Dts.TextFileSplitter).AssemblyQualifiedName;
            CManagedComponentWrapper instance = textFileSplitter.Instantiate();

            instance.ProvideComponentProperties();

            textFileSplitter.RuntimeConnectionCollection[0].ConnectionManager = DtsConvert.GetExtendedInterface(flatFileConnectionManager);
            textFileSplitter.RuntimeConnectionCollection[0].ConnectionManagerID = flatFileConnectionManager.ID;
            instance.AcquireConnections(null);
            instance.ReinitializeMetaData();
            instance.ReleaseConnections();

            Assert.AreEqual(DTSValidationStatus.VS_ISVALID, instance.Validate());
        }

        [TestMethod]
        public void TestValidate_ValidateComponentProperties_VS_ISBROKEN()
        {
            Microsoft.SqlServer.Dts.Runtime.Package package = new Microsoft.SqlServer.Dts.Runtime.Package();
            Executable exec = package.Executables.Add("STOCK:PipelineTask");
            Microsoft.SqlServer.Dts.Runtime.TaskHost thMainPipe = exec as Microsoft.SqlServer.Dts.Runtime.TaskHost;
            MainPipe dataFlowTask = thMainPipe.InnerObject as MainPipe;

            IDTSComponentMetaData100 textFileSplitter = dataFlowTask.ComponentMetaDataCollection.New();
            textFileSplitter.Name = "Row Splitter Test";
            textFileSplitter.ComponentClassID = typeof(Martin.SQLServer.Dts.TextFileSplitter).AssemblyQualifiedName;
            CManagedComponentWrapper instance = textFileSplitter.Instantiate();

            instance.ProvideComponentProperties();

            textFileSplitter.OutputCollection[0].CustomPropertyCollection[ManageProperties.typeOfOutput].Value = Utilities.typeOfOutputEnum.DataRecords;

            Assert.AreEqual(DTSValidationStatus.VS_ISBROKEN, instance.Validate());
        }

        [TestMethod]
        public void TestValidate_ValidateComponentProperties_VS_ISCORRUPT()
        {
            Microsoft.SqlServer.Dts.Runtime.Package package = new Microsoft.SqlServer.Dts.Runtime.Package();
            Executable exec = package.Executables.Add("STOCK:PipelineTask");
            Microsoft.SqlServer.Dts.Runtime.TaskHost thMainPipe = exec as Microsoft.SqlServer.Dts.Runtime.TaskHost;
            MainPipe dataFlowTask = thMainPipe.InnerObject as MainPipe;

            IDTSComponentMetaData100 textFileSplitter = dataFlowTask.ComponentMetaDataCollection.New();
            textFileSplitter.Name = "Row Splitter Test";
            textFileSplitter.ComponentClassID = typeof(Martin.SQLServer.Dts.TextFileSplitter).AssemblyQualifiedName;
            CManagedComponentWrapper instance = textFileSplitter.Instantiate();

            instance.ProvideComponentProperties();

            textFileSplitter.OutputCollection.RemoveObjectByIndex(0);

            Assert.AreEqual(DTSValidationStatus.VS_ISCORRUPT, instance.Validate());
        }



        #endregion

        #region ValidateComponentProperties

        [TestMethod]
        public void TestValidate_ValidateComponentProperties_MissingComponentProperty()
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

            textFileSplitter.CustomPropertyCollection.RemoveObjectByIndex(2);

            Assert.AreEqual(DTSValidationStatus.VS_ISCORRUPT, instance.Validate());
            Assert.AreEqual(2, events.errorMessages.Count);
            Assert.AreEqual("[Warning] Text File Splitter Source: The Connection Manager Needs to be Setup.", events.errorMessages[1]);
            Assert.AreEqual("[Error] Text File Splitter Source: The custom property columnDelimiter is missing.", events.errorMessages[0]);
        }

        #endregion

        #region ValidateOutputs

        [TestMethod]
        public void TestValidate_ValidateOutputs_WrongNumberOfOutputs()
        {
            Microsoft.SqlServer.Dts.Runtime.Package package = new Microsoft.SqlServer.Dts.Runtime.Package();
            Executable exec = package.Executables.Add("STOCK:PipelineTask");
            Microsoft.SqlServer.Dts.Runtime.TaskHost thMainPipe = exec as Microsoft.SqlServer.Dts.Runtime.TaskHost;
            MainPipe dataFlowTask = thMainPipe.InnerObject as MainPipe;
            ComponentEventHandler events = new ComponentEventHandler();
            dataFlowTask.Events = DtsConvert.GetExtendedInterface(events as IDTSComponentEvents);

            // Create a flat file source
            ConnectionManager flatFileConnectionManager = package.Connections.Add("FLATFILE");
            flatFileConnectionManager.Properties["Format"].SetValue(flatFileConnectionManager, "Delimited");
            flatFileConnectionManager.Properties["Name"].SetValue(flatFileConnectionManager, "Flat File Connection");
            flatFileConnectionManager.Properties["ConnectionString"].SetValue(flatFileConnectionManager, @".\SampleSourceFile.txt");
            flatFileConnectionManager.Properties["ColumnNamesInFirstDataRow"].SetValue(flatFileConnectionManager, false);
            flatFileConnectionManager.Properties["HeaderRowDelimiter"].SetValue(flatFileConnectionManager, "\r\n");
            flatFileConnectionManager.Properties["TextQualifier"].SetValue(flatFileConnectionManager, "\"");
            flatFileConnectionManager.Properties["DataRowsToSkip"].SetValue(flatFileConnectionManager, 0);
            flatFileConnectionManager.Properties["Unicode"].SetValue(flatFileConnectionManager, false);
            flatFileConnectionManager.Properties["CodePage"].SetValue(flatFileConnectionManager, 1252);

            // Create the columns in the flat file
            IDTSConnectionManagerFlatFile100 flatFileConnection = flatFileConnectionManager.InnerObject as IDTSConnectionManagerFlatFile100;
            IDTSConnectionManagerFlatFileColumn100 rowTypeColumn = flatFileConnection.Columns.Add();
            rowTypeColumn.ColumnDelimiter = "|";
            rowTypeColumn.ColumnType = "Delimited";
            rowTypeColumn.DataType = DataType.DT_STR;
            rowTypeColumn.DataPrecision = 0;
            rowTypeColumn.DataScale = 0;
            rowTypeColumn.MaximumWidth = 10;
            ((IDTSName100)rowTypeColumn).Name = "rowType";
            IDTSConnectionManagerFlatFileColumn100 dataColumn = flatFileConnection.Columns.Add();
            dataColumn.ColumnDelimiter = "\r\n";
            dataColumn.ColumnType = "Delimited";
            dataColumn.DataType = DataType.DT_TEXT;
            dataColumn.DataPrecision = 0;
            dataColumn.DataScale = 0;
            ((IDTSName100)dataColumn).Name = "Data";


            IDTSComponentMetaData100 textFileSplitter = dataFlowTask.ComponentMetaDataCollection.New();
            textFileSplitter.Name = "Row Splitter Test";
            textFileSplitter.ComponentClassID = typeof(Martin.SQLServer.Dts.TextFileSplitter).AssemblyQualifiedName;
            CManagedComponentWrapper instance = textFileSplitter.Instantiate();

            instance.ProvideComponentProperties();

            textFileSplitter.RuntimeConnectionCollection[0].ConnectionManager = DtsConvert.GetExtendedInterface(flatFileConnectionManager);
            textFileSplitter.RuntimeConnectionCollection[0].ConnectionManagerID = flatFileConnectionManager.ID;
            instance.AcquireConnections(null);
            instance.ReinitializeMetaData();
            instance.ReleaseConnections();

            textFileSplitter.OutputCollection.RemoveObjectByIndex(3);

            Assert.AreEqual(DTSValidationStatus.VS_ISCORRUPT, instance.Validate());
            Assert.AreEqual(1, events.errorMessages.Count);
            Assert.AreEqual(String.Format("[Error] Text File Splitter Source: {0}", MessageStrings.UnexpectedNumberOfOutputs), events.errorMessages[0]);
        }

        [TestMethod]
        public void TestValidate_ValidateOutputs_MissingErrorOutput()
        {
            Microsoft.SqlServer.Dts.Runtime.Package package = new Microsoft.SqlServer.Dts.Runtime.Package();
            Executable exec = package.Executables.Add("STOCK:PipelineTask");
            Microsoft.SqlServer.Dts.Runtime.TaskHost thMainPipe = exec as Microsoft.SqlServer.Dts.Runtime.TaskHost;
            MainPipe dataFlowTask = thMainPipe.InnerObject as MainPipe;
            ComponentEventHandler events = new ComponentEventHandler();
            dataFlowTask.Events = DtsConvert.GetExtendedInterface(events as IDTSComponentEvents);

            // Create a flat file source
            ConnectionManager flatFileConnectionManager = package.Connections.Add("FLATFILE");
            flatFileConnectionManager.Properties["Format"].SetValue(flatFileConnectionManager, "Delimited");
            flatFileConnectionManager.Properties["Name"].SetValue(flatFileConnectionManager, "Flat File Connection");
            flatFileConnectionManager.Properties["ConnectionString"].SetValue(flatFileConnectionManager, @".\SampleSourceFile.txt");
            flatFileConnectionManager.Properties["ColumnNamesInFirstDataRow"].SetValue(flatFileConnectionManager, false);
            flatFileConnectionManager.Properties["HeaderRowDelimiter"].SetValue(flatFileConnectionManager, "\r\n");
            flatFileConnectionManager.Properties["TextQualifier"].SetValue(flatFileConnectionManager, "\"");
            flatFileConnectionManager.Properties["DataRowsToSkip"].SetValue(flatFileConnectionManager, 0);
            flatFileConnectionManager.Properties["Unicode"].SetValue(flatFileConnectionManager, false);
            flatFileConnectionManager.Properties["CodePage"].SetValue(flatFileConnectionManager, 1252);

            // Create the columns in the flat file
            IDTSConnectionManagerFlatFile100 flatFileConnection = flatFileConnectionManager.InnerObject as IDTSConnectionManagerFlatFile100;
            IDTSConnectionManagerFlatFileColumn100 rowTypeColumn = flatFileConnection.Columns.Add();
            rowTypeColumn.ColumnDelimiter = "|";
            rowTypeColumn.ColumnType = "Delimited";
            rowTypeColumn.DataType = DataType.DT_STR;
            rowTypeColumn.DataPrecision = 0;
            rowTypeColumn.DataScale = 0;
            rowTypeColumn.MaximumWidth = 10;
            ((IDTSName100)rowTypeColumn).Name = "rowType";
            IDTSConnectionManagerFlatFileColumn100 dataColumn = flatFileConnection.Columns.Add();
            dataColumn.ColumnDelimiter = "\r\n";
            dataColumn.ColumnType = "Delimited";
            dataColumn.DataType = DataType.DT_TEXT;
            dataColumn.DataPrecision = 0;
            dataColumn.DataScale = 0;
            ((IDTSName100)dataColumn).Name = "Data";


            IDTSComponentMetaData100 textFileSplitter = dataFlowTask.ComponentMetaDataCollection.New();
            textFileSplitter.Name = "Row Splitter Test";
            textFileSplitter.ComponentClassID = typeof(Martin.SQLServer.Dts.TextFileSplitter).AssemblyQualifiedName;
            CManagedComponentWrapper instance = textFileSplitter.Instantiate();

            instance.ProvideComponentProperties();

            textFileSplitter.RuntimeConnectionCollection[0].ConnectionManager = DtsConvert.GetExtendedInterface(flatFileConnectionManager);
            textFileSplitter.RuntimeConnectionCollection[0].ConnectionManagerID = flatFileConnectionManager.ID;
            instance.AcquireConnections(null);
            instance.ReinitializeMetaData();
            instance.ReleaseConnections();

            textFileSplitter.OutputCollection.RemoveObjectByIndex(1);
            IDTSOutput100 newOutput = instance.InsertOutput(DTSInsertPlacement.IP_AFTER, 0);
            newOutput.Name = "NewOutput";

            Assert.AreEqual(DTSValidationStatus.VS_ISBROKEN, instance.Validate());
            Assert.AreEqual(1, events.errorMessages.Count);
            Assert.AreEqual(String.Format("[Error] Text File Splitter Source: {0}", MessageStrings.NoErrorOutput), events.errorMessages[0]);
        }

        [TestMethod]
        public void TestValidate_ValidateOutputs_MissingPassThroughOutput()
        {
            Microsoft.SqlServer.Dts.Runtime.Package package = new Microsoft.SqlServer.Dts.Runtime.Package();
            Executable exec = package.Executables.Add("STOCK:PipelineTask");
            Microsoft.SqlServer.Dts.Runtime.TaskHost thMainPipe = exec as Microsoft.SqlServer.Dts.Runtime.TaskHost;
            MainPipe dataFlowTask = thMainPipe.InnerObject as MainPipe;
            ComponentEventHandler events = new ComponentEventHandler();
            dataFlowTask.Events = DtsConvert.GetExtendedInterface(events as IDTSComponentEvents);

            // Create a flat file source
            ConnectionManager flatFileConnectionManager = package.Connections.Add("FLATFILE");
            flatFileConnectionManager.Properties["Format"].SetValue(flatFileConnectionManager, "Delimited");
            flatFileConnectionManager.Properties["Name"].SetValue(flatFileConnectionManager, "Flat File Connection");
            flatFileConnectionManager.Properties["ConnectionString"].SetValue(flatFileConnectionManager, @".\SampleSourceFile.txt");
            flatFileConnectionManager.Properties["ColumnNamesInFirstDataRow"].SetValue(flatFileConnectionManager, false);
            flatFileConnectionManager.Properties["HeaderRowDelimiter"].SetValue(flatFileConnectionManager, "\r\n");
            flatFileConnectionManager.Properties["TextQualifier"].SetValue(flatFileConnectionManager, "\"");
            flatFileConnectionManager.Properties["DataRowsToSkip"].SetValue(flatFileConnectionManager, 0);
            flatFileConnectionManager.Properties["Unicode"].SetValue(flatFileConnectionManager, false);
            flatFileConnectionManager.Properties["CodePage"].SetValue(flatFileConnectionManager, 1252);

            // Create the columns in the flat file
            IDTSConnectionManagerFlatFile100 flatFileConnection = flatFileConnectionManager.InnerObject as IDTSConnectionManagerFlatFile100;
            IDTSConnectionManagerFlatFileColumn100 rowTypeColumn = flatFileConnection.Columns.Add();
            rowTypeColumn.ColumnDelimiter = "|";
            rowTypeColumn.ColumnType = "Delimited";
            rowTypeColumn.DataType = DataType.DT_STR;
            rowTypeColumn.DataPrecision = 0;
            rowTypeColumn.DataScale = 0;
            rowTypeColumn.MaximumWidth = 10;
            ((IDTSName100)rowTypeColumn).Name = "rowType";
            IDTSConnectionManagerFlatFileColumn100 dataColumn = flatFileConnection.Columns.Add();
            dataColumn.ColumnDelimiter = "\r\n";
            dataColumn.ColumnType = "Delimited";
            dataColumn.DataType = DataType.DT_TEXT;
            dataColumn.DataPrecision = 0;
            dataColumn.DataScale = 0;
            ((IDTSName100)dataColumn).Name = "Data";


            IDTSComponentMetaData100 textFileSplitter = dataFlowTask.ComponentMetaDataCollection.New();
            textFileSplitter.Name = "Row Splitter Test";
            textFileSplitter.ComponentClassID = typeof(Martin.SQLServer.Dts.TextFileSplitter).AssemblyQualifiedName;
            CManagedComponentWrapper instance = textFileSplitter.Instantiate();

            instance.ProvideComponentProperties();

            textFileSplitter.RuntimeConnectionCollection[0].ConnectionManager = DtsConvert.GetExtendedInterface(flatFileConnectionManager);
            textFileSplitter.RuntimeConnectionCollection[0].ConnectionManagerID = flatFileConnectionManager.ID;
            instance.AcquireConnections(null);
            instance.ReinitializeMetaData();
            instance.ReleaseConnections();

            textFileSplitter.OutputCollection.RemoveObjectByIndex(0);
            IDTSOutput100 newOutput = instance.InsertOutput(DTSInsertPlacement.IP_AFTER, 0);
            newOutput.Name = "NewOutput";

            Assert.AreEqual(DTSValidationStatus.VS_ISBROKEN, instance.Validate());
            Assert.AreEqual(1, events.errorMessages.Count);
            Assert.AreEqual(String.Format("[Error] Text File Splitter Source: {0}", MessageStrings.InvalidPassThoughOutput), events.errorMessages[0]);
        }

        [TestMethod]
        public void TestValidate_ValidateOutputs_MissingKeyOutput()
        {
            Microsoft.SqlServer.Dts.Runtime.Package package = new Microsoft.SqlServer.Dts.Runtime.Package();
            Executable exec = package.Executables.Add("STOCK:PipelineTask");
            Microsoft.SqlServer.Dts.Runtime.TaskHost thMainPipe = exec as Microsoft.SqlServer.Dts.Runtime.TaskHost;
            MainPipe dataFlowTask = thMainPipe.InnerObject as MainPipe;
            ComponentEventHandler events = new ComponentEventHandler();
            dataFlowTask.Events = DtsConvert.GetExtendedInterface(events as IDTSComponentEvents);

            // Create a flat file source
            ConnectionManager flatFileConnectionManager = package.Connections.Add("FLATFILE");
            flatFileConnectionManager.Properties["Format"].SetValue(flatFileConnectionManager, "Delimited");
            flatFileConnectionManager.Properties["Name"].SetValue(flatFileConnectionManager, "Flat File Connection");
            flatFileConnectionManager.Properties["ConnectionString"].SetValue(flatFileConnectionManager, @".\SampleSourceFile.txt");
            flatFileConnectionManager.Properties["ColumnNamesInFirstDataRow"].SetValue(flatFileConnectionManager, false);
            flatFileConnectionManager.Properties["HeaderRowDelimiter"].SetValue(flatFileConnectionManager, "\r\n");
            flatFileConnectionManager.Properties["TextQualifier"].SetValue(flatFileConnectionManager, "\"");
            flatFileConnectionManager.Properties["DataRowsToSkip"].SetValue(flatFileConnectionManager, 0);
            flatFileConnectionManager.Properties["Unicode"].SetValue(flatFileConnectionManager, false);
            flatFileConnectionManager.Properties["CodePage"].SetValue(flatFileConnectionManager, 1252);

            // Create the columns in the flat file
            IDTSConnectionManagerFlatFile100 flatFileConnection = flatFileConnectionManager.InnerObject as IDTSConnectionManagerFlatFile100;
            IDTSConnectionManagerFlatFileColumn100 rowTypeColumn = flatFileConnection.Columns.Add();
            rowTypeColumn.ColumnDelimiter = "|";
            rowTypeColumn.ColumnType = "Delimited";
            rowTypeColumn.DataType = DataType.DT_STR;
            rowTypeColumn.DataPrecision = 0;
            rowTypeColumn.DataScale = 0;
            rowTypeColumn.MaximumWidth = 10;
            ((IDTSName100)rowTypeColumn).Name = "rowType";
            IDTSConnectionManagerFlatFileColumn100 dataColumn = flatFileConnection.Columns.Add();
            dataColumn.ColumnDelimiter = "\r\n";
            dataColumn.ColumnType = "Delimited";
            dataColumn.DataType = DataType.DT_TEXT;
            dataColumn.DataPrecision = 0;
            dataColumn.DataScale = 0;
            ((IDTSName100)dataColumn).Name = "Data";


            IDTSComponentMetaData100 textFileSplitter = dataFlowTask.ComponentMetaDataCollection.New();
            textFileSplitter.Name = "Row Splitter Test";
            textFileSplitter.ComponentClassID = typeof(Martin.SQLServer.Dts.TextFileSplitter).AssemblyQualifiedName;
            CManagedComponentWrapper instance = textFileSplitter.Instantiate();

            instance.ProvideComponentProperties();

            textFileSplitter.RuntimeConnectionCollection[0].ConnectionManager = DtsConvert.GetExtendedInterface(flatFileConnectionManager);
            textFileSplitter.RuntimeConnectionCollection[0].ConnectionManagerID = flatFileConnectionManager.ID;
            instance.AcquireConnections(null);
            instance.ReinitializeMetaData();
            instance.ReleaseConnections();

            textFileSplitter.OutputCollection.RemoveObjectByIndex(2);
            IDTSOutput100 newOutput = instance.InsertOutput(DTSInsertPlacement.IP_AFTER, 0);
            newOutput.Name = "NewOutput";

            Assert.AreEqual(DTSValidationStatus.VS_ISBROKEN, instance.Validate());
            Assert.AreEqual(2, events.errorMessages.Count);
            Assert.IsTrue(events.errorMessages.Contains(String.Format("[Error] Text File Splitter Source: {0}", MessageStrings.InvalidPassKeyOutput)), MessageStrings.InvalidPassKeyOutput);
        }

        [TestMethod]
        public void TestValidate_ValidateOutputs_MissingRowCountOutput()
        {
            Microsoft.SqlServer.Dts.Runtime.Package package = new Microsoft.SqlServer.Dts.Runtime.Package();
            Executable exec = package.Executables.Add("STOCK:PipelineTask");
            Microsoft.SqlServer.Dts.Runtime.TaskHost thMainPipe = exec as Microsoft.SqlServer.Dts.Runtime.TaskHost;
            MainPipe dataFlowTask = thMainPipe.InnerObject as MainPipe;
            ComponentEventHandler events = new ComponentEventHandler();
            dataFlowTask.Events = DtsConvert.GetExtendedInterface(events as IDTSComponentEvents);

            // Create a flat file source
            ConnectionManager flatFileConnectionManager = package.Connections.Add("FLATFILE");
            flatFileConnectionManager.Properties["Format"].SetValue(flatFileConnectionManager, "Delimited");
            flatFileConnectionManager.Properties["Name"].SetValue(flatFileConnectionManager, "Flat File Connection");
            flatFileConnectionManager.Properties["ConnectionString"].SetValue(flatFileConnectionManager, @".\SampleSourceFile.txt");
            flatFileConnectionManager.Properties["ColumnNamesInFirstDataRow"].SetValue(flatFileConnectionManager, false);
            flatFileConnectionManager.Properties["HeaderRowDelimiter"].SetValue(flatFileConnectionManager, "\r\n");
            flatFileConnectionManager.Properties["TextQualifier"].SetValue(flatFileConnectionManager, "\"");
            flatFileConnectionManager.Properties["DataRowsToSkip"].SetValue(flatFileConnectionManager, 0);
            flatFileConnectionManager.Properties["Unicode"].SetValue(flatFileConnectionManager, false);
            flatFileConnectionManager.Properties["CodePage"].SetValue(flatFileConnectionManager, 1252);

            // Create the columns in the flat file
            IDTSConnectionManagerFlatFile100 flatFileConnection = flatFileConnectionManager.InnerObject as IDTSConnectionManagerFlatFile100;
            IDTSConnectionManagerFlatFileColumn100 rowTypeColumn = flatFileConnection.Columns.Add();
            rowTypeColumn.ColumnDelimiter = "|";
            rowTypeColumn.ColumnType = "Delimited";
            rowTypeColumn.DataType = DataType.DT_STR;
            rowTypeColumn.DataPrecision = 0;
            rowTypeColumn.DataScale = 0;
            rowTypeColumn.MaximumWidth = 10;
            ((IDTSName100)rowTypeColumn).Name = "rowType";
            IDTSConnectionManagerFlatFileColumn100 dataColumn = flatFileConnection.Columns.Add();
            dataColumn.ColumnDelimiter = "\r\n";
            dataColumn.ColumnType = "Delimited";
            dataColumn.DataType = DataType.DT_TEXT;
            dataColumn.DataPrecision = 0;
            dataColumn.DataScale = 0;
            ((IDTSName100)dataColumn).Name = "Data";


            IDTSComponentMetaData100 textFileSplitter = dataFlowTask.ComponentMetaDataCollection.New();
            textFileSplitter.Name = "Row Splitter Test";
            textFileSplitter.ComponentClassID = typeof(Martin.SQLServer.Dts.TextFileSplitter).AssemblyQualifiedName;
            CManagedComponentWrapper instance = textFileSplitter.Instantiate();

            instance.ProvideComponentProperties();

            textFileSplitter.RuntimeConnectionCollection[0].ConnectionManager = DtsConvert.GetExtendedInterface(flatFileConnectionManager);
            textFileSplitter.RuntimeConnectionCollection[0].ConnectionManagerID = flatFileConnectionManager.ID;
            instance.AcquireConnections(null);
            instance.ReinitializeMetaData();
            instance.ReleaseConnections();

            textFileSplitter.OutputCollection.RemoveObjectByIndex(3);
            IDTSOutput100 newOutput = instance.InsertOutput(DTSInsertPlacement.IP_AFTER, 0);
            newOutput.Name = "NewOutput";

            Assert.AreEqual(DTSValidationStatus.VS_ISBROKEN, instance.Validate());
            Assert.AreEqual(1, events.errorMessages.Count);
            Assert.AreEqual(String.Format("[Error] Text File Splitter Source: {0}", MessageStrings.InvalidNumberOfRowsOutput), events.errorMessages[0]);
        }

        #endregion

        #region ValidateRegularOutput

        [TestMethod]
        public void TestValidate_ValidateRegularOutput_SyncID()
        {
            Microsoft.SqlServer.Dts.Runtime.Package package = new Microsoft.SqlServer.Dts.Runtime.Package();
            Executable exec = package.Executables.Add("STOCK:PipelineTask");
            Microsoft.SqlServer.Dts.Runtime.TaskHost thMainPipe = exec as Microsoft.SqlServer.Dts.Runtime.TaskHost;
            MainPipe dataFlowTask = thMainPipe.InnerObject as MainPipe;
            ComponentEventHandler events = new ComponentEventHandler();
            dataFlowTask.Events = DtsConvert.GetExtendedInterface(events as IDTSComponentEvents);

            // Create a flat file source
            ConnectionManager flatFileConnectionManager = package.Connections.Add("FLATFILE");
            flatFileConnectionManager.Properties["Format"].SetValue(flatFileConnectionManager, "Delimited");
            flatFileConnectionManager.Properties["Name"].SetValue(flatFileConnectionManager, "Flat File Connection");
            flatFileConnectionManager.Properties["ConnectionString"].SetValue(flatFileConnectionManager, @".\SampleSourceFile.txt");
            flatFileConnectionManager.Properties["ColumnNamesInFirstDataRow"].SetValue(flatFileConnectionManager, false);
            flatFileConnectionManager.Properties["HeaderRowDelimiter"].SetValue(flatFileConnectionManager, "\r\n");
            flatFileConnectionManager.Properties["TextQualifier"].SetValue(flatFileConnectionManager, "\"");
            flatFileConnectionManager.Properties["DataRowsToSkip"].SetValue(flatFileConnectionManager, 0);
            flatFileConnectionManager.Properties["Unicode"].SetValue(flatFileConnectionManager, false);
            flatFileConnectionManager.Properties["CodePage"].SetValue(flatFileConnectionManager, 1252);

            // Create the columns in the flat file
            IDTSConnectionManagerFlatFile100 flatFileConnection = flatFileConnectionManager.InnerObject as IDTSConnectionManagerFlatFile100;
            IDTSConnectionManagerFlatFileColumn100 rowTypeColumn = flatFileConnection.Columns.Add();
            rowTypeColumn.ColumnDelimiter = "|";
            rowTypeColumn.ColumnType = "Delimited";
            rowTypeColumn.DataType = DataType.DT_STR;
            rowTypeColumn.DataPrecision = 0;
            rowTypeColumn.DataScale = 0;
            rowTypeColumn.MaximumWidth = 10;
            ((IDTSName100)rowTypeColumn).Name = "rowType";
            IDTSConnectionManagerFlatFileColumn100 dataColumn = flatFileConnection.Columns.Add();
            dataColumn.ColumnDelimiter = "\r\n";
            dataColumn.ColumnType = "Delimited";
            dataColumn.DataType = DataType.DT_TEXT;
            dataColumn.DataPrecision = 0;
            dataColumn.DataScale = 0;
            ((IDTSName100)dataColumn).Name = "Data";


            IDTSComponentMetaData100 textFileSplitter = dataFlowTask.ComponentMetaDataCollection.New();
            textFileSplitter.Name = "Row Splitter Test";
            textFileSplitter.ComponentClassID = typeof(Martin.SQLServer.Dts.TextFileSplitter).AssemblyQualifiedName;
            CManagedComponentWrapper instance = textFileSplitter.Instantiate();

            instance.ProvideComponentProperties();

            textFileSplitter.RuntimeConnectionCollection[0].ConnectionManager = DtsConvert.GetExtendedInterface(flatFileConnectionManager);
            textFileSplitter.RuntimeConnectionCollection[0].ConnectionManagerID = flatFileConnectionManager.ID;
            instance.AcquireConnections(null);
            instance.ReinitializeMetaData();
            instance.ReleaseConnections();

            IDTSOutput100 passthroughOutput = textFileSplitter.OutputCollection[0];
            instance.SetOutputColumnProperty(passthroughOutput.ID, passthroughOutput.OutputColumnCollection[0].ID, ManageProperties.usageOfColumn, Utilities.usageOfColumnEnum.RowType);
            instance.SetOutputColumnProperty(passthroughOutput.ID, passthroughOutput.OutputColumnCollection[1].ID, ManageProperties.usageOfColumn, Utilities.usageOfColumnEnum.RowData);

            IDTSOutput100 newOutput = instance.InsertOutput(DTSInsertPlacement.IP_AFTER, 0);
            newOutput.Name = "NewOutput";
            newOutput.SynchronousInputID = 1;

            Assert.AreEqual(DTSValidationStatus.VS_ISBROKEN, instance.Validate());
            Assert.AreEqual(1, events.errorMessages.Count);
            Assert.IsTrue(events.errorMessages.Contains(String.Format("[Error] Text File Splitter Source: {0}",MessageStrings.OutputIsSyncronous(newOutput.Name))), MessageStrings.OutputIsSyncronous(newOutput.Name));
        }

        #endregion



        #region SetOutputColumnProperty

        [TestMethod()]
        public void TestSetOutputColumnProperty_ChildToChild()
        {
            Microsoft.SqlServer.Dts.Runtime.Package package = new Microsoft.SqlServer.Dts.Runtime.Package();
            Executable exec = package.Executables.Add("STOCK:PipelineTask");
            Microsoft.SqlServer.Dts.Runtime.TaskHost thMainPipe = exec as Microsoft.SqlServer.Dts.Runtime.TaskHost;
            MainPipe dataFlowTask = thMainPipe.InnerObject as MainPipe;
            ComponentEventHandler events = new ComponentEventHandler();
            dataFlowTask.Events = DtsConvert.GetExtendedInterface(events as IDTSComponentEvents);

            // Create a flat file source
            ConnectionManager flatFileConnectionManager = package.Connections.Add("FLATFILE");
            flatFileConnectionManager.Properties["Format"].SetValue(flatFileConnectionManager, "Delimited");
            flatFileConnectionManager.Properties["Name"].SetValue(flatFileConnectionManager, "Flat File Connection");
            flatFileConnectionManager.Properties["ConnectionString"].SetValue(flatFileConnectionManager, @".\SampleSourceFile.txt");
            flatFileConnectionManager.Properties["ColumnNamesInFirstDataRow"].SetValue(flatFileConnectionManager, false);
            flatFileConnectionManager.Properties["HeaderRowDelimiter"].SetValue(flatFileConnectionManager, "\r\n");
            flatFileConnectionManager.Properties["TextQualifier"].SetValue(flatFileConnectionManager, "\"");
            flatFileConnectionManager.Properties["DataRowsToSkip"].SetValue(flatFileConnectionManager, 0);
            flatFileConnectionManager.Properties["Unicode"].SetValue(flatFileConnectionManager, false);
            flatFileConnectionManager.Properties["CodePage"].SetValue(flatFileConnectionManager, 1252);

            // Create the columns in the flat file
            IDTSConnectionManagerFlatFile100 flatFileConnection = flatFileConnectionManager.InnerObject as IDTSConnectionManagerFlatFile100;
            IDTSConnectionManagerFlatFileColumn100 rowTypeColumn = flatFileConnection.Columns.Add();
            rowTypeColumn.ColumnDelimiter = "|";
            rowTypeColumn.ColumnType = "Delimited";
            rowTypeColumn.DataType = DataType.DT_STR;
            rowTypeColumn.DataPrecision = 0;
            rowTypeColumn.DataScale = 0;
            rowTypeColumn.MaximumWidth = 10;
            ((IDTSName100)rowTypeColumn).Name = "rowType";
            IDTSConnectionManagerFlatFileColumn100 dataColumn = flatFileConnection.Columns.Add();
            dataColumn.ColumnDelimiter = "\r\n";
            dataColumn.ColumnType = "Delimited";
            dataColumn.DataType = DataType.DT_TEXT;
            dataColumn.DataPrecision = 0;
            dataColumn.DataScale = 0;
            ((IDTSName100)dataColumn).Name = "Data";


            IDTSComponentMetaData100 textFileSplitter = dataFlowTask.ComponentMetaDataCollection.New();
            textFileSplitter.Name = "Row Splitter Test";
            textFileSplitter.ComponentClassID = typeof(Martin.SQLServer.Dts.TextFileSplitter).AssemblyQualifiedName;
            CManagedComponentWrapper instance = textFileSplitter.Instantiate();

            instance.ProvideComponentProperties();

            textFileSplitter.RuntimeConnectionCollection[0].ConnectionManager = DtsConvert.GetExtendedInterface(flatFileConnectionManager);
            textFileSplitter.RuntimeConnectionCollection[0].ConnectionManagerID = flatFileConnectionManager.ID;
            instance.AcquireConnections(null);
            instance.ReinitializeMetaData();
            instance.ReleaseConnections();

            IDTSOutput100 passthroughOutput = textFileSplitter.OutputCollection[0];
            instance.SetOutputColumnProperty(passthroughOutput.ID, passthroughOutput.OutputColumnCollection[0].ID, ManageProperties.usageOfColumn, Utilities.usageOfColumnEnum.RowType);
            instance.SetOutputColumnProperty(passthroughOutput.ID, passthroughOutput.OutputColumnCollection[1].ID, ManageProperties.usageOfColumn, Utilities.usageOfColumnEnum.RowData);

            IDTSOutput100 errorOutput = textFileSplitter.OutputCollection[1];
            IDTSOutput100 keyOutput = textFileSplitter.OutputCollection[2];
            int keyID = keyOutput.ID;
            IDTSOutput100 numberOfRowsOutput = textFileSplitter.OutputCollection[3];

            // Setup keyOutput with 3 columns (2 as keys)

            IDTSOutputColumn100 keyColumn1 = instance.InsertOutputColumnAt(keyID, 0, "KeyColumn1", String.Empty);
            IDTSOutputColumn100 keyColumn2 = instance.InsertOutputColumnAt(keyID, 1, "KeyColumn2", String.Empty);
            IDTSOutputColumn100 keyColumn3 = instance.InsertOutputColumnAt(keyID, 2, "KeyColumn3", String.Empty);

            // Add a master Output with 4 columns, 1 as Master
            IDTSOutput100 masterOutput = instance.InsertOutput(DTSInsertPlacement.IP_AFTER, numberOfRowsOutput.ID);
            masterOutput.Name = "masterOutput";
            int masterID = masterOutput.ID;
            instance.SetOutputProperty(masterID, ManageProperties.typeOfOutput, Utilities.typeOfOutputEnum.MasterRecord);
            IDTSOutputColumn100 masterColumn1 = instance.InsertOutputColumnAt(masterID, 1, "MasterColumn1", String.Empty);
            IDTSOutputColumn100 masterColumn2 = instance.InsertOutputColumnAt(masterID, 2, "MasterColumn2", String.Empty);
            IDTSOutputColumn100 masterColumn3 = instance.InsertOutputColumnAt(masterID, 3, "MasterColumn3", String.Empty);
            IDTSOutputColumn100 masterColumn4 = instance.InsertOutputColumnAt(masterID, 4, "MasterColumn4", String.Empty);
            instance.SetOutputColumnProperty(masterID, masterColumn2.ID, ManageProperties.usageOfColumn, Utilities.usageOfColumnEnum.MasterValue);

            // Add a child Output with 2 columns.
            IDTSOutput100 childOutput = instance.InsertOutput(DTSInsertPlacement.IP_AFTER, masterOutput.ID);
            childOutput.Name = "childOutput";
            int childID = childOutput.ID;
            instance.SetOutputProperty(childID, ManageProperties.typeOfOutput, Utilities.typeOfOutputEnum.ChildRecord);
            instance.SetOutputProperty(childID, ManageProperties.masterRecordID, masterID);
            IDTSOutputColumn100 childColumn1 = instance.InsertOutputColumnAt(childID, 2, "ChildColumn1", String.Empty);
            IDTSOutputColumn100 childColumn2 = instance.InsertOutputColumnAt(childID, 3, "ChildColumn2", String.Empty);

            // Make sure that the output is correct before resetting Master Record ID
            Assert.AreEqual(DTSValidationStatus.VS_ISVALID, instance.Validate(), "Validation Failed");
            Assert.AreEqual(4, childOutput.OutputColumnCollection.Count, "Incorrect Number of Columns");

            instance.SetOutputProperty(childID, ManageProperties.masterRecordID, masterID);

            Assert.AreEqual(DTSValidationStatus.VS_ISVALID, instance.Validate(), "Second Validation Failed");
            Assert.AreEqual(4, childOutput.OutputColumnCollection.Count, "Incorrect Number of Columns");
            List<String> columnNames = new List<string>();
            foreach (IDTSOutputColumn100 childColumn in childOutput.OutputColumnCollection)
            {
                columnNames.Add(childColumn.Name);
            }
            Assert.IsTrue(columnNames.Contains("MasterColumn2"), "MasterColumn2");
            Assert.IsTrue(columnNames.Contains("ChildColumn1"), "ChildColumn1");
            Assert.IsTrue(columnNames.Contains("ChildColumn2"), "ChildColumn2");
        }


        [TestMethod()]
        public void TestSetOutputColumnProperty_ChildToData()
        {
            Microsoft.SqlServer.Dts.Runtime.Package package = new Microsoft.SqlServer.Dts.Runtime.Package();
            Executable exec = package.Executables.Add("STOCK:PipelineTask");
            Microsoft.SqlServer.Dts.Runtime.TaskHost thMainPipe = exec as Microsoft.SqlServer.Dts.Runtime.TaskHost;
            MainPipe dataFlowTask = thMainPipe.InnerObject as MainPipe;
            ComponentEventHandler events = new ComponentEventHandler();
            dataFlowTask.Events = DtsConvert.GetExtendedInterface(events as IDTSComponentEvents);

            // Create a flat file source
            ConnectionManager flatFileConnectionManager = package.Connections.Add("FLATFILE");
            flatFileConnectionManager.Properties["Format"].SetValue(flatFileConnectionManager, "Delimited");
            flatFileConnectionManager.Properties["Name"].SetValue(flatFileConnectionManager, "Flat File Connection");
            flatFileConnectionManager.Properties["ConnectionString"].SetValue(flatFileConnectionManager, @".\SampleSourceFile.txt");
            flatFileConnectionManager.Properties["ColumnNamesInFirstDataRow"].SetValue(flatFileConnectionManager, false);
            flatFileConnectionManager.Properties["HeaderRowDelimiter"].SetValue(flatFileConnectionManager, "\r\n");
            flatFileConnectionManager.Properties["TextQualifier"].SetValue(flatFileConnectionManager, "\"");
            flatFileConnectionManager.Properties["DataRowsToSkip"].SetValue(flatFileConnectionManager, 0);
            flatFileConnectionManager.Properties["Unicode"].SetValue(flatFileConnectionManager, false);
            flatFileConnectionManager.Properties["CodePage"].SetValue(flatFileConnectionManager, 1252);

            // Create the columns in the flat file
            IDTSConnectionManagerFlatFile100 flatFileConnection = flatFileConnectionManager.InnerObject as IDTSConnectionManagerFlatFile100;
            IDTSConnectionManagerFlatFileColumn100 rowTypeColumn = flatFileConnection.Columns.Add();
            rowTypeColumn.ColumnDelimiter = "|";
            rowTypeColumn.ColumnType = "Delimited";
            rowTypeColumn.DataType = DataType.DT_STR;
            rowTypeColumn.DataPrecision = 0;
            rowTypeColumn.DataScale = 0;
            rowTypeColumn.MaximumWidth = 10;
            ((IDTSName100)rowTypeColumn).Name = "rowType";
            IDTSConnectionManagerFlatFileColumn100 dataColumn = flatFileConnection.Columns.Add();
            dataColumn.ColumnDelimiter = "\r\n";
            dataColumn.ColumnType = "Delimited";
            dataColumn.DataType = DataType.DT_TEXT;
            dataColumn.DataPrecision = 0;
            dataColumn.DataScale = 0;
            ((IDTSName100)dataColumn).Name = "Data";


            IDTSComponentMetaData100 textFileSplitter = dataFlowTask.ComponentMetaDataCollection.New();
            textFileSplitter.Name = "Row Splitter Test";
            textFileSplitter.ComponentClassID = typeof(Martin.SQLServer.Dts.TextFileSplitter).AssemblyQualifiedName;
            CManagedComponentWrapper instance = textFileSplitter.Instantiate();

            instance.ProvideComponentProperties();

            textFileSplitter.RuntimeConnectionCollection[0].ConnectionManager = DtsConvert.GetExtendedInterface(flatFileConnectionManager);
            textFileSplitter.RuntimeConnectionCollection[0].ConnectionManagerID = flatFileConnectionManager.ID;
            instance.AcquireConnections(null);
            instance.ReinitializeMetaData();
            instance.ReleaseConnections();

            IDTSOutput100 passthroughOutput = textFileSplitter.OutputCollection[0];
            instance.SetOutputColumnProperty(passthroughOutput.ID, passthroughOutput.OutputColumnCollection[0].ID, ManageProperties.usageOfColumn, Utilities.usageOfColumnEnum.RowType);
            instance.SetOutputColumnProperty(passthroughOutput.ID, passthroughOutput.OutputColumnCollection[1].ID, ManageProperties.usageOfColumn, Utilities.usageOfColumnEnum.RowData);

            IDTSOutput100 errorOutput = textFileSplitter.OutputCollection[1];
            IDTSOutput100 keyOutput = textFileSplitter.OutputCollection[2];
            int keyID = keyOutput.ID;
            IDTSOutput100 numberOfRowsOutput = textFileSplitter.OutputCollection[3];

            // Setup keyOutput with 3 columns (2 as keys)

            IDTSOutputColumn100 keyColumn1 = instance.InsertOutputColumnAt(keyID, 0, "KeyColumn1", String.Empty);
            IDTSOutputColumn100 keyColumn2 = instance.InsertOutputColumnAt(keyID, 1, "KeyColumn2", String.Empty);
            IDTSOutputColumn100 keyColumn3 = instance.InsertOutputColumnAt(keyID, 2, "KeyColumn3", String.Empty);

            // Add a master Output with 4 columns, 1 as Master
            IDTSOutput100 masterOutput = instance.InsertOutput(DTSInsertPlacement.IP_AFTER, numberOfRowsOutput.ID);
            masterOutput.Name = "masterOutput";
            int masterID = masterOutput.ID;
            instance.SetOutputProperty(masterID, ManageProperties.typeOfOutput, Utilities.typeOfOutputEnum.MasterRecord);
            IDTSOutputColumn100 masterColumn1 = instance.InsertOutputColumnAt(masterID, 1, "MasterColumn1", String.Empty);
            IDTSOutputColumn100 masterColumn2 = instance.InsertOutputColumnAt(masterID, 2, "MasterColumn2", String.Empty);
            IDTSOutputColumn100 masterColumn3 = instance.InsertOutputColumnAt(masterID, 3, "MasterColumn3", String.Empty);
            IDTSOutputColumn100 masterColumn4 = instance.InsertOutputColumnAt(masterID, 4, "MasterColumn4", String.Empty);
            instance.SetOutputColumnProperty(masterID, masterColumn2.ID, ManageProperties.usageOfColumn, Utilities.usageOfColumnEnum.MasterValue);

            // Add a child Output with 2 columns.
            IDTSOutput100 childOutput = instance.InsertOutput(DTSInsertPlacement.IP_AFTER, masterOutput.ID);
            childOutput.Name = "childOutput";
            int childID = childOutput.ID;
            instance.SetOutputProperty(childID, ManageProperties.typeOfOutput, Utilities.typeOfOutputEnum.ChildRecord);
            instance.SetOutputProperty(childID, ManageProperties.masterRecordID, masterID);
            IDTSOutputColumn100 childColumn1 = instance.InsertOutputColumnAt(childID, 2, "ChildColumn1", String.Empty);
            IDTSOutputColumn100 childColumn2 = instance.InsertOutputColumnAt(childID, 3, "ChildColumn2", String.Empty);

            // Make sure that the output is correct before resetting Master Record ID
            Assert.AreEqual(DTSValidationStatus.VS_ISVALID, instance.Validate(), "Validation Failed");
            Assert.AreEqual(4, childOutput.OutputColumnCollection.Count, "Incorrect Number of Columns");

            instance.SetOutputProperty(childID, ManageProperties.typeOfOutput, Utilities.typeOfOutputEnum.DataRecords);

            Assert.AreEqual(DTSValidationStatus.VS_ISVALID, instance.Validate(), "Second Validation Failed");
            Assert.AreEqual(3, childOutput.OutputColumnCollection.Count, "Incorrect Number of Columns");
            List<String> columnNames = new List<string>();
            foreach (IDTSOutputColumn100 childColumn in childOutput.OutputColumnCollection)
            {
                columnNames.Add(childColumn.Name);
            }
            Assert.IsTrue(columnNames.Contains("ChildColumn1"), "ChildColumn1");
            Assert.IsTrue(columnNames.Contains("ChildColumn2"), "ChildColumn2");
        }

        [TestMethod()]
        public void TestSetOutputColumnProperty_MasterToData_OneChild()
        {
            Microsoft.SqlServer.Dts.Runtime.Package package = new Microsoft.SqlServer.Dts.Runtime.Package();
            Executable exec = package.Executables.Add("STOCK:PipelineTask");
            Microsoft.SqlServer.Dts.Runtime.TaskHost thMainPipe = exec as Microsoft.SqlServer.Dts.Runtime.TaskHost;
            MainPipe dataFlowTask = thMainPipe.InnerObject as MainPipe;
            ComponentEventHandler events = new ComponentEventHandler();
            dataFlowTask.Events = DtsConvert.GetExtendedInterface(events as IDTSComponentEvents);

            // Create a flat file source
            ConnectionManager flatFileConnectionManager = package.Connections.Add("FLATFILE");
            flatFileConnectionManager.Properties["Format"].SetValue(flatFileConnectionManager, "Delimited");
            flatFileConnectionManager.Properties["Name"].SetValue(flatFileConnectionManager, "Flat File Connection");
            flatFileConnectionManager.Properties["ConnectionString"].SetValue(flatFileConnectionManager, @".\SampleSourceFile.txt");
            flatFileConnectionManager.Properties["ColumnNamesInFirstDataRow"].SetValue(flatFileConnectionManager, false);
            flatFileConnectionManager.Properties["HeaderRowDelimiter"].SetValue(flatFileConnectionManager, "\r\n");
            flatFileConnectionManager.Properties["TextQualifier"].SetValue(flatFileConnectionManager, "\"");
            flatFileConnectionManager.Properties["DataRowsToSkip"].SetValue(flatFileConnectionManager, 0);
            flatFileConnectionManager.Properties["Unicode"].SetValue(flatFileConnectionManager, false);
            flatFileConnectionManager.Properties["CodePage"].SetValue(flatFileConnectionManager, 1252);

            // Create the columns in the flat file
            IDTSConnectionManagerFlatFile100 flatFileConnection = flatFileConnectionManager.InnerObject as IDTSConnectionManagerFlatFile100;
            IDTSConnectionManagerFlatFileColumn100 rowTypeColumn = flatFileConnection.Columns.Add();
            rowTypeColumn.ColumnDelimiter = "|";
            rowTypeColumn.ColumnType = "Delimited";
            rowTypeColumn.DataType = DataType.DT_STR;
            rowTypeColumn.DataPrecision = 0;
            rowTypeColumn.DataScale = 0;
            rowTypeColumn.MaximumWidth = 10;
            ((IDTSName100)rowTypeColumn).Name = "rowType";
            IDTSConnectionManagerFlatFileColumn100 dataColumn = flatFileConnection.Columns.Add();
            dataColumn.ColumnDelimiter = "\r\n";
            dataColumn.ColumnType = "Delimited";
            dataColumn.DataType = DataType.DT_TEXT;
            dataColumn.DataPrecision = 0;
            dataColumn.DataScale = 0;
            ((IDTSName100)dataColumn).Name = "Data";


            IDTSComponentMetaData100 textFileSplitter = dataFlowTask.ComponentMetaDataCollection.New();
            textFileSplitter.Name = "Row Splitter Test";
            textFileSplitter.ComponentClassID = typeof(Martin.SQLServer.Dts.TextFileSplitter).AssemblyQualifiedName;
            CManagedComponentWrapper instance = textFileSplitter.Instantiate();

            instance.ProvideComponentProperties();

            textFileSplitter.RuntimeConnectionCollection[0].ConnectionManager = DtsConvert.GetExtendedInterface(flatFileConnectionManager);
            textFileSplitter.RuntimeConnectionCollection[0].ConnectionManagerID = flatFileConnectionManager.ID;
            instance.AcquireConnections(null);
            instance.ReinitializeMetaData();
            instance.ReleaseConnections();

            IDTSOutput100 passthroughOutput = textFileSplitter.OutputCollection[0];
            instance.SetOutputColumnProperty(passthroughOutput.ID, passthroughOutput.OutputColumnCollection[0].ID, ManageProperties.usageOfColumn, Utilities.usageOfColumnEnum.RowType);
            instance.SetOutputColumnProperty(passthroughOutput.ID, passthroughOutput.OutputColumnCollection[1].ID, ManageProperties.usageOfColumn, Utilities.usageOfColumnEnum.RowData);

            IDTSOutput100 errorOutput = textFileSplitter.OutputCollection[1];
            IDTSOutput100 keyOutput = textFileSplitter.OutputCollection[2];
            int keyID = keyOutput.ID;
            IDTSOutput100 numberOfRowsOutput = textFileSplitter.OutputCollection[3];

            // Setup keyOutput with 3 columns (2 as keys)

            IDTSOutputColumn100 keyColumn1 = instance.InsertOutputColumnAt(keyID, 0, "KeyColumn1", String.Empty);
            IDTSOutputColumn100 keyColumn2 = instance.InsertOutputColumnAt(keyID, 1, "KeyColumn2", String.Empty);
            IDTSOutputColumn100 keyColumn3 = instance.InsertOutputColumnAt(keyID, 2, "KeyColumn3", String.Empty);

            // Add a master Output with 4 columns, 1 as Master
            IDTSOutput100 masterOutput = instance.InsertOutput(DTSInsertPlacement.IP_AFTER, numberOfRowsOutput.ID);
            masterOutput.Name = "masterOutput";
            int masterID = masterOutput.ID;
            instance.SetOutputProperty(masterID, ManageProperties.typeOfOutput, Utilities.typeOfOutputEnum.MasterRecord);
            IDTSOutputColumn100 masterColumn1 = instance.InsertOutputColumnAt(masterID, 1, "MasterColumn1", String.Empty);
            IDTSOutputColumn100 masterColumn2 = instance.InsertOutputColumnAt(masterID, 2, "MasterColumn2", String.Empty);
            IDTSOutputColumn100 masterColumn3 = instance.InsertOutputColumnAt(masterID, 3, "MasterColumn3", String.Empty);
            IDTSOutputColumn100 masterColumn4 = instance.InsertOutputColumnAt(masterID, 4, "MasterColumn4", String.Empty);
            instance.SetOutputColumnProperty(masterID, masterColumn2.ID, ManageProperties.usageOfColumn, Utilities.usageOfColumnEnum.MasterValue);

            // Add a child Output with 2 columns.
            IDTSOutput100 childOutput = instance.InsertOutput(DTSInsertPlacement.IP_AFTER, masterOutput.ID);
            childOutput.Name = "childOutput";
            int childID = childOutput.ID;
            instance.SetOutputProperty(childID, ManageProperties.typeOfOutput, Utilities.typeOfOutputEnum.ChildRecord);
            instance.SetOutputProperty(childID, ManageProperties.masterRecordID, masterID);
            IDTSOutputColumn100 childColumn1 = instance.InsertOutputColumnAt(childID, 2, "ChildColumn1", String.Empty);
            IDTSOutputColumn100 childColumn2 = instance.InsertOutputColumnAt(childID, 3, "ChildColumn2", String.Empty);

            // Make sure that the output is correct before resetting Master Record ID
            Assert.AreEqual(DTSValidationStatus.VS_ISVALID, instance.Validate(), "Validation Failed");
            Assert.AreEqual(4, childOutput.OutputColumnCollection.Count, "Incorrect Number of Columns");

            bool exceptionThrown = false;
            try
            {
                // Switch the Master to a Data Record
                instance.SetOutputProperty(masterID, ManageProperties.typeOfOutput, Utilities.typeOfOutputEnum.DataRecords);
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                exceptionThrown = true;
                Assert.AreEqual(MessageStrings.ThereAreChildRecordsForMaster(masterOutput.Name), ex.Message);
            }

            Assert.IsTrue(exceptionThrown, "Exception was NOT thrown!");
        }


        [TestMethod()]
        public void TestSetOutputColumnProperty_MasterToData_ChildMaster()
        {
            Microsoft.SqlServer.Dts.Runtime.Package package = new Microsoft.SqlServer.Dts.Runtime.Package();
            Executable exec = package.Executables.Add("STOCK:PipelineTask");
            Microsoft.SqlServer.Dts.Runtime.TaskHost thMainPipe = exec as Microsoft.SqlServer.Dts.Runtime.TaskHost;
            MainPipe dataFlowTask = thMainPipe.InnerObject as MainPipe;
            ComponentEventHandler events = new ComponentEventHandler();
            dataFlowTask.Events = DtsConvert.GetExtendedInterface(events as IDTSComponentEvents);

            // Create a flat file source
            ConnectionManager flatFileConnectionManager = package.Connections.Add("FLATFILE");
            flatFileConnectionManager.Properties["Format"].SetValue(flatFileConnectionManager, "Delimited");
            flatFileConnectionManager.Properties["Name"].SetValue(flatFileConnectionManager, "Flat File Connection");
            flatFileConnectionManager.Properties["ConnectionString"].SetValue(flatFileConnectionManager, @".\SampleSourceFile.txt");
            flatFileConnectionManager.Properties["ColumnNamesInFirstDataRow"].SetValue(flatFileConnectionManager, false);
            flatFileConnectionManager.Properties["HeaderRowDelimiter"].SetValue(flatFileConnectionManager, "\r\n");
            flatFileConnectionManager.Properties["TextQualifier"].SetValue(flatFileConnectionManager, "\"");
            flatFileConnectionManager.Properties["DataRowsToSkip"].SetValue(flatFileConnectionManager, 0);
            flatFileConnectionManager.Properties["Unicode"].SetValue(flatFileConnectionManager, false);
            flatFileConnectionManager.Properties["CodePage"].SetValue(flatFileConnectionManager, 1252);

            // Create the columns in the flat file
            IDTSConnectionManagerFlatFile100 flatFileConnection = flatFileConnectionManager.InnerObject as IDTSConnectionManagerFlatFile100;
            IDTSConnectionManagerFlatFileColumn100 rowTypeColumn = flatFileConnection.Columns.Add();
            rowTypeColumn.ColumnDelimiter = "|";
            rowTypeColumn.ColumnType = "Delimited";
            rowTypeColumn.DataType = DataType.DT_STR;
            rowTypeColumn.DataPrecision = 0;
            rowTypeColumn.DataScale = 0;
            rowTypeColumn.MaximumWidth = 10;
            ((IDTSName100)rowTypeColumn).Name = "rowType";
            IDTSConnectionManagerFlatFileColumn100 dataColumn = flatFileConnection.Columns.Add();
            dataColumn.ColumnDelimiter = "\r\n";
            dataColumn.ColumnType = "Delimited";
            dataColumn.DataType = DataType.DT_TEXT;
            dataColumn.DataPrecision = 0;
            dataColumn.DataScale = 0;
            ((IDTSName100)dataColumn).Name = "Data";


            IDTSComponentMetaData100 textFileSplitter = dataFlowTask.ComponentMetaDataCollection.New();
            textFileSplitter.Name = "Row Splitter Test";
            textFileSplitter.ComponentClassID = typeof(Martin.SQLServer.Dts.TextFileSplitter).AssemblyQualifiedName;
            CManagedComponentWrapper instance = textFileSplitter.Instantiate();

            instance.ProvideComponentProperties();

            textFileSplitter.RuntimeConnectionCollection[0].ConnectionManager = DtsConvert.GetExtendedInterface(flatFileConnectionManager);
            textFileSplitter.RuntimeConnectionCollection[0].ConnectionManagerID = flatFileConnectionManager.ID;
            instance.AcquireConnections(null);
            instance.ReinitializeMetaData();
            instance.ReleaseConnections();

            IDTSOutput100 passthroughOutput = textFileSplitter.OutputCollection[0];
            instance.SetOutputColumnProperty(passthroughOutput.ID, passthroughOutput.OutputColumnCollection[0].ID, ManageProperties.usageOfColumn, Utilities.usageOfColumnEnum.RowType);
            instance.SetOutputColumnProperty(passthroughOutput.ID, passthroughOutput.OutputColumnCollection[1].ID, ManageProperties.usageOfColumn, Utilities.usageOfColumnEnum.RowData);

            IDTSOutput100 errorOutput = textFileSplitter.OutputCollection[1];
            IDTSOutput100 keyOutput = textFileSplitter.OutputCollection[2];
            int keyID = keyOutput.ID;
            IDTSOutput100 numberOfRowsOutput = textFileSplitter.OutputCollection[3];

            // Setup keyOutput with 3 columns (2 as keys)

            IDTSOutputColumn100 keyColumn1 = instance.InsertOutputColumnAt(keyID, 0, "KeyColumn1", String.Empty);
            IDTSOutputColumn100 keyColumn2 = instance.InsertOutputColumnAt(keyID, 1, "KeyColumn2", String.Empty);
            IDTSOutputColumn100 keyColumn3 = instance.InsertOutputColumnAt(keyID, 2, "KeyColumn3", String.Empty);

            // Add a master Output with 4 columns, 1 as Master
            IDTSOutput100 masterOutput = instance.InsertOutput(DTSInsertPlacement.IP_AFTER, numberOfRowsOutput.ID);
            masterOutput.Name = "masterOutput";
            int masterID = masterOutput.ID;
            instance.SetOutputProperty(masterID, ManageProperties.typeOfOutput, Utilities.typeOfOutputEnum.MasterRecord);
            IDTSOutputColumn100 masterColumn1 = instance.InsertOutputColumnAt(masterID, 1, "MasterColumn1", String.Empty);
            IDTSOutputColumn100 masterColumn2 = instance.InsertOutputColumnAt(masterID, 2, "MasterColumn2", String.Empty);
            IDTSOutputColumn100 masterColumn3 = instance.InsertOutputColumnAt(masterID, 3, "MasterColumn3", String.Empty);
            IDTSOutputColumn100 masterColumn4 = instance.InsertOutputColumnAt(masterID, 4, "MasterColumn4", String.Empty);
            instance.SetOutputColumnProperty(masterID, masterColumn2.ID, ManageProperties.usageOfColumn, Utilities.usageOfColumnEnum.MasterValue);

            // Add a masterChild Output with 3 columns.
            IDTSOutput100 masterChildOutput = instance.InsertOutput(DTSInsertPlacement.IP_AFTER, masterID);
            masterChildOutput.Name = "masterChildOutput";
            int masterChildID = masterChildOutput.ID;
            instance.SetOutputProperty(masterChildID, ManageProperties.typeOfOutput, Utilities.typeOfOutputEnum.ChildMasterRecord);
            instance.SetOutputProperty(masterChildID, ManageProperties.masterRecordID, masterID);
            IDTSOutputColumn100 childColumn1 = instance.InsertOutputColumnAt(masterChildID, 2, "ChildMasterColumn1", String.Empty);
            childColumn1 = instance.InsertOutputColumnAt(masterChildID, 3, "ChildMasterColumn2", String.Empty);
            childColumn1 = instance.InsertOutputColumnAt(masterChildID, 4, "ChildMasterColumn3", String.Empty);
            instance.SetOutputColumnProperty(masterChildID, childColumn1.ID, ManageProperties.usageOfColumn, Utilities.usageOfColumnEnum.MasterValue);

            // Add a child Output with 2 columns.
            IDTSOutput100 childOutput2 = instance.InsertOutput(DTSInsertPlacement.IP_AFTER, masterOutput.ID);
            childOutput2.Name = "childOutput";
            int childID2 = childOutput2.ID;
            instance.SetOutputProperty(childID2, ManageProperties.typeOfOutput, Utilities.typeOfOutputEnum.ChildRecord);
            instance.SetOutputProperty(childID2, ManageProperties.masterRecordID, masterChildID);
            IDTSOutputColumn100 childColumn2 = instance.InsertOutputColumnAt(childID2, 3, "ChildColumn1", String.Empty);
            childColumn2 = instance.InsertOutputColumnAt(childID2, 4, "ChildColumn2", String.Empty);


            // Make sure that the output is correct before resetting Master Record ID
            Assert.AreEqual(DTSValidationStatus.VS_ISVALID, instance.Validate(), "Validation Failed");
            Assert.AreEqual(5, childOutput2.OutputColumnCollection.Count, "Incorrect Number of Columns in ChildOutput2");

            bool exceptionThrown = false;
            try
            {
                // Switch the Master to a Data Record
                instance.SetOutputProperty(masterID, ManageProperties.typeOfOutput, Utilities.typeOfOutputEnum.DataRecords);
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                exceptionThrown = true;
                Assert.AreEqual(MessageStrings.ThereAreChildRecordsForMaster(masterOutput.Name), ex.Message);
            }

            Assert.IsTrue(exceptionThrown, "Exception was NOT thrown!");
            Assert.IsTrue(events.errorMessages.Contains(String.Format("[Error] Text File Splitter Source: {0}", MessageStrings.MasterHasChild(masterOutput.Name, masterChildOutput.Name))), "Error Message Missing");
        }

        [TestMethod()]
        public void TestSetOutputColumnProperty_MasterToData_NoChildren()
        {
            Microsoft.SqlServer.Dts.Runtime.Package package = new Microsoft.SqlServer.Dts.Runtime.Package();
            Executable exec = package.Executables.Add("STOCK:PipelineTask");
            Microsoft.SqlServer.Dts.Runtime.TaskHost thMainPipe = exec as Microsoft.SqlServer.Dts.Runtime.TaskHost;
            MainPipe dataFlowTask = thMainPipe.InnerObject as MainPipe;
            ComponentEventHandler events = new ComponentEventHandler();
            dataFlowTask.Events = DtsConvert.GetExtendedInterface(events as IDTSComponentEvents);

            // Create a flat file source
            ConnectionManager flatFileConnectionManager = package.Connections.Add("FLATFILE");
            flatFileConnectionManager.Properties["Format"].SetValue(flatFileConnectionManager, "Delimited");
            flatFileConnectionManager.Properties["Name"].SetValue(flatFileConnectionManager, "Flat File Connection");
            flatFileConnectionManager.Properties["ConnectionString"].SetValue(flatFileConnectionManager, @".\SampleSourceFile.txt");
            flatFileConnectionManager.Properties["ColumnNamesInFirstDataRow"].SetValue(flatFileConnectionManager, false);
            flatFileConnectionManager.Properties["HeaderRowDelimiter"].SetValue(flatFileConnectionManager, "\r\n");
            flatFileConnectionManager.Properties["TextQualifier"].SetValue(flatFileConnectionManager, "\"");
            flatFileConnectionManager.Properties["DataRowsToSkip"].SetValue(flatFileConnectionManager, 0);
            flatFileConnectionManager.Properties["Unicode"].SetValue(flatFileConnectionManager, false);
            flatFileConnectionManager.Properties["CodePage"].SetValue(flatFileConnectionManager, 1252);

            // Create the columns in the flat file
            IDTSConnectionManagerFlatFile100 flatFileConnection = flatFileConnectionManager.InnerObject as IDTSConnectionManagerFlatFile100;
            IDTSConnectionManagerFlatFileColumn100 rowTypeColumn = flatFileConnection.Columns.Add();
            rowTypeColumn.ColumnDelimiter = "|";
            rowTypeColumn.ColumnType = "Delimited";
            rowTypeColumn.DataType = DataType.DT_STR;
            rowTypeColumn.DataPrecision = 0;
            rowTypeColumn.DataScale = 0;
            rowTypeColumn.MaximumWidth = 10;
            ((IDTSName100)rowTypeColumn).Name = "rowType";
            IDTSConnectionManagerFlatFileColumn100 dataColumn = flatFileConnection.Columns.Add();
            dataColumn.ColumnDelimiter = "\r\n";
            dataColumn.ColumnType = "Delimited";
            dataColumn.DataType = DataType.DT_TEXT;
            dataColumn.DataPrecision = 0;
            dataColumn.DataScale = 0;
            ((IDTSName100)dataColumn).Name = "Data";


            IDTSComponentMetaData100 textFileSplitter = dataFlowTask.ComponentMetaDataCollection.New();
            textFileSplitter.Name = "Row Splitter Test";
            textFileSplitter.ComponentClassID = typeof(Martin.SQLServer.Dts.TextFileSplitter).AssemblyQualifiedName;
            CManagedComponentWrapper instance = textFileSplitter.Instantiate();

            instance.ProvideComponentProperties();

            textFileSplitter.RuntimeConnectionCollection[0].ConnectionManager = DtsConvert.GetExtendedInterface(flatFileConnectionManager);
            textFileSplitter.RuntimeConnectionCollection[0].ConnectionManagerID = flatFileConnectionManager.ID;
            instance.AcquireConnections(null);
            instance.ReinitializeMetaData();
            instance.ReleaseConnections();

            IDTSOutput100 passthroughOutput = textFileSplitter.OutputCollection[0];
            instance.SetOutputColumnProperty(passthroughOutput.ID, passthroughOutput.OutputColumnCollection[0].ID, ManageProperties.usageOfColumn, Utilities.usageOfColumnEnum.RowType);
            instance.SetOutputColumnProperty(passthroughOutput.ID, passthroughOutput.OutputColumnCollection[1].ID, ManageProperties.usageOfColumn, Utilities.usageOfColumnEnum.RowData);

            IDTSOutput100 errorOutput = textFileSplitter.OutputCollection[1];
            IDTSOutput100 keyOutput = textFileSplitter.OutputCollection[2];
            int keyID = keyOutput.ID;
            IDTSOutput100 numberOfRowsOutput = textFileSplitter.OutputCollection[3];

            // Setup keyOutput with 3 columns (2 as keys)

            IDTSOutputColumn100 keyColumn1 = instance.InsertOutputColumnAt(keyID, 0, "KeyColumn1", String.Empty);
            IDTSOutputColumn100 keyColumn2 = instance.InsertOutputColumnAt(keyID, 1, "KeyColumn2", String.Empty);
            IDTSOutputColumn100 keyColumn3 = instance.InsertOutputColumnAt(keyID, 2, "KeyColumn3", String.Empty);

            // Add a master Output with 4 columns, 1 as Master
            IDTSOutput100 masterOutput = instance.InsertOutput(DTSInsertPlacement.IP_AFTER, numberOfRowsOutput.ID);
            masterOutput.Name = "masterOutput";
            int masterID = masterOutput.ID;
            instance.SetOutputProperty(masterID, ManageProperties.typeOfOutput, Utilities.typeOfOutputEnum.MasterRecord);
            IDTSOutputColumn100 masterColumn1 = instance.InsertOutputColumnAt(masterID, 1, "MasterColumn1", String.Empty);
            IDTSOutputColumn100 masterColumn2 = instance.InsertOutputColumnAt(masterID, 2, "MasterColumn2", String.Empty);
            IDTSOutputColumn100 masterColumn3 = instance.InsertOutputColumnAt(masterID, 3, "MasterColumn3", String.Empty);
            IDTSOutputColumn100 masterColumn4 = instance.InsertOutputColumnAt(masterID, 4, "MasterColumn4", String.Empty);
            instance.SetOutputColumnProperty(masterID, masterColumn2.ID, ManageProperties.usageOfColumn, Utilities.usageOfColumnEnum.MasterValue);


            // Make sure that the output is correct before resetting Master Record ID
            Assert.AreEqual(DTSValidationStatus.VS_ISVALID, instance.Validate(), "Validation Failed");

            instance.SetOutputProperty(masterID, ManageProperties.typeOfOutput, Utilities.typeOfOutputEnum.DataRecords);

            Assert.AreEqual(DTSValidationStatus.VS_ISVALID, instance.Validate(), "Second Validation Failed");
            List<String> columnNames = new List<string>();
            foreach (IDTSOutputColumn100 childColumn in masterOutput.OutputColumnCollection)
            {
                columnNames.Add(childColumn.Name);
            }
            Assert.IsTrue(columnNames.Contains("MasterColumn1"), "MasterColumn1");
            Assert.IsTrue(columnNames.Contains("MasterColumn2"), "MasterColumn2");
            Assert.IsTrue(columnNames.Contains("MasterColumn3"), "MasterColumn3");
            Assert.IsTrue(columnNames.Contains("MasterColumn4"), "MasterColumn4");
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
