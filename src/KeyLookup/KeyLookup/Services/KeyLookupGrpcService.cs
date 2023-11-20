using System.Net;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using KeyLookup.Contracts;

namespace KeyLookup.Services;

public class KeyLookupGrpcService(ILogger<KeyLookupGrpcService> logger, IKeyLookupStore store) : KeyLookupService.KeyLookupServiceBase
{
	public override async Task<Empty> Store(StoreRequest request, ServerCallContext context)
	{
		await store.StoreAsync(request.Key, request.Content.Memory, context.CancellationToken);
		return new Empty();
	}

	public override async Task<GetResponse> Get(GetRequest request, ServerCallContext context)
	{
		var payload = await store.GetAsync(request.Key, context.CancellationToken);

		if (payload.HasValue)
		{
			context.GetHttpContext().Response.StatusCode = (int)HttpStatusCode.OK;
			return new GetResponse() { Content = ByteString.CopyFrom(payload.Value.ToArray()) };
		}

		context.GetHttpContext().Response.StatusCode = (int)HttpStatusCode.NotFound;
		return new GetResponse() { Content = ByteString.Empty };
	}
}
