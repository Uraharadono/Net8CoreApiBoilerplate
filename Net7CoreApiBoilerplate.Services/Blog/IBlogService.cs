using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Net7CoreApiBoilerplate.Infrastructure.DbUtility;
using Net7CoreApiBoilerplate.Infrastructure.Services;
using Net7CoreApiBoilerplate.Services.Blog.Dto;
using NLog;

namespace Net7CoreApiBoilerplate.Services.Blog
{
    public interface IBlogService : IService
    {
        Task<List<BlogDto>> GetBlogs();
        Task<BlogDto> GetBlog(long blogId);
        Task<BlogDto> AddBlog(BlogDto dto);
        Task<bool> UpdateBlog(BlogDto dto);
    }

    public class BlogService : IBlogService
    {
        private readonly IUnitOfWork _uow;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public BlogService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<BlogDto>> GetBlogs()
        {
            try
            {
                var blogs = await _uow.Query<DbContext.Entities.Blog>()
                                      .AsNoTracking()
                                      .ToListAsync();
                if (!blogs.Any())
                    return null;

                return blogs.Select(s => new BlogDto { Id = s.Id, Url = s.Url }).ToList();
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return null;
            }
        }

        public async Task<BlogDto> GetBlog(long blogId)
        {
            try
            {
                var blog = _uow.Query<DbContext.Entities.Blog>(s => s.Id == blogId)
                               .AsNoTracking()
                               .FirstOrDefault();
                if (blog == null)
                    return null;

                return new BlogDto { Id = blog.Id, Url = blog.Url };
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return null;
            }
        }

        public async Task<BlogDto> AddBlog(BlogDto dto)
        {
            try
            {
                var dbBlog = new DbContext.Entities.Blog
                {
                    Url = dto.Url
                };

                await _uow.Context.Set<DbContext.Entities.Blog>().AddAsync(dbBlog);
                await _uow.CommitAsync();

                dto.Id = dbBlog.Id;

                return dto;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return null;
            }
        }

        public async Task<bool> UpdateBlog(BlogDto dto)
        {
            try
            {
                var dbBlog = await _uow.Query<DbContext.Entities.Blog>(s => s.Id == dto.Id).FirstOrDefaultAsync();

                if (dbBlog == null)
                    return false;

                dbBlog.Id = dto.Id;
                dbBlog.Url = dto.Url;
                await _uow.CommitAsync();

                return true;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return false;
            }
        }
    }
}
