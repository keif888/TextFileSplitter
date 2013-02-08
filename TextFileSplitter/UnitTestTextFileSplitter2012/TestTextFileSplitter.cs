using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Martin.SQLServer.Dts;
using Microsoft.SqlServer.Dts.Pipeline;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime.Wrapper;
using Microsoft.SqlServer.Dts.Runtime;
using System.Diagnostics;


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

            Assert.AreEqual(3, textFileSplitter.Version);
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

            Assert.AreEqual(3, textFileSplitter.Version);
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
            Assert.AreEqual(3, textFileSplitter.Version, "Version failed to match on reload");
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

            Assert.AreEqual(DTSValidationStatus.VS_ISVALID,  instance.Validate());
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

    }
}
