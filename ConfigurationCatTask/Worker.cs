using Microsoft.Extensions.Options;

namespace ConfigurationCatTask;

public class Worker(ILogger<Worker> logger,IOptions<ConfigurationCat> options,HttpClient httpClient) : BackgroundService
{
    private readonly ConfigurationCat _options = options.Value;
    private readonly ILogger<Worker> _logger = logger;
    private readonly HttpClient _httpClient = httpClient;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            for (int i = 0; i < _options.RetryCount; i++)
            {
                try
                {
                    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(_options.TimeoutSeconds));
                    using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken, cts.Token);

                    var response = await _httpClient.GetAsync(_options.ApiUrl, linkedCts.Token);

                    if (response.IsSuccessStatusCode)
                    {
                        _logger.LogInformation("Data fetched successfully!");
                        break;
                    }

                    _logger.LogWarning("API request failed. Retrying...");
                }
                catch (TaskCanceledException)
                {
                    _logger.LogWarning("Request timed out.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Exception during API request");
                }
            }

            await Task.Delay(_options.DelayMilliseconds, stoppingToken);
        }
    }
}

