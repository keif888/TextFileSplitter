using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Martin.SQLServer.Dts;
using Microsoft.SqlServer.Dts.Pipeline;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime.Wrapper;
using Microsoft.SqlServer.Dts.Runtime;
using System.Collections.Generic;

using System.Data.SqlServerCe;
using System.IO;
using System.Data;
using System.Diagnostics;


namespace UnitTestTextFileSplitter2012
{
    [TestClass]
    public class TestPrimeOutput
    {

        const string sqlCEDatabaseName = @".\TestPrimeOutput.sdf";
        const string sqlCEPassword = "MartinSource";
        SqlCeEngine sqlCEEngine = null;

        private static string connectionString()
        {
            return String.Format("DataSource=\"{0}\"; Password='{1}'", sqlCEDatabaseName, sqlCEPassword);
        }

        [TestInitialize]
        public void SetupSQLCEDatabase()
        {
            if (File.Exists(sqlCEDatabaseName))
            {
                File.Delete(sqlCEDatabaseName);
            }

            sqlCEEngine = new SqlCeEngine(connectionString());
            sqlCEEngine.CreateDatabase();

            SqlCeConnection connection = new SqlCeConnection(connectionString());
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            String tableCreate = "CREATE TABLE [RowCount] ([KeyValue] nvarchar(255), [NumberOfRows] bigint, [KeyValueStatus] nvarchar(255))";
            SqlCeCommand command = new SqlCeCommand(tableCreate, connection);
            command.ExecuteNonQuery();

            tableCreate = "CREATE TABLE [KeyRecords] ([KeyColumn1] uniqueidentifier, [KeyColumn2] integer, [KeyColumn3] nvarchar(255))";
            command.CommandText = tableCreate;
            command.ExecuteNonQuery();

            connection.Close();
            sqlCEEngine.Dispose();
        }

        //[TestCleanup]
        //public void CleanupSQLCEDatabase()
        //{
        //    if (File.Exists(sqlCEDatabaseName))
        //    {
        //        File.Delete(sqlCEDatabaseName);
        //    }
        //}


        [TestMethod]
        public void TestPrimeOutput_KeyRecordsOnly()
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
            flatFileConnectionManager.Properties["ConnectionString"].SetValue(flatFileConnectionManager, @".\KeyRecordsOnly.txt");
            flatFileConnectionManager.Properties["ColumnNamesInFirstDataRow"].SetValue(flatFileConnectionManager, false);
            flatFileConnectionManager.Properties["HeaderRowDelimiter"].SetValue(flatFileConnectionManager, "\r\n");
            flatFileConnectionManager.Properties["TextQualifier"].SetValue(flatFileConnectionManager, "\"");
            flatFileConnectionManager.Properties["DataRowsToSkip"].SetValue(flatFileConnectionManager, 0);
            flatFileConnectionManager.Properties["Unicode"].SetValue(flatFileConnectionManager, false);
            flatFileConnectionManager.Properties["CodePage"].SetValue(flatFileConnectionManager, 1252);

            // Create the columns in the flat file
            IDTSConnectionManagerFlatFile100 flatFileConnection = flatFileConnectionManager.InnerObject as IDTSConnectionManagerFlatFile100;
            IDTSConnectionManagerFlatFileColumn100 rowTypeColumn = flatFileConnection.Columns.Add();
            rowTypeColumn.ColumnDelimiter = ",";
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
            textFileSplitter.ComponentClassID = typeof(Martin.SQLServer.Dts.TextFileSplitter).AssemblyQualifiedName;
            CManagedComponentWrapper instance = textFileSplitter.Instantiate();

            instance.ProvideComponentProperties();
            textFileSplitter.Name = "Row Splitter Test";
            instance.SetComponentProperty(ManageProperties.columnDelimiter, ",");

            textFileSplitter.RuntimeConnectionCollection[0].ConnectionManager = DtsConvert.GetExtendedInterface(flatFileConnectionManager);
            textFileSplitter.RuntimeConnectionCollection[0].ConnectionManagerID = flatFileConnectionManager.ID;
            instance.AcquireConnections(null);
            instance.ReinitializeMetaData();
            instance.ReleaseConnections();

