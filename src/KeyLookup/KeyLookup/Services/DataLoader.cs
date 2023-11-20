using KeyLookup.Contracts;

namespace KeyLookup.Services;

public class DataLoader
{
	private readonly ILogger<DataLoader> _logger;
	private readonly IKeyLookupStore _sourceStore;
	private readonly IKeyLookupStore _targetStore;

	public DataLoader(ILogger<DataLoader> logger, IKeyLookupStore sourceStore, IKeyLookupStore targetStore)
	{
		this._logger = logger;
		this._sourceStore = sourceStore;
		this._targetStore = targetStore;
	}

	public async Task LoadData(CancellationToken cancellationToken = default)
	{

	}
}
