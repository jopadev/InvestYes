using HtmlAgilityPack;
using InvestYes.Domain.ValueObjects;
using System.Globalization;
using System.Text.RegularExpressions;

namespace InvestYes.Infrastructure.ExternalApis.FundsExplorer;

public static class FundsExplorerParser
{
    public static MarketAsset Parse(string html)
    {
        var oHtmlDocument = new HtmlDocument();

        oHtmlDocument.LoadHtml(html);

        var oFieldsDictionary = ReadIndicators(oHtmlDocument);

        var oMarketData = new MarketAsset
        {
            Symbol = oFieldsDictionary.GetString("Ticker"),
            Name = oFieldsDictionary.GetString("Nome"),
            Price = oFieldsDictionary.GetDecimal("Cotação"),
            PriceToNav = oFieldsDictionary.GetDecimal("P/VP"),
            BookValuePerShare = oFieldsDictionary.GetDecimal("VP/Cota"),
            DividendYield12M = oFieldsDictionary.GetDecimal("Div. Yield"),
            DividendYield = oFieldsDictionary.GetDecimal("Div. Yield") / 12,
            FfoYield = oFieldsDictionary.GetDecimal("FFO Yield"),
            Volume = oFieldsDictionary.GetDecimal("Vol $ méd (2m)") / 2
        };

        var sVolume = oFieldsDictionary.GetString("Liquidez Média Diária").Trim().ToUpper();

        if (!string.IsNullOrEmpty(sVolume))
        {
            char lastChar = sVolume[^1];
            bool hasSuffix = "KMB".Contains(lastChar);
            string numberPart = hasSuffix ? sVolume[..^1] : sVolume;

            if (decimal.TryParse(numberPart, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal value))
            {
                oMarketData.Volume = lastChar switch
                {
                    'K' => value * 1_000,
                    'M' => value * 1_000_000,
                    'B' => value * 1_000_000_000,
                    _ => value
                };
            }
        }

        oMarketData.DividendYield12M = oFieldsDictionary.GetDecimal("Dividend Yield");
        oMarketData.DividendYield = oFieldsDictionary.GetDecimal("Dividend Yield") / 12;

        return oMarketData;
    }


    public static Dictionary<string, string> ReadIndicators(HtmlDocument document)
    {
        var oIndicadoresDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        var oInformacoesHeaderDictionary = new Dictionary<string, string>{
        { "headerTicker__content__title","Ticker" },
        {"headerTicker__content__name","Nome"},
        {"headerTicker__content__cnpj","CNPJ"},
        };

        var sPattern = @">([^<]+)<";

        MatchCollection oMatchCollection;

        foreach (var item in oInformacoesHeaderDictionary)
        {
            try
            {
                var oHtmlNode = document.DocumentNode.SelectNodes($"//*[contains(@class,'{item.Key}')]");

                if (oHtmlNode is not null)
                {
                    var sConteudo = oHtmlNode.FirstOrDefault().InnerHtml;

                    oMatchCollection = Regex.Matches(sConteudo, sPattern);

                    if (!oMatchCollection.Any())
                        oIndicadoresDictionary.Add(item.Value, sConteudo);
                    else
                    {
                        var sLinha = oMatchCollection[0].Groups[1].Value.Trim().ToSanitize();

                        oIndicadoresDictionary.Add(item.Value, sLinha);
                    }
                }
            }
            catch (Exception)
            {
                continue;
                throw;
            }
        }

        var oHtmlNodeColletion = document.DocumentNode.SelectNodes("//div[contains(@class,'headerTicker__content__price')]");

        var sPreco = oHtmlNodeColletion.FirstOrDefault().InnerHtml;

        oMatchCollection = Regex.Matches(sPreco, sPattern);

        if (oMatchCollection.Count >= 2)
        {
            var sLinha1 = oMatchCollection[0].Groups[1].Value.Trim().ToSanitize();

            oIndicadoresDictionary.Add("Cotação", sLinha1);

            var sLinha2 = oMatchCollection[1].Groups[1].Value.Trim().ToSanitize();

            oIndicadoresDictionary.Add("Variação", sLinha2);
        }

        var oHtmlNodeColletion2 = document.DocumentNode.SelectNodes("//div[contains(@class,'indicators__box')]");

        if (oHtmlNodeColletion2 == null)
            return oIndicadoresDictionary;

        foreach (var node in oHtmlNodeColletion2)
        {
            var oHtmlNodeColletion3 = node.SelectNodes("./p");

            if (oHtmlNodeColletion3 == null || oHtmlNodeColletion3.Count < 2)
                continue;

            var sKey = HtmlEntity.DeEntitize(oHtmlNodeColletion3[0].InnerText).Trim();

            var sValue = HtmlEntity.DeEntitize(oHtmlNodeColletion3[1].InnerText).ToSanitize();
            
            oIndicadoresDictionary[sKey] = sValue;
        }

        return oIndicadoresDictionary;
    }

    public static string ToSanitize(this string value)
    {
       value = value.Replace("R$", "")
                .Replace("%", "")
                .Replace("\r", "")
                .Replace("\n", "")
                .Trim();

        return value;
    }
}