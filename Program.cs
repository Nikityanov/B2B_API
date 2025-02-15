using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using B2B_API.Data;
using B2B_API.Interfaces;
using B2B_API.Repositories;
using B2B_API.Services;
using B2B_API.Models.Auth;

namespace B2B_API
{
    public static class JwtHelper // Объявляем статический класс JwtHelper
    {
        internal const int MinimumSecretKeyLength = 32; // Константа для минимальной длины ключа

        internal static void ValidateSecretKeyLength(string secretKey)
        {
            if (string.IsNullOrEmpty(secretKey) || secretKey.Length < MinimumSecretKeyLength)
            {
                throw new InvalidOperationException(
                    $"JWT секретный ключ должен быть не менее {MinimumSecretKeyLength} символов."); // Используем константу в сообщении
            }
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Добавляем контекст базы данных
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Регистрируем репозитории и сервисы
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<JwtService>();
            builder.Services.AddScoped<CategoryService>();

            // Настройка аутентификации
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                        ValidAudience = builder.Configuration["JwtSettings:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                                            Encoding.UTF8.GetBytes(GetJwtSecretKey(builder))),
                        ClockSkew = TimeSpan.Zero, // Убираем временное окно
                        ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256 } // Фиксируем алгоритм
                    };
                });

            // Настройка Swagger с поддержкой JWT
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddAuthorization();
            builder.Services.AddControllers();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "B2B API",
                    Version = "v1",
                    Description = "API для B2B торговой платформы"
                });

                // Добавляем поддержку JWT в Swagger
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            // Получение секретного ключа JWT из переменных окружения
            // Валидация конфигурации JWT при старте приложения
            builder.Services.AddOptions<JwtSettings>()
                .BindConfiguration("JwtSettings")
                .ValidateDataAnnotations()
                .Validate(jwt =>
                {
                    if (string.IsNullOrEmpty(jwt.SecretKey))
                        return false;
                    if (jwt.SecretKey.Length < 32)
                        return false;
                    return true;
                }, "SecretKey должен быть минимум 32 символа")
                .ValidateOnStart();

            











                

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "B2B API V1");
                });
            }

            app.UseHttpsRedirection();

            // Добавляем middleware аутентификации и авторизации
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            // Создаем базу данных при запуске приложения
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<ApplicationDbContext>();
                context.Database.EnsureCreated();

                // Вывод количества записей в базе данных
                var priceListsCount = context.PriceLists?.Count() ?? 0;
                var productsCount = context.Products?.Count() ?? 0;
                var usersCount = context.Users?.Count() ?? 0;

                Console.WriteLine($"Количество прайс-листов: {priceListsCount}");
                Console.WriteLine($"Количество продуктов: {productsCount}");
                Console.WriteLine($"Количество пользователей: {usersCount}");
                var applicationUrls = builder.Configuration["applicationUrl"]?.Split(';') ?? Array.Empty<string>();
                var httpUrl = applicationUrls.FirstOrDefault(u => u.StartsWith("http://"));
                var httpPort = httpUrl?.Split(':').LastOrDefault() ?? "5021";
                Console.WriteLine($"Swagger UI доступен по адресу: http://localhost:{httpPort}/swagger");
            }

            app.Run();
        }

        private static string GetJwtSecretKey(WebApplicationBuilder builder)
        {
            // Обновленный порядок с учетом переменных окружения
            var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
                ?? builder.Configuration["JwtSettings:SecretKey"];

            if (secretKey == null)
            {
                throw new InvalidOperationException("JWT secret key is not configured.");
            }

            JwtHelper.ValidateSecretKeyLength(secretKey); // Вызываем функцию валидации через класс JwtHelper

            return secretKey;
        }


    }
}