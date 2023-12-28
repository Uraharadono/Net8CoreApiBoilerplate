using Net8CoreApiBoilerplate.Infrastructure.Services;
using Net8CoreApiBoilerplate.Services.VueBoilerplate.Dto;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using Net8CoreApiBoilerplate.Infrastructure.DbUtility;
using NLog;
using Microsoft.EntityFrameworkCore;
using Net8CoreApiBoilerplate.Infrastructure.Settings;
using Net8CoreApiBoilerplate.Utility.Extensions;
using System.IO;

namespace Net8CoreApiBoilerplate.Services.VueBoilerplate
{
    /* ============================================================================
     * YOU DO NOT NEED THIS SERVICE OR ANYTHING IN PARENT DIRECTORY OF THIS FILE
     * It was created only for the purposes of showing off different functionalities
     * for my other boilerplate project: "Vue 3 Webpack Boilerplate V2"
     * https://github.com/Uraharadono/Vue3WebpackBoilerplateV2/
     * ============================================================================ */
    public interface IVueBoilerplateService : IService
    {
        Task<ClientsOverviewDto> GetClients(ClientsOverviewFilterDto filter);
        Task<bool> AddArticleDocument(MockDocumentUploadDto dto);
    }

    public class VueBoilerplateService : IVueBoilerplateService
    {
        private readonly IUnitOfWork _uow;
        private readonly IDocumentSettings _documentSettings;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        public VueBoilerplateService(IUnitOfWork uow, IDocumentSettings documentSettings)
        {
            _uow = uow;
            _documentSettings = documentSettings;
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
                    query = query.OrderBy(current => current.Id);
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
                    Id = client.Id,
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

        public async Task<bool> AddArticleDocument(MockDocumentUploadDto dto)
        {
            try
            {
                var guid = Guid.NewGuid();

                #region Poco how to save new record in database using raw SQL
                //var doc = new MockArticleDocument
                //{
                //    OriginalName = dto.FileName,
                //    UniqueName = guid + "." + dto.FileName.GetExtension(),
                //    DocumentType = dto.DocumentType,
                //    Date = DateTime.Now,
                //    ArticleId = dto.ArticleId,
                //    // Publisher = dto.CurrentPublisherId,
                //};

                //await using (var context = _uow.Context)
                //{
                //    await using (var dbContextTransaction = await context.Database.BeginTransactionAsync())
                //    {
                //        try
                //        {
                //            var commandText = @"INSERT INTO ArticleDocument 
                //            (
                //             OID
                //            ,Date
                //            ,OriginalName
                //            ,UniqueName
                //            ,ArticleId
                //            ,DocumentType
                //            -- ,Publisher
                //            )
                //            VALUES (
                //                    NEXT VALUE FOR ArticleDocumentSeq,
                //                    @Date,
                //                    @OriginalName,
                //                    @UniqueName,
                //                    @ArticleId,
                //                    @DocumentType
                //                    -- ,@Publisher

                //            )";
                //            var date = new SqlParameter("@Date", doc.Date);
                //            var name = new SqlParameter("@OriginalName", doc.OriginalName);
                //            var uniqueName = new SqlParameter("@UniqueName", doc.UniqueName);
                //            var articleId = new SqlParameter("@ArticleId", doc.ArticleId);
                //            var documentType = new SqlParameter("@DocumentType", doc.DocumentType ?? "OrderContents");
                //            // var publisher = new SqlParameter("@Publisher", doc.Publisher);

                //            context.Database.ExecuteSqlRaw(commandText, date, name, uniqueName, articleId, documentType);// publisher, 
                //            dbContextTransaction.Commit();
                //        }
                //        catch (Exception e)
                //        {
                //            _logger.Error(e);
                //            await dbContextTransaction.RollbackAsync();
                //            System.Diagnostics.Debug.WriteLine(e.Message);
                //            return false;
                //        }
                //    }
                //}
                #endregion

                // Check if folder for upload exists, create it if it doesn't
                var directoryPath = _documentSettings.BaseFolder + _documentSettings.ArticleDocumentsFolder;
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                var documentPath = _documentSettings.BaseFolder + _documentSettings.ArticleDocumentsFolder + guid + $".{dto.FileName.GetExtension()}";
                if (File.Exists(documentPath))
                {
                    File.Delete(documentPath);
                }

                await File.WriteAllBytesAsync(documentPath, dto.FileData);

                return true;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                System.Diagnostics.Debug.WriteLine(e.Message);
                return false;
            }
        }
    }
}
