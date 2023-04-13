using System.Diagnostics.CodeAnalysis;

namespace Template.Application.Settings;

[ExcludeFromCodeCoverage]
public class PollyPoliciesSettings
{
    public int RetryCount { get; set; }
    public double RetryBase { get; set; }
}
