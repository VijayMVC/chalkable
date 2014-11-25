using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Services.Master
{
    public interface IChalkableDepartmentService
    {
        ChalkableDepartment Add(string name, IList<string> keywords, byte[] icon);
        ChalkableDepartment Edit(Guid id, string name, IList<string> keywords, byte[] icon);
        void Delete(Guid id);

        IList<ChalkableDepartment> GetChalkableDepartments();
        ChalkableDepartment GetChalkableDepartmentById(Guid id);
    }

    public class ChalkableDepartmentService : MasterServiceBase, IChalkableDepartmentService
    {
        public ChalkableDepartmentService(IServiceLocatorMaster serviceLocator) : base(serviceLocator)
        {
        }


        public ChalkableDepartment Add(string name, IList<string> keywords, byte[] icon)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            var res = new ChalkableDepartment
            {
                Id = Guid.NewGuid(),
                Keywords = keywords.JoinString(","),
                Name = name
            };
            using (var uow = Update())
            {
                new ChalkableDepartmentDataAccess(uow).Insert(res);
                ServiceLocator.DepartmentIconService.UploadPicture(res.Id, icon);
                uow.Commit();
            }
            return res;
        }

        public ChalkableDepartment Edit(Guid id, string name, IList<string> keywords, byte[] icon)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            using (var uow = Update())
            {
                var da = new ChalkableDepartmentDataAccess(uow);
                var res = da.GetById(id);
                res.Keywords = keywords.JoinString(",");
                res.Name = name;
                da.Update(res);
                ServiceLocator.DepartmentIconService.UploadPicture(id, icon);
                uow.Commit();
                return res;
            }
        }

        public void Delete(Guid id)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            using (var uow = Update())
            {
                new ChalkableDepartmentDataAccess(uow).Delete(id);
                ServiceLocator.DepartmentIconService.DeletePicture(id);
                uow.Commit();
            }
        }
        
        public IList<ChalkableDepartment> GetChalkableDepartments()
        {
            return DoRead(u => new ChalkableDepartmentDataAccess(u).GetAll());
        }

        public ChalkableDepartment GetChalkableDepartmentById(Guid id)
        {
            return DoRead(u => new ChalkableDepartmentDataAccess(u).GetByIdOrNull(id));
        }
    }
}