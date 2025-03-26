using Infrastructure.Tenancy;

namespace Infrastructure.OpenApi
{
    public class TenantHeaderAttribute()
        : SwaggerHearderAttribute(
            headerName: TenancyConstants.TenantIdName,
            description: "Enter Your tenant name to access This Api.",
            defaultName: string.Empty,
            isRequired: true)
    {
    }
}
