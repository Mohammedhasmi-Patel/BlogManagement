using BlogManagement.Extension;
using BlogManagement.Seeders;

var builder = WebApplication.CreateBuilder(args);

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

try
{
    await DatabaseSeeder.SeedAsync(app.Services);
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred during database migration/seeding on startup.");
}

app.Run();

