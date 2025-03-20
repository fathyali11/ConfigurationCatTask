namespace ConfigurationCatTask;

public class ConfigurationCat
{
    public string ApiUrl { get; set; }=string.Empty;
    public int RetryCount { get; set; }
    public int DelayMilliseconds { get; set; }
    public int TimeoutSeconds { get; set; }
}

