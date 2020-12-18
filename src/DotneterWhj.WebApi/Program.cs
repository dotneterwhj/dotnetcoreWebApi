using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using DotneterWhj.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DotneterWhj.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var dbContext = (MyDbContext)scope.ServiceProvider.GetRequiredService<DbContext>();

                dbContext.Database.Migrate();

                var testBlog = dbContext.Advertisements.FirstOrDefault(b => b.Url == "http://test.com");
                if (testBlog == null)
                {
                    dbContext.Advertisements.Add(new Models.Advertisement
                    {
                        Id = 1,
                        ImgUrl = "https://tse1-mm.cn.bing.net/th/id/OIP.b2bjF9AlRqohZu-Yef1zlwHaLH?pid=Api&rs=1",
                        Remark = "美女",
                        Title = "美女",
                        Url = "http://test.com"
                    });
                }
                dbContext.SaveChanges();
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
