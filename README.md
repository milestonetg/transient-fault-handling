# MilestoneTG Transient Fault Handling Libraries

[![Build Status](https://milestonetg.visualstudio.com/Milestone/_apis/build/status/transient-fault-handling?branchName=master)](https://milestonetg.visualstudio.com/Milestone/_build/latest?definitionId=38&branchName=master)
![Nuget](https://img.shields.io/nuget/v/MilestoneTG.TransientFaultHandling)

Much of the work in this project is derived from Microsoft's Enterrpise Library Transient Fault Handling Block and
Microsoft's RestClientRuntime.

## Usage

### Database Operations

You can execute a command using the `ReliableSqlConnection` class...

``` cs
using(var cn = new ReliableSqlConnection(connectionString))
{
	using(var cmd = new SqlCommand(sql))
	{
		cn.Open();
		using(var rdr = cn.ExecuteCommand<IDataReader>(cmd))
		{
			...
		}
	}
}
```

...or you can use the `OpenWithRetry()` and `ExecuteXxxxWithRetry()` extension methods, and pass a specific `RetryPolicy`...

``` cs
using(var cn = new SqlConnection(connectionString))
{
	using(var cmd = new SqlCommand(sql, cn))
	{
		cn.OpenWithRetry(connectionRetryPolicy);

		using(var rdr = cmd.ExecuteReaderWithRetry(commandRetrypolicy)}
		{
			...
		}
	}
}
```

Using the extension methods allows you to taylor retry policies specific to a particular command.

### Http Operations

``` cs
var retryHandler = new RetryDelegatingHandler { InnerHandler = new HttpClientHandler() };

var httpClient = new HttpClient(retryHandler);
```

## Idempotency

Be careful when using retries on operations that change state--ie. database INSERT, UPDATE, DELETE and Http POST, PUT, DELETE.
Ensure that the operation is idempotent--the operation will yield the same result regardless of how many times it is
performed. Severed connections, espectially in HTTP, can result in the request still completing. Most databases, will
rollback a transaction automatically if a connection is broken prior to commit.

## Entity Framework

EF6 and EF Core, provide transient retry out of the box with the SQL Server, Pomelo.EntityFrameworkCore.MySql,
Npgsql.EntityFrameworkCore.PostgreSQL, and DevArt providers. MilestoneTG.TransientFaultHandling.Data.* is not needed in
these scenarios.

## HttpClient Packages

### MilestoneTG.TransientFaultHandling.Http

Contains `RetryDelegatingHandler` and supporting constructs that adds transient retry capability to the HttpClient
pipeline.

### MilestoneTG.TransientFaultHandling.Http.Extensions

HttpClientBuilder extensions to support HttpClientFactory.

## Database Packages

### Microsoft SQL Server

#### MilestoneTG.TransientFaultHandling.Data.SqlServer

Supports transient fault handling for Microsoft SQL Server, including cloud provided solutions such as Azure 
SQL Database, AWS RDS for SQL Server.

#### MilestoneTG.NHibernate.TransientFaultHandling.SqlServer

NHiberate provider for Microsoft SQL Server that leverages reliable connections and commands.

### MySQL (as of v2.2)

#### MilestoneTG.TransientFaultHandling.Data.MySql

Supports transient fault handling for most implementations of MySQL including MySQL, MariaDB, Percona MySQL,
Azure Database for MySQL, AWS Aurora MySQL (including Aurora Serverless) and, AWS RDS for MySQL.

### PostGreSQL (as of v2.2)

#### MilestoneTG.TransientFaultHandling.Data.Postgres

Supports transient fault handling for most implementations of PostreSQL including PostgreSQL, EnterpriseDB, Percona
PostgreSQL, Azure Database for PostgreSQL, AWS Aurora PostgreSQL, AWS RDS for PostgreSQL, and AWS Red Shift.

### Oracle (as of v3.0)

#### MilestoneTG.TransientFaultHandling.Data.Oracle

Supports transient fault handling for most deployments of Oracle including on premises, Azure, AWS RDS, and Oracle Cloud.
