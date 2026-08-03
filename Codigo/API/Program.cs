using Microsoft.EntityFrameworkCore;
using Service;
using Core.Service;
using Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MedicaContext>(options =>
    options.UseMySQL(builder.Configuration.GetConnectionString("MedicaConnection") ?? ""));

builder.Services.AddScoped<IPacienteService, PacienteService>();
builder.Services.AddScoped<IMedicamentoService, MedicamentoService>();
builder.Services.AddScoped<IPlanejamentoService, PlanejamentoService>();
builder.Services.AddScoped<IExecucaoService, ExecucaoService>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

// Configuração atualizada do Swagger
builder.Services.AddSwaggerGen(c =>
{
    // Utiliza o nome completo da classe (incluindo o namespace) 
    // para gerar o schema e evitar erros 500 por nomes duplicados
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