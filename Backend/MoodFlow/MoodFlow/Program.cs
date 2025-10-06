using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MoodFlow.Data;
using MoodFlow.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args); //webappbuilder setting dependency inj, logs, configuration

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); //used for swagger, allows minimal apis to be discovered for swagger
builder.Services.AddSwaggerGen(); //swager ui

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "MoodFlow API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme //telling swagger my api uses jwt for authentication
    {
        In = ParameterLocation.Header, //token will be sent in http header
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer" //name of the security scheme
    }); //this enables users to auth in swagger lockkey icon
    c.AddSecurityRequirement(new OpenApiSecurityRequirement { //tels wich endpoints require auth
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference { //points to Bearer
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            }, //all endpoints use auth
            new string[] {}
        }
    });
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true, //Prevents accepting tokens from unknown sources.
            ValidateAudience = true, //Ensures the token is intended for your application, not another one.
            ValidateLifetime = true, //Makes sure the token is not expired
            ValidateIssuerSigningKey = true, //Ensures the token was digitally signed using the correct key.
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });
//DI setup, Scoped one instance per http requ
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IDiaryItemService, DiaryItemService>();
builder.Services.AddScoped<ISentimentAnalysisService, SentimentAnalysisService>();
builder.Services.AddScoped<IMeditationService, MeditationService>();
builder.Services.AddScoped<QuoteService>();
builder.Services.AddHttpClient();
builder.Services.AddHostedService<DailyReminderService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();