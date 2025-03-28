﻿using Application;
using Application.Exceptions;
using Application.Features.Identity.Tokens;
using Finbuckle.MultiTenant.Abstractions;
using Infrastructure.Constants;
using Infrastructure.Identity.Models;
using Infrastructure.Tenancy;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Identity.Tokens
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IMultiTenantContextAccessor<ABCSchoolTenantInfo> _tenantContextAccessor;
        private readonly JwtSettings _JwtSettings;

        public TokenService(
            UserManager<ApplicationUser> userManager,
            IMultiTenantContextAccessor<ABCSchoolTenantInfo> tenantContextAccessor,
            RoleManager<ApplicationRole> roleManager,
            IOptions<JwtSettings> jwtSettings)
        {
            _userManager = userManager;
            _tenantContextAccessor = tenantContextAccessor;
            _roleManager = roleManager;
            _JwtSettings = jwtSettings.Value;
        }

        public async Task<TokenResponse> LoginAsync(TokenRequest request)
        {
            #region Validation
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

            if (_tenantContextAccessor.MultiTenantContext.TenantInfo.Id is not TenancyConstants.Root.Id) 
            {
                if (_tenantContextAccessor.MultiTenantContext.TenantInfo.ValidUpTo < DateTime.UtcNow)
                {
                    throw new UnauthorizedException(["Tenant Subscription has expired. Contact Administrator."]); 
                }
            }
            #endregion

            // Generate Jwt
            return await GenerateTokenAndUpdateUserAsync(userInDb); 
        }

        public async Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            var userPrincipal = GetClaimPrincipleFromExpiringToken(request.CurrentJwt);
            var userEmail = userPrincipal.GetEmail();

            var userInDb = await _userManager.FindByEmailAsync(userEmail)
                ?? throw new UnauthorizedException(["Authentication failed."]);

            if (userInDb.RefreshToken != request.CurrentRefreshToken || userInDb.RefreshTokenExpiryTime < DateTime.UtcNow)
            {
                throw new UnauthorizedException(["Invalid token."]);
            }

            return await GenerateTokenAndUpdateUserAsync(userInDb);
        }

        private ClaimsPrincipal GetClaimPrincipleFromExpiringToken(string expiringToken)
        {
            var tkValidationParams = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero,
                RoleClaimType = ClaimTypes.Role,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_JwtSettings.Secret))
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var pricipal = tokenHandler.ValidateToken(expiringToken, tkValidationParams, out var securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken
                || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new UnauthorizedException(["Invalid token provided. Failed to generate new token."]);
            }
            return pricipal;
        }

        private async Task<TokenResponse> GenerateTokenAndUpdateUserAsync(ApplicationUser user)
        {
            // Generate Jwt
            var newJwt = await GenerateToken(user);

            // Refresh Token
            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_JwtSettings.RefreshTokrnExpiryTimeInDays);

            await _userManager.UpdateAsync(user);

            return new TokenResponse
            {
                Jwt = newJwt,
                RefreshToken = user.RefreshToken,
                RefreshTokenExpiryDate = user.RefreshTokenExpiryTime
            };
        }

        private async Task<string> GenerateToken(ApplicationUser user)
        {
            // Encrypted Token
            return GenerateEncryptedToken(GenerateSigningCredentials(), await GetUserClaims(user)); 
        }

        private string GenerateEncryptedToken(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
        {
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_JwtSettings.TokrnExpiryTimeInMinutes),
                signingCredentials: signingCredentials);

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }

        private SigningCredentials GenerateSigningCredentials()
        {
            byte[] secret = Encoding.UTF8.GetBytes(_JwtSettings.Secret);
            return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
        }

        private async Task<IEnumerable<Claim>> GetUserClaims(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);

            var roleClaims = new List<Claim>();
            var permissionClaims = new List<Claim>();

            foreach (var userRole in userRoles)
            {
                roleClaims.Add(new Claim(ClaimTypes.Role, userRole));
                var currentRole = await _roleManager.FindByNameAsync(userRole);

                var allPermissionsForCurrentRole = await _roleManager.GetClaimsAsync(currentRole);

                permissionClaims.AddRange(allPermissionsForCurrentRole);
            }
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Name, user.FirstName),
                new(ClaimTypes.Surname, user.LastName),
                new(ClaimConstants.Tenant, _tenantContextAccessor.MultiTenantContext.TenantInfo.Id),
                new(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty)
            }
            .Union(roleClaims)
            .Union(userClaims)
            .Union(permissionClaims);

            return claims;
        }

        private string GenerateRefreshToken()
        {
            byte[] randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);
        }
    }
}
