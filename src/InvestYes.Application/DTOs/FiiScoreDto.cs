public sealed class FiiScoreDto
{
    public string Symbol { get; init; } = "";

    public decimal Score { get; init; }

    public string Classification { get; init; } = "";

    public List<string> Reasons { get; init; } = [];
}