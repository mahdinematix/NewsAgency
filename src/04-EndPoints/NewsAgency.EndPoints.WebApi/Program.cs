using NewsAgency.EndPoints.WebApi.ExceptionHandling;
using NewsAgency.Infra.Configuration;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;
var connectionString = configuration.GetConnectionString("NewsAgencyDb");

services.WireUp(connectionString);

services.AddExceptionHandler<GlobalExceptionHandler>();

services.AddProblemDetails();

services.AddControllers();

services.AddSwaggerGen();


var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program
{
}