using Duende.IdentityServer.Models;

namespace IdentityServer;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        [
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        ];

    public static IEnumerable<ApiScope> ApiScopes =>
        [
            new ApiScope("post_query_api.read"),
        ];

    public static IEnumerable<ApiResource> ApiResources =>
        [
            new ApiResource("post_query_api", "Post Query Api")
            {
                Scopes = { "post_query_api.read" },
                // UserClaims = { "role", "email" } // opcional
            }
        ];

    public static IEnumerable<Client> Clients =>
        [
            // m2m client credentials flow client
            new Client
            {
                ClientId = "developer",
                ClientName = "Client Credentials Client",

                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = { new Secret("Abc12345".Sha256()) },

                AllowedScopes = { "post_query_api.read" }
            },

            // interactive client using code flow + pkce
            new Client
            {
                ClientId = "ui",
                ClientSecrets = { new Secret("Abc12345".Sha256()) },

                AllowedGrantTypes = GrantTypes.Code,
                RequirePkce = true,

                RedirectUris = { "http://localhost:3000/api/auth/callback/duende" },
                //FrontChannelLogoutUri = "https://localhost:44300/signout-oidc",
                PostLogoutRedirectUris = { "http://localhost:3000" },

                AllowOfflineAccess = true,
                AllowedScopes = { "openid", "profile", "post_query_api.read" }
            },
        ];
}
