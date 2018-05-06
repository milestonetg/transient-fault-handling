# MilestoneTG.TransientFaultHandling

Core transient fault handling interfaces and retry strategies based on Microsoft's transient fault handling provided in 
Microsoft.Rest.ClientRuntime.

Common Types:
* RetryPolicy
* ITransientErrorDetectionStrategy
* IncrementalRetryStrategy
* FixedIntervalRetryStrategy
* ExponentialBackoffRetryStrategy

Used by the other MilestoneTG.TransientFaultHandling.* packages.