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

#pragma warning disable 618

namespace MilestoneTG.TransientFaultHandling.Tests.RetryPolicyScenarios.given_retry_limit_exceeded_exception_with_inner_exception
{
    using System;
    using System.Threading.Tasks;
    using MilestoneTG.Common.TestSupport.ContextBase;
    using MilestoneTG.TransientFaultHandling;
    using MilestoneTG.TransientFaultHandling.TestSupport;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    public abstract class Context : ArrangeActAssert
    {
        protected RetryPolicy retryPolicy;
        protected Mock<RetryStrategy> retryStrategyMock;

        protected override void Arrange()
        {
            this.retryStrategyMock = new Mock<RetryStrategy>("name", false);
            this.retryStrategyMock.Setup(x => x.GetShouldRetryHandler())
                .Returns(
                    () =>
                        delegate (int currentRetryCount, Exception lastException)
                        {
                            return new RetryCondition(currentRetryCount == 0, TimeSpan.Zero);
                        }
                    );
            this.retryPolicy = new RetryPolicy<AlwaysTransientErrorDetectionStrategy>(retryStrategyMock.Object);
        }
    }

    [TestClass]
    public class when_executing_action : Context
    {
        private int execCount;

        protected override void Act()
        {
            this.retryPolicy.ExecuteAction(
                () =>
                {
                    execCount++;
                    throw new RetryLimitExceededException();
                });
        }

        [TestMethod]
        public void then_does_not_retry()
        {
            Assert.AreEqual(1, this.execCount);
        }
    }

    [TestClass]
    public class when_executing_func : Context
    {
        private int execCount;

        protected override void Act()
        {
            this.retryPolicy.ExecuteAction<int>(
                () =>
                {
                    execCount++;
                    throw new RetryLimitExceededException();
                });
        }

        [TestMethod]
        public void then_does_not_retry()
        {
            Assert.AreEqual(1, this.execCount);
        }
    }

    [TestClass]
    public class when_executing_async : Context
    {
        private int timesStarted;
        private Task task;
        private AggregateException exception;

        protected override void Act()
        {
            this.task = this.retryPolicy
                .ExecuteAsync(() =>
                {
                    int result = ++timesStarted;
                    return Task.Run<int>((Func<int>)(() => { throw new RetryLimitExceededException(); }));
                });

            try
            { 
                task.Wait(TimeSpan.FromSeconds(2));
            }
            catch (AggregateException e)
            {
                this.exception = e;
            }
        }

        [TestMethod]
        public void then_does_not_retry()
        {
            Assert.AreEqual(1, this.timesStarted);
        }

        //TODO: Verify these conditions are no longer valid under the new AsyncExecution
        // Task ends up faulted instead of canceled, which seems more appropriate.
        //[TestMethod]
        //public void then_is_canceled()
        //{
        //    Assert.AreEqual(TaskStatus.Canceled, this.task.Status);
        //}

        //[TestMethod]
        //public void then_exception_is_task_canceled_exception()
        //{
        //    Assert.IsInstanceOfType(this.exception.InnerException, typeof(TaskCanceledException));
        //}
    }
}
