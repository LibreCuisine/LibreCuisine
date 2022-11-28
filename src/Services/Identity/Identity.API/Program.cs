using Identity.API.Data;
using Identity.API.Options;
using Identity.API.Repositories;
using Identity.API.Services;
using Identity.Common.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection(JwtOptions.Jwt));
builder.Services.AddDbContext<IdentityDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IHashService, HashService>();
builder.Services.AddScoped<ITokenService, TokenService>();
// Add services to the container.
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

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<IdentityDbContext>();
    context.Database.Migrate();
}
app.UseHttpsRedirection();

app.MapPost("register", ([FromBody] UserDto userDto, IUserService usrSrv, ITokenService tokenSrv) =>
{
    var id = usrSrv.RegisterUser(userDto);
    var scopes = usrSrv.GetScopesOfUser(id);
    return tokenSrv.GenerateToken(userDto.Username, scopes);
}).WithName("Register").WithOpenApi();

app.MapPost("login", ([FromBody] UserDto userDto, IUserService usrSrv, ITokenService tknSrv) =>
{
    var usr = usrSrv.ValidateUser(userDto);
    return usr is null ? TypedResults.BadRequest() : Results.Ok(tknSrv.GenerateToken(usr.Username, usr.Scopes));
})
.WithName("Login")
.Produces<string>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest)
.WithOpenApi();

app.Run();
