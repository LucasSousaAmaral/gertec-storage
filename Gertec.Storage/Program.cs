using Gertec.Storage.Application.Commands;
using Gertec.Storage.Application.PipelineBehaviors;
using Gertec.Storage.Infrastructure;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateProductCommand).Assembly));

// 1) Add Controllers
builder.Services.AddControllers();

// 2) Adiciona repositórios (Infrastructure)
builder.Services.AddInfrastructure(builder.Configuration);

// 3) Registra o MediatR (buscando Handlers no Assembly do Commands ou de Application)
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateProductCommand).Assembly));

// 4) (Opcional) Se tiver Behavior de logging de erro
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ErrorLoggingBehavior<,>));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();