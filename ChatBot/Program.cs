using ChatBot.Components;
using ChatBot.Components.Account;
using ChatBot.Data;
using ChatBot.Models;
using ChatBot.Services;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
    .AddIdentityCookies();
builder.Services.AddIdentityCore<ApplicationUser>()
    .AddDefaultTokenProviders()
    .AddSignInManager();
builder.Services.AddScoped<IUserStore<ApplicationUser>, JsonUserStore>();
builder.Services.AddTransient<IEmailSender<ApplicationUser>, ChatBot.Services.NoOpEmailSender>();
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});
builder.Services.AddHttpContextAccessor();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
});
builder.Services.AddAuthorizationBuilder();

var openAiApiKey = builder.Configuration["OpenAI:ApiKey"] ?? throw new Exception("Brak klucza API z OpenAI w appsettings.json.");

builder.Services.AddHttpClient("OpenAI");
builder.Services.AddSingleton<JsonSettingsStore>();
builder.Services.AddScoped<OpenAIService>(sp =>
{
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient("OpenAI");
    var logger = sp.GetRequiredService<ILogger<OpenAIService>>();
    var settingsStore = sp.GetRequiredService<JsonSettingsStore>();
    var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
    var userId = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "default";

    return new OpenAIService(httpClient, openAiApiKey, logger, settingsStore, httpContextAccessor);
});
builder.Services.AddScoped<ChatMemory>();

var app = builder.Build();
app.UseCookiePolicy();
app.UseStatusCodePages(async context =>
{
    if (context.HttpContext.Response.StatusCode / 100 is 4 or 5)
        context.HttpContext.Response.Redirect("/Error");
});
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.UseStaticFiles();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();