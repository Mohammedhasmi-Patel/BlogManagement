using BlogManagement.Extension;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureProjectService(builder.Configuration);


var app = builder.Build();


app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
