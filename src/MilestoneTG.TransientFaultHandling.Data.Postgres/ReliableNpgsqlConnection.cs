using Npgsql;
using System;
using System.Data;
using System.Data.Common;

namespace MilestoneTG.TransientFaultHandling.Data.Postgres
{
    public class ReliableNpgsqlConnection : ReliableDbConnection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReliableNpgsqlConnection"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public ReliableNpgsqlConnection(string connectionString)
            : this(connectionString, RetryManager.Instance.GetDefaultNpgsqlConnectionRetryPolicy())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReliableNpgsqlConnection"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="retryPolicy">The retry policy.</param>
        public ReliableNpgsqlConnection(string connectionString, RetryPolicy retryPolicy)
            : this(connectionString, retryPolicy, RetryManager.Instance.GetDefaultNpgsqlCommandRetryPolicy() ?? retryPolicy)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReliableNpgsqlConnection"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string used to open the Npgsql Database.</param>
        /// <param name="connectionRetryPolicy">The retry policy that defines whether to retry a request if a connection fails to be established.</param>
        /// <param name="commandRetryPolicy">The retry policy that defines whether to retry a request if a command fails to be executed.</param>
        public ReliableNpgsqlConnection(string connectionString, RetryPolicy connectionRetryPolicy, RetryPolicy commandRetryPolicy)
            : base(connectionString, connectionRetryPolicy, commandRetryPolicy)
        {
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>ReliableDbConnection.</returns>
        protected override ReliableDbConnection Clone()
        {
            return new ReliableNpgsqlConnection(this.ConnectionString, this.ConnectionRetryPolicy, this.CommandRetryPolicy);
        }

        /// <summary>
        /// Creates the connection.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>DbConnection.</returns>
        protected override DbConnection CreateConnection(string connectionString)
        {
            return new NpgsqlConnection(connectionString);
        }

        /// <summary>
        /// Creates the connection string failover policy.
        /// </summary>
        /// <returns>RetryPolicy.</returns>
        protected override RetryPolicy CreateConnectionStringFailoverPolicy()
        {
            return new RetryPolicy<PostgreSqlDatabaseTransientErrorDetectionStrategy>(1, TimeSpan.FromMilliseconds(1));
        }
    }
}
