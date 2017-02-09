using System;
using Chalkable.BusinessLogic.Security;

namespace Chalkable.BusinessLogic.Services.Master.PictureServices
{
    public class CustomReportTemplateIconService : PersonPictureService
    {
        public CustomReportTemplateIconService(IServiceLocatorMaster serviceLocator) : base(serviceLocator)
        {
            SupportedSizes.Add(new PictureSize { Height = 24, Width = 24 });
            SupportedSizes.Add(new PictureSize { Height = 64, Width = 64 });
            SupportedSizes.Add(new PictureSize { Height = 40, Width = 40 });
            SupportedSizes.Add(new PictureSize { Height = 47, Width = 47 });
        }

        public override void UploadPicture(Guid id, byte[] content)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            base.UploadPicture(id, content);
        }
        public override void DeletePicture(Guid id)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            base.DeletePicture(id);
        }
    }
}
