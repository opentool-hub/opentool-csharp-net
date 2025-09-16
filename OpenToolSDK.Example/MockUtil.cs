namespace OpenToolSDK.Example;

public class MockUtil
{
    private readonly List<string> _storage = ["Hello", "World"];

    public int Count() => _storage.Count;

    public int Create(string text)
    {
        _storage.Add(text);
        return _storage.Count - 1;
    }

    public string Read(int id)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(id, nameof(id));
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(id, _storage.Count, nameof(id));

        return _storage[id];
    }

    public void Update(int id, string text)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(id, nameof(id));
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(id, _storage.Count, nameof(id));

        _storage[id] = text;
    }

    public void Delete(int id)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(id, nameof(id));
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(id, _storage.Count, nameof(id));

        _storage.RemoveAt(id);
    }

    public void Run() =>
        throw new Exception("A fatal error to break this tool.");
}