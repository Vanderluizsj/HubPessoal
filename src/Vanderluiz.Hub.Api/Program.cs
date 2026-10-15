using Scalar.AspNetCore;
using Vanderluiz.Hub.Application;
using Vanderluiz.Hub.Infrastructure;
using Vanderluiz.Hub.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Adiciona controllers e suporte a OpenAPI
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Configuração do CORS para os clientes (MVC, Blazor, etc.)
var origensCors = builder.Configuration.GetSection("Cors:OrigensPermitidas").Get<string[]>()
    ?? ["http://localhost:5280", "https://localhost:7236"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("HubClientPolicy", policy =>
    {
        policy.WithOrigins(origensCors)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Camadas de Aplicação e Infraestrutura
var connectionString = builder.Configuration.GetConnectionString("HubDb")
    ?? throw new InvalidOperationException("Connection string 'HubDb' não configurada na API.");

builder.Services.AddApplication();
builder.Services.AddInfrastructure(connectionString);

var app = builder.Build();

// Inicialização automática do banco de dados (Migrations + Seed)
if (app.Configuration.GetValue("Database:InicializarAoIniciar", app.Environment.IsDevelopment()))
{
    using var scope = app.Services.CreateScope();
    try
    {
        var initializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();
        await initializer.InicializarAsync();
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Erro ao inicializar o banco de dados na inicialização da API.");
    }
}

// Pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "Vanderluiz Hub API";
        options.Theme = ScalarTheme.Purple;
        options.DefaultHttpClient = new(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.UseHttpsRedirection();
app.UseCors("HubClientPolicy");
app.UseAuthorization();

app.MapControllers();

app.Run();
