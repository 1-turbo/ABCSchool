﻿using Domain.Entities;
using Finbuckle.MultiTenant.Abstractions;
using Infrastructure.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Contexts
{
    internal class ApplicationDbContext : BaseDbContext
    {
        public ApplicationDbContext(
            IMultiTenantContextAccessor<ABCSchoolTenantInfo> tenantInfoContextAccessor, 
            DbContextOptions<ApplicationDbContext> options) 
            : base(tenantInfoContextAccessor, options)
        {
        }
        public DbSet<School> Schools => Set<School>();
    }
}
