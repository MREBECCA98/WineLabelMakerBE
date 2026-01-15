using Microsoft.EntityFrameworkCore;
using WineLabelMakerBE.Models.Data;
using WineLabelMakerBE.Services;
using WineLabelMakerBE.Services.Interface;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Configurazione del DbContext con SQL Server 
builder.Services.AddDbContext<ApplicationDbContext>(
    option => option.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"))
    );

//Iniezione dei service
builder.Services.AddScoped<IRequestService, RequestService>();
builder.Services.AddScoped<IMessageService, MessageService>();


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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
