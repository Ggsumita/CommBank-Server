using CommBank.Models;
using CommBank.Services;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Ensure that Secrets.json is loaded correctly
builder.Configuration.SetBasePath(Directory.GetCurrentDirectory() + "/CommBank-Server")
    .AddJsonFile("Secrets.json", optional: false, reloadOnChange: true);

// Debug: Print the connection string to verify it is loaded
var connectionString = builder.Configuration.GetConnectionString("CommBank");
Console.WriteLine($"Connection String: {connectionString}");  // This will print the connection string

// MongoDB Client Setup
if (string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine("Connection string is empty or null. Please check the Secrets.json file.");
}
else
{
    var mongoClient = new MongoClient(connectionString);
    var mongoDatabase = mongoClient.GetDatabase("CommBank");

    // Register services
    IAccountsService accountsService = new AccountsService(mongoDatabase);
    IAuthService authService = new AuthService(mongoDatabase);
    IGoalsService goalsService = new GoalsService(mongoDatabase);
    ITagsService tagsService = new TagsService(mongoDatabase);
    ITransactionsService transactionsService = new TransactionsService(mongoDatabase);
    IUsersService usersService = new UsersService(mongoDatabase);

    builder.Services.AddSingleton(accountsService);
    builder.Services.AddSingleton(authService);
    builder.Services.AddSingleton(goalsService);
    builder.Services.AddSingleton(tagsService);
    builder.Services.AddSingleton(transactionsService);
    builder.Services.AddSingleton(usersService);
}

builder.Services.AddCors();

var app = builder.Build();

app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
