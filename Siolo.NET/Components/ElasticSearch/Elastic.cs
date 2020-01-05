using Nest;
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
			var searchResponse = await _client.SearchAsync<HitClass>(s =>
				s.Index("logstash_generic-*")
				 .Query(q =>
					q.Bool(b =>
						b.Must(
							m => m.Match(match => match.Field(obj => obj.md5).Query(hash)), 
							m => m.Match(match => match.Field(obj => obj.event_type).Query("drop"))
						)
					)
				));

			return searchResponse.Documents.First().src_ip;
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
