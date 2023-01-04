using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Net7CoreApiBoilerplate.DbContext.Entities.Identity;
using Net7CoreApiBoilerplate.Infrastructure.DbUtility;

namespace Net7CoreApiBoilerplate.DbContext.Entities
{
    public class Author : IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required, StringLength(255), Column(TypeName = "VARCHAR")]
        public string PenName { get; set; }

        // 1 on 1 relationship with ApplicationUserId
        public long ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        [InverseProperty("AuthorNavigation")]
        public List<Post> Posts { get; set; }
    }
}
