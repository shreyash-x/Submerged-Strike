using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class Pool<T> where T : MonoBehaviour
{
    private readonly T _obj;
    private readonly Queue<T> _queue;

    private readonly List<T> _active = new List<T>();

    public event Action<T> Borrowed;
    public event Action<T> Returned;

    public IReadOnlyList<T> Active => _active;

    public Pool(T obj, int hint = 0)
    {
        _obj = obj;
        _queue = new Queue<T>();
        for (int i = 0; i < hint; i++)
        {
            var go = Object.Instantiate(obj);
            go.gameObject.SetActive(false);
            _queue.Enqueue(go);
        }
    }

    public T Borrow(bool setActive = true)
    {
        if (_queue.Count > 0)
        {
            var go = _queue.Dequeue();
            if(setActive) go.gameObject.SetActive(true);
            _active.Add(go);
            Borrowed?.Invoke(go);
            return go;
        }
        _obj.gameObject.SetActive(setActive);
        var obj = Object.Instantiate(_obj);
        _active.Add(obj);
        Borrowed?.Invoke(obj);
        return obj;
    }

    public void Return(T obj)
    {
        _queue.Enqueue(obj);
        obj.gameObject.SetActive(false);
        _active.Remove(obj);
        Returned?.Invoke(obj);
    }
}