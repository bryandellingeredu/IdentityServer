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
builder.Services.AddAuthentication()
    .AddOpenIdConnect("AzureADEDU", "Sign In With EDU", options =>
    {
        options.SaveTokens = true;
        options.GetClaimsFromUserInfoEndpoint = true;
        options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
        options.ClaimActions.MapUniqueJsonKey("preferred_username", "preferred_username");
        options.ClaimActions.MapUniqueJsonKey("email", "email");
        options.ClientId = "158b1fe2-2fd7-4e97-9785-ed91a49a7baf";
        options.ClientSecret = "SIT8Q~Rn6773Zco4WQoPIeo~OGRx8JlmBUiQqcVD"; // Add your client secret here
        options.Authority = "https://login.microsoftonline.com/44f5f615-327a-4d5a-86d5-c9251297d7e4/v2.0";
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
        options.ClientId = "158b1fe2-2fd7-4e97-9785-ed91a49a7baf";
        options.ClientSecret = "SIT8Q~Rn6773Zco4WQoPIeo~OGRx8JlmBUiQqcVD"; // Add your client secret here
        options.Authority = "https://login.microsoftonline.com/44f5f615-327a-4d5a-86d5-c9251297d7e4/v2.0";
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