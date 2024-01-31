using System.Text.Encodings.Web;
using System.Text.Unicode;

using Ater.Web.Abstraction;

using Http.API;
using Http.API.Worker;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

IServiceCollection services = builder.Services;
ConfigurationManager configuration = builder.Configuration;

//AppDomain.CurrentDomain.FirstChanceException += (sender, eventArgs) =>
//{
//    if (eventArgs.Exception is OutOfMemoryException)
//    {
//        Console.WriteLine($"=== OutOfMemory: {eventArgs.Exception.Message}, {eventArgs.Exception.StackTrace}");
//    }
//    else
//    {
//        Console.WriteLine($"Caught exception: {eventArgs.Exception.Message}");
//    }
//};

// 1 添加默认组件
builder.AddDefaultComponents();
// 2 配置
services.ConfigWebComponents(configuration);

services.AddHttpContextAccessor();
services.AddTransient<IUserContext, UserContext>();
services.AddTransient<ITenantProvider, TenantProvider>();
// 3 数据及业务接口注入
services.AddManager();
// 其他模块Manager
//services.AddSystemModManagers();

// 4 其他自定义选项及服务
services.AddSingleton(typeof(CacheService));
services.AddSingleton<IEmailService, EmailService>();

services.AddControllers()
    .ConfigureApiBehaviorOptions(o =>
    {
        o.InvalidModelStateResponseFactory = context =>
        {
            return new CustomBadRequest(context, null);
        };
    }).AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
    });

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseCors("default");
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/client/swagger.json", name: "client");
        c.SwaggerEndpoint("/swagger/admin/swagger.json", "admin");
    });
}
else
{
    app.UseCors("default");
    //app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseStaticFiles();
// 异常统一处理
app.UseExceptionHandler(ExceptionHandler.Handler());
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapFallbackToFile("index.html");

using (app)
{
    // 初始化工作
    await using (AsyncServiceScope scope = app.Services.CreateAsyncScope())
    {
        IServiceProvider provider = scope.ServiceProvider;
        await InitDataTask.InitDataAsync(provider);
    }
    app.Run();
}

public partial class Program { }