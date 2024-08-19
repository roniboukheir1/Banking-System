using BankingSystem.API.Middleware;
using BankingSystem.Application.Commands;
using BankingSystem.Application.Common;
using BankingSystem.Application.Queries;
using BankingSystem.Application.Repo;
using BankingSystem.Application.Services;
using BankingSystem.Domain.Models;
using BankingSystem.Infrastructure.Configuration;
using BankingSystem.Infrastructure.Services;
using BankingSystem.Persistance.Configurations;
using BankingSystem.Persistance.Data;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.OpenApi.Models;
using University_Management_System.Common.Configurations;
using Serilog;
using Serilog.Sinks.PostgreSQL;
using University_Management_System.API.Configurations;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEmailService(builder.Configuration);
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers()
    .AddOData(opt => opt.Filter().Expand().Select().OrderBy().Count().SetMaxTop(100)
    .AddRouteComponents("odata", GetEdmModel()));
builder.Services.AddHealthCheckServices(builder.Configuration);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var columnWriters = new Dictionary<string, ColumnWriterBase>
{
    { "message", new RenderedMessageColumnWriter() },
    { "message_template", new MessageTemplateColumnWriter() },
    { "level", new LevelColumnWriter() },
    { "timestamp", new TimestampColumnWriter() },
    { "exception", new ExceptionColumnWriter() },
    { "log_event", new LogEventSerializedColumnWriter() },
    { "environment", new SinglePropertyColumnWriter("Environment", PropertyWriteMethod.ToString) },
};

Log.Logger = new LoggerConfiguration()
    .Enrich.WithEnvironmentName()
    .Enrich.WithThreadId()
    .Enrich.WithProcessId()
    .WriteTo.Console()
    .WriteTo.PostgreSQL(
        connectionString: connectionString,
        tableName: "logs",
        columnOptions: columnWriters,
        needAutoCreateTable: true) 
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

builder.Services.AddAuthenticationServices(builder.Configuration);
builder.Services.AddScoped<TenantService>();
builder.Services.AddDatabaseServices(builder.Configuration);
builder.Services.AddScoped<BankingSystemContextFactory>(provider =>
    {
        var options = provider.GetRequiredService<DbContextOptions<BankingSystemContext>>();
        var defaultSchema = "branch1";
        return new BankingSystemContextFactory(options, defaultSchema);
    });
builder.Services.AddRepositories();
builder.Services.AddMemoryCache();
builder.Services.AddScoped<BranchService>();
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(typeof(CreateAccountCommand).Assembly);
    config.RegisterServicesFromAssembly(typeof(GetCustomerAccountsQuery).Assembly);
});

builder.Services.AddScoped<TokenServices>();
builder.Services.AddScoped<IEventStore, EventStore>();
builder.Services.AddScoped<TransactionMessagePublisher>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API V1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();  
app.UseAuthorization();

// Middleware to handle tenant-specific logic, uncomment if you have implemented TenantMiddleware
app.UseMiddleware<TenantMiddleware>();

app.UseMiddleware<ExceptionMiddleware>();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.UseSwaggerUI();

app.Run();

IEdmModel GetEdmModel()
{
    var odataBuilder = new ODataConventionModelBuilder();
    odataBuilder.EntitySet<Customer>("Customers");
    odataBuilder.EntitySet<Account>("Accounts");
    odataBuilder.EntitySet<Transaction>("Transactions");
    return odataBuilder.GetEdmModel();
}
