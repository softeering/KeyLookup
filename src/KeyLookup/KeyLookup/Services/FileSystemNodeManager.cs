using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text.Json;
using KeyLookup.Contracts;
using KeyLookup.Models;
using Microsoft.Extensions.Options;

namespace KeyLookup.Services;

public class FileSystemNodeManager : INodeManager
{
	private const string NodeStateFileExtension = ".node";
	private readonly ILogger<FileSystemNodeManager> _logger;
	private readonly DirectoryInfo _nodesRoot;

	public FileSystemNodeManager(ILogger<FileSystemNodeManager> logger, IOptions<KeyLookupOptions> options)
	{
		this._logger = logger;

		if (!Directory.Exists(options.Value.RootFolder))
			throw new ArgumentException($"Root Folder {options.Value.RootFolder} needs to exist");

		var nodesRoot = new DirectoryInfo(Path.Combine(options.Value.RootFolder, "nodes"));
		if (!nodesRoot.Exists)
			Directory.CreateDirectory(nodesRoot.FullName);

		this._nodesRoot = nodesRoot;
	}

	public async IAsyncEnumerable<KeyLookupNode> GetNodesAsync(CancellationToken cancellationToken = default)
	{
		foreach (var nodeFile in _nodesRoot.GetFiles($"*{NodeStateFileExtension}"))
		{
			using var file = nodeFile.OpenRead();
			yield return await JsonSerializer.DeserializeAsync<KeyLookupNode>(file).ConfigureAwait(false);
		}
	}

	public async Task RegisterLocalNodeAsync()
	{
		IPAddress? localAddress = await this.GetLocalIPv4AddressAsync();
		if (localAddress is null)
			throw new Exception("Failed to define local IP address");

		var filePath = Path.Combine(_nodesRoot.FullName, $"{localAddress.ToString().Replace('.', '-')}{NodeStateFileExtension}");
		KeyLookupNode? currentState = null;

		if (File.Exists(filePath))
		{
			using var currentStream = File.OpenRead(filePath);
			currentState = await JsonSerializer.DeserializeAsync<KeyLookupNode>(currentStream).ConfigureAwait(false);
		}

		var newState = new KeyLookupNode(
			localAddress.ToString(),
			currentState?.StartedAtUtc ?? DateTime.UtcNow,
			DateTime.UtcNow
		);

		using var fileStream = File.Open(filePath, FileMode.Create, FileAccess.Write, FileShare.Write);
		await JsonSerializer.SerializeAsync(fileStream, newState).ConfigureAwait(false);
	}

	private async Task<IPAddress?> GetLocalIPv4AddressAsync()
	{
		try
		{
			string hostName = Dns.GetHostName();
			return (await Dns.GetHostEntryAsync(hostName).ConfigureAwait(false))
				.AddressList
				.FirstOrDefault(ip => !ip.ToString().Contains(':'));
		}
		catch (SocketException ex)
		{
			this._logger.LogWarning(ex, "Failed getting local IP using the DnsHostEntry. Falling back to Network interfaces...");

			return NetworkInterface.GetAllNetworkInterfaces()
				.Where(nic => nic.OperationalStatus == OperationalStatus.Up)
				.SelectMany(nic => nic.GetIPProperties().UnicastAddresses)
				.FirstOrDefault(address => !address.Address.ToString().Equals("127.0.0.1") &&  address.Address.AddressFamily == AddressFamily.InterNetwork)?
				.Address;
		}
	}
}
