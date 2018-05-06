using MilestoneTG.TransientFaultHandling.Data.SqlServer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;

namespace MilestoneTG.NHibernate.TransientFaultHandling.SqlServer
{
    /// <summary>
    /// Wrap <see cref="ReliableSqlConnection" /> in a class that extends <see cref="DbConnection" />
    /// so internal type casts within NHibernate don't fail.
    /// </summary>
    /// <seealso cref="System.Data.Common.DbConnection" />
    public class ReliableSqlDbConnection : DbConnection
    {
        /// <summary>
        /// The disposed
        /// </summary>
        bool disposed = false;
        /// <summary>
        /// The underlying <see cref="ReliableSqlConnection" />.
        /// </summary>
        /// <value>The reliable connection.</value>
        public ReliableSqlConnection ReliableConnection { get; set; }

        /// <summary>
        /// Constructs a <see cref="ReliableSqlDbConnection" /> to wrap around the given <see cref="ReliableSqlConnection" />.
        /// </summary>
        /// <param name="connection">The <see cref="ReliableSqlConnection" /> to wrap</param>
        public ReliableSqlDbConnection(ReliableSqlConnection connection)
        {
            ReliableConnection = connection;
        }

        /// <summary>
        /// Explicit type-casting between <see cref="ReliableSqlDbConnection" /> and <see cref="ReliableSqlConnection" />.
        /// </summary>
        /// <param name="connection">The <see cref="ReliableSqlDbConnection" /> being casted</param>
        /// <returns>The underlying <see cref="ReliableSqlConnection" /></returns>
        public static explicit operator SqlConnection(ReliableSqlDbConnection connection)
        {
            return connection.ReliableConnection.Current as SqlConnection;
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="T:System.ComponentModel.Component"></see> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposed)
                return;
            if (disposing)
            {
                ReliableConnection.Dispose();
            }
            disposed = true;
            base.Dispose(disposing);
        }

        #region Wrapping code
        /// <summary>
        /// Starts a database transaction.
        /// </summary>
        /// <param name="isolationLevel">Specifies the isolation level for the transaction.</param>
        /// <returns>An object representing the new transaction.</returns>
        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            return (DbTransaction)ReliableConnection.BeginTransaction(isolationLevel);
        }

        /// <summary>
        /// Closes the connection to the database. This is the preferred method of closing any open connection.
        /// </summary>
        public override void Close()
        {
            ReliableConnection.Close();
        }

        /// <summary>
        /// Returns schema information for the data source of this <see cref="T:System.Data.Common.DbConnection"></see>.
        /// </summary>
        /// <returns>A <see cref="T:System.Data.DataTable"></see> that contains schema information.</returns>
        public override DataTable GetSchema()
        {
            return ReliableConnection.ConnectionRetryPolicy.ExecuteAction(
                () => ReliableConnection.Current.GetSchema()
            );
        }

        /// <summary>
        /// Returns schema information for the data source of this <see cref="T:System.Data.Common.DbConnection"></see> using the specified string for the schema name.
        /// </summary>
        /// <param name="collectionName">Specifies the name of the schema to return.</param>
        /// <returns>A <see cref="T:System.Data.DataTable"></see> that contains schema information.</returns>
        public override DataTable GetSchema(string collectionName)
        {
            return ReliableConnection.ConnectionRetryPolicy.ExecuteAction(
                () => ReliableConnection.Current.GetSchema(collectionName)
            );
        }

        /// <summary>
        /// Returns schema information for the data source of this <see cref="T:System.Data.Common.DbConnection"></see> using the specified string for the schema name and the specified string array for the restriction values.
        /// </summary>
        /// <param name="collectionName">Specifies the name of the schema to return.</param>
        /// <param name="restrictionValues">Specifies a set of restriction values for the requested schema.</param>
        /// <returns>A <see cref="T:System.Data.DataTable"></see> that contains schema information.</returns>
        public override DataTable GetSchema(string collectionName, string[] restrictionValues)
        {
            return ReliableConnection.ConnectionRetryPolicy.ExecuteAction(
                () => ReliableConnection.Current.GetSchema(collectionName, restrictionValues)
            );
        }

        /// <summary>
        /// Changes the current database for an open connection.
        /// </summary>
        /// <param name="databaseName">Specifies the name of the database for the connection to use.</param>
        public override void ChangeDatabase(string databaseName)
        {
            ReliableConnection.ChangeDatabase(databaseName);
        }

        /// <summary>
        /// Creates and returns a <see cref="T:System.Data.Common.DbCommand"></see> object associated with the current connection.
        /// </summary>
        /// <returns>A <see cref="T:System.Data.Common.DbCommand"></see> object.</returns>
        protected override DbCommand CreateDbCommand()
        {
            return ReliableConnection.CreateCommand();
        }

        /// <summary>
        /// Opens a database connection with the settings specified by the <see cref="P:System.Data.Common.DbConnection.ConnectionString"></see>.
        /// </summary>
        public override void Open()
        {
            ReliableConnection.Open();
        }

        /// <summary>
        /// Gets or sets the string used to open the connection.
        /// </summary>
        /// <value>The connection string.</value>
        public override string ConnectionString { get { return ReliableConnection.ConnectionString; } set { ReliableConnection.ConnectionString = value; } }
        /// <summary>
        /// Gets the time to wait while establishing a connection before terminating the attempt and generating an error.
        /// </summary>
        /// <value>The connection timeout.</value>
        public override int ConnectionTimeout { get { return ReliableConnection.ConnectionTimeout; } }
        /// <summary>
        /// Gets the name of the current database after a connection is opened, or the database name specified in the connection string before the connection is opened.
        /// </summary>
        /// <value>The database.</value>
        public override string Database { get { return ReliableConnection.Database; } }
        /// <summary>
        /// Gets the name of the database server to which to connect.
        /// </summary>
        /// <value>The data source.</value>
        public override string DataSource { get { return ""; } }
        /// <summary>
        /// Gets a string that represents the version of the server to which the object is connected.
        /// </summary>
        /// <value>The server version.</value>
        public override string ServerVersion { get { return ""; } }
        /// <summary>
        /// Gets a string that describes the state of the connection.
        /// </summary>
        /// <value>The state.</value>
        public override ConnectionState State { get { return ReliableConnection.State; } }
        #endregion
    }
}
