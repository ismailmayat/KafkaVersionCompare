namespace KafkaVersionCompare.Services;

public class CacheInitializationService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public CacheInitializationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // Perform cache pre-population logic here
        using (var scope = _serviceProvider.CreateScope())
        {
            var cpReleaseBuilder = scope.ServiceProvider.GetRequiredService<ICPReleaseBuilder>();
            await cpReleaseBuilder.BuildReleaseFromCrawl();

            var releaseBuilder = scope.ServiceProvider.GetRequiredService<IReleaseBuilder>();
            await releaseBuilder.BuildReleaseFromCrawl();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}