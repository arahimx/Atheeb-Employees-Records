using Core.IRepository;
using Infrastructure.Data;
using Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using WebApi.Mapping;
using RestApi.Seeds;
using RestApi.Interface;
using RestApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Set up the connection to the SQL Server database
string connectionString = builder.Configuration.GetConnectionString("SQLConnection");

// Register the database context with dependency injection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddHttpContextAccessor();

// Add essential services for controllers and API documentation
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(Assembly.GetAssembly(typeof(MappingProfile)));
builder.Services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
builder.Services.AddScoped<IFileService>(provider =>
{
    var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"); // Ensure path is correct
    var httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();
    return new FileService(uploadFolder, httpContextAccessor);
});
// Configure CORS to allow only specific domains
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        policy =>
        {
            policy.WithOrigins("localhost")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

// Set up middleware for development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    AppDbInitializer.Seed(app);  // Seed initial data for development
}

// Apply HSTS to enforce HTTPS
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();  // Use HSTS in production to enforce HTTPS
}

app.UseHttpsRedirection();    // Redirect HTTP to HTTPS
app.UseRouting();
app.UseCors("AllowSpecificOrigins");  // Apply CORS policy

//app.UseMiddleware<JwtTokenMiddleware>();  // Uncomment for JWT authentication
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseStaticFiles(); // For serving static files


app.Run();  // Start the application
