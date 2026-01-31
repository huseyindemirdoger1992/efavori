using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace api.tr
{
    public class UserInfos
    {
        private readonly IHttpContextAccessor _accessor;

        public UserInfos(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public UserDetail GetCurrentUserDetails()
        {
            // 1. Context tamamen null ise (örn. HTTP dışı bir thread'den çağrılırsa) patlamasın
            var context = _accessor?.HttpContext;
            if (context == null) return new UserDetail();

            // 2. Header'ları güvenli okumak için yardımcı bir fonksiyon (Aşağıda tanımladık)
            string GetHeader(string key) =>
                context.Request.Headers.TryGetValue(key, out var value) ? value.ToString() : "Not Provided";

            return new UserDetail
            {
                // Connection nesnesi null olabilir (test ortamlarında sık olur)
                IpAddress = context.Connection?.RemoteIpAddress?.ToString() ?? "0.0.0.0",

                // ToString() yerine güvenli metot kullanımı
                UserAgent = GetHeader("User-Agent"),
                Referrer = GetHeader("Referer"),
                Languages = GetHeader("Accept-Language"),

                // Path ve Method genelde null olmaz ama defansif kalmakta fayda var
                RequestPath = context.Request?.Path.Value ?? "UnknownPath",
                RequestMethod = context.Request?.Method ?? "UnknownMethod",

                // TraceIdentifier her zaman bir değer üretir
                TraceId = context.TraceIdentifier ?? Guid.NewGuid().ToString()
            };
        }

        public string GetIpAddress() =>
            _accessor?.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "0.0.0.0";
    }

    public record UserDetail
    {
        // Default değerleri "Unknown" yaparak boş string (empty) riskini de azaltıyoruz
        public string IpAddress { get; init; } = "Unknown";
        public string UserAgent { get; init; } = "Unknown";
        public string RequestPath { get; init; } = "Unknown";
        public string RequestMethod { get; init; } = "Unknown";
        public string Referrer { get; init; } = "Unknown";
        public string Languages { get; init; } = "Unknown";
        public string TraceId { get; init; } = "Unknown";
    }
}