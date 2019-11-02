using System;

namespace MilestoneTG.TransientFaultHandling
{
    /// <summary>
    /// Supports creating a retry policy using configuration.
    /// </summary>
    public class RetryPolicyOptions
    {

        /// <summary>
        /// Exponential backoff strategy
        /// </summary>
        public const string EXPONENTIAL = "Exponential";
        
        /// <summary>
        /// Incremental strategy
        /// </summary>
        public const string INCREMENTAL = "Incremental";

        /// <summary>
        /// Fixed interval strategy
        /// </summary>
        public const string FIXED = "Fixed";

        /// <summary>
        /// Gets or sets whether or not retry is enabled. Setting to false sets the 
        /// strategy to NoRetry.
        /// </summary>
        /// <value></value>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the name of the retry strategy. Value must be one of the consts.
        /// </summary>
        /// <value>The name of the retry strategy.</value>
        public string RetryStrategyName { get; set; }

        /// <summary>
        /// Gets or sets the retry count.
        /// </summary>
        /// <value>The retry count.</value>
        public int RetryCount { get; set; } = 3;

        /// <summary>
        /// Gets or sets a value indicating whether [first fast retry].
        /// </summary>
        /// <value>if true, the first retry will be immediate and the remainder will be subject to the policy. Default: true.</value>
        public bool FirstFastRetry { get; set; } = true;

        /// <summary>
        /// Gets or sets the interval for incremental and fixed strategies.
        /// </summary>
        /// <value>The interval.</value>
        public TimeSpan Interval { get; set; } = TimeSpan.FromSeconds(1.0);

        /// <summary>
        /// Gets or sets the increment for the incremental strategy.
        /// </summary>
        /// <value>The increment.</value>
        public TimeSpan Increment { get; set; } = TimeSpan.FromSeconds(1.0);

        /// <summary>
        /// Gets or sets the minimum backoff for the exponential strategy.
        /// </summary>
        /// <value>The minimum backoff.</value>
        public TimeSpan MinBackoff { get; set; } = TimeSpan.FromSeconds(1.0);

        /// <summary>
        /// Gets or sets the maximum backoff for the exponential strategy.
        /// </summary>
        /// <value>The maximum backoff.</value>
        public TimeSpan MaxBackoff { get; set; } = TimeSpan.FromSeconds(10.0);

        /// <summary>
        /// Gets or sets the delta backoff for the exponential strategy.
        /// </summary>
        /// <value>The delta backoff.</value>
        public TimeSpan DeltaBackoff { get; set; } = TimeSpan.FromSeconds(10.0);
    }
}
