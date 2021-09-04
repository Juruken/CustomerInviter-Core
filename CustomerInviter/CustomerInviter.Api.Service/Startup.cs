using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace CustomerInviter.Api.Service
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson();

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyOrigin();
                    policy.WithHeaders();
                    policy.WithExposedHeaders("Content-Type", "Content-Encoding", "Content-Length", "ETag", "Location");
                });
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseCors();
            app.UseRouting();
            app.UseSerilogRequestLogging();
            app.UseEndpoints(endPoints => endPoints.MapControllers());
        }
    }
}