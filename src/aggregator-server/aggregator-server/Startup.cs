using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace aggregator_server
{
    public class Startup
    {
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        readonly string DatabaseFilePath = "Database/database.bin";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                  builder =>
                                  {
                                      builder.AllowAnyOrigin()
                                             .AllowAnyMethod()
                                             .AllowAnyHeader();
                                  });
            });

            LiteDBFunctions.DoLiteDBGlobalSetUp();

            //var databaseStream = new MemoryStream();

            var databaseDirectory = Path.GetDirectoryName(DatabaseFilePath);
            Directory.CreateDirectory(databaseDirectory);   // TODO: error handling

            var database = new LiteDB.LiteDatabase($"Filename={DatabaseFilePath}");
            var pollConfigurationRepository = new LiteDBPollConfigurationRepository(database);
            var documentRepository = new LiteDBDocumentRepository(database);

            var poller = new Poller(5000, pollConfigurationRepository, documentRepository);    // TODO: fix parameters
            var pollerTask = poller.DoPollingLoop();

            services.AddSingleton(typeof(IPollConfigurationRepository), pollConfigurationRepository);
            services.AddSingleton(typeof(IDocumentRepository), documentRepository);

            services.AddControllers();
            
            services.AddMvc().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.MaxDepth = 64;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(MyAllowSpecificOrigins);

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
