using DAL.Extensions;
using BLL.Extensions;
using JwtAuthorizationApi.Services;
using JwtAuthorizationApi.Services.Auth.Authorization.Requirements;
using JwtAuthorizationApi.Services.Extentions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using JwtAuthorizationApi.Services.Auth.Authentication;
using Microsoft.AspNetCore.Authorization;
using JwtAuthorizationApi.Services.Auth.Authorization.Handlers;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddRepository(builder.Configuration);
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITokenFactory, TokenFactory>();
builder.Services.AddScoped<IAuthorizationHandler, RoleAuthorizationHandler>();
builder.Services.AddBll();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "JwtAuth", Version = "v1" });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration.GetJwtIssuer(),
        ValidAudience = builder.Configuration.GetJwtAudience(),
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetJwtKey())),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization(options =>
{
    foreach (var permission in builder.Configuration.GetJwtPermissions())
    {
        options.AddPolicy(permission, builder =>
            builder.AddRequirements(new RoleAuthorizationRequiment(permission)));
    }
});

builder.Services.AddControllers();



var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.RoutePrefix = "";
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Jwt v1");
    });
}

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
