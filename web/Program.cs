using application.Interfaces;
using application.Services;
using infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// Program.cs - Update the registration
builder.Services.AddSingleton<IPolicyRepository, InMemoryRepository>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IPremiumCalculatorService, PremiumCalculatorService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    DataSeeder.SeedData(scope.ServiceProvider);
}


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