using Net7CoreApiBoilerplate.Infrastructure.Services;
using Net7CoreApiBoilerplate.Services.VueBoilerplate.Dto;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System;
using System.Threading.Tasks;
using Net7CoreApiBoilerplate.Infrastructure.DbUtility;
using NLog;
using Net7CoreApiBoilerplate.Services.Email;
using System.Runtime;
using Microsoft.EntityFrameworkCore;

namespace Net7CoreApiBoilerplate.Services.VueBoilerplate
{
    public interface IVueBoilerplateService : IService
    {
        Task<ClientsOverviewDto> GetClients(ClientsOverviewFilterDto filter);
    }

    public class VueBoilerplateService : IVueBoilerplateService
    {
        private readonly IUnitOfWork _uow;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        public VueBoilerplateService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ClientsOverviewDto> GetClients(ClientsOverviewFilterDto filter)
        {
            try
            {
                // I am using the mock database class called "MockSephirothClient" and extension method
                // but in your case it would be proper Database table object like commented code below
                IQueryable<MockSephirothClient> query = MockSephirothData.GetMockSephirothData()
                                                            .Where(s => s.IsActive == true)
                                                            .Include(s => s.ClientTypeNavigation)
                                                            .Include(s => s.Addresses)
                                                            .AsNoTracking();

                #region How things should be done properly in production enviroment
                // DO NOT USE VAR HERE; IT WONT WORK:
                // https://stackoverflow.com/questions/50423538/cannot-convert-implicitly-a-type-system-linq-iqueryable-into-microsoft-entity
                //IQueryable<Client> query = _uow.Query<Client>()
                //    .Where(s => s.IsActive == true)
                //    .Include(s => s.ClientTypeNavigation)
                //    .Include(s => s.Addresses)
                //    .AsNoTracking();
                #endregion

                if (!string.IsNullOrEmpty(filter.Name))
                    query = query.Where(s => s.Name.Contains(filter.Name));
                if (!string.IsNullOrEmpty(filter.Email))
                    query = query.Where(s => s.Name.Contains(filter.Email));
                // Address part of query
                if (!string.IsNullOrEmpty(filter.Address))
                    query = query.Where(s => s.Addresses.FirstOrDefault() != null &&
                                             (s.Addresses.First().Street.Contains(filter.Address) ||
                                              s.Addresses.First().Number.Contains(filter.Address) ||
                                              s.Addresses.First().NumberAddition.Contains(filter.Address)
                                             ));
                if (!string.IsNullOrEmpty(filter.Place))
                    query = query.Where(s => s.Addresses.FirstOrDefault() != null &&
                                             (s.Addresses.First().PostCode.Contains(filter.Place) || s.Addresses.First().Place.Contains(filter.Place)));
                if (!string.IsNullOrEmpty(filter.ClientType))
                    query = query.Where(s => s.ClientTypeNavigation.Name.ToLower().Contains(filter.ClientType.ToLower().Trim()));

                int count = query.Count();

                if (string.IsNullOrWhiteSpace(filter.SortFieldName))
                {
                    query = query.OrderBy(current => current.Oid);
                }
                else
                {
                    #region How things where working before EF Core 3.0
                    // More info here: https://stackoverflow.com/questions/59494591/net-core-3-invalidoperationexception-on-orderby-with-dynamic-field-name
                    // And here: https://github.com/dotnet/efcore/issues/19091

                    // Try 1:
                    // query = filter.SortDirection == "0" ?
                    // query.OrderBy(current => current.GetPropertyValue(filter.SortFieldName)) :
                    // query.OrderByDescending(current => current.GetPropertyValue(filter.SortFieldName));

                    // Try 2:
                    //if (filter.SortDirection == "0")
                    //    query = query.Sort(filter.SortFieldName, true);
                    //else
                    //    query = query.Sort(filter.SortFieldName, false);
                    #endregion

                    // Unfortunately I have to do sorting like this, as even if reflection was working, I have to order by Address as well
                    var isAscending = filter.SortDirection != "0";
                    switch (filter.SortFieldName.ToLower())
                    {
                        case "name":
                            query = isAscending ? query.OrderBy(s => s.Name) : query.OrderByDescending(s => s.Name);
                            break;
                        case "email":
                            query = isAscending ? query.OrderBy(s => s.Email) : query.OrderByDescending(s => s.Email);
                            break;
                        case "address":
                            query = isAscending ? query.OrderBy(s => s.Addresses.First().Street) : query.OrderByDescending(s => s.Addresses.First().Street);
                            break;
                        case "place":
                            query = isAscending ? query.OrderBy(s => s.Addresses.First().PostCode) : query.OrderByDescending(s => s.Addresses.First().PostCode);
                            break;
                        case "clienttype":
                            query = isAscending ? query.OrderBy(s => s.ClientTypeNavigation.Name) : query.OrderByDescending(s => s.ClientTypeNavigation.Name);
                            break;
                    }
                }

                var data = query
                    .Skip(filter.Skip)
                    .Take(filter.Take)
                    .ToList();

                var response = new ClientsOverviewDto
                {
                    Count = count,
                    Items = new List<ClientsOverviewItemDto>()
                };

                response.Items.AddRange(data.Select(client => new ClientsOverviewItemDto
                {
                    Id = client.Oid,
                    Name = client.Name,
                    Address = client.Addresses.FirstOrDefault()?.AddressLine1,
                    Place = client.Addresses.FirstOrDefault()?.Place,
                    Telephone = client.Telephone,
                    Email = client.Email,
                    ClientType = client.ClientTypeNavigation.Name
                }).ToList()
                );
                return response;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }
        }

    }
}
