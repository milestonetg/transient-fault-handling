using Npgsql;
using System.Threading.Tasks;

namespace MilestoneTG.TransientFaultHandling.Data.Postgres
{
    /// <summary>
    /// Provides a set of extension methods that add retry capabilities to the standard implementation.
    /// </summary>
    public static class NpgsqlConnectionExtensions
    {
        /// <summary>
        /// Opens a database connection with the connection settings specified in the ConnectionString property of the connection object.
        /// Uses the default retry policy when opening the connection.
        /// </summary>
        /// <param name="connection">The connection object that is required for the extension method declaration.</param>
        public static void OpenWithRetry(this NpgsqlConnection connection)
        {
            connection.OpenWithRetry(RetryManager.Instance.GetDefaultNpgsqlConnectionRetryPolicy());
        }

        /// <summary>
        /// Opens a database connection with the connection settings specified in the ConnectionString property of the connection object.
        /// Uses the default retry policy when opening the connection.
        /// </summary>
        /// <param name="connection">The connection object that is required for the extension method declaration.</param>
        public static Task OpenWithRetryAsync(this NpgsqlConnection connection)
        {
            return connection.OpenWithRetryAsync(RetryManager.Instance.GetDefaultNpgsqlConnectionRetryPolicy());
        }
    }
}
