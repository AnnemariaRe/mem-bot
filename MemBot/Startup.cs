using MemBot.Action;
using MemBot.Service;

namespace MemBot;

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

        services.AddSingleton<Bot>();
        services.AddSingleton<IHandleUpdateService, HandleUpdateService>();
        services.AddSingleton<IAction, MessageAction>();
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