using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Siolo.NET.Components;
using Siolo.NET.Components.Network;

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

		[Route("api/flush")] [HttpGet]
		public async Task<IActionResult> FlushAll()
		{
			await _manager.Redis.Flush();

			return Ok(_response.SetStatus(true, "OK"));
		}

		[Route("api/eltest")] [HttpGet]
		public IActionResult ElkTest()
		{
			_manager.Elastic.CreateTempIndex();

			return Ok();
		}


		[Route("api/n4test")]
		[HttpGet]
		public async Task<IActionResult> N4Test()
		{
			return Ok(await _manager.Neo4J.FindAllPaths("192.168.16.3/24", "192.168.2.4/24"));
		}
	}
}