using AutoMapper;
using ContactlessEntry.Cloud.Configuration;
using ContactlessEntry.Cloud.Models;
using ContactlessEntry.Cloud.Services;
using ContactlessEntry.Cloud.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ContactlessEntry.Cloud
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
            services.AddAutoMapper(typeof(AutoMapperProfile));

            services.Configure<MicroserviceSettings>(Configuration.GetSection(nameof(MicroserviceSettings)));
            services.AddSingleton<IMicroserviceSettings>(serviceProvider => serviceProvider.GetRequiredService<IOptions<MicroserviceSettings>>().Value);

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = true,
                        ValidateIssuer = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        ValidAudience = Configuration["MicroserviceSettings:JwtIssuer"],
                        ValidIssuer = Configuration["MicroserviceSettings:JwtIssuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["MicroserviceSettings:JwtKey"]))
                    };
                });

            services.AddSingleton<IFaceClient>(new FaceClient(new ApiKeyServiceClientCredentials(Configuration["MicroserviceSettings:FaceSubscriptionKey"]), new DelegatingHandler[] { })
            {
                Endpoint = Configuration["MicroserviceSettings:FaceApiUrl"]
            });

            var mockRepository = new Mock<IAccessRepository>();
            mockRepository
                .Setup(ar => ar.CreateAccessAsync(It.IsNotNull<Access>()))
                .Returns((Access input) => Task.FromResult(input));

            services.AddTransient((serviceProvider) => { return new Mock<IOpenDoorService>().Object; });
            services.AddTransient((serviceProvider) => { return mockRepository.Object; });
            services.AddTransient<IFaceClientService, FaceClientService>();
            services.AddTransient<IAccessManager, AccessManager>();

            services.AddControllers(options =>
            {
                options.InputFormatters.Insert(0, new StreamInputFormatter());
            });

            services.AddHealthChecks();

            services.AddMicroserviceSwagger();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
            }

            app.UseHttpsRedirection();

            app.UseMicroserviceSwagger();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });
        }
    }
}
