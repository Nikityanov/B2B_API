using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using B2B_API.Data;
using B2B_API.Models.Auth;
using MediatR;
using FluentResults;
using B2B_API.API.Middleware;
using B2B_API.Domain.Interfaces;
using B2B_API.Infrastructure.Persistence;
using B2B_API.Infrastructure.External;
using B2B_API.Infrastructure.Repositories;
using B2B_API.CrossCutting.Validation;
using B2B_API.Domain.Entities;

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
            Console.WriteLine("Начало процесса регистрации сервисов в DI контейнере...");
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            Console.WriteLine("Базовые сервисы OpenAPI зарегистрированы");

            // Добавляем контекст базы данных
            Console.WriteLine("Регистрация контекста базы данных...");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
            Console.WriteLine("Контекст базы данных зарегистрирован");

            // Регистрируем инфраструктуру
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<JwtService>();

            // Регистрируем репозитории
            Console.WriteLine("Регистрация репозиториев в DI контейнере...");
            builder.Services.AddScoped<IRepository<Category>, EfRepository<Category>>();
            builder.Services.AddScoped<IRepository<Product>, EfRepository<Product>>();
            builder.Services.AddScoped<IRepository<User>, EfRepository<User>>();
            builder.Services.AddScoped<IRepository<Order>, EfRepository<Order>>();
            builder.Services.AddScoped<IRepository<OrderItem>, EfRepository<OrderItem>>();
            builder.Services.AddScoped<IRepository<PriceList>, EfRepository<PriceList>>();
            builder.Services.AddScoped<IRepository<PriceListProduct>, EfRepository<PriceListProduct>>();
            Console.WriteLine("Регистрация репозиториев завершена");

            // Регистрируем валидаторы команд
            Console.WriteLine("Регистрация валидаторов команд в DI контейнере...");
            builder.Services.AddScoped<CreateCategoryCommandValidator>();
            builder.Services.AddScoped<CreateProductCommandValidator>();
            builder.Services.AddScoped<CreateUserCommandValidator>();
            builder.Services.AddScoped<CreateOrderCommandValidator>();
            builder.Services.AddScoped<CreatePriceListCommandValidator>();
            builder.Services.AddScoped<AddOrderItemCommandValidator>();
            builder.Services.AddScoped<AddProductToPriceListCommandValidator>();
            builder.Services.AddScoped<DeleteOrderCommandValidator>();
            builder.Services.AddScoped<DeletePriceListCommandValidator>();
            builder.Services.AddScoped<RemoveProductFromPriceListCommandValidator>();
            builder.Services.AddScoped<UpdateOrderCommandValidator>();
            builder.Services.AddScoped<UpdatePriceListCommandValidator>();
            builder.Services.AddScoped<UpdateProductPriceCommandValidator>();
            Console.WriteLine("Регистрация валидаторов команд завершена");

            // Регистрируем MediatR для CQRS паттерна
            Console.WriteLine("Регистрация MediatR...");
            builder.Services.AddMediatR(typeof(Program).Assembly);
            Console.WriteLine("MediatR зарегистрирован");

            // Настройка аутентификации
            Console.WriteLine("Настройка аутентификации JWT...");
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
            Console.WriteLine("Настройка контроллеров и авторизации...");
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddAuthorization();
            builder.Services.AddControllers();
            Console.WriteLine("Контроллеры и авторизация настроены");
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

            // Добавляем CORS
            Console.WriteLine("Регистрация CORS...");
            builder.Services.AddCors();
            Console.WriteLine("CORS зарегистрирован");

            Console.WriteLine("Процесс регистрации сервисов в DI контейнере завершен");

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

            // Настройка CORS
            app.UseCors(builder => builder
                .WithOrigins("http://localhost:3000") // Разрешить запросы с frontend домена
                .AllowAnyMethod() // Разрешить все методы (GET, POST, PUT, DELETE и т.д.)
                .AllowAnyHeader() // Разрешить все заголовки
                .AllowCredentials() // Разрешить передачу учетных данных
                );

            // Добавляем глобальный обработчик исключений
            app.UseGlobalExceptionHandler();

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
            // Получаем секретный ключ из конфигурации
            var secretKey = builder.Configuration["JwtSettings:SecretKey"];

            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("JWT secret key is not configured in JwtSettings.");
            }

            JwtHelper.ValidateSecretKeyLength(secretKey); // Вызываем функцию валидации через класс JwtHelper

            return secretKey;
        }


    }
}
