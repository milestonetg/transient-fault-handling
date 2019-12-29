using System;
using System.Data;
using MySql.Data.MySqlClient;
using MilestoneTG.TransientFaultHandling.Data;

namespace MilestoneTG.TransientFaultHandling.Data.MySql
{
    /// <summary>
    /// Provides the transient error detection logic for transient faults that are specific to SQL Database.
    /// </summary>
    public sealed class MySqlDatabaseTransientErrorDetectionStrategy : ITransientErrorDetectionStrategy
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
                MySqlException sqlException;
                if ((sqlException = ex as MySqlException) != null)
                {
                    switch ((MySqlErrorCode)sqlException.Number)
                    {
                        // Thrown if timer queue couldn't be cleared while reading sockets
                        case MySqlErrorCode.CommandTimeoutExpired:
                        // Unable to open connection
                        case MySqlErrorCode.UnableToConnectToHost:
                        // Too many connections
                        case MySqlErrorCode.ConnectionCountError:
                        // Lock wait timeout exceeded; try restarting transaction
                        case MySqlErrorCode.LockWaitTimeout:
                        // Deadlock found when trying to get lock; try restarting transaction
                        case MySqlErrorCode.LockDeadlock:
                        // Transaction branch was rolled back: deadlock was detected
                        case MySqlErrorCode.XARBDeadlock:
                            // Retry in all cases above
                            return true;
                    }
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