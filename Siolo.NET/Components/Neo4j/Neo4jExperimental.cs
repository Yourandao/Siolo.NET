using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Neo4jClient;

using Siolo.NET.Components.Network;

namespace Siolo.NET.Components.Neo4j
{
	public class Neo4JExperimental : IDisposable
	{
		private readonly GraphClient _client;

		public Neo4JExperimental(string host, string port, string login, string password)
		{
			_client = new GraphClient(new Uri($"http://{host}:{port}/db/data"), login, password);
			_client.ConnectAsync().GetAwaiter().GetResult();
		}

		public async Task DeleteHost(string host)
		{
			await _client.Cypher
			             .Match($"(a:Host {{ ip : '{host}'}})")
			             .DetachDelete("a").ExecuteWithoutResultsAsync();
		}

		public async Task<string> PushHostRaw(string ip)
		{
			bool isSubnet = NetworkUtility.IsSubnet(ip);

			var query = _client.Cypher
			             .Create($"(a:Host {{ ip : '{ ip }', subnet: '{ isSubnet }'}})");

			if (!isSubnet)
			{
				await query.ExecuteWithoutResultsAsync();

				var subnetIp = new Regex(@"\d+\/\d{2}").Replace(ip, "0");

				await CreateRelation(new NodeEntity(subnetIp, true),
											new NodeEntity(ip, false));

				return subnetIp;
			}
			else
			{
				await query.ExecuteWithoutResultsAsync();

				return default;
			}
		}

		public async Task CreateRelation(NodeEntity host1, NodeEntity host2)
		{
			if (host1.IsSubnet() ||  host2.IsSubnet())
			{
				await _client.Cypher.Match("(a:Host)").Match("b:Host").Where($"a.ip contains '{host1.ip}'")
				             .AndWhere($"b.ip contains '{host2.ip}'")
				             .Create("(a)-[:KNOWS {from: a.ip, to: b.ip}]->(b)")
				             .Create("(b)-[:KNOWS{from: b.ip, to: a.ip}]->(a)").ExecuteWithoutResultsAsync();
			}
		}

		public async Task<List<List<NodeEntity>>> FindAllPaths(string first, string second)
		{
			var results = await _client
			              .Cypher.Match($"r = (a:Host {{ip: '{first}'}})-[:KNOWS*1..6]->(b:Host {{ip: '{second}'}})")
			              .Where("all(n in nodes(r) where size([m in nodes(r) where m = n]) = 1)")
			              .Return<List<NodeEntity>>("nodes(r)")
			              .ResultsAsync;

			return results.ToList();
		}

		public async Task<List<string>> GetAllHosts()
		{
			var results = await _client.Cypher.Match("(n:Host)")
			                     .Where("n.subnet = 'False'")
			                     .Return<string>("n.ip")
			                     .ResultsAsync;

			return results.ToList();
		}

		public void Dispose()
		{
			_client.Dispose();
		}
	}
}
