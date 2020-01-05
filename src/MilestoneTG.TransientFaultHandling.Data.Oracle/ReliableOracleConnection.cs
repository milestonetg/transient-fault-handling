using Oracle.ManagedDataAccess.Client;
using System;
using System.Data.Common;

namespace MilestoneTG.TransientFaultHandling.Data.Oracle
{
    public class ReliableOracleConnection : ReliableDbConnection
    {
        //, RetryManager.Instance.GetDefaultOracleCommandRetryPolicy() ?? retryPolicy
        //GetDefaultOracleConnectionRetryPolicy

        /// <summary>
        /// Initializes a new instance of the <see cref="ReliableOracleConnection"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public ReliableOracleConnection(string connectionString)
            : this(connectionString, RetryManager.Instance.GetDefaultOracleConnectionRetryPolicy())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReliableOracleConnection"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="retryPolicy">The retry policy.</param>
        public ReliableOracleConnection(string connectionString, RetryPolicy retryPolicy)
            : this(connectionString, retryPolicy, RetryManager.Instance.GetDefaultOracleCommandRetryPolicy() ?? retryPolicy)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReliableOracleConnection"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string used to open the Oracle Database.</param>
        /// <param name="connectionRetryPolicy">The retry policy that defines whether to retry a request if a connection fails to be established.</param>
        /// <param name="commandRetryPolicy">The retry policy that defines whether to retry a request if a command fails to be executed.</param>
        public ReliableOracleConnection(string connectionString, RetryPolicy connectionRetryPolicy, RetryPolicy commandRetryPolicy)
            : base(connectionString, connectionRetryPolicy, commandRetryPolicy)
        {
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>ReliableDbConnection.</returns>
        protected override ReliableDbConnection Clone()
        {
            return new ReliableOracleConnection(this.ConnectionString, this.ConnectionRetryPolicy, this.CommandRetryPolicy);
        }

        /// <summary>
        /// Creates the connection.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>DbConnection.</returns>
        protected override DbConnection CreateConnection(string connectionString)
        {
            return new OracleConnection(connectionString);
        }

        /// <summary>
        /// Creates the connection string failover policy.
        /// </summary>
        /// <returns>RetryPolicy.</returns>
        protected override RetryPolicy CreateConnectionStringFailoverPolicy()
        {
            return new RetryPolicy<OracleDatabaseTransientErrorDetectionStrategy>(1, TimeSpan.FromMilliseconds(1));
        }
    }
}
