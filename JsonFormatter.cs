namespace Marketplace_Web
{
	public class JsonFormatter
	{
		public static string JSONFormatting(string jsonResponse)
		{
			jsonResponse = jsonResponse.Replace("\\", "");
			jsonResponse = jsonResponse.Substring(1, jsonResponse.Length - 2);
			return jsonResponse;
		}
	}
}
