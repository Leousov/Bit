using Bit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
string connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connection));
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy(name: MyAllowSpecificOrigins,
//                      policy =>
//                      {
//                          policy.WithOrigins("http://localhost:5292/Data/Read/", "http://localhost:5292/Data/Write/");
//                      });
//});
builder.Services.AddCors();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(builder => { builder.AllowAnyOrigin(); builder.AllowAnyHeader(); builder.AllowAnyMethod(); });
app.UseAuthorization();
app.MapControllers();
//app.MapControllerRoute(
//    name: "d_create",
//    pattern: "{controller=Data}/{action=Create}/");

//app.Map(
//    "{controller=Data}/{action=Index}/{id?}",
//    (string controller, string action, string? id) =>
//        action);

app.MapGet("/Data/Read/", (ApplicationContext db) => db.Data.ToList());
app.MapPost("/Data/Write/", (Data d, ApplicationContext db) => { db.Data.Add(d); db.SaveChanges(); return d; });

app.Run();