            IDTSOutput100 passthroughOutput = textFileSplitter.OutputCollection[0];
            instance.SetOutputColumnProperty(passthroughOutput.ID, passthroughOutput.OutputColumnCollection[0].ID, ManageProperties.usageOfColumn, Utilities.usageOfColumnEnum.RowType);
            instance.SetOutputColumnProperty(passthroughOutput.ID, passthroughOutput.OutputColumnCollection[1].ID, ManageProperties.usageOfColumn, Utilities.usageOfColumnEnum.RowData);
            passthroughOutput.ErrorRowDisposition = DTSRowDisposition.RD_IgnoreFailure;
            IDTSOutput100 errorOutput = textFileSplitter.OutputCollection[1];
            IDTSOutput100 keyOutput = textFileSplitter.OutputCollection[2];
            int keyID = keyOutput.ID;
            keyOutput.ErrorRowDisposition = DTSRowDisposition.RD_IgnoreFailure;
            IDTSOutput100 numberOfRowsOutput = textFileSplitter.OutputCollection[3];

            // Setup keyOutput with 3 columns (2 as keys)
            instance.SetOutputProperty(keyID, ManageProperties.rowTypeValue, "Key");
            IDTSOutputColumn100 keyColumn1 = instance.InsertOutputColumnAt(keyID, 0, "KeyColumn1", String.Empty);
            IDTSOutputColumn100 keyColumn2 = instance.InsertOutputColumnAt(keyID, 1, "KeyColumn2", String.Empty);
            IDTSOutputColumn100 keyColumn3 = instance.InsertOutputColumnAt(keyID, 2, "KeyColumn3", String.Empty);

            IDTSComponentMetaData100 trashDestination = dataFlowTask.ComponentMetaDataCollection.New();
            trashDestination.ComponentClassID = typeof(Konesans.Dts.Pipeline.TrashDestination.Trash).AssemblyQualifiedName;
            CManagedComponentWrapper trashInstance = trashDestination.Instantiate();
            trashInstance.ProvideComponentProperties();

            IDTSPath100 newPath = dataFlowTask.PathCollection.New();
            newPath.AttachPathAndPropagateNotifications(textFileSplitter.OutputCollection[2], trashDestination.InputCollection[0]);

            PackageEventHandler packageEvents = new PackageEventHandler();

            Microsoft.SqlServer.Dts.Runtime.DTSExecResult result = package.Execute(null, null, packageEvents as IDTSEvents, null, null);
            Assert.AreEqual(Microsoft.SqlServer.Dts.Runtime.DTSExecResult.Success, result, "Execution Failed");
            Assert.IsTrue(packageEvents.eventMessages.Contains("[Information] 0: Row Splitter Test: Total Number of records read is 4."), "record count message missing");
        }



        [TestMethod]
        public void TestPrimeOutput_UnmappedRecords()
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
            rowTypeColumn.ColumnDelimiter = @"|";
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
            textFileSplitter.ComponentClassID = typeof(Martin.SQLServer.Dts.TextFileSplitter).AssemblyQualifiedName;
            CManagedComponentWrapper instance = textFileSplitter.Instantiate();

            instance.ProvideComponentProperties();
            textFileSplitter.Name = "Row Splitter Test";
            instance.SetComponentProperty(ManageProperties.columnDelimiter, @"|");
            textFileSplitter.RuntimeConnectionCollection[0].ConnectionManager = DtsConvert.GetExtendedInterface(flatFileConnectionManager);
            textFileSplitter.RuntimeConnectionCollection[0].ConnectionManagerID = flatFileConnectionManager.ID;
            instance.AcquireConnections(null);
            instance.ReinitializeMetaData();
            instance.ReleaseConnections();

