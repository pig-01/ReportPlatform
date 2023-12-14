using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ReportPlatform.BAL.Factories;
using ReportPlatform.Components;
using ReportPlatform.Helpers;
using Serilog;
using Serilog.Events;
using System.Reflection;
using System.Security.Claims;
using System.Text;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("Starting web application");

    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog(); // Move this line here

    builder.Services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            // 當驗證失敗時，回應標頭會包含 WWW-Authenticate 標頭，這裡會顯示失敗的詳細錯誤原因
            options.IncludeErrorDetails = true; // 預設值為 true，有時會特別關閉

            options.TokenValidationParameters = new TokenValidationParameters
            {
                // 透過這項宣告，就可以從 "sub" 取值並設定給 User.Identity.Name
                NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
                // 透過這項宣告，就可以從 "roles" 取值，並可讓 [Authorize] 判斷角色
                RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",

                // 一般我們都會驗證 Issuer
                ValidateIssuer = true,
                ValidIssuer = builder.Configuration.GetValue<string>("JwtSettings:Issuer"),

                // 通常不太需要驗證 Audience
                ValidateAudience = false,
                //ValidAudience = "JwtAuthDemo", // 不驗證就不需要填寫

                // 一般我們都會驗證 Token 的有效期間
                ValidateLifetime = true,

                // 如果 Token 中包含 key 才需要驗證，一般都只有簽章而已
                ValidateIssuerSigningKey = false,

                // "1234567890123456" 應該從 IConfiguration 取得
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("JwtSettings:SignKey")))
            };
        });

    builder.Services.AddAuthorization();

    // Add services to the container.
    builder.Services.AddRazorComponents()
        .AddInteractiveServerComponents();

    builder.Services.AddLogging();
    builder.Services.AddSerilog();
    builder.Services.AddControllers();
    builder.Services.AddSwaggerGen(option =>
    {
        option.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "報表平台認證_API",
            Version = "v1",
            Description = "報表平台認證API服務提供安全的身份驗證機制，以確保只有授權用戶能夠訪問敏感數據和功能。使用JWT（JSON Web Token）的Bearer方案，用戶需要在標頭中傳遞包含其授權令牌的 Authorization 標頭，格式為 \"Bearer [空格] 令牌\"。這有效確保了API請求的合法性，保障了報表平台的安全性，同時為開發者提供了方便的身份認證機制。",
            Contact = new OpenApiContact
            {
                Name = "報表平台",
                Email = ""
            }
        });
        option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
        {
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "JWT（JSON Web Token）使用 Bearer 方式的 Authorization Header。\r\n在下面的文字輸入框中輸入 'Bearer' [空格] 然後是您的 Token。\r\n範例：\"Bearer af249d7a2e1f\"",
        });
        option.AddSecurityRequirement(new OpenApiSecurityRequirement()
        {
            {
                new OpenApiSecurityScheme {
                    Reference = new OpenApiReference {
                        Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });

        /// 加入xml檔案到swagger
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        option.IncludeXmlComments(xmlPath);
    });

    builder.Services.AddAntiforgery(o => o.HeaderName = "XSRF-TOKEN");

    builder.Services.AddSingleton<JwtHelpers>();
    builder.Services.AddSingleton<ClaimsPrincipal>();
    builder.Services.AddScoped<IReportFactory, ReportFactory>();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger(options =>
        {
            options.SerializeAsV2 = true;
        });
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            options.RoutePrefix = string.Empty;
        });
    }
    else
    {
        app.UseExceptionHandler("/Error", createScopeForErrors: true);
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();
    app.UseStaticFiles();
    app.UseAntiforgery();

    app.MapControllers();
    app.MapRazorComponents<App>()
        .AddInteractiveServerRenderMode();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}


