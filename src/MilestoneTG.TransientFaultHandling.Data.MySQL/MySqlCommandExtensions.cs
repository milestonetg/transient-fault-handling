using System;
using System.Data;
using System.Xml;
using MilestoneTG.TransientFaultHandling.Data;
using MySql.Data.MySqlClient;

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
            return ExecuteNonQueryWithRetry(command, RetryManager.Instance.GetDefaultMySqlCommandRetryPolicy());
        }

        /// <summary>
        /// Executes a Transact-MySql statement against the connection and returns the number of rows affected. Uses the specified retry policy when executing the command.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <param name="retryPolicy">The retry policy that determines whether to retry a command if a connection fails while executing the command.</param>
        /// <returns>The number of rows affected.</returns>
        public static int ExecuteNonQueryWithRetry(this MySqlCommand command, RetryPolicy retryPolicy)
        {
            return ExecuteNonQueryWithRetry(command, retryPolicy, RetryPolicy.NoRetry);
        }

        /// <summary>
        /// Executes a Transact-MySql statement against the connection and returns the number of rows affected. Uses the specified retry policies when executing the command
        /// and establishing a connection.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <param name="cmdRetryPolicy">The command retry policy that determines whether to retry a command if it fails while executing.</param>
        /// <param name="conRetryPolicy">The connection retry policy that determines whether to re-establish a connection if it drops while executing the command.</param>
        /// <returns>The number of rows affected.</returns>
        public static int ExecuteNonQueryWithRetry(this MySqlCommand command, RetryPolicy cmdRetryPolicy, RetryPolicy conRetryPolicy)
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
            return ExecuteReaderWithRetry(command, RetryManager.Instance.GetDefaultMySqlCommandRetryPolicy());
        }

        /// <summary>
        /// Sends the specified command to the connection and builds a MySqlDataReader object that contains the results.
        /// Uses the specified retry policy when executing the command.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <param name="retryPolicy">The retry policy that determines whether to retry a command if a connection fails while executing the command.</param>
        /// <returns>A System.Data.MySqlClient.MySqlDataReader object.</returns>
        public static MySqlDataReader ExecuteReaderWithRetry(this MySqlCommand command, RetryPolicy retryPolicy)
        {
            return ExecuteReaderWithRetry(command, retryPolicy, RetryPolicy.NoRetry);
        }

        /// <summary>
        /// Sends the specified command to the connection and builds a MySqlDataReader object that contains the results.
        /// Uses the specified retry policies when executing the command and
        /// establishing a connection.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <param name="cmdRetryPolicy">The command retry policy that determines whether to retry a command if it fails while executing.</param>
        /// <param name="conRetryPolicy">The connection retry policy that determines whether to re-establish a connection if it drops while executing the command.</param>
        /// <returns>A System.Data.MySqlClient.MySqlDataReader object.</returns>
        public static MySqlDataReader ExecuteReaderWithRetry(this MySqlCommand command, RetryPolicy cmdRetryPolicy, RetryPolicy conRetryPolicy)
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
        /// Sends the specified command to the connection and builds a MySqlDataReader object by using the specified 
        /// command behavior. Uses the default retry policy when executing the command.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <param name="behavior">One of the enumeration values that specifies the command behavior.</param>
        /// <returns>A System.Data.MySqlClient.MySqlDataReader object.</returns>
        public static MySqlDataReader ExecuteReaderWithRetry(this MySqlCommand command, CommandBehavior behavior)
        {
            return ExecuteReaderWithRetry(command, behavior, RetryManager.Instance.GetDefaultMySqlCommandRetryPolicy());
        }

        /// <summary>
        /// Sends the specified command to the connection and builds a MySqlDataReader object by using the specified
        /// command behavior. Uses the specified retry policy when executing the command.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <param name="behavior">One of the enumeration values that specifies the command behavior.</param>
        /// <param name="retryPolicy">The retry policy that determines whether to retry a command if a connection fails while executing the command.</param>
        /// <returns>A System.Data.MySqlClient.MySqlDataReader object.</returns>
        public static MySqlDataReader ExecuteReaderWithRetry(this MySqlCommand command, CommandBehavior behavior, RetryPolicy retryPolicy)
        {
            return ExecuteReaderWithRetry(command, behavior, retryPolicy, RetryPolicy.NoRetry);
        }

        /// <summary>
        /// Sends the specified command to the connection and builds a MySqlDataReader object by using the specified
        /// command behavior. Uses the specified retry policies when executing the command
        /// and establishing a connection.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <param name="behavior">One of the enumeration values that specifies the command behavior.</param>
        /// <param name="cmdRetryPolicy">The command retry policy that determines whether to retry a command if it fails while executing.</param>
        /// <param name="conRetryPolicy">The connection retry policy that determines whether to re-establish a connection if it drops while executing the command.</param>
        /// <returns>A System.Data.MySqlClient.MySqlDataReader object.</returns>
        public static MySqlDataReader ExecuteReaderWithRetry(this MySqlCommand command, CommandBehavior behavior, RetryPolicy cmdRetryPolicy, RetryPolicy conRetryPolicy)
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
            return ExecuteScalarWithRetry(command, RetryManager.Instance.GetDefaultMySqlCommandRetryPolicy());
        }

        /// <summary>
        /// Executes the query, and returns the first column of the first row in the result set returned by the query. Additional columns or rows are ignored.
        /// Uses the specified retry policy when executing the command.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <param name="retryPolicy">The retry policy that determines whether to retry a command if a connection fails while executing the command.</param>
        /// <returns> The first column of the first row in the result set, or a null reference if the result set is empty. Returns a maximum of 2033 characters.</returns>
        public static object ExecuteScalarWithRetry(this MySqlCommand command, RetryPolicy retryPolicy)
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
        public static object ExecuteScalarWithRetry(this MySqlCommand command, RetryPolicy cmdRetryPolicy, RetryPolicy conRetryPolicy)
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
        #endregion

        #region ExecuteXmlReaderWithRetry method implementations
        /// <summary>
        /// Sends the specified command to the connection and builds an XmlReader object that contains the results.
        /// Uses the default retry policy when executing the command.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <returns>An System.Xml.XmlReader object.</returns>
        public static XmlReader ExecuteXmlReaderWithRetry(this MySqlCommand command)
        {
            return ExecuteXmlReaderWithRetry(command, RetryManager.Instance.GetDefaultMySqlCommandRetryPolicy());
        }

        /// <summary>
        /// Sends the specified command to the connection and builds an XmlReader object that contains the results.
        /// Uses the specified retry policy when executing the command.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <param name="retryPolicy">The retry policy that determines whether to retry a command if a connection fails while executing the command.</param>
        /// <returns>An System.Xml.XmlReader object.</returns>
        public static XmlReader ExecuteXmlReaderWithRetry(this MySqlCommand command, RetryPolicy retryPolicy)
        {
            return ExecuteXmlReaderWithRetry(command, retryPolicy, RetryPolicy.NoRetry);
        }

        /// <summary>
        /// Sends the specified command to the connection and builds an XmlReader object that contains the results.
        /// Uses the specified retry policies when executing the command and establishing a connection.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <param name="cmdRetryPolicy">The command retry policy that determines whether to retry a command if it fails while executing.</param>
        /// <param name="conRetryPolicy">The connection retry policy that determines whether to re-establish a connection if it drops while executing the command.</param>
        /// <returns>An System.Xml.XmlReader object.</returns>
        public static XmlReader ExecuteXmlReaderWithRetry(this MySqlCommand command, RetryPolicy cmdRetryPolicy, RetryPolicy conRetryPolicy)
        {
            throw new NotSupportedException();
        }
        #endregion

        private static void GuardConnectionIsNotNull(MySqlCommand command)
        {
            if (command.Connection == null)
            {
                throw new InvalidOperationException(Resources.ConnectionHasNotBeenInitialized);
            }
        }

        private static bool EnsureValidConnection(MySqlCommand command, RetryPolicy retryPolicy)
        {
            if (command != null)
            {
                GuardConnectionIsNotNull(command);

                // Verify whether or not the connection is valid and is open. This code may be retried therefore
                // it is important to ensure that a connection is re-established should it have previously failed.
                if (command.Connection.State != ConnectionState.Open)
                {
                    // Attempt to open the connection using the retry policy that matches the policy for MySql commands.
                    command.Connection.OpenWithRetry(retryPolicy);

                    return true;
                }
            }

            return false;
        }
    }
}
