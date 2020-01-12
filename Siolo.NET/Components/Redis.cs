using System;

using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siolo.NET.Components
{
	public class Redis : IDisposable
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

		public async Task<bool> PushNew(string host, string value) => 
			await _db.SetAddAsync(host, value);

		public async Task<bool> Exists(string hostValue)
		{
			return await _db.KeyExistsAsync(hostValue);
		}

		public async Task PushHostData(Dictionary<string, List<string>> data)
		{
			foreach (var (host, value) in data)
			{
				foreach (var wildcard in value)
				{
					await this.PushNew(host, wildcard);
				}
			}
		}

		public async IAsyncEnumerable<string> GetHostWildcards(string host)
		{
			var results = _server.Keys(pattern: $"{host}*").ToList();

			if (!results.Any())
			{
				yield break;
			}

			string key = results.First();

			foreach (var value in await _db.SetMembersAsync(key))
			{
				if (value != default)
				{
					yield return value;
				}
			}
		}

		public void Dispose()
		{
			_connection.Close();
		}
	}
}
