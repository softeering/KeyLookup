using System.Runtime.CompilerServices;
using KeyLookup.Contracts;
using KeyLookup.Models;
using Microsoft.Extensions.Options;

namespace KeyLookup.Services;

public class FileSystemKeyLookupStore : IKeyLookupStore
{
	private const string DataFileExtension = ".kl";
	private readonly ILogger<FileSystemKeyLookupStore> _logger;
	private readonly DirectoryInfo _entriesRoot;
	private readonly DirectoryInfo _hashesRoot;

	public FileSystemKeyLookupStore(ILogger<FileSystemKeyLookupStore> logger, IOptions<KeyLookupOptions> options)
	{
		this._logger = logger;

		if (!Directory.Exists(options.Value.RootFolder))
			throw new ArgumentException($"Root Folder {options.Value.RootFolder} needs to exist");

		this._entriesRoot = new DirectoryInfo(Path.Combine(options.Value.RootFolder, "entries"));
		this._hashesRoot = new DirectoryInfo(Path.Combine(options.Value.RootFolder, "hashes"));
	}

	public async IAsyncEnumerable<string> ListEntriesAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		foreach (var file in this._entriesRoot.GetFiles("*.kl", SearchOption.AllDirectories))
		{
			yield return Path.GetFileNameWithoutExtension(file.Name);
		}
	}

	public async IAsyncEnumerable<(string, string[])> ListHashEntriesAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		foreach (var directory in this._hashesRoot.GetDirectories("*", SearchOption.AllDirectories))
		{
			var files = directory.GetFiles("*.kl");
			if (files.Length > 0)
				yield return (directory.Name, files.Select(f => Path.GetFileNameWithoutExtension(f.Name)).ToArray());
		}
	}

	public async Task StoreAsync(string key, ReadOnlyMemory<byte> content, CancellationToken cancellationToken = default)
	{
		var filePath = ComputeEntryFilePath(this._entriesRoot, key);
		using Stream target = File.Open(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
		await target.WriteAsync(content, cancellationToken);
	}

	public async Task HStoreAsync(string key, string field, ReadOnlyMemory<byte> content, CancellationToken cancellationToken = default)
	{
		var filePath = ComputeHashFilePath(this._entriesRoot, key, field);
		using Stream target = File.Open(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
		await target.WriteAsync(content, cancellationToken);
	}

	public Task DeleteAsync(string key, CancellationToken cancellationToken = default)
	{
		var filePath = ComputeEntryFilePath(this._entriesRoot, key);
		if (File.Exists(filePath))
			File.Delete(filePath);

		return Task.CompletedTask;
	}

	public Task HDeleteAsync(string key, string? field, CancellationToken cancellationToken = default)
	{
		if (field is null)
		{
			var filePath = ComputeHashDirectoryPath(this._entriesRoot, key);
			if (Directory.Exists(filePath))
				Directory.Delete(filePath);
		}
		else
		{
			var filePath = ComputeHashFilePath(this._entriesRoot, key, field);
			if (File.Exists(filePath))
				File.Delete(filePath);
		}

		return Task.CompletedTask;
	}

	public Task HDeleteAllAsync(string key, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	public Task HMDeleteAsync(string key, string[] fields, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	public async Task<ReadOnlyMemory<byte>?> GetAsync(string key, CancellationToken cancellationToken = default)
	{
		var filePath = ComputeEntryFilePath(this._entriesRoot, key);

		if (File.Exists(filePath))
		{
			using Stream source = File.OpenRead(filePath);
			var result = new ReadOnlyMemory<byte>();
			await source.WriteAsync(result, cancellationToken);
			return result;
		}
		else
		{
			return null;
		}
	}

	public Task<IDictionary<string, ReadOnlyMemory<byte>?>> MGetAsync(string[] keys, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	public Task<ReadOnlyMemory<byte>?> HGetAsync(string key, string field, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	public Task<IDictionary<string, ReadOnlyMemory<byte>?>> HMGetAsync(string key, string[] fields, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	public Task<IDictionary<string, ReadOnlyMemory<byte>?>> HGetAllAsync(string key, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	private static string ComputeEntryFilePath(DirectoryInfo root, string key)
	{
		var slot = 1000;
		var path = Path.Combine(root.FullName, slot.ToString(), $"{key}{DataFileExtension}");
		EnsureDirectory(path);
		return path;
	}

	private static string ComputeHashFilePath(DirectoryInfo root, string key, string field)
	{
		var slot = 1000;
		var path = Path.Combine(root.FullName, slot.ToString(), key, $"{field}{DataFileExtension}");
		EnsureDirectory(path);
		return path;
	}

	private static string ComputeHashDirectoryPath(DirectoryInfo root, string key)
	{
		var slot = 1000;
		var path = Path.Combine(root.FullName, slot.ToString(), key);
		EnsureDirectory(path);
		return path;
	}

	private static void EnsureDirectory(string path)
	{
		if (!Directory.Exists(Path.GetDirectoryName(path)))
			Directory.CreateDirectory(Path.GetDirectoryName(path)!);
	}
}
