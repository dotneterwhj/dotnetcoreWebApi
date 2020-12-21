using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extras.DynamicProxy;
using DotneterWhj.Core;
using DotneterWhj.IRepository;
using DotneterWhj.IServices;
using DotneterWhj.Repository;
using DotneterWhj.Services;
using DotneterWhj.WebApi.Aop;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.Filters;

namespace DotneterWhj.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSingleton(new AppSettings(Configuration));

            services.AddSingleton<IMemoryCache, MemoryCache>();

            services.AddTransient<IRedisBasketRepository, RedisBasketRepository>();

            services.AddSingleton<ConnectionMultiplexer>(s =>
            {
                //获取连接字符串
                string redisConfiguration = AppSettings.App(new string[] { "Redis", "ConnectionString" });

                var configuration = ConfigurationOptions.Parse(redisConfiguration, true);

                configuration.ResolveDns = true;

                return ConnectionMultiplexer.Connect(configuration);
            });

            services.AddScoped<IAdvertisementService, AdvertisementService>();
            services.AddScoped<IAdvertisementRepository, AdvertisementRepository>();

            services.AddDbContextPool<DbContext, MyDbContext>(options =>
             {
                 options.UseMySql("Database=DotnetCoreWebApi;Data Source=127.0.0.1;Port=3306;User Id=root;Password=12345678;Charset=utf8;TreatTinyAsBoolean=false;",
                     x => x.MigrationsAssembly("DotneterWhj.Migrations"));
             });

            #region 鉴权认证

            //读取配置文件
            var audienceConfig = Configuration.GetSection("Audience");
            var symmetricKeyAsBase64 = audienceConfig["Secret"];
            var keyByteArray = Encoding.ASCII.GetBytes(symmetricKeyAsBase64);
            var signingKey = new SymmetricSecurityKey(keyByteArray);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingKey,//参数配置在下边
                    ValidateIssuer = true,
                    ValidIssuer = audienceConfig["Issuer"],//发行人
                    ValidateAudience = true,
                    ValidAudience = audienceConfig["Audience"],//订阅人
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,//这个是缓冲过期时间，也就是说，即使我们配置了过期时间，这里也要考虑进去，过期时间+缓冲，默认好像是7分钟，你可以直接设置为0
                    RequireExpirationTime = true,
                };
            });

            #endregion

            #region 授权

            services.AddAuthorization(configure =>
            {
                configure.AddPolicy("Admin", policy => policy.RequireRole("Admin").Build());
                configure.AddPolicy("System", policy => policy.RequireRole("System").Build());
            });

            #endregion

            #region 跨域

            services.AddCors(corsOptions => 
            {
                corsOptions.AddPolicy("AllRequest", policy => 
                {
                    policy.AllowAnyOrigin();
                    policy.AllowAnyHeader();
                    policy.AllowAnyMethod();
                });
            });

            #endregion



            services.AddSwaggerGen(c =>
            {
                var basePath = AppContext.BaseDirectory;
                var xmlPath = Path.Combine(basePath, "DotneterWhj.WebApi.xml");//这个就是刚刚配置的xml文件名
                c.IncludeXmlComments(xmlPath, true);
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "DotneterWhj.WebApi", Version = "v1" });

                c.OperationFilter<AddResponseHeadersFilter>();
                c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();

                c.OperationFilter<SecurityRequirementsOperationFilter>();

                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Description = "输入Bearer jwt",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Name = "Authorization",
                    BearerFormat = "Bearer {bearer_token}",
                    Scheme = "http2.0"
                });

            });
        }


        public void ConfigureContainer(ContainerBuilder builder)
        {
            var basePath = Microsoft.DotNet.PlatformAbstractions.ApplicationEnvironment.ApplicationBasePath;

            // 注册要通过反射创建的组件

            builder.RegisterType<LogAop>();
            builder.RegisterType<MemoryCacheAop>();
            builder.RegisterType<RedisCacheAop>();

            builder.RegisterGeneric(typeof(BaseRepository<>)).As(typeof(IBaseRepository<>));

            //builder.RegisterGeneric(typeof(BaseService<>)).As(typeof(IBaseService<>))
            //    .EnableInterfaceInterceptors()
            //    .InterceptedBy(typeof(LogAop));

            //builder.RegisterType<AdvertisementServices>().As<IAdvertisementServices>();
            //builder.RegisterType<BlogCacheAOP>();//可以直接替换其他拦截器
            //builder.RegisterType<BlogRedisCacheAOP>();//可以直接替换其他拦截器
            //builder.RegisterType<BlogLogAOP>();//这样可以注入第二个

            // ※※★※※ 如果你是第一次下载项目，请先F6编译，然后再F5执行，※※★※※

            #region 带有接口层的服务注入

            try
            {
                #region Service.dll 注入，有对应接口
                // 获取项目绝对路径，请注意，这个是实现类的dll文件，不是接口 IService.dll ，注入容器当然是Activatore
                var servicesDllFile = Path.Combine(basePath, "DotneterWhj.Services.dll");

                var assemblysServices = Assembly.LoadFrom(servicesDllFile);// 直接采用加载文件的方法  ※※★※※ 如果你是第一次下载项目，请先F6编译，然后再F5执行，※※★※※

                builder.RegisterAssemblyTypes(assemblysServices)
                          .AsImplementedInterfaces()
                          .InstancePerLifetimeScope()
                          .EnableInterfaceInterceptors()// 引用Autofac.Extras.DynamicProxy;
                                                        // 如果你想注入两个，就这么写  InterceptedBy(typeof(BlogCacheAOP), typeof(BlogLogAOP));
                                                        // 如果想使用Redis缓存，请必须开启 redis 服务，端口号我的是6319，如果不一样还是无效，否则请使用memory缓存 BlogCacheAOP
                          .InterceptedBy(typeof(LogAop), typeof(RedisCacheAop));//允许将拦截器服务的列表分配给注册。 

                #endregion

                #region Repository.dll 注入，有对应接口

                //var repositoryDllFile = Path.Combine(basePath, "DotneterWhj.Repository.dll");
                //var assemblysRepository = Assembly.LoadFrom(repositoryDllFile);
                //builder.RegisterAssemblyTypes(assemblysRepository).AsImplementedInterfaces();

                #endregion
            }
            catch (Exception ex)
            {
                throw new Exception("※※★※※ 如果你是第一次下载项目，请先对整个解决方案dotnet build（F6编译），然后再对api层 dotnet run（F5执行），\n因为解耦了，如果你是发布的模式，请检查bin文件夹是否存在Repository.dll和service.dll ※※★※※" + ex.Message + "\n" + ex.InnerException);
            }

            #endregion


            #region 没有接口层的服务层注入

            // 因为没有接口层，所以不能实现解耦，只能用 Load 方法。
            // 注意如果使用没有接口的服务，并想对其使用 AOP 拦截，就必须设置为虚方法
            //var assemblysServicesNoInterfaces = Assembly.Load("DotneterWhj.Services");
            //builder.RegisterAssemblyTypes(assemblysServicesNoInterfaces);

            #endregion

            #region 没有接口的单独类 class 注入
            // 只能注入该类中的虚方法
            // builder.RegisterAssemblyTypes(Assembly.GetAssembly(typeof(Love)))
            //    .EnableClassInterceptors()
            //    .InterceptedBy(typeof(LogAop));

            #endregion

            //这里不要再 build 了
            //var ApplicationContainer = builder.Build();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "DotneterWhj.WebApi v1");

                    // 将swagger首页，设置成我们自定义的页面，记得这个字符串的写法：解决方案名.index.html
                    // 这里是配合MiniProfiler进行性能监控的，《文章：完美基于AOP的接口性能分析》，如果你不需要，可以暂时先注释掉，不影响大局。
                    //c.IndexStream = () => GetType().GetTypeInfo().Assembly.GetManifestResourceStream("DotneterWhj.WebApi.index.html");

                });
            }

            app.UseCors("AllRequest");

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
