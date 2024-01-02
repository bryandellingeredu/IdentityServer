using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

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
                        "movieAPI"
                    },
                    AllowAccessTokensViaBrowser = true,
                },
                new Client
                    {
                        ClientId = "movieClient",
                         ClientSecrets = { new Secret("secret".Sha256()) },
                         AllowedGrantTypes = GrantTypes.ClientCredentials,
                        AllowedScopes = {"movieAPI"}
                    },
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
               new ApiScope("movieAPI", "Movie API")
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
