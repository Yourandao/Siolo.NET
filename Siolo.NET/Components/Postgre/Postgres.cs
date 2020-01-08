using System;

using Npgsql;
using System.Collections.Generic;
using System.Threading.Tasks;
using Siolo.NET.Components.Network;

namespace Siolo.NET.Components.Postgre
{
	public class Postgres : IDisposable
	{
		private volatile NpgsqlConnection _connection;

		private const string UuidNil = "00000000-0000-0000-0000000000";

		public Postgres(string host, string port)
		{
			_connection = new NpgsqlConnection($"User ID=postgres;Password=qwerty;Server={host};Port={port};Database=siolo;Integrated Security=true;Pooling=true;");
			_connection.Open();
		}

		public async Task<Dictionary<string, List<string>>> GetEntities()
		{
			var temp = new Dictionary<string, List<string>>();

			await using var command = new NpgsqlCommand("select * from get_hosts_info()", _connection);
			var reader = await command.ExecuteReaderAsync();

			while (await reader.ReadAsync())
			{
				var ip = reader[1].ToString();
				var wildcart = reader[2].ToString();
				var mask = (int) reader[0];

				var host = $"{ip}/{mask}";

				if (wildcart != "")
				{
					if (!temp.ContainsKey(host))
					{
						temp.Add(host, new List<string>() { wildcart });
					}
					else
					{
						temp[host].Add(wildcart);
					}
				}
			}

			await reader.CloseAsync();

			return temp;
		}

		public async Task<bool> InsertNewOne(string host)
		{
			bool isSubnet = NetworkUtility.IsSubnet(host);
			int mask = NetworkUtility.GetMask(host);
			string ip = NetworkUtility.GetIp(host);

			var command = new NpgsqlCommand($"select * from register({mask}, '{ip}', {isSubnet})", _connection);

			var reader = await command.ExecuteReaderAsync();
			reader.Read();

			var result = reader[0];

			await reader.CloseAsync();

			return result.ToString() != UuidNil;
		}

		public async Task<bool> AttachPolicy(string host, string wildcart)
		{
			int mask = NetworkUtility.GetMask(host);
			string ip = NetworkUtility.GetIp(host);

			var command = new NpgsqlCommand($"select * from register_rule_on_host({mask}, '{ip}', '{wildcart}')",
				_connection);

			var reader = await command.ExecuteReaderAsync();
			reader.Read();

			var result = reader[0];

			await reader.CloseAsync();

			return bool.Parse(result.ToString().ToLower());
		}

		public async Task<bool> RegisterPolicy(string info, string wildcart)
		{
			await using var command = new NpgsqlCommand($"select * from register_policy('{info}', '{wildcart}')", _connection);
			
			var reader = await command.ExecuteReaderAsync();
			reader.Read();

			var result = reader[0];

			await reader.CloseAsync();

			return result.ToString() == UuidNil;
		}

		public void Dispose()
		{
			_connection.Close();
		}
	}
}