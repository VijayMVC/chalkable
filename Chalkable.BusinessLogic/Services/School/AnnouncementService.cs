using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IAnnouncementService
    {

    }

    public class AnnouncementService : SchoolServiceBase, IAnnouncementService
    {
        public AnnouncementService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }
    }
}
