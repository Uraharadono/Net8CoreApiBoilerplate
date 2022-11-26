using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Net7CoreApiBoilerplate.Infrastructure.DbUtility;
using Net7CoreApiBoilerplate.Infrastructure.Services;
using Net7CoreApiBoilerplate.Services.Blog.Dto;
using NLog;
#pragma warning disable 1998

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
                var blogs = _uow.Query<DbContext.Entities.Blog>().ToList();

                if (!blogs.Any())
                    return null;

                return blogs.Select(s => new BlogDto { Id = s.Oid, Url = s.Url }).ToList();
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
                var blog = _uow.Query<DbContext.Entities.Blog>(s => s.Oid == blogId).FirstOrDefault();
                if (blog == null)
                    return null;

                return new BlogDto { Id = blog.Oid, Url = blog.Url };
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

                _uow.Context.Set<DbContext.Entities.Blog>().Add(dbBlog);
                _uow.Commit();

                dto.Id = dbBlog.Oid;

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
                var dbBlog = _uow.Query<DbContext.Entities.Blog>(s => s.Oid == dto.Id).FirstOrDefault();

                if (dbBlog == null)
                    return false;

                dbBlog.Oid = dto.Id;
                dbBlog.Url = dto.Url;
                _uow.Commit();

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
