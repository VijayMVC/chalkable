using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{



    public abstract class BaseDemoStorage<T, U> where U:new()
    {
        protected DemoStorage Storage { get; private set; }

        protected Dictionary<T, U> data = new Dictionary<T, U>();

        protected int index = 0;

        public bool IsEmpty()
        {
            return data.Count == 0;
        }

        public Dictionary<T, U> GetData()
        {
            return data;
        } 


        public int GetNextFreeId()
        {
            int res = index;
            ++index;
            return res;
        }

        protected BaseDemoStorage(DemoStorage storage)
        {
            Storage = storage;
        }

        public U GetById(T id)
        {
            return data[id];
        }

        public void Delete(T id)
        {
            data.Remove(id);
        }
        
        public IList<U> GetAll()
        {
            return data.Select(x => x.Value).ToList();
        }

        public void Delete(IList<T> ids)
        {
            foreach (var id in ids)
            {
                Delete(id);
            }
        }

        public abstract void Setup();

    }
}
