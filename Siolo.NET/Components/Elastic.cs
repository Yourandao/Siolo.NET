using System;
using System.Net.Sockets;
using System.Text;
using Nest;

namespace Siolo.NET.Components
{
	public class Elastic
	{
		private readonly ElasticClient _client;

		public Elastic()
		{
			var node = new Uri("http://localhost:9200");
			var settings = new ConnectionSettings(node);

			_client = new ElasticClient(settings);
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
