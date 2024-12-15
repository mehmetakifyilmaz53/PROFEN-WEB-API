using Microsoft.AspNetCore.Authorization;
using Pro_Web_API.Data.Repositories.Abstract;
using System.Reflection;

namespace Pro_Web_API.WebAPI.Middlewares
{
    public class CustomForbiddenMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomForbiddenMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var logRepository = context.RequestServices.GetRequiredService<IApiLogRepository>();

            var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "Bilinmiyor";


            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            try
            {
                var controllerName = context.Request.RouteValues["controller"]?.ToString();
                var actionName = context.Request.RouteValues["action"]?.ToString();
                var method = context.Request.Method;
                var path = context.Request.Path;
                await _next(context);

                if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
                {

                    if (controllerName == "User" && actionName == "Register")
                    {
                        var response = new
                        {
                            success = false,
                            message = "Sadece Admin kullanıcılar bu rolden kayıt yaptırabilir."
                        };
                        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
                    }
                    else
                    {
                        var response = new
                        {
                            success = false,
                            message = "Bu API'ye erişim yetkiniz yok."
                        };
                        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
                    }

                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                }


                context.Response.Body.Seek(0, SeekOrigin.Begin);
                var responseBodyText = await new StreamReader(context.Response.Body).ReadToEndAsync();
                context.Response.Body.Seek(0, SeekOrigin.Begin);




                if ((controllerName == "User" || controllerName == "Category" || controllerName == "Product") && (actionName == "Register" || actionName == "Login" || actionName == "DeleteUser" || actionName == "UpdateUser" || actionName == "DeleteProduct" || actionName == "AddProduct"))
                {
                    var controllerType = Assembly.GetExecutingAssembly()
.GetTypes()
.FirstOrDefault(t => t.Name == $"{controllerName}Controller");

                    string allowedRoles = "Bütün Kullanıcılar";
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


                    await logRepository.SaveLogAsync(
                        controllerName,
                        actionName,
                        method,
                        path,
                        $"İzin Verilen Kullanıcı Rolleri: {allowedRoles}",
                              ipAddress,
                        responseBodyText
                    );
                }

            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
            finally
            {

                await responseBody.CopyToAsync(originalBodyStream);
            }


        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var response = new
            {
                success = false,
                message = "Sunucu hatası oluştu.",
                details = exception.Message
            };

            return context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
        }
    }
}
