using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using Siolo.NET.Components;
using Siolo.NET.Components.Logstash;
using Siolo.NET.Components.Network;

using System.IO;
using System.Threading.Tasks;

namespace Siolo.NET.Controllers
{
	[ApiController]
	public class UtilityController : ControllerBase
	{
		private readonly DatabaseManager _manager;

		private readonly Response _response;

		public UtilityController(DatabaseManager manager)
		{
			_manager = manager;
			_response = new Response();
		}

		[Route("api/flush"), HttpPost]
		public async Task<IActionResult> FlushAll()
		{
			await _manager.Redis.Flush();

			return Ok(_response.SetStatus(true, "OK"));
		}

		[Route("test/vttest/"), HttpPost]
		public async Task<string> VtTest([FromForm] IFormFile file)
		{
			using (var ms = new MemoryStream())
			{
				file.OpenReadStream().CopyTo(ms);
				await _manager.Mongo.UploadFile(file.FileName, file.OpenReadStream());

				return JsonConvert.SerializeObject(await _manager.VirusTotal.GetShortReportFromFileBytesAsync(ms.ToArray()),
													Formatting.Indented);
			}
		}

		[Route("test/logstash_test/"), HttpPost]
		public async Task<IActionResult> Logstash_test()
		{
			await _manager.Logstash.SendEventAsync(new EventDrop("123.123.123.123/12", "this_is_md5", "ANOTHER:CLASS"));

			return Ok();
		}

		[Route("test/neo4jtest"), HttpGet]
		public async Task<IActionResult> Neo4j()
		{
			return Ok(await _manager.Neo4J.FindAllPaths("192.168.8.1/32", "192.168.9.2/32"));
		}


		[Route("test/el_test/"), HttpPost]
		public async Task<IActionResult> El_test()
		{
			//return await _manager.Elastic.FindFirstIncidentByFileHash("3af1008ba9f6dddaf99907d9458ee775");
			return Ok(await _manager.Elastic.FindAllIncidents());
		}

		[Route("test/mongotest"), HttpPost]
		public async Task MongoTest([FromBody] string hash)
		{
			await _manager.Mongo.InsertShortReport(hash, "");
		}
	}
}