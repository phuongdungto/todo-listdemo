using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
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
    options.UseLoggerFactory(LoggerDb.GetLoggerFactory());
});
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);
builder.Services.AddSingleton<ISieveProcessor, SieveProcessor>();
builder.Services.AddControllersWithViews().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);

builder.Services.AddScoped<IUserRepository, UserService>();
builder.Services.AddScoped<IProjectRepository, ProjectService>();
builder.Services.AddScoped<ITasksRepository, TaskService>();

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    // Thiết lập về Password
    options.Password.RequireDigit = false; // Không bắt phải có số
    options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
    options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
    options.Password.RequireUppercase = false; // Không bắt buộc chữ in
    options.Password.RequiredLength = 8; // Số ký tự tối thiểu của password
    options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

    // Cấu hình Lockout - khóa user
    /*options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Khóa 5 phút
    options.Lockout.MaxFailedAccessAttempts = 5; // Thất bại 5 lần thì khóa
    options.Lockout.AllowedForNewUsers = true;*/

    // Cấu hình về User.
    /*options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";*/
    options.User.RequireUniqueEmail = true;  // Email là duy nhất

    // Cấu hình đăng nhập.
    /*options.SignIn.RequireConfirmedEmail = true;            // Cấu hình xác thực địa chỉ email (email phải tồn tại)
    options.SignIn.RequireConfirmedPhoneNumber = false; */    // Xác thực số điện thoại
    /*options.SignIn.RequireConfirmedAccount = true;*/
}).AddEntityFrameworkStores<TodoDbContext>().AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(opt =>
{
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateActor = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        RequireExpirationTime = true,
        ValidateIssuerSigningKey = true,
        ValidAudience = builder.Configuration.GetSection("JWT:ValidAudience").Value,
        ValidIssuer = builder.Configuration.GetSection("JWT:ValidIssuer").Value,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("JWT:Secret").Value)),

        /*ClockSkew = TimeSpan.Zero*/
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
