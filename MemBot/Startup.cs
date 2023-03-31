using MemBot.Command;
using MemBot.Context;
using MemBot.Repository;
using MemBot.Service;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;

namespace MemBot
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private readonly IConfiguration _configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(
                _configuration.GetConnectionString("Db")), ServiceLifetime.Singleton);

            services.AddSingleton<Bot>();
            services.AddSingleton<IHandleUpdateService, HandleUpdateService>();
            services.AddSingleton<IUserRepo, UserRepo>();
            services.AddSingleton<ICommand, StartCommand>();
            
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            serviceProvider.GetRequiredService<Bot>().GetClient().Wait();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}