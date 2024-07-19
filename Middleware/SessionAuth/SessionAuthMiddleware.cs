using ASP.NET_Classwork.Data;
using ASP.NET_Classwork.Data.Entities;
using System.Globalization;

namespace ASP.NET_Classwork.Middleware.SessionAuth
{
    public class SessionAuthMiddleware
    {
        // при побудові проєкту визначається послідовність запуску всіх Middleware і кожен з них у конструктор приймає посилання на наступний - _next, задача - зберегти його
        private readonly RequestDelegate _next;

        public SessionAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        // Робота Middleware - кожен запит буде оброблятись цим методом
        // Оскільки конструктор вже задіяний, інжекція здійснюється через параметри методу
        public async Task InvokeAsync(HttpContext context, DataContext dataContext)
        {
            // Перевіряємо чи є у сесії збережений токен
            if (context.Session.Keys.Contains("token")) 
            { 
                // Вилучаємо ID токену з сесійного сховища
                String tokenId = context.Session.GetString("token")!;
                // Переводимо до GUID
                Guid id = Guid.Parse(tokenId);
                // Шукаємо у БД і перевіряємо чи знайдений
                if (dataContext.Tokens.Find(id) is Token token) // Pattern Maching
                {
                    // Перевіряємо придатність (термін дії)
                    if (token.ExpiresAt > DateTime.Now)
                    {
                        // Зберігаємо токен у контексті
                        context.Items.Add("token", token);
                    }
                }
            }
            // Передача управління наступному Middleware
            await _next(context);
            // Після цього виклику - зворотний хід (вихід Response)
        }
    }
}
