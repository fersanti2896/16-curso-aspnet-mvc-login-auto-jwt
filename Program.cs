using Dominio.IRepository;
using Dominio.IServicios;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Persistencia.Context;
using Persistencia.Repository;
using Servicios.Services;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("Conexion");

builder.Services.AddDbContext<ApplicationDbContext>(
                    opciones => { opciones.UseSqlServer( connectionString ); }
                 );

builder.Services.AddScoped<IConsultaGraficaService, ConsultaGraficaService>();
builder.Services.AddScoped<IConsultaGraficaRepository, ConsultaGraficaRepository>();
builder.Services.AddScoped<IConsultaPedidoService, ConsultaPedidoService>();
builder.Services.AddScoped<IConsultaPedidoRepository, ConsultaPedidoRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

string SecretKey = builder.Configuration["Jwt:SecretKey"];
var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters {
                                    ValidateIssuer           = true,
                                    ValidateAudience         = true,
                                    ValidateLifetime         = true,
                                    ValidateIssuerSigningKey = true,
                                    IssuerSigningKey         = securityKey,
                                    ClockSkew                = TimeSpan.Zero,
                                    ValidIssuer              = builder.Configuration["Jwt:Issuer"],
                                    ValidAudience            = builder.Configuration["Jwt:Audience"],
                                }
                );

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
    //app.UseDeveloperExceptionPage();
    app.UseExceptionHandler(a => a.Run(async context => {
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature.Error;
                
        var result = JsonConvert.SerializeObject(new {
                                                    error = "Ocurrió un error, consulte a su adminstrador"
                                });
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsync(result);
    }));
} else {
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseAuthentication();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}");

app.Run();
