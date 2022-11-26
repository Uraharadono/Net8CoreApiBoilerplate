using Microsoft.AspNetCore.Identity;
using Net7CoreApiBoilerplate.Infrastructure.DbUtility;

namespace Net7CoreApiBoilerplate.DbContext.Entities.Identity
{
    public class ApplicationRole : IdentityRole<long>, IIdentityEntity
    {
        public ApplicationRole() { }
        public ApplicationRole(string name) { Name = name; }
    }
}
