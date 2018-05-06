using MilestoneTG.TransientFaultHandling.Data.SqlServer;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace MilestoneTG.NHibernate.TransientFaultHandling.SqlServer
{
    /// <summary>
    /// An <see cref="IDbCommand" /> implementation that wraps a <see cref="SqlCommand" /> object such that any
    /// queries that are executed are executed via a <see cref="ReliableSqlConnection" />.
    /// </summary>
    /// <seealso cref="System.Data.Common.DbCommand" />
    /// <remarks>Note: For this to work it requires that the Connection property be set with a <see cref="ReliableSqlConnection" /> object.</remarks>
    public class ReliableSqlCommand : DbCommand
    {
        /// <summary>
        /// The underlying <see cref="SqlCommand" /> being proxied.
        /// </summary>
        /// <value>The current.</value>
        public System.Data.SqlClient.SqlCommand Current { get; private set; }

        /// <summary>
        /// The <see cref="ReliableSqlConnection" /> that has been assigned to the command via the Connection property.
        /// </summary>
        /// <value>The reliable connection.</value>
        public ReliableSqlConnection ReliableConnection { get; set; }

        /// <summary>
        /// Constructs a <see cref="ReliableSqlCommand" />.
        /// </summary>
        public ReliableSqlCommand()
        {
            Current = new System.Data.SqlClient.SqlCommand();
        }

        /// <summary>
        /// Explicit type-casting between a <see cref="ReliableSqlCommand" /> and a <see cref="SqlCommand" />.
        /// </summary>
        /// <param name="command">The <see cref="ReliableSqlCommand" /> being casted</param>
        /// <returns>The underlying <see cref="SqlCommand" /> being proxied.</returns>
        public static explicit operator System.Data.SqlClient.SqlCommand(ReliableSqlCommand command)
        {
            return command.Current;
        }

        /// <summary>
        /// Returns the underlying <see cref="SqlConnection" /> and expects a <see cref="ReliableSqlConnection" /> when being set.
        /// </summary>
        /// <value>The database connection.</value>
        protected override DbConnection DbConnection
        {
            get { return Current.Connection; }
            set
            {
                ReliableConnection = ((ReliableSqlDbConnection)value).ReliableConnection;
                Current.Connection = ReliableConnection.Current as SqlConnection;
            }
        }


        #region Wrapping code

        /// <summary>
        /// Creates a prepared (or compiled) version of the command on the data source.
        /// </summary>
        public override void Prepare()
        {
            Current.Prepare();
        }

        /// <summary>
        /// Attempts to cancels the execution of a <see cref="T:System.Data.Common.DbCommand"></see>.
        /// </summary>
        public override void Cancel()
        {
            Current.Cancel();
        }

        /// <summary>
        /// Creates a new instance of a <see cref="T:System.Data.Common.DbParameter"></see> object.
        /// </summary>
        /// <returns>A <see cref="T:System.Data.Common.DbParameter"></see> object.</returns>
        protected override DbParameter CreateDbParameter()
        {
            return Current.CreateParameter();
        }

        /// <summary>
        /// Executes a SQL statement against a connection object.
        /// </summary>
        /// <returns>The number of rows affected.</returns>
        public override int ExecuteNonQuery()
        {
            return ReliableConnection.ExecuteCommand(Current);
        }

        /// <summary>
        /// Executes the <see cref="P:System.Data.IDbCommand.CommandText"></see> against the <see cref="P:System.Data.IDbCommand.Connection"></see> and builds an <see cref="T:System.Data.IDataReader"></see>.
        /// </summary>
        /// <returns>An <see cref="T:System.Data.IDataReader"></see> object.</returns>
        public new IDataReader ExecuteReader()
        {
            return ReliableConnection.ExecuteCommand<IDataReader>(Current);
        }

        /// <summary>
        /// Executes the command text against the connection.
        /// </summary>
        /// <param name="behavior">An instance of <see cref="T:System.Data.CommandBehavior"></see>.</param>
        /// <returns>A task representing the operation.</returns>
        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            return ReliableConnection.ExecuteCommand<DbDataReader>(Current, behavior);
        }

        /// <summary>
        /// Executes the query and returns the first column of the first row in the result set returned by the query. All other columns and rows are ignored.
        /// </summary>
        /// <returns>The first column of the first row in the result set.</returns>
        public override object ExecuteScalar()
        {
            return ReliableConnection.ExecuteCommand<int>(Current);
        }

        /// <summary>
        /// Gets or sets the <see cref="P:System.Data.Common.DbCommand.DbTransaction"></see> within which this <see cref="T:System.Data.Common.DbCommand"></see> object executes.
        /// </summary>
        /// <value>The database transaction.</value>
        protected override DbTransaction DbTransaction
        {
            get { return Current.Transaction; }
            set { Current.Transaction = (SqlTransaction)value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the command object should be visible in a customized interface control.
        /// </summary>
        /// <value><c>true</c> if [design time visible]; otherwise, <c>false</c>.</value>
        public override bool DesignTimeVisible
        {
            get { return Current.DesignTimeVisible; }
            set { Current.DesignTimeVisible = value; }
        }

        /// <summary>
        /// Gets or sets the text command to run against the data source.
        /// </summary>
        /// <value>The command text.</value>
        public override string CommandText
        {
            get { return Current.CommandText; }
            set { Current.CommandText = value; }
        }

        /// <summary>
        /// Gets or sets the wait time before terminating the attempt to execute a command and generating an error.
        /// </summary>
        /// <value>The command timeout.</value>
        public override int CommandTimeout
        {
            get { return Current.CommandTimeout; }
            set { Current.CommandTimeout = value; }
        }

        /// <summary>
        /// Indicates or specifies how the <see cref="P:System.Data.Common.DbCommand.CommandText"></see> property is interpreted.
        /// </summary>
        /// <value>The type of the command.</value>
        public override CommandType CommandType
        {
            get { return Current.CommandType; }
            set { Current.CommandType = value; }
        }

        /// <summary>
        /// Gets the collection of <see cref="T:System.Data.Common.DbParameter"></see> objects.
        /// </summary>
        /// <value>The database parameter collection.</value>
        protected override DbParameterCollection DbParameterCollection
        {
            get { return Current.Parameters; }
        }

        /// <summary>
        /// Gets or sets how command results are applied to the <see cref="T:System.Data.DataRow"></see> when used by the Update method of a <see cref="T:System.Data.Common.DbDataAdapter"></see>.
        /// </summary>
        /// <value>The updated row source.</value>
        public override UpdateRowSource UpdatedRowSource
        {
            get { return Current.UpdatedRowSource; }
            set { Current.UpdatedRowSource = value; }
        }
        #endregion
    }

}
