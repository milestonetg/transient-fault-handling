using System;
using System.Data;
using System.Threading.Tasks;
using System.Xml;
using MilestoneTG.TransientFaultHandling.Data;
using Npgsql;

namespace MilestoneTG.TransientFaultHandling.Data.Postgres
{
    /// <summary>
    /// Provides a set of extension methods that add retry capabilities to the standard System.Data.SqlClient.SqlCommand implementation.
    /// </summary>
    public static class NpgsqlCommandExtensions
    {
        #region ExecuteNonQueryWithRetry method implementations
        /// <summary>
        /// Executes a Transact-Npgsql statement against the connection and returns the number of rows affected. Uses the default retry policy when executing the command.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <returns>The number of rows affected.</returns>
        public static int ExecuteNonQueryWithRetry(this NpgsqlCommand command)
        {
            return command.ExecuteNonQueryWithRetry(RetryManager.Instance.GetDefaultNpgsqlCommandRetryPolicy());
        }

        /// <summary>
        /// Executes a Transact-Npgsql statement against the connection and returns the number of rows affected. Uses the default retry policy when executing the command.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <returns>The number of rows affected.</returns>
        public static Task<int> ExecuteNonQueryWithRetryAsync(this NpgsqlCommand command)
        {
            return command.ExecuteNonQueryWithRetryAsync(RetryManager.Instance.GetDefaultNpgsqlCommandRetryPolicy());
        }

        #endregion

        #region ExecuteReaderWithRetry method implementations
        /// <summary>
        /// Sends the specified command to the connection and builds a NpgsqlDataReader object that contains the results.
        /// Uses the default retry policy when executing the command.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <returns>A System.Data.NpgsqlClient.NpgsqlDataReader object.</returns>
        public static NpgsqlDataReader ExecuteReaderWithRetry(this NpgsqlCommand command)
        {
            return (NpgsqlDataReader)command.ExecuteReaderWithRetry(RetryManager.Instance.GetDefaultNpgsqlCommandRetryPolicy());
        }

        /// <summary>
        /// Sends the specified command to the connection and builds a NpgsqlDataReader object that contains the results.
        /// Uses the default retry policy when executing the command.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <returns>A System.Data.NpgsqlClient.NpgsqlDataReader object.</returns>
        public static async Task<NpgsqlDataReader> ExecuteReaderWithRetryAsync(this NpgsqlCommand command)
        {
            return (NpgsqlDataReader)await command.ExecuteReaderWithRetryAsync(RetryManager.Instance.GetDefaultNpgsqlCommandRetryPolicy());
        }

        /// <summary>
        /// Sends the specified command to the connection and builds a NpgsqlDataReader object by using the specified 
        /// command behavior. Uses the default retry policy when executing the command.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <param name="behavior">One of the enumeration values that specifies the command behavior.</param>
        /// <returns>A System.Data.NpgsqlClient.NpgsqlDataReader object.</returns>
        public static NpgsqlDataReader ExecuteReaderWithRetry(this NpgsqlCommand command, CommandBehavior behavior)
        {
            return (NpgsqlDataReader)command.ExecuteReaderWithRetry(behavior, RetryManager.Instance.GetDefaultNpgsqlCommandRetryPolicy());
        }

        /// <summary>
        /// Sends the specified command to the connection and builds a NpgsqlDataReader object by using the specified 
        /// command behavior. Uses the default retry policy when executing the command.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <param name="behavior">One of the enumeration values that specifies the command behavior.</param>
        /// <returns>A System.Data.NpgsqlClient.NpgsqlDataReader object.</returns>
        public static async Task<NpgsqlDataReader> ExecuteReaderWithRetryAsync(this NpgsqlCommand command, CommandBehavior behavior)
        {
            return (NpgsqlDataReader)await command.ExecuteReaderWithRetryAsync(behavior, RetryManager.Instance.GetDefaultNpgsqlCommandRetryPolicy());
        }

        #endregion

        #region ExecuteScalarWithRetry method implementations

        /// <summary>
        /// Executes the query, and returns the first column of the first row in the result set returned by the query. Additional columns or rows are ignored.
        /// Uses the default retry policy when executing the command.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <returns> The first column of the first row in the result set, or a null reference if the result set is empty. Returns a maximum of 2033 characters.</returns>
        public static object ExecuteScalarWithRetry(this NpgsqlCommand command)
        {
            return command.ExecuteScalarWithRetry(RetryManager.Instance.GetDefaultNpgsqlCommandRetryPolicy());
        }

        /// <summary>
        /// Executes the query, and returns the first column of the first row in the result set returned by the query. Additional columns or rows are ignored.
        /// Uses the default retry policy when executing the command.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <returns> The first column of the first row in the result set, or a null reference if the result set is empty. Returns a maximum of 2033 characters.</returns>
        public static async Task<object> ExecuteScalarWithRetryAsync(this NpgsqlCommand command)
        {
            return await command.ExecuteScalarWithRetryAsync(RetryManager.Instance.GetDefaultNpgsqlCommandRetryPolicy());
        }

        #endregion
    }
}
