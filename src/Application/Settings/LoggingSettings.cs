using System.Diagnostics.CodeAnalysis;

namespace Application.Settings;

[ExcludeFromCodeCoverage]
public class LoggingSettings
{
    public bool LogRequestEnabled { get; set; }
    public bool LogResponseEnabled { get; set; }
}