namespace KeyLookup.Services;

public class RawRequestMiddleware
{
	public void Configure(IApplicationBuilder app)
	{
		app.Use(async (context, next) =>
		{
			context.Request.EnableBuffering();

			await next();
		});
	}
}
