using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Siolo.NET.Components.Network
{
	public static class NetworkUtility
	{
		public static string GetIp(string host)
		{
			var ipRegex = new Regex(@"(.*)\/\d+");

			return ipRegex.Match(host).Groups[1].Value;
		}

		public static bool IsSubnet(string host)
		{
			var subnetRegex = new Regex(@"\.(\d)\/\d+");

			return subnetRegex.Match(host).Groups[1].Value.ToLower() == "0";
		}

		public static int GetMask(string host)
		{
			var maskRegex = new Regex(@".*\/(\d+)");

			return int.Parse(maskRegex.Match(host).Groups[1].Value);
		}

		public static Response SetStatus(this Response response, bool status, string message)
		{
			response.Message = message;
			response.Status = status;

			return response;
		}

		public static async Task<bool> IsRestricted(IAsyncEnumerable<string> extensions, string fullClass)
		{
			return await GetRestrictingPolicy(extensions, fullClass) != "";
		}

		public static async Task<string> GetRestrictingPolicy(IAsyncEnumerable<string> extensions, string fullClass)
		{
			var fullClassParts = fullClass.Split(':');

			await foreach (var extension in extensions)
			{
				var extensionParts = extension.Split(':');

				if ((extensionParts[0] == fullClassParts[0] || extensionParts[0] == "*") &&
					(extensionParts[1] == fullClassParts[1] || extensionParts[1] == "*"))
				{
					return extension;
				}
			}

			return "";
		}
	}
}
