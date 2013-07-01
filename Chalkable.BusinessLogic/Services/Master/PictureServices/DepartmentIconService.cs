using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;

namespace Chalkable.BusinessLogic.Services.Master.PictureServices
{
    public class DepartmentService : PictureService
    {
        public DepartmentService(IServiceLocatorMaster serviceLocator)
            : base(serviceLocator)
        {
        }
        public override void UploadPicture(Guid id, byte[] content)
        {
            if (!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();
            base.UploadPicture(id, content);
        }
        public override void DeletePicture(Guid id)
        {
            if (!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();
            base.DeletePicture(id);
        }
    }
}
