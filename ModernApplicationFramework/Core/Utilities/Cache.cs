﻿using System;

namespace ModernApplicationFramework.Core.Utilities
{
    internal class Cache<TKey, TObject>
    {
        private readonly Action<TObject> _objectRemovedCallback;
        private Tuple<TKey, TObject>[] _cache;
        private Func<TKey, TObject> _creationCallback;
        private Tuple<TKey, TObject> _permanentMember;

        public Cache(int cacheSize, Func<TKey, TObject> objectCreationCallback)
        {
            CommonInit(cacheSize, objectCreationCallback);
        }

        public Cache(int cacheSize, Func<TKey, TObject> objectCreationCallback, Action<TObject> objectRemovedCallback)
        {
            CommonInit(cacheSize, objectCreationCallback);
            _objectRemovedCallback = objectRemovedCallback ?? throw new ArgumentNullException(nameof(objectRemovedCallback));
        }

        public void Clear()
        {
            for (var index = 0; index < _cache.Length; ++index)
            {
                CallItemRemovedCallback(_cache[index]);
                _cache[index] = null;
            }
            CallItemRemovedCallback(_permanentMember);
            _permanentMember = null;
        }

        public TObject GetItem(TKey key)
        {
            var index1 = 0;
            while (index1 < _cache.Length && _cache[index1] != null && !Equals(_cache[index1].Item1, key))
                ++index1;
            if (index1 != _cache.Length)
            {
                var tuple = _cache[index1];
                if (tuple == null)
                {
                    var @object = _creationCallback(key);
                    tuple = new Tuple<TKey, TObject>(key, @object);
                }
                for (var index2 = index1; index2 > 0; --index2)
                    _cache[index2] = _cache[index2 - 1];
                _cache[0] = tuple;
                return tuple.Item2;
            }
            var object1 = _creationCallback(key);
            CallItemRemovedCallback(_cache[_cache.Length - 1]);
            for (var index2 = _cache.Length - 1; index2 > 0; --index2)
                _cache[index2] = _cache[index2 - 1];
            _cache[0] = new Tuple<TKey, TObject>(key, object1);
            return object1;
        }

        private void CallItemRemovedCallback(Tuple<TKey, TObject> removedItem)
        {
            if (removedItem == null || removedItem.Item2 == null || _objectRemovedCallback == null)
                return;
            _objectRemovedCallback(removedItem.Item2);
        }

        private void CommonInit(int cacheSize, Func<TKey, TObject> objectCreationCallback)
        {
            if (cacheSize <= 0)
                throw new ArgumentException("Cache Size must be greater than zero");
            _cache = new Tuple<TKey, TObject>[cacheSize];
            _creationCallback = objectCreationCallback ?? throw new ArgumentNullException(nameof(objectCreationCallback));
        }
    }
}