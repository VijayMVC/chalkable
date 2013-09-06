using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Storage;

namespace Chalkable.BusinessLogic.Services.Master.PictureServices
{
    public interface IPictureService
    {
        void UploadPicture(Guid id, byte[] content, int? height, int? width);
        void DeletePicture(Guid id, int? height, int? width);
        byte[] GetPicture(Guid id, int? height, int? width);
        void UploadPicture(Guid id, byte[] content);
        void DeletePicture(Guid id);
    }

    public class PictureService : MasterServiceBase, IPictureService
    {
        protected IList<PictureSize> supportedSizes; 
        public PictureService(IServiceLocatorMaster serviceLocator) : base(serviceLocator)
        {
            supportedSizes = new List<PictureSize>();
        }

        public static string GetPicturesRelativeAddress()
        {
            return (new BlobHelper()).GetBlobsRelativeAddress(PICTURE_CONTAINER_NAME);
        }

        private const string PICTURE_CONTAINER_NAME = "pictureconteiner";
        private string PictureName(Guid id, int? height, int? width)
        {
            var name = id.ToString();
            if (height.HasValue && width.HasValue)
                name += "-" + height + "x" + width;
            return name;
        }
        public virtual void UploadPicture(Guid id, byte[] content, int? height, int? width)
        {
            if(!BaseSecurity.HasChalkableRole(Context))
                throw new ChalkableSecurityException();
            if (height.HasValue && width.HasValue)
                content = ImageUtils.Scale(content, height.Value, width.Value);

            ServiceLocator.StorageBlobService.AddBlob(PICTURE_CONTAINER_NAME, PictureName(id, height, width), content);
        }
        public virtual void DeletePicture(Guid id, int? height, int? width)
        {
            if (!BaseSecurity.HasChalkableRole(Context))
                throw new ChalkableSecurityException();
            ServiceLocator.StorageBlobService.DeleteBlob(PICTURE_CONTAINER_NAME, PictureName(id, height, width));
        }
        public byte[] GetPicture(Guid id, int? height, int? width)
        {
            return ServiceLocator.StorageBlobService.GetBlobContent(PICTURE_CONTAINER_NAME, PictureName(id, height, width));
        }

        protected class PictureSize
        {
            public int Height { get; set; }
            public int Width { get; set; }
        }
        public virtual void UploadPicture(Guid id, byte[] content)
        {
            ModifyPicture((h, w) => UploadPicture(id, content, h, w));
        }
        public virtual void DeletePicture(Guid id)
        {
            ModifyPicture((h, w) => DeletePicture(id, h, w));
        }
        private void ModifyPicture(Action<int?, int?> action)
        {
            foreach (var pictureSize in supportedSizes)
            {
                action(pictureSize.Height, pictureSize.Width);
            }
            action(null, null);
        }
    }
}
