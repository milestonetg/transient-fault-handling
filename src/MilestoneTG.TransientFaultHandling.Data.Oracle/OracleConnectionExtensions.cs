using Oracle.ManagedDataAccess.Client;
using System.Threading.Tasks;

namespace MilestoneTG.TransientFaultHandling.Data.Oracle
{
    /// <summary>
    /// Provides a set of extension methods that add retry capabilities to the standard <see cref="System.Data.SqlClient.SqlConnection"/> implementation.
    /// </summary>
    public static class OracleConnectionExtensions
    {
        /// <summary>
        /// Opens a database connection with the connection settings specified in the ConnectionString property of the connection object.
        /// Uses the default retry policy when opening the connection.
        /// </summary>
        /// <param name="connection">The connection object that is required for the extension method declaration.</param>
        public static void OpenWithRetry(this OracleConnection connection)
        {
            connection.OpenWithRetry(RetryManager.Instance.GetDefaultOracleConnectionRetryPolicy());
        }

        /// <summary>
        /// Opens a database connection with the connection settings specified in the ConnectionString property of the connection object.
        /// Uses the default retry policy when opening the connection.
        /// </summary>
        /// <param name="connection">The connection object that is required for the extension method declaration.</param>
        public static Task OpenWithRetryAsync(this OracleConnection connection)
        {
            return connection.OpenWithRetryAsync(RetryManager.Instance.GetDefaultOracleConnectionRetryPolicy());
        }

    }
}
