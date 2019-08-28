using System.Linq;
using System.Reflection;
using FluentValidation;
using FutreTechAPI.BL;
using FutreTechAPI.BL.NotificationV2;
using FutreTechAPI.BL.NotificationV2.Decorators;
using FutreTechAPI.BL.NotificationV3.Decorators;
using FutreTechAPI.BL.NotificationV4;
using FutreTechAPI.BL.NotificationV4B;
using FutreTechAPI.BL.NotificationV4B.Pipelines;
using FutreTechAPI.BL.NotificationV4B.Queries;
using FutreTechAPI.Helpers;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static FutreTechAPI.BL.NotificationV4B.Queries.GetCitiyById;
using static FutreTechAPI.BL.NotificationV4B.SendEmailCommand;
using static FutreTechAPI.BL.NotificationV4B.SendSMSCommand;

namespace FutreTechAPI
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
            services.Scan(scan => scan
                .FromAssembliesOf(typeof(BL.NotificationV1.INotificationService))
                .AddClasses(t => t.Where(r => !r.Name.Contains("Decorator") && !r.Name.Contains("Cache") && r.Namespace != "FutreTechAPI.BL.NotificationV4"))
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            );

            services.AddSingleton<ICache, DictionaryCache>();

            #region V2
            services.Decorate<INotificationServiceV2, NotificationServiceV2LoogerDecorator>();
            services.Decorate<INotificationServiceV2>((inner, provider) => new NotificationServiceV2CacheDecorator(inner, provider.GetRequiredService<ICache>()));
            #endregion

            #region v3
            services.Decorate<INotificationServiceV3, NotificationServiceV3LoogerDecorator>();
            services.Decorate<INotificationServiceV3>((inner, provider) => new NotificationServiceV3RequestCacheDecorator(inner, provider.GetRequiredService<ICache>()));
            //services.Decorate<INotificationServiceV3>((inner, provider) => new NotificationServiceV3RequestValidatorDecorator(inner, provider.GetRequiredService<IObjectValidator>()));
            #endregion

            #region v4

            services.AddTransient<IValidator<BL.NotificationV4.SendEmail>, BL.NotificationV4.SendEmailValidator>();

            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            //services.AddScoped(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
            //services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RetryUnitOfWorkBehavior<,>));
            services.AddMediatR(typeof(Startup).GetTypeInfo().Assembly);

            #endregion

            #region V4B

            services.AddTransient(provider => new BL.NotificationV4B.Mediator(provider));

            services.RegisterCommandWithPipeline<SendSMSCommand, SendSMSHandler>(Command.Pipelines.DefaultWithRetry<SendSMSCommand>);
            services.RegisterCommandWithPipeline<SendEmailCommand, SendEmailHandler>(Command.Pipelines.Default<SendEmailCommand>);
            services.RegisterQueryWithPipeline<GetCitiyById, GetCityByIdHandler, GetCityByIdResponse>(Query.Pipelines.Default<GetCitiyById, GetCityByIdResponse>);

            #endregion
            
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo() { Title = "My API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "DKS API");
                c.InjectStylesheet("/swagger-ui/custom.css");
            });

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
