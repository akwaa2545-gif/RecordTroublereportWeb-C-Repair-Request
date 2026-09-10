using Microsoft.AspNetCore.Identity;

namespace RepairRequest.Web.Data;

public static class RoleInitializer
{
    private static readonly string[] RoleNames = ["Requester", "Technician", "Supervisor", "Administrator"];

    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        foreach (var roleName in RoleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var result = await roleManager.CreateAsync(new IdentityRole(roleName));
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException($"Unable to create the '{roleName}' role.");
                }
            }
        }
    }
}
