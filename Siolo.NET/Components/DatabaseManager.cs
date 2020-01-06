using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

using Newtonsoft.Json;

using Siolo.NET.Components.ElasticSearch;
using Siolo.NET.Components.Logstash;
using Siolo.NET.Components.Neo4j;
using Siolo.NET.Components.Postgre;
using Siolo.NET.Components.VT;

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

		public async Task RegisterIncident(IFormFile file, string host, string fullClass, VTShortReport shortReport)
		{
			await RegisterIncidentAtMongoAsync(file, shortReport);
			await RegisterIncidentAtElasticAsync(host, fullClass, shortReport);
		}

		private async Task RegisterIncidentAtMongoAsync(IFormFile file, VTShortReport shortReport)
		{
			await Mongo.UploadFile(file.FileName, file.OpenReadStream());

			await Mongo.InsertShortReport(shortReport.md5,
				JsonConvert.SerializeObject(shortReport, Formatting.Indented));
		}

		private async Task RegisterIncidentAtElasticAsync(string host, string fullClass, VTShortReport shortReport)
		{
			await Logstash.SendEventAsync(new EventDrop(host, shortReport.md5, fullClass));

			var incident = new EventIncident(host, shortReport.md5, fullClass);
			var possibleDestinationIp = await Elastic.FindFirstIncidentByFileHash(shortReport.md5);

			var paths = await Neo4J.FindAllPaths(possibleDestinationIp, host);

			incident.SetPossibleRoutes(JsonConvert.SerializeObject(paths));

			await Logstash.SendEventAsync(incident);
		}
	}
}
