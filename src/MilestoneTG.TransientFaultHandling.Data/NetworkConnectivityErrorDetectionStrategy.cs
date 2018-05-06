using System;
using System.Collections.Generic;
using System.Text;

namespace MilestoneTG.TransientFaultHandling.Data
{
    /// <summary>
    /// Implements a strategy that detects network connectivity errors such as "host not found".
    /// </summary>
    public abstract class NetworkConnectivityErrorDetectionStrategy : ITransientErrorDetectionStrategy
    {
        /// <summary>
        /// Determines whether the specified exception represents a transient failure that
        /// can be compensated by a retry.
        /// </summary>
        /// <param name="ex">The exception object to be verified.</param>
        /// <returns>true if the specified exception is considered as transient; otherwise,
        /// false.</returns>
        public abstract bool IsTransient(Exception ex);
    }
}
