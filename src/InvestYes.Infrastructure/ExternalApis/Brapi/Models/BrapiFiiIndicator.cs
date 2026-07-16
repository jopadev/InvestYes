using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace InvestYes.Infrastructure.ExternalApis.Brapi.Models;

public sealed class BrapiFiiIndicator
{
    [JsonPropertyName("symbol")]
    public string Symbol { get; set; } = "";

    [JsonPropertyName("asOfDate")]
    [JsonConverter(typeof(BrapiDateTimeOffsetConverter))]
    public DateTimeOffset AsOfDate { get; set; }

    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    [JsonPropertyName("navPerShare")]
    public decimal NavPerShare { get; set; }

    [JsonPropertyName("priceToNav")]
    public decimal PriceToNav { get; set; }

    [JsonPropertyName("dividendYield12m")]
    public decimal DividendYield12m { get; set; }

    [JsonPropertyName("dividendYield1m")]
    public decimal DividendYield1m { get; set; }

    [JsonPropertyName("monthlyReturn")]
    public decimal MonthlyReturn { get; set; }

    [JsonPropertyName("totalInvestors")]
    public long TotalInvestors { get; set; }

    [JsonPropertyName("sharesOutstanding")]
    public long SharesOutstanding { get; set; }

    [JsonPropertyName("equity")]
    public decimal Equity { get; set; }

    [JsonPropertyName("totalAssets")]
    public decimal TotalAssets { get; set; }

    [JsonPropertyName("segmentType")]
    public string SegmentType { get; set; } = "";

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("cnpj")]
    public string Cnpj { get; set; } = "";

    [JsonPropertyName("mandate")]
    public string? Mandate { get; set; }

    [JsonPropertyName("segmentoAtuacao")]
    public string SegmentoAtuacao { get; set; } = "";

    [JsonPropertyName("tipoGestao")]
    public string TipoGestao { get; set; } = "";

    [JsonPropertyName("administratorName")]
    public string AdministratorName { get; set; } = "";

    [JsonPropertyName("administratorCnpj")]
    public string AdministratorCnpj { get; set; } = "";

    [JsonPropertyName("administratorAddress")]
    public string AdministratorAddress { get; set; } = "";

    [JsonPropertyName("administratorAddressNumber")]
    public string AdministratorAddressNumber { get; set; } = "";

    [JsonPropertyName("administratorAddressComplement")]
    public string? AdministratorAddressComplement { get; set; }

    [JsonPropertyName("administratorDistrict")]
    public string AdministratorDistrict { get; set; } = "";

    [JsonPropertyName("administratorCity")]
    public string AdministratorCity { get; set; } = "";

    [JsonPropertyName("administratorState")]
    public string AdministratorState { get; set; } = "";

    [JsonPropertyName("administratorZipCode")]
    public string AdministratorZipCode { get; set; } = "";

    [JsonPropertyName("administratorPhone1")]
    public string? AdministratorPhone1 { get; set; }

    [JsonPropertyName("administratorPhone2")]
    public string? AdministratorPhone2 { get; set; }

    [JsonPropertyName("administratorPhone3")]
    public string? AdministratorPhone3 { get; set; }

    [JsonPropertyName("administratorWebsite")]
    public string? AdministratorWebsite { get; set; }

    [JsonPropertyName("administratorEmail")]
    public string? AdministratorEmail { get; set; }
}

public sealed class BrapiDateTimeOffsetConverter : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        var value = reader.GetString();

        if (string.IsNullOrWhiteSpace(value))
            return default;

        value = value.Replace(" ", "T");

        if (value.EndsWith("+00"))
            value += ":00";

        return DateTimeOffset.Parse(
            value,
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal);
    }

    public override void Write(
        Utf8JsonWriter writer,
        DateTimeOffset value,
        JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("O"));
    }
}