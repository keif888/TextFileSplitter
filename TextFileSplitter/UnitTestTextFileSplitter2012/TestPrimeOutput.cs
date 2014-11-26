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

            tableCreate = "CREATE TABLE [KeyRecords2] ([KeyColumn1] uniqueidentifier, [KeyColumn2] nvarchar(255), [KeyColumn3] nvarchar(255), [KeyColumn4] DATETIME)";
            command.CommandText = tableCreate;
            command.ExecuteNonQuery();

            tableCreate = "CREATE TABLE [ErrorResults] ([ErrorCode] integer, [ErrorColumn] integer, [ErrorMessage] nvarchar(4000), [ColumnData] nvarchar(4000), [RowData] nvarchar(4000), [KeyColumn1] uniqueidentifier )";
            command.CommandText = tableCreate;
            command.ExecuteNonQuery();

            tableCreate = "CREATE TABLE [RowCountOutput] ([KeyValue] nvarchar(255), [NumberOfRows] integer, [KeyValueStatus] nvarchar(255))";
            command.CommandText = tableCreate;
            command.ExecuteNonQuery();

            tableCreate = "CREATE TABLE [Output099] ([KeyColumn1] uniqueidentifier, [output099Column1] nvarchar(50), [output099Column2] nvarchar(50), [output099Column3] nvarchar(50))";
            command.CommandText = tableCreate;
            command.ExecuteNonQuery();

            tableCreate = "CREATE TABLE [Output002] ([KeyColumn1] uniqueidentifier, [output002Column1] nvarchar(50), [MasterKeyColumn2] nvarchar(50), [output002Column3] nvarchar(50))";
            command.CommandText = tableCreate;
            command.ExecuteNonQuery();

            tableCreate = "CREATE TABLE [Output003] ([KeyColumn1] uniqueidentifier, [MasterKeyColumn2] nvarchar(50), [output003Column1] nvarchar(50),  [output003Column2] nvarchar(50),  [output003Column3] nvarchar(50),  [output003Column4] nvarchar(50))";
            command.CommandText = tableCreate;
            command.ExecuteNonQuery();

            tableCreate = "CREATE TABLE [Output004] ([KeyColumn1] uniqueidentifier, [MasterKeyColumn2] nvarchar(50), [output004Column1] nvarchar(50), [output004Column2] nvarchar(50), [output004Column3] nvarchar(50), [output004Column4] nvarchar(50), [output004Column5] nvarchar(50), [output004Column6] nvarchar(50))";
            command.CommandText = tableCreate;
            command.ExecuteNonQuery();

            tableCreate = "CREATE TABLE [Account] ([AccountKey] uniqueidentifier, [AccountCode] integer, [AccountName] nvarchar(255))";
            command.CommandText = tableCreate;
            command.ExecuteNonQuery();

            tableCreate = "CREATE TABLE [Invoice] ([AccountKey] uniqueidentifier, [InvoiceID] integer, [InvoiceDate] datetime, [InvoiceStore] nvarchar(255))";
            command.CommandText = tableCreate;
            command.ExecuteNonQuery();

            tableCreate = "CREATE TABLE [InvoiceItem] ([AccountKey] uniqueidentifier, [InvoiceID] integer, [Quantity] numeric(18,2), [Cost] numeric(18,2), [Description] nvarchar(255))";
            command.CommandText = tableCreate;
            command.ExecuteNonQuery();

            tableCreate = "CREATE TABLE [InvoiceSubItem] ([AccountKey] uniqueidentifier, [InvoiceID] integer, [ItemDescription] nvarchar(255), [Quantity] numeric(18,2), [Cost] numeric(18,2), [Description] nvarchar(255))";
            command.CommandText = tableCreate;
            command.ExecuteNonQuery();

            tableCreate = "CREATE TABLE [InvoiceTail] ([AccountKey] uniqueidentifier, [InvoiceID] integer, [Total] numeric(18,2), [Tax] numeric(18,2))";
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
            ConnectionManager sqlCECM = null;
            IDTSComponentMetaData100 sqlCETarget = null;
            CManagedComponentWrapper sqlCEInstance = null;
            CreateSQLCEComponent(package, dataFlowTask, "KeyRecords", out sqlCECM, out sqlCETarget, out sqlCEInstance);

            // Create the path from source to destination.
            CreatePath(dataFlowTask, keyOutput, sqlCETarget, sqlCEInstance);

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
            Assert.IsTrue(packageEvents.eventMessages.Contains("[Information] 0: Row Splitter Test: The Unexpected RowType value of 003 was found 6 times."), "[Information] 0: Row Splitter Test: The Unexpected RowType value of 003 was found 5 times.");
            Assert.IsTrue(packageEvents.eventMessages.Contains("[Information] 0: Row Splitter Test: The Unexpected RowType value of 004 was found 3 times."), "[Information] 0: Row Splitter Test: The Unexpected RowType value of 004 was found 2 times.");

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
                else if (rowCount == 2)
                {
                    Assert.AreEqual(12346, sqlData.GetInt32(1), "Row 2 KeyColumn2");
                    Assert.AreEqual("2nd Start Invoice Record", sqlData.GetString(2), "Row 2 KeyColumn3");
                }
                else if (rowCount == 3)
                {
                    Assert.AreEqual(12347, sqlData.GetInt32(1), "Row 3 KeyColumn2");
                    Assert.AreEqual("3rd Start Invoice Record", sqlData.GetString(2), "Row 3 KeyColumn3");
                }
            }

            connection.Close();
            Assert.AreEqual(3, rowCount, "Rows in KeyRecords");
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
            ConnectionManager sqlCECM = null;
            IDTSComponentMetaData100 sqlCETarget = null;
            CManagedComponentWrapper sqlCEInstance = null;
            CreateSQLCEComponent(package, dataFlowTask, "KeyRecords", out sqlCECM, out sqlCETarget, out sqlCEInstance);

            // Create the path from source to destination.
            CreatePath(dataFlowTask, keyOutput, sqlCETarget, sqlCEInstance);

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
            Assert.IsTrue(packageEvents.eventMessages.Contains("[Information] 0: Row Splitter Test: The Unexpected RowType value of 002 was found 2 times."), "RowType value of 002");
            Assert.IsTrue(packageEvents.eventMessages.Contains("[Information] 0: Row Splitter Test: The Unexpected RowType value of 003 was found 6 times."), "RowType value of 003");
            Assert.IsTrue(packageEvents.eventMessages.Contains("[Information] 0: Row Splitter Test: The Unexpected RowType value of 004 was found 3 times."), "RowType value of 004");

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
                else if (rowCount == 2)
                {
                    Assert.AreEqual(12346, sqlData.GetInt32(1), "Row 2 KeyColumn2");
                    Assert.AreEqual("2nd Start Invoice Record", sqlData.GetString(2), "Row 2 KeyColumn3");
                }
                else if (rowCount == 3)
                {
                    Assert.AreEqual(12347, sqlData.GetInt32(1), "Row 3 KeyColumn2");
                    Assert.AreEqual("3rd Start Invoice Record", sqlData.GetString(2), "Row 3 KeyColumn3");
                }
            }

            connection.Close();
            Assert.AreEqual(3, rowCount, "Rows in KeyRecords");
        }

        [TestMethod]
        public void TestPrimeOutput_AllMappingsOutput()
        {
            Microsoft.SqlServer.Dts.Runtime.Package package = new Microsoft.SqlServer.Dts.Runtime.Package();
            Executable exec = package.Executables.Add("STOCK:PipelineTask");
            Microsoft.SqlServer.Dts.Runtime.TaskHost thMainPipe = exec as Microsoft.SqlServer.Dts.Runtime.TaskHost;
            thMainPipe.Name = "Pipeline Task";
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
            textFileSplitter.Name = "Text File Splitter Data Source";
            instance.SetComponentProperty(ManageProperties.columnDelimiter, @"|");
            textFileSplitter.RuntimeConnectionCollection[0].ConnectionManager = DtsConvert.GetExtendedInterface(flatFileConnectionManager);
            textFileSplitter.RuntimeConnectionCollection[0].ConnectionManagerID = flatFileConnectionManager.ID;
            instance.AcquireConnections(null);
            instance.ReinitializeMetaData();
            instance.ReleaseConnections();

            IDTSOutput100 passthroughOutput = textFileSplitter.OutputCollection[0];
            instance.SetOutputColumnProperty(passthroughOutput.ID, passthroughOutput.OutputColumnCollection[0].ID, ManageProperties.usageOfColumn, Utilities.usageOfColumnEnum.RowType);
            instance.SetOutputColumnProperty(passthroughOutput.ID, passthroughOutput.OutputColumnCollection[1].ID, ManageProperties.usageOfColumn, Utilities.usageOfColumnEnum.RowData);
            passthroughOutput.ErrorRowDisposition = DTSRowDisposition.RD_RedirectRow;
            IDTSOutput100 errorOutput = textFileSplitter.OutputCollection[1];
            errorOutput.Name = "ErrorOutput";
            IDTSOutput100 keyOutput = textFileSplitter.OutputCollection[2];
            int keyID = keyOutput.ID;
            keyOutput.ErrorRowDisposition = DTSRowDisposition.RD_RedirectRow;
            keyOutput.TruncationRowDisposition = DTSRowDisposition.RD_RedirectRow;
            keyOutput.Name = "KeyOutput";
            IDTSOutput100 numberOfRowsOutput = textFileSplitter.OutputCollection[3];

            // Setup keyOutput with 2 columns
            instance.SetOutputProperty(keyID, ManageProperties.rowTypeValue, "001");
            IDTSOutputColumn100 keyColumn1 = instance.InsertOutputColumnAt(keyID, 1, "KeyColumn2", String.Empty);
            IDTSOutputColumn100 keyColumn2 = instance.InsertOutputColumnAt(keyID, 2, "KeyColumn3", String.Empty);
            keyOutput.OutputColumnCollection[0].Name = "KeyColumn1";

            IDTSOutput100 output002 = instance.InsertOutput(DTSInsertPlacement.IP_AFTER, keyID);
            output002.Name = "output_002";
            int output002ID = output002.ID;
            instance.SetOutputProperty(output002ID, ManageProperties.rowTypeValue, "002");
            instance.SetOutputProperty(output002ID, ManageProperties.typeOfOutput, Utilities.typeOfOutputEnum.MasterRecord);
            output002.ErrorRowDisposition = DTSRowDisposition.RD_RedirectRow;
            output002.TruncationRowDisposition = DTSRowDisposition.RD_RedirectRow;
            IDTSOutputColumn100 output002Column1 = instance.InsertOutputColumnAt(output002ID, 1, "output002Column1", String.Empty);
            instance.SetOutputColumnDataTypeProperties(output002ID, output002Column1.ID, DataType.DT_STR, 50, 0, 0, 1252);
            IDTSOutputColumn100 output002Column2 = instance.InsertOutputColumnAt(output002ID, 2, "MasterKeyColumn2", String.Empty);
            instance.SetOutputColumnDataTypeProperties(output002ID, output002Column2.ID, DataType.DT_STR, 50, 0, 0, 1252);
            ManageProperties.SetPropertyValue(output002Column2.CustomPropertyCollection, ManageProperties.usageOfColumn, Utilities.usageOfColumnEnum.MasterValue);
            IDTSOutputColumn100 output002Column3 = instance.InsertOutputColumnAt(output002ID, 3, "output002Column3", String.Empty);
            instance.SetOutputColumnDataTypeProperties(output002ID, output002Column3.ID, DataType.DT_STR, 50, 0, 0, 1252);

            IDTSOutput100 output003 = instance.InsertOutput(DTSInsertPlacement.IP_AFTER, keyID);
            output003.Name = "output_003";
            int output003ID = output003.ID;
            instance.SetOutputProperty(output003ID, ManageProperties.rowTypeValue, "003");
            instance.SetOutputProperty(output003ID, ManageProperties.typeOfOutput, Utilities.typeOfOutputEnum.ChildRecord);
            instance.SetOutputProperty(output003ID, ManageProperties.masterRecordID, output002ID);
            output003.ErrorRowDisposition = DTSRowDisposition.RD_RedirectRow;
            output003.TruncationRowDisposition = DTSRowDisposition.RD_RedirectRow;
            IDTSOutputColumn100 output003Column1 = instance.InsertOutputColumnAt(output003ID, 2, "output003Column1", String.Empty);
            instance.SetOutputColumnDataTypeProperties(output003ID, output003Column1.ID, DataType.DT_STR, 50, 0, 0, 1252);
            IDTSOutputColumn100 output003Column2 = instance.InsertOutputColumnAt(output003ID, 3, "output003Column2", String.Empty);
            instance.SetOutputColumnDataTypeProperties(output003ID, output003Column2.ID, DataType.DT_STR, 50, 0, 0, 1252);
            IDTSOutputColumn100 output003Column3 = instance.InsertOutputColumnAt(output003ID, 4, "output003Column3", String.Empty);
            instance.SetOutputColumnDataTypeProperties(output003ID, output003Column3.ID, DataType.DT_STR, 50, 0, 0, 1252);
            IDTSOutputColumn100 output003Column4 = instance.InsertOutputColumnAt(output003ID, 5, "output003Column4", String.Empty);
            instance.SetOutputColumnDataTypeProperties(output003ID, output003Column4.ID, DataType.DT_STR, 50, 0, 0, 1252);

            IDTSOutput100 output004 = instance.InsertOutput(DTSInsertPlacement.IP_AFTER, keyID);
            output004.Name = "output_004";
            int output004ID = output004.ID;
            instance.SetOutputProperty(output004ID, ManageProperties.rowTypeValue, "004");
            instance.SetOutputProperty(output004ID, ManageProperties.typeOfOutput, Utilities.typeOfOutputEnum.ChildRecord);
            instance.SetOutputProperty(output004ID, ManageProperties.masterRecordID, output002ID);
            output004.ErrorRowDisposition = DTSRowDisposition.RD_RedirectRow;
            output004.TruncationRowDisposition = DTSRowDisposition.RD_RedirectRow;
            IDTSOutputColumn100 output004Column1 = instance.InsertOutputColumnAt(output004ID, 2, "output004Column1", String.Empty);
            instance.SetOutputColumnDataTypeProperties(output004ID, output004Column1.ID, DataType.DT_STR, 50, 0, 0, 1252);
            IDTSOutputColumn100 output004Column2 = instance.InsertOutputColumnAt(output004ID, 3, "output004Column2", String.Empty);
            instance.SetOutputColumnDataTypeProperties(output004ID, output004Column2.ID, DataType.DT_STR, 50, 0, 0, 1252);
            IDTSOutputColumn100 output004Column3 = instance.InsertOutputColumnAt(output004ID, 4, "output004Column3", String.Empty);
            instance.SetOutputColumnDataTypeProperties(output004ID, output004Column3.ID, DataType.DT_STR, 50, 0, 0, 1252);
            IDTSOutputColumn100 output004Column4 = instance.InsertOutputColumnAt(output004ID, 5, "output004Column4", String.Empty);
            instance.SetOutputColumnDataTypeProperties(output004ID, output004Column4.ID, DataType.DT_STR, 50, 0, 0, 1252);
            IDTSOutputColumn100 output004Column5 = instance.InsertOutputColumnAt(output004ID, 6, "output004Column5", String.Empty);
            instance.SetOutputColumnDataTypeProperties(output004ID, output004Column4.ID, DataType.DT_STR, 50, 0, 0, 1252);
            IDTSOutputColumn100 output004Column6 = instance.InsertOutputColumnAt(output004ID, 7, "output004Column6", String.Empty);
            instance.SetOutputColumnDataTypeProperties(output004ID, output004Column4.ID, DataType.DT_STR, 50, 0, 0, 1252);


            // Add SQL CE Connection
            ConnectionManager sqlCECM = null;
            IDTSComponentMetaData100 sqlCETarget = null;
            CManagedComponentWrapper sqlCEInstance = null;
            CreateSQLCEComponent(package, dataFlowTask, "KeyRecords", out sqlCECM, out sqlCETarget, out sqlCEInstance);

            // Create the path from source to destination.
            CreatePath(dataFlowTask, keyOutput, sqlCETarget, sqlCEInstance);

            // Create the SQL Ce Target.
            CreateSQLCEComponent(package, dataFlowTask, "Output002", out sqlCECM, out sqlCETarget, out sqlCEInstance);

            // Create the path from source to destination.
            CreatePath(dataFlowTask, output002, sqlCETarget, sqlCEInstance);

            // Create the SQL Ce Target.
            CreateSQLCEComponent(package, dataFlowTask, "Output003", out sqlCECM, out sqlCETarget, out sqlCEInstance);

            // Create the path from source to destination.
            CreatePath(dataFlowTask, output003, sqlCETarget, sqlCEInstance);

            // Create the SQL Ce Target.
            CreateSQLCEComponent(package, dataFlowTask, "Output004", out sqlCECM, out sqlCETarget, out sqlCEInstance);

            // Create the path from source to destination.
            CreatePath(dataFlowTask, output004, sqlCETarget, sqlCEInstance);

            CreateSQLCEComponent(package, dataFlowTask, "ErrorResults", out sqlCECM, out sqlCETarget, out sqlCEInstance);
            CreatePath(dataFlowTask, errorOutput, sqlCETarget, sqlCEInstance);


            PackageEventHandler packageEvents = new PackageEventHandler();

            
            Microsoft.SqlServer.Dts.Runtime.Application application = new Microsoft.SqlServer.Dts.Runtime.Application();
            application.SaveToXml(@"D:\Temp\TestPackage.dtsx", package, null);
            Microsoft.SqlServer.Dts.Runtime.DTSExecResult result = package.Execute(null, null, packageEvents as IDTSEvents, null, null);
            foreach (String message in packageEvents.eventMessages)
            {
                Debug.WriteLine(message);
            }
            Assert.AreEqual(Microsoft.SqlServer.Dts.Runtime.DTSExecResult.Success, result, "Execution Failed");

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
                else if (rowCount == 2)
                {
                    Assert.AreEqual(12346, sqlData.GetInt32(1), "Row 2 KeyColumn2");
                    Assert.AreEqual("2nd Start Invoice Record", sqlData.GetString(2), "Row 2 KeyColumn3");
                }
                else if (rowCount == 3)
                {
                    Assert.AreEqual(12347, sqlData.GetInt32(1), "Row 3 KeyColumn2");
                    Assert.AreEqual("3rd Start Invoice Record", sqlData.GetString(2), "Row 3 KeyColumn3");
                }
            }
            Assert.AreEqual(3, rowCount, "Rows in KeyRecords");

            sqlCommand = new SqlCeCommand("SELECT COUNT(*) FROM [ErrorResults]", connection);
            sqlData = sqlCommand.ExecuteReader(CommandBehavior.Default);
            while (sqlData.Read())
            {
                Assert.AreEqual(0, sqlData.GetInt32(0), "Number of Errors Not Zero");
            }

            sqlCommand = new SqlCeCommand("SELECT COUNT(*) FROM [Output002]", connection);
            sqlData = sqlCommand.ExecuteReader(CommandBehavior.Default);
            while (sqlData.Read())
            {
                Assert.AreEqual(2, sqlData.GetInt32(0), "Number of Output002 Records Wrong");
            }

            sqlCommand = new SqlCeCommand("SELECT COUNT(*) FROM [Output003]", connection);
            sqlData = sqlCommand.ExecuteReader(CommandBehavior.Default);
            while (sqlData.Read())
            {
                Assert.AreEqual(6, sqlData.GetInt32(0), "Number of Output003 Records Wrong");
            }

            sqlCommand = new SqlCeCommand("SELECT COUNT(*) FROM [Output003] WHERE [MasterKeyColumn2] IS NULL", connection);
            sqlData = sqlCommand.ExecuteReader(CommandBehavior.Default);
            while (sqlData.Read())
            {
                Assert.AreEqual(1, sqlData.GetInt32(0), "Number of Output003 Records That are NULL for MasterKeyColumn2 is Wrong");
            }

            sqlCommand = new SqlCeCommand("SELECT COUNT(*) FROM [Output004]", connection);
            sqlData = sqlCommand.ExecuteReader(CommandBehavior.Default);
            while (sqlData.Read())
            {
                Assert.AreEqual(3, sqlData.GetInt32(0), "Number of Output004 Records Wrong");
            }


            connection.Close();
        }

        [TestMethod]
        public void TestPrimeOutput_KeyMasterAndChildMaster()
        {
            // Create an SSIS Package
            Microsoft.SqlServer.Dts.Runtime.Package package = new Microsoft.SqlServer.Dts.Runtime.Package();
            // Add a Pipeline Task called Pipeline Task
            Executable exec = package.Executables.Add("STOCK:PipelineTask");
            Microsoft.SqlServer.Dts.Runtime.TaskHost thMainPipe = exec as Microsoft.SqlServer.Dts.Runtime.TaskHost;
            thMainPipe.Name = "Pipeline Task";
            MainPipe dataFlowTask = thMainPipe.InnerObject as MainPipe;

            // Add an Event Handler
            ComponentEventHandler events = new ComponentEventHandler();
            dataFlowTask.Events = DtsConvert.GetExtendedInterface(events as IDTSComponentEvents);

            // Create a flat file source
            ConnectionManager flatFileConnectionManager = package.Connections.Add("FLATFILE");
            flatFileConnectionManager.Properties["Format"].SetValue(flatFileConnectionManager, "Delimited");
            flatFileConnectionManager.Properties["Name"].SetValue(flatFileConnectionManager, "Flat File Connection");
            flatFileConnectionManager.Properties["ConnectionString"].SetValue(flatFileConnectionManager, @".\KeyMasterAndChildMaster.txt");
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
            rowTypeColumn.MaximumWidth = 3;
            ((IDTSName100)rowTypeColumn).Name = "rowType";
            IDTSConnectionManagerFlatFileColumn100 dataColumn = flatFileConnection.Columns.Add();
            dataColumn.ColumnDelimiter = "\r\n";
            dataColumn.ColumnType = "Delimited";
            dataColumn.DataType = DataType.DT_TEXT;
            dataColumn.DataPrecision = 0;
            dataColumn.DataScale = 0;
            ((IDTSName100)dataColumn).Name = "Data";

            // Create the Text File Splitter component
            IDTSComponentMetaData100 textFileSplitter = dataFlowTask.ComponentMetaDataCollection.New();
            textFileSplitter.ComponentClassID = typeof(Martin.SQLServer.Dts.TextFileSplitter).AssemblyQualifiedName;
            CManagedComponentWrapper instance = textFileSplitter.Instantiate();
            
            // Setup the text file splitter component's base properties, and assign the file connection manager
            instance.ProvideComponentProperties();
            textFileSplitter.Name = "Text File Splitter Data Source";
            instance.SetComponentProperty(ManageProperties.columnDelimiter, @"|");
            textFileSplitter.RuntimeConnectionCollection[0].ConnectionManager = DtsConvert.GetExtendedInterface(flatFileConnectionManager);
            textFileSplitter.RuntimeConnectionCollection[0].ConnectionManagerID = flatFileConnectionManager.ID;
            instance.AcquireConnections(null);
            instance.ReinitializeMetaData();
            instance.ReleaseConnections();

            // Define the Pass Through Output, and it's column types
            IDTSOutput100 passthroughOutput = textFileSplitter.OutputCollection[0];
            instance.SetOutputColumnProperty(passthroughOutput.ID, passthroughOutput.OutputColumnCollection[0].ID, ManageProperties.usageOfColumn, Utilities.usageOfColumnEnum.RowType);
            instance.SetOutputColumnProperty(passthroughOutput.ID, passthroughOutput.OutputColumnCollection[1].ID, ManageProperties.usageOfColumn, Utilities.usageOfColumnEnum.RowData);
            passthroughOutput.ErrorRowDisposition = DTSRowDisposition.RD_RedirectRow;

            // Setup the Error Output
            IDTSOutput100 errorOutput = textFileSplitter.OutputCollection[1];
            errorOutput.Name = "ErrorOutput";

            // Setup the Key Value output (Account)
            IDTSOutput100 accountOutput = textFileSplitter.OutputCollection[2];
            int keyID = accountOutput.ID;
            accountOutput.Name = "Account";
            accountOutput.ErrorRowDisposition = DTSRowDisposition.RD_RedirectRow;
            accountOutput.TruncationRowDisposition = DTSRowDisposition.RD_RedirectRow;
            instance.SetOutputProperty(keyID, ManageProperties.rowTypeValue, "001");

            // Setup accountOutput's 3 columns (AccountKey, AccountCode and AccountName
            accountOutput.OutputColumnCollection[0].Name = "AccountKey";
            IDTSOutputColumn100 keyColumn1 = instance.InsertOutputColumnAt(keyID, 1, "AccountCode", String.Empty);
            instance.SetOutputColumnDataTypeProperties(keyID, keyColumn1.ID, DataType.DT_I4, 0, 0, 0, 0);
            IDTSOutputColumn100 keyColumn2 = instance.InsertOutputColumnAt(keyID, 2, "AccountName", String.Empty);
            instance.SetOutputColumnDataTypeProperties(keyID, keyColumn2.ID, DataType.DT_STR, 255, 0, 0, 1252);

            // Setup the number of Rows output, but leave the default name.
            IDTSOutput100 numberOfRowsOutput = textFileSplitter.OutputCollection[3];

            // Setup the Invoice output as a Master Record
            IDTSOutput100 invoiceOutput = instance.InsertOutput(DTSInsertPlacement.IP_AFTER, keyID);
            invoiceOutput.Name = "Invoice";
            int invoiceOutputID = invoiceOutput.ID;
            instance.SetOutputProperty(invoiceOutputID, ManageProperties.rowTypeValue, "002");
            instance.SetOutputProperty(invoiceOutputID, ManageProperties.typeOfOutput, Utilities.typeOfOutputEnum.MasterRecord);
            invoiceOutput.ErrorRowDisposition = DTSRowDisposition.RD_RedirectRow;
            invoiceOutput.TruncationRowDisposition = DTSRowDisposition.RD_RedirectRow;

            // Setup invoiceOutput's columns, with the InvoiceID as the MasterValue
            IDTSOutputColumn100 invoiceIDOutput = instance.InsertOutputColumnAt(invoiceOutputID, 1, "InvoiceID", String.Empty);
            instance.SetOutputColumnDataTypeProperties(invoiceOutputID, invoiceIDOutput.ID, DataType.DT_I4, 0, 0, 0, 0);
            ManageProperties.SetPropertyValue(invoiceIDOutput.CustomPropertyCollection, ManageProperties.usageOfColumn, Utilities.usageOfColumnEnum.MasterValue);
            IDTSOutputColumn100 invoiceDateOutput = instance.InsertOutputColumnAt(invoiceOutputID, 2, "InvoiceDate", String.Empty);
            instance.SetOutputColumnDataTypeProperties(invoiceOutputID, invoiceDateOutput.ID, DataType.DT_DBDATE, 0, 0, 0, 0);
            ManageProperties.SetPropertyValue(invoiceDateOutput.CustomPropertyCollection, ManageProperties.dotNetFormatString, "yyyy-MM-dd");
            IDTSOutputColumn100 invoiceStoreOutput = instance.InsertOutputColumnAt(invoiceOutputID, 3, "InvoiceStore", String.Empty);
            instance.SetOutputColumnDataTypeProperties(invoiceOutputID, invoiceStoreOutput.ID, DataType.DT_STR, 255, 0, 0, 1252);

            // Setup invoiceItemOutput as a ChildMasterRecord, with Invoice as the Master
            IDTSOutput100 invoiceItemOutput = instance.InsertOutput(DTSInsertPlacement.IP_AFTER, keyID);
            invoiceItemOutput.Name = "InvoiceItem";
            int invoiceItemID = invoiceItemOutput.ID;
            instance.SetOutputProperty(invoiceItemID, ManageProperties.rowTypeValue, "003");
            instance.SetOutputProperty(invoiceItemID, ManageProperties.typeOfOutput, Utilities.typeOfOutputEnum.ChildMasterRecord);
            instance.SetOutputProperty(invoiceItemID, ManageProperties.masterRecordID, invoiceOutputID);
            invoiceItemOutput.ErrorRowDisposition = DTSRowDisposition.RD_RedirectRow;
            invoiceItemOutput.TruncationRowDisposition = DTSRowDisposition.RD_RedirectRow;

            // Setup invoiceITemOutput columns, with Description as the Master Value
            IDTSOutputColumn100 quantityOutput = instance.InsertOutputColumnAt(invoiceItemID, 2, "Quantity", String.Empty);
            instance.SetOutputColumnDataTypeProperties(invoiceItemID, quantityOutput.ID, DataType.DT_NUMERIC, 0, 18, 2, 0);
            IDTSOutputColumn100 costOutput = instance.InsertOutputColumnAt(invoiceItemID, 3, "Cost", String.Empty);
            instance.SetOutputColumnDataTypeProperties(invoiceItemID, costOutput.ID, DataType.DT_NUMERIC, 0, 18, 2, 0);
            IDTSOutputColumn100 descriptionOutput = instance.InsertOutputColumnAt(invoiceItemID, 4, "Description", String.Empty);
            instance.SetOutputColumnDataTypeProperties(invoiceItemID, descriptionOutput.ID, DataType.DT_STR, 255, 0, 0, 1252);
            ManageProperties.SetPropertyValue(descriptionOutput.CustomPropertyCollection, ManageProperties.usageOfColumn, Utilities.usageOfColumnEnum.MasterValue);

            // Setup invoiceItemOutput as a ChildMasterRecord, with Invoice as the Master
            IDTSOutput100 invoiceSubItemOutput = instance.InsertOutput(DTSInsertPlacement.IP_AFTER, keyID);
            invoiceSubItemOutput.Name = "InvoiceSubItem";
            int invoiceSubItemID = invoiceSubItemOutput.ID;
            instance.SetOutputProperty(invoiceSubItemID, ManageProperties.rowTypeValue, "004");
            instance.SetOutputProperty(invoiceSubItemID, ManageProperties.typeOfOutput, Utilities.typeOfOutputEnum.ChildRecord);
            instance.SetOutputProperty(invoiceSubItemID, ManageProperties.masterRecordID, invoiceItemID);
            invoiceSubItemOutput.ErrorRowDisposition = DTSRowDisposition.RD_RedirectRow;
            invoiceSubItemOutput.TruncationRowDisposition = DTSRowDisposition.RD_RedirectRow;

            // Setup invoiceItemOutput columns, with Description as the Master Value
            invoiceSubItemOutput.OutputColumnCollection[2].Name = "ItemDescription"; // Rename the Parent Description column
            IDTSOutputColumn100 quantityOutput2 = instance.InsertOutputColumnAt(invoiceSubItemID, 3, "Quantity", String.Empty);
            instance.SetOutputColumnDataTypeProperties(invoiceSubItemID, quantityOutput2.ID, DataType.DT_NUMERIC, 0, 18, 2, 0);
            IDTSOutputColumn100 costOutput2 = instance.InsertOutputColumnAt(invoiceSubItemID, 4, "Cost", String.Empty);
            instance.SetOutputColumnDataTypeProperties(invoiceSubItemID, costOutput2.ID, DataType.DT_NUMERIC, 0, 18, 2, 0);
            IDTSOutputColumn100 descriptionOutput2 = instance.InsertOutputColumnAt(invoiceSubItemID, 5, "Description", String.Empty);
            instance.SetOutputColumnDataTypeProperties(invoiceSubItemID, descriptionOutput2.ID, DataType.DT_STR, 255, 0, 0, 1252);
            ManageProperties.SetPropertyValue(descriptionOutput2.CustomPropertyCollection, ManageProperties.isColumnOptional, true);

            // Setup invoiceTailOutput as a Child, with Invoice as the Master
            IDTSOutput100 invoiceTailOutput = instance.InsertOutput(DTSInsertPlacement.IP_AFTER, keyID);
            invoiceTailOutput.Name = "InvoiceTail";
            int invoiceTailID = invoiceTailOutput.ID;
            instance.SetOutputProperty(invoiceTailID, ManageProperties.rowTypeValue, "005");
            instance.SetOutputProperty(invoiceTailID, ManageProperties.typeOfOutput, Utilities.typeOfOutputEnum.ChildRecord);
            instance.SetOutputProperty(invoiceTailID, ManageProperties.masterRecordID, invoiceOutputID);
            invoiceTailOutput.ErrorRowDisposition = DTSRowDisposition.RD_RedirectRow;
            invoiceTailOutput.TruncationRowDisposition = DTSRowDisposition.RD_RedirectRow;

            // Setup invoiceTailOutput columns
            IDTSOutputColumn100 totalOutput = instance.InsertOutputColumnAt(invoiceTailID, 2, "Total", String.Empty);
            instance.SetOutputColumnDataTypeProperties(invoiceTailID, totalOutput.ID, DataType.DT_NUMERIC, 0, 18, 2, 0);
            IDTSOutputColumn100 taxOutput = instance.InsertOutputColumnAt(invoiceTailID, 3, "Tax", String.Empty);
            instance.SetOutputColumnDataTypeProperties(invoiceTailID, taxOutput.ID, DataType.DT_NUMERIC, 0, 18, 2, 0);


            // Add SQL CE Connection
            ConnectionManager sqlCECM = null;
            IDTSComponentMetaData100 sqlCETarget = null;
            CManagedComponentWrapper sqlCEInstance = null;
            CreateSQLCEComponent(package, dataFlowTask, "Account", out sqlCECM, out sqlCETarget, out sqlCEInstance);

            // Create the path from source to destination.
            CreatePath(dataFlowTask, accountOutput, sqlCETarget, sqlCEInstance);

            // Create the SQL Ce Target.
            CreateSQLCEComponent(package, dataFlowTask, "Invoice", out sqlCECM, out sqlCETarget, out sqlCEInstance);

            // Create the path from source to destination.
            CreatePath(dataFlowTask, invoiceOutput, sqlCETarget, sqlCEInstance);

            // Create the SQL Ce Target.
            CreateSQLCEComponent(package, dataFlowTask, "InvoiceItem", out sqlCECM, out sqlCETarget, out sqlCEInstance);

            // Create the path from source to destination.
            CreatePath(dataFlowTask, invoiceItemOutput, sqlCETarget, sqlCEInstance);

            // Create the SQL Ce Target.
            CreateSQLCEComponent(package, dataFlowTask, "InvoiceSubItem", out sqlCECM, out sqlCETarget, out sqlCEInstance);

            // Create the path from source to destination.
            CreatePath(dataFlowTask, invoiceSubItemOutput, sqlCETarget, sqlCEInstance);

            // Create the SQL Ce Target.
            CreateSQLCEComponent(package, dataFlowTask, "InvoiceTail", out sqlCECM, out sqlCETarget, out sqlCEInstance);

            // Create the path from source to destination.
            CreatePath(dataFlowTask, invoiceTailOutput, sqlCETarget, sqlCEInstance);

            // Create the Error Output connection
            CreateSQLCEComponent(package, dataFlowTask, "ErrorResults", out sqlCECM, out sqlCETarget, out sqlCEInstance);
            CreatePath(dataFlowTask, errorOutput, sqlCETarget, sqlCEInstance);

            // Create a package events handler, to catch the output when running.
            PackageEventHandler packageEvents = new PackageEventHandler();

            // Create an application object, to enable saving the package
            Microsoft.SqlServer.Dts.Runtime.Application application = new Microsoft.SqlServer.Dts.Runtime.Application();
            // Save the package
            application.SaveToXml(@"D:\Temp\TestPackage.dtsx", package, null);

            // Execute the package
            Microsoft.SqlServer.Dts.Runtime.DTSExecResult result = package.Execute(null, null, packageEvents as IDTSEvents, null, null);
            foreach (String message in packageEvents.eventMessages)
            {
                Debug.WriteLine(message);
            }
            // Make sure the package worked.
            Assert.AreEqual(Microsoft.SqlServer.Dts.Runtime.DTSExecResult.Success, result, "Execution Failed");

            SqlCeConnection connection = new SqlCeConnection(connectionString());
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            SqlCeCommand sqlCommand = new SqlCeCommand("SELECT * FROM [Account] ORDER BY [AccountCode]", connection);
            SqlCeDataReader sqlData = sqlCommand.ExecuteReader(CommandBehavior.Default);
            int rowCount = 0;
            while (sqlData.Read())
            {
                rowCount++;
                switch (rowCount)
                {
                    case 1: 
                        Assert.AreEqual(12345, sqlData.GetInt32(1), "AccountCode <> 12345");
                        Assert.AreEqual("Joe Smith", sqlData.GetString(2), "AccountName <> Joe Smith");
                        break;
                    case 2:
                        Assert.AreEqual(12346, sqlData.GetInt32(1), "AccountCode <> 12346");
                        Assert.AreEqual("James Smith", sqlData.GetString(2), "AccountName <> James Smith");
                        break;
                    case 3:
                        Assert.AreEqual(12356, sqlData.GetInt32(1), "Account Code <> 12356");
                        Assert.AreEqual("Mike Smith", sqlData.GetString(2), "AccountName <> Mike Smith");
                        break;
                    case 4:
                        Assert.AreEqual(12856, sqlData.GetInt32(1), "Account Code <> 12856");
                        Assert.AreEqual("John Smith", sqlData.GetString(2), "AccountName <> John Smith");
                        break;
                    default:
                        Assert.Fail(string.Format("Account has to many records AccountCode {0}, AccountName {1}", sqlData.GetInt32(1), sqlData.GetString(2)));
                        break;
                }
            }
            Assert.AreEqual(4, rowCount, "Rows in Account");

            sqlCommand = new SqlCeCommand("SELECT * FROM [ErrorResults]", connection);
            sqlData = sqlCommand.ExecuteReader(CommandBehavior.Default);
            rowCount = 0;
            while (sqlData.Read())
            {
                rowCount++;
                Debug.WriteLine(String.Format("ErrorCode = {0}, ErrorColumn = {1}, ErrorMessage = {2}, ColumnData = {3}, RowData = {4}, KeyColumn1 = {5}", (sqlData.IsDBNull(0)) ? -1 : sqlData.GetInt32(0), (sqlData.IsDBNull(1)) ? -1 : sqlData.GetInt32(1), (sqlData.IsDBNull(2)) ? "NULL" : sqlData.GetString(2), (sqlData.IsDBNull(3)) ? "NULL" : sqlData.GetString(3), (sqlData.IsDBNull(4)) ? "NULL" : sqlData.GetString(4), (sqlData.IsDBNull(5)) ? new Guid() : sqlData.GetSqlGuid(5)));
            }
            Assert.AreEqual(0, rowCount, "Number of Errors Not Zero");

            sqlCommand = new SqlCeCommand("SELECT [Account].[AccountKey], [AccountCode], [InvoiceID], [InvoiceDate], [InvoiceStore] FROM [Invoice] LEFT OUTER JOIN [Account] ON [Invoice].[AccountKey] = [Account].[AccountKey] ORDER BY [InvoiceID]", connection);
            sqlData = sqlCommand.ExecuteReader(CommandBehavior.Default);
            rowCount = 0;
            while (sqlData.Read())
            {
                rowCount++;
                switch (rowCount)
                {
                    case 1:
                        Assert.AreEqual(12345, sqlData.GetInt32(1), "AccountCode <> 12345");
                        Assert.AreEqual(34566, sqlData.GetInt32(2), "InvoiceID <> 34566");
                        Assert.AreEqual(new DateTime(2014, 11, 20), sqlData.GetDateTime(3), "InvoiceDate <> 2014-11-20");
                        Assert.AreEqual("Brisbane", sqlData.GetString(4), "InvoiceStore <> Brisbane");
                        break;
                    case 2:
                        Assert.AreEqual(12345, sqlData.GetInt32(1), "AccountCode <> 12345");
                        Assert.AreEqual(34567, sqlData.GetInt32(2), "InvoiceID <> 34567");
                        Assert.AreEqual(new DateTime(2014, 11, 25), sqlData.GetDateTime(3), "InvoiceDate <> 2014-11-25");
                        Assert.AreEqual("Milton", sqlData.GetString(4), "InvoiceStore <> Milton");
                        break;
                    case 3:
                        Assert.AreEqual(12346, sqlData.GetInt32(1), "Account Code <> 12346");
                        Assert.AreEqual(34568, sqlData.GetInt32(2), "InvoiceID <> 34568");
                        Assert.AreEqual(new DateTime(2014, 11, 26), sqlData.GetDateTime(3), "InvoiceDate <> 2014-11-26");
                        Assert.AreEqual("Gold Coast", sqlData.GetString(4), "InvoiceStore <> Gold Coast");
                        break;
                    default:
                        Assert.Fail(string.Format("Invoice has to many records InvoiceID {0}, InvoiceStore {1}", sqlData.GetInt32(2), sqlData.GetString(4)));
                        break;
                }
            }
            Assert.AreEqual(3, rowCount, "Number of Invoices Not Three");

            sqlCommand = new SqlCeCommand("SELECT [Account].[AccountKey], [AccountCode], [InvoiceID], [Cost], [Quantity], [Description] FROM [InvoiceItem] LEFT OUTER JOIN [Account] ON [InvoiceItem].[AccountKey] = [Account].[AccountKey] ORDER BY [AccountCode], [Cost]", connection);
            sqlData = sqlCommand.ExecuteReader(CommandBehavior.Default);
            rowCount = 0;
            while (sqlData.Read())
            {
                rowCount++;
                switch (rowCount)
                {
                    case 1:
                        Assert.AreEqual(12345, sqlData.GetInt32(1), "AccountCode <> 12345");
                        Assert.AreEqual(34566, sqlData.GetInt32(2), "InvoiceID <> 34566");
                        Assert.AreEqual(1.0m, sqlData.GetDecimal(3), "Cost <> 1.0");
                        Assert.AreEqual(1.0m, sqlData.GetDecimal(4), "Quantity <> 1.0");
                        Assert.AreEqual("Generic Item 1", sqlData.GetString(5), "Description <> Generic Item 1");
                        break;
                    case 2:
                        Assert.AreEqual(12345, sqlData.GetInt32(1), "AccountCode <> 12345");
                        Assert.AreEqual(34566, sqlData.GetInt32(2), "InvoiceID <> 34566");
                        Assert.AreEqual(2.0m, sqlData.GetDecimal(3), "Cost <> 2.0");
                        Assert.AreEqual(2.0m, sqlData.GetDecimal(4), "Quantity <> 2.0");
                        Assert.AreEqual("Generic Item 2", sqlData.GetString(5), "Description <> Generic Item 2");
                        break;
                    case 3:
                        Assert.AreEqual(12345, sqlData.GetInt32(1), "AccountCode <> 12345");
                        Assert.AreEqual(34567, sqlData.GetInt32(2), "InvoiceID <> 34566");
                        Assert.AreEqual(3.0m, sqlData.GetDecimal(3), "Cost <> 3.0");
                        Assert.AreEqual(1.0m, sqlData.GetDecimal(4), "Quantity <> 1.0");
                        Assert.AreEqual("Generic Item 1 I2", sqlData.GetString(5), "Description <> Generic Item 1 I2");
                        break;
                    case 4:
                        Assert.AreEqual(12345, sqlData.GetInt32(1), "AccountCode <> 12345");
                        Assert.AreEqual(34567, sqlData.GetInt32(2), "InvoiceID <> 34566");
                        Assert.AreEqual(4.0m, sqlData.GetDecimal(3), "Cost <> 4.0");
                        Assert.AreEqual(5.0m, sqlData.GetDecimal(4), "Quantity <> 5.0");
                        Assert.AreEqual("Generic Item 2 I2", sqlData.GetString(5), "Description <> Generic Item 2 I2");
                        break;
                    case 5:
                        Assert.AreEqual(12856, sqlData.GetInt32(1), "AccountCode <> 12856");
                        Assert.IsTrue(sqlData.IsDBNull(2), "InvoiceID was not null");
                        Assert.AreEqual(5.0m, sqlData.GetDecimal(3), "Cost <> 5.0");
                        Assert.AreEqual(1.0m, sqlData.GetDecimal(4), "Quantity <> 1.0");
                        Assert.AreEqual("Item 1 No Invoice", sqlData.GetString(5), "Description <> Item 1 No Invoice");
                        break;
                    case 6:
                        Assert.AreEqual(12856, sqlData.GetInt32(1), "AccountCode <> 12856");
                        Assert.IsTrue(sqlData.IsDBNull(2), "InvoiceID was not null");
                        Assert.AreEqual(6.0m, sqlData.GetDecimal(3), "Cost <> 6.0");
                        Assert.AreEqual(4.0m, sqlData.GetDecimal(4), "Quantity <> 4.0");
                        Assert.AreEqual("Item 2 No Invoice", sqlData.GetString(5), "Description <> Item 2 No Invoice");
                        break;
                    default:
                        Assert.Fail(string.Format("Invoice has to many records InvoiceID {0}, InvoiceStore {1}", sqlData.GetInt32(2), sqlData.GetString(4)));
                        break;
                }
            }
            Assert.AreEqual(6, rowCount, "Number of InvoiceItems Not Six");


            connection.Close();
        }


        [TestMethod]
        public void TestPrimeOutput_TestBadDateWithNullsEnabled()
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
            rowTypeColumn.ColumnDelimiter = @",";
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
            instance.SetComponentProperty(ManageProperties.columnDelimiter, @",");
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
            instance.SetOutputProperty(keyID, ManageProperties.rowTypeValue, "Key");
            IDTSOutputColumn100 keyColumn1 = instance.InsertOutputColumnAt(keyID, 1, "KeyColumn2", String.Empty);
            IDTSOutputColumn100 keyColumn2 = instance.InsertOutputColumnAt(keyID, 2, "KeyColumn3", String.Empty);
            IDTSOutputColumn100 keyColumn3 = instance.InsertOutputColumnAt(keyID, 3, "KeyColumn4", String.Empty);
            keyOutput.OutputColumnCollection[0].Name = "KeyColumn1";
            instance.SetOutputColumnDataTypeProperties(keyID, keyColumn3.ID, DataType.DT_DBDATE, 0, 0, 0, 0);
            instance.SetOutputColumnProperty(keyID, keyColumn3.ID, ManageProperties.nullResultOnConversionError, true);
            instance.SetOutputColumnProperty(keyID, keyColumn3.ID, ManageProperties.dotNetFormatString, "yyyy-MM-dd");

            // Add SQL CE Connection
            ConnectionManager sqlCECM = null;
            IDTSComponentMetaData100 sqlCETarget = null;
            CManagedComponentWrapper sqlCEInstance = null;
            CreateSQLCEComponent(package, dataFlowTask, "KeyRecords2", out sqlCECM, out sqlCETarget, out sqlCEInstance);

            // Create the path from source to destination.
            CreatePath(dataFlowTask, keyOutput, sqlCETarget, sqlCEInstance);

            PackageEventHandler packageEvents = new PackageEventHandler();

            Microsoft.SqlServer.Dts.Runtime.DTSExecResult result = package.Execute(null, null, packageEvents as IDTSEvents, null, null);
            foreach (String message in packageEvents.eventMessages)
            {
                Debug.WriteLine(message);
            }
            Assert.AreEqual(Microsoft.SqlServer.Dts.Runtime.DTSExecResult.Success, result, "Execution Failed");

            SqlCeConnection connection = new SqlCeConnection(connectionString());
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            SqlCeCommand sqlCommand = new SqlCeCommand("SELECT * FROM [KeyRecords2] ORDER BY [KeyColumn2]", connection);
            SqlCeDataReader sqlData = sqlCommand.ExecuteReader(CommandBehavior.Default);
            int rowCount = 0;
            while (sqlData.Read())
            {
                rowCount++;
                switch (rowCount)
                {
                    case 1:
                        Assert.AreEqual("Column1-1", sqlData.GetString(1), "Row 1 Column 2");
                        Assert.AreEqual("Column2", sqlData.GetString(2), "Row 1 Column 3");
                        Assert.AreEqual(DateTime.Parse("2013-02-26"), sqlData.GetDateTime(3), "Row 1 Column 4");
                        break;
                    case 2:
                        Assert.AreEqual("Column1-2", sqlData.GetString(1), "Row 1 Column 2");
                        Assert.AreEqual("Column2", sqlData.GetString(2), "Row 1 Column 3");
                        Assert.AreEqual(DateTime.Parse("2013-02-27"), sqlData.GetDateTime(3), "Row 1 Column 4");
                        break;
                    case 3:
                        Assert.AreEqual("Column1-3", sqlData.GetString(1), "Row 1 Column 2");
                        Assert.AreEqual("Column2", sqlData.GetString(2), "Row 1 Column 3");
                        Assert.AreEqual(DateTime.Parse("2013-02-28"), sqlData.GetDateTime(3), "Row 1 Column 4");
                        break;
                    case 4:
                        Assert.AreEqual("Column1-4", sqlData.GetString(1), "Row 1 Column 2");
                        Assert.AreEqual("Column2", sqlData.GetString(2), "Row 1 Column 3");
                        Assert.IsTrue(sqlData.IsDBNull(3), "Row 1 Column 4");
                        break;
                    default:
                        Assert.Fail("To Many Records Returned!");
                        break;
                }
            }

            connection.Close();
            Assert.AreEqual(4, rowCount, "Rows in KeyRecords");
        }

        [TestMethod]
        public void TestPrimeOutput_TestDataFileWithManyErrorsAndRowCountOutput()
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
            flatFileConnectionManager.Properties["ConnectionString"].SetValue(flatFileConnectionManager, @".\BadDataFile.txt");
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
            passthroughOutput.ErrorRowDisposition = DTSRowDisposition.RD_RedirectRow;
            IDTSOutput100 errorOutput = textFileSplitter.OutputCollection[1];
            IDTSOutput100 keyOutput = textFileSplitter.OutputCollection[2];
            int keyID = keyOutput.ID;
            keyOutput.ErrorRowDisposition = DTSRowDisposition.RD_RedirectRow;
            IDTSOutput100 numberOfRowsOutput = textFileSplitter.OutputCollection[3];

            // Setup keyOutput with 2 columns
            instance.SetOutputProperty(keyID, ManageProperties.rowTypeValue, "001");
            IDTSOutputColumn100 keyColumn1 = instance.InsertOutputColumnAt(keyID, 1, "KeyColumn2", String.Empty);
            instance.SetOutputColumnDataTypeProperties(keyID, keyColumn1.ID, DataType.DT_I4, 0, 0, 0, 0);
            //instance.SetOutputColumnProperty(keyID, keyColumn1.ID, ManageProperties.usageOfColumn, Utilities.usageOfColumnEnum.Key);
            IDTSOutputColumn100 keyColumn2 = instance.InsertOutputColumnAt(keyID, 2, "KeyColumn3", String.Empty);
            instance.SetOutputColumnDataTypeProperties(keyID, keyColumn2.ID, DataType.DT_WSTR, 20, 0, 0, 0);
            keyOutput.OutputColumnCollection[0].Name = "KeyColumn1";
            errorOutput.OutputColumnCollection[5].Name = "KeyColumn1";

            // 002|2001-01-01|Joe Bloggs|Missing Key Value

            IDTSOutput100 output002 = instance.InsertOutput(DTSInsertPlacement.IP_AFTER, keyID);//textFileSplitter.OutputCollection.New();
            int outputID = output002.ID;
            instance.SetOutputProperty(outputID, ManageProperties.rowTypeValue, "099");
            instance.SetOutputProperty(outputID, ManageProperties.typeOfOutput, Utilities.typeOfOutputEnum.DataRecords);
            output002.ErrorRowDisposition = DTSRowDisposition.RD_RedirectRow;
            output002.TruncationRowDisposition = DTSRowDisposition.RD_RedirectRow;
            IDTSOutputColumn100 output099Column1 = instance.InsertOutputColumnAt(outputID, 1, "output099Column1", String.Empty);
            instance.SetOutputColumnDataTypeProperties(outputID, output099Column1.ID, DataType.DT_STR, 50, 0, 0, 1252);
            IDTSOutputColumn100 output099Column2 = instance.InsertOutputColumnAt(outputID, 2, "output099Column2", String.Empty);
            instance.SetOutputColumnDataTypeProperties(outputID, output099Column2.ID, DataType.DT_STR, 50, 0, 0, 1252);
            IDTSOutputColumn100 output099Column3 = instance.InsertOutputColumnAt(outputID, 3, "output099Column3", String.Empty);
            instance.SetOutputColumnDataTypeProperties(outputID, output099Column3.ID, DataType.DT_STR, 50, 0, 0, 1252);


            // Add SQL CE Connection
            ConnectionManager sqlCECM = null;
            IDTSComponentMetaData100 sqlCETarget = null;
            CManagedComponentWrapper sqlCEInstance = null;
            CreateSQLCEComponent(package, dataFlowTask, "KeyRecords", out sqlCECM, out sqlCETarget, out sqlCEInstance);
            CreatePath(dataFlowTask, keyOutput, sqlCETarget, sqlCEInstance);

            CreateSQLCEComponent(package, dataFlowTask, "ErrorResults", out sqlCECM, out sqlCETarget, out sqlCEInstance);
            CreatePath(dataFlowTask, errorOutput, sqlCETarget, sqlCEInstance);

            CreateSQLCEComponent(package, dataFlowTask, "RowCountOutput", out sqlCECM, out sqlCETarget, out sqlCEInstance);
            CreatePath(dataFlowTask, numberOfRowsOutput, sqlCETarget, sqlCEInstance);

            CreateSQLCEComponent(package, dataFlowTask, "Output099", out sqlCECM, out sqlCETarget, out sqlCEInstance);
            CreatePath(dataFlowTask, output002, sqlCETarget, sqlCEInstance);

            PackageEventHandler packageEvents = new PackageEventHandler();

            Microsoft.SqlServer.Dts.Runtime.DTSExecResult result = package.Execute(null, null, packageEvents as IDTSEvents, null, null);
            foreach (String message in packageEvents.eventMessages)
            {
                Debug.WriteLine(message);
            }
            Assert.AreEqual(Microsoft.SqlServer.Dts.Runtime.DTSExecResult.Success, result, "Execution Failed");

            SqlCeConnection connection = new SqlCeConnection(connectionString());
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            SqlCeCommand sqlCommand = new SqlCeCommand("SELECT * FROM [ErrorResults] WHERE ErrorMessage = 'Unexpected Row Type Value 002 found on Record 3.'", connection);
            SqlCeDataReader sqlData = sqlCommand.ExecuteReader(CommandBehavior.Default);
            int rowCount = 0;
            while (sqlData.Read())
            {
                rowCount++;
            }
            Assert.AreEqual(1, rowCount, "Record 3 Error Missing");

            sqlCommand.CommandText = "SELECT COUNT(*) FROM [ErrorResults] WHERE ErrorMessage LIKE 'Unexpected Row Type Value%'";
            sqlData = sqlCommand.ExecuteReader(CommandBehavior.Default);
            while (sqlData.Read())
            {
                Assert.AreEqual(9, sqlData.GetInt32(0), "Number of Unexpected incorrect");
            }

            sqlCommand.CommandText = "SELECT COUNT(*) FROM [ErrorResults] WHERE ErrorMessage LIKE '%not found after field%'";
            sqlData = sqlCommand.ExecuteReader(CommandBehavior.Default);
            while (sqlData.Read())
            {
                Assert.AreEqual(2, sqlData.GetInt32(0), "Number of Delimiter Not Found incorrect");
            }

            rowCount = 0;
            sqlCommand.CommandText = "SELECT * FROM [ErrorResults] WHERE ErrorMessage LIKE '%not found after field%' AND RowData = 'IAMBAD'";
            sqlData = sqlCommand.ExecuteReader(CommandBehavior.Default);
            while (sqlData.Read())
            {
                Assert.IsTrue(sqlData.IsDBNull(5), "KeyColumn1 has a value for IAMBAD");
                rowCount++;
            }
            Assert.AreEqual(1, rowCount, "2nd IAMBAD Error Missing");

            sqlCommand.CommandText = "SELECT COUNT(*) FROM [ErrorResults] WHERE ErrorMessage LIKE '%The value is too large to fit in the column data area of the buffer%'";
            sqlData = sqlCommand.ExecuteReader(CommandBehavior.Default);
            while (sqlData.Read())
            {
                Assert.AreEqual(2, sqlData.GetInt32(0), "Number of To Big incorrect");
            }

            rowCount = 0;
            sqlCommand.CommandText = "SELECT * FROM [ErrorResults] WHERE ErrorMessage LIKE '%The value is too large to fit in the column data area of the buffer%' AND ColumnData = '2nd Start Invoice Record'";
            sqlData = sqlCommand.ExecuteReader(CommandBehavior.Default);
            while (sqlData.Read())
            {
                Assert.IsTrue(sqlData.IsDBNull(5), "KeyColumn1 has a value for ValueToLarge");
                rowCount++;
            }
            Assert.AreEqual(1, rowCount, "2nd Start Invoice Record Error Missing");

            sqlCommand.CommandText = "SELECT COUNT(*) FROM [ErrorResults] WHERE ErrorMessage LIKE '%Int32%'";
            sqlData = sqlCommand.ExecuteReader(CommandBehavior.Default);
            while (sqlData.Read())
            {
                Assert.AreEqual(1, sqlData.GetInt32(0), "Number of Int32 Conversion Errors incorrect");
            }

            sqlCommand.CommandText = "SELECT COUNT(*) FROM [RowCountOutput]";
            sqlData = sqlCommand.ExecuteReader(CommandBehavior.Default);
            while (sqlData.Read())
            {
                Assert.AreEqual(7, sqlData.GetInt32(0), "Number of RowCountOutput Rows incorrect");
            }

            sqlCommand.CommandText = "SELECT * FROM [RowCountOutput] WHERE KeyValue = 'Total Records'";
            sqlData = sqlCommand.ExecuteReader(CommandBehavior.Default);
            while (sqlData.Read())
            {
                Assert.AreEqual(17, sqlData.GetInt32(1), "Number of Total Records incorrect");
                Assert.AreEqual("Disconnected", sqlData.GetString(2), "KeyValue Status on Total Records incorrect");
            }

            sqlCommand.CommandText = "SELECT * FROM [RowCountOutput] WHERE KeyValue = '001'";
            sqlData = sqlCommand.ExecuteReader(CommandBehavior.Default);
            while (sqlData.Read())
            {
                Assert.AreEqual(1, sqlData.GetInt32(1), "Number of 001 incorrect");
                Assert.AreEqual("Connected and Processed", sqlData.GetString(2), "KeyValue Status on 001 incorrect");
            }

            sqlCommand.CommandText = "SELECT * FROM [RowCountOutput] WHERE KeyValue = 'Error Records'";
            sqlData = sqlCommand.ExecuteReader(CommandBehavior.Default);
            while (sqlData.Read())
            {
                Assert.AreEqual(15, sqlData.GetInt32(1), "Number of Error Records incorrect");
                Assert.AreEqual("Connected and Processed", sqlData.GetString(2), "KeyValue Status on Error Records incorrect");
            }

            sqlCommand.CommandText = "SELECT * FROM [RowCountOutput] WHERE KeyValue = '003'";
            sqlData = sqlCommand.ExecuteReader(CommandBehavior.Default);
            while (sqlData.Read())
            {
                Assert.AreEqual(5, sqlData.GetInt32(1), "Number of 003 incorrect");
                Assert.AreEqual("Not configured", sqlData.GetString(2), "KeyValue Status on 003 incorrect");
            }

            sqlCommand.CommandText = "SELECT COUNT(*) FROM [ErrorResults] WHERE ErrorMessage LIKE 'Exception Key Value%'";
            sqlData = sqlCommand.ExecuteReader(CommandBehavior.Default);
            while (sqlData.Read())
            {
                Assert.AreEqual(1, sqlData.GetInt32(0), "099 Value before Key Not Detected.");
            }


            connection.Close();
        }


        private void CreatePath(MainPipe dataFlowTask, IDTSOutput100 fromOutput, IDTSComponentMetaData100 toComponent, CManagedComponentWrapper toInstance)
        {
            // Create the path from source to destination.
            IDTSPath100 path = dataFlowTask.PathCollection.New();
            path.AttachPathAndPropagateNotifications(fromOutput, toComponent.InputCollection[0]);

            // Get the destination's default input and virtual input.
            IDTSInput100 input = toComponent.InputCollection[0];
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
                    IDTSInputColumn100 inputColumn = toInstance.SetUsageType(input.ID, vInput, vColumn.LineageID, DTSUsageType.UT_READONLY);
                    // Map input column to external column
                    toInstance.MapInputColumn(input.ID, inputColumn.ID, externalColumn.ID);
                }
            }
        }

        private void CreateSQLCEComponent(Microsoft.SqlServer.Dts.Runtime.Package package, MainPipe dataFlowTask, String tableName, out ConnectionManager sqlCECM, out IDTSComponentMetaData100 sqlCETarget, out CManagedComponentWrapper sqlCEInstance)
        {
            // Add SQL CE Connection
            sqlCECM = package.Connections.Add("SQLMOBILE");
            sqlCECM.ConnectionString = connectionString();
            sqlCECM.Name = "SQLCE Destination " + tableName;

            sqlCETarget = dataFlowTask.ComponentMetaDataCollection.New();
            sqlCETarget.ComponentClassID = typeof(Microsoft.SqlServer.Dts.Pipeline.SqlCEDestinationAdapter).AssemblyQualifiedName;
            sqlCEInstance = sqlCETarget.Instantiate();
            sqlCEInstance.ProvideComponentProperties();
            sqlCETarget.Name = "SQLCE Target " + tableName;
            sqlCETarget.RuntimeConnectionCollection[0].ConnectionManager = DtsConvert.GetExtendedInterface(sqlCECM);
            sqlCETarget.RuntimeConnectionCollection[0].ConnectionManagerID = sqlCECM.ID;

            sqlCETarget.CustomPropertyCollection["Table Name"].Value = tableName;
            sqlCEInstance.AcquireConnections(null);
            sqlCEInstance.ReinitializeMetaData();
            sqlCEInstance.ReleaseConnections();
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
