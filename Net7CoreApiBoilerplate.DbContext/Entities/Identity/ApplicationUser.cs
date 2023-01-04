using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Net7CoreApiBoilerplate.Infrastructure.DbUtility;

namespace Net7CoreApiBoilerplate.DbContext.Entities.Identity
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser<long>, IEntity
    {
        [Required, Column(TypeName = "VARCHAR(250)")]
        public string FirstName { get; set; }

        [Required, Column(TypeName = "VARCHAR(250)")]
        public string LastName { get; set; }

        [Required, Column(TypeName = "VARCHAR(100)")]
        public string Title { get; set; }

        public DateTime BirthDate { get; set; }

        //[Required]
        //[DefaultValue(EGenderType.Unresolved)]
        //public EGenderType Gender { get; set; }

        /* =============== Navigation properties =============== */

        // 1 on 1 relationship with Author
        public virtual Author Author { get; set; }



        /* =============== Non-mapped properties =============== */
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";
    }
}
