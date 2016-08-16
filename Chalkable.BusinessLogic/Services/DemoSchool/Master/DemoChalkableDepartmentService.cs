using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Master
{
    public class DemoChalkableDepartmentStorage : BaseDemoGuidStorage<ChalkableDepartment>
    {
        public DemoChalkableDepartmentStorage()
            : base(x => x.Id)
        {
        }

        public ChalkableDepartment GetByIdOrNull(Guid id)
        {
            return data.ContainsKey(id) ? data[id] : null;
        }
    }

    public class DemoChalkableDepartmentService : DemoMasterServiceBase, IChalkableDepartmentService
    {
        private DemoChalkableDepartmentStorage ChalkableDepartmentStorage { get; set; }
        public DemoChalkableDepartmentService(IServiceLocatorMaster serviceLocator) : base(serviceLocator)
        {
            ChalkableDepartmentStorage = new DemoChalkableDepartmentStorage();
        }

        public ChalkableDepartment Add(string name, IList<string> keywords, byte[] icon)
        {
            if(!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();

            var res = new ChalkableDepartment
            {
                Id = Guid.NewGuid(),
                Keywords = keywords.JoinString(","),
                Name = name
            };

            ChalkableDepartmentStorage.Add(res);
            ServiceLocator.DepartmentIconService.UploadPicture(res.Id, icon);
            return res;
        }

        public ChalkableDepartment Edit(Guid id, string name, IList<string> keywords, byte[] icon)
        {
            if (!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();

            var res = ChalkableDepartmentStorage.GetById(id);
            res.Keywords = keywords.JoinString(",");
            res.Name = name;
            ChalkableDepartmentStorage.Update(res);
            ServiceLocator.DepartmentIconService.UploadPicture(id, icon);
            return res;
        }

        public void Delete(Guid id)
        {
            if (!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();
            ChalkableDepartmentStorage.Delete(id);
            ServiceLocator.DepartmentIconService.DeletePicture(id);
        }

        public IList<ChalkableDepartment> GetChalkableDepartments()
        {
            return ChalkableDepartmentStorage.GetAll();
        }
        public ChalkableDepartment GetChalkableDepartmentById(Guid id)
        {
            return ChalkableDepartmentStorage.GetByIdOrNull(id);
        }
    }
}