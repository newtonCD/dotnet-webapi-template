using Template.Application.Common;
using Template.Application.Common.Interfaces;

namespace Template.Application.Tests;

public class TestDatabaseConfiguration : IDbOperationConfiguration
{
    public string CommandConnectionString { get; set; }
    public string QueryConnectionString { get; set; }

    public bool UseSingleDatabaseValue { private get; set; }

    public bool UseSingleDatabase()
    {
        return UseSingleDatabaseValue;
    }

    public DbOperationConfiguration ToDatabaseConfiguration()
    {
        return new DbOperationConfiguration
        {
            CommandConnectionString = CommandConnectionString,
            QueryConnectionString = QueryConnectionString
        };
    }
}