using System.Diagnostics.CodeAnalysis;

namespace Application.Settings;

[ExcludeFromCodeCoverage]
public class CacheSettings
{
    public string Type { get; set; }
    public int ExpirationInMinutes { get; set; }
    public string RedisConnectionString { get; set; }
}
