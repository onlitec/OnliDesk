namespace OnliDesk.Server.Services;

public class ClientIdProvider
{
    private readonly Random _random = new();
    private readonly object _lock = new();

    public string GenerateClientId()
    {
        lock (_lock)
        {
            // Generate format: XXX XXX XXX (3 groups of 3 digits)
            var part1 = _random.Next(100, 999);
            var part2 = _random.Next(100, 999);
            var part3 = _random.Next(100, 999);
            
            return $"{part1} {part2} {part3}";
        }
    }
}
