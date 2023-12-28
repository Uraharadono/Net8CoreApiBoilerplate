using Microsoft.AspNetCore.Identity;
using Net8CoreApiBoilerplate.Infrastructure.DbUtility;

namespace Net8CoreApiBoilerplate.DbContext.Entities.Identity
{
    public class ApplicationRole : IdentityRole<long>, IEntity
    {
        public ApplicationRole() { }
        public ApplicationRole(string name) { Name = name; }
    }
}
