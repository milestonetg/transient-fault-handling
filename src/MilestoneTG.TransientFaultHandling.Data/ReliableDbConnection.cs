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
using System.Globalization;
using System.Threading.Tasks;
using System.Xml;

namespace MilestoneTG.TransientFaultHandling.Data
{
    /// <summary>
    /// Provides a reliable way of opening connections to, and executing commands against, the SQL Database 
    /// databases taking potential network unreliability and connection retry requirements into account.
    /// </summary>
    public abstract class ReliableDbConnection : IDbConnection, ICloneable
    {
        private readonly DbConnection underlyingConnection;
        private readonly RetryPolicy connectionRetryPolicy;
        private readonly RetryPolicy commandRetryPolicy;
        private readonly RetryPolicy connectionStringFailoverPolicy;

        private string connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReliableDbConnection"/> class with the specified connection string
        /// and a policy that defines whether to retry a request if a connection or command
        /// fails.
        /// </summary>
        /// <param name="connectionString">The connection string used to open the SQL Database.</param>
        /// <param name="connectionRetryPolicy">The retry policy that defines whether to retry a request if a connection fails to be established.</param>
        /// <param name="commandRetryPolicy">The retry policy that defines whether to retry a request if a command fails to be executed.</param>
        public ReliableDbConnection(string connectionString, RetryPolicy connectionRetryPolicy, RetryPolicy commandRetryPolicy)
        {
            this.connectionString = connectionString;
            this.underlyingConnection = CreateConnection(connectionString);
            this.connectionRetryPolicy = connectionRetryPolicy;
            this.commandRetryPolicy = commandRetryPolicy;

            // Configure a special retry policy that enables detecting network connectivity errors to be able to determine whether we need to failover
            // to the original connection string containing the DNS name of the SQL Database server.
            this.connectionStringFailoverPolicy = CreateConnectionStringFailoverPolicy();
        }

        /// <summary>
        /// Creates the connection string failover policy.
        /// </summary>
        /// <returns>RetryPolicy.</returns>
        protected abstract RetryPolicy CreateConnectionStringFailoverPolicy();

        #region Public properties
        /// <summary>
        /// Gets or sets the connection string for opening a connection to the SQL Database.
        /// </summary>
        public string ConnectionString
        {
            get
            {
                return this.connectionString;
            }

            set
            {
                this.connectionString = value;
                this.underlyingConnection.ConnectionString = value;
            }
        }

        /// <summary>
        /// Gets the policy that determines whether to retry a connection request, based on how many 
        /// times the request has been made and the reason for the last failure. 
        /// </summary>
        public RetryPolicy ConnectionRetryPolicy
        {
            get { return this.connectionRetryPolicy; }
        }

        /// <summary>
        /// Gets the policy that determines whether to retry a command, based on how many 
        /// times the request has been made and the reason for the last failure. 
        /// </summary>
        public RetryPolicy CommandRetryPolicy
        {
            get { return this.commandRetryPolicy; }
        }

        /// <summary>
        /// Gets an instance of the SqlConnection object that represents the connection to a SQL Database instance.
        /// </summary>
        public DbConnection Current
        {
            get { return this.underlyingConnection; }
        }

