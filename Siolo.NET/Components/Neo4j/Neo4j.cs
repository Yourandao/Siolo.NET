using Neo4j.Driver;
using Siolo.NET.Components.Network;
using System.Collections.Generic;
using System.Linq;
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

		private async Task<T> ExecuteWithResult<T>(string query, string field)
		{
			var session = _driver.AsyncSession();

			try
			{
				IResultCursor cursor = await session.RunAsync(query);

				var result = await cursor.SingleAsync(record => record[field].As<T>());

				return result;
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

		public async Task<string> PushHostRaw(string ip)
		{
			bool isSubnet = NetworkUtility.IsSubnet(ip);

			var query = $@"create (a:Host {{ ip : '{ ip }', subnet : '{ isSubnet }'}})";

			if (!isSubnet)
			{
				await Execute(query);

				var subnetIp = new Regex(@"\d+\/\d{2}").Replace(ip, "0");

				await CreateRelation(new Neo4jHostObject(subnetIp, true),
											new Neo4jHostObject(ip, false));

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

		public async Task CreateRelation(Neo4jHostObject host1, Neo4jHostObject host2)
		{
			if (host1.IsSubnet || host2.IsSubnet)
			{
				var query =
					@$"match (a:Host), (b:Host) where a.ip contains '{host1.Ip}' and b.ip contains '{host2.Ip}' create (a)-[:KNOWS {{from: a.ip, to: b.ip}}]->(b), (b)-[:KNOWS{{from: b.ip, to: a.ip}}]->(a)";

				await Execute(query);
			}
		}

		public async Task<List<List<Neo4jRelation>>> FindAllPaths(string first, string second)
		{
			var query =
				$"match path = (a:Host {{ip : '{first}'}})-[:KNOWS*1..6]->(t:Host {{ip : '{second}'}}) unwind relationships(path) as rel return collect({{from : rel.from, to : rel.to }}) as PathData";

			var result = await ExecuteWithResult<List<Dictionary<string, object>>>(query, "PathData");

			var paths = new List<List<Neo4jRelation>>();
			var path = new List<Neo4jRelation>();

			bool isFirst = true;

			foreach (var way in result.Select(elem => elem.Select(o => o.Value.ToString()).ToList()))
			{
				if (way[0] == first && !isFirst)
				{
					paths.Add(path);
					path.Clear();
				}

				isFirst = false;

				path.Add(new Neo4jRelation(way[0], way[1]));
			}

			return paths;
		}

		public async Task<List<string>> GetAllHosts()
		{
			var query = "match (n:Host) return n.ip as ip";
			var result = await ExecuteWithMultipleResult<string>(query, "ip");

			return result;
		}
	}
}
