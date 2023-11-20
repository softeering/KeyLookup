namespace KeyLookup.Contracts;

public interface IKeyLookupStore
{
	IAsyncEnumerable<string> ListEntriesAsync(CancellationToken cancellationToken = default);
	IAsyncEnumerable<(string, string[])> ListHashEntriesAsync(CancellationToken cancellationToken = default);

	Task StoreAsync(string key, ReadOnlyMemory<byte> content, CancellationToken cancellationToken = default);
	Task<ReadOnlyMemory<byte>?> GetAsync(string key, CancellationToken cancellationToken = default);
	Task<IDictionary<string, ReadOnlyMemory<byte>?>> MGetAsync(string[] keys, CancellationToken cancellationToken = default);
	Task DeleteAsync(string key, CancellationToken cancellationToken = default);

	Task HStoreAsync(string key, string field, ReadOnlyMemory<byte> content, CancellationToken cancellationToken = default);
	Task<ReadOnlyMemory<byte>?> HGetAsync(string key, string field, CancellationToken cancellationToken = default);
	Task<IDictionary<string, ReadOnlyMemory<byte>?>> HMGetAsync(string key, string[] fields, CancellationToken cancellationToken = default);
	Task<IDictionary<string, ReadOnlyMemory<byte>?>> HGetAllAsync(string key, CancellationToken cancellationToken = default);
	Task HDeleteAsync(string key, string field, CancellationToken cancellationToken = default);
	Task HDeleteAllAsync(string key, CancellationToken cancellationToken = default);
	Task HMDeleteAsync(string key, string[] fields, CancellationToken cancellationToken = default);
}
