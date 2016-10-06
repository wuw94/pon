using UnityEngine;
using System.Collections.Generic;

public class ExpirationStack<T>
{
    public struct StackContent
    {
        public T element;
        public float timestamp;
        public StackContent(T element, float timestamp)
        {
            this.element = element;
            this.timestamp = timestamp;
        }
    }

    private float _expiration_time;

    private Queue<StackContent> _contents = new Queue<StackContent>();

    public ExpirationStack(float expiration_time)
    {
        this._expiration_time = expiration_time;
    }

    public void Add(T element)
    {
        _contents.Enqueue(new StackContent(element, Time.time));
    }

    public Queue<StackContent> GetContents()
    {
        return _contents;
    }

    public void Update()
    {
        if (_contents.Count > 0 && (Time.time > _contents.Peek().timestamp + _expiration_time))
            _contents.Dequeue();
    }
}