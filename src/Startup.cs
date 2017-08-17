using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Text;
using W.WebApi.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using W.WebApi.Helper;

namespace W.WebApi
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            var connStr = Configuration.GetSection("Data:DefaultConnection:ConnectionString").Value;
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Add response compression services.
            services.AddResponseCompression();

            // Add framework services.
            //services.AddDbContext<ApiContext>(options =>
            //    options.UseMySql(Configuration.GetConnectionString("DefaultConnection")));
            var sqlConnectionString = Configuration.GetSection("Data:DefaultConnection:ConnectionString").Value;
            services.AddDbContext<ApiContext>(options =>
                options.UseMySql(
                    sqlConnectionString
                )
            );


            // Add framework services.
            services.AddMvc();
            
            services.AddMvc().AddJsonOptions(options =>
            {
                //Format response date to json
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                //Format response json 结构区分大小写，保持原有结构
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            //使用中间件
            app.UseRequestToken();

            //app.Use((context, next) =>
            //{
            //    var path = context.Request.Path.Value;
            //    var cultureQuery = context.Request.Query["culture"];

            //    // Call the next delegate/middleware in the pipeline
            //    if (path.IndexOf("/api/") >= 0)
            //    {
            //        return next();
            //    }
            //    return new RequestDelegate();
            //});

            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello, World!");
            //});

            //Adds middleware for dynamically compressing HTTP Responses.
            app.UseResponseCompression();

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddConsole(minLevel: LogLevel.Information);
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

        }
    }


    #region token
    public class RequestTokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public RequestTokenMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<RequestTokenMiddleware>();
        }

        //public async Task Invoke(HttpContext context)
        public Task Invoke(HttpContext context)
        {
            _logger.LogInformation("User IP: " + context.Connection.RemoteIpAddress.ToString());
            var path = context.Request.Path.Value;
            if (path.ToLower().IndexOf("/api/") >= 0)
            {
                var request = context.Request;
                Result result = Token.check(request);
                if (result.success == false)
                {
                    return failResponseMassageTask(context, result);
                }

                if (context.Request.Method != "GET")
                {
                    try
                    {
                        using (var bodyReader = new System.IO.StreamReader(request.Body))
                        {
                            string body = bodyReader.ReadToEnd();
                            //todo:客户端请求数据若加密，此处需解密body
                            context.Request.Body = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(body));
                        }
                    }
                    catch (Exception)
                    {
                        return failResponseMassageTask(context, Result.FAILURE(("解析参数错误:BODY")));
                    }
                }
            }
            return _next.Invoke(context);
        }

        private Task failResponseMassageTask(HttpContext context, Result result)
        {
            _logger.LogWarning("token failure");
            //Install-Package System.Text.Encoding.CodePages
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            context.Response.ContentType = "application/json; charset=utf-8";
            return context.Response.WriteAsync(result.toString(), Encoding.UTF8);
        }
    }

    public static class RequestTokenExtensions
    {
        /// <summary>
        /// 校验访问者身份
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseRequestToken(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestTokenMiddleware>();
        }
    }
    #endregion
}
