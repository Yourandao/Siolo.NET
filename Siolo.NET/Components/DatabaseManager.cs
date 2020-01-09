using System.Threading.Tasks;
using System.Linq;

using Microsoft.AspNetCore.Http;

using Newtonsoft.Json;

using Siolo.NET.Components.ElasticSearch;
using Siolo.NET.Components.Logstash;
using Siolo.NET.Components.Neo4j;
using Siolo.NET.Components.Postgre;
using Siolo.NET.Components.VT;
using Siolo.NET.Components.Network;

namespace Siolo.NET.Components
{
	public class DatabaseManager
	{
		private const string Host = "104.248.28.149";

		public Mongo Mongo { get; }

		public Redis Redis { get; }

		public Neo4J Neo4J { get; }

		public Postgres Postgres { get; }

		public Elastic Elastic { get; }

		public VirusTotal VirusTotal { get; }

		public Logstash.Logstash Logstash { get; }

		public DatabaseManager()
		{
			Mongo = new Mongo(Host, "27017", "vt_reports", "short_vt_reports");
			Redis = new Redis(Host, "6379");
			Neo4J = new Neo4J(Host, "7687", "neo4j", "test");
			Postgres = new Postgres(Host, "5432");
			Elastic = new Elastic(Host, "9200");
			VirusTotal = new VirusTotal(@"Resources\.virustotal.api", @"Resources\sigs.json", Mongo);
			Logstash = new Logstash.Logstash(Host, 5044);
		}

		public async Task UpdateRedisStorage()
		{
			await Redis.PushHostData(await Postgres.GetEntities());
		}

		public async Task<bool> PushNewHost(string ip)
		{
			string subnetIp = await Neo4J.PushHostRaw(ip);

			if (subnetIp != default)
			{
				var parentSubnetPolicies = Redis.GetHostWildcarts(subnetIp);

				await foreach (var policy in parentSubnetPolicies)
				{
					await Postgres.AttachPolicy(ip, policy);
				}
			}

			if (!await Postgres.InsertNewOne(ip))
			{
				return false;
			}

			await UpdateRedisStorage();

			return true;
		}

		public async Task RegisterIncident(IFormFile file, string host, VTShortReport shortReport, string restrictingPolicy)
		{
			await RegisterIncidentAtMongoAsync(file, shortReport);
			await RegisterIncidentAtElasticAsync(host, shortReport, restrictingPolicy);
		}

		private async Task RegisterIncidentAtMongoAsync(IFormFile file, VTShortReport shortReport)
		{
			await Mongo.UploadFile(file.FileName, file.OpenReadStream());

			await Mongo.InsertShortReport(shortReport.md5,
				JsonConvert.SerializeObject(shortReport, Formatting.Indented));
		}

		private async Task RegisterIncidentAtElasticAsync(string host, VTShortReport shortReport, string restrictingPolicy)
		{
			var incident = new EventIncident(host, shortReport.md5, shortReport.full_class, restrictingPolicy);
			var firstOccurrenceIp = await Elastic.FindFirstOccurrenceIpByFileHash(shortReport.md5);

			var paths = await Neo4J.FindAllPaths(firstOccurrenceIp, host);

			incident.SetPossibleRoutes(JsonConvert.SerializeObject(paths));
			ExcludeRestrictedRoutes(incident, shortReport);

			await Logstash.SendEventAsync(incident);
		}

		private void ExcludeRestrictedRoutes(EventIncident e, VTShortReport shortReport)
		{
			e.PossibleRoutes = (from route in e.PossibleRoutes
									  where route.All(host =>
											!NetworkUtility.IsRestricted(Redis.GetHostWildcarts(host), shortReport.full_class.ToLower()).Result)
									  select route).ToArray();
		}
	}
}
