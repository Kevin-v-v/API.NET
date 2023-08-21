using System.Text;
using BankAPI.Data;
using BankAPI.Services;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//DbContext
builder.Services.AddSqlServer<BankContext>(builder.Configuration.GetConnectionString("BankConnection"));

//Service Layer
builder.Services.AddScoped<ClientService>();
builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<AccountTypeService>();
builder.Services.AddScoped<LoginService>();
builder.Services.AddScoped<BankTransactionService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer("admin", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters{
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:AdminKey"])),
        ValidateIssuer = false,
        ValidateAudience = false
    };
})
.AddJwtBearer("client", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters{
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:ClientKey"])),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddAuthorization(options => {
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .AddAuthenticationSchemes("client")
            .Build();
    options.AddPolicy("Client", new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .AddAuthenticationSchemes("client")
            .Build());
    options.AddPolicy("SuperAdmin", policy => policy.RequireClaim("AdminType", "Super"));
    options.AddPolicy("Admin",new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .AddAuthenticationSchemes("admin")
            .Build());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
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
