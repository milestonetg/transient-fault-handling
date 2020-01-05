using System.Data.Common;
using System.Threading.Tasks;

namespace MilestoneTG.TransientFaultHandling.Data
{
    /// <summary>
    /// DbConnection Extension methods.
    /// </summary>
    public static class DbConnectionExtensions
    {

        /// <summary>
        /// Opens a database connection with the connection settings specified in the ConnectionString property of the connection object.
        /// Uses the specified retry policy when opening the connection.
        /// </summary>
        /// <param name="connection">The connection object that is required for the extension method declaration.</param>
        /// <param name="retryPolicy">The retry policy that defines whether to retry a request if the connection fails.</param>
        public static void OpenWithRetry(this DbConnection connection, RetryPolicy retryPolicy)
        {
            // Check if retry policy was specified, if not, use the default retry policy.
            (retryPolicy != null ? retryPolicy : RetryPolicy.NoRetry).ExecuteAction(connection.Open);
        }

        /// <summary>
        /// Opens a database connection with the connection settings specified in the ConnectionString property of the connection object.
        /// Uses the specified retry policy when opening the connection.
        /// </summary>
        /// <param name="connection">The connection object that is required for the extension method declaration.</param>
        /// <param name="retryPolicy">The retry policy that defines whether to retry a request if the connection fails.</param>
        public static async Task OpenWithRetryAsync(this DbConnection connection, RetryPolicy retryPolicy)
        {
            // Check if retry policy was specified, if not, use the default retry policy.
            await (retryPolicy != null ? retryPolicy : RetryPolicy.NoRetry).ExecuteAsync(connection.OpenAsync);
        }
    }
}
