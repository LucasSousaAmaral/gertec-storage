using Gertec.Storage.Application.Commands;
using Gertec.Storage.Application.PipelineBehaviors;
using Gertec.Storage.Infrastructure;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 1) Add Controllers
builder.Services.AddControllers();

// 2) Add Repos
builder.Services.AddInfrastructure(builder.Configuration);

// 3) Add MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateProductCommand).Assembly));

// 4) Add Middleware
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ErrorLoggingBehavior<,>));


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();