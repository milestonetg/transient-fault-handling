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

namespace MilestoneTG.TransientFaultHandling.Tests.RetryManagerScenarios.given_the_default_retry_manager
{
    using System;
    using System.Collections.Generic;
    using MilestoneTG.Common.TestSupport.ContextBase;
    using MilestoneTG.TransientFaultHandling;
    using MilestoneTG.TransientFaultHandling.Data;
    using MilestoneTG.TransientFaultHandling.Data.SqlServer;
    using MilestoneTG.TransientFaultHandling.TestSupport;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class Context : ArrangeActAssert
    {
        protected RetryStrategy defaultStrategy;
        protected RetryStrategy defaultSqlConnectionStrategy;
        protected RetryStrategy defaultSqlCommandStrategy;
        protected RetryStrategy otherStrategy;

        protected RetryManager managerWithAllDefaults;
        protected RetryManager managerWithOnlyDefault;

        protected override void Arrange()
        {
            this.defaultStrategy = new FixedIntervalRetryStrategy("default", 5, TimeSpan.FromMilliseconds(10));
            this.otherStrategy = new IncrementalRetryStrategy("other", 5, TimeSpan.FromMilliseconds(10), TimeSpan.FromMilliseconds(10));
            this.defaultSqlConnectionStrategy = new FixedIntervalRetryStrategy("defaultSqlConnection", 5, TimeSpan.FromMilliseconds(10));
            this.defaultSqlCommandStrategy = new FixedIntervalRetryStrategy("defaultSqlCommand", 5, TimeSpan.FromMilliseconds(10));

            this.managerWithAllDefaults = new RetryManager(
                new[]
                {
                    this.defaultStrategy,
                    this.defaultSqlConnectionStrategy,
                    this.defaultSqlCommandStrategy,
                    this.otherStrategy,
                },
                "default",
                new Dictionary<string, string> 
                { 
                    { "SQL", "defaultSqlCommand" },
                    { "SQLConnection", "defaultSqlConnection" },
                });

            this.managerWithOnlyDefault = new RetryManager(
              new[]
                {
                    this.defaultStrategy,
                    this.defaultSqlConnectionStrategy,
                    this.defaultSqlCommandStrategy,
                    this.otherStrategy,
                },
              "default");
        }
    }

    [TestClass]
    public class when_getting_default_retry_strategy : Context
    {
        private RetryStrategy retryStrategy;

        protected override void Act()
        {
            this.retryStrategy = this.managerWithAllDefaults.GetRetryStrategy();
        }
        
        [TestMethod]
        public void then_value_is_matching()
        {
            Assert.AreSame(defaultStrategy, retryStrategy);
        }
    }

    [TestClass]
    public class when_getting_retry_strategy_by_name : Context
    {
        private RetryStrategy retryStrategy;

        protected override void Act()
        {
            this.retryStrategy = this.managerWithAllDefaults.GetRetryStrategy("other");
        }

        [TestMethod]
        public void then_value_is_matching()
        {
            Assert.AreSame(otherStrategy, retryStrategy);
        }
    }

    [TestClass]
    public class when_getting_retry_strategy_by_invalid_name : Context
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void then_value_is_matching()
        {
            this.managerWithAllDefaults.GetRetryStrategy("invalid");
        }
    }

    [TestClass]
    public class when_getting_retry_policy_with_default_retry_strategy : Context
    {
        private RetryPolicy retryPolicy;

        protected override void Act()
        {
            this.retryPolicy = this.managerWithAllDefaults.GetRetryPolicy<AlwaysTransientErrorDetectionStrategy>();
        }

        [TestMethod]
        public void then_values_are_matching()
        {
            Assert.IsInstanceOfType(retryPolicy.ErrorDetectionStrategy, typeof (AlwaysTransientErrorDetectionStrategy));
            Assert.AreSame(defaultStrategy, this.retryPolicy.RetryStrategy);
        }
    }

    [TestClass]
    public class when_getting_retry_policy_with_retry_strategy_by_name : Context
    {
        private RetryPolicy retryPolicy;

        protected override void Act()
        {
            this.retryPolicy = this.managerWithAllDefaults.GetRetryPolicy<AlwaysTransientErrorDetectionStrategy>("other");
        }

        [TestMethod]
        public void then_values_are_matching()
        {
            Assert.IsInstanceOfType(retryPolicy.ErrorDetectionStrategy, typeof(AlwaysTransientErrorDetectionStrategy));
            Assert.AreSame(otherStrategy, this.retryPolicy.RetryStrategy);
        }
    }

    [TestClass]
    public class when_getting_retry_policy_with_unknown_retry_strategy : Context
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void then_default_strategy_is_returned()
        {
            this.managerWithAllDefaults.GetRetryPolicy<AlwaysTransientErrorDetectionStrategy>("unkown");
        }
    }
    
    [TestClass]
    public class when_getting_default_sql_connection_strategy_when_provided : Context
    {
        private RetryPolicy retryPolicy;

        protected override void Act()
        {
            this.retryPolicy = this.managerWithAllDefaults.GetDefaultSqlConnectionRetryPolicy();
        }

        [TestMethod]
        public void then_values_are_matching()
        {
            Assert.IsInstanceOfType(retryPolicy.ErrorDetectionStrategy, typeof(SqlDatabaseTransientErrorDetectionStrategy));
            Assert.AreSame(this.defaultSqlConnectionStrategy, this.retryPolicy.RetryStrategy);
        }
    }

    [TestClass]
    public class when_getting_default_sql_connection_strategy_when_not_provided : Context
    {
        private RetryPolicy retryPolicy;

        protected override void Act()
        {
            this.retryPolicy = this.managerWithOnlyDefault.GetDefaultSqlConnectionRetryPolicy();
        }

        [TestMethod]
        public void then_fallback_to_default_retry_strategy()
        {
            Assert.IsInstanceOfType(retryPolicy.ErrorDetectionStrategy, typeof(SqlDatabaseTransientErrorDetectionStrategy));
            Assert.AreSame(this.defaultStrategy, this.retryPolicy.RetryStrategy);
        }
    }

    [TestClass]
    public class when_getting_default_sql_command_strategy_when_provided : Context
    {
        private RetryPolicy retryPolicy;

        protected override void Act()
        {
            this.retryPolicy = this.managerWithAllDefaults.GetDefaultSqlCommandRetryPolicy();
        }

        [TestMethod]
        public void then_values_are_matching()
        {
            Assert.IsInstanceOfType(retryPolicy.ErrorDetectionStrategy, typeof(SqlDatabaseTransientErrorDetectionStrategy));
            Assert.AreSame(this.defaultSqlCommandStrategy, this.retryPolicy.RetryStrategy);
        }
    }

    [TestClass]
    public class when_getting_default_sql_command_strategy_when_not_provided : Context
    {
        private RetryPolicy retryPolicy;

        protected override void Act()
        {
            this.retryPolicy = this.managerWithOnlyDefault.GetDefaultSqlCommandRetryPolicy();
        }

        [TestMethod]
        public void then_fallback_to_default_retry_strategy()
        {
            Assert.IsInstanceOfType(retryPolicy.ErrorDetectionStrategy, typeof(SqlDatabaseTransientErrorDetectionStrategy));
            Assert.AreSame(this.defaultStrategy, this.retryPolicy.RetryStrategy);
        }
    }
}
