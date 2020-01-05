using MilestoneTG.TransientFaultHandling;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace MilestoneTG.TransientFaultHandling.Data.SqlServer
{
    /// <summary>
    /// Implements transient fault handling for Microsoft SQL Server Client connections.
    /// </summary>
    /// <seealso cref="MilestoneTG.TransientFaultHandling.Data.ReliableDbConnection" />
    public class ReliableSqlConnection : ReliableDbConnection
    {
        //, RetryManager.Instance.GetDefaultSqlCommandRetryPolicy() ?? retryPolicy
        //GetDefaultSqlConnectionRetryPolicy

        /// <summary>
        /// Initializes a new instance of the <see cref="ReliableSqlConnection"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public ReliableSqlConnection(string connectionString) 
            : this(connectionString, RetryManager.Instance.GetDefaultSqlConnectionRetryPolicy())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReliableSqlConnection"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="retryPolicy">The retry policy.</param>
        public ReliableSqlConnection(string connectionString, RetryPolicy retryPolicy) 
            : this(connectionString, retryPolicy, RetryManager.Instance.GetDefaultSqlCommandRetryPolicy() ?? retryPolicy)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReliableSqlConnection"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string used to open the SQL Database.</param>
        /// <param name="connectionRetryPolicy">The retry policy that defines whether to retry a request if a connection fails to be established.</param>
        /// <param name="commandRetryPolicy">The retry policy that defines whether to retry a request if a command fails to be executed.</param>
        public ReliableSqlConnection(string connectionString, RetryPolicy connectionRetryPolicy, RetryPolicy commandRetryPolicy) 
            : base(connectionString, connectionRetryPolicy, commandRetryPolicy)
        {
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>ReliableDbConnection.</returns>
        protected override ReliableDbConnection Clone()
        {
            return new ReliableSqlConnection(this.ConnectionString, this.ConnectionRetryPolicy, this.CommandRetryPolicy);
        }

        /// <summary>
        /// Creates the connection.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>DbConnection.</returns>
        protected override DbConnection CreateConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }

        /// <summary>
        /// Creates the connection string failover policy.
        /// </summary>
        /// <returns>RetryPolicy.</returns>
        protected override RetryPolicy CreateConnectionStringFailoverPolicy()
        {
            return new RetryPolicy<SqlServerNetworkConnectivityErrorDetectionStrategy>(1, TimeSpan.FromMilliseconds(1));
        }

        /// <summary>
        /// Executes the XML reader.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command">The command.</param>
        /// <param name="behavior">The behavior.</param>
        /// <param name="closeOpenedConnectionOnSuccess">if set to <c>true</c> [close opened connection on success].</param>
        /// <returns>T.</returns>
        /// <exception cref="System.NotSupportedException"></exception>
        protected override T ExecuteXmlReader<T>(IDbCommand command, CommandBehavior behavior, bool closeOpenedConnectionOnSuccess)
        {
            if (command is SqlCommand)
            {
                object result;
                var xmlReader = (command as SqlCommand).ExecuteXmlReader();

                closeOpenedConnectionOnSuccess = false;

                if ((behavior & CommandBehavior.CloseConnection) == CommandBehavior.CloseConnection)
                {
                    // Implicit conversion from XmlReader to <T> via an intermediary object.
                    result = new SqlXmlReader(command.Connection, xmlReader);
                }
                else
                {
                    // Implicit conversion from XmlReader to <T> via an intermediary object.
                    result = xmlReader;
                }

                return (T)result;
            }

            throw new NotSupportedException();
        }

        /// <summary>
        /// Execute XML reader as an asynchronous operation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command">The command.</param>
        /// <param name="behavior">The behavior.</param>
        /// <param name="closeOpenedConnectionOnSuccess">if set to <c>true</c> [close opened connection on success].</param>
        /// <returns>T.</returns>
        /// <exception cref="System.NotSupportedException"></exception>
        protected override async Task<T> ExecuteXmlReaderAsync<T>(IDbCommand command, CommandBehavior behavior, bool closeOpenedConnectionOnSuccess)
        {
            if (command is SqlCommand)
            {
                object result;
                var xmlReader = await (command as SqlCommand).ExecuteXmlReaderAsync();

                closeOpenedConnectionOnSuccess = false;

                if ((behavior & CommandBehavior.CloseConnection) == CommandBehavior.CloseConnection)
                {
                    // Implicit conversion from XmlReader to <T> via an intermediary object.
                    result = new SqlXmlReader(command.Connection, xmlReader);
                }
                else
                {
                    // Implicit conversion from XmlReader to <T> via an intermediary object.
                    result = xmlReader;
                }

                return (T)result;
            }

            throw new NotSupportedException();
        }
    }
}
