namespace MilestoneTG.TransientFaultHandling.Data.Oracle
{
    using System;

    /// <summary>
    /// Extends the <see cref="RetryManager"/> class to use it with the Oracle Database retry strategy.
    /// </summary>
    public static class RetryManagerOracleExtensions
    {
        /// <summary>
        /// The technology name that can be used to get the default Oracle command retry strategy.
        /// </summary>
        public const string DefaultStrategyCommandTechnologyName = "Oracle";

        /// <summary>
        /// The technology name that can be used to get the default Oracle connection retry strategy.
        /// </summary>
        public const string DefaultStrategyConnectionTechnologyName = "OracleConnection";

        /// <summary>
        /// Returns the default retry strategy for Oracle commands.
        /// </summary>
        /// <returns>The default retry strategy for Oracle commands (or the default strategy, if no default could be found).</returns>
        public static RetryStrategy GetDefaultOracleCommandRetryStrategy(this RetryManager retryManager)
        {
            if (retryManager == null) throw new ArgumentNullException("retryManager");

            return retryManager.GetDefaultRetryStrategy(DefaultStrategyCommandTechnologyName);
        }

        /// <summary>
        /// Returns the default retry policy dedicated to handling transient conditions with Oracle commands.
        /// </summary>
        /// <returns>The retry policy for Oracle commands with the corresponding default strategy (or the default strategy, if no retry strategy assigned to Oracle commands was found).</returns>
        public static RetryPolicy GetDefaultOracleCommandRetryPolicy(this RetryManager retryManager)
        {
            if (retryManager == null) throw new ArgumentNullException("retryManager");

            return new RetryPolicy(new OracleDatabaseTransientErrorDetectionStrategy(), retryManager.GetDefaultOracleCommandRetryStrategy());
        }

        /// <summary>
        /// Returns the default retry strategy for Oracle connections.
        /// </summary>
        /// <returns>The default retry strategy for Oracle connections (or the default strategy, if no default could be found).</returns>
        public static RetryStrategy GetDefaultOracleConnectionRetryStrategy(this RetryManager retryManager)
        {
            if (retryManager == null) throw new ArgumentNullException("retryManager");

            try
            {
                return retryManager.GetDefaultRetryStrategy(DefaultStrategyConnectionTechnologyName);
            }
            catch (ArgumentOutOfRangeException)
            {
                return retryManager.GetDefaultRetryStrategy(DefaultStrategyCommandTechnologyName);
            }
        }

        /// <summary>
        /// Returns the default retry policy dedicated to handling transient conditions with Oracle connections.
        /// </summary>
        /// <returns>The retry policy for Oracle connections with the corresponding default strategy (or the default strategy, if no retry strategy for Oracle connections was found).</returns>
        public static RetryPolicy GetDefaultOracleConnectionRetryPolicy(this RetryManager retryManager)
        {
            if (retryManager == null) throw new ArgumentNullException("retryManager");

            return new RetryPolicy(new OracleDatabaseTransientErrorDetectionStrategy(), retryManager.GetDefaultOracleConnectionRetryStrategy());
        }
    }
}
