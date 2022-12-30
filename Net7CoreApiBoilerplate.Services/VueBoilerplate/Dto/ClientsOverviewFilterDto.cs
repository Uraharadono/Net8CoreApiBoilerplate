using Net7CoreApiBoilerplate.Services.Shared;

namespace Net7CoreApiBoilerplate.Services.VueBoilerplate.Dto
{
    public class ClientsOverviewFilterDto : BaseGridFilter
    {
        // Note: Keep in mind for grid header search filter to work, we need to keep same name of variable as we did in our "columns" list on frontend
        public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Place { get; set; }
        public string ClientType { get; set; }
    }
}
