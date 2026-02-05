//EMAIL
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Net;
using System.Net.Mail;
using System.Text;
using WineLabelMakerBE.Models.Data;
using WineLabelMakerBE.Models.Entity;
using WineLabelMakerBE.Services;
using WineLabelMakerBE.Services.Interface;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Configurazione del DbContext con SQL Server 
builder.Services.AddDbContext<ApplicationDbContext>(
    option => option.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"))
    );

//SERVICE
builder.Services.AddScoped<IRequestService, RequestService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IEmailService, EmailService>();

//IDENTITY 
builder.Services.AddScoped<UserManager<ApplicationUser>>();
builder.Services.AddScoped<SignInManager<ApplicationUser>>();
builder.Services.AddScoped<RoleManager<IdentityRole>>();


builder.Services.AddIdentity<ApplicationUser, IdentityRole>(
    options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 8;
        options.SignIn.RequireConfirmedAccount = false;
    }).AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost5173", builder =>
    {
        builder
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


//Configurazione autenticazione
builder.Services.AddAuthentication(
    options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(
    options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true, // Validazione dell'emittente del token
            ValidateAudience = true, // Validazione del destinatario del token
            ValidateLifetime = true, // Validazione della scadenza del token
            ValidateIssuerSigningKey = true, // Validazione della chiave di firma del token
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecurityKey"]))
        };
    });



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

//AUTENTICAZIONE SWAGGER TOKEN AMMINISTRATORE

builder.Services.AddSwaggerGen(option =>
{
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    { Name = "Authorization", In = ParameterLocation.Header, Type = SecuritySchemeType.Http, Scheme = "Bearer" });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
            Array.Empty<string>() } });
});

//-----------------------------------------------------------------
//EMAIL-Configurazione SMTP per GMAIL
var smtpClient = new SmtpClient("smtp.gmail.com")
{
    Port = 587,
    Credentials = new NetworkCredential(
        "reb.matarozzo@gmail.com",
        "nfgs sfmw jhbc ymxi"
    ),
    EnableSsl = true
};

//REGISTRAZIONE FLUENT EMAIL
builder.Services
    .AddFluentEmail("reb.matarozzo@gmail.com")
    .AddRazorRenderer()
    .AddSmtpSender(smtpClient);
//-----------------------------------------------------------------


var app = builder.Build();

//DbSeader
await DbSeader.SeedAsync(app.Services);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//else
//{
//    app.UseHsts();
//}

app.UseHttpsRedirection();

app.UseCors("AllowLocalhost5173");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
