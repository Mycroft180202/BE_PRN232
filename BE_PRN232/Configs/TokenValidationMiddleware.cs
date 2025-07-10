using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using BE_PRN232.Entities;
using Microsoft.EntityFrameworkCore;

public class TokenValidationMiddleware
{
    private readonly RequestDelegate _next;

    public TokenValidationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, EcommerceClothingDbContext dbContext)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (!string.IsNullOrEmpty(token))
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Nếu userId có trong token thì kiểm tra trong DB
            if (userId != null)
            {
                var exists = await dbContext.UserRefreshTokens
                    .AnyAsync(t => t.UserId == Guid.Parse(userId) && t.Token == token);

                if (!exists)
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Token đã bị thu hồi hoặc không hợp lệ.");
                    return;
                }
            }
        }

        await _next(context);
    }
}