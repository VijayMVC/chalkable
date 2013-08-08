using System;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;

namespace Chalkable.BusinessLogic.Services.Master.PictureServices
{
    public class FundRequestPictureService : PictureService
    {
        public FundRequestPictureService(IServiceLocatorMaster serviceLocator)
            : base(serviceLocator)
        {
        }
        public override void UploadPicture(Guid id, byte[] content)
        {
            if (!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();
            if (content != null)
                base.UploadPicture(id, content);
            //TODO: if content null download default icon
        }

        public override void DeletePicture(Guid id)
        {
            throw new NotSupportedException();
        }
    }
}