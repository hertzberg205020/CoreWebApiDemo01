using CoreWebApiDemo01.Entity;
using EFCoreBooks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System.Data.SqlClient;
using CoreWebApiDemo01.Filters;
using Microsoft.AspNetCore.Mvc;

namespace CoreWebApiDemo01
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            # region 註冊服務到IOC容器內
            
            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            
            # region 啟用記憶體快取
            
            builder.Services.AddMemoryCache();
            
            # endregion
            
            # region 註冊過濾器
            
            builder.Services.Configure<MvcOptions>(opt =>
            {
                // 過濾器需要考慮註冊順序
                opt.Filters.Add<RateLimitActionFilter>();
                opt.Filters.Add<ExceptionHandleFilter>();
                opt.Filters.Add<MyActionFilter1>();
            });
            
            # endregion
            
            # region 註冊資料庫上下文物件
            
            builder.Services.AddDbContext<MyDbContext>(opt =>
            {
                var conn = builder.Configuration.GetSection("ConnectionStrings:base").Value;
                opt.UseSqlServer(conn);
            });
            
            # endregion

            # region 加入資料庫作為設定源
            
            var webBuilder = builder.WebHost;
            
            webBuilder.ConfigureAppConfiguration((hostCtx, configBuilder) => {
                var configRoot = builder.Configuration;
                string connStr = configRoot.GetConnectionString("base");
                // 以數據庫作為 設定源(setting source)
                configBuilder.AddDbConfiguration(() => 
                new SqlConnection(connStr),
                    reloadOnChange: true,
                    reloadInterval: TimeSpan.FromSeconds(2));
            });
            
            # endregion
            
            # region 註冊設定
            
            // Redis配置
            builder.Services.AddSingleton<IConnectionMultiplexer>(provider =>
            {
                // 在Program.cs中讀取設定的一種方法
                string redisConnStr = builder.Configuration.GetSection("Redis").Value;
                return ConnectionMultiplexer.Connect(redisConnStr);
            });
            
            builder.Services.Configure<Smtp>(builder.Configuration.GetSection("smtp"));
            
            # endregion

            # region CORS
            
            string[] urls = new[] { "http://localhost:5173" };
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder => 
                builder.WithOrigins(urls)
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .AllowCredentials());

            });
            
            # endregion
            
            var app = builder.Build();
            
            # endregion

            # region 註冊middleware
            
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();
            
            # endregion
            
            
            
            app.Run();
        }
    }
}