
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MinimalAPIPelicula;
using MinimalAPIPelicula.Endpoints;
using MinimalAPIPelicula.Entidades;
using MinimalAPIPelicula.Migrations;
using MinimalAPIPelicula.Repositorios;
using MinimalAPIPelicula.Servicios;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);
var origenesPermitidos = builder.Configuration.GetValue<string>("origenesPermitidos")!;

//inicio area de los servicios

//con esto ya tenemos entityframework en nuestra api
builder.Services.AddDbContext<ApplicationDbContext>(opciones => 
    opciones.UseSqlServer(" name = DefaultConnection"));

builder.Services.AddCors(opciones =>
{
    opciones.AddDefaultPolicy(configuracion =>
    {
        configuracion.WithOrigins(origenesPermitidos).AllowAnyMethod().AllowAnyHeader();

    });

    opciones.AddPolicy("libre", configuracion =>
    {
        configuracion.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });


});

builder.Services.AddOutputCache();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IRepositorioGeneros, RepositorioGenerosL>();
builder.Services.AddScoped<IRepositorioActores, RepositorioActores>();
builder.Services.AddScoped<IRepositorioPeliculas, RepositorioPeliculas>();
builder.Services.AddScoped<IRepositorioComentarios, RepositorioComentarios>();

builder.Services.AddScoped<IAlmacenadorArchivos, AlmacenadorArchivosLocal>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

//fin de area de los servicios

var app = builder.Build();

//inicio de area de los middleware

app.UseSwagger();
app.UseSwaggerUI();

app.UseStaticFiles();

app.UseCors();

app.UseOutputCache();

app.MapGet("/", [EnableCors(policyName: "libre")]() => "hola, mundo");

app.MapGroup("/Generos").MapGeneros();
app.MapGroup("/Actores").MapActores();
app.MapGroup("/Peliculas").MapPeliculas();
app.MapGroup("/Pelicula/{peliculaId:int}/Comentarios").MapComentarios();

//fin de area de los middleware
app.Run();



