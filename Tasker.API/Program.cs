using System.IdentityModel.Tokens.Jwt;
using System.Text;

using FluentValidation;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using Tasker.API.Endpoints;
using Tasker.API.Helpers.Interceptors;
using Tasker.API.Middlware;
using Tasker.BLL.Interfaces;
using Tasker.BLL.Models.User;
using Tasker.BLL.Services;
using Tasker.BLL.Validators.ToDoTask;
using Tasker.DAL.Data;
using Tasker.DAL.Entities;
using Tasker.DAL.Interfaces;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
builder.Services.AddDbContext<TaskerDbContext>(
        (sp, options) =>
        {
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            if (!builder.Environment.IsDevelopment())
            {
                var password = Environment.GetEnvironmentVariable("MSSQL_SA_PASSWORD");
                connectionString = string.Format(connectionString, password);
            }

            options.UseSqlServer(connectionString)
                .AddInterceptors(sp.GetRequiredService<UptadeDatedEntityInterceptor>());
        });

builder.Services.AddIdentity<TaskerUser, IdentityRole<Guid>>(options => options.User.RequireUniqueEmail = true)
    .AddEntityFrameworkStores<TaskerDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    //HACK: This is a workaround for the issue with the JwtBearerDefaults.AuthenticationScheme
    // Retrieve JwtSettings from configuration
    var jwtSettings = builder.Services.BuildServiceProvider().GetRequiredService<IOptions<JwtSettings>>().Value;

    var key = Encoding.ASCII.GetBytes(jwtSettings.Secret);

    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = jwtSettings.ValidIssuer,
        ValidAudience = jwtSettings.ValidAudience,
        IssuerSigningKey = new SymmetricSecurityKey(key),
    };
});

// Add Authorization services
builder.Services.AddAuthorization();

builder.Services.AddControllers();

// Register the Swagger generator, defining 1 or more Swagger documents
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<UptadeDatedEntityInterceptor>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IToDoTaskService, ToDoTaskService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddTransient<GlobalErrorHandlingMiddleware>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateToDoTaskValidator>();

var app = builder.Build();

// Resister global error handling middleware
app.UseMiddleware<GlobalErrorHandlingMiddleware>();

app.UseAuthentication();

app.UseAuthorization();

app.MapTaskEndpoints();
app.MapAuthenticationEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TaskerDbContext>();
    db.Database.Migrate();
}

app.Run();