using Infrastructure.Constants;
using Microsoft.AspNetCore.Authorization;

namespace Infrastructure.Identity.Auth
{
    public class ShouldHavePermissionAttribute : AuthorizeAttribute
    {
        public ShouldHavePermissionAttribute(string feature, string action)
        {
            Policy = SchoolPermission.NameFor(action, feature);
        }
    }
}
