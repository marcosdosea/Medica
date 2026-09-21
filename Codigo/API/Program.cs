using Microsoft.EntityFrameworkCore;
using Service;
using Core.Service;
using Core;

using MedicaAPI.Filter;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MedicaContext>(options =>
    options.UseMySQL(builder.Configuration.GetConnectionString("MedicaConnection") ?? ""));

builder.Services.AddScoped<IPacienteService, PacienteService>();
builder.Services.AddScoped<IMedicamentoService, MedicamentoService>();
builder.Services.AddScoped<IPlanejamentoService, PlanejamentoService>();
builder.Services.AddScoped<IExecucaoService, ExecucaoService>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiExceptionFilter>();
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.CustomSchemaIds(type => type.FullName);
});

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