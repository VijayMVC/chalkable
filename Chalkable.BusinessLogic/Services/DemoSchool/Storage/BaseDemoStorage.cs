using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{

    public abstract class BaseDemoStorage<TKey, TValue> where TKey: struct where TValue:new()
    {
        protected DemoStorage Storage { get; private set; }

        protected Dictionary<TKey, TValue> data = new Dictionary<TKey, TValue>();

        protected TKey Index = default(TKey);
        private bool IsAutoIncrement;

        protected Func<TValue, TKey> KeyFieldAction { get; private set; }

        public bool IsEmpty()
        {
            return data.Count == 0;
        }

        public Dictionary<TKey, TValue> GetData()
        {
            return data;
        }


        public IList<TValue> Add(IList<TValue> items)
        {
            foreach (var item in items)
            {
                if (IsAutoIncrement)
                {
                    var key = GetNextFreeId();
                    data.Add(key, ModifyValue(key, item));
                }
                else
                {
                    var key = KeyFieldAction(item);
                    if (!data.ContainsKey(key))
                    {
                        data.Add(key, item);
                    }
                }
                
            }
            return items;
        }

        public TValue Add(TValue item)
        {
            return Add(new List<TValue> { item}).First();
        }

        public abstract TKey GetNextFreeId();

        public TValue ModifyValue(TKey key, TValue value, string defaultName = "Id")
        {
            var prop = value.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).FirstOrDefault(x => x.Name == defaultName);

            if (prop != null)
            {
                prop.SetValue(value, key);
            }
            return value;
        }


        protected BaseDemoStorage(DemoStorage storage, Func<TValue, TKey> keyField, bool autoIncrement = false)
        {
            Storage = storage;
            KeyFieldAction = keyField;
            IsAutoIncrement = autoIncrement;
        }
        
        public void Update(TValue item)
        {
            var key = KeyFieldAction(item);
            if (data.ContainsKey(key))
                data[key] = item;
        }

        public IList<TValue> Update(IList<TValue> items)
        {
            foreach (var item in items)
            {
                Update(item);
            }
            return items;
        }

        public TValue GetById(TKey id)
        {
            return data[id];
        }

        public void Delete(TKey id)
        {
            data.Remove(id);
        }

        public void Delete(TValue item)
        {
            data.Remove(KeyFieldAction(item));
        }

        public void Delete(IList<TValue> items)
        {
            foreach (var item in items)
            {
                Delete(item);
            }
        }

        public IList<TValue> GetAll()
        {
            return data.Select(x => x.Value).ToList();
        }

        public void Delete(IList<TKey> ids)
        {
            foreach (var id in ids)
            {
                Delete(id);
            }
        }

        

    }
}
