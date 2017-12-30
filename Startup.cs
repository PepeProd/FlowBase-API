﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlowBaseAPI.DataLayer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FlowBaseAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //TODO: Implement sql server connection
            //services.AddDbContext<FlowbaseContext>(opt => opt.UseInMemoryDatabase("Chemicals"));
            services.AddMvc();

            var connection = @"Server=(localdb)\mssqllocaldb;Database=flowbasedb;Trusted_Connection=True;ConnectRetryCount=0";
            services.AddDbContext<FlowbaseContext>(options => options.UseSqlServer(connection));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

            app.UseMvc();
        }
    }
}
