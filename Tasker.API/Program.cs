using Microsoft.EntityFrameworkCore;

using Tasker.DAL.Data;
using Tasker.DAL.Interfaces;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<TaskerDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();