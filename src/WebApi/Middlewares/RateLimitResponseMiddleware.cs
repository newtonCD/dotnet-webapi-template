using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebApi.Middlewares;

[ExcludeFromCodeCoverage]
public class RateLimitResponseMiddleware
{
    private readonly RequestDelegate _next;

    public RateLimitResponseMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        Stream originalBodyStream = context?.Response.Body;

        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        try
        {
            await _next(context).ConfigureAwait(false);
        }
        finally
        {
            if (context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
            {
                responseBody.SetLength(0);
                context.Response.ContentType = "application/problem+json";

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status429TooManyRequests,
                    Title = "Muitas requisições.",
                    Detail = "Excedido o limite de requisições, tente novamente mais tarde."
                };

                JsonSerializerOptions jsonOptions = context.RequestServices.GetService<JsonSerializerOptions>() ?? new JsonSerializerOptions();
                string jsonString = JsonSerializer.Serialize(problemDetails, jsonOptions);
                byte[] jsonData = Encoding.UTF8.GetBytes(jsonString);

                context.Response.ContentLength = jsonData.Length;
                await responseBody.WriteAsync(jsonData.AsMemory(0, jsonData.Length)).ConfigureAwait(false);
                responseBody.Seek(0, SeekOrigin.Begin);
            }

            responseBody.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalBodyStream).ConfigureAwait(false);
        }
    }
}