using Net8CoreApiBoilerplate.DbContext.Entities.Identity;
using Net8CoreApiBoilerplate.DbContext.Enums;
using Net8CoreApiBoilerplate.Infrastructure.DbUtility;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Net8CoreApiBoilerplate.DbContext.Entities
{
    public partial class Logging : IEntity
    {
        [Key]
        public long Id { get; set; }
        public DateTime LogDatum { get; set; }
        public ELogType LogType { get; set; }
        
        [Required, MaxLength(500), Column(TypeName = "VARCHAR")]
        public string LogValue { get; set; }

        [MaxLength(500), Column(TypeName = "VARCHAR")]
        public string LogText { get; set; }

        [Required]
        [ForeignKey("UserNavigation")]
        public long UserId { get; set; }
        public virtual ApplicationUser UserNavigation { get; set; }
    }
}
