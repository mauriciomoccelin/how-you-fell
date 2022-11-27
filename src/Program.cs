using HowYouFell.Api.Data;
using HowYouFell.Api.Services;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<IAspNetUserService, AspNetUserService>();
builder.Services.AddScoped<IMongoRepository, MongoRepository>();
builder.Services
    .AddAuthentication("Bearer")
    .AddJwtBearer(
        options =>
        {
            var projectId = builder.Configuration["Authentication:Google:ProjectId"];

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateLifetime = true,
                ValidateAudience = true,
                ValidAudience = projectId,
                ValidIssuer = $"https://securetoken.google.com/{projectId}",
            };

            options.Authority = $"https://securetoken.google.com/{projectId}";
        }
    );

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
