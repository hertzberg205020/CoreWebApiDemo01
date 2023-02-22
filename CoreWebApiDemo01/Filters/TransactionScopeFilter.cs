using System.Transactions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CoreWebApiDemo01.Filters;

public class TransactionScopeFilter: IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        // context.ActionDescriptor: 是當前要被執行Action()方法的描述訊息
        // context.ActionArguments: 是當前要被執行Action()方法參數述訊息

        // controllerActionDescriptor == null  // 不是一個Controller的Action
        
        bool IsTx = false;  // 是否進行交易控制

        if (context.ActionDescriptor is ControllerActionDescriptor)
        {
            ControllerActionDescriptor controllerActionDescriptor =
                (ControllerActionDescriptor)context.ActionDescriptor;
            // 有標記 [NotTransaction]特性的Action不啟用交易
            IsTx = !controllerActionDescriptor.MethodInfo.IsDefined(typeof(NotTransactionAttribute), true);
        }
        
        if (!IsTx)
        {
            await next();
            return;
        }

        using (var txScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            var res = await next();
            if (res.Exception == null)
            {
                txScope.Complete();
            }
        }
    }
}