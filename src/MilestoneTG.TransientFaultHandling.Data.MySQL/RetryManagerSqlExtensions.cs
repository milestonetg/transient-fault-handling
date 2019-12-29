namespace MilestoneTG.TransientFaultHandling.Data.MySql
{
    using System;

    /// <summary>
    /// Extends the <see cref="RetryManager"/> class to use it with the MySql Database retry strategy.
    /// </summary>
    public static class RetryManagerMySqlExtensions
    {
        /// <summary>
        /// The technology name that can be used to get the default MySql command retry strategy.
        /// </summary>
        public const string DefaultStrategyCommandTechnologyName = "MySql";

        /// <summary>
        /// The technology name that can be used to get the default MySql connection retry strategy.
        /// </summary>
        public const string DefaultStrategyConnectionTechnologyName = "MySqlConnection";

        /// <summary>
        /// Returns the default retry strategy for MySql commands.
        /// </summary>
        /// <returns>The default retry strategy for MySql commands (or the default strategy, if no default could be found).</returns>
        public static RetryStrategy GetDefaultMySqlCommandRetryStrategy(this RetryManager retryManager)
        {
            if (retryManager == null) throw new ArgumentNullException("retryManager");

            return retryManager.GetDefaultRetryStrategy(DefaultStrategyCommandTechnologyName);
        }

        /// <summary>
        /// Returns the default retry policy dedicated to handling transient conditions with MySql commands.
        /// </summary>
        /// <returns>The retry policy for MySql commands with the corresponding default strategy (or the default strategy, if no retry strategy assigned to MySql commands was found).</returns>
        public static RetryPolicy GetDefaultMySqlCommandRetryPolicy(this RetryManager retryManager)
        {
            if (retryManager == null) throw new ArgumentNullException("retryManager");

            return new RetryPolicy(new MySqlDatabaseTransientErrorDetectionStrategy(), retryManager.GetDefaultMySqlCommandRetryStrategy());
        }

        /// <summary>
        /// Returns the default retry strategy for MySql connections.
        /// </summary>
        /// <returns>The default retry strategy for MySql connections (or the default strategy, if no default could be found).</returns>
        public static RetryStrategy GetDefaultMySqlConnectionRetryStrategy(this RetryManager retryManager)
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
        /// Returns the default retry policy dedicated to handling transient conditions with MySql connections.
        /// </summary>
        /// <returns>The retry policy for MySql connections with the corresponding default strategy (or the default strategy, if no retry strategy for MySql connections was found).</returns>
        public static RetryPolicy GetDefaultMySqlConnectionRetryPolicy(this RetryManager retryManager)
        {
            if (retryManager == null) throw new ArgumentNullException("retryManager");

            return new RetryPolicy(new MySqlDatabaseTransientErrorDetectionStrategy(), retryManager.GetDefaultMySqlConnectionRetryStrategy());
        }
    }
}
