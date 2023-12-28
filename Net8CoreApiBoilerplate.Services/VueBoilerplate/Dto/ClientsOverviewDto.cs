using Net8CoreApiBoilerplate.Services.Shared;
using System.Collections.Generic;

namespace Net8CoreApiBoilerplate.Services.VueBoilerplate.Dto
{
    public class ClientsOverviewItemDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Place { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public string ClientType { get; set; }
    }

    public class ClientsOverviewDto : BaseFilteredResponse
    {
        // Giving it name of "Items" just so we can keep our Grid logic un-faulty
        public List<ClientsOverviewItemDto> Items { get; set; }
    }
}
