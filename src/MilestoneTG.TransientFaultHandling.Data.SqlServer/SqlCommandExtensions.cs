#region license
// ==============================================================================
// Microsoft patterns & practices Enterprise Library
// Transient Fault Handling Application Block
// ==============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
// ==============================================================================
#endregion

using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Xml;
using MilestoneTG.TransientFaultHandling.Data;

namespace MilestoneTG.TransientFaultHandling.Data.SqlServer
{
    /// <summary>
    /// Provides a set of extension methods that add retry capabilities to the standard System.Data.SqlClient.SqlCommand implementation.
    /// </summary>
    public static class SqlCommandExtensions
    {
        #region ExecuteNonQueryWithRetry method implementations
        /// <summary>
        /// Executes a Transact-SQL statement against the connection and returns the number of rows affected. Uses the default retry policy when executing the command.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <returns>The number of rows affected.</returns>
        public static int ExecuteNonQueryWithRetry(this SqlCommand command)
        {
            return command.ExecuteNonQueryWithRetry(RetryManager.Instance.GetDefaultSqlCommandRetryPolicy());
        }

        /// <summary>
        /// Executes a Transact-SQL statement against the connection and returns the number of rows affected. Uses the default retry policy when executing the command.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <returns>The number of rows affected.</returns>
        public static Task<int> ExecuteNonQueryWithRetryAsync(this SqlCommand command)
        {
            return command.ExecuteNonQueryWithRetryAsync(RetryManager.Instance.GetDefaultSqlCommandRetryPolicy());
        }

        #endregion

        #region ExecuteReaderWithRetry method implementations
        /// <summary>
        /// Sends the specified command to the connection and builds a SqlDataReader object that contains the results.
        /// Uses the default retry policy when executing the command.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <returns>A System.Data.SqlClient.SqlDataReader object.</returns>
        public static SqlDataReader ExecuteReaderWithRetry(this SqlCommand command)
        {
            return (SqlDataReader) command.ExecuteReaderWithRetry(RetryManager.Instance.GetDefaultSqlCommandRetryPolicy());
        }

        /// <summary>
        /// Sends the specified command to the connection and builds a SqlDataReader object that contains the results.
        /// Uses the default retry policy when executing the command.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <returns>A System.Data.SqlClient.SqlDataReader object.</returns>
        public static async Task<SqlDataReader> ExecuteReaderWithRetryAsync(this SqlCommand command)
        {
            return (SqlDataReader) await command.ExecuteReaderWithRetryAsync(RetryManager.Instance.GetDefaultSqlCommandRetryPolicy());
        }

        /// <summary>
        /// Sends the specified command to the connection and builds a SqlDataReader object by using the specified 
        /// command behavior. Uses the default retry policy when executing the command.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <param name="behavior">One of the enumeration values that specifies the command behavior.</param>
        /// <returns>A System.Data.SqlClient.SqlDataReader object.</returns>
        public static SqlDataReader ExecuteReaderWithRetry(this SqlCommand command, CommandBehavior behavior)
        {
            return (SqlDataReader)command.ExecuteReaderWithRetry(behavior, RetryManager.Instance.GetDefaultSqlCommandRetryPolicy());
        }

        /// <summary>
        /// Sends the specified command to the connection and builds a SqlDataReader object by using the specified 
        /// command behavior. Uses the default retry policy when executing the command.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <param name="behavior">One of the enumeration values that specifies the command behavior.</param>
        /// <returns>A System.Data.SqlClient.SqlDataReader object.</returns>
        public static async Task<SqlDataReader> ExecuteReaderWithRetryAsync(this SqlCommand command, CommandBehavior behavior)
        {
            return (SqlDataReader) await command.ExecuteReaderWithRetryAsync(behavior, RetryManager.Instance.GetDefaultSqlCommandRetryPolicy());
        }
       
        #endregion

        #region ExecuteScalarWithRetry method implementations

        /// <summary>
        /// Executes the query, and returns the first column of the first row in the result set returned by the query. Additional columns or rows are ignored.
        /// Uses the default retry policy when executing the command.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <returns> The first column of the first row in the result set, or a null reference if the result set is empty. Returns a maximum of 2033 characters.</returns>
        public static object ExecuteScalarWithRetry(this SqlCommand command)
        {
            return command.ExecuteScalarWithRetry(RetryManager.Instance.GetDefaultSqlCommandRetryPolicy());
        }

        /// <summary>
        /// Executes the query, and returns the first column of the first row in the result set returned by the query. Additional columns or rows are ignored.
        /// Uses the default retry policy when executing the command.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <returns> The first column of the first row in the result set, or a null reference if the result set is empty. Returns a maximum of 2033 characters.</returns>
        public static async Task<object> ExecuteScalarWithRetryAsync(this SqlCommand command)
        {
            return await command.ExecuteScalarWithRetryAsync(RetryManager.Instance.GetDefaultSqlCommandRetryPolicy());
        }