            IDTSOutput100 passthroughOutput = textFileSplitter.OutputCollection[0];
            instance.SetOutputColumnProperty(passthroughOutput.ID, passthroughOutput.OutputColumnCollection[0].ID, ManageProperties.usageOfColumn, Utilities.usageOfColumnEnum.RowType);
            instance.SetOutputColumnProperty(passthroughOutput.ID, passthroughOutput.OutputColumnCollection[1].ID, ManageProperties.usageOfColumn, Utilities.usageOfColumnEnum.RowData);
            passthroughOutput.ErrorRowDisposition = DTSRowDisposition.RD_IgnoreFailure;
            IDTSOutput100 errorOutput = textFileSplitter.OutputCollection[1];
            IDTSOutput100 keyOutput = textFileSplitter.OutputCollection[2];
            int keyID = keyOutput.ID;
            keyOutput.ErrorRowDisposition = DTSRowDisposition.RD_IgnoreFailure;
            IDTSOutput100 numberOfRowsOutput = textFileSplitter.OutputCollection[3];

            // Setup keyOutput with 2 columns
            instance.SetOutputProperty(keyID, ManageProperties.rowTypeValue, "001");
            IDTSOutputColumn100 keyColumn1 = instance.InsertOutputColumnAt(keyID, 1, "KeyColumn2", String.Empty);
            IDTSOutputColumn100 keyColumn2 = instance.InsertOutputColumnAt(keyID, 2, "KeyColumn3", String.Empty);
            keyOutput.OutputColumnCollection[0].Name = "KeyColumn1";

            // Add SQL CE Connection
            ConnectionManager sqlCECM = package.Connections.Add("SQLMOBILE");
            sqlCECM.ConnectionString = connectionString();
            sqlCECM.Name = "SQLCE Destination";

            IDTSComponentMetaData100 sqlCETarget = dataFlowTask.ComponentMetaDataCollection.New();
            sqlCETarget.ComponentClassID = typeof(Microsoft.SqlServer.Dts.Pipeline.SqlCEDestinationAdapter).AssemblyQualifiedName; //"{874F7595-FB5F-40FF-96AF-FBFF8250E3EF}"; // SQL Compact
            CManagedComponentWrapper sqlCEInstance = sqlCETarget.Instantiate();
            sqlCEInstance.ProvideComponentProperties();
            sqlCETarget.Name = "SQLCE Target";
            sqlCETarget.RuntimeConnectionCollection[0].ConnectionManager = DtsConvert.GetExtendedInterface(sqlCECM);
            sqlCETarget.RuntimeConnectionCollection[0].ConnectionManagerID = sqlCECM.ID;

            Debug.WriteLine(String.Format("Count is {0}", sqlCETarget.CustomPropertyCollection.Count));
            foreach (IDTSCustomProperty100 property in sqlCETarget.CustomPropertyCollection)
            {
                Debug.WriteLine(String.Format("Property Name {0} has value {1}", property.Name, property.Value));
            }
            sqlCETarget.CustomPropertyCollection["Table Name"].Value = "KeyRecords";
            sqlCEInstance.AcquireConnections(null);
            sqlCEInstance.ReinitializeMetaData();
            sqlCEInstance.ReleaseConnections();

            // Create the path from source to destination.
            IDTSPath100 path = dataFlowTask.PathCollection.New();
            path.AttachPathAndPropagateNotifications(keyOutput, sqlCETarget.InputCollection[0]);

            // Get the destination's default input and virtual input.
            IDTSInput100 input = sqlCETarget.InputCollection[0];
            IDTSVirtualInput100 vInput = input.GetVirtualInput();

            // Iterate through the virtual input column collection.
            foreach (IDTSVirtualInputColumn100 vColumn in vInput.VirtualInputColumnCollection)
            {
                // Find external column by name
                IDTSExternalMetadataColumn100 externalColumn = null;
                foreach (IDTSExternalMetadataColumn100 column in input.ExternalMetadataColumnCollection)
                {
                    if (String.Compare(column.Name, vColumn.Name, true) == 0)
                    {
                        externalColumn = column;
                        break;
                    }
                }
                if (externalColumn != null)
                {
                    // Select column, and retain new input column
                    IDTSInputColumn100 inputColumn = sqlCEInstance.SetUsageType(input.ID, vInput, vColumn.LineageID, DTSUsageType.UT_READONLY);
                    // Map input column to external column
                    sqlCEInstance.MapInputColumn(input.ID, inputColumn.ID, externalColumn.ID);
                }
            }

