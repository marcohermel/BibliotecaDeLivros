using System.Security.Principal;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Biblioteca.Models;

namespace Biblioteca
{
    public static class IdentityExtensions
    {
        private static readonly UserManager<ApplicationUser> _userManager;


        public const string RoleAdmin = "Admin";
        public const string RoleClient = "Client";
        public static bool IsAdmin(this IPrincipal principal) => principal.IsInRole(RoleAdmin);
        public static bool IsUser(this IPrincipal principal) => principal.IsInRole(RoleClient);
        public static string GetFullName(this IIdentity identity)
        {
            return ((ClaimsIdentity)identity).FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname").Value;
        }
    }
}