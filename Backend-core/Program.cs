using Backend_core.Data;
using Backend_core.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;


var builder = WebApplication.CreateBuilder(args);


// Load JWT settings from appsettings.json
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

// Configure JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

// Register AppDbContext with SQL Server connection
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure CORS to allow requests from the React frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Add controller services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Build the application
var app = builder.Build();

// Use HTTPS redirection middleware
app.UseHttpsRedirection();

// Map controller routes
app.MapControllers();


// Database seeding for initial data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (!db.Users.Any(u => u.Username == "testuser"))
    {
        db.Users.Add(new User
        {
            Username = "testuser",
            PasswordHash = "testpass" // we are not hashing, maybe in the future change it 
        });
        db.SaveChanges();
    }

    // creating categories
    if (!db.Categories.Any())
    {
        db.Categories.AddRange(
            new Category { Name = "Prywatny" },
            new Category { Name = "S³u¿bowy" },
            new Category { Name = "Inne" }
        );
        db.SaveChanges();
    }

    // creating subcategories for "s³u¿bowy"
    var businessCategory = db.Categories.FirstOrDefault(c => c.Name == "S³u¿bowy");
    if (businessCategory != null && !db.Subcategories.Any(s => s.CategoryId == businessCategory.Id))
    {
        db.Subcategories.AddRange(
            new Subcategory { Name = "Szef", CategoryId = businessCategory.Id },
            new Subcategory { Name = "Klient", CategoryId = businessCategory.Id }
        );
        db.SaveChanges();
    }

    // Custom contacts for start
    if (!db.Contacts.Any())
    {
        var privateCategory = db.Categories.First(c => c.Name == "Prywatny");
        var otherCategory = db.Categories.First(c => c.Name == "Inne");
        var szefSubcategory = db.Subcategories.First(s => s.Name == "Szef");

        db.Contacts.AddRange(

            new Contact
            {
                FirstName = "Alicja",
                LastName = "Prywatna",
                Email = "alicja@prywatna.pl",
                PasswordHash = "hashPrywatny",
                PhoneNumber = "112",
                BirthDate = new DateTime(1985, 5, 10),
                CategoryId = privateCategory.Id
            },

            new Contact
            {
                FirstName = "Tomasz",
                LastName = "Wolny",
                Email = "tomasz@inne.pl",
                PasswordHash = "hashInne",
                PhoneNumber = "112",
                BirthDate = new DateTime(1985, 5, 10),
                CategoryId = otherCategory.Id,
                CustomSubcategory = "Znajomy z si³owni" 
            },

            new Contact
            {
                FirstName = "Barbara",
                LastName = "Szefowa",
                Email = "barbara@firma.pl",
                PasswordHash = "hashSzef",
                PhoneNumber = "112",
                BirthDate = new DateTime(1985, 5, 10),
                CategoryId = businessCategory.Id,
                SubcategoryId = szefSubcategory.Id
            }
        );
        db.SaveChanges();
    }
}

// Enable CORS for frontend
app.UseCors("AllowFrontend");

// Enable authentication and authorization middlewares
app.UseAuthentication();
app.UseAuthorization();

// Map controller routes again (redundant, but harmless)
app.MapControllers();

// Start the application
app.Run();
