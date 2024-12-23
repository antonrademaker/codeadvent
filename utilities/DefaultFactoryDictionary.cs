namespace AoC.Utilities;

public class DefaultFactoryDictionary<TKey, TValue> : Dictionary<TKey, TValue> where TKey : notnull
{
    private readonly Func<TKey, TValue> factory;

    public DefaultFactoryDictionary(Func<TKey, TValue> creator)
    {
        this.factory = creator;
    }

    public new TValue this[TKey key]
    {
        get
        {
            if (!TryGetValue(key, out var val))
            {
                val = factory(key);
                Add(key, val);
            }
            return val;
        }
        set
        {
            base[key] = value;
        }
    }
}