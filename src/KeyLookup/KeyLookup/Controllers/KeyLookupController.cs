using System.Net;
using KeyLookup.Contracts;
using KeyLookup.Services;
using Microsoft.AspNetCore.Mvc;

namespace KeyLookup.Controllers;

[ApiController]
[Route("api")]
[MiddlewareFilter<RawRequestMiddleware>]
public class KeyLookupController(ILogger<KeyLookupController> logger, IKeyLookupStore store) : ControllerBase
{
	[HttpPost("entry/{key}")]
	public async Task<IActionResult> Store(string key, CancellationToken cancellationToken = default)
	{
		await store.StoreAsync(key, this.Request.Body, cancellationToken);
		return Ok();
	}

	[HttpGet("entry/{key}")]
	public async Task Get(string key, CancellationToken cancellationToken = default)
	{
		var result = await store.GetAsync(key, cancellationToken);
		if (result is null)
		{
			this.Response.StatusCode = (int)HttpStatusCode.NotFound;
		}
		else
		{
			this.Response.StatusCode = (int)HttpStatusCode.OK;
			await result.CopyToAsync(this.Response.Body);
		}
	}
}
