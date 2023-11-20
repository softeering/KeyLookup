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

if (keyLookupOptions.EnablePreload)
{
	// builder.Services.AddSingleton<DataLoader>();
}

builder.Services.AddGrpc(options =>
{

}).AddJsonTranscoding();
builder.Services.AddControllers();

if (builder.Environment.IsDevelopment())
{
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
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
