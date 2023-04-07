namespace Application.Common.Interfaces;

public interface IDbOperationConfiguration
{
    string CommandConnectionString { get; set; }
    string QueryConnectionString { get; set; }

    bool UseSingleDatabase();
}