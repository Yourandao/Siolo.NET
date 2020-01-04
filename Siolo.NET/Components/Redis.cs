using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Siolo.NET.Components
{
	public class Redis
	{
		private readonly ConnectionMultiplexer _connection;

		private readonly IDatabase _db;
		private readonly IServer _server;

		public Redis(string host, string port)
		{
			_connection = ConnectionMultiplexer.Connect($"{host}:{port}");

			_db = _connection.GetDatabase();
			_server = _connection.GetServer($"{host}:{port}");
		}

		public async Task Flush()
		{
			await _db.ExecuteAsync("flushall");
		}

		public async Task<bool> PushNew(string host, string value)
		{
			return await _db.SetAddAsync(host, value);
		}

		public async Task<bool> Exists(string hostValue)
		{
			return await _db.KeyExistsAsync(hostValue);
		}

		public async Task PushHostData(Dictionary<string, List<string>> data)
		{
			foreach (var (host, value) in data)
			{
				foreach (var wildcart in value)
				{
					await this.PushNew(host, wildcart);
				}
			}
		}

		public async IAsyncEnumerable<string> GetHostWildcarts(string host)
		{
			string key = _server.Keys(pattern: $"{host}*").First();

			foreach (var value in await _db.SetMembersAsync(key))
			{
				if (value != default)
				{
					yield return value;
				}
			}
		}

		public async Task Close()
		{
			await _connection.CloseAsync();
		}
	}
}