            PackageEventHandler packageEvents = new PackageEventHandler();

            Microsoft.SqlServer.Dts.Runtime.DTSExecResult result = package.Execute(null, null, packageEvents as IDTSEvents, null, null);
            foreach (String message in packageEvents.eventMessages)
            {
                Debug.WriteLine(message);
            }
            Assert.AreEqual(Microsoft.SqlServer.Dts.Runtime.DTSExecResult.Success, result, "Execution Failed");
            Assert.IsTrue(packageEvents.eventMessages.Contains("[Warning] 0: Row Splitter Test: The RowType value of 002 was not expected and has 3 columns!"), "[Warning] 0: Row Splitter Test: The RowType value of 002 was not expected and has 3 columns!");
            Assert.IsTrue(packageEvents.eventMessages.Contains("[Warning] 0: Row Splitter Test: The RowType value of 003 was not expected and has 4 columns!"), "[Warning] 0: Row Splitter Test: The RowType value of 003 was not expected and has 4 columns!");
            Assert.IsTrue(packageEvents.eventMessages.Contains("[Warning] 0: Row Splitter Test: The RowType value of 004 was not expected and has 6 columns!"), "[Warning] 0: Row Splitter Test: The RowType value of 004 was not expected and has 6 columns!");
            Assert.IsTrue(packageEvents.eventMessages.Contains("[Information] 0: Row Splitter Test: The Unexpected RowType value of 002 was found 2 times."), "[Information] 0: Row Splitter Test: The Unexpected RowType value of 002 was found 2 times.");
            Assert.IsTrue(packageEvents.eventMessages.Contains("[Information] 0: Row Splitter Test: The Unexpected RowType value of 003 was found 5 times."), "[Information] 0: Row Splitter Test: The Unexpected RowType value of 003 was found 5 times.");
            Assert.IsTrue(packageEvents.eventMessages.Contains("[Information] 0: Row Splitter Test: The Unexpected RowType value of 004 was found 2 times."), "[Information] 0: Row Splitter Test: The Unexpected RowType value of 004 was found 2 times.");
            Assert.IsTrue(packageEvents.eventMessages.Contains("[Information] 0: Row Splitter Test: The Unexpected RowType value of 004 was found 2 times."), "[Information] 0: Row Splitter Test: The Unexpected RowType value of 004 was found 2 times.");

            SqlCeConnection connection = new SqlCeConnection(connectionString());
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            SqlCeCommand sqlCommand = new SqlCeCommand("SELECT * FROM [KeyRecords] ORDER BY [KeyColumn2]", connection);
            SqlCeDataReader sqlData = sqlCommand.ExecuteReader(CommandBehavior.Default);
            int rowCount = 0;
            while (sqlData.Read())
            {
                rowCount++;
                if (rowCount == 1)
                {
                    Assert.AreEqual(12345, sqlData.GetInt32(1), "Row 1 KeyColumn2");
                    Assert.AreEqual("Start Invoice Record", sqlData.GetString(2), "Row 1 KeyColumn3");
                }
                else
                {
                    Assert.AreEqual(12346, sqlData.GetInt32(1), "Row 2 KeyColumn2");
                    Assert.AreEqual("2nd Start Invoice Record", sqlData.GetString(2), "Row 2 KeyColumn3");
                }
            }

