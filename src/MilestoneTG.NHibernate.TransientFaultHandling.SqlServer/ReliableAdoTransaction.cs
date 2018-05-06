using NHibernate;
using NHibernate.Engine;
using NHibernate.Transaction;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace MilestoneTG.NHibernate.TransientFaultHandling.SqlServer
{
    /// <summary>
    /// Provides a transaction implementation that includes transient fault-handling retry logic.
    /// </summary>
    public class ReliableAdoTransaction : AdoTransaction, ITransaction
    {
        private readonly ISessionImplementor _session;

        /// <summary>
        /// Constructs a <see cref="ReliableAdoTransaction"/>.
        /// </summary>
        /// <param name="session">NHibernate session to use.</param>
        public ReliableAdoTransaction(ISessionImplementor session) : base(session)
        {
            _session = session;
        }

        /// <summary>
        /// Begins the transaction.
        /// </summary>
        public new void Begin()
        {
            Begin(IsolationLevel.Unspecified);
        }

        /// <summary>
        /// Begins the <see cref="T:System.Data.Common.DbTransaction" /> on the <see cref="T:System.Data.Common.DbConnection" />
        /// used by the <see cref="T:NHibernate.ISession" />.
        /// </summary>
        /// <param name="isolationLevel">The isolation level.</param>
        public new void Begin(IsolationLevel isolationLevel)
        {
            ExecuteWithRetry(_session.Connection as ReliableSqlDbConnection, () => base.Begin(isolationLevel));
        }

        /// <summary>
        /// Executes the given action with the command retry policy on the given <see cref="ReliableSqlDbConnection"/>.
        /// </summary>
        /// <param name="connection">The reliable connection</param>
        /// <param name="action">The action to execute</param>
        public static void ExecuteWithRetry(ReliableSqlDbConnection connection, System.Action action)
        {
            connection.ReliableConnection.CommandRetryPolicy.ExecuteAction(() =>
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                action();
            }
            );
        }
    }

}
