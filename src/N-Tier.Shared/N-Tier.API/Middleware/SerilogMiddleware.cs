using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Events;

namespace N_Tier.API.Middleware;

public class SerilogMiddleware
{
    private const string MessageTemplate =
        "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
    
    private static readonly ILogger Log = Serilog.Log.ForContext<SerilogMiddleware>();
    
    private readonly RequestDelegate _next;
    
    public SerilogMiddleware(RequestDelegate next)
    {
        if (next == null) throw new ArgumentNullException(nameof(next));
        _next = next;
    }
    
    public async Task Invoke(HttpContext httpContext)
    {
        if (httpContext == null) throw new ArgumentNullException(nameof(httpContext));
        
        var sw = Stopwatch.StartNew();
        try
        {
            await _next(httpContext);
            sw.Stop();
            
            var statusCode = httpContext.Response?.StatusCode;
            var level = statusCode > 499 ? LogEventLevel.Error : LogEventLevel.Information;
            
            var log = level == LogEventLevel.Error
                ? LogForErrorContext(httpContext, GetType().Assembly.GetName().Version.ToString())
                : Log;
            log.Write(level, MessageTemplate, httpContext.Request.Method, httpContext.Request.Path, statusCode,
                sw.Elapsed.TotalMilliseconds);
        }
        // Never caught, because `LogException()` returns false.
        catch (Exception ex) when (LogException(httpContext, sw, ex, GetType().Assembly.GetName().Version.ToString()))
        {
        }
    }
    
    private static bool LogException(HttpContext httpContext, Stopwatch sw, Exception ex, string AppVersion)
    {
        sw.Stop();
        
        LogForErrorContext(httpContext, AppVersion)
            .Error(ex, MessageTemplate, httpContext.Request.Method, httpContext.Request.Path, 500,
                sw.Elapsed.TotalMilliseconds);
        
        return false;
    }
    
    private static ILogger LogForErrorContext(HttpContext httpContext, string AppVersion)
    {
        var request = httpContext.Request;
        
        var result = Log
            .ForContext("RequestHeaders", request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()), true)
            .ForContext("RequestHost", request.Host)
            .ForContext("AppVersion", AppVersion)
            .ForContext("RequestProtocol", request.Protocol);
        
        if (request.HasFormContentType)
            result = result.ForContext("RequestForm", request.Form.ToDictionary(v => v.Key, v => v.Value.ToString()));
        
        return result;
    }
}
