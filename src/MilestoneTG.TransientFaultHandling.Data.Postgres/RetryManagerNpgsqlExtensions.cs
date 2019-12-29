namespace MilestoneTG.TransientFaultHandling.Data.Postgres
{
    using System;

    /// <summary>
    /// Extends the <see cref="RetryManager"/> class to use it with the Npgsql Database retry strategy.
    /// </summary>
    public static class RetryManagerNpgsqlExtensions
    {
        /// <summary>
        /// The technology name that can be used to get the default Npgsql command retry strategy.
        /// </summary>
        public const string DefaultStrategyCommandTechnologyName = "Npgsql";

        /// <summary>
        /// The technology name that can be used to get the default Npgsql connection retry strategy.
        /// </summary>
        public const string DefaultStrategyConnectionTechnologyName = "NpgsqlConnection";

        /// <summary>
        /// Returns the default retry strategy for Npgsql commands.
        /// </summary>
        /// <returns>The default retry strategy for Npgsql commands (or the default strategy, if no default could be found).</returns>
        public static RetryStrategy GetDefaultNpgsqlCommandRetryStrategy(this RetryManager retryManager)
        {
            if (retryManager == null) throw new ArgumentNullException("retryManager");

            return retryManager.GetDefaultRetryStrategy(DefaultStrategyCommandTechnologyName);
        }

        /// <summary>
        /// Returns the default retry policy dedicated to handling transient conditions with Npgsql commands.
        /// </summary>
        /// <returns>The retry policy for Npgsql commands with the corresponding default strategy (or the default strategy, if no retry strategy assigned to Npgsql commands was found).</returns>
        public static RetryPolicy GetDefaultNpgsqlCommandRetryPolicy(this RetryManager retryManager)
        {
            if (retryManager == null) throw new ArgumentNullException("retryManager");

            return new RetryPolicy(new PostgreSqlDatabaseTransientErrorDetectionStrategy(), retryManager.GetDefaultNpgsqlCommandRetryStrategy());
        }

        /// <summary>
        /// Returns the default retry strategy for Npgsql connections.
        /// </summary>
        /// <returns>The default retry strategy for Npgsql connections (or the default strategy, if no default could be found).</returns>
        public static RetryStrategy GetDefaultNpgsqlConnectionRetryStrategy(this RetryManager retryManager)
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
        /// Returns the default retry policy dedicated to handling transient conditions with Npgsql connections.
        /// </summary>
        /// <returns>The retry policy for Npgsql connections with the corresponding default strategy (or the default strategy, if no retry strategy for Npgsql connections was found).</returns>
        public static RetryPolicy GetDefaultNpgsqlConnectionRetryPolicy(this RetryManager retryManager)
        {
            if (retryManager == null) throw new ArgumentNullException("retryManager");

            return new RetryPolicy(new PostgreSqlDatabaseTransientErrorDetectionStrategy(), retryManager.GetDefaultNpgsqlConnectionRetryStrategy());
        }
    }
}
