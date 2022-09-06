using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PSW.ITT.Data;
using PSW.ITT.Data.Sql.UnitOfWork;
using PSW.ITT.Service.Services;
using PSW.Lib.Consul;
using PSW.RabbitMq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using PSW.ITT.Service.AutoMapper;
using PSW.Common.Crypto;
using PSW.Lib.Logs;
using PSW.ITT.Service.Strategies;
using PSW.ITT.Service.IServices;

namespace PSW.ITT.Api
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
            string salt = Environment.GetEnvironmentVariable("ENCRYPTION_SALT");
            string password = Environment.GetEnvironmentVariable("ENCRYPTION_PASSWORD");

            if (string.IsNullOrWhiteSpace(salt) || string.IsNullOrWhiteSpace(password))
            {
                throw new Exception("Please provide salt and password for Crypto Algorithm in Environment Variable");
            }

            services.AddSingleton<IAppSettingsProcessor>(_ => new AppSettingsDecrypter<AesManaged>(_.GetService<IConfiguration>(),
                password,
                salt));
            services.AddScoped<ICryptoAlgorithm>(x =>
            {
                return new CryptoFactory().Create<AesManaged>(password, salt);
            });
            //  var cryptoAlgorithm = new CryptoFactory().Create<AesManaged>(password, salt);
            // services.AddScoped<ICryptoAlgorithm>(cryptoAlgorithm);
            
            services.AddTransient<IITTService, ITTService>();
            services.AddTransient<IStrategyFactory, StrategyFactory>();
            services.AddTransient<IITTOpenService, ITTOpenService>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();

            // Auto Mapper Profiles 
            services.AddAutoMapper(
                typeof(DTOToEntityMappingProfile),
                typeof(EntityToDTOMappingProfile)
            );

           
            services.AddHostedService<ITTWorkerQueueService>();
            services.AddControllers()
                    .AddJsonOptions(options =>
                    {
                        options.JsonSerializerOptions.IgnoreNullValues = true;
                    });

            services.AddSingleton<IEventBus, RabbitMqBus>(s =>
            {
                var lifetime = s.GetRequiredService<IHostApplicationLifetime>();
                return new RabbitMqBus(lifetime, Configuration);
            });

            //--- This Section is for Securing API (via IdentityServer) ---------------------------------------------
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = Environment.GetEnvironmentVariable("ASPNETCORE_IDENTITY_SERVER_ISSUER");
                    options.ApiName = "auth";
                    options.ApiSecret = "auth";
                    options.RequireHttpsMetadata = false;
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            Console.WriteLine("OnAuthenticationFailed: " +
                                              context.Exception.Message);
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = context =>
                        {
                            Console.WriteLine("OnTokenValidated: " +
                                              context.SecurityToken);
                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("authorizedUserPolicy", policyAdmin =>
                {
                    policyAdmin.RequireClaim("client_id", "psw.client.spa");
                });
            });

            services.AddCors(options =>
                {
                    options.AddPolicy("CorsPolicy",
                        builder => builder.SetIsOriginAllowed(_ => true)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
                });

            services.AddConsul(Configuration);
            services.AddHealthChecks();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IAppSettingsProcessor settingsDecrypter, IITTService ITTService, IUnitOfWork unitOfWork, IEventBus eventBus, IHostApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UsePSWLogger();

            var UseConsulDev = Configuration.GetSection("UseConsulDev").Value;

            if (UseConsulDev.ToLower() == "true")
            {
                app.UseConsul(lifetime);
            }

            app.UseCors("CorsPolicy");

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
