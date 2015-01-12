using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Storage;

namespace Chalkable.BusinessLogic.Services.Master.PictureServices
{
    public interface IPictureService
    {
        void UploadPicture(Guid id, byte[] content, int? width, int? height);
        void DeletePicture(Guid id, int? width, int? height);
        void UploadPicture(Guid id, byte[] content);
        void DeletePicture(Guid id);

        void UploadPicture(string name, byte[] content, int? height, int? width);
        void DeletePicture(string name, int? height, int? width);
        byte[] GetPicture(string name, int? height, int? width);
        void UploadPicture(string name, byte[] content);
        void DeletePicture(string name);
    }

    public class PictureService : MasterServiceBase, IPictureService
    {
        protected IList<PictureSize> SupportedSizes; 
        public PictureService(IServiceLocatorMaster serviceLocator) : base(serviceLocator)
        {
            SupportedSizes = new List<PictureSize>();
        }

        public static string GetPicturesRelativeAddress()
        {
            return (new BlobHelper()).GetBlobsRelativeAddress(PICTURE_CONTAINER_NAME);
        }

        public static string GeDemoPicturesRelativeAddress()
        {
            return (new BlobHelper()).GetBlobsRelativeAddress(DEMO_PICTURE_CONTAINER_NAME);
        }

        private const string PICTURE_CONTAINER_NAME = "pictureconteiner";
        private const string DEMO_PICTURE_CONTAINER_NAME = "demopicturecontainer";

        private string PictureName(string baseName, int? height, int? width)
        {
            var name = baseName;
            if (height.HasValue && width.HasValue)
                name += "-" + width + "x" + height;
            return name;
        }

        public virtual void UploadPicture(Guid id, byte[] content, int? width, int? height)
        {
            UploadPicture(id.ToString(), content, width, height);
        }

        public virtual void DeletePicture(Guid id, int? width, int? height)
        {
            DeletePicture(id.ToString(), width, height);
        }

        public virtual void UploadPicture(Guid id, byte[] content)
        {
            UploadPicture(id.ToString(), content);
        }

        public virtual void DeletePicture(Guid id)
        {
            DeletePicture(id.ToString());
        }

        public virtual void UploadPicture(string name, byte[] content, int? height, int? width)
        {
            if(!BaseSecurity.HasChalkableRole(Context))
                throw new ChalkableSecurityException();
            if (height.HasValue && width.HasValue)
                content = ImageUtils.Scale(content, width.Value, height.Value);

            ServiceLocator.StorageBlobService.AddBlob(PICTURE_CONTAINER_NAME, PictureName(name, height, width), content);
        }
        public virtual void DeletePicture(string name, int? height, int? width)
        {
            if (!BaseSecurity.HasChalkableRole(Context))
                throw new ChalkableSecurityException();
            ServiceLocator.StorageBlobService.DeleteBlob(PICTURE_CONTAINER_NAME, PictureName(name, height, width));
        }
        public byte[] GetPicture(string name, int? height, int? width)
        {
            return ServiceLocator.StorageBlobService.GetBlobContent(PICTURE_CONTAINER_NAME, PictureName(name, height, width));
        }

        protected class PictureSize
        {
            public int Height { get; set; }
            public int Width { get; set; }
        }
        public virtual void UploadPicture(string name, byte[] content)
        {
            ModifyPicture((h, w) => UploadPicture(name, content, h, w));
        }
        public virtual void DeletePicture(string name)
        {
            ModifyPicture((h, w) => DeletePicture(name, h, w));
        }
        private void ModifyPicture(Action<int?, int?> action)
        {
            foreach (var pictureSize in SupportedSizes)
            {
                action(pictureSize.Height, pictureSize.Width);
            }
            action(null, null);
        }
    }
}
