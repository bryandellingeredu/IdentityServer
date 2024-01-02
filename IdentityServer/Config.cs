using IdentityServer4;
using IdentityServer4.Models;

namespace IdentityServer
{
    public class Config
    {
        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new Client
                {
                    ClientId = "react_client",
                    ClientName = "React Client",
                    AllowedGrantTypes = GrantTypes.Code,
                    ClientUri = "http://localhost:3000/registration",
                    RequireClientSecret = false,
                    RedirectUris = { "http://localhost:3000/registration/callback" },
                    PostLogoutRedirectUris = { "http://localhost:3000/registration" },
                    AllowedCorsOrigins = { "http://localhost:3000" },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "API"
                    },
                    AllowAccessTokensViaBrowser = true,
                },
                new Client
                    {
                        ClientId = "APIClient",
                         ClientSecrets = { new Secret("secret".Sha256()) },
                         AllowedGrantTypes = GrantTypes.ClientCredentials,
                        AllowedScopes = { "API" }
                    },
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
               new ApiScope("API", "API")
            };

        public static IEnumerable<ApiResource> ApiResources =>
        new ApiResource[]
       {

       };

        public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
       {
           new IdentityResources.OpenId(),
           new IdentityResources.Profile(),
           new IdentityResources.Email(),
       };
    }
}
