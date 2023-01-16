using ElmaSmartFarm.DataLibraryCore;
using ElmaSmartFarm.DataLibraryCore.Config;
using ElmaSmartFarm.DataLibraryCore.Interfaces;
using ElmaSmartFarm.DataLibraryCore.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ElmaSmartFarm.Service;

public class Program
{
    public static IHost ServiceHost { get; private set; }
    public static async Task Main(string[] args)
    {
        var folder = "log";
        var fileName = "Logfile.txt";
        var path = Path.Combine(folder, fileName);
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.File(path)
            .WriteTo.Console()
            .CreateLogger();

        try
        {
            Log.Information("Starting up the Service...");
            ServiceHost = Host.CreateDefaultBuilder(args)
            .UseWindowsService()
            .ConfigureServices(services =>
            {
                services.AddHostedService<Worker>()
                .AddSingleton<Config, Config>()
                .AddSingleton<IDataAccess, SqlDataAccess>()
                .AddSingleton<IDbProcessor, MsSqlDbProcessor>()
                .AddSingleton<PoultryEntities, PoultryEntities>();
            })
            .UseSerilog()
            .Build();

            #region WebApi
            var builder = WebApplication.CreateBuilder(args);

            //builder.Services.AddSwaggerGen(options =>
            //{
            //    options.AddSecurityDefinition("oauth2", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            //    {
            //        Description = "Standard authorization",
            //        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            //        Name = "Authorization",
            //        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey
            //    });
            //    options.OperationFilter<SecurityRequirementsOperationFilter>();
            //});

            //builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            //{
            //    options.TokenValidationParameters = new()
            //    {
            //        ValidateLifetime = true,
            //        ValidateIssuer = false,
            //        ValidateAudience = false,
            //        ValidateIssuerSigningKey = true,
            //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(new Config().JwtKey))
            //    };
            //});
            //builder.Services.AddSingleton<PoultryEntities, PoultryEntities>();
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            var app = builder.Build();

            //if (app.Environment.IsDevelopment())
            //{
            //    app.UseSwagger();
            //    app.UseSwaggerUI();
            //}

            app.UseHttpsRedirection();

            app.UseAuthentication();
            //app.UseAuthorization();

            app.MapControllers();

            //app.MapGet("/", () => $"Hello!. Time is: {DateTime.Now}");

            _ = Task.Run(() =>
              app.Run("http://0.0.0.0:8083"));
            #endregion

            await ServiceHost.RunAsync();
            return;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "There was a problem starting up the Service.");
            return;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}