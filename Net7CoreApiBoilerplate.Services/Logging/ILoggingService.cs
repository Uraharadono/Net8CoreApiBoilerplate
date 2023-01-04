using Net7CoreApiBoilerplate.DbContext.Entities;
using Net7CoreApiBoilerplate.DbContext.Enums;
using Net7CoreApiBoilerplate.Infrastructure.DbUtility;
using Net7CoreApiBoilerplate.Infrastructure.Services;
using NLog;
using System;
using System.Threading.Tasks;

namespace Net7CoreApiBoilerplate.Services.Logging
{
    public interface ILoggingService : IService
    {
        // Used to save log that has commit immediately
        Task<bool> SaveLog(DateTime date, long currentUserId, ELogType type, string value, string text = null);
        // Used to save log that has will be committed elsewhere
        Task<bool> SaveLogNoCommit(DateTime date, long currentUserId, ELogType type, string value, string text = null);
    }

    public class LoggingService : ILoggingService
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IUnitOfWork _uow;
        public LoggingService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        // Used to save log that has commit immediately
        public async Task<bool> SaveLog(DateTime date, long currentUserId, ELogType type, string value, string text = null)
        {
            try
            {
                DbContext.Entities.Logging dbLogging = new DbContext.Entities.Logging();
                // dbLogging.Id = _uow.Query<Logging>().OrderBy(s => s.Oid).Select(s => s.Oid).Last() + 1;
                dbLogging.Id = new Random().NextInt64();
                dbLogging.LogDatum = date;
                dbLogging.LogType = type;
                dbLogging.LogValue = value;
                dbLogging.UserId = currentUserId;
                dbLogging.LogText = text;

                // _uow.Context.Add(dbLogging);
                await _uow.Context.Set<DbContext.Entities.Logging>().AddAsync(dbLogging);
                await _uow.CommitAsync();

                return true;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return false;
            }
        }

        // Used to save log that has will be committed elsewhere
        public async Task<bool> SaveLogNoCommit(DateTime date, long currentUserId, ELogType type, string value, string text = null)
        {
            try
            {
                DbContext.Entities.Logging dbLogging = new DbContext.Entities.Logging();
                // dbLogging.Id = _uow.Query<Logging>().OrderBy(s => s.Oid).Select(s => s.Oid).Last() + 1;
                dbLogging.Id = new Random().NextInt64();
                dbLogging.LogDatum = date;
                dbLogging.LogType = type;
                dbLogging.LogValue = value;
                dbLogging.UserId = currentUserId;
                dbLogging.LogText = text;

                await _uow.Context.Set<DbContext.Entities.Logging>().AddAsync(dbLogging);

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
