using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Data.Common;

namespace MilestoneTG.TransientFaultHandling.Data.MySql
{
    public class ReliableMySqlConnection : ReliableDbConnection
    {
        //, RetryManager.Instance.GetDefaultMySqlCommandRetryPolicy() ?? retryPolicy
        //GetDefaultMySqlConnectionRetryPolicy

        /// <summary>
        /// Initializes a new instance of the <see cref="ReliableMySqlConnection"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public ReliableMySqlConnection(string connectionString)
            : this(connectionString, RetryManager.Instance.GetDefaultMySqlConnectionRetryPolicy())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReliableMySqlConnection"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="retryPolicy">The retry policy.</param>
        public ReliableMySqlConnection(string connectionString, RetryPolicy retryPolicy)
            : this(connectionString, retryPolicy, RetryManager.Instance.GetDefaultMySqlCommandRetryPolicy() ?? retryPolicy)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReliableMySqlConnection"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string used to open the MySql Database.</param>
        /// <param name="connectionRetryPolicy">The retry policy that defines whether to retry a request if a connection fails to be established.</param>
        /// <param name="commandRetryPolicy">The retry policy that defines whether to retry a request if a command fails to be executed.</param>
        public ReliableMySqlConnection(string connectionString, RetryPolicy connectionRetryPolicy, RetryPolicy commandRetryPolicy)
            : base(connectionString, connectionRetryPolicy, commandRetryPolicy)
        {
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>ReliableDbConnection.</returns>
        protected override ReliableDbConnection Clone()
        {
            return new ReliableMySqlConnection(this.ConnectionString, this.ConnectionRetryPolicy, this.CommandRetryPolicy);
        }

        /// <summary>
        /// Creates the connection.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>DbConnection.</returns>
        protected override DbConnection CreateConnection(string connectionString)
        {
            return new MySqlConnection(connectionString);
        }

        /// <summary>
        /// Creates the connection string failover policy.
        /// </summary>
        /// <returns>RetryPolicy.</returns>
        protected override RetryPolicy CreateConnectionStringFailoverPolicy()
        {
            return new RetryPolicy<MySqlDatabaseTransientErrorDetectionStrategy>(1, TimeSpan.FromMilliseconds(1));
        }
    }
}
