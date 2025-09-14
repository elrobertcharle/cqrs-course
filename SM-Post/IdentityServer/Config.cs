using Duende.IdentityServer.Models;

namespace IdentityServer;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        [
        new IdentityResources.OpenId(),
        new IdentityResources.Profile(),
        ];

    private const string wgb_query_api_read_scope = "wgb_query_api.read";
    private const string wgb_command_api_write_scope = "wgb_command_api.write";
    public static IEnumerable<ApiScope> ApiScopes =>
        [
        new ApiScope(wgb_query_api_read_scope),
        new ApiScope(wgb_command_api_write_scope),
        ];

    public static IEnumerable<ApiResource> ApiResources =>
        [
        new ApiResource("wgb_query_api", "Query Api")
        {
            Scopes = { wgb_query_api_read_scope},
            // UserClaims = { "role", "email" } // opcional
        },
        new ApiResource("wgb_command_api", "Command Api")
        {
            Scopes = { wgb_command_api_write_scope },
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

                AllowedScopes = { wgb_query_api_read_scope, wgb_command_api_write_scope }
            },

            // interactive user using code flow + pkce
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
                AllowedScopes = { "openid", "profile", wgb_query_api_read_scope, wgb_command_api_write_scope }
            },
        ];
}
