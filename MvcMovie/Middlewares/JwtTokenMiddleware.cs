namespace MvcMovie.Middlewares;
public class JwtTokenMiddleware
{
    private readonly RequestDelegate _next;

    public JwtTokenMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var token = context.Request.Cookies["token"];

        if (!string.IsNullOrEmpty(token))
        {
            context.Request.Headers.Add("Authorization", "Bearer " + token);
        }

        await _next(context);
    }
}
