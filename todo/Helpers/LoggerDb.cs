using Microsoft.EntityFrameworkCore;

namespace todo.Helpers
{
    public class LoggerDb
    {
        public static ILoggerFactory GetLoggerFactory()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(builder =>
                    builder.AddConsole()
                            .AddFilter(DbLoggerCategory.Database.Command.Name,
                                    Microsoft.Extensions.Logging.LogLevel.Information));
            return serviceCollection.BuildServiceProvider()
                    .GetService<ILoggerFactory>();
        }
    }
}
