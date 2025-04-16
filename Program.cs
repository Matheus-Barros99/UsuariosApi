using Microsoft.EntityFrameworkCore;
using System.Configuration;
using UsuariosApi.Context;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql("server=localhost;database=usuariosdb;user=root;password=Thuquinha2513$",
        ServerVersion.AutoDetect("server=localhost;database=usuariosdb;user=root;password=Thuquinha2513$")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/openapi/v1.json",
            "v1");
    });
}

app.UseAuthorization();

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.MapControllers();

app.Run();
