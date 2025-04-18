using System.Text;
using System.Threading.RateLimiting;
using ApplicationCore.Entities;
using Infrastructure.Data;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client.Utils;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Filters;
using Serilog.Templates;
using Serilog.Templates.Themes;
using WebApi;

var builder = WebApplication.CreateBuilder(args);
var isProd = builder.Environment.IsProduction();

//Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console(new ExpressionTemplate(
        // Include trace and span ids when present.
        "[{@t:HH:mm:ss} {@l:u3}{#if @tr is not null} ({substring(@tr,0,4)}:{substring(@sp,0,4)}){#end}] {@m}\n{@x}",
        theme: TemplateTheme.Literate))
    .Filter.ByExcluding(Matching.FromSource("Microsoft.AspNetCore.Cors.Infrastructure.CorsService"))
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

try
{
    ConfigureServices(builder.Services);

    var app = builder.Build();

    ConfigureMiddleware(app);

    app.Run();
}
catch (Exception e)
{
    Log.Fatal(e, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

return;

void ConfigureServices(IServiceCollection s)
{
    s.AddDbContext<AppDbContext>(options =>
    {
        //if(!builder.Configuration["ConnectionString"].IsNullOrEmpty()) 
        if (isProd) options.UseNpgsql(builder.Configuration["ConnectionString"]);
        else options.UseInMemoryDatabase("AppВb");
        //options.UseInMemoryDatabase("AppВb");
    });

    s.AddIdentity<User, IdentityRole<Guid>>(options =>
        {
            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.User.RequireUniqueEmail = false;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

    var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
    s.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings?.Issuer,
                ValidAudience = jwtSettings?.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings?.Secret ?? string.Empty)),
                ClockSkew = TimeSpan.Zero
            };
        });
    s.AddAuthorization();

    s.AddHealthChecks();

    s.AddOpenApi();
    s.AddEndpointsApiExplorer();
    s.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { 
            Title = "SQL Learning API", 
            Version = "v1",
            Description = "API for SQL Learning platform with different exercise types",
            Contact = new OpenApiContact
            {
                Name = "SQL Learning Team"
            }
        });

        // Include XML comments
        var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        c.IncludeXmlComments(xmlPath);

        // Security configuration
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "JWT Authorization header using the Bearer scheme."
        });
        
        if(isProd) c.AddServer(new OpenApiServer { Url = "/api/v1/" });
        else c.AddServer(new OpenApiServer { Url = "/" });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    });


    // Rate Limiting
    s.AddRateLimiter(options =>
    {
        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

        options.AddPolicy("ApiPolicy", context =>
            RateLimitPartition.GetSlidingWindowLimiter(
                context.User.Identity?.Name ?? context.Connection.RemoteIpAddress?.ToString(),
                _ => new SlidingWindowRateLimiterOptions
                {
                    PermitLimit = 100,
                    Window = TimeSpan.FromMinutes(1),
                    SegmentsPerWindow = 2
                }));
    });

    // CORS Configuration
    s.AddCors(options =>
    {
        options.AddPolicy("AppCorsPolicy", policy =>
        {
            var allowedOrigins = builder.Configuration["AllowedOrigins"]?.Split(';') ?? [];
            policy.WithOrigins(allowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
    });

    //s.AddApplicationCore();

    s.AddInfrastructure(builder.Configuration);
    s.AddControllers();
}

void ConfigureMiddleware(WebApplication app)
{
    app.UseSerilogRequestLogging();

    if(isProd) app.UsePathBase("/api/v1/");
    app.UseRouting();

    app.UseCors("AppCorsPolicy");

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseRateLimiter();
    app.MapControllers();


    app.MapHealthChecks("/health");


    app.UseSwagger(c =>
    {
        c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
        {
            // Add server URL with /api prefix for production
            swaggerDoc.Servers = new List<OpenApiServer>
            {
                !isProd? 
                    new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}/" } : // dev
                    new OpenApiServer { Url = "https://rpi.tail707b9c.ts.net/api/v1/" } // prod
            };
        });
    });

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("v1/swagger.json", "API v1");
        c.RoutePrefix = "swagger";
    });

    app.MapDefaultControllerRoute();

    app.MapGet("/", (HttpContext c) => c.Response.Redirect("swagger/index.html"));
}