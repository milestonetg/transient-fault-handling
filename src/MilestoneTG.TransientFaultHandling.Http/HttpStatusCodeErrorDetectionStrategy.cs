// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Net;

namespace MilestoneTG.TransientFaultHandling.Http
{
    /// <summary>
    /// Default Http error detection strategy based on Http Status Code.
    /// </summary>
    /// <remarks>
    /// Forked from Microsoft.Rest.ClientRuntime.
    /// In Microsoft's original implementation, 500-Server Error was considered transient.
    /// Unless you're running at Netflix scale and velocity, we have found that it is
    /// more likely that a HTTP 500 error will not be retry-able within the scope of a 
    /// retry policy. We recommend HTTP 500 errors be handled with a circuit breaker.
    /// </remarks>
    public class HttpStatusCodeErrorDetectionStrategy : ITransientErrorDetectionStrategy
    {
        bool includeTimeouts;
        bool includeServerError;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpStatusCodeErrorDetectionStrategy"/> class.
        /// </summary>
        public HttpStatusCodeErrorDetectionStrategy() : this(true, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpStatusCodeErrorDetectionStrategy"/> class using the supplied options.
        /// </summary>
        /// <param name="options">The options.</param>
        public HttpStatusCodeErrorDetectionStrategy(HttpRetryPolicyOptions options)
            :this(options.IncludeTimeouts, options.IncludeServerErrors)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpStatusCodeErrorDetectionStrategy"/> class.
        /// </summary>
        /// <param name="includeTimeouts">if set to <c>true</c>, request timeouts and gateway timeouts will be considered transient. Default true.</param>
        /// <param name="includeServerError">if set to <c>true</c>, HTTP 500-Internal Server Error will be considered transient. Default false.</param>
        public HttpStatusCodeErrorDetectionStrategy(bool includeTimeouts, bool includeServerError)
        {
            this.includeTimeouts = includeTimeouts;
            this.includeServerError = includeServerError;
        }

        /// <summary>
        /// Returns true if there is a network connection failure, interruption, or
        /// status code in HttpRequestExceptionWithStatus exception is 
        /// 408-Request Timeout, 502-Bad Gateway, 503-Service Unavailable, 
        /// or 504-Gateway Timeout.
        /// </summary>
        /// <param name="ex">Exception to check against.</param>
        /// <returns>True if exception is transient otherwise false.</returns>
        public bool IsTransient(Exception ex)
        {
            if (ex != null)
            {
                if (ex is HttpRequestWithStatusException httpException)
                {
                    switch(httpException.StatusCode)
                    {
                        case HttpStatusCode.RequestTimeout when includeTimeouts:
                        case HttpStatusCode.GatewayTimeout when includeTimeouts:
                        case HttpStatusCode.InternalServerError when includeServerError:
                        case HttpStatusCode.BadGateway:
                        case HttpStatusCode.ServiceUnavailable:
                            return true;
                    }
                }

#if !NETSTANDARD1_1
                // if the inner exception is a WebException, check for network failures
                if (ex.InnerException is WebException webException)
                {
                    switch (webException.Status)
                    {
                        case WebExceptionStatus.ConnectFailure:
                        case WebExceptionStatus.ConnectionClosed:
                        case WebExceptionStatus.SendFailure:
                            return true;
                    }
                }
#endif
            }
            return false;
        }
    }
}