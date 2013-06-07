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
