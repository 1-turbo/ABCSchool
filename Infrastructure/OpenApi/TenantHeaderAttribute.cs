using Infrastructure.Tenancy;

namespace Infrastructure.OpenApi
{
    public class TenantHeaderAttribute()
        : SwaggerHearderAttribute(
            headerName: TenancyConstants.TenantIdName,
            description: "Enter your tenant name to access this API.",
            defaultName: string.Empty,
            isRequired: true,
            defaultValue: string.Empty)
    {
    }
}

