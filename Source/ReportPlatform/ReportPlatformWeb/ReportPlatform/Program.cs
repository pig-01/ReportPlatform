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
            // �����ҥ��ѮɡA�^�����Y�|�]�t WWW-Authenticate ���Y�A�o�̷|��ܥ��Ѫ��Բӿ��~��]
            options.IncludeErrorDetails = true; // �w�]�Ȭ� true�A���ɷ|�S�O����

            options.TokenValidationParameters = new TokenValidationParameters
            {
                // �z�L�o���ŧi�A�N�i�H�q "sub" ���Ȩó]�w�� User.Identity.Name
                NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
                // �z�L�o���ŧi�A�N�i�H�q "roles" ���ȡA�åi�� [Authorize] �P�_����
                RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",

                // �@��ڭ̳��|���� Issuer
                ValidateIssuer = true,
                ValidIssuer = builder.Configuration.GetValue<string>("JwtSettings:Issuer"),

                // �q�`���ӻݭn���� Audience
                ValidateAudience = false,
                //ValidAudience = "JwtAuthDemo", // �����ҴN���ݭn��g

                // �@��ڭ̳��|���� Token �����Ĵ���
                ValidateLifetime = true,

                // �p�G Token ���]�t key �~�ݭn���ҡA�@�볣�u��ñ���Ӥw
                ValidateIssuerSigningKey = false,

                // "1234567890123456" ���ӱq IConfiguration ���o
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
            Title = "�����x�{��_API",
            Version = "v1",
            Description = "�����x�{��API�A�ȴ��Ѧw�����������Ҿ���A�H�T�O�u�����v�Τ����X�ݱӷP�ƾکM�\��C�ϥ�JWT�]JSON Web Token�^��Bearer��סA�Τ�ݭn�b���Y���ǻ��]�t����v�O�P�� Authorization ���Y�A�榡�� \"Bearer [�Ů�] �O�P\"�C�o���ĽT�O�FAPI�ШD���X�k�ʡA�O�٤F�����x���w���ʡA�P�ɬ��}�o�̴��ѤF��K�������{�Ҿ���C",
            Contact = new OpenApiContact
            {
                Name = "�����x",
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
            Description = "JWT�]JSON Web Token�^�ϥ� Bearer �覡�� Authorization Header�C\r\n�b�U������r��J�ؤ���J 'Bearer' [�Ů�] �M��O�z�� Token�C\r\n�d�ҡG\"Bearer af249d7a2e1f\"",
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

        /// �[�Jxml�ɮר�swagger
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


