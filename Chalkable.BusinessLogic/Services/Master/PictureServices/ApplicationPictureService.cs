using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;

namespace Chalkable.BusinessLogic.Services.Master.PictureServices
{
    public class ApplicationPictureService : PictureService
    {
        public ApplicationPictureService(IServiceLocatorMaster serviceLocator) : base(serviceLocator)
        {
        }

        public override void UploadPicture(Guid id, byte[] content, int? width, int? height)
        {
            if (!ApplicationSecurity.CanUploadApplication(Context))
                throw new ChalkableSecurityException();
            if (content != null)
                base.UploadPicture(id, content, height, width);
        }

        public override void DeletePicture(Guid id, int? width, int? height)
        {
            if (!ApplicationSecurity.CanUploadApplication(Context))
                throw new ChalkableSecurityException();
            base.DeletePicture(id, height, width);
        }
    }
}
