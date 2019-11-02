namespace MilestoneTG.TransientFaultHandling.Http
{
    /// <summary>
    /// Class HttpRetryPolicyOptions.
    /// Implements the <see cref="MilestoneTG.TransientFaultHandling.RetryPolicyOptions" />
    /// </summary>
    /// <seealso cref="MilestoneTG.TransientFaultHandling.RetryPolicyOptions" />
    public class HttpRetryPolicyOptions : RetryPolicyOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether to include timeouts as transient errors.
        /// </summary>
        /// <value>if true, timeouts will be considered transient faults. Default: true</value>
        public bool IncludeTimeouts { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to include server errors as transient errors.
        /// </summary>
        /// <value>if true, server errors will be considered transient faults. Default: false</value>
        public bool IncludeServerErrors { get; set; } = false;
    }
}
