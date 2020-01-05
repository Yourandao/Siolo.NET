﻿using Nest;
using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
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

		public async Task<string> FindIp(string hash)
		{
			var searchResponse = await _client.SearchAsync<LogEvent>(s =>
				s.Index("logstash_generic-*")
				 .Sort(sort => sort.Ascending(obj => obj.Timestamp))
				 .Query(query =>
					 query.Bool(@bool =>
						 @bool.Must(
							 must => must.Term(c => c.Field(f => f.Md5).Value(hash)),
							 must => must.Term(c => c.Field(f => f.EventType).Value("drop"))
						)
					)
				));

			return searchResponse.Documents.First().Ip;
		}

		public void CreateTempIndex()
		{
			TcpClient tcp = new TcpClient("127.0.0.1", 5044);
			string data = "{\"data\":\"hui\"}";

			var bytes = Encoding.ASCII.GetBytes(data);
			using var stream = tcp.GetStream();

			stream.Write(bytes);
			stream.Close();

			tcp.Close();
		}
	}
}
