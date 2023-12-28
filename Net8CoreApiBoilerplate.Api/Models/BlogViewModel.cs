using System.ComponentModel.DataAnnotations;

namespace Net8CoreApiBoilerplate.Api.Models
{
    public class BlogViewModel
    {
        [Required]
        public long Id { get; set; }
        [Required]
        public string Url { get; set; }
    }
}
