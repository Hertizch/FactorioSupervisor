using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using System.Collections.Immutable;

namespace FactorioSupervisor.ObservableImmutable
{
    public class ObservableImmutableDictionary<T, TV> : ObservableCollectionObject, IImmutableDictionary<T, TV>, IDictionary<T, TV>, IDictionary
    {
        private ImmutableDictionary<T, TV> _items;

        public ObservableImmutableDictionary() : this(new KeyValuePair<T, TV>[0], LockTypeEnum.SpinWait)
        {
        }

        public ObservableImmutableDictionary(IEnumerable<KeyValuePair<T, TV>> items) : this(items, LockTypeEnum.SpinWait)
        {
        }

        public ObservableImmutableDictionary(LockTypeEnum lockType) : this(new KeyValuePair<T, TV>[0], lockType)
        {
        }

        public ObservableImmutableDictionary(IEnumerable<KeyValuePair<T, TV>> items, LockTypeEnum lockType) : base(lockType)
        {
            SyncRoot = new object();
            _items = ImmutableDictionary<T, TV>.Empty.AddRange(items);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryOperation(Func<ImmutableDictionary<T, TV>, ImmutableDictionary<T, TV>> operation)
        {
            return TryOperation(operation, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool DoOperation(Func<ImmutableDictionary<T, TV>, ImmutableDictionary<T, TV>> operation)
        {
            return DoOperation(operation, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryOperation(Func<ImmutableDictionary<T, TV>, ImmutableDictionary<T, TV>> operation, NotifyCollectionChangedEventArgs args)
        {
            try
            {
                if (TryLock())
                {
                    var oldList = _items;
                    var newItems = operation(oldList);

                    if (newItems == null)
                    {
                        // user returned null which means he cancelled operation
                        return false;
                    }

                    _items = newItems;

                    if (args != null)
                        RaiseNotifyCollectionChanged(args);
                    return true;
                }
            }
            finally
            {
                Unlock();
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryOperation(Func<ImmutableDictionary<T, TV>, KeyValuePair<ImmutableDictionary<T, TV>, NotifyCollectionChangedEventArgs>> operation)
        {
            try
            {
                if (TryLock())
                {
                    var oldList = _items;
                    var kvp = operation(oldList);
                    var newItems = kvp.Key;
                    var args = kvp.Value;

                    if (newItems == null)
                    {
                        // user returned null which means he cancelled operation
                        return false;
                    }

                    _items = newItems;

                    if (args != null)
                        RaiseNotifyCollectionChanged(args);
                    return true;
                }
            }
            finally
            {
                Unlock();
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool DoOperation(Func<ImmutableDictionary<T, TV>, ImmutableDictionary<T, TV>> operation, NotifyCollectionChangedEventArgs args)
        {
            bool result;

            try
            {
                Lock();
                var oldItems = _items;
                var newItems = operation(_items);

                if (newItems == null)
                {
                    // user returned null which means he cancelled operation
                    return false;
                }

                result = (_items = newItems) != oldItems;

                if (args != null)
                    RaiseNotifyCollectionChanged(args);
            }
            finally
            {
                Unlock();
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool DoOperation(Func<ImmutableDictionary<T, TV>, KeyValuePair<ImmutableDictionary<T, TV>, NotifyCollectionChangedEventArgs>> operation)
        {
            bool result;

            try
            {
                Lock();
                var oldItems = _items;
                var kvp = operation(_items);
                var newItems = kvp.Key;
                var args = kvp.Value;

                if (newItems == null)
                {
                    // user returned null which means he cancelled operation
                    return false;
                }

                result = (_items = newItems) != oldItems;

                if (args != null)
                    RaiseNotifyCollectionChanged(args);
            }
            finally
            {
                Unlock();
            }

            return result;
        }

        public bool DoAdd(Func<ImmutableDictionary<T, TV>, KeyValuePair<T, TV>> valueProvider)
        {
            return DoOperation
                (
                currentItems =>
                {
                    var kvp = valueProvider(currentItems);
                    var newItems = _items.Add(kvp.Key, kvp.Value);
                    return new KeyValuePair<ImmutableDictionary<T, TV>, NotifyCollectionChangedEventArgs>(newItems, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, kvp));
                }
                );
        }

        public bool DoAddRange(Func<ImmutableDictionary<T, TV>, IEnumerable<KeyValuePair<T, TV>>> valueProvider)
        {
            return DoOperation
                (
                currentItems =>
                    currentItems.AddRange(valueProvider(currentItems))
                );
        }

        public bool DoClear()
        {
            return DoOperation
                (
                currentItems =>
                    currentItems.Clear()
                );
        }

        public bool DoRemove(Func<ImmutableDictionary<T, TV>, KeyValuePair<T, TV>> valueProvider)
        {
            return DoOperation
                (
                currentItems =>
                {
                    var newKvp = valueProvider(currentItems);
                    var oldKvp = new KeyValuePair<T, TV>(newKvp.Key, currentItems[newKvp.Key]);
                    var newItems = currentItems.Remove(newKvp.Key);
                    return new KeyValuePair<ImmutableDictionary<T, TV>, NotifyCollectionChangedEventArgs>(newItems, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldKvp));
                }
                );
        }

        public bool DoRemoveRange(Func<ImmutableDictionary<T, TV>, IEnumerable<T>> valueProvider)
        {
            return DoOperation
                (
                currentItems =>
                    currentItems.RemoveRange(valueProvider(currentItems))
                );
        }

        public bool DoSetItem(Func<ImmutableDictionary<T, TV>, KeyValuePair<T, TV>> valueProvider)
        {
            return DoOperation
                (
                currentItems =>
                {
                    var newKvp = valueProvider(currentItems);
                    var oldKvp = new KeyValuePair<T, TV>(newKvp.Key, currentItems[newKvp.Key]);
                    var newItems = currentItems.SetItem(newKvp.Key, newKvp.Value);
                    return new KeyValuePair<ImmutableDictionary<T, TV>, NotifyCollectionChangedEventArgs>(newItems, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, oldKvp, newKvp));
                }
                );
        }

        public bool DoSetItems(Func<ImmutableDictionary<T, TV>, IEnumerable<KeyValuePair<T, TV>>> valueProvider)
        {
            return DoOperation
                (
                currentItems =>
                    currentItems.SetItems(valueProvider(currentItems))
                );
        }

        public bool TryAdd(Func<ImmutableDictionary<T, TV>, KeyValuePair<T, TV>> valueProvider)
        {
            return TryOperation
                (
                currentItems =>
                {
                    var kvp = valueProvider(currentItems);
                    var newItems = _items.Add(kvp.Key, kvp.Value);
                    return new KeyValuePair<ImmutableDictionary<T, TV>, NotifyCollectionChangedEventArgs>(newItems, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, kvp));
                }
                );
        }

        public bool TryAddRange(Func<ImmutableDictionary<T, TV>, IEnumerable<KeyValuePair<T, TV>>> valueProvider)
        {
            return TryOperation
                (
                currentItems =>
                    currentItems.AddRange(valueProvider(currentItems))
                );
        }

        public bool TryClear()
        {
            return TryOperation
                (
                currentItems =>
                    currentItems.Clear()
                );
        }

        public bool TryRemove(Func<ImmutableDictionary<T, TV>, KeyValuePair<T, TV>> valueProvider)
        {
            return TryOperation
                (
                currentItems =>
                {
                    var newKvp = valueProvider(currentItems);
                    var oldKvp = new KeyValuePair<T, TV>(newKvp.Key, currentItems[newKvp.Key]);
                    var newItems = currentItems.Remove(newKvp.Key);
                    return new KeyValuePair<ImmutableDictionary<T, TV>, NotifyCollectionChangedEventArgs>(newItems, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldKvp));
                }
                );
        }

        public bool TryRemoveRange(Func<ImmutableDictionary<T, TV>, IEnumerable<T>> valueProvider)
        {
            return TryOperation
                (
                currentItems =>
                    currentItems.RemoveRange(valueProvider(currentItems))
                );
        }

        public bool TrySetItem(Func<ImmutableDictionary<T, TV>, KeyValuePair<T, TV>> valueProvider)
        {
            return TryOperation
                (
                currentItems =>
                {
                    var newKvp = valueProvider(currentItems);
                    var oldKvp = new KeyValuePair<T, TV>(newKvp.Key, currentItems[newKvp.Key]);
                    var newItems = currentItems.SetItem(newKvp.Key, newKvp.Value);
                    return new KeyValuePair<ImmutableDictionary<T, TV>, NotifyCollectionChangedEventArgs>(newItems, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, oldKvp, newKvp));
                }
                );
        }

        public bool TrySetItems(Func<ImmutableDictionary<T, TV>, IEnumerable<KeyValuePair<T, TV>>> valueProvider)
        {
            return TryOperation
                (
                currentItems =>
                    currentItems.SetItems(valueProvider(currentItems))
                );
        }

        public ImmutableDictionary<T, TV> ToImmutableDictionary()
        {
            return _items;
        }

        public IEnumerator<KeyValuePair<T, TV>> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IImmutableDictionary<T, TV> Add(T key, TV value)
        {
            _items = _items.Add(key, value);
            RaiseNotifyCollectionChanged();
            return this;
        }

        public IImmutableDictionary<T, TV> AddRange(IEnumerable<KeyValuePair<T, TV>> pairs)
        {
            _items = _items.AddRange(pairs);
            RaiseNotifyCollectionChanged();
            return this;
        }

        public IImmutableDictionary<T, TV> Clear()
        {
            _items = _items.Clear();
            RaiseNotifyCollectionChanged();
            return this;
        }

        public bool Contains(KeyValuePair<T, TV> pair)
        {
            return _items.Contains(pair);
        }

        public IImmutableDictionary<T, TV> Remove(T key)
        {
            _items = _items.Remove(key);
            RaiseNotifyCollectionChanged();
            return this;
        }

        public IImmutableDictionary<T, TV> RemoveRange(IEnumerable<T> keys)
        {
            _items = _items.RemoveRange(keys);
            RaiseNotifyCollectionChanged();
            return this;
        }

        public IImmutableDictionary<T, TV> SetItem(T key, TV value)
        {
            _items = _items.SetItem(key, value);
            RaiseNotifyCollectionChanged();
            return this;
        }

        public IImmutableDictionary<T, TV> SetItems(IEnumerable<KeyValuePair<T, TV>> items)
        {
            _items = _items.SetItems(items);
            RaiseNotifyCollectionChanged();
            return this;
        }

        public bool TryGetKey(T equalKey, out T actualKey)
        {
            return _items.TryGetKey(equalKey, out actualKey);
        }

        public bool ContainsKey(T key)
        {
            return _items.ContainsKey(key);
        }

        public IEnumerable<T> Keys => _items.Keys;

        public bool TryGetValue(T key, out TV value)
        {
            return _items.TryGetValue(key, out value);
        }

        public IEnumerable<TV> Values => _items.Values;

        public int Count => _items.Count;

        void IDictionary<T, TV>.Add(T key, TV value)
        {
            Add(key, value);
        }

        ICollection<T> IDictionary<T, TV>.Keys => (_items as IDictionary<T, TV>).Keys;

        bool IDictionary<T, TV>.Remove(T key)
        {
            var oldItems = _items;
            var newItems = _items = oldItems.Remove(key);

            if (oldItems == newItems)
                return false;

            RaiseNotifyCollectionChanged();
            return true;
        }

        ICollection<TV> IDictionary<T, TV>.Values => (_items as IDictionary<T, TV>).Values;

        public TV this[T key]
        {
            get => _items[key];
            set
            {
                _items.SetItem(key, value);
                RaiseNotifyCollectionChanged();
            }
        }

        public void Add(KeyValuePair<T, TV> item)
        {
            (_items as IDictionary<T, TV>).Add(item);
            RaiseNotifyCollectionChanged();
        }

        void ICollection<KeyValuePair<T, TV>>.Clear()
        {
            Clear();
        }

        public void CopyTo(KeyValuePair<T, TV>[] array, int arrayIndex)
        {
            (_items as IDictionary<T, TV>).CopyTo(array, arrayIndex);
        }

        public bool IsReadOnly => false;

        public bool Remove(KeyValuePair<T, TV> item)
        {
            var result = (_items as IDictionary<T, TV>).Remove(item);
            RaiseNotifyCollectionChanged();
            return result;
        }

        public void Add(object key, object value)
        {
            Add((T)key, (TV)value);
        }

        void IDictionary.Clear()
        {
            Clear();
        }

        public bool Contains(object key)
        {
            return (_items as IDictionary).Contains(key);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return (_items as IDictionary).GetEnumerator();
        }

        public bool IsFixedSize => false;

        ICollection IDictionary.Keys => (_items as IDictionary).Keys;

        public void Remove(object key)
        {
            (_items as IDictionary).Remove(key);
            RaiseNotifyCollectionChanged();
        }

        ICollection IDictionary.Values => (_items as IDictionary).Values;

        public object this[object key]
        {
            get => this[(T)key];
            set => this[(T)key] = (TV)value;
        }

        public void CopyTo(Array array, int index)
        {
            (_items as IDictionary).CopyTo(array, index);
        }

        public bool IsSynchronized => false;

        public object SyncRoot { get; }
    }
}
