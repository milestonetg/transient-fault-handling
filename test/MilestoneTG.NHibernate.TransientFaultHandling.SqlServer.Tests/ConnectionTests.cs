using System.Data;
using System.Data.SqlClient;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate.Driver;
using NHibernate.SqlAzure.Tests.Config;

namespace NHibernate.SqlAzure.Tests
{
    [TestClass]
    class WhenConnectingLocalTestingSqlAzureClientDriverShould : ConnectionTests<LocalTestingReliableSql2008ClientDriver>
    {
        [TestMethod]
        public void Establish_connection_during_temporary_shutdown_of_sql_server()
        {
            TestConnectionEstablishment();
        }
    }

    [TestClass]
    class WhenConnectingSql2008ClientDriverShould : ConnectionTests<Sql2008ClientDriver>
    {
        [TestMethod]
        [ExpectedException(typeof(SqlException), AllowDerivedTypes = true)]
        public void Fail_to_establish_connection_during_temporary_shutdown_of_sql_server()
        {
            TestConnectionEstablishment();
        }
    }

    abstract class ConnectionTests<T> : NonPooledNHibernateTestBase<T>
        where T : SqlClientDriver
    {
        protected void TestConnectionEstablishment()
        {
            using (TemporarilyShutdownSqlServerExpress())
            {
                for (var i = 0; i < 1000; i++)
                {
                    using (var session = CreateSession())
                    {
                        Assert.IsTrue(session.Connection.State == ConnectionState.Open, "Connection state not open.");
                        Thread.Sleep(1);
                    }
                }
            }
        }
    }
}
