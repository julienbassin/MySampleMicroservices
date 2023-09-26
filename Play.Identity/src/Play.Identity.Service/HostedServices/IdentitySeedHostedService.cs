using System.Threading;
using System.Threading.Tasks;
using Duende.IdentityServer.Validation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Play.Common.Settings;
using Play.Identity.Service.Entities;
using Play.Identity.Service.Roles;
using Play.Identity.Service.Settings;

namespace Play.Identity.Service.HostedServices;

public class IdentitySeedHostedService : IHostedService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IdentitySettings _settings;

    public IdentitySeedHostedService(IServiceScopeFactory serviceScopeFactory,
                                    IOptions<IdentitySettings> identityOptions)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _settings = identityOptions.Value;
    }
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        await CreateRoleIfNotExistsAsync(UserRoles.Admin, roleManager);
        await CreateRoleIfNotExistsAsync(UserRoles.Player, roleManager);

        // check if admin exists ? 

        var adminUser = await userManager.FindByEmailAsync(_settings.AdminUserEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = _settings.AdminUserEmail,
                Email = _settings.AdminUserEmail
            };

            // TO DO: check if the user has been created properly
            await userManager.CreateAsync(adminUser, _settings.AdminUserPassword);
            await userManager.AddToRoleAsync(adminUser, UserRoles.Admin);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private static async Task CreateRoleIfNotExistsAsync(string role,
                                                        RoleManager<ApplicationRole> roleManager)
    {
        if (!await roleManager.RoleExistsAsync(role))
            // To DO : add some checks for the return of the creation of role 
            await roleManager.CreateAsync(new ApplicationRole { Name = role });
    }
}