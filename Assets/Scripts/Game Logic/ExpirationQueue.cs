using UnityEngine;
using System.Collections.Generic;

public class ExpirationQueue<T>
{
    public struct ExpirationQueueElement
    {
        public T element;
        public float timestamp;
        public ExpirationQueueElement(T element, float timestamp)
        {
            this.element = element;
            this.timestamp = timestamp;
        }
    }

    private float _expiration_time;

    private Queue<ExpirationQueueElement> _contents = new Queue<ExpirationQueueElement>();

    public ExpirationQueue(float expiration_time)
    {
        this._expiration_time = expiration_time;
    }

    public void Add(T element)
    {
        _contents.Enqueue(new ExpirationQueueElement(element, Time.time));
    }

    public Queue<ExpirationQueueElement> GetContents()
    {
        return _contents;
    }

    public void Update()
    {
        if (_contents.Count > 0 && (Time.time > _contents.Peek().timestamp + _expiration_time))
            _contents.Dequeue();
    }
}