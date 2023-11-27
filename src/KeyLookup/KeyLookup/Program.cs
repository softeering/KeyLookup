using KeyLookup.Checks;
using KeyLookup.Contracts;
using KeyLookup.Models;
using KeyLookup.Services;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions();
var keyLookupOptionsSection = builder.Configuration.GetSection("KeyLookup");
builder.Services.Configure<KeyLookupOptions>(keyLookupOptionsSection);
var keyLookupOptions = keyLookupOptionsSection.Get<KeyLookupOptions>()!;

builder.Services.AddHealthChecks()
	.AddCheck<KeyLookupCheck>(nameof(KeyLookupCheck));

builder.Services.AddSingleton<IKeyLookupStore, FileSystemKeyLookupStore>();
builder.Services.AddSingleton<INodeManager, FileSystemNodeManager>();
builder.Services.AddHostedService<NodeRegistrationJob>();

if (keyLookupOptions.EnablePreload)
{
	// builder.Services.AddSingleton<DataLoader>();
}

builder.Services.AddGrpc(options =>
{
	options.EnableDetailedErrors = true;

}).AddJsonTranscoding();
builder.Services.AddControllers();

if (builder.Environment.IsDevelopment())
{
	builder.Services.AddGrpcReflection();
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
	app.MapGrpcReflectionService();
	app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client.");
}
else
{
	app.UseExceptionHandler("/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseRouting();

app.MapHealthChecks("/health", new HealthCheckOptions() { AllowCachingResponses = false });
app.MapGrpcService<KeyLookupGrpcService>();
app.MapControllers();

await app.RunAsync();
