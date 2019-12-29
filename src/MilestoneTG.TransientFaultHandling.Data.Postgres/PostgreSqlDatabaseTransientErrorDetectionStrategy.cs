using System;
using Npgsql;

namespace MilestoneTG.TransientFaultHandling.Data.Postgres
{
    /// <summary>
    /// Provides the transient error detection logic for transient faults that are specific to SQL Database.
    /// </summary>
    public sealed class PostgreSqlDatabaseTransientErrorDetectionStrategy : ITransientErrorDetectionStrategy
    {
        /// <summary>
        /// Determines whether the specified exception represents a transient failure that can be compensated by a retry.
        /// </summary>
        /// <param name="ex">The exception object to be verified.</param>
        /// <returns>true if the specified exception is considered as transient; otherwise, false.</returns>
        public bool IsTransient(Exception ex)
        {
            if (ex != null)
            {
                NpgsqlException sqlException;
                if ((sqlException = ex as NpgsqlException) != null)
                {
                    return sqlException.IsTransient;
                }
                else if (ex is TimeoutException)
                {
                    return true;
                }
            }

            return false;
        }
    }
}