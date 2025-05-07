using Tutorial3_Task;
using Tutorial3_Task.Repository;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IDeviceManager>(service =>
{
    IDeviceRepository deviceManager = service.GetService<IDeviceRepository>();
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("Connection string 'UniversityDatabase' is not configured.");
    }
    return new DeviceManager(deviceManager);
});

// Add services to the container.

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