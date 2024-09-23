using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SalesCommissionsAPI;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();


builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


// Register CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder =>
        builder.WithOrigins("http://localhost:5173")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials());
});


//// Add services to the container.
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("MDDATAConnection")));

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("InsiteConnection")));

// Add services to the container.
builder.Services.AddDbContext<LocalDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LocalDatabaseConnection")));

// Add Identity services
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<LocalDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)//options =>
                                                                          //{

// This will determine which scheme to use based on the request
//options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; // Default to JWT
//options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//.AddJwtBearer(options =>
//{
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuer = true,
//        ValidateAudience = true,
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = true,
//        ValidIssuer = builder.Configuration["Jwt:Issuer"],
//        ValidAudience = builder.Configuration["Jwt:Issuer"],
//        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
//    };
//})
.AddNegotiate(); // Add Windows Authentication using Kerberos or NTLM
//.AddScheme<IISDefaults.AuthenticationScheme, NegotiateHandler>("Windows", o => { }); // Explicit Windows Authentication


// Add authorization services
//builder.Services.AddAuthorization();
builder.Services.AddAuthorization(options =>
{
    // Optionally configure authorization policies based on Windows or JWT roles
    //options.AddPolicy("RequireWindowsAuth", policy =>
    //    policy.RequireAuthenticatedUser().AddAuthenticationSchemes(IISDefaults.AuthenticationScheme));

    // Configure policy for Windows Authentication (Negotiate)
    options.AddPolicy("RequireWindowsAuth", policy =>
        policy.RequireAuthenticatedUser().AddAuthenticationSchemes(NegotiateDefaults.AuthenticationScheme));

    // Define a policy for a specific Active Directory group
    options.AddPolicy("RequireSpecificGroup", policy =>
        policy.RequireAuthenticatedUser()
              .RequireRole("YOUR_AD_GROUP_NAME")); // Replace with your actual AD group name
});

// Add controllers
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use CORS policy
app.UseCors("AllowSpecificOrigin");

app.UseHttpsRedirection();
app.UseAuthentication();  // Ensure this is before UseAuthorization
app.UseAuthorization();

app.MapControllers();

app.Run();
