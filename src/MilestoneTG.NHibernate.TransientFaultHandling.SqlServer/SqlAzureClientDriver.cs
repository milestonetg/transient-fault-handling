using MilestoneTG.TransientFaultHandling.Data.SqlServer;
namespace MilestoneTG.NHibernate.TransientFaultHandling.SqlServer
{
    /// <summary>
    /// NHibernate client driver for SQL Azure that includes the Enterprise Library transient fault-handling.
    /// Note: It doesn't handle timeout errors, which can sometimes be transient. If you have timeout errors
    /// that aren't caused by unoptimized queries then use 
    /// </summary>
    public class SqlAzureClientDriver : DefaultReliableSql2008ClientDriver<SqlDatabaseTransientErrorDetectionStrategy> { }

}
