namespace KeyLookup.Contracts;

public interface IKeyLookupStore
{
	IAsyncEnumerable<string> ListEntriesAsync(CancellationToken cancellationToken = default);
	IAsyncEnumerable<(string, string[])> ListHashEntriesAsync(CancellationToken cancellationToken = default);

	Task StoreAsync(string key, Stream content, CancellationToken cancellationToken = default);
	Task<Stream?> GetAsync(string key, CancellationToken cancellationToken = default);
	Task<IDictionary<string, Stream?>> MGetAsync(string[] keys, CancellationToken cancellationToken = default);
	Task DeleteAsync(string key, CancellationToken cancellationToken = default);

	Task HStoreAsync(string key, string field, Stream content, CancellationToken cancellationToken = default);
	Task<Stream?> HGetAsync(string key, string field, CancellationToken cancellationToken = default);
	Task<IDictionary<string, Stream?>> HMGetAsync(string key, string[] fields, CancellationToken cancellationToken = default);
	Task<IDictionary<string, Stream?>> HGetAllAsync(string key, CancellationToken cancellationToken = default);
	Task HDeleteAsync(string key, string field, CancellationToken cancellationToken = default);
	Task HDeleteAllAsync(string key, CancellationToken cancellationToken = default);
	Task HMDeleteAsync(string key, string[] fields, CancellationToken cancellationToken = default);
}
