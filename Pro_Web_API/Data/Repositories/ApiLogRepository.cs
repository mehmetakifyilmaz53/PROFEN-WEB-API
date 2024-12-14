
using Pro_Web_API.Core.Entities;
using Pro_Web_API.Data.Contexts;

namespace Pro_Web_API.Data.Repositories
{
    public class ApiLogRepository : IApiLogRepository
    {
        private readonly AppDbContext _context;

        public ApiLogRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task SaveLogAsync(string controller, string action, string method, string path, string message, string roles, string response)
        {
            var log = new ApiLog
            {
                ControllerName = controller,
                ActionName = action,
                HttpMethod = method,
                RequestPath = path,
                LogTime = DateTime.Now.AddTicks(-(DateTime.Now.Ticks % TimeSpan.TicksPerSecond)),
                Message = message,
                Roles = roles,
                Response = response
            };

            _context.ApiLogs.Add(log);
            await _context.SaveChangesAsync();
        }


    }
}
