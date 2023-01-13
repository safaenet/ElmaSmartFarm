using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using MQTTnet;
using MQTTnet.Client;
using Serilog;
using Serilog.Events;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.IO;
using System.Text;
using System.Threading;

var folder = "log";
var fileName = "Logfile.txt";
var path = Path.Combine(folder, fileName);
Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .Enrich.FromLogContext()
                    .WriteTo.File(path)
                    .CreateLogger();

try
{
    //var builder = WebApplication.CreateBuilder(args);

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
    //        //ValidateLifetime = true,
    //        ValidateIssuerSigningKey = true,
    //        //ValidIssuer = builder.Configuration["Jwt:Issuer"],
    //        //ValidAudience = builder.Configuration["Jwt:Audience"],
    //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    //    };
    //});

    //var options = new MqttClientOptionsBuilder()
    //    .WithClientId(Guid.NewGuid().ToString())
    //    .WithTcpServer("192.168.1.106", 1883)
    //    .WithCleanSession()
    //    .Build();
    //IMqttClient mqttclient = new MqttFactory().CreateMqttClient();
    //var connection = mqttclient.ConnectAsync(options, CancellationToken.None);
    //connection.Wait();
    //var res = connection.Result;

    //builder.Services.AddSingleton<IMqttClient>(mqttclient);

    //builder.Services.AddControllers();
    //builder.Services.AddEndpointsApiExplorer();
    
    //var app = builder.Build();

    //if (app.Environment.IsDevelopment())
    //{
    //    app.UseSwagger();
    //    app.UseSwaggerUI();
    //}

    //app.UseHttpsRedirection();

    //app.UseAuthentication();
    //app.UseAuthorization();

    //app.MapControllers();

    //app.MapGet("/", () => $"Hello Safa!. Time: {DateTime.Now}");

    //app.Run("http://localhost:8083");
}
catch (Exception ex)
{
    Log.Error(ex, "Error in Program.cs");
}