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
            await _next(context);

            // Eğer yanıt 403 Forbidden ise özelleştirilmiş bir mesaj döndür
            if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
            {
                // Yanıtı yeniden oluştur
                context.Response.ContentType = "application/json";
                var response = new
                {
                    success = false,
                    message = "Bu API'ye erişim yetkiniz yok."
                };
                context.Response.StatusCode = StatusCodes.Status403Forbidden;

                // JSON formatında yanıt gönder
                await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
            }
        }
    }
}
