using Store.Api.MapperConfigurations;
using Store.Api.Middlwares;

namespace Store.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

			builder.Services.AddCors(options =>
			{
				options.AddPolicy("AllowBlazorApp", policy =>
				{
					policy.WithOrigins("https://localhost:7185")
						  .AllowAnyMethod()
						  .AllowAnyHeader();
				});
			});

			builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddAutoMapper(typeof(ViewModelsMappingProfile));

            string connectionString = builder.Configuration.GetConnectionString("StoreDB");

            Business.Configuration.Configure(builder.Services, connectionString);

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("AllowBlazorApp");

			app.UseMiddleware<ExceptionMiddlware>();

            app.UseMiddleware<AuditLogMiddlware>();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
