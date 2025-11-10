using JnvKmmAlumniApi.Data;
using JnvKmmAlumniApi.Interfaces;
using JnvKmmAlumniApi.Repositories;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// 🔓 Add open CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<DapperContext>();
builder.Services.AddScoped<MemberRepository>();
builder.Services.AddTransient<IEventsRepository, EventsRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

// 🔐 Apply open CORS policy
app.UseCors("AllowAll");

app.UseAuthorization();

// Serve static files from wwwroot
app.UseStaticFiles();

// Serve ProfileImages from custom folder
// app.UseStaticFiles(new StaticFileOptions
// {
//     FileProvider = new PhysicalFileProvider(
//         Path.Combine(builder.Environment.ContentRootPath, "ProfileImages")),
//     RequestPath = "/ProfileImages"
// });

try
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(
            Path.Combine(builder.Environment.ContentRootPath, "ProfileImages")),
        RequestPath = "/ProfileImages"
    });
}
catch (DirectoryNotFoundException ex)
{
    app.Logger.LogWarning("ProfileImages folder not found: " + ex.Message);
}
app.MapControllers();

app.Run();
