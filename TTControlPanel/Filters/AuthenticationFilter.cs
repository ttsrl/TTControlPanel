using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using TTControlPanel.Services;

namespace TTControlPanel.Filters
{
    public class AuthenticationFilter : IAsyncAuthorizationFilter
    {
        private readonly DBContext _db;

        public AuthenticationFilter(DBContext db )
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var aa = context.ActionDescriptor.FilterDescriptors.Select(x => x.Filter).OfType<AuthenticationAttribute>().FirstOrDefault();
            var naa = context.ActionDescriptor.FilterDescriptors.Select(x => x.Filter).OfType<NoAuthenticationAttribute>().FirstOrDefault();

            var user = await _db.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == (context.HttpContext.Session.GetInt32("UserId") ?? -1));
            context.HttpContext.Items["User"] = user;

            if (aa != null && naa == null)
            {
                if (user == null || !user.Role["GrantLogin"])
                {
                    NotFound(context);
                    return;
                }

                if(aa.Grants != null)
                {
                    if (aa.Grants.Any(g => !user.Role[g]))
                    {
                        NotFound(context);
                        return;
                    }
                }
            }

        }

        public void NotFound(AuthorizationFilterContext context)
        {
            context.Result = new NotFoundResult();
        }
    }

    public class AuthenticationAttribute : Attribute, IFilterMetadata
    {
        public AuthenticationAttribute(params string[] grants)
        {
            Grants = grants;
        }

        public AuthenticationAttribute()
        {
            Grants = null;
        }

        public string[] Grants { get; set; }
    }

    public class NoAuthenticationAttribute : Attribute, IFilterMetadata
    {
    }

}