using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Siolo.NET.Components;
using Siolo.NET.Components.Logstash;
using Siolo.NET.Components.Neo4j;
using Siolo.NET.Components.Network;
using Siolo.NET.Models;
// ReSharper disable All

namespace Siolo.NET.Controllers
{
	[ApiController]
	public class HostsController : ControllerBase
	{
		private readonly DatabaseManager _manager;

		private readonly Response _response;

		public HostsController(DatabaseManager manager)
		{
			_manager = manager;
			_response = new Response();
		}

		[Route("api/wildcart")] [HttpPost]
		public async Task<IActionResult> AddWildcart([FromBody] PolicyContract contract)
		{
			try
			{
				await _manager.Postgres.RegisterPolicy(contract.Info, contract.Wildcart);

				return Ok(_response.SetStatus(true, "OK"));
			}
			catch (Exception e)
			{
				return BadRequest(_response.SetStatus(false, $"NOK. {e.Message}"));
			}
		}

		[Route("api/attach")] [HttpPost]
		public async Task<IActionResult> Attach([FromBody] AttachContract contract)
		{
			try
			{
				await _manager.Postgres.AttachPolicy(contract.Ip, contract.Wildcart);
				await _manager.Redis.PushHostData(await _manager.Postgres.GetEntities());

				return Ok(_response.SetStatus(true, "OK"));
			}
			catch (Exception e)
			{
				return BadRequest(_response.SetStatus(false, $"NOK. {e.Message}"));
			}
		}

		[Route("api/link")] [HttpPost]
		public async Task<IActionResult> LinkSubnets([FromBody] RelationContract contract)
		{
			try
			{
				await _manager.Neo4J.CreateRelation(new Neo4jHostObject(contract.First, true), 
														new Neo4jHostObject(contract.Second, true));

				return Ok(_response.SetStatus(true, "OK"));
			}
			catch (Exception e)
			{
				return BadRequest(_response.SetStatus(false, $"NOK. {e.Message}"));
			}
		}

		[Route("api/upload")] [HttpPost]
		public async Task<IActionResult> UploadFile([FromForm] IFormFile file, [FromForm] string host)
		{
			try
			{
				using (var memoryStream = new MemoryStream())
				{
					file.OpenReadStream().CopyTo(memoryStream);

					var policies = _manager.Redis.GetHostWildcarts(host);
					var shortReport = await _manager.VirusTotal.GetShortReportFromFileBytesAsync(memoryStream.ToArray());

					await _manager.Logstash.SendEventAsync(new EventDrop(host, shortReport.md5, shortReport.full_class));

					if (file != null && await NetworkUtility.IsRestricted(policies, shortReport.full_class.ToLower()))
					{
						await _manager.RegisterIncident(file, host, shortReport);
						return Ok(_response.SetStatus(true, "OK. Incident registered"));
					}

					return Ok(_response.SetStatus(true, "OK"));
				}
			}
			catch (Exception e)
			{
				return BadRequest(_response.SetStatus(false, $"NOK. {e.Message}"));
			}
		}

		[Route("api/findpath")] [HttpPost]
		public async Task<IActionResult> FindPaths([FromBody] RelationContract relation)
		{
			try
			{
				return Ok(await _manager.Neo4J.FindAllPaths(relation.First, relation.Second));
			}
			catch (Exception e)
			{
				return BadRequest(_response.SetStatus(false, $"NOK. {e.Message}"));
			}
		}

		[Route("api/getall")] [HttpPost]
		public async Task<IActionResult> GetAll()
		{
			try
			{
				return Ok(await _manager.Neo4J.GetAllHosts());
			}
			catch (Exception e)
			{
				return BadRequest(_response.SetStatus(false, $"NOK. {e.Message}"));
			}
		}
	}
}