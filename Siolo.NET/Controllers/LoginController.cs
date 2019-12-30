using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Siolo.NET.Components;
using Siolo.NET.Components.Network;
using Siolo.NET.Models;

namespace Siolo.NET.Controllers
{
	[ApiController]
	public class LoginController : ControllerBase
	{
		private readonly DatabaseManager _manager;

		private readonly Response _response;

		public LoginController(DatabaseManager manager)
		{
			_manager = manager;
			_response = new Response();
		}

		[Route("api/login")] [HttpPost]
		public async Task<IActionResult> Login([FromBody] IpContract contract)
		{
			try
			{
				await _manager.Neo4J.PushHostRaw(contract.Ip);

				if (!await _manager.Postgres.InsertNewOne(contract.Ip))
				{
					return Unauthorized(_response.SetStatus(false, "NOK. Failed to push into postgres"));
				}

				await _manager.Redis.PushHostData(await _manager.Postgres.GetEntities());

				return Ok(_response.SetStatus(true, "OK"));
			}
			catch (Exception e)
			{
				return BadRequest(_response.SetStatus(false, $"NOK. {e.Message}"));
			}
		}

		[Route("api/logout")] [HttpPost]
		public async Task<IActionResult> Logout([FromBody] IpContract contract)
		{
			try
			{
				await _manager.Neo4J.DeleteHost(contract.Ip);

				return Ok(_response.SetStatus(true, "Ok"));
			}
			catch (Exception e)
			{
				return BadRequest(_response.SetStatus(false, $"NOK. {e.Message}"));
			}
		}
	}
}
