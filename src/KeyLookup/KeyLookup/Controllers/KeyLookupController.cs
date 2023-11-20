using KeyLookup.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace KeyLookup.Controllers;

[Controller]

public class KeyLookupController(ILogger<KeyLookupController> logger, IKeyLookupStore store) : ControllerBase
{
	[HttpPost("entry/{key}")]
	public async Task Store(string key, CancellationToken cancellationToken = default)
	{

	}

	[HttpGet("entry/{key}")]
	public async Task Get(string key, CancellationToken cancellationToken = default)
	{

	}
}
