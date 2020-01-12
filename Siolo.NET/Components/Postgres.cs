#nullable enable
using Npgsql;
using Siolo.NET.Components.Network;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Siolo.NET.Components
{
	public class Postgres : IDisposable
	{
		private volatile NpgsqlConnection _connection;

		private const string UUID_NIL = "00000000-0000-0000-0000000000";

		public Postgres(string host, string port, string login, string password, string db)
		{
			_connection = new NpgsqlConnection($"User ID={login};Password={password};Server={host};Port={port};Database={db};Integrated Security=true;Pooling=true;");
			_connection.Open();
		}

		public async Task<Dictionary<string, List<string>>> GetEntities()
		{
			var temp = new Dictionary<string, List<string>>();

			await using var command = new NpgsqlCommand("select * from get_hosts_info()", _connection);
			await using var reader = await command.ExecuteReaderAsync();

			while (await reader.ReadAsync())
			{
				string? ip = reader[1].ToString();
				string? wildcard = reader[2].ToString();
				int mask = (int)reader[0];

				var host = $"{ip}/{mask}";

				if (string.IsNullOrEmpty(wildcard))
					continue;

				if (!temp.ContainsKey(host))
				{
					temp.Add(host, new List<string>() { wildcard });
				}
				else
				{
					temp[host].Add(wildcard);
				}
			}

			await reader.CloseAsync();

			return temp;
		}

		public async Task<bool> InsertNewOne(string host)
		{
			(bool isSubnet, int mask, string ip) = NetworkUtility.Deconstruct(host);

			await using (var command = new NpgsqlCommand("select * from register(@mask, '@ip', @subnet)", _connection))
			{
				command.Parameters.AddWithValue("mask", mask);
				command.Parameters.AddWithValue("ip", ip);
				command.Parameters.AddWithValue("subnet", isSubnet);

				return (await command.ExecuteScalarAsync()).ToString() != UUID_NIL;
			}
		}

		public async Task<bool> AttachPolicy(string host, string wildcard)
		{
			(_, int mask, string ip) = NetworkUtility.Deconstruct(host);

			await using (var command = new NpgsqlCommand("select * from register_rule_on_host(@mask, '@ip', '@wildcard')",
												   _connection))
			{
				command.Parameters.AddWithValue("mask", mask);
				command.Parameters.AddWithValue("ip", ip);
				command.Parameters.AddWithValue("wildcard", wildcard);

				return bool.Parse((await command.ExecuteScalarAsync())?.ToString()?.ToLower() 
									?? throw new InvalidOperationException());
			}
		}

		public async Task<bool> RegisterPolicy(string info, string wildcard)
		{
			await using (var command =
				new NpgsqlCommand($"select * from register_policy('@info', '@wildcard')", _connection))
			{
				command.Parameters.AddWithValue("info", info);
				command.Parameters.AddWithValue("wildcard", wildcard);

				return (await command.ExecuteScalarAsync()).ToString() != UUID_NIL;
			}

		}

		public void Dispose()
		{
			_connection.Close();
		}
	}
}