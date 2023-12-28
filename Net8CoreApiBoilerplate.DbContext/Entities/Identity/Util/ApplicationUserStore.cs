namespace Net8CoreApiBoilerplate.DbContext.Entities.Identity.Util
{
    public class ApplicationUserStore : Microsoft.AspNetCore.Identity.EntityFrameworkCore.UserStore<ApplicationUser, ApplicationRole, Microsoft.EntityFrameworkCore.DbContext, long, ApplicationUserClaim, ApplicationUserRole, ApplicationUserLogin, ApplicationUserToken, ApplicationRoleClaim>
    {
        public ApplicationUserStore(Microsoft.EntityFrameworkCore.DbContext context)
            : base(context)
        {
        }
    }
}
