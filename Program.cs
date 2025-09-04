using E_CommerceSystem.Repositories;
using E_CommerceSystem.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using E_CommerceSystem.Auth;
using E_CommerceSystem.Middleware;

namespace E_CommerceSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers();

            // Add services to the container.
            builder.Services.AddScoped<IUserRepo, UserRepo>();
            builder.Services.AddScoped<IUserService, UserService>();


            builder.Services.AddScoped<IProductRepo, ProductRepo>();
            builder.Services.AddScoped<IProductService, ProductService>();

            builder.Services.AddScoped<IOrderProductsRepo, OrderProductsRepo>();
            builder.Services.AddScoped<IOrderProductsService, OrderProductsService>();

            builder.Services.AddScoped<IOrderRepo, OrderRepo>();
            builder.Services.AddScoped<IOrderService, OrderService>();

            builder.Services.AddScoped<IReviewRepo, ReviewRepo>();
            builder.Services.AddScoped<IReviewService, ReviewService>();

            //register category ...
            builder.Services.AddScoped<ICategoryRepo, CategoryRepo>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            //register supplier ...
            builder.Services.AddScoped<ISupplierRepo, SupplierRepo>();
            builder.Services.AddScoped<ISupplierService, SupplierService>();

            //register AdminServices ...
            builder.Services.AddScoped<IAdminServices, AdminServices>();

            //register TokenService and AuthService ...
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IAuthService, AuthService>();

            //register EmailService ...
            builder.Services.AddScoped<IEmailService, EmailService>();


            // Add AutoMapper and scan for profiles ...
            builder.Services.AddAutoMapper(typeof(MappingProfile));

            //register DbContext ...
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseLazyLoadingProxies() //to enable lazy loading ...
                    .UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // ---- STEP 2: read/bind JwtSettings (NEW) ----
            var jwtSettings = builder.Configuration.GetSection("JwtSettings");
            builder.Services.Configure<JwtSettings>(jwtSettings); // bind for IOptions<JwtSettings> later

            var secretKey = jwtSettings["SecretKey"]
                ?? throw new InvalidOperationException("JwtSettings:SecretKey is missing.");

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var accessMinutes = jwtSettings.GetValue<int>("AccessTokenMinutes", 15);
            var refreshDays = jwtSettings.GetValue<int>("RefreshTokenDays", 7);
            // --------------------------------------------

            // Add JWT Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false, // You can set this to true if you want to validate the issuer.
                    ValidateAudience = false, // You can set this to true if you want to validate the audience.
                    ValidateLifetime = true, // Ensures the token hasn't expired.
                    ValidateIssuerSigningKey = true, // Ensures the token is properly signed.
                    IssuerSigningKey = signingKey // Match with your token generation key.
                };
                // Read token from cookie if Authorization header is missing
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        if (string.IsNullOrEmpty(context.Token))
                        {
                            var cookieToken = context.Request.Cookies["access_token"];
                            if (!string.IsNullOrEmpty(cookieToken))
                            {
                                context.Token = cookieToken;
                            }
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "E-Commerce API", Version = "v1" });
                // Use HTTP Bearer so Swagger auto-prefixes "Bearer "
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Paste ONLY your JWT below. Swagger will add 'Bearer ' automatically.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
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
                        //new string[] {}
                        Array.Empty<string>()
                    }
                });
            });

            //to enable Role-based Authorization ...
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", p => p.RequireRole(Roles.Admin));
                options.AddPolicy("ManagerOnly", p => p.RequireRole(Roles.Manager));
                options.AddPolicy("CustomerOnly", p => p.RequireRole(Roles.Customer));
                options.AddPolicy("AdminOrManager", p => p.RequireRole(Roles.Admin, Roles.Manager));
            });

            var app = builder.Build();

            //to enable custom error handling middleware ...
            app.UseMiddleware<ErrorHandlingMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication(); //jwt check middleware
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }

    // Small POCO to enable IOptions<JwtSettings> in services (keeps step 2 complete)
    public sealed class JwtSettings
    {
        public string SecretKey { get; set; } = string.Empty;
        public int AccessTokenMinutes { get; set; } = 15;
        public int RefreshTokenDays { get; set; } = 7;
    }
}
