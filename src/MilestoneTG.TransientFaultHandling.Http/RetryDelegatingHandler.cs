// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MilestoneTG.TransientFaultHandling.Http
{
    /// <summary>
    /// Http retry handler.
    /// </summary>
    public class RetryDelegatingHandler : DelegatingHandler
    {
        private const int DefaultNumberOfAttempts = 3;
        private readonly TimeSpan DefaultBackoffDelta = new TimeSpan(0, 0, 10);
        private readonly TimeSpan DefaultMaxBackoff = new TimeSpan(0, 0, 10);
        private readonly TimeSpan DefaultMinBackoff = new TimeSpan(0, 0, 1);


        /// <summary>
        /// Initializes a new instance of the <see cref="RetryDelegatingHandler"/> class using the supplied options. 
        /// </summary>
        public RetryDelegatingHandler(HttpRetryPolicyOptions options)
        {
            RetryPolicy = new RetryPolicy(new HttpStatusCodeErrorDetectionStrategy(options), options);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RetryDelegatingHandler"/> class. 
        /// Sets default retry policy base on Exponential Backoff.
        /// </summary>
        public RetryDelegatingHandler() 
        {
            var retryStrategy = new ExponentialBackoffRetryStrategy(
                DefaultNumberOfAttempts,
                DefaultMinBackoff,
                DefaultMaxBackoff,
                DefaultBackoffDelta);
            RetryPolicy = new RetryPolicy<HttpStatusCodeErrorDetectionStrategy>(retryStrategy);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RetryDelegatingHandler"/> class. Sets 
        /// the default retry policy base on Exponential Backoff.
        /// Sets the InnerHandler to the provided HttpMessageHandler.
        /// </summary>
        /// <param name="innerHandler">Inner http handler.</param>
        public RetryDelegatingHandler(HttpMessageHandler innerHandler)
            : this()
        {
            InnerHandler = innerHandler;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RetryDelegatingHandler"/> class. 
        /// </summary>
        /// <param name="retryPolicy">Retry policy to use.</param>
        public RetryDelegatingHandler(RetryPolicy retryPolicy)
        {
            RetryPolicy = retryPolicy ?? throw new ArgumentNullException("retryPolicy");

            RetryPolicy.Retrying += (sender, args) =>
            {
                internalRetrying?.Invoke(sender, args);
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RetryDelegatingHandler"/> class with the provided
        /// RetryPolicy and sets the InnerHandler to the provided HttpMessageHandler.
        /// </summary>
        /// <param name="retryPolicy">Retry policy to use.</param>
        /// <param name="innerHandler">Inner http handler.</param>
        public RetryDelegatingHandler(RetryPolicy retryPolicy, HttpMessageHandler innerHandler)
            : this(retryPolicy)
        {
            InnerHandler = innerHandler;
        }

        /// <summary>
        /// Gets or sets retry policy.
        /// </summary>
        public RetryPolicy RetryPolicy { get; set; }

        /// <summary>
        /// Sends an HTTP request to the inner handler to send to the server as an asynchronous
        /// operation. Retries request if needed based on Retry Policy.
        /// </summary>
        /// <param name="request">The HTTP request message to send to the server.</param>
        /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
        /// <returns>Returns System.Threading.Tasks.Task&lt;TResult&gt;. The 
        /// task object representing the asynchronous operation.</returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            HttpResponseMessage responseMessage = null;
            try
            {
                await RetryPolicy.ExecuteAsync(async () =>
                {
                    responseMessage = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

                    if (!responseMessage.IsSuccessStatusCode)
                    {
                        // dispose the message unless we have stopped retrying
                        this.internalRetrying += (sender, args) =>
                        {
                            if (responseMessage != null)
                            {
                                responseMessage.Dispose();
                                responseMessage = null;
                            }
                        };

                        throw new HttpRequestWithStatusException(string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.ResponseStatusCodeError,
                            (int) responseMessage.StatusCode,
                            responseMessage.StatusCode)) {StatusCode = responseMessage.StatusCode};
                    }

                    return responseMessage;
                }, cancellationToken).ConfigureAwait(false);

                return responseMessage;
            }
            catch
            {
                if (responseMessage != null)
                {
                    return responseMessage;
                }
                else
                {
                    throw;
                }
            }
            finally
            {
                if (internalRetrying != null)
                {
                    foreach (EventHandler<RetryingEventArgs> d in internalRetrying.GetInvocationList())
                    {
                        internalRetrying -= d;
                    }
                }
            }
        }

        /// <summary>
        /// An instance of a callback delegate that will be invoked whenever a retry condition is encountered.
        /// </summary>
        private event EventHandler<RetryingEventArgs> internalRetrying;
    }
}