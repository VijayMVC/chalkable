using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;

namespace Chalkable.BusinessLogic.Services.Master
{
    public interface IPictureService
    {
        void UploadPicture(Guid id, byte[] content, int height, int width);
        void DeletePicture(Guid id, int height, int width);
    }

    public class PictureService : MasterServiceBase, IPictureService
    {
        public PictureService(IServiceLocatorMaster serviceLocator) : base(serviceLocator)
        {
        }

        private const string PICTURE_CONTEINER_NAME = "pictureconteiner";

        private string PictureName(Guid id, int height, int width)
        {
            return id + "-" + height + "x" + width;
        }
        public void UploadPicture(Guid id, byte[] content, int height, int width)
        {
            if(!BaseSecurity.HasChalkableRole(Context))
                throw new ChalkableSecurityException();

            content = ImageUtils.Scale(content, height, width);
            ServiceLocator.StorageBlobService.AddBlob(PICTURE_CONTEINER_NAME, PictureName(id, height, width), content);
        }

        public void DeletePicture(Guid id, int height, int width)
        {
            if (!BaseSecurity.HasChalkableRole(Context))
                throw new ChalkableSecurityException();
            ServiceLocator.StorageBlobService.DeleteBlob(PICTURE_CONTEINER_NAME, PictureName(id, height, width));
        }
    }
}
