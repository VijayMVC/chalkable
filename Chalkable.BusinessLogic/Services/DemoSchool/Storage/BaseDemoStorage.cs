using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{



    public class BaseDemoStorage<T, U> where U:new()
    {
        protected DemoStorage Storage { get; private set; }

        protected Dictionary<T, U> data = new Dictionary<T, U>(); 

        public BaseDemoStorage(DemoStorage storage)
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

        public void Init()
        {
            //some init data
        }


    }
}
