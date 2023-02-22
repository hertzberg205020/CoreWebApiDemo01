using Microsoft.AspNetCore.Mvc.Filters;

namespace CoreWebApiDemo01.Filters;

public class MyActionFilter1: IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        Console.WriteLine("MyActionFilter前半段邏輯");
        
        // next是一個委託，
        // next指向下一個filter 或者 當沒有下一個filter時，
        // 指向實際要執行的Action()
        ActionExecutedContext res = await next();
        
        if (res.Exception != null)
        {
            Console.WriteLine("MyActionFilter1: 發生異常");
        }
        else
        {
            Console.WriteLine("MyActionFilter1: 成功執行");
        }
    }
}