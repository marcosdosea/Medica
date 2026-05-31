using Microsoft.EntityFrameworkCore;
using Service;
using Core.Service;
using Core;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<MedicaContext>(options =>
    options.UseMySQL(builder.Configuration.GetConnectionString("MedicaConnection") ?? ""));

builder.Services.AddScoped<IPacienteService, PacienteService>();
builder.Services.AddScoped<IMedicamentoService, MedicamentoService>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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