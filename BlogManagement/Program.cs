using BlogManagement.Extension;
using BlogManagement.Seeders;

var builder = WebApplication.CreateBuilder(args);

// Dynamically bind to the PORT environment variable provided by Vercel
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

builder.Services.ConfigureProjectService(builder.Configuration);
var app = builder.Build();
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseGlobalExceptionHandler();

app.UseCors("FrontendCors");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => Results.Ok(new { status = "healthy", message = "BlogManagement API is running." }));

// Run seeding in background without blocking server startup (prevents Vercel initialization timeout)
_ = Task.Run(async () =>
{
    try
    {
        await Task.Delay(1000); // small delay to ensure Kestrel is listening
        await DatabaseSeeder.SeedAsync(app.Services);
    }
    catch (Exception ex)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during database migration/seeding on startup.");
    }
});

app.Run();

