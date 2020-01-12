using Newtonsoft.Json;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Siolo.NET.Components.Logstash
{
    public class Logstash
    {
        private readonly string _ip;
        private readonly int _port;

        public Logstash(string ip, int port)
        {
            _ip = ip;
            _port = port;
        }

        public async Task SendEventAsync(EventBase e)
        {
            try
            {
	            using (var client = new TcpClient(_ip, _port))
                {
		            string message = JsonConvert.SerializeObject(e);
	                var data = System.Text.Encoding.ASCII.GetBytes(message);

	                var stream = client.GetStream();
	                await stream.WriteAsync(data, 0, data.Length);

	                stream.Close();
	            }
            }
            catch (Exception exc)
            {
                Console.WriteLine($@"[Error] <Logstash::SendEventAsync: {exc.Message}");
            }
        }
    }
}
