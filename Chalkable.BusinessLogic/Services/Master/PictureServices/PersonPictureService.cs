using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;

namespace Chalkable.BusinessLogic.Services.Master.PictureServices
{
    public class PersonPictureService : PictureService
    {
        public PersonPictureService(IServiceLocatorMaster serviceLocator)
            : base(serviceLocator)
        {
            supportedSizes.Add(new PictureSize { Height = 24, Width = 24 });
            supportedSizes.Add(new PictureSize { Height = 40, Width = 40 });
            supportedSizes.Add(new PictureSize { Height = 47, Width = 47 });
            supportedSizes.Add(new PictureSize { Height = 64, Width = 64 });
            supportedSizes.Add(new PictureSize { Height = 128, Width = 128 });
            supportedSizes.Add(new PictureSize { Height = 256, Width = 256 });
        }

        public override void UploadPicture(Guid id, byte[] content)
        {
            if (!(BaseSecurity.IsAdminEditor(Context)
                || (Context.Role == CoreRoles.TEACHER_ROLE && Context.UserId == id)))
                throw new ChalkableSecurityException();
            base.UploadPicture(id, content);
        }
        public override void DeletePicture(Guid id)
        {
            if (!(BaseSecurity.IsAdminEditor(Context)
                || (Context.Role == CoreRoles.TEACHER_ROLE && Context.UserId == id)))
                throw new ChalkableSecurityException();
            base.DeletePicture(id);
        }
    }
}
