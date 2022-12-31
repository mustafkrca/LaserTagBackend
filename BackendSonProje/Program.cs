using BackendSonProje.Models.Entites;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<UserDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("devConnection")));


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(options =>
options.WithOrigins("http://localhost:3000/")
.AllowAnyMethod()
.AllowAnyHeader()
.AllowAnyOrigin()
);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
