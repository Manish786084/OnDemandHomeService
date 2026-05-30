using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace OnDemandHomeService.Filters
{
    public class RoleAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly int[] _roles;
        public RoleAuthorizeAttribute(params int[] roles)
        {
                _roles = roles;
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var userId = context.HttpContext.Session.GetInt32("UserId");
            var roleId = context.HttpContext.Session.GetInt32("Role");
            if(userId==null)
            {
                context.Result = new RedirectToActionResult("RegisterLogin", "Auth", null);
                return;
            }
            if (roleId == null || !_roles.Contains(roleId.Value))
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Auth", null);
            }
        }
    }
}
