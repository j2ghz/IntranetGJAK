﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetGJAK.Models
{
    public class File
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
    }

    public interface IFileRepository
    {
        void Add(File item);

        IEnumerable<File> GetAll();

        File Find(string key);

        File Remove(string key);

        void Update(File item);
    }

    public class FileRepository : IFileRepository
    {
        private static ConcurrentDictionary<string, File> _Files = new ConcurrentDictionary<string, File>();

        public FileRepository()
        {
            Add(new File { Name = "Item1" });
        }

        public IEnumerable<File> GetAll()
        {
            return _Files.Values;
        }

        public void Add(File item)
        {
            item.Key = Guid.NewGuid().ToString();
            _Files[item.Key] = item;
        }

        public File Find(string key)
        {
            File item;
            _Files.TryGetValue(key, out item);
            return item;
        }

        public File Remove(string key)
        {
            File item;
            _Files.TryGetValue(key, out item);
            _Files.TryRemove(key, out item);
            return item;
        }

        public void Update(File item)
        {
            _Files[item.Key] = item;
        }
    }
}