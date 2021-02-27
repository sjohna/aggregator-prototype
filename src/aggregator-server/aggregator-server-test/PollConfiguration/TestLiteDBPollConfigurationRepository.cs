using aggregator_server;
using LiteDB;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace aggregator_server_test
{
    [TestFixture]
    class TestLiteDBPollConfigurationRepository : TestIPollConfigurationRepository
    {
        private MemoryStream databaseStream;
        private LiteDatabase database;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            LiteDBFunctions.DoLiteDBGlobalSetUp();
        }

        [SetUp]
        public void SetUp()
        {
            databaseStream = new MemoryStream();
            database = new LiteDatabase(databaseStream);
            repository = new LiteDBPollConfigurationRepository(database);
        }

        [TearDown]
        public void TearDown()
        {
            if (repository != null) repository.Dispose();
            if (database != null) database.Dispose();
            if (databaseStream != null) databaseStream.Dispose();
        }
    }
}
