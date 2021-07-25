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
    class TestLiteDBPollConfigurationRepositoryApplyEvent : TestIPollConfigurationRepositoryApplyEvent
    {
        private LiteDatabase database;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            LiteDBFunctions.DoLiteDBGlobalSetUp();
        }

        [SetUp]
        public void SetUp()
        {
            database = new LiteDatabase(":memory:");
            repository = new LiteDBPollConfigurationRepository(database);
        }

        [TearDown]
        public void TearDown()
        {
            repository?.Dispose();
            database?.Dispose();
        }
    }
}
