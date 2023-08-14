using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using NLog;
using NLog.Web;

using Sieve.Services;

using System.Text;

using todo.Data;
using todo.Helpers;
using todo.Middlewares;
using todo.Models;
using todo.Repositories;
using todo.Services;

var logger = LogManager.Setup()
    .LoadConfigurationFromAppSettings()
    .GetCurrentClassLogger();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

ILoggerFactory GetLoggerFactory()
{
    IServiceCollection serviceCollection = new ServiceCollection();
    serviceCollection.AddLogging(builder =>
            builder.AddConsole()
                    .AddFilter(DbLoggerCategory.Database.Command.Name,
                            Microsoft.Extensions.Logging.LogLevel.Information));
    return serviceCollection.BuildServiceProvider()
            .GetService<ILoggerFactory>();
}
builder.Logging.ClearProviders();
builder.Host.UseNLog();
builder.Services.AddTransient<ErrorHandlingMiddleware>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<TodoDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyDB"));
    options.UseLoggerFactory(GetLoggerFactory());
});
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);
builder.Services.AddSingleton<ISieveProcessor, SieveProcessor>();
/*builder.Services.Configure<SieveOptions>(builder.Configuration.GetSection("Sieve"));*/
builder.Services.AddControllersWithViews().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);

builder.Services.AddScoped<IUserRepository, UserService>();
builder.Services.AddScoped<IProjectRepository, ProjectService>();
builder.Services.AddScoped<ITasksRepository, TaskService>();

builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

var secretKey = builder.Configuration["AppSettings:SecretKey"];
var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
{
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,

        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),

        ClockSkew = TimeSpan.Zero
    };
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.CongfigureExceptionMiddleware();

app.MapControllers();

app.Run();