        #endregion

        #region ExecuteXmlReaderWithRetry method implementations
        /// <summary>
        /// Sends the specified command to the connection and builds an XmlReader object that contains the results.
        /// Uses the default retry policy when executing the command.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <returns>An System.Xml.XmlReader object.</returns>
        public static XmlReader ExecuteXmlReaderWithRetry(this SqlCommand command)
        {
            return ExecuteXmlReaderWithRetry(command, RetryManager.Instance.GetDefaultSqlCommandRetryPolicy());
        }

        /// <summary>
        /// Sends the specified command to the connection and builds an XmlReader object that contains the results.
        /// Uses the default retry policy when executing the command.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <returns>An System.Xml.XmlReader object.</returns>
        public static Task<XmlReader> ExecuteXmlReaderWithRetryAsync(this SqlCommand command)
        {
            return ExecuteXmlReaderWithRetryAsync(command, RetryManager.Instance.GetDefaultSqlCommandRetryPolicy());
        }
        
        /// <summary>
        /// Sends the specified command to the connection and builds an XmlReader object that contains the results.
        /// Uses the specified retry policy when executing the command.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <param name="retryPolicy">The retry policy that determines whether to retry a command if a connection fails while executing the command.</param>
        /// <returns>An System.Xml.XmlReader object.</returns>
        public static XmlReader ExecuteXmlReaderWithRetry(this SqlCommand command, RetryPolicy retryPolicy)
        {
            return ExecuteXmlReaderWithRetry(command, retryPolicy, RetryPolicy.NoRetry);
        }

        /// <summary>
        /// Sends the specified command to the connection and builds an XmlReader object that contains the results.
        /// Uses the specified retry policy when executing the command.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <param name="retryPolicy">The retry policy that determines whether to retry a command if a connection fails while executing the command.</param>
        /// <returns>An System.Xml.XmlReader object.</returns>
        public static Task<XmlReader> ExecuteXmlReaderWithRetryAsync(this SqlCommand command, RetryPolicy retryPolicy)
        {
            return ExecuteXmlReaderWithRetryAsync(command, retryPolicy, RetryPolicy.NoRetry);
        }

        /// <summary>
        /// Sends the specified command to the connection and builds an XmlReader object that contains the results.
        /// Uses the specified retry policies when executing the command and establishing a connection.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <param name="cmdRetryPolicy">The command retry policy that determines whether to retry a command if it fails while executing.</param>
        /// <param name="conRetryPolicy">The connection retry policy that determines whether to re-establish a connection if it drops while executing the command.</param>
        /// <returns>An System.Xml.XmlReader object.</returns>
        public static XmlReader ExecuteXmlReaderWithRetry(this SqlCommand command, RetryPolicy cmdRetryPolicy, RetryPolicy conRetryPolicy)
        {
            GuardConnectionIsNotNull(command);

            // Check if retry policy was specified, if not, use the default retry policy.
            return (cmdRetryPolicy ?? RetryPolicy.NoRetry).ExecuteAction(() =>
            {
                var hasOpenConnection = EnsureValidConnection(command, conRetryPolicy);

                try
                {
                    return command.ExecuteXmlReader();
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
        /// Sends the specified command to the connection and builds an XmlReader object that contains the results.
        /// Uses the specified retry policies when executing the command and establishing a connection.
        /// </summary>
        /// <param name="command">The command object that is required for the extension method declaration.</param>
        /// <param name="cmdRetryPolicy">The command retry policy that determines whether to retry a command if it fails while executing.</param>
        /// <param name="conRetryPolicy">The connection retry policy that determines whether to re-establish a connection if it drops while executing the command.</param>
        /// <returns>An System.Xml.XmlReader object.</returns>
        public static Task<XmlReader> ExecuteXmlReaderWithRetryAsync(this SqlCommand command, RetryPolicy cmdRetryPolicy, RetryPolicy conRetryPolicy)
        {
            GuardConnectionIsNotNull(command);

            // Check if retry policy was specified, if not, use the default retry policy.
            return (cmdRetryPolicy ?? RetryPolicy.NoRetry).ExecuteAsync(async () =>
            {
                var hasOpenConnection = EnsureValidConnection(command, conRetryPolicy);

                try
                {
                    return await command.ExecuteXmlReaderAsync();
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

        private static void GuardConnectionIsNotNull(SqlCommand command)
        {
            if (command.Connection == null)
            {
                throw new InvalidOperationException(Resources.ConnectionHasNotBeenInitialized);
            }
        }

        private static bool EnsureValidConnection(SqlCommand command, RetryPolicy retryPolicy)
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
    }
}
