using Application.Exceptions;
using Application.Features.Identity.Tokens;
using Finbuckle.MultiTenant.Abstractions;
using Infrastructure.Identity.Models;
using Infrastructure.Tenancy;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity.Tokens
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMultiTenantContextAccessor<ABCSchoolTenantInfo> _tenantContextAccessor;

        public TokenService(
            UserManager<ApplicationUser> userManager, 
            IMultiTenantContextAccessor<ABCSchoolTenantInfo> tenantContextAccessor)
        {
            _userManager = userManager;
            _tenantContextAccessor = tenantContextAccessor;
        }

        public async Task<TokenResponse> LoginAsync(TokenRequest request)
        {
            // Validation
            if (! _tenantContextAccessor.MultiTenantContext.TenantInfo.IsActive)
            {
                throw new UnauthorizedException(["Tenant Subscription is not active. Contact Administrator."]);
            }
            var userInDb = await _userManager.FindByNameAsync(request.Username)
                ?? throw new UnauthorizedException(["Authentication not successful."]);

            if (await _userManager.CheckPasswordAsync(userInDb, request.Password))
            {
                throw new UnauthorizedException(["Incorrect Username or Password."]);
            }

            if (userInDb.IsActive)
            {
                throw new UnauthorizedException(["User Not Active. Contact Administrator."]);
            }

           
        }

        public Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
