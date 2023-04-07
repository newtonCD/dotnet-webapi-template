using System.Diagnostics.CodeAnalysis;

namespace Application.Settings;

[ExcludeFromCodeCoverage]
public class PollyPoliciesSettings
{
    public int RetryCount { get; set; }
    public double RetryBase { get; set; }
}
