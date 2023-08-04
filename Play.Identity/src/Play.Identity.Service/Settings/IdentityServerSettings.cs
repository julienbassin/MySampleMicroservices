using System;
using System.Collections.Generic;
using Duende.IdentityServer.Models;

namespace Play.Identity.Service.Settings
{
    public class IdentityServerSettings
    {
        public IReadOnlyCollection<ApiScope> ApiScopes { get; init; } = Array.Empty<ApiScope>();
        public IReadOnlyCollection<Client> Clients { get; init; }
        public IReadOnlyCollection<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };
    }
}