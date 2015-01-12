using System;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;

namespace Chalkable.BusinessLogic.Services.Master.PictureServices
{
    public interface IPersonPictureService
    {
        void UploadPicture(Guid districtId, int personId, byte[] content);
        void DeletePicture(Guid districtId, int personId);
    }

    public class PersonPictureService : PictureService, IPersonPictureService
    {
        public PersonPictureService(IServiceLocatorMaster serviceLocator)
            : base(serviceLocator)
        {
            SupportedSizes.Add(new PictureSize { Height = 24, Width = 24 });
            SupportedSizes.Add(new PictureSize { Height = 40, Width = 40 });
            SupportedSizes.Add(new PictureSize { Height = 47, Width = 47 });
            SupportedSizes.Add(new PictureSize { Height = 64, Width = 64 });
            SupportedSizes.Add(new PictureSize { Height = 70, Width = 70 });
            SupportedSizes.Add(new PictureSize { Height = 128, Width = 128 });
            SupportedSizes.Add(new PictureSize { Height = 256, Width = 256 });
        }

        public void UploadPicture(Guid districtId, int personId, byte[] content)
        {
            if (!(BaseSecurity.IsAdminEditor(Context)))
                throw new ChalkableSecurityException();
            base.UploadPicture(GenerateKeyForBlob(districtId, personId), content);
        }
        public void DeletePicture(Guid districtId, int personId)
        {
            if (!(BaseSecurity.IsAdminEditor(Context)))
                throw new ChalkableSecurityException();
            base.DeletePicture(GenerateKeyForBlob(districtId, personId));
        }

        private string GenerateKeyForBlob(Guid districtId, int personId)
        {
            return string.Format("{0}_{1}", districtId, personId);
        }
        
    }
}
