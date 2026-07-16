using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.Text.Json.Serialization;

namespace InvestYes.Infrastructure.ExternalApis.Brapi.Models;

using System.Text.Json.Serialization;

public sealed class BrapiFiiIndicatorsResponse
{
    [JsonPropertyName("fiis")]
    public List<BrapiFiiIndicator> Fiis { get; set; } = [];

    [JsonPropertyName("requestedAt")]
    public DateTime RequestedAt { get; set; }

    [JsonPropertyName("took")]
    public int Took { get; set; }
}

