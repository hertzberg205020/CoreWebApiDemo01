using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreBooks
{
    internal class MyDbContextDesignFactory : IDesignTimeDbContextFactory<MyDbContext>
    {
        /// <summary>
        /// 開發時(Add-Migration, Update-Database)運行
        /// 正式上線不會與此類相關
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public MyDbContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<MyDbContext> builder = new DbContextOptionsBuilder<MyDbContext>();
            //string conn = @"Data Source=.;Initial Catalog=demo2;User ID=sa;Password=P@ssw0rd;TrustServerCertificate=true;MultipleActiveResultSets=true;Integrated Security=SSPI";
            //string conn = Environment.GetEnvironmentVariable("base");

            // 將Connection string 設置在secrets.json
            IConfiguration config = new ConfigurationBuilder()
                                       // SomeClass can be any class in the assembly
                                       // .AddUserSecrets<SomeClass>()
                                       .AddUserSecrets<Book>()
                                       .Build();

            var conn = config.GetConnectionString("base");

            builder.UseSqlServer(conn);
            return new MyDbContext(builder.Options);
        }
    }
}
