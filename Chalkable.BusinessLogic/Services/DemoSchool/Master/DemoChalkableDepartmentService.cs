using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Master
{
    public class DemoChalkableDepartmentService : DemoMasterServiceBase, IChalkableDepartmentService
    {
        public DemoChalkableDepartmentService(IServiceLocatorMaster serviceLocator, DemoStorage storage) : base(serviceLocator, storage)
        {
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

            Storage.ChalkableDepartmentStorage.Add(res);
            ServiceLocator.DepartmentIconService.UploadPicture(res.Id, icon);
            return res;
        }

        public ChalkableDepartment Edit(Guid id, string name, IList<string> keywords, byte[] icon)
        {
            if (!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();

            var res = Storage.ChalkableDepartmentStorage.GetById(id);
            res.Keywords = keywords.JoinString(",");
            res.Name = name;
            Storage.ChalkableDepartmentStorage.Update(res);
            ServiceLocator.DepartmentIconService.UploadPicture(id, icon);
            return res;
        }

        public void Delete(Guid id)
        {
            if (!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();
            Storage.ChalkableDepartmentStorage.Delete(id);
            ServiceLocator.DepartmentIconService.DeletePicture(id);
        }

        public IList<ChalkableDepartment> GetChalkableDepartments()
        {
            return Storage.ChalkableDepartmentStorage.GetAll();
        }
        public ChalkableDepartment GetChalkableDepartmentById(Guid id)
        {
            return Storage.ChalkableDepartmentStorage.GetByIdOrNull(id);
        }
    }
}