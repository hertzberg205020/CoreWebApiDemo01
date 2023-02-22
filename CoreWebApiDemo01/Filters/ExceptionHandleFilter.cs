using Lombok.NET;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CoreWebApiDemo01.Filters;

[AllArgsConstructor]
public partial class ExceptionHandleFilter: IAsyncExceptionFilter
{
    private readonly IWebHostEnvironment _hostEnvironment;
    
    /// <summary>
    /// 當Controller中的Action發生未處理異常
    /// </summary>
    /// <param name="context"></param>
    /// <exception cref="NotImplementedException"></exception>
    public Task OnExceptionAsync(ExceptionContext context)
    {
        // 1. context.Exception: 表示異常訊息的物件
        // 2. 若給context.ExceptionHandled賦值為true，
        //    其他的ExceptionFilter不會再執行
        // 3. context.Result的值會被輸出給客戶端

        string msg;
        msg = _hostEnvironment.IsDevelopment() ? context.Exception.Message.ToString() : "伺服器發生未處理異常";

        // public class ObjectResult : ActionResult
        ObjectResult ret = new ObjectResult(new { code = 500, message = msg });
        context.Result = ret;
        // 異常已經處理完成，後續不需要其他的ExceptionFilter協助
        context.ExceptionHandled = true;
        
        // 方法內沒有await的code，
        // 故方法名稱不需要加async
        // 返回一個Task型別的物件
        return Task.CompletedTask;
    }
}