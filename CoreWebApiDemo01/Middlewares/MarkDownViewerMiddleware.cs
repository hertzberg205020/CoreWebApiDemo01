using System.Net;
using System.Text;
using MarkdownSharp;
using Microsoft.Extensions.Caching.Memory;
using Ude;

namespace CoreWebApiDemo01.Middlewares;

public class MarkDownViewerMiddleware
{
    private readonly RequestDelegate next;
    private readonly IWebHostEnvironment _hostEnv;
    private readonly IMemoryCache _memoryCache;

    public MarkDownViewerMiddleware(RequestDelegate next, IWebHostEnvironment hostEnv, IMemoryCache memoryCache)
    {
        this.next = next;
        _hostEnv = hostEnv;
        _memoryCache = memoryCache;
    }

    /// <summary>
    /// 檢查stream的編碼
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    private static string DetectCharset(Stream stream)
    {
        CharsetDetector detector = new();
        detector.Feed(stream);
        detector.DataEnd();
        string charset = detector.Charset ?? "UTF-8";
        // 檔案讀取指針歸位，
        // 因為CharsetDetector讀取檔案時已經將stream的指針往後移了，
        // 後續需要重新再讀取檔案
        stream.Position = 0;
        return charset;
    }

    /// <summary>
    /// 讀取純文字檔案內容
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    private static async Task<string> ReadText(Stream stream)
    {
        // 檢視檔案編碼類型
        string charset = DetectCharset(stream);
        using (var reader = new StreamReader(stream, Encoding.GetEncoding(charset)))
        {
            return await reader.ReadToEndAsync();
        }
    }


    public async Task InvokeAsync(HttpContext ctx)
    {
        string path = ctx.Request.Path.Value ?? "";
        // 只處理.md檔案請求
        if (!path.EndsWith(".md"))
        {
            await next(ctx);
            return;
        }
        
        // 依據路由找出對應實體路徑下的檔案
        var file = _hostEnv.WebRootFileProvider.GetFileInfo(path);

        if (!file.Exists)  // 檔案是否存在
        {
            await next(ctx);
            return;
        }

        ctx.Response.ContentType = $"text/html;charset=UTF-8";
        ctx.Response.StatusCode = (int)HttpStatusCode.OK;

        string cacheKey = $"{nameof(MarkDownViewerMiddleware)}_{path}_{file.LastModified}";

        var html = await _memoryCache.GetOrCreateAsync(cacheKey, async cacheEntry =>
        {
            cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
            using (var stream = file.CreateReadStream())
            {
                string text = await ReadText(stream);  // 讀取.md檔案
                Markdown md = new();
                return md.Transform(text);  // md -> html
            }
        });

        await ctx.Response.WriteAsync(html);

    }
}