using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace KeyLookup.Checks;

public class KeyLookupCheck : IHealthCheck
{
	public KeyLookupCheck()
	{
		
	}

	public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
	{
		// var result = Try<HealthCheckResult>(() =>
		// {
		// 	var database = this._mongoClient.GetDatabase(this._options.DataStorage?.MongoDatabase);
		// 	var collections = database.ListCollectionNames().ToList();
		// 	return database == null ? HealthCheckResult.Unhealthy("Failed connecting to database") : HealthCheckResult.Healthy("Connection to database is ok");
		// }).RecoverWith(error => HealthCheckResult.Unhealthy(error.Message)).Value;

		return Task.FromResult(HealthCheckResult.Healthy());
	}
}
