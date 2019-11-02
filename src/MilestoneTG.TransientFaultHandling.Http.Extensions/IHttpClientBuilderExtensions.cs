using Microsoft.Extensions.DependencyInjection;
using System;

namespace MilestoneTG.TransientFaultHandling.Http.Extensions
{
    /// <summary>
    /// Extension methods for IHttpClientBuilder to support transient retries.
    /// </summary>
    public static class IHttpClientBuilderExtensions
    {
        /// <summary>
        /// Adds the retry handler.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>IHttpClientBuilder.</returns>
        /// <exception cref="System.ArgumentNullException">builder</exception>
        public static IHttpClientBuilder AddRetryHandler(this IHttpClientBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }


            builder.AddHttpMessageHandler(() => new RetryDelegatingHandler());

            return builder;
        }

        /// <summary>
        /// Adds the retry handler.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="retryPolicy">The retry policy.</param>
        /// <returns>IHttpClientBuilder.</returns>
        /// <exception cref="System.ArgumentNullException">builder</exception>
        /// <exception cref="System.ArgumentNullException">retryPolicy</exception>
        public static IHttpClientBuilder AddRetryHandler(this IHttpClientBuilder builder, RetryPolicy retryPolicy)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (retryPolicy == null)
            {
                throw new ArgumentNullException(nameof(retryPolicy));
            }

            builder.AddHttpMessageHandler(() => new RetryDelegatingHandler(retryPolicy));

            return builder;
        }

        /// <summary>
        /// Adds the retry handler.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="retryPolicyOptions">The retry policy options.</param>
        /// <returns>IHttpClientBuilder.</returns>
        /// <exception cref="System.ArgumentNullException">builder</exception>
        /// <exception cref="System.ArgumentNullException">retryPolicyOptions</exception>
        public static IHttpClientBuilder AddRetryHandler(this IHttpClientBuilder builder, HttpRetryPolicyOptions retryPolicyOptions)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (retryPolicyOptions == null)
            {
                throw new ArgumentNullException(nameof(retryPolicyOptions));
            }

            builder.AddHttpMessageHandler(() => new RetryDelegatingHandler(retryPolicyOptions));

            return builder;
        }

        /// <summary>
        /// Adds the retry handler.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="configure">The configure.</param>
        /// <returns>IHttpClientBuilder.</returns>
        /// <exception cref="System.ArgumentNullException">builder</exception>
        /// <exception cref="System.ArgumentNullException">configure</exception>
        public static IHttpClientBuilder AddRetryHandler(this IHttpClientBuilder builder, Action<HttpRetryPolicyOptions> configure)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            HttpRetryPolicyOptions retryPolicyOptions = new HttpRetryPolicyOptions();
            
            configure(retryPolicyOptions);
            
            builder.AddHttpMessageHandler(() => new RetryDelegatingHandler(retryPolicyOptions));

            return builder;
        }
    }
}
