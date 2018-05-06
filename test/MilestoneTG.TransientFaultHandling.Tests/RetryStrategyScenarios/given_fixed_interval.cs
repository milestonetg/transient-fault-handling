#region license
// ==============================================================================
// Microsoft patterns & practices Enterprise Library
// Transient Fault Handling Application Block
// ==============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
// ==============================================================================
#endregion

namespace MilestoneTG.TransientFaultHandling.Tests.RetryStrategyScenarios.given_fixed_interval
{
    using System;
    using Common.TestSupport.ContextBase;

    using MilestoneTG.TransientFaultHandling;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public abstract class Context : ArrangeActAssert
    {
        protected override void Arrange()
        {
        }
    }

    [TestClass]
    public class when_using_default_values : Context
    {
        protected RetryStrategy retryStrategy;
        protected ShouldRetryHandler shouldRetry;

        protected override void Act()
        {
            this.retryStrategy = new FixedIntervalRetryStrategy();
            this.shouldRetry = this.retryStrategy.GetShouldRetryHandler();
        }

        [TestMethod]
        public void then_default_values_are_used()
        {
            Assert.IsNull(retryStrategy.Name);

            RetryCondition c1 = shouldRetry(0, null);
            Assert.IsTrue(c1.RetryAllowed);
            Assert.AreEqual(TimeSpan.FromSeconds(1), c1.DelayBeforeRetry);

            RetryCondition c2 = shouldRetry(9, null);
            Assert.IsTrue(c2.RetryAllowed);
            Assert.AreEqual(TimeSpan.FromSeconds(1), c2.DelayBeforeRetry);

            RetryCondition c3 = shouldRetry(10, null);
            Assert.IsFalse(c3.RetryAllowed);
            Assert.AreEqual(TimeSpan.Zero, c3.DelayBeforeRetry);
        }
    }

    [TestClass]
    public class when_using_default_interval : Context
    {
        protected RetryStrategy retryStrategy;
        protected ShouldRetryHandler shouldRetry;

        protected override void Act()
        {
            this.retryStrategy = new FixedIntervalRetryStrategy(5);
            this.shouldRetry = this.retryStrategy.GetShouldRetryHandler();
        }

        [TestMethod]
        public void then_default_values_are_used()
        {
            Assert.IsNull(retryStrategy.Name);

            RetryCondition c1 = shouldRetry(0, null);
            Assert.IsTrue(c1.RetryAllowed);
            Assert.AreEqual(TimeSpan.FromSeconds(1), c1.DelayBeforeRetry);

            RetryCondition c2 = shouldRetry(4, null);
            Assert.IsTrue(c2.RetryAllowed);
            Assert.AreEqual(TimeSpan.FromSeconds(1), c2.DelayBeforeRetry);

            RetryCondition c3 = shouldRetry(5, null);
            Assert.IsFalse(c3.RetryAllowed);
            Assert.AreEqual(TimeSpan.Zero, c3.DelayBeforeRetry);
        }
    }

    [TestClass]
    public class when_using_custom_values : Context
    {
        protected RetryStrategy retryStrategy;
        protected ShouldRetryHandler shouldRetry;

        protected override void Act()
        {
            this.retryStrategy = new FixedIntervalRetryStrategy("name", 5, TimeSpan.FromSeconds(5));
            this.shouldRetry = this.retryStrategy.GetShouldRetryHandler();
        }

        [TestMethod]
        public void then_default_values_are_used()
        {
            Assert.AreEqual("name", retryStrategy.Name);

            RetryCondition c1 = shouldRetry(0, null);
            Assert.IsTrue(c1.RetryAllowed);
            Assert.AreEqual(TimeSpan.FromSeconds(5), c1.DelayBeforeRetry);

            RetryCondition c2 = shouldRetry(4, null);
            Assert.IsTrue(c2.RetryAllowed);
            Assert.AreEqual(TimeSpan.FromSeconds(5), c2.DelayBeforeRetry);

            RetryCondition c3 = shouldRetry(5, null);
            Assert.IsFalse(c3.RetryAllowed);
            Assert.AreEqual(TimeSpan.Zero, c3.DelayBeforeRetry);
        }
    }
}
