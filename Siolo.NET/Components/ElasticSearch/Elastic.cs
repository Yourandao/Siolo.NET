using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siolo.NET.Components.ElasticSearch
{
	public class Elastic
	{
		private readonly ElasticClient _client;

		public Elastic(string host, string port)
		{
			var node = new Uri($"http://{host}:{port}");
			var settings = new ConnectionSettings(node);

			_client = new ElasticClient(settings);
		}

		public async Task<string> FindFirstOccurrenceIpByFileHash(string hash)
		{
			var searchResponse = await _client.SearchAsync<LogEvent>(search =>
				search.Index("logstash_generic-*")
					.Sort(sort => sort.Ascending(obj => obj.timestamp))
					.Query(query =>
						query.Bool(@bool =>
							@bool.Must(
								must => must.Term(c => c.Field(f => f.md5).Value(hash)),
								must => must.Term(c => c.Field(f => f.event_type).Value("drop"))
							)
						)
					));

			return searchResponse.Documents.First().ip;
		}

		public async Task<IEnumerable<object>> FindAllIncidents()
		{
			var searchResponse = await _client.SearchAsync<LogEvent>(search =>
				search.Index("logstash_generic-*")
					.Sort(sort => sort.Descending(obj => obj.timestamp))
					.Query(query =>
						query.Match(match =>
							match.Field(obj => obj.event_type).Query("incident")))
			);

			var incidents = new List<object>();

			foreach (var hit in searchResponse.HitsMetadata.Hits)
			{
				incidents.Add(new { Id = hit.Id, Data = hit.Source });
			}

			return incidents;
		}

		public async Task<IEnumerable<LogEvent>> FindIncident(string id)
		{
			var searchResponse = await _client.SearchAsync<LogEvent>(new SearchRequest() { Query = new RawQuery($"{{\"term\": {{\"_id\": {{\"value\": \"{id}\"}}}}}}") });
			return (from src in searchResponse.Hits select src.Source).ToList();
		}
	}
}
