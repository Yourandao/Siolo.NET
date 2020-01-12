using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Siolo.NET.Components.Network
{
	public static class NetworkUtility
	{
		public static string GetIp(string host) => host.Split('/')[1];
		

		private static string GetSubnetIp(string host)
		{
			const int bitsCount = 8;

			var hostParts = host.Split('/');
			var ipParts = hostParts[0].Split('.');

			var maskParts = new List<string>();

			int mask = int.Parse(hostParts[1]);
			for (int i = 0; i < 4; i++)
			{
				int unitsCount = mask >= bitsCount ? bitsCount : mask % bitsCount;
				int zerosCount = bitsCount - mask > 0 ? bitsCount - mask : 0;

				maskParts.Add(Convert.ToInt32(new string('1', unitsCount) + new string('0', zerosCount), 2).ToString());
				mask -= bitsCount;
			}

			var subnet = new List<string>();
			for (int i = 0; i < 4; i++)
			{
				subnet.Add((int.Parse(ipParts[i]) & int.Parse(maskParts[i])).ToString());
			}

			return string.Join('.', subnet);
		}

		public static bool IsSubnet(string host)
		{
			string ip = host.Split('/')[0];

			return ip == GetSubnetIp(host);
		}

		public static int GetMask(string host) => int.Parse(host.Split('/')[1]);

		public static Response SetStatus(this Response response, bool status, string message)
		{
			response.Message = message;
			response.Status = status;

			return response;
		}

		public static (bool, int, string) Deconstruct(string host) => (IsSubnet(host), GetMask(host), GetIp(host));

		public static async Task<bool> IsRestricted(IAsyncEnumerable<string> extensions, string fullClass)
					=> await GetRestrictingPolicy(extensions, fullClass) != "";

		public static async Task<string> GetRestrictingPolicy(IAsyncEnumerable<string> extensions, string fullClass)
		{
			var fullClassParts = fullClass.Split(':');

			await foreach (string extension in extensions)
			{
				var extensionParts = extension.Split(':');

				if ((extensionParts[0] == fullClassParts[0] || extensionParts[0] == "*") &&
					(extensionParts[1] == fullClassParts[1] || extensionParts[1] == "*"))
				{
					return extension;
				}
			}

			return string.Empty;
		}
	}
}
