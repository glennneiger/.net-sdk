﻿namespace DreamFactory.Tests.Api
{
    using System.Collections.Generic;
    using System.Linq;
    using DreamFactory.Api;
    using DreamFactory.Api.Implementation;
    using DreamFactory.Http;
    using DreamFactory.Model.Builder;
    using DreamFactory.Model.Database;
    using DreamFactory.Rest;
    using DreamFactory.Serialization;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Shouldly;

    [TestClass]
    public class DatabaseApiTests
    {
        private const string BaseAddress = "http://localhost";

        #region --- Schema ---

        [TestMethod]
        public void ShouldGetAccessComponentsAsync()
        {
            // Arrange
            IDatabaseApi databaseApi = CreateDatabaseApi();

            // Act
            List<TableInfo> result = databaseApi.GetAccessComponentsAsync().Result.ToList();

            // Assert
            result.Count.ShouldBe(3);
            result.Any(x => x.Name == "todo").ShouldBe(true);
        }

        [TestMethod]
        public void ShouldGetTableNamesAsync()
        {
            // Arrange
            IDatabaseApi databaseApi = CreateDatabaseApi("alt");

            // Act
            List<string> names = databaseApi.GetTableNamesAsync(true).Result.ToList();

            // Assert
            names.Count.ShouldBe(7);
            names.ShouldContain("todo");
            names.ShouldContain("_schema/todo");
        }

        [TestMethod]
        public void ShouldCreateTableAsync()
        {
            // Arrange
            IDatabaseApi databaseApi = CreateDatabaseApi();
            TableSchema schema = CreateTestTableSchema();

            // Act & Assert
            databaseApi.CreateTableAsync(schema).Wait();
        }

        [TestMethod]
        public void ShouldUpdateTableAsync()
        {
            // Arrange
            IDatabaseApi databaseApi = CreateDatabaseApi();
            TableSchema schema = CreateTestTableSchema();

            // Act & Assert
            databaseApi.UpdateTableAsync(schema).Wait();
        }

        [TestMethod]
        public void ShouldDeleteTableAsync()
        {
            // Arrange
            IDatabaseApi databaseApi = CreateDatabaseApi();

            // Act
            bool success = databaseApi.DeleteTableAsync("staff").Result;

            // Assert
            success.ShouldBe(true);
        }

        [TestMethod]
        public void ShouldDescribeTableAsync()
        {
            // Arrange
            IDatabaseApi databaseApi = CreateDatabaseApi();

            // Act
            TableSchema schema = databaseApi.DescribeTableAsync("staff").Result;

            // Assert
            schema.Name.ShouldBe("staff");
        }

        [TestMethod]
        public void ShouldDescribeFieldAsync()
        {
            // Arrange
            IDatabaseApi databaseApi = CreateDatabaseApi();

            // Act
            FieldSchema schema = databaseApi.DescribeFieldAsync("staff", "field").Result;

            // Assert
            schema.Name.ShouldBe("field");
        }

        #endregion

        #region --- Records ---

        [TestMethod]
        public void ShouldCreateRecordsAsync()
        {
            // Arrange
            IDatabaseApi databaseApi = CreateDatabaseApi();
            IEnumerable<StaffRecord> records = CreateStaffRecords();
            SqlQuery query = new SqlQuery { Fields = "*" };

            // Act
            List<StaffRecord> created = databaseApi.CreateRecordsAsync("staff", records, query).Result.ToList();

            // Assert
            created.Count.ShouldBe(3);
            created.First().Uid.ShouldBe(1);
            created.Last().Uid.ShouldBe(3);
        }

        [TestMethod]
        public void ShouldUpdateRecordsAsync()
        {
            // Arrange
            IDatabaseApi databaseApi = CreateDatabaseApi();
            IEnumerable<StaffRecord> records = CreateStaffRecords().Skip(1);

            // Act & Assert
            databaseApi.UpdateRecordsAsync("staff", records).Wait();
        }

        [TestMethod]
        public void ShouldGetRecordsAsync()
        {
            // Arrange
            IDatabaseApi databaseApi = CreateDatabaseApi();
            SqlQuery query = new SqlQuery { Fields = "*" };

            // Act
            List<StaffRecord> records = databaseApi.GetRecordsAsync<StaffRecord>("staff", query).Result.ToList();

            // Assert
            records.Count.ShouldBe(3);
            records.First().Uid.ShouldBe(1);
            records.Last().Uid.ShouldBe(3);
        }

        [TestMethod]
        public void ShouldDeleteRecordsAsync()
        {
            // Arrange
            IDatabaseApi databaseApi = CreateDatabaseApi();
            IEnumerable<StaffRecord> records = CreateStaffRecords().Take(1);

            // Act & Assert
            databaseApi.DeleteRecordsAsync("staff", records).Wait();
        }

        #endregion

        #region --- Stored ---

        [TestMethod]
        public void ShouldGetStoredProcNamesAsync()
        {
            // Arrange
            IDatabaseApi databaseApi = CreateDatabaseApi();

            // Act
            List<string> names = databaseApi.GetStoredProcNamesAsync().Result.ToList();

            // Assert
            names.Count.ShouldBe(2);
            names.First().ShouldBe("foo");
        }

        [TestMethod]
        public void ShouldGetStoredFuncNamesAsync()
        {
            // Arrange
            IDatabaseApi databaseApi = CreateDatabaseApi();

            // Act
            List<string> names = databaseApi.GetStoredFuncNamesAsync().Result.ToList();

            // Assert
            names.Count.ShouldBe(2);
            names.First().ShouldBe("bar");
        }

        [TestMethod]
        public void ShouldCallStoredProcAsync()
        {
            // Arrange
            IDatabaseApi databaseApi = CreateDatabaseApi();
            IStoreProcParamsBuilder builder =
                new StoreProcParamsBuilder()
                    .WithOutParam<string>("bar")
                    .WithOutParam<int>("foo");
            StoredProcParam[] parameters = builder.Build();

            // Act
            ProcResponse result = databaseApi.CallStoredProcAsync<ProcResponse>("foo", "dataset", parameters).Result;

            // Assert
            result.Foo.ShouldBe(123);
            result.Bar.ShouldBe("test");
            result.Dataset.Count.ShouldBe(2);
            result.Dataset.Any(x => x.FirstName == "Selena").ShouldBe(true);
        }

        [TestMethod]
        public void ShouldCallStoredFuncAsync()
        {
            // Arrange
            IDatabaseApi databaseApi = CreateDatabaseApi();
            IStoreProcParamsBuilder builder =
                new StoreProcParamsBuilder()
                    .WithOutParam<string>("bar")
                    .WithOutParam<int>("foo");
            StoredProcParam[] parameters = builder.Build();

            // Act
            ProcResponse result = databaseApi.CallStoredFuncAsync<ProcResponse>("foo", "dataset", parameters).Result;

            // Assert
            result.Foo.ShouldBe(123);
            result.Bar.ShouldBe("test");
            result.Dataset.Count.ShouldBe(2);
            result.Dataset.Any(x => x.FirstName == "Selena").ShouldBe(true);
        }

        #endregion

        private static IDatabaseApi CreateDatabaseApi(string alt = null)
        {
            IHttpFacade httpFacade = new TestDataHttpFacade(alt);
            HttpAddress address = new HttpAddress(BaseAddress, RestApiVersion.V1);
            HttpHeaders headers = new HttpHeaders();
            return new DatabaseApi(address, httpFacade, new JsonContentSerializer(), headers, "db");
        }

        private static TableSchema CreateTestTableSchema()
        {
            ITableSchemaBuilder builder = new TableSchemaBuilder();
            return builder.WithName("staff").WithFieldsFrom<StaffRecord>().WithKeyField("uid").Build();
        }

        private static IEnumerable<StaffRecord> CreateStaffRecords()
        {
            yield return new StaffRecord { FirstName = "Andrei", LastName = "Smirnov", Age = 35, Active = true };
            yield return new StaffRecord { FirstName = "Mike", LastName = "Meyers", Age = 33, Active = false };
            yield return new StaffRecord { FirstName = "Selena", LastName = "Gomez", Age = 24, Active = false };
        }

        internal class StaffRecord
        {
            public int Uid { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public int Age { get; set; }
            public bool Active { get; set; }
        }

        internal class ProcResponse
        {
            public List<StaffRecord> Dataset { get; set; }
            public int Foo { get; set; }
            public string Bar { get; set; }
        }
    }
}