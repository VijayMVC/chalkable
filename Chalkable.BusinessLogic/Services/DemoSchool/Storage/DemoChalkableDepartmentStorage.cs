using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.Common;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoChalkableDepartmentStorage
    {
        private Dictionary<Guid, ChalkableDepartment> chalkableDepartmentData = new Dictionary<Guid, ChalkableDepartment>(); 
        public void Add(ChalkableDepartment res)
        {
            if (!chalkableDepartmentData.ContainsKey(res.Id))
                chalkableDepartmentData[res.Id] = res;
        }

        public ChalkableDepartment GetById(Guid id)
        {
            return chalkableDepartmentData[id];
        }

        public void Update(ChalkableDepartment res)
        {
            if (chalkableDepartmentData.ContainsKey(res.Id))
                chalkableDepartmentData[res.Id] = res;
        }

        public void Delete(Guid id)
        {
            chalkableDepartmentData.Remove(id);
        }

        public IList<ChalkableDepartment> GetAll()
        {
            return chalkableDepartmentData.Select(x => x.Value).ToList();
        }

        public ChalkableDepartment GetByIdOrNull(Guid id)
        {
            return chalkableDepartmentData.ContainsKey(id) ? chalkableDepartmentData[id] : null;
        }
    }
}