        /// <summary>
        /// Gets the CONTEXT_INFO value that was set for the current session. This value can be used to trace query execution problems. 
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "As designed")]
        public Guid SessionTracingId
        {
            get
            {
                try
                {
                    using (var query = SqlCommandFactory.CreateGetContextInfoCommand(this.Current))
                    {
                        // Execute the query in retry-aware fashion, retry if necessary.
                        return this.ExecuteCommand<Guid>(query);
                    }
                }
                catch
                {
                    // Any failure will result in returning a default GUID value. This is by design.
                    return Guid.Empty;
                }
            }
        }

        /// <summary>
        /// Gets a value that specifies the time to wait while trying to establish a connection before terminating
        /// the attempt and generating an error.
        /// </summary>
        public int ConnectionTimeout
        {
            get { return this.underlyingConnection.ConnectionTimeout; }
        }

        /// <summary>
        /// Gets the name of the current database or the database to be used after a
        /// connection is opened.
        /// </summary>
        public string Database
        {
            get { return this.underlyingConnection.Database; }
        }

        /// <summary>
        /// Gets the current state of the connection.
        /// </summary>
        public ConnectionState State
        {
            get { return this.underlyingConnection.State; }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Opens a database connection with the settings specified by the ConnectionString and ConnectionRetryPolicy properties.
        /// </summary>
        /// <returns>An object that represents the open connection.</returns>
        public DbConnection Open()
        {
            return this.Open(this.ConnectionRetryPolicy);
        }

        /// <summary>
        /// Opens a database connection with the settings specified by the ConnectionString and ConnectionRetryPolicy properties.
        /// </summary>
        /// <returns>An object that represents the open connection.</returns>
        public Task<DbConnection> OpenAsync()
        {
            return this.OpenAsync(this.ConnectionRetryPolicy);
        }

        /// <summary>
        /// Opens a database connection with the settings specified by the connection string and the specified retry policy.
        /// </summary>
        /// <param name="retryPolicy">The retry policy that defines whether to retry a request if the connection fails to open.</param>
        /// <returns>An object that represents the open connection.</returns>
        public DbConnection Open(RetryPolicy retryPolicy)
        {
            // Check if retry policy was specified, if not, disable retries by executing the Open method using RetryPolicy.NoRetry.
            (retryPolicy ?? RetryPolicy.NoRetry).ExecuteAction(() =>
                this.connectionStringFailoverPolicy.ExecuteAction(() =>
                    {
                        if (this.underlyingConnection.State != ConnectionState.Open)
                        {
                            this.underlyingConnection.Open();
                        }
                    }));

            return this.underlyingConnection;
        }

        /// <summary>
        /// Opens a database connection with the settings specified by the connection string and the specified retry policy.
        /// </summary>
        /// <param name="retryPolicy">The retry policy that defines whether to retry a request if the connection fails to open.</param>
        /// <returns>An object that represents the open connection.</returns>
        public async Task<DbConnection> OpenAsync(RetryPolicy retryPolicy)
        {
            // Check if retry policy was specified, if not, disable retries by executing the Open method using RetryPolicy.NoRetry.
            await (retryPolicy ?? RetryPolicy.NoRetry).ExecuteAsync(async () =>
                await this.connectionStringFailoverPolicy.ExecuteAsync(async () =>
                {
                    if (this.underlyingConnection.State != ConnectionState.Open)
                    {
                        await this.underlyingConnection.OpenAsync();
                    }
                }));

            return this.underlyingConnection;
        }


        /// <summary>
        /// Executes a SQL command and returns a result that is defined by the specified type <typeparamref name="T"/>. This method uses the retry policy specified when 
        /// instantiating the SqlAzureConnection class (or the default retry policy if no policy was set at construction time).
        /// </summary>
        /// <typeparam name="T">IDataReader, XmlReader, or any other .NET Framework type that defines the type of result to be returned.</typeparam>
        /// <param name="command">The SQL command to be executed.</param>
        /// <returns>An instance of an IDataReader, XmlReader, or any other .NET Framework object that contains the result.</returns>
        public T ExecuteCommand<T>(IDbCommand command)
        {
            return this.ExecuteCommand<T>(command, this.CommandRetryPolicy, CommandBehavior.Default);
        }

        /// <summary>
        /// Executes a SQL command and returns a result that is defined by the specified type <typeparamref name="T"/>. This method uses the retry policy specified when 
        /// instantiating the SqlAzureConnection class (or the default retry policy if no policy was set at construction time).
        /// </summary>
        /// <typeparam name="T">IDataReader, XmlReader, or any other .NET Framework type that defines the type of result to be returned.</typeparam>
        /// <param name="command">The SQL command to be executed.</param>
        /// <param name="behavior">A description of the results of the query and its effect on the database.</param>
        /// <returns>An instance of an IDataReader, XmlReader, or any other .NET Frameork object that contains the result.</returns>
        public T ExecuteCommand<T>(IDbCommand command, CommandBehavior behavior)
        {
            return this.ExecuteCommand<T>(command, this.CommandRetryPolicy, behavior);
        }

        /// <summary>
        /// Executes a SQL command by using the specified retry policy, and returns a result that is defined by the specified type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">IDataReader, XmlReader, or any other .NET Framework type that defines the type of result to be returned.</typeparam>
        /// <param name="command">The SQL command to be executed.</param>
        /// <param name="retryPolicy">The retry policy that defines whether to retry a command if a connection fails while executing the command.</param>
        /// <returns>An instance of an IDataReader, XmlReader, or any other .NET Frameork object that contains the result.</returns>
        public T ExecuteCommand<T>(IDbCommand command, RetryPolicy retryPolicy)
        {
            return this.ExecuteCommand<T>(command, retryPolicy, CommandBehavior.Default);
        }


        /// <summary>
        /// Executes a SQL command and returns a result that is defined by the specified type <typeparamref name="T"/>. This method uses the retry policy specified when 
        /// instantiating the SqlAzureConnection class (or the default retry policy if no policy was set at construction time).
        /// </summary>
        /// <typeparam name="T">IDataReader, XmlReader, or any other .NET Framework type that defines the type of result to be returned.</typeparam>
        /// <param name="command">The SQL command to be executed.</param>
        /// <returns>An instance of an IDataReader, XmlReader, or any other .NET Framework object that contains the result.</returns>
        public Task<T> ExecuteCommandAsync<T>(DbCommand command)
        {
            return this.ExecuteCommandAsync<T>(command, this.CommandRetryPolicy, CommandBehavior.Default);
        }

        /// <summary>
        /// Executes a SQL command and returns a result that is defined by the specified type <typeparamref name="T"/>. This method uses the retry policy specified when 
        /// instantiating the SqlAzureConnection class (or the default retry policy if no policy was set at construction time).
        /// </summary>
        /// <typeparam name="T">IDataReader, XmlReader, or any other .NET Framework type that defines the type of result to be returned.</typeparam>
        /// <param name="command">The SQL command to be executed.</param>
        /// <param name="behavior">A description of the results of the query and its effect on the database.</param>
        /// <returns>An instance of an IDataReader, XmlReader, or any other .NET Frameork object that contains the result.</returns>
        public Task<T> ExecuteCommandAsync<T>(DbCommand command, CommandBehavior behavior)
        {
            return this.ExecuteCommandAsync<T>(command, this.CommandRetryPolicy, behavior);
        }

        /// <summary>
        /// Executes a SQL command by using the specified retry policy, and returns a result that is defined by the specified type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">IDataReader, XmlReader, or any other .NET Framework type that defines the type of result to be returned.</typeparam>
        /// <param name="command">The SQL command to be executed.</param>
        /// <param name="retryPolicy">The retry policy that defines whether to retry a command if a connection fails while executing the command.</param>
        /// <returns>An instance of an IDataReader, XmlReader, or any other .NET Frameork object that contains the result.</returns>
        public Task<T> ExecuteCommandAsync<T>(DbCommand command, RetryPolicy retryPolicy)
        {
            return this.ExecuteCommandAsync<T>(command, retryPolicy, CommandBehavior.Default);
        }


        /// <summary>
        /// Executes a SQL command by using the specified retry policy, and returns a result that is defined by the specified type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">IDataReader, XmlReader, or any other .NET Framework type that defines the type of result to be returned.</typeparam>
        /// <param name="command">The SQL command to be executed.</param>
        /// <param name="retryPolicy">The retry policy that defines whether to retry a command if a connection fails while executing the command.</param>
        /// <param name="behavior">A description of the results of the query and its effect on the database.</param>
        /// <returns>An instance of an IDataReader, XmlReader, or any other .NET Frameork object that contains the result.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Disposed by client code")]
        public T ExecuteCommand<T>(IDbCommand command, RetryPolicy retryPolicy, CommandBehavior behavior)
        {
            var actionResult = default(T);

            var resultType = typeof(T);

            var hasOpenedConnection = false;

            var closeOpenedConnectionOnSuccess = false;

            try
            {
                (retryPolicy ?? RetryPolicy.NoRetry).ExecuteAction(() =>
                {
                    actionResult = this.connectionStringFailoverPolicy.ExecuteAction(() =>
                    {
                        // Make sure the command has been associated with a valid connection. If not, associate it with an opened SQL connection.
                        if (command.Connection == null)
                        {
                            // Open a new connection and assign it to the command object.
                            command.Connection = this.Open();
                            hasOpenedConnection = true;
                        }

                        // Verify whether or not the connection is valid and is open. This code may be retried therefore
                        // it is important to ensure that a connection is re-established should it have previously failed.
                        if (command.Connection.State != ConnectionState.Open)
                        {
                            command.Connection.Open();
                            hasOpenedConnection = true;
                        }

                        if (typeof(IDataReader).IsAssignableFrom(resultType))
                        {
                            closeOpenedConnectionOnSuccess = false;

                            return (T)command.ExecuteReader(behavior);
                        }

                        if (resultType == typeof(XmlReader))
                        {
                            return ExecuteXmlReader<T>(command, behavior, closeOpenedConnectionOnSuccess);
                        }

                        if (resultType == typeof(NonQueryResult))
                        {
                            var result = new NonQueryResult { RecordsAffected = command.ExecuteNonQuery() };

                            closeOpenedConnectionOnSuccess = true;

                            return (T)Convert.ChangeType(result, resultType, CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            var result = command.ExecuteScalar();

                            closeOpenedConnectionOnSuccess = true;

                            if (result != null)
                            {
                                return (T)Convert.ChangeType(result, resultType, CultureInfo.InvariantCulture);
                            }

                            return default(T);
                        }
                    });
                });

                if (hasOpenedConnection && closeOpenedConnectionOnSuccess &&
                   command.Connection != null && command.Connection.State == ConnectionState.Open)
                {
                    command.Connection.Close();
                }
            }
            catch (Exception)
            {
                if (hasOpenedConnection &&
                   command.Connection != null && command.Connection.State == ConnectionState.Open)
                {
                    command.Connection.Close();
                }

                throw;
            }

            return actionResult;
        }

        /// <summary>
        /// Executes a SQL command by using the specified retry policy, and returns a result that is defined by the specified type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">IDataReader, XmlReader, or any other .NET Framework type that defines the type of result to be returned.</typeparam>
        /// <param name="command">The SQL command to be executed.</param>
        /// <param name="retryPolicy">The retry policy that defines whether to retry a command if a connection fails while executing the command.</param>
        /// <param name="behavior">A description of the results of the query and its effect on the database.</param>
        /// <returns>An instance of an IDataReader, XmlReader, or any other .NET Frameork object that contains the result.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Disposed by client code")]
        public async Task<T> ExecuteCommandAsync<T>(DbCommand command, RetryPolicy retryPolicy, CommandBehavior behavior)
        {
            var actionResult = default(T);

            var resultType = typeof(T);

            var hasOpenedConnection = false;

            var closeOpenedConnectionOnSuccess = false;

            try
            {
                await (retryPolicy ?? RetryPolicy.NoRetry).ExecuteAsync(async () =>
                {
                    actionResult = await this.connectionStringFailoverPolicy.ExecuteAsync<T>(async () =>
                    {
                        // Make sure the command has been associated with a valid connection. If not, associate it with an opened SQL connection.
                        if (command.Connection == null)
                        {
                            // Open a new connection and assign it to the command object.
                            command.Connection = await this.OpenAsync();
                            hasOpenedConnection = true;
                        }

                        // Verify whether or not the connection is valid and is open. This code may be retried therefore
                        // it is important to ensure that a connection is re-established should it have previously failed.
                        if (command.Connection.State != ConnectionState.Open)
                        {
                            await command.Connection.OpenAsync();
                            hasOpenedConnection = true;
                        }

                        if (typeof(IDataReader).IsAssignableFrom(resultType))
                        {
                            closeOpenedConnectionOnSuccess = false;

                            return (T)Convert.ChangeType(await command.ExecuteReaderAsync(behavior), resultType);
                        }

                        if (resultType == typeof(XmlReader))
                        {
                            return await ExecuteXmlReaderAsync<T>(command, behavior, closeOpenedConnectionOnSuccess);
                        }

                        if (resultType == typeof(NonQueryResult))
                        {
                            var result = new NonQueryResult { RecordsAffected = await command.ExecuteNonQueryAsync() };

                            closeOpenedConnectionOnSuccess = true;

                            return (T)Convert.ChangeType(result, resultType, CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            var result = await command.ExecuteScalarAsync();

                            closeOpenedConnectionOnSuccess = true;

                            if (result != null)
                            {
                                return (T)Convert.ChangeType(result, resultType, CultureInfo.InvariantCulture);
                            }

                            return default(T);
                        }
                    });
                });

                if (hasOpenedConnection && closeOpenedConnectionOnSuccess &&
                   command.Connection != null && command.Connection.State == ConnectionState.Open)
                {
                    command.Connection.Close();
                }
            }
            catch (Exception)
            {
                if (hasOpenedConnection &&
                   command.Connection != null && command.Connection.State == ConnectionState.Open)
                {
                    command.Connection.Close();
                }

                throw;
            }

            return actionResult;
        }

        /// <summary>
        /// Executes the XML reader.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command">The command.</param>
        /// <param name="behavior">The behavior.</param>
        /// <param name="closeOpenedConnectionOnSuccess">if set to <c>true</c> [close opened connection on success].</param>
        /// <returns>T.</returns>
        protected virtual T ExecuteXmlReader<T>(IDbCommand command, CommandBehavior behavior, bool closeOpenedConnectionOnSuccess)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Executes the XML reader.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command">The command.</param>
        /// <param name="behavior">The behavior.</param>
        /// <param name="closeOpenedConnectionOnSuccess">if set to <c>true</c> [close opened connection on success].</param>
        /// <returns>T.</returns>
        protected virtual Task<T> ExecuteXmlReaderAsync<T>(IDbCommand command, CommandBehavior behavior, bool closeOpenedConnectionOnSuccess)
        {
            throw new NotSupportedException();
        }


        /// <summary>
        /// Executes a SQL command and returns the number of rows affected.
        /// </summary>
        /// <param name="command">The SQL command to be executed.</param>
        /// <returns>The number of rows affected.</returns>
        public int ExecuteCommand(IDbCommand command)
        {
            return this.ExecuteCommand(command, this.CommandRetryPolicy);
        }

        /// <summary>
        /// Executes a SQL command and returns the number of rows affected.
        /// </summary>
        /// <param name="command">The SQL command to be executed.</param>
        /// <param name="retryPolicy">The retry policy that defines whether to retry a command if a connection fails while executing the command.</param>
        /// <returns>The number of rows affected.</returns>
        public int ExecuteCommand(IDbCommand command, RetryPolicy retryPolicy)
        {
            var result = this.ExecuteCommand<NonQueryResult>(command, retryPolicy);

            return result.RecordsAffected;
        }
        #endregion

        #region IDbConnection implementation
        /// <summary>
        /// Begins a database transaction with the specified System.Data.IsolationLevel value.
        /// </summary>
        /// <param name="il">One of the enumeration values that specifies the isolation level for the transaction.</param>
        /// <returns>An object that represents the new transaction.</returns>
        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            return this.underlyingConnection.BeginTransaction(il);
        }

        /// <summary>
        /// Begins a database transaction.
        /// </summary>
        /// <returns>An object that represents the new transaction.</returns>
        public IDbTransaction BeginTransaction()
        {
            return this.underlyingConnection.BeginTransaction();
        }

        /// <summary>
        /// Changes the current database for an open Connection object.
        /// </summary>
        /// <param name="databaseName">The name of the database to use in place of the current database.</param>
        public void ChangeDatabase(string databaseName)
        {
            this.underlyingConnection.ChangeDatabase(databaseName);
        }

        /// <summary>
        /// Opens a database connection with the settings specified by the ConnectionString
        /// property of the provider-specific Connection object.
        /// </summary>
        void IDbConnection.Open()
        {
            this.Open();
        }

        /// <summary>
        /// Closes the connection to the database.
        /// </summary>
        public void Close()
        {
            this.underlyingConnection.Close();
        }

        /// <summary>
        /// Creates and returns a SqlCommand object that is associated with the underlying SqlConnection.
        /// </summary>
        /// <returns>A System.Data.SqlClient.SqlCommand object that is associated with the underlying connection.</returns>
        public DbCommand CreateCommand()
        {
            return this.underlyingConnection.CreateCommand();
        }

        /// <summary>
        /// Creates and returns an object that implements the IDbCommand interface that is associated 
        /// with the underlying SqlConnection.
        /// </summary>
        /// <returns>A System.Data.SqlClient.SqlCommand object that is associated with the underlying connection.</returns>
        IDbCommand IDbConnection.CreateCommand()
        {
            return this.underlyingConnection.CreateCommand();
        }
        #endregion

        #region ICloneable implementation
        /// <summary>
        /// Creates a new connection that is a copy of the current instance, including the connection
        /// string, connection retry policy, and command retry policy.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        object ICloneable.Clone()
        {
            return this.Clone();
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>ReliableDbConnection.</returns>
        protected abstract ReliableDbConnection Clone();
        #endregion

        #region IDisposable implementation
        /// <summary>
        /// Performs application-defined tasks that are associated with freeing, releasing, or
        /// resetting managed and unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or
        /// resetting managed and unmanaged resources.
        /// </summary>
        /// <param name="disposing">A flag indicating that managed resources must be released.</param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.underlyingConnection.State == ConnectionState.Open)
                {
                    this.underlyingConnection.Close();
                }

                this.underlyingConnection.Dispose();
            }
        }
        #endregion

        /// <summary>
        /// Creates the connection.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>DbConnection.</returns>
        protected abstract DbConnection CreateConnection(string connectionString);

        #region Private helper classes
        /// <summary>
        /// This helpers class is intended to be used exclusively for fetching the number of affected records when executing a command by using ExecuteNonQuery.
        /// </summary>
        private sealed class NonQueryResult
        {
            public int RecordsAffected { get; set; }
        }
        #endregion
    }
}