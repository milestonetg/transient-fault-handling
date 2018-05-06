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

using System;
using System.Data;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
    using MilestoneTG.TransientFaultHandling.Data;
    using MilestoneTG.TransientFaultHandling.Data.SqlServer;

namespace MilestoneTG.TransientFaultHandling.Tests
{
    [TestClass]
    public class StrategyTests
    {
        [Description("F2.1.2")]
        [Priority(1)]
        [TestMethod]
        public void TestTransientErrorIgnoreStrategyWithoutError()
        {
            RetryPolicy noRetryPolicy = RetryPolicy.NoRetry;
            int execCount = 0;

            try
            {
                noRetryPolicy.ExecuteAction(() =>
                {
                    execCount++;
                });
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            Assert.AreEqual<int>(1, execCount, "The action was not executed the expected amount of times");
        }

        [Description("F2.2.2")]
        [Priority(1)]
        [TestMethod]
        public void TestSqlAzureTransientErrorDetectionStrategyWithNonRetryableError()
        {
            RetryPolicy defaultPolicy = new RetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>(1, RetryStrategy.DefaultRetryInterval);
            int execCount = 0;

            try
            {
                defaultPolicy.ExecuteAction(() =>
                {
                    execCount++;
                    throw new ApplicationException("Forced Exception");
                });
            }
            catch (ApplicationException ex)
            {
                Assert.AreEqual("Forced Exception", ex.Message);
            }

            Assert.AreEqual<int>(1, execCount, "The action was not executed the expected amount of times");
        }
    }
}
