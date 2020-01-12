using System;

using Neo4j.Driver;
using Siolo.NET.Components.Network;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Siolo.NET.Components.Neo4j
{
	[Obsolete("Obsolete. Use Siolo.NET.Components.Neo4JExperimental instead.")]
	public class Neo4J : IDisposable
	{
		private readonly IDriver _driver;

		public Neo4J(string host, string port, string login, string password) => 
			_driver = GraphDatabase.Driver($"bolt://{host}:{port}", AuthTokens.Basic(login, password));

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

		private async Task<List<List<NodeEntity>>> ExecuteWithResult(string query, string field)
		{
			var session = _driver.AsyncSession();

			try
			{
				IResultCursor cursor = await session.RunAsync(query);

				var results = new List<List<NodeEntity>>();

				while (await cursor.FetchAsync())
				{
					var node = JsonConvert.SerializeObject(cursor.Current[field].As<List<INode>>().Select(o => o.Properties));
					results.Add(JsonConvert.DeserializeObject<List<NodeEntity>>(node));
				}

				return results;
			}
			finally
			{
				await session.CloseAsync();
			}
		}

		private async Task<List<T>> ExecuteWithMultipleResult<T>(string query, string field)
		{
			var session = _driver.AsyncSession();

			try
			{
				IResultCursor cursor = await session.RunAsync(query);

				var result = await cursor.ToListAsync(record => record[field].As<T>());

				return result;
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

		public async Task DeleteHost(NodeEntity host)
		{
			var query = $@"match (a:Host {{ip : '{ host.ip }'}}) detach delete a";

			await Execute(query);
		}

		public async Task PushHost(NodeEntity host)
		{
			var query = $@"create (a:Host {{ ip : '{ host.ip }', subnet : '{ host.IsSubnet() }'}})";

			await Execute(query);
		}

		public async Task<string> PushHostRaw(string ip)
		{
			bool isSubnet = NetworkUtility.IsSubnet(ip);

			var query = $@"create (a:Host {{ ip : '{ ip }', subnet : '{ isSubnet }'}})";

			if (!isSubnet)
			{
				Execute(query).Wait();

				var subnetIp = new Regex(@"\d+\/\d{2}").Replace(ip, "0");

				await CreateRelation(new NodeEntity(subnetIp, true), 
											new NodeEntity(ip, false));

				return subnetIp;
			}
			else
			{
				await Execute(query);

				return default;
			}
		}

		/*
		 * MATCH path=(a:Host{ip:'192.168.6.2/32'})-[:KNOWS*1..6]->(t:Host{ip:'192.168.5.3/32'}) 
		   unwind relationships(path) as rel
		   return collect({from: rel.from, to: rel.to})

		   match (a:Host{ip:'192.168.6.2/32'})-[r:KNOWS*5..6]->(t:Host{ip:'192.168.5.3/32'})
		   unwind r as rel
		   return collect(distinct r)
		 */

		public async Task CreateRelation(NodeEntity host1, NodeEntity host2)
		{
			if (host1.IsSubnet() || host2.IsSubnet())
			{
				var query =
					@$"match (a:Host), (b:Host) where a.ip contains '{host1.ip}' and b.ip contains '{host2.ip}' create (a)-[:KNOWS {{from: a.ip, to: b.ip}}]->(b), (b)-[:KNOWS{{from: b.ip, to: a.ip}}]->(a)";

				await Execute(query);
			}
		}

		public async Task<List<List<NodeEntity>>> FindAllPaths(string first, string second)
		{
			var query =
				$"match r = (a:Host {{ip : '{first}'}})-[:KNOWS*1..6]->(t:Host {{ip : '{second}'}}) WHERE ALL(n in nodes(r) WHERE size([m in nodes(r) WHERE m=n]) = 1) return nodes(r) as r";

			var result = await ExecuteWithResult(query, "r");

			return result;
		}

		public async Task<List<string>> GetAllHosts()
		{
			var query = "match (n:Host) where n.subnet = 'False' return n.ip as ip";
			var result = await ExecuteWithMultipleResult<string>(query, "ip");

			return result;
		}

		public void Dispose()
		{
			_driver.CloseAsync().GetAwaiter().GetResult();
		}
	}
}