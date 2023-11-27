using KeyLookup.Contracts;
using KeyLookup.Models;
using KeyLookup.Services;
using Microsoft.AspNetCore.Mvc;

namespace KeyLookup.Controllers;

[ApiController]
[Route("api/[Controller]")]
[MiddlewareFilter<RawRequestMiddleware>]
public class NodeController(ILogger<NodeController> logger, INodeManager nodeManager) : ControllerBase
{
	[HttpGet("")]
	public IAsyncEnumerable<KeyLookupNode> Get(CancellationToken cancellationToken = default)
	{
		return nodeManager.GetNodesAsync(cancellationToken);
	}
}
