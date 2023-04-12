public class GeneralCounterService
{
    private readonly Dictionary<string, int> _counters = new();

    public void IncrementCounter(string url)
    {
        if (_counters.ContainsKey(url))
        {
            _counters[url]++;
        }
        else
        {
            _counters[url] = 1;
        }
    }

    public int GetCounter(string url)
    {
        return _counters.ContainsKey(url) ? _counters[url] : 0;
    }

    public IReadOnlyDictionary<string, int> GetAllCounters()
    {
        return _counters;
    }
}
