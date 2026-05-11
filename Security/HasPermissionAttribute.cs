using Microsoft.AspNetCore.Authorization;

namespace Gestionale.Api.Security;

public class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(string permission)
    {
        Policy = $"{PermissionPolicyProvider.PolicyPrefix}{permission}";
    }
}