            connection.Close();
            Assert.AreEqual(2, rowCount, "Rows in KeyRecords");
        }

        [TestMethod]
        public void TestPrimeOutput_RowCountOutput()
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
            rowTypeColumn.ColumnDelimiter = @"|";
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
            textFileSplitter.ComponentClassID = typeof(Martin.SQLServer.Dts.TextFileSplitter).AssemblyQualifiedName;
            CManagedComponentWrapper instance = textFileSplitter.Instantiate();

            instance.ProvideComponentProperties();
            textFileSplitter.Name = "Row Splitter Test";
            instance.SetComponentProperty(ManageProperties.columnDelimiter, @"|");
            textFileSplitter.RuntimeConnectionCollection[0].ConnectionManager = DtsConvert.GetExtendedInterface(flatFileConnectionManager);
            textFileSplitter.RuntimeConnectionCollection[0].ConnectionManagerID = flatFileConnectionManager.ID;
            instance.AcquireConnections(null);
            instance.ReinitializeMetaData();
            instance.ReleaseConnections();

            IDTSOutput100 passthroughOutput = textFileSplitter.OutputCollection[0];
            instance.SetOutputColumnProperty(passthroughOutput.ID, passthroughOutput.OutputColumnCollection[0].ID, ManageProperties.usageOfColumn, Utilities.usageOfColumnEnum.RowType);
            instance.SetOutputColumnProperty(passthroughOutput.ID, passthroughOutput.OutputColumnCollection[1].ID, ManageProperties.usageOfColumn, Utilities.usageOfColumnEnum.RowData);
            passthroughOutput.ErrorRowDisposition = DTSRowDisposition.RD_IgnoreFailure;
            IDTSOutput100 errorOutput = textFileSplitter.OutputCollection[1];
            IDTSOutput100 keyOutput = textFileSplitter.OutputCollection[2];
            int keyID = keyOutput.ID;
            keyOutput.ErrorRowDisposition = DTSRowDisposition.RD_IgnoreFailure;
            IDTSOutput100 numberOfRowsOutput = textFileSplitter.OutputCollection[3];

            // Setup keyOutput with 2 columns
            instance.SetOutputProperty(keyID, ManageProperties.rowTypeValue, "001");
            IDTSOutputColumn100 keyColumn1 = instance.InsertOutputColumnAt(keyID, 1, "KeyColumn2", String.Empty);
            IDTSOutputColumn100 keyColumn2 = instance.InsertOutputColumnAt(keyID, 2, "KeyColumn3", String.Empty);
            keyOutput.OutputColumnCollection[0].Name = "KeyColumn1";

            // Add SQL CE Connection
            ConnectionManager sqlCECM = package.Connections.Add("SQLMOBILE");
            sqlCECM.ConnectionString = connectionString();
            sqlCECM.Name = "SQLCE Destination";

            IDTSComponentMetaData100 sqlCETarget = dataFlowTask.ComponentMetaDataCollection.New();
            sqlCETarget.ComponentClassID = typeof(Microsoft.SqlServer.Dts.Pipeline.SqlCEDestinationAdapter).AssemblyQualifiedName; //"{874F7595-FB5F-40FF-96AF-FBFF8250E3EF}"; // SQL Compact
            CManagedComponentWrapper sqlCEInstance = sqlCETarget.Instantiate();
            sqlCEInstance.ProvideComponentProperties();
            sqlCETarget.Name = "SQLCE Target";
            sqlCETarget.RuntimeConnectionCollection[0].ConnectionManager = DtsConvert.GetExtendedInterface(sqlCECM);
            sqlCETarget.RuntimeConnectionCollection[0].ConnectionManagerID = sqlCECM.ID;

            Debug.WriteLine(String.Format("Count is {0}", sqlCETarget.CustomPropertyCollection.Count));
            foreach (IDTSCustomProperty100 property in sqlCETarget.CustomPropertyCollection)
            {
                Debug.WriteLine(String.Format("Property Name {0} has value {1}", property.Name, property.Value));
            }
            sqlCETarget.CustomPropertyCollection["Table Name"].Value = "KeyRecords";
            sqlCEInstance.AcquireConnections(null);
            sqlCEInstance.ReinitializeMetaData();
            sqlCEInstance.ReleaseConnections();

            // Create the path from source to destination.
            IDTSPath100 path = dataFlowTask.PathCollection.New();
            path.AttachPathAndPropagateNotifications(keyOutput, sqlCETarget.InputCollection[0]);

            // Get the destination's default input and virtual input.
            IDTSInput100 input = sqlCETarget.InputCollection[0];
            IDTSVirtualInput100 vInput = input.GetVirtualInput();

            // Iterate through the virtual input column collection.
            foreach (IDTSVirtualInputColumn100 vColumn in vInput.VirtualInputColumnCollection)
            {
                // Find external column by name
                IDTSExternalMetadataColumn100 externalColumn = null;
                foreach (IDTSExternalMetadataColumn100 column in input.ExternalMetadataColumnCollection)
                {
                    if (String.Compare(column.Name, vColumn.Name, true) == 0)
                    {
                        externalColumn = column;
                        break;
                    }
                }
                if (externalColumn != null)
                {
                    // Select column, and retain new input column
                    IDTSInputColumn100 inputColumn = sqlCEInstance.SetUsageType(input.ID, vInput, vColumn.LineageID, DTSUsageType.UT_READONLY);
                    // Map input column to external column
                    sqlCEInstance.MapInputColumn(input.ID, inputColumn.ID, externalColumn.ID);
                }
            }

            PackageEventHandler packageEvents = new PackageEventHandler();

            Microsoft.SqlServer.Dts.Runtime.DTSExecResult result = package.Execute(null, null, packageEvents as IDTSEvents, null, null);
            foreach (String message in packageEvents.eventMessages)
            {
                Debug.WriteLine(message);
            }
            Assert.AreEqual(Microsoft.SqlServer.Dts.Runtime.DTSExecResult.Success, result, "Execution Failed");
            Assert.IsTrue(packageEvents.eventMessages.Contains("[Warning] 0: Row Splitter Test: The RowType value of 002 was not expected and has 3 columns!"), "[Warning] 0: Row Splitter Test: The RowType value of 002 was not expected and has 3 columns!");
            Assert.IsTrue(packageEvents.eventMessages.Contains("[Warning] 0: Row Splitter Test: The RowType value of 003 was not expected and has 4 columns!"), "[Warning] 0: Row Splitter Test: The RowType value of 003 was not expected and has 4 columns!");
            Assert.IsTrue(packageEvents.eventMessages.Contains("[Warning] 0: Row Splitter Test: The RowType value of 004 was not expected and has 6 columns!"), "[Warning] 0: Row Splitter Test: The RowType value of 004 was not expected and has 6 columns!");
            Assert.IsTrue(packageEvents.eventMessages.Contains("[Information] 0: Row Splitter Test: The Unexpected RowType value of 002 was found 2 times."), "[Information] 0: Row Splitter Test: The Unexpected RowType value of 002 was found 2 times.");
            Assert.IsTrue(packageEvents.eventMessages.Contains("[Information] 0: Row Splitter Test: The Unexpected RowType value of 003 was found 5 times."), "[Information] 0: Row Splitter Test: The Unexpected RowType value of 003 was found 5 times.");
            Assert.IsTrue(packageEvents.eventMessages.Contains("[Information] 0: Row Splitter Test: The Unexpected RowType value of 004 was found 2 times."), "[Information] 0: Row Splitter Test: The Unexpected RowType value of 004 was found 2 times.");
            Assert.IsTrue(packageEvents.eventMessages.Contains("[Information] 0: Row Splitter Test: The Unexpected RowType value of 004 was found 2 times."), "[Information] 0: Row Splitter Test: The Unexpected RowType value of 004 was found 2 times.");

            SqlCeConnection connection = new SqlCeConnection(connectionString());
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            SqlCeCommand sqlCommand = new SqlCeCommand("SELECT * FROM [KeyRecords] ORDER BY [KeyColumn2]", connection);
            SqlCeDataReader sqlData = sqlCommand.ExecuteReader(CommandBehavior.Default);
            int rowCount = 0;
            while (sqlData.Read())
            {
                rowCount++;
                if (rowCount == 1)
                {
                    Assert.AreEqual(12345, sqlData.GetInt32(1), "Row 1 KeyColumn2");
                    Assert.AreEqual("Start Invoice Record", sqlData.GetString(2), "Row 1 KeyColumn3");
                }
                else
                {
                    Assert.AreEqual(12346, sqlData.GetInt32(1), "Row 2 KeyColumn2");
                    Assert.AreEqual("2nd Start Invoice Record", sqlData.GetString(2), "Row 2 KeyColumn3");
                }
            }

            connection.Close();
            Assert.AreEqual(2, rowCount, "Rows in KeyRecords");
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
