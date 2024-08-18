using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Sinks.PostgreSQL;

namespace BankingSystem.Infrastructure.Configuration;

public static class SerilogConfiguration
{
	public static void AddSerilogServices(this IServiceCollection services, IConfiguration configuration)
	{
		var connectionString = configuration.GetConnectionString("DefaultConnection");
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
	}
}
