using freecurrencyapi.Helpers;
namespace WebApplication1.Models
{
	public class FreeCurrencyApi
	{
		private string ApiKey { get; }
		public FreeCurrencyApi(string apiKey)
		{
			ApiKey = apiKey;
		}
		public string Status()
		{
			return RequestHelper.Status(ApiKey);
		}
		public string Currencies(string currencies = "")
		{
			return RequestHelper.Currencies(ApiKey, currencies);
		}
		public string Latest(string baseCurrency = "USD", string currencies = "")
		{
			return RequestHelper.Latest(ApiKey, baseCurrency, currencies);
		}
		public string Historical(string data, string baseCurrency = "USD", string currencies = "")
		{
			return RequestHelper.Historical(ApiKey, data, baseCurrency, currencies);
		}
	}
}

