using MySql.Data.MySqlClient;
using System.Data;
using System.Threading.Tasks;

namespace MilestoneTG.TransientFaultHandling.Data.MySql
{
    /// <summary>
    /// Provides a set of extension methods that add retry capabilities to the standard System.Data.SqlClient.SqlCommand implementation.
    /// </summary>
    public static class MySqlCommandExtensions
    {
        #region ExecuteNonQueryWithRetry method implementations
        /// <summary>
        /// Executes a Transact-MySql statement against the connection and returns the number of rows affected. Uses the default retry policy when executing the command.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <returns>The number of rows affected.</returns>
        public static int ExecuteNonQueryWithRetry(this MySqlCommand command)
        {
            return command.ExecuteNonQueryWithRetry(RetryManager.Instance.GetDefaultMySqlCommandRetryPolicy());
        }

        /// <summary>
        /// Executes a Transact-MySql statement against the connection and returns the number of rows affected. Uses the default retry policy when executing the command.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <returns>The number of rows affected.</returns>
        public static Task<int> ExecuteNonQueryWithRetryAsync(this MySqlCommand command)
        {
            return command.ExecuteNonQueryWithRetryAsync(RetryManager.Instance.GetDefaultMySqlCommandRetryPolicy());
        }

        #endregion

        #region ExecuteReaderWithRetry method implementations
        /// <summary>
        /// Sends the specified command to the connection and builds a MySqlDataReader object that contains the results.
        /// Uses the default retry policy when executing the command.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <returns>A System.Data.MySqlClient.MySqlDataReader object.</returns>
        public static MySqlDataReader ExecuteReaderWithRetry(this MySqlCommand command)
        {
            return (MySqlDataReader)command.ExecuteReaderWithRetry(RetryManager.Instance.GetDefaultMySqlCommandRetryPolicy());
        }

        /// <summary>
        /// Sends the specified command to the connection and builds a MySqlDataReader object that contains the results.
        /// Uses the default retry policy when executing the command.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <returns>A System.Data.MySqlClient.MySqlDataReader object.</returns>
        public static async Task<MySqlDataReader> ExecuteReaderWithRetryAsync(this MySqlCommand command)
        {
            return (MySqlDataReader)await command.ExecuteReaderWithRetryAsync(RetryManager.Instance.GetDefaultMySqlCommandRetryPolicy());
        }

        /// <summary>
        /// Sends the specified command to the connection and builds a MySqlDataReader object by using the specified 
        /// command behavior. Uses the default retry policy when executing the command.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <param name="behavior">One of the enumeration values that specifies the command behavior.</param>
        /// <returns>A System.Data.MySqlClient.MySqlDataReader object.</returns>
        public static MySqlDataReader ExecuteReaderWithRetry(this MySqlCommand command, CommandBehavior behavior)
        {
            return (MySqlDataReader)command.ExecuteReaderWithRetry(behavior, RetryManager.Instance.GetDefaultMySqlCommandRetryPolicy());
        }

        /// <summary>
        /// Sends the specified command to the connection and builds a MySqlDataReader object by using the specified 
        /// command behavior. Uses the default retry policy when executing the command.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <param name="behavior">One of the enumeration values that specifies the command behavior.</param>
        /// <returns>A System.Data.MySqlClient.MySqlDataReader object.</returns>
        public static async Task<MySqlDataReader> ExecuteReaderWithRetryAsync(this MySqlCommand command, CommandBehavior behavior)
        {
            return (MySqlDataReader)await command.ExecuteReaderWithRetryAsync(behavior, RetryManager.Instance.GetDefaultMySqlCommandRetryPolicy());
        }

        #endregion

        #region ExecuteScalarWithRetry method implementations

        /// <summary>
        /// Executes the query, and returns the first column of the first row in the result set returned by the query. Additional columns or rows are ignored.
        /// Uses the default retry policy when executing the command.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <returns> The first column of the first row in the result set, or a null reference if the result set is empty. Returns a maximum of 2033 characters.</returns>
        public static object ExecuteScalarWithRetry(this MySqlCommand command)
        {
            return command.ExecuteScalarWithRetry(RetryManager.Instance.GetDefaultMySqlCommandRetryPolicy());
        }

        /// <summary>
        /// Executes the query, and returns the first column of the first row in the result set returned by the query. Additional columns or rows are ignored.
        /// Uses the default retry policy when executing the command.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <returns> The first column of the first row in the result set, or a null reference if the result set is empty. Returns a maximum of 2033 characters.</returns>
        public static async Task<object> ExecuteScalarWithRetryAsync(this MySqlCommand command)
        {
            return await command.ExecuteScalarWithRetryAsync(RetryManager.Instance.GetDefaultMySqlCommandRetryPolicy());
        }

        #endregion
    }
}
