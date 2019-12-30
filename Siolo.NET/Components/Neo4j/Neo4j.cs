using Neo4j.Driver;
using Siolo.NET.Components.Network;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Siolo.NET.Components.Neo4j
{
	public class Neo4J
	{
		private readonly IDriver _driver;

		public Neo4J(string host, string port, string login, string password)
		{
			_driver = GraphDatabase.Driver($"bolt://{host}:{port}", AuthTokens.Basic(login, password));
		}

		private async Task Execute(string query)
		{
			var session = _driver.AsyncSession();

			try
			{
				IResultCursor cursor = await session.RunAsync(query);
				await cursor.ConsumeAsync();
			}
			finally
			{
				await session.CloseAsync();
			}
		}

		private async Task<List<IRelationship>> ExecuteWithResult<T>(string query, string field = "name")
		{
			var session = _driver.AsyncSession();

			try
			{
				IResultCursor cursor = await session.RunAsync(query);

				return new List<IRelationship>();
			}
			finally
			{
				await session.CloseAsync();
			}
		}

		public async Task DeleteHost(string host)
		{
			var query = $@"match (a:Host {{ip : '{host}'}}) detach delete a";

			await Execute(query);
		}

		public async Task DeleteHost(Neo4jHostObject host)
		{
			var query = $@"match (a:Host {{ip : '{ host.Ip }'}}) detach delete a";

			await Execute(query);
		}

		public async Task PushHost(Neo4jHostObject host)
		{
			var query = $@"create (a:Host {{ ip : '{ host.Ip }', subnet : '{ host.IsSubnet }'}})";

			await Execute(query);
		}

		public async Task PushHostRaw(string ip)
		{
			bool isSubnet = NetworkUtility.IsSubnet(ip);

			var query = $@"create (a:Host {{ ip : '{ ip }', subnet : '{ isSubnet }'}})";

			if (!isSubnet)
			{
				await Execute(query);

				var subnetIp = new Regex(@"\d+\/\d{2}").Replace(ip, "0");

				await CreateRelation(new Neo4jHostObject(subnetIp, true),
											new Neo4jHostObject(ip, false));
			}
			else
			{
				await Execute(query);
			}
		}

		/*
		 * MATCH (a:Host{ip:'192.168.16.3/24'})-[r*5..6]->(t:Host{ip:'192.168.2.4/24'}) 
		   return r
		 */

		public async Task CreateRelation(Neo4jHostObject host1, Neo4jHostObject host2)
		{
			if (host1.IsSubnet || host2.IsSubnet)
			{
				var query =
					@$"match (a:Host), (b:Host) where a.ip contains '{host1.Ip}' and b.ip contains '{host2.Ip}' create (a)-[:KNOWS {{from: a.ip, to: b.ip}}]->(b), (b)-[:KNOWS{{from: b.ip, to: a.ip}}]->(a)";

				await Execute(query);
			}
		}

		public async Task<List<IRelationship>> FindAllPaths(string first, string second)
		{
			var query = $"MATCH (a:Host{{ip: '{first}'}})-[r*5..6]->(t:Host{{ip : '{second}'}}) return r";
			var result = await ExecuteWithResult<IRelationship>(query, "r");

			return result;
		}

		//public async Task<List<string>> GetAllHosts()
		//{
		//	var query = "match (n:Host) return n";

		//	return await ExecuteWithResult<string>(query);
		//}
	}
}
