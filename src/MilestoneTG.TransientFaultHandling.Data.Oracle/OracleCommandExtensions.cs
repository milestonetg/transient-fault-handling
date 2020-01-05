using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Threading.Tasks;

namespace MilestoneTG.TransientFaultHandling.Data.Oracle
{
    /// <summary>
    /// Provides a set of extension methods that add retry capabilities to the standard System.Data.SqlClient.SqlCommand implementation.
    /// </summary>
    public static class OracleCommandExtensions
    {
        #region ExecuteNonQueryWithRetry method implementations
        /// <summary>
        /// Executes a Transact-Oracle statement against the connection and returns the number of rows affected. Uses the default retry policy when executing the command.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <returns>The number of rows affected.</returns>
        public static int ExecuteNonQueryWithRetry(this OracleCommand command)
        {
            return command.ExecuteNonQueryWithRetry(RetryManager.Instance.GetDefaultOracleCommandRetryPolicy());
        }

        /// <summary>
        /// Executes a Transact-Oracle statement against the connection and returns the number of rows affected. Uses the default retry policy when executing the command.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <returns>The number of rows affected.</returns>
        public static Task<int> ExecuteNonQueryWithRetryAsync(this OracleCommand command)
        {
            return command.ExecuteNonQueryWithRetryAsync(RetryManager.Instance.GetDefaultOracleCommandRetryPolicy());
        }

        #endregion

        #region ExecuteReaderWithRetry method implementations
        /// <summary>
        /// Sends the specified command to the connection and builds a OracleDataReader object that contains the results.
        /// Uses the default retry policy when executing the command.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <returns>A System.Data.OracleClient.OracleDataReader object.</returns>
        public static OracleDataReader ExecuteReaderWithRetry(this OracleCommand command)
        {
            return (OracleDataReader)command.ExecuteReaderWithRetry(RetryManager.Instance.GetDefaultOracleCommandRetryPolicy());
        }

        /// <summary>
        /// Sends the specified command to the connection and builds a OracleDataReader object that contains the results.
        /// Uses the default retry policy when executing the command.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <returns>A System.Data.OracleClient.OracleDataReader object.</returns>
        public static async Task<OracleDataReader> ExecuteReaderWithRetryAsync(this OracleCommand command)
        {
            return (OracleDataReader)await command.ExecuteReaderWithRetryAsync(RetryManager.Instance.GetDefaultOracleCommandRetryPolicy());
        }

        /// <summary>
        /// Sends the specified command to the connection and builds a OracleDataReader object by using the specified 
        /// command behavior. Uses the default retry policy when executing the command.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <param name="behavior">One of the enumeration values that specifies the command behavior.</param>
        /// <returns>A System.Data.OracleClient.OracleDataReader object.</returns>
        public static OracleDataReader ExecuteReaderWithRetry(this OracleCommand command, CommandBehavior behavior)
        {
            return (OracleDataReader)command.ExecuteReaderWithRetry(behavior, RetryManager.Instance.GetDefaultOracleCommandRetryPolicy());
        }

        /// <summary>
        /// Sends the specified command to the connection and builds a OracleDataReader object by using the specified 
        /// command behavior. Uses the default retry policy when executing the command.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <param name="behavior">One of the enumeration values that specifies the command behavior.</param>
        /// <returns>A System.Data.OracleClient.OracleDataReader object.</returns>
        public static async Task<OracleDataReader> ExecuteReaderWithRetryAsync(this OracleCommand command, CommandBehavior behavior)
        {
            return (OracleDataReader)await command.ExecuteReaderWithRetryAsync(behavior, RetryManager.Instance.GetDefaultOracleCommandRetryPolicy());
        }

        #endregion

        #region ExecuteScalarWithRetry method implementations

        /// <summary>
        /// Executes the query, and returns the first column of the first row in the result set returned by the query. Additional columns or rows are ignored.
        /// Uses the default retry policy when executing the command.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <returns> The first column of the first row in the result set, or a null reference if the result set is empty. Returns a maximum of 2033 characters.</returns>
        public static object ExecuteScalarWithRetry(this OracleCommand command)
        {
            return command.ExecuteScalarWithRetry(RetryManager.Instance.GetDefaultOracleCommandRetryPolicy());
        }

        /// <summary>
        /// Executes the query, and returns the first column of the first row in the result set returned by the query. Additional columns or rows are ignored.
        /// Uses the default retry policy when executing the command.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <returns> The first column of the first row in the result set, or a null reference if the result set is empty. Returns a maximum of 2033 characters.</returns>
        public static async Task<object> ExecuteScalarWithRetryAsync(this OracleCommand command)
        {
            return await command.ExecuteScalarWithRetryAsync(RetryManager.Instance.GetDefaultOracleCommandRetryPolicy());
        }

        #endregion
    }
}
