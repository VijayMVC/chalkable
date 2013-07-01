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
    public interface IAvatarService
    {
        void UploadAvatar(Guid userId, byte[] content);
    }

    public  class AvatarService : MasterServiceBase, IAvatarService
    {
        private static IList<ComparablePair<int,int>> supportedAvatarSizes = new List<ComparablePair<int, int>>
            {
                new ComparablePair<int, int>(24, 24),
                new ComparablePair<int, int>(40, 40),
                new ComparablePair<int, int>(47, 47),
                new ComparablePair<int, int>(64, 64),
                new ComparablePair<int, int>(128, 128),
                new ComparablePair<int, int>(256, 256),
            }; 

        public AvatarService(IServiceLocatorMaster serviceLocator) : base(serviceLocator)
        {
        }

        public void UploadAvatar(Guid userId, byte[] content)
        {
            if(!(BaseSecurity.IsAdminEditor(Context) || (Context.Role == CoreRoles.TEACHER_ROLE && Context.UserId == userId)))
                throw new ChalkableSecurityException();

            foreach (var avatarSize in supportedAvatarSizes)
            {
                ServiceLocator.PictureService.UploadPicture(userId, content, avatarSize.First, avatarSize.Second);
            }
        }
    }
}
