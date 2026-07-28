using System.Collections.Generic;
using System.Linq;

namespace ModbusPilot.Core.Utils
{
    public class CircularBuffer<T>
    {
        private readonly Queue<T> _queue;
        private readonly int _capacity;
        private readonly object _lock = new object(); // 新增一把锁

        public CircularBuffer(int capacity)
        {
            _capacity = capacity;
            _queue = new Queue<T>(capacity);
        }

        public void Add(T item)
        {
            lock (_lock) // 写入时加锁
            {
                if (_queue.Count >= _capacity)
                    _queue.Dequeue();
                _queue.Enqueue(item);
            }
        }

        public int Count { get { lock (_lock) return _queue.Count; } }
        // 关键：导出或绘图时，先拷贝一份快照，防止遍历时被修改
        public T[] ToArray()
        {
            lock (_lock)
            {
                return _queue.ToArray();
            }
        }
        public IEnumerable<T> GetAll() => _queue;
        public void Clear()
        {
            lock (_lock)
            {
                _queue.Clear();
            }
        }
        public T LastOrDefault() => _queue.LastOrDefault();
    }
}
