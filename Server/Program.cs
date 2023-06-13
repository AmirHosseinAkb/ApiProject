using System.Drawing.Text;using Common;
using Data;
using Data.Contracts;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region DbContext

builder.Services.AddDbContext<MyApiContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyApiConnection")));

#endregion

#region IOC

builder.Services.AddScoped<IUserRepository, UserRepository>();

#endregion

#region SiteSettingsConfigs
SiteSettings _siteSettings;
_siteSettings = builder.Configuration.GetSection(nameof(SiteSettings)).Get<SiteSettings>();
builder.Services.Configure<SiteSettings>(builder.Configuration.GetSection(nameof(SiteSettings)));
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
