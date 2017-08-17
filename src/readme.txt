部署
http://www.cnblogs.com/ants/p/5732337.html

组件
NuGet

//by http://www.cnblogs.com/savorboard/p/aspnetcore-response-compression-caching.html
//这个中间件是 .NET Core 1.1 版本中新增加的，看名字应该知道，它主要是负责对输出的内容进行压缩， 那么在我们WEB开发中主要就是 GZip 压缩了。
//Gzip 压缩是我们在 WEB 中经常会使用的一项性能优化技术，它可以对页面输出的内容使用压缩算法（GZip）进行体积的压缩， 那在以前的时候，我们可以使用 IIS 来做这项工作，但是现在我们的程序脱离 IIS了，就必须有一个中间件来帮我们做这件事情了，它就是我们要介绍的这个中间件。
Install-Package Microsoft.AspNetCore.ResponseCompression

//这个中间件也是 .NET Core 1.1 版本中新增加的，同样看名字应该知道，它主要是负责对输出的内容进行缓存设置。在以前我们可以同样在 IIS 中设置这些东西，但是粒度可能并没有这么细。
Install-Package Microsoft.AspNetCore.ResponseCaching

//Logging
Install-Package Microsoft.Extensions.Logging.Console -Pre

//还原依赖
dotnet restore

//编译
dotnet build

//运行
dotnet run

dotnet publish 



//EF使用
//Install-Package Pomelo.EntityFrameworkCore.MySql
//Install-Package MySql.Data.EntityFrameworkCore -Pre
//Install-Package Microsoft.EntityFrameworkCore.Sqlite –Pre
1.创建实体
public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
2.DataContext
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
    }
3.创建数据库
打开 Startup.cs  在 ConfigureServices 下添加如下代码：
        public void ConfigureServices(IServiceCollection services)
        {
            var connection = "Filename=./efcoredemo.db";
            services.AddDbContext<DataContext>(options => options.UseSqlite(connection));
            // Add framework services.
            services.AddMvc();
        }


//Microsoft.EntityFrameworkCore.Tools
//Install-Package Microsoft.EntityFrameworkCore.Tools –Pre
//如果在升级到Entity Framework Core 1.1之后再次出现此问题，请务必Microsoft.EntityFrameworkCore.Tools使用Microsoft.EntityFrameworkCore.Tools.DotNet版本替换依赖关系1.1.0-preview4
如果您使用VS2017与没有project.json文件的新的.csproj项目，请添加此项
您需要编辑.csproj文件（在解决方案资源管理器中右键单击，然后单击编辑whatever.csproj），然后将其粘贴到
<ItemGroup>
    <DotNetCliToolReference Include="Microsoft.EntityFrameworkCore.Tools.DotNet">
      <Version>1.0.0-*</Version>
    </DotNetCliToolReference>
  </ItemGroup>
//创建数据库
dotnet ef migrations add initialCreate

//更新数据库
dotnet ef database update


//dotnet ef dbcontext scaffold "server=localhost;user id=root;password=root;database=hanjumed;port=3306;sslmode=none" "Pomelo.EntityFrameworkCore.Mysql" -o Models
//dotnet ef migrations remove