﻿using System.Threading.Tasks;
using System.Linq;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

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
		public Mongo Mongo { get; }

		public Redis Redis { get; }

		public Neo4J Neo4J { get; }

		public Postgres Postgres { get; }

		public Elastic Elastic { get; }

		public VirusTotal VirusTotal { get; }

		public Logstash.Logstash Logstash { get; }

		public DatabaseManager(IConfiguration config)
		{
			string host = config["Host"];

			Mongo = new Mongo(host, config["Mongo:Port"], config["Mongo:Login"], 
									config["Mongo:Password"], "vt_reports", "short_vt_reports");

			Redis = new Redis(host, config["Redis:Port"]);
			Neo4J = new Neo4J(host, config["Neo4j:Port"], config["Neo4j:Login"], config["Neo4j:Password"]);

			Postgres = new Postgres(host, config["Postgre:Port"], config["Postgre:Login"], 
											config["Postgre:Password"], config["Postgre:Database"]);

			Elastic = new Elastic(host, config["ElasticSearch:Port"]);

			VirusTotal = new VirusTotal(@"Resources\.virustotal.api", @"Resources\sigs.json", Mongo);
			Logstash = new Logstash.Logstash(host, int.Parse(config["Logstash:Port"]));
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
				//var parentSubnetPolicies = Redis.GetHostWildcarts(subnetIp);
				//var enumer = parentSubnetPolicies.GetAsyncEnumerator();

				//while (enumer.Current != null && await enumer.MoveNextAsync())
				//{
				//	await Postgres.AttachPolicy(ip, enumer.Current);
				//}

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

			incident.SetPossibleRoutes(paths);
			ExcludeRestrictedRoutes(incident, shortReport);

			await Logstash.SendEventAsync(incident);
		}

		private void ExcludeRestrictedRoutes(EventIncident e, VTShortReport shortReport)
		{
			e.PossibleRoutes = (from route in e.PossibleRoutes
									  where route.Skip(1).Take(route.Length - 2).All(host =>
														  NetworkUtility.IsSubnet(host) || 
														  !NetworkUtility.IsRestricted(Redis.GetHostWildcarts(host), shortReport.full_class.ToLower()).Result)
									  select route).ToArray();
		}
	}
}
