using GiftPalette.Components;
using GiftPalette.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add HttpClient for API calls
builder.Services.AddHttpClient();

// Add application services
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddSingleton<IOrderService, OrderService>();

// Configure AIChatConfiguration with environment variable priority
builder.Services.Configure<AIChatConfiguration>(config =>
{
    // First try to get from environment variables (higher priority)
    var endpoint = Environment.GetEnvironmentVariable("AIChatConfiguration__Endpoint") 
                  ?? builder.Configuration["AIChatConfiguration:Endpoint"] 
                  ?? string.Empty;
    
    var agentId = Environment.GetEnvironmentVariable("AIChatConfiguration__AgentId") 
                 ?? builder.Configuration["AIChatConfiguration:AgentId"] 
                 ?? string.Empty;
    
    config.Endpoint = endpoint;
    config.AgentId = agentId;
});

// Configure ApiConfiguration with environment variable priority
builder.Services.Configure<ApiConfiguration>(config =>
{
    var baseUrl = Environment.GetEnvironmentVariable("ApiConfiguration__BaseUrl") 
                 ?? builder.Configuration["ApiConfiguration:BaseUrl"] 
                 ?? "http://localhost:5062";
    
    config.BaseUrl = baseUrl;
});

builder.Services.AddScoped<IAIChatService, AIChatService>();

// Add Chat State Service (Singleton for global state management)
builder.Services.AddSingleton<ChatStateService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
