using BibliotecaWeb.Filter;
using Core;
using Core.Service;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using MedicaWeb.Areas.Identity.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Service;

namespace MedicaWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            DotNetEnv.Env.TraversePath().Load();

            var builder = WebApplication.CreateBuilder(args);

            var caminhoFirebase = Path.Combine(builder.Environment.ContentRootPath, builder.Configuration["Firebase:CredentialPath"] ?? "");
            if (File.Exists(caminhoFirebase))
            {
                if (FirebaseApp.DefaultInstance == null)
                {
                    FirebaseApp.Create(new AppOptions()
                    {
                        Credential = CredentialFactory.FromFile<ServiceAccountCredential>(caminhoFirebase).ToGoogleCredential()
                    });
                }
            }

            builder.Services.AddControllersWithViews(options =>
            {
                options.Filters.Add<CustomExceptionFilter>();
            });
            builder.Services.AddRazorPages();
            builder.Services.AddHttpClient();

            var connectionString = builder.Configuration.GetConnectionString("MedicaConnection")!;

            builder.Services.AddDbContext<MedicaContext>(options =>
                options.UseMySQL(connectionString));

            builder.Services.AddDbContext<IdentityContext>(options =>
                options.UseMySQL(connectionString));

            builder.Services.AddDefaultIdentity<Usuario>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.SignIn.RequireConfirmedEmail = true;
                options.SignIn.RequireConfirmedPhoneNumber = false;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.User.AllowedUserNameCharacters =
                        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<IdentityContext>()
            .AddSignInManager<ApplicationSignInManager>();

            builder.Services.AddTransient<IEmailSender, IdentityEmailSender>();

            builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
                options.TokenLifespan = TimeSpan.FromHours(2)
            );

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = "BibliotecaCookieName";
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
                options.SlidingExpiration = true;
            });

            builder.Services.AddScoped<IPacienteService, PacienteService>();
            builder.Services.AddScoped<IMedicamentoService, MedicamentoService>();
            builder.Services.AddScoped<IPlanejamentoService, PlanejamentoService>();
            builder.Services.AddScoped<ICuidadorService, CuidadorService>();
            builder.Services.AddTransient<IVinculoService, VinculoService>();
            builder.Services.AddScoped<INotificacaoService, NotificacaoService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IDispositivoService, DispositivoService>();

            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapRazorPages();

            app.Run();
        }
    }
}