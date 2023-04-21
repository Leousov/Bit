using Bit;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;

Console.WriteLine(AesEncryptor.Decrypt(AesEncryptor.Encrypt("asdasd")));
var builder = WebApplication.CreateBuilder(args);

string connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connection));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();
builder.Services.AddAuthentication("Bearer")  
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // указывает, будет ли валидироваться издатель при валидации токена
            ValidateIssuer = true,
            // строка, представляющая издателя
            ValidIssuer = AuthOptions.ISSUER,
            // будет ли валидироваться потребитель токена
            ValidateAudience = true,
            // установка потребителя токена
            ValidAudience = AuthOptions.AUDIENCE,
            // будет ли валидироваться время существования
            ValidateLifetime = true,
            // установка ключа безопасности
            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
            // валидация ключа безопасности
            ValidateIssuerSigningKey = true,
        };
    });
builder.Services.AddAuthorization();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(builder => { builder.AllowAnyOrigin(); builder.AllowAnyHeader(); builder.AllowAnyMethod(); });
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/Data/Read/", [Authorize](ApplicationContext db) => { // 
    try
    {
        var data = db.Data.ToList();
        Console.WriteLine(data);
        return Results.Json(AesEncryptor.Encrypt(data.ToString()));
    }
    catch
    {
        return Results.StatusCode(400);
    }
});
app.MapPost("/Data/Write/", [Authorize](Data d, ApplicationContext db) =>
{
    try {
        db.Data.Add(d);
        db.SaveChanges();
        return d;
    }
    catch
    {
        Results.StatusCode(400);
        return null;
    }
     });
app.MapPost("/login/", (Users loginData, ApplicationContext db) =>
{
    bool q = true;
    Users? person = new Users();
    foreach (var user in db.Users.ToList())
    {
        if (loginData.Login == user.Login && loginData.Password == user.Password)
        {
            person = user;
            q = false;
        }
    }
    if (q) return Results.Unauthorized();
    var claims = new List<Claim> { new Claim(ClaimTypes.Name, person.Login) };
    var jwt = new JwtSecurityToken(
            issuer: AuthOptions.ISSUER,
            audience: AuthOptions.AUDIENCE,
            claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)),
            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
    var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
    var response = new
    {
        access_token = encodedJwt,
        username = person.Login
    };

    return Results.Json(response);
});
app.MapPost("/registration", (Users loginData, ApplicationContext db) =>
{
    try
    {
        db.Users.Add(loginData);
        db.SaveChanges();
        return loginData;
    }
    catch
    {
        Results.StatusCode(400);
        return null;
    }
});

app.Run();
public class AuthOptions
{
    public const string ISSUER = "MyAuthServer"; // издатель токена
    public const string AUDIENCE = "MyAuthClient"; // потребитель токена
    const string KEY = "mysupersecret_secretkey!123";   // ключ для шифрации
    public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
}

