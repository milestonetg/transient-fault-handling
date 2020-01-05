using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilestoneTG.TransientFaultHandling.Data
{
    public static class DbCommandExtensions
    {
        #region ExecuteNonQueryWithRetry method implementations

        /// <summary>
        /// Executes a Transact-SQL statement against the connection and returns the number of rows affected. Uses the specified retry policy when executing the command.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <param name="retryPolicy">The retry policy that determines whether to retry a command if a connection fails while executing the command.</param>
        /// <returns>The number of rows affected.</returns>
        public static int ExecuteNonQueryWithRetry(this DbCommand command, RetryPolicy retryPolicy)
        {
            return ExecuteNonQueryWithRetry(command, retryPolicy, RetryPolicy.NoRetry);
        }

        /// <summary>
        /// Executes a Transact-SQL statement against the connection and returns the number of rows affected. Uses the specified retry policy when executing the command.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <param name="retryPolicy">The retry policy that determines whether to retry a command if a connection fails while executing the command.</param>
        /// <returns>The number of rows affected.</returns>
        public static Task<int> ExecuteNonQueryWithRetryAsync(this DbCommand command, RetryPolicy retryPolicy)
        {
            return ExecuteNonQueryWithRetryAsync(command, retryPolicy, RetryPolicy.NoRetry);
        }

        /// <summary>
        /// Executes a Transact-SQL statement against the connection and returns the number of rows affected. Uses the specified retry policies when executing the command
        /// and establishing a connection.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <param name="cmdRetryPolicy">The command retry policy that determines whether to retry a command if it fails while executing.</param>
        /// <param name="conRetryPolicy">The connection retry policy that determines whether to re-establish a connection if it drops while executing the command.</param>
        /// <returns>The number of rows affected.</returns>
        public static int ExecuteNonQueryWithRetry(this DbCommand command, RetryPolicy cmdRetryPolicy, RetryPolicy conRetryPolicy)
        {
            GuardConnectionIsNotNull(command);

            // Check if retry policy was specified, if not, use the default retry policy.
            return (cmdRetryPolicy ?? RetryPolicy.NoRetry).ExecuteAction(() =>
            {
                var hasOpenConnection = EnsureValidConnection(command, conRetryPolicy);

                try
                {
                    return command.ExecuteNonQuery();
                }
                finally
                {
                    if (hasOpenConnection && command.Connection != null && command.Connection.State == ConnectionState.Open)
                    {
                        command.Connection.Close();
                    }
                }
            });
        }

        /// <summary>
        /// Executes a Transact-SQL statement against the connection and returns the number of rows affected. Uses the specified retry policies when executing the command
        /// and establishing a connection.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <param name="cmdRetryPolicy">The command retry policy that determines whether to retry a command if it fails while executing.</param>
        /// <param name="conRetryPolicy">The connection retry policy that determines whether to re-establish a connection if it drops while executing the command.</param>
        /// <returns>The number of rows affected.</returns>
        public static async Task<int> ExecuteNonQueryWithRetryAsync(this DbCommand command, RetryPolicy cmdRetryPolicy, RetryPolicy conRetryPolicy)
        {
            GuardConnectionIsNotNull(command);

            // Check if retry policy was specified, if not, use the default retry policy.
            return await (cmdRetryPolicy ?? RetryPolicy.NoRetry).ExecuteAsync(async () =>
            {
                var hasOpenConnection = EnsureValidConnection(command, conRetryPolicy);

                try
                {
                    return await command.ExecuteNonQueryAsync();
                }
                finally
                {
                    if (hasOpenConnection && command.Connection != null && command.Connection.State == ConnectionState.Open)
                    {
                        command.Connection.Close();
                    }
                }
            });
        }

        #endregion

        #region ExecuteReaderWithRetry method implementations

        /// <summary>
        /// Sends the specified command to the connection and builds a SqlDataReader object that contains the results.
        /// Uses the specified retry policy when executing the command.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <param name="retryPolicy">The retry policy that determines whether to retry a command if a connection fails while executing the command.</param>
        /// <returns>A System.Data.SqlClient.SqlDataReader object.</returns>
        public static DbDataReader ExecuteReaderWithRetry(this DbCommand command, RetryPolicy retryPolicy)
        {
            return ExecuteReaderWithRetry(command, retryPolicy, RetryPolicy.NoRetry);
        }

        /// <summary>
        /// Sends the specified command to the connection and builds a SqlDataReader object that contains the results.
        /// Uses the specified retry policies when executing the command and
        /// establishing a connection.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <param name="cmdRetryPolicy">The command retry policy that determines whether to retry a command if it fails while executing.</param>
        /// <param name="conRetryPolicy">The connection retry policy that determines whether to re-establish a connection if it drops while executing the command.</param>
        /// <returns>A System.Data.SqlClient.SqlDataReader object.</returns>
        public static DbDataReader ExecuteReaderWithRetry(this DbCommand command, RetryPolicy cmdRetryPolicy, RetryPolicy conRetryPolicy)
        {
            GuardConnectionIsNotNull(command);

            // Check if retry policy was specified, if not, use the default retry policy.
            return (cmdRetryPolicy ?? RetryPolicy.NoRetry).ExecuteAction(() =>
            {
                var hasOpenConnection = EnsureValidConnection(command, conRetryPolicy);

                try
                {
                    return command.ExecuteReader();
                }
                catch (Exception)
                {
                    if (hasOpenConnection && command.Connection != null && command.Connection.State == ConnectionState.Open)
                    {
                        command.Connection.Close();
                    }

                    throw;
                }
            });
        }

        /// <summary>
        /// Sends the specified command to the connection and builds a SqlDataReader object by using the specified
        /// command behavior. Uses the specified retry policy when executing the command.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <param name="behavior">One of the enumeration values that specifies the command behavior.</param>
        /// <param name="retryPolicy">The retry policy that determines whether to retry a command if a connection fails while executing the command.</param>
        /// <returns>A System.Data.SqlClient.SqlDataReader object.</returns>
        public static DbDataReader ExecuteReaderWithRetry(this DbCommand command, CommandBehavior behavior, RetryPolicy retryPolicy)
        {
            return ExecuteReaderWithRetry(command, behavior, retryPolicy, RetryPolicy.NoRetry);
        }

        /// <summary>
        /// Sends the specified command to the connection and builds a SqlDataReader object by using the specified
        /// command behavior. Uses the specified retry policies when executing the command
        /// and establishing a connection.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <param name="behavior">One of the enumeration values that specifies the command behavior.</param>
        /// <param name="cmdRetryPolicy">The command retry policy that determines whether to retry a command if it fails while executing.</param>
        /// <param name="conRetryPolicy">The connection retry policy that determines whether to re-establish a connection if it drops while executing the command.</param>
        /// <returns>A System.Data.SqlClient.SqlDataReader object.</returns>
        public static DbDataReader ExecuteReaderWithRetry(this DbCommand command, CommandBehavior behavior, RetryPolicy cmdRetryPolicy, RetryPolicy conRetryPolicy)
        {
            GuardConnectionIsNotNull(command);

            // Check if retry policy was specified, if not, use the default retry policy.
            return (cmdRetryPolicy ?? RetryPolicy.NoRetry).ExecuteAction(() =>
            {
                var hasOpenConnection = EnsureValidConnection(command, conRetryPolicy);

                try
                {
                    return command.ExecuteReader(behavior);
                }
                catch (Exception)
                {
                    if (hasOpenConnection && command.Connection != null && command.Connection.State == ConnectionState.Open)
                    {
                        command.Connection.Close();
                    }

                    throw;
                }
            });
        }

        ///////

        /// <summary>
        /// Sends the specified command to the connection and builds a SqlDataReader object that contains the results.
        /// Uses the specified retry policy when executing the command.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <param name="retryPolicy">The retry policy that determines whether to retry a command if a connection fails while executing the command.</param>
        /// <returns>A System.Data.SqlClient.SqlDataReader object.</returns>
        public static Task<DbDataReader> ExecuteReaderWithRetryAsync(this DbCommand command, RetryPolicy retryPolicy)
        {
            return ExecuteReaderWithRetryAsync(command, retryPolicy, RetryPolicy.NoRetry);
        }

        /// <summary>
        /// Sends the specified command to the connection and builds a SqlDataReader object that contains the results.
        /// Uses the specified retry policies when executing the command and
        /// establishing a connection.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <param name="cmdRetryPolicy">The command retry policy that determines whether to retry a command if it fails while executing.</param>
        /// <param name="conRetryPolicy">The connection retry policy that determines whether to re-establish a connection if it drops while executing the command.</param>
        /// <returns>A System.Data.SqlClient.SqlDataReader object.</returns>
        public static Task<DbDataReader> ExecuteReaderWithRetryAsync(this DbCommand command, RetryPolicy cmdRetryPolicy, RetryPolicy conRetryPolicy)
        {
            GuardConnectionIsNotNull(command);

            // Check if retry policy was specified, if not, use the default retry policy.
            return (cmdRetryPolicy ?? RetryPolicy.NoRetry).ExecuteAsync(async () =>
            {
                var hasOpenConnection = EnsureValidConnection(command, conRetryPolicy);

                try
                {
                    return await command.ExecuteReaderAsync();
                }
                catch (Exception)
                {
                    if (hasOpenConnection && command.Connection != null && command.Connection.State == ConnectionState.Open)
                    {
                        command.Connection.Close();
                    }

                    throw;
                }
            });
        }

        /// <summary>
        /// Sends the specified command to the connection and builds a SqlDataReader object by using the specified
        /// command behavior. Uses the specified retry policy when executing the command.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <param name="behavior">One of the enumeration values that specifies the command behavior.</param>
        /// <param name="retryPolicy">The retry policy that determines whether to retry a command if a connection fails while executing the command.</param>
        /// <returns>A System.Data.SqlClient.SqlDataReader object.</returns>
        public static Task<DbDataReader> ExecuteReaderWithRetryAsync(this DbCommand command, CommandBehavior behavior, RetryPolicy retryPolicy)
        {
            return ExecuteReaderWithRetryAsync(command, behavior, retryPolicy, RetryPolicy.NoRetry);
        }

        /// <summary>
        /// Sends the specified command to the connection and builds a SqlDataReader object by using the specified
        /// command behavior. Uses the specified retry policies when executing the command
        /// and establishing a connection.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <param name="behavior">One of the enumeration values that specifies the command behavior.</param>
        /// <param name="cmdRetryPolicy">The command retry policy that determines whether to retry a command if it fails while executing.</param>
        /// <param name="conRetryPolicy">The connection retry policy that determines whether to re-establish a connection if it drops while executing the command.</param>
        /// <returns>A System.Data.SqlClient.SqlDataReader object.</returns>
        public static Task<DbDataReader> ExecuteReaderWithRetryAsync(this DbCommand command, CommandBehavior behavior, RetryPolicy cmdRetryPolicy, RetryPolicy conRetryPolicy)
        {
            GuardConnectionIsNotNull(command);

            // Check if retry policy was specified, if not, use the default retry policy.
            return (cmdRetryPolicy ?? RetryPolicy.NoRetry).ExecuteAsync(async () =>
            {
                var hasOpenConnection = EnsureValidConnection(command, conRetryPolicy);

                try
                {
                    return await command.ExecuteReaderAsync(behavior);
                }
                catch (Exception)
                {
                    if (hasOpenConnection && command.Connection != null && command.Connection.State == ConnectionState.Open)
                    {
                        command.Connection.Close();
                    }

                    throw;
                }
            });
        }

        #endregion

        #region ExecuteScalarWithRetry method implementations

        /// <summary>
        /// Executes the query, and returns the first column of the first row in the result set returned by the query. Additional columns or rows are ignored.
        /// Uses the specified retry policy when executing the command.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <param name="retryPolicy">The retry policy that determines whether to retry a command if a connection fails while executing the command.</param>
        /// <returns> The first column of the first row in the result set, or a null reference if the result set is empty. Returns a maximum of 2033 characters.</returns>
        public static object ExecuteScalarWithRetry(this DbCommand command, RetryPolicy retryPolicy)
        {
            return ExecuteScalarWithRetry(command, retryPolicy, RetryPolicy.NoRetry);
        }

        /// <summary>
        /// Executes the query, and returns the first column of the first row in the result set returned by the query. Additional columns or rows are ignored.
        /// Uses the specified retry policies when executing the command and establishing a connection.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <param name="cmdRetryPolicy">The command retry policy that determines whether to retry a command if it fails while executing.</param>
        /// <param name="conRetryPolicy">The connection retry policy that determines whether to re-establish a connection if it drops while executing the command.</param>
        /// <returns> The first column of the first row in the result set, or a null reference if the result set is empty. Returns a maximum of 2033 characters.</returns>
        public static object ExecuteScalarWithRetry(this DbCommand command, RetryPolicy cmdRetryPolicy, RetryPolicy conRetryPolicy)
        {
            GuardConnectionIsNotNull(command);

            // Check if retry policy was specified, if not, use the default retry policy.
            return (cmdRetryPolicy ?? RetryPolicy.NoRetry).ExecuteAction(() =>
            {
                var hasOpenConnection = EnsureValidConnection(command, conRetryPolicy);

                try
                {
                    return command.ExecuteScalar();
                }
                finally
                {
                    if (hasOpenConnection && command.Connection != null && command.Connection.State == ConnectionState.Open)
                    {
                        command.Connection.Close();
                    }
                }
            });
        }

        //

        /// <summary>
        /// Executes the query, and returns the first column of the first row in the result set returned by the query. Additional columns or rows are ignored.
        /// Uses the specified retry policy when executing the command.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <param name="retryPolicy">The retry policy that determines whether to retry a command if a connection fails while executing the command.</param>
        /// <returns> The first column of the first row in the result set, or a null reference if the result set is empty. Returns a maximum of 2033 characters.</returns>
        public static Task<object> ExecuteScalarWithRetryAsync(this DbCommand command, RetryPolicy retryPolicy)
        {
            return ExecuteScalarWithRetryAsync(command, retryPolicy, RetryPolicy.NoRetry);
        }

        /// <summary>
        /// Executes the query, and returns the first column of the first row in the result set returned by the query. Additional columns or rows are ignored.
        /// Uses the specified retry policies when executing the command and establishing a connection.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <param name="cmdRetryPolicy">The command retry policy that determines whether to retry a command if it fails while executing.</param>
        /// <param name="conRetryPolicy">The connection retry policy that determines whether to re-establish a connection if it drops while executing the command.</param>
        /// <returns> The first column of the first row in the result set, or a null reference if the result set is empty. Returns a maximum of 2033 characters.</returns>
        public static Task<object> ExecuteScalarWithRetryAsync(this DbCommand command, RetryPolicy cmdRetryPolicy, RetryPolicy conRetryPolicy)
        {
            GuardConnectionIsNotNull(command);

            // Check if retry policy was specified, if not, use the default retry policy.
            return (cmdRetryPolicy ?? RetryPolicy.NoRetry).ExecuteAsync(async () =>
            {
                var hasOpenConnection = EnsureValidConnection(command, conRetryPolicy);

                try
                {
                    return await command.ExecuteScalarAsync();
                }
                finally
                {
                    if (hasOpenConnection && command.Connection != null && command.Connection.State == ConnectionState.Open)
                    {
                        command.Connection.Close();
                    }
                }
            });
        }
        #endregion

        private static void GuardConnectionIsNotNull(DbCommand command)
        {
            if (command.Connection == null)
            {
                throw new InvalidOperationException(Resources.ConnectionHasNotBeenInitialized);
            }
        }

        private static bool EnsureValidConnection(DbCommand command, RetryPolicy retryPolicy)
        {
            if (command != null)
            {
                GuardConnectionIsNotNull(command);

                // Verify whether or not the connection is valid and is open. This code may be retried therefore
                // it is important to ensure that a connection is re-established should it have previously failed.
                if (command.Connection.State != ConnectionState.Open)
                {
                    // Attempt to open the connection using the retry policy that matches the policy for SQL commands.
                    command.Connection.OpenWithRetry(retryPolicy);

                    return true;
                }
            }

            return false;
        }

        private static async Task<bool> EnsureValidConnectionAsync(DbCommand command, RetryPolicy retryPolicy)
        {
            if (command != null)
            {
                GuardConnectionIsNotNull(command);

                // Verify whether or not the connection is valid and is open. This code may be retried therefore
                // it is important to ensure that a connection is re-established should it have previously failed.
                if (command.Connection.State != ConnectionState.Open)
                {
                    // Attempt to open the connection using the retry policy that matches the policy for SQL commands.
                    await command.Connection.OpenWithRetryAsync(retryPolicy);

                    return true;
                }
            }

            return false;
        }
    }
}
