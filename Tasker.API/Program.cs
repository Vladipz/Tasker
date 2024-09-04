using FluentValidation;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Tasker.API.Endpoints;
using Tasker.API.Helpers.Interceptors;
using Tasker.BLL.Interfaces;
using Tasker.BLL.Services;
using Tasker.BLL.Validators.ToDoTask;
using Tasker.DAL.Data;
using Tasker.DAL.Entities;
using Tasker.DAL.Interfaces;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<TaskerDbContext>(
        (sp, options) => options
        .UseSqlServer(connectionString)
        .AddInterceptors(
            sp.GetRequiredService<UptadeDatedEntityInterceptor>()));

builder.Services.AddIdentity<TaskerUser, IdentityRole<Guid>>(options => options.User.RequireUniqueEmail = true)
    .AddEntityFrameworkStores<TaskerDbContext>()
    .AddDefaultTokenProviders();

// Register the Swagger generator, defining 1 or more Swagger documents
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<UptadeDatedEntityInterceptor>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IToDoTaskService, ToDoTaskService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddValidatorsFromAssemblyContaining<CreateToDoTaskValidator>();

var app = builder.Build();

app.MapTaskEndpoints();
app.MapAuthenticationEndpoints();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "Hello World!");

app.Run();