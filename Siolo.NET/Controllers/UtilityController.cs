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
	}
}