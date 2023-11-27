using KeyLookup.Models;

namespace KeyLookup.Contracts;

public interface INodeManager
{
	IAsyncEnumerable<KeyLookupNode> GetNodesAsync(CancellationToken cancellationToken = default);
	Task RegisterLocalNodeAsync();
}
