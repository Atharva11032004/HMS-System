namespace ReservationService;

using Microsoft.EntityFrameworkCore;
using ReservationService.Data;
using ReservationService.Mappings;
using ReservationService.Models;
using ReservationService.Services;
using FluentValidation;
using Microsoft.OpenApi.Models;
using Serilog;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Host.UseSerilog((context, config) => config.ReadFrom.Configuration(context.Configuration));

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Reservation Service", Version = "v1" });
            var jwtScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            };
            c.AddSecurityDefinition("Bearer", jwtScheme);
            c.AddSecurityRequirement(new OpenApiSecurityRequirement { { jwtScheme, [] } });
        });

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = "Bearer";
            options.DefaultChallengeScheme = "Bearer";
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "defaultkey"))
            };
        });

        builder.Services.AddAuthorization();
        builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
        builder.Services.AddAutoMapper(typeof(MappingProfile));
        builder.Services.AddHttpClient<HttpClientService>();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<ReservationBusinessService>();
        builder.Services.AddHealthChecks();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.Migrate();
            await SeedDataAsync(db);
        }

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Reservation Service v1"));
        }

        app.UseSerilogRequestLogging();
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.MapHealthChecks("/health");

        await app.RunAsync();
    }

    private static async Task SeedDataAsync(ApplicationDbContext db)
    {
        if (!db.RoomTypes.Any())
        {
            db.RoomTypes.AddRange(
                new RoomType { Name = "Standard", Description = "Standard room", MaxOccupancy = 2 },
                new RoomType { Name = "Deluxe", Description = "Deluxe room", MaxOccupancy = 4 }
            );
            await db.SaveChangesAsync();
        }

        if (!db.Rooms.Any())
        {
            db.Rooms.AddRange(
                new Room { RoomNumber = "101", RoomTypeId = 1 },
                new Room { RoomNumber = "102", RoomTypeId = 1 },
                new Room { RoomNumber = "201", RoomTypeId = 2 }
            );
            await db.SaveChangesAsync();
        }

        if (!db.Guests.Any())
        {
            db.Guests.AddRange(
                new Guest { FirstName = "John", LastName = "Doe", Email = "john@example.com", Phone = "123456789" },
                new Guest { FirstName = "Jane", LastName = "Smith", Email = "jane@example.com", Phone = "987654321" }
            );
            await db.SaveChangesAsync();
        }

        if (!db.Pricings.Any())
        {
            db.Pricings.AddRange(
                new Pricing { RoomTypeId = 1, PricePerNight = 100, EffectiveDate = DateTime.Now },
                new Pricing { RoomTypeId = 2, PricePerNight = 200, EffectiveDate = DateTime.Now }
            );
            await db.SaveChangesAsync();
        }
    }
}
