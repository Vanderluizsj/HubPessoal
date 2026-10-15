using Vanderluiz.Hub.Blazor.Components;
using Vanderluiz.Hub.Blazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Estado de autenticação do Admin (Modo Leitura vs Modo Admin)
builder.Services.AddScoped<AdminStateService>();

// Registra o cliente HTTP para consumo da Web API
var apiBaseUrl = builder.Configuration["Api:BaseUrl"] ?? "https://localhost:7066";
builder.Services.AddHttpClient<HubApiClient>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(15);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
