using Pro_Web_API.Core.Entities;

namespace Pro_Web_API.Data.Repositories
{
    public interface IApiLogRepository
    {
        Task SaveLogAsync(string controller, string action, string method, string path, string message, string ipAddress, string response);
    }
}
