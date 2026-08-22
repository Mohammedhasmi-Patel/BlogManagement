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

app.UseGlobalExceptionHandler();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
