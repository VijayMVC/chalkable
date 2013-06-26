using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IAttendanceService
    {
        
    }

    public class AttendanceService : SchoolServiceBase, IAttendanceService
    {
        public AttendanceService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

    }
}
