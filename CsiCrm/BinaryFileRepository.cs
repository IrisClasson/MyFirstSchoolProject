using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace CsiCrm
{
    internal class BinaryFileRepository<T> : IRepository<T>
    {
        private readonly string _path;
        private List<T> _items;

        public BinaryFileRepository(string path)
        {
            _path = path;
        }

        public void Load()
        {
            var formatter = new BinaryFormatter();
            using (var input = File.Open(_path, FileMode.OpenOrCreate))
            {
                if (input.Length > 0)
                    _items = (List<T>)formatter.Deserialize(input);
            }
        }

        public void Save()
        {
            var formatter = new BinaryFormatter();
            using (Stream output = File.Create(_path))
            {
                formatter.Serialize(output, _items);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        public void Add(T item)
        {
            _items.Add(item);
        }

        public bool Remove(T item)
        {
            return _items.Remove(item);
        }
    }
}
