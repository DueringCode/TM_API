using Microsoft.EntityFrameworkCore;
using TMAPI_Backend.Configuration;
using TMAPI_Backend.Data;
using TMAPI_Backend.Services;

namespace TMAPI_Backend
{
    public class Program
    {
        static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddScoped<UserService>();
            builder.Services.AddScoped<TaskService>();
           
            builder.Services.AddScoped<JwtService>();
            builder.Services.AddJwtAuthentication(builder.Configuration);

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
