using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoGroupStorage : BaseDemoIntStorage<Group>
    {
        public DemoGroupStorage()
            : base(x => x.Id, true)
        {
        }
    }

    public class DemoStudentGroupStorage : BaseDemoIntStorage<StudentGroup>
    {
        public DemoStudentGroupStorage()
            : base(x => x.GroupRef)
        {
        }
    }


    public class DemoGroupService : DemoSchoolServiceBase, IGroupService
    {
        public DemoGroupService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public Group AddGroup(string name, IntList studentsIds)
        {
            throw new NotImplementedException();
        }

        public Group EditGroup(int groupId, string name, IntList studentsIds)
        {
            throw new NotImplementedException();
        }

        public void DeleteGroup(int groupId)
        {
            throw new NotImplementedException();
        }

        public IList<Group> GetGroups(int ownerId, string filter)
        {
            throw new NotImplementedException();
        }

        public GroupInfo Info(int groupId)
        {
            throw new NotImplementedException();
        }
    }
}
