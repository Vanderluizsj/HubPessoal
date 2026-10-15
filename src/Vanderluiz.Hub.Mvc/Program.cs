using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Vanderluiz.Hub.Application;
using Vanderluiz.Hub.Infrastructure;
using Vanderluiz.Hub.Infrastructure.Data;
using Vanderluiz.Hub.Mvc.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.Configure<PerfilOptions>(builder.Configuration.GetSection(PerfilOptions.Secao));

var connectionString = builder.Configuration.GetConnectionString("HubDb")
    ?? throw new InvalidOperationException("Connection string 'HubDb' não configurada.");

builder.Services.AddApplication();
builder.Services.AddInfrastructure(connectionString);

var app = builder.Build();

// Aplica migrations e seed de exemplo (controlado por "Database:InicializarAoIniciar").
if (app.Configuration.GetValue("Database:InicializarAoIniciar", app.Environment.IsDevelopment()))
{
    using var scope = app.Services.CreateScope();
    try
    {
        await scope.ServiceProvider.GetRequiredService<DbInitializer>().InicializarAsync();
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Falha ao inicializar o banco de dados. Verifique a connection string 'HubDb'.");
    }
}

var ptBr = new CultureInfo("pt-BR");
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(ptBr),
    SupportedCultures = [ptBr],
    SupportedUICultures = [ptBr]
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/erro");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/erro/{0}");

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
