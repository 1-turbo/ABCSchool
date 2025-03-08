using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Infrastructure.Constants;
using Infrastructure.Identity.Models;
using Infrastructure.Tenancy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace Infrastructure.Contexts
{
    public class ApplicationDbSeeder(
        IMultiTenantContextAccessor<ABCSchoolTenantInfo> tenantInfoContextAccessor,
        RoleManager<ApplicationRole> roleManager,
        ApplicationDbContext applicationDbContext,
        UserManager<ApplicationUser> userManager)
    {
        private readonly IMultiTenantContextAccessor<ABCSchoolTenantInfo> _tenantInfoContextAccessor = tenantInfoContextAccessor;
        private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ApplicationDbContext _applicationDbContext = applicationDbContext;

        public async Task InitializeDatabaseAsync(CancellationToken cancellationToken)
        {
            if(_applicationDbContext.Database.GetMigrations().Any())
            {
                if ((await _applicationDbContext.Database.GetPendingMigrationsAsync(cancellationToken)).Any())
                {
                    await _applicationDbContext.Database.MigrateAsync(cancellationToken);
                }

                if(await _applicationDbContext.Database.CanConnectAsync(cancellationToken))
                {
                    // Seeding
                    // Default Roles > Assign permissions/claims
                    await InitializeDefaultRolesAsync(cancellationToken);
                    // Users > Assign Roles 
                }
            }
        }

        private async Task InitializeDefaultRolesAsync(CancellationToken ct)
        {
            foreach (var roleName in RoleConstants.DefaultRoles)
            {
                if(await _roleManager.Roles.SingleOrDefaultAsync(role => role.Name == roleName, ct) is not ApplicationRole incommingRole)
                {
                    incommingRole = new ApplicationRole()
                    {
                        Name = roleName,
                        Description = $"{roleName} Role"
                    };

                    await _roleManager.CreateAsync(incommingRole);
                }

                
                if (roleName == RoleConstants.Basic)
                {
                    // Assign Basic permissions   
                    await AssignPermissionsToRole(SchoolPermissions.Basic, incommingRole, ct);
                }
                else if(roleName == RoleConstants.Admin)
                {
                    // Assign Admin permissions   
                    await AssignPermissionsToRole(SchoolPermissions.Admin, incommingRole, ct);

                    if(_tenantInfoContextAccessor.MultiTenantContext.TenantInfo.Id == TenancyConstants.Root.Id)
                    {
                        await AssignPermissionsToRole(SchoolPermissions.Root, incommingRole, ct);
                    }
                }
            }
        }

        private async Task AssignPermissionsToRole(
            IReadOnlyList<SchoolPermission> rolePermissions, 
            ApplicationRole role, CancellationToken ct)
        {
            var currentClaims = await _roleManager.GetClaimsAsync(role);

            foreach (var rolePermission in rolePermissions)
            {
                if(!currentClaims.Any(c => c.Type == ClaimConstants.Permission && c.Value == rolePermission.Name))
                {
                    await _applicationDbContext.RoleClaims.AddAsync(new ApplicationRoleClaim
                    {
                        RoleId = role.Id,
                        ClaimType = ClaimConstants.Permission,
                        ClaimValue = rolePermission.Name,
                        Description = rolePermission.Description,
                        Group = rolePermission.Group
                    }, ct);

                    await _applicationDbContext.SaveChangesAsync(ct);
                }
            }
        }

        private async Task InitializeAdminUserAsync()
        {
            if (string.IsNullOrEmpty(_tenantInfoContextAccessor.MultiTenantContext.TenantInfo.Email)) return;

            if (await _userManager.Users
                .SingleOrDefaultAsync(user => user.Email == _tenantInfoContextAccessor.MultiTenantContext.TenantInfo.Email)
                is not ApplicationUser incomingUser)
            {
                incomingUser = new ApplicationUser
                {
                    FirstName = TenancyConstants.FirstName,
                    LastName = TenancyConstants.LastName,
                    Email = tenantInfoContextAccessor.MultiTenantContext.TenantInfo.Email,
                    UserName = tenantInfoContextAccessor.MultiTenantContext.TenantInfo.Email,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    NormalizedEmail = tenantInfoContextAccessor.MultiTenantContext.TenantInfo.Email.ToUpperInvariant(),
                    NormalizedUserName = tenantInfoContextAccessor.MultiTenantContext.TenantInfo.Email.ToUpperInvariant(),
                    IsActive = true,
                };

                var passwordHash = new PasswordHasher<ApplicationUser>();

                incomingUser.PasswordHash = passwordHash.HashPassword(incomingUser, TenancyConstants.DefaultPassword);
                await _userManager.CreateAsync(incomingUser);
            }

        }
    }
}
