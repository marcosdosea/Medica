using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using Service;

namespace MedicaWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<MedicaContext>(
                options => options.UseMySQL(builder.Configuration.GetConnectionString("MedicaConnection")!));

            builder.Services.AddScoped<IPrescricaoService, PrescricaoService>();
            builder.Services.AddScoped<IPacienteService, PacienteService>();

            // Alterado para Scoped para manter o mesmo ciclo de vida dos outros serviços
            builder.Services.AddScoped<IMedicamentoService, MedicamentoService>();

            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            // ALTERADO: Agora a rota padrão inicial é o Index de Paciente!
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}