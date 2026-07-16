using System.Diagnostics;

namespace InvestYes.Infrastructure.Observability;

public static class Telemetry
{
    public const string SourceName = "InvestYes";

    public static readonly ActivitySource ActivitySource =
        new(SourceName);
}