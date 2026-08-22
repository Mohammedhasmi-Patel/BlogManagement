using BlogManagement.Extension;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureProjectService(builder.Configuration);


var app = builder.Build();
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// await DatabaseSeeder.SeedAsync(app.Services);

app.UseGlobalExceptionHandler();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
