using HowYouFell.Api.Data;
using HowYouFell.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<IAspNetUserService, AspNetUserService>();
builder.Services.AddScoped<IMongoRepository, MongoRepository>();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(
        options =>
        {
            var validationParameters = new TokenValidationParameters
            {
                NameClaimType = "name",
                RoleClaimType = "role",
                ValidateIssuer = false,
                ValidateAudience = true
            };

            options.TokenValidationParameters = validationParameters;
            options.Audience= builder.Configuration.GetValue<string>("KeyCloak:Audience");
            options.Authority = builder.Configuration.GetValue<string>("KeyCloak:Authority");
            options.RequireHttpsMetadata = builder.Configuration.GetValue<bool>("KeyCloak:RequireHttps");
        }
    );

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
