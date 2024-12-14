using Microsoft.AspNetCore.Authorization;
using Pro_Web_API.Data.Repositories;
using System.Diagnostics;
using System.Reflection;

namespace Pro_Web_API.WebAPI.Middlewares
{
    public class ApiLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public ApiLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // ApiLogRepository'yi Dependency Injection üzerinden al
            var logRepository = context.RequestServices.GetRequiredService<IApiLogRepository>();

            // Kullanıcı rolleri
            var userRoles = context.User.Claims
                .Where(c => c.Type == "role")
                .Select(c => c.Value)
                .ToList();
            var roles = userRoles.Any() ? string.Join(", ", userRoles) : "Anonim Kullanıcı";

            // Cevabı yakalamak için orijinal Response.Body'yi proxy'le
            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            try
            {
                // Sonraki middleware'e devam et
                await _next(context);

                // Cevap gövdesini oku
                context.Response.Body.Seek(0, SeekOrigin.Begin);
                var responseBodyText = await new StreamReader(context.Response.Body).ReadToEndAsync();
                context.Response.Body.Seek(0, SeekOrigin.Begin);

                // Route bilgileri ve loglama işlemi
                var controllerName = context.Request.RouteValues["controller"]?.ToString();
                var actionName = context.Request.RouteValues["action"]?.ToString();
                var method = context.Request.Method;
                var path = context.Request.Path;

                // Refleksiyon ile ilgili Controller ve Action'dan 'Authorize' niteliğini al
                var controllerType = Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .FirstOrDefault(t => t.Name == $"{controllerName}Controller");

                string allowedRoles = "Yetki Bilgisi Yok";
                if (controllerType != null)
                {
                    var actionMethod = controllerType.GetMethods()
                        .FirstOrDefault(m => m.Name == actionName);

                    if (actionMethod != null)
                    {
                        var authorizeAttribute = actionMethod
                            .GetCustomAttributes<AuthorizeAttribute>()
                            .FirstOrDefault();

                        if (authorizeAttribute != null)
                        {
                            allowedRoles = authorizeAttribute.Roles ?? "Tüm Kullanıcılar";
                        }
                    }
                }

                // Log kaydet
                await logRepository.SaveLogAsync(
                    controllerName,
                    actionName,
                    method,
                    path,
                    $"İzin Verilen Kullanıcı Rolleri: {allowedRoles}",
                    roles,
                    responseBodyText
                );
            }
            finally
            {
                // Orijinal Response.Body'yi geri yükle
                await responseBody.CopyToAsync(originalBodyStream);
            }
        }

    }
}
