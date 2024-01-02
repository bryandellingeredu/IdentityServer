using IdentityServer;
using IdentityServer4;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Distributed;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders().AddConsole().AddDebug().SetMinimumLevel(LogLevel.Trace);

builder.Services.AddControllersWithViews();

// Configure IdentityServer services
builder.Services.AddIdentityServer()
    .AddInMemoryClients(Config.Clients)
    .AddInMemoryApiScopes(Config.ApiScopes)
    .AddInMemoryIdentityResources(Config.IdentityResources)
    .AddInMemoryApiScopes(Config.ApiScopes)
    .AddInMemoryApiResources(Config.ApiResources)
    .AddDeveloperSigningCredential();

builder.Services.AddTransient<IProfileService, CustomProfileService>();
builder.Services.AddDistributedMemoryCache();

builder.Services.AddCors(options =>
{
    options.AddPolicy("default", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add Azure AD authentication
var eduClientId = builder.Configuration["EduClientId"];
var CACClientId = builder.Configuration["CACClientId"];
var eduClientSecret = builder.Configuration["EduClientSecret"];
var CACClientSecret = builder.Configuration["CACClientSecret"];
var eduAuthority = $"https://login.microsoftonline.com/{builder.Configuration["EduAuthority"]}";
var CACAuthority = $"https://login.microsoftonline.com/{builder.Configuration["CACAuthority"]}";

builder.Services.AddAuthentication()
    .AddOpenIdConnect("AzureADEDU", "Sign In With EDU", options =>
    {
        options.SaveTokens = true;
        options.GetClaimsFromUserInfoEndpoint = true;
        options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
        options.ClaimActions.MapUniqueJsonKey("preferred_username", "preferred_username");
        options.ClaimActions.MapUniqueJsonKey("email", "email");
        options.ClientId = eduClientId;
        options.ClientSecret = eduClientSecret;
        options.Authority = eduAuthority;
        options.ResponseType = "code";
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("email");

        options.CallbackPath = "/signin-oidc";

        options.SaveTokens = true;
        options.TokenValidationParameters.NameClaimType = "name";
        options.Events.OnUserInformationReceived = async ctx =>
        {

            var email = ctx.User.RootElement.GetString("email");
            var name = ctx.User.RootElement.GetString("name");
            if (!string.IsNullOrEmpty(email))
            {
                if (!string.IsNullOrEmpty(email))
                {
                    EmailClaimStorage.EmailClaims.TryAdd(name, email);
                }
            }
        };

    });

builder.Services.AddAuthentication()
    .AddOpenIdConnect("AzureADARMY", "Sign In With CAC", options =>
    {
        options.SaveTokens = true;
        options.GetClaimsFromUserInfoEndpoint = true;
        options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
        options.ClaimActions.MapUniqueJsonKey("preferred_username", "preferred_username");
        options.ClaimActions.MapUniqueJsonKey("email", "email");
        options.ClientId = CACClientId;
        options.ClientSecret = CACClientSecret;
        options.Authority = CACAuthority;
        options.ResponseType = "code";
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("email");

        options.CallbackPath = "/signin-oidc";

        options.SaveTokens = true;
        options.TokenValidationParameters.NameClaimType = "name";
        options.Events.OnUserInformationReceived = async ctx =>
        {

            var email = ctx.User.RootElement.GetString("email");
            var name = ctx.User.RootElement.GetString("name");
            if (!string.IsNullOrEmpty(email))
            {
                if (!string.IsNullOrEmpty(email))
                {
                    EmailClaimStorage.EmailClaims.TryAdd(name, email);
                }
            }
        };

    });

var app = builder.Build();

// Add IdentityServer middleware
app.UseCors("default");
app.UseStaticFiles();
app.UseRouting();
app.UseIdentityServer();
app.UseAuthentication(); // Add this line
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
});



app.Run();