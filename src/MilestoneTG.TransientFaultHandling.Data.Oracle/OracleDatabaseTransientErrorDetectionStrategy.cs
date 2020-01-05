using Oracle.ManagedDataAccess.Client;
using System;

namespace MilestoneTG.TransientFaultHandling.Data.Oracle
{
    /// <summary>
    /// Provides the transient error detection logic for transient faults that are specific to SQL Database.
    /// </summary>
    public sealed class OracleDatabaseTransientErrorDetectionStrategy : ITransientErrorDetectionStrategy
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
                OracleException sqlException;
                if ((sqlException = ex as OracleException) != null)
                {
                    foreach (OracleError error in sqlException.Errors)
                    {
                        // There are thousands of ORA error codes. These are a few network level ones that should
                        // be retry-able.
                        switch (error.Number)
                        {
                            case 39:
                            case 317:
                            case 539:
                            case 12150:
                            case 12152:
                            case 12161:
                            case 12200:
                            case 12224: //no listener
                            case 12225: //host unreachable
                            case 12231:
                            case 12233:
                            case 12234:
                            case 12531:
                            case 12535: //timeout
                            case 12540: //too many connections
                            case 12541: //no listener
                            case 12560: //protocol adapter error
                            case 12636: //packet send failed
                            case 12637: //packet receive failed
                                return true;
                        }
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