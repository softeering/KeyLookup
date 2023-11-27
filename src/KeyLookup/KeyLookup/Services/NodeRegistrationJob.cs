using KeyLookup.Contracts;
using KeyLookup.Models;
using Microsoft.Extensions.Options;

namespace KeyLookup.Services;

public class NodeRegistrationJob(ILogger<NodeRegistrationJob> logger, IOptions<KeyLookupOptions> options, INodeManager nodeManager)
	: BackgroundService
{
	private readonly TimeSpan _heartBeatInterval = options.Value.NodeHeartBeatInterval;

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				await nodeManager.RegisterLocalNodeAsync().ConfigureAwait(false);
			}
			catch (Exception error)
			{
				logger.LogError(error, "Error occurred while registering local node");
			}
			finally
			{
				await Task.Delay(_heartBeatInterval, stoppingToken).ConfigureAwait(false);
			}
		}
	}
}
