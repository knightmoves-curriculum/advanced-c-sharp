public class RateLimitingService
{
    private readonly Dictionary<string, List<DateTime>> _requests = new();
    private readonly int _maxRequests = 10;
    private readonly TimeSpan _timeWindow = TimeSpan.FromMinutes(1); 

    public bool IsRequestAllowed(string clientKey)
    {
        if (!_requests.ContainsKey(clientKey))
        {
            _requests[clientKey] = new List<DateTime>();
        }

        _requests[clientKey].RemoveAll(request => request < DateTime.UtcNow - _timeWindow);

        if (_requests[clientKey].Count >= _maxRequests)
        {
            return false;
        }

        _requests[clientKey].Add(DateTime.UtcNow);
        return true;
    }
}
