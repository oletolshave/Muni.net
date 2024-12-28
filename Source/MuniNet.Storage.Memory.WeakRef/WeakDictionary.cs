#nullable disable
using System.Runtime.CompilerServices;

namespace BernhardHaus.Collections.WeakDictionary;

public class WeakDictionary<TKey, TValue>
    where TValue : class
{
    private readonly Dictionary<TKey, WeakReference> _internalDictionary = new();
    private readonly ConditionalWeakTable<TValue, Finalizer> _conditionalWeakTable = new();
    private readonly object _lock = new();

    public TValue this[TKey key]
    {
        set
        {
            if (value is null)
                throw new ArgumentNullException();

            Remove(key);
            Add(key, value);
        }
    }

    public void Add(TKey key, TValue value)
    {
        var finalizer = new Finalizer(key);
        
        lock (_lock)
        {
            this._internalDictionary.Add(key, new WeakReference(value));
        }

        finalizer.ValueFinalized += k =>
        {
            lock (_lock)
            {
                RemoveWithLockHeld(k);
            }
        };

        lock (_lock)
        {
            this._conditionalWeakTable.Add(value, finalizer);
        }
    }

    public bool Remove(TKey key)
    {
        lock (_lock)
        {
            return RemoveWithLockHeld(key);
        }
    }

    private bool RemoveWithLockHeld(TKey key)
    {
        return this._internalDictionary.Remove(key);
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        lock (_lock)
        {
            if (this._internalDictionary.TryGetValue(key, out var valueReference))
            {
                value = (TValue)valueReference.Target;
                return true;
            }

            value = default;
            return false;
        }
    }

    private sealed class Finalizer
    {
        private readonly TKey valueKey;

        public Finalizer(TKey valueKey)
        {
            this.valueKey = valueKey;
        }

        ~Finalizer()
        {
            ValueFinalized?.Invoke(this.valueKey);
        }

        public event ValueFinalizedDelegate ValueFinalized;
    }

    private delegate void ValueFinalizedDelegate(TKey valueKey);
}