using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Net8CoreApiBoilerplate.Infrastructure.DbUtility;

namespace Net8CoreApiBoilerplate.DbContext.Entities
{
    public class Post : IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public long BlogId { get; set; }
        public Blog Blog { get; set; }

        [Required]
        [ForeignKey("AuthorNavigation")]
        public long AuthorId { get; set; }
        public Author AuthorNavigation { get; set; }
    }
}
