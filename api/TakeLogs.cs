using data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace api
{
    public class TakeLogs
    {
        private readonly IDbContextFactory<_ApplicationConnectionDb> _dbFactory;
        private readonly UserInfos _userInfos;

        public TakeLogs(IDbContextFactory<_ApplicationConnectionDb> dbFactory, UserInfos userInfos)
        {
            _dbFactory = dbFactory;
            _userInfos = userInfos;
        }

        public async Task TakeIt(Guid? userId, string? title, string? action, string? exception = null, string? stackTrace = null)
        {
            var details = _userInfos.GetCurrentUserDetails();

            // Log nesnesini bir kez burada hazırlıyoruz (DRY prensibi)
            var log = new Logs
            {
                UserId = userId,
                PageNameSpaceTitle = title,
                Action = action,
                IpAddress = details.IpAddress,
                UserAgent = details.UserAgent,
                RequestPath = details.RequestPath,
                Languages = details.Languages,
                Exception = exception,
                StackTrace = stackTrace,
                Date = DateTime.UtcNow
            };

            try
            {
                await SaveToDb(log);
            }
            catch (Exception ex)
            {
                // EĞER KAYIT SIRASINDA HATA ALINIRSA:
                // Log nesnesini oluşan yeni hata bilgileriyle güncelleyip tekrar deniyoruz.
                try
                {
                    log.PageNameSpaceTitle = $"[LOGGING ERROR] {title}";
                    log.Exception = $"Original Exception: {exception} | Logging System Error: {ex.Message}";
                    log.StackTrace = ex.StackTrace;

                    await SaveToDb(log);
                }
                catch
                {
                    // Shut Up
                }
            }
        }

        // Tekrardan kaçınmak için kayıt işlemini ayırdık
        private async Task SaveToDb(Logs log)
        {
            using var db = await _dbFactory.CreateDbContextAsync();
            db.Logs.Add(log);
            await db.SaveChangesAsync();
        }
    }
}