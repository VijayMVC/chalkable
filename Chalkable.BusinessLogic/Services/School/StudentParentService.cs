using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IStudentParentService
    {
        StudentParent Add(int studentId, int parentId);
        void Delete(int studentParentId);
        IList<StudentParentDetails> GetParents(int studentId);
        void SetParents(int studentId, IList<int> parentIds);
        //IList<Person> ListPossibleParents(Guid studentId);
        
    }
    public class StudentParentService : SchoolServiceBase, IStudentParentService
    {
        public StudentParentService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public StudentParent Add(int studentId, int parentId)
        {
            throw new NotImplementedException();
        }

        public void Delete(int studentParentId)
        {
            throw new NotImplementedException();
        }

        public IList<StudentParentDetails> GetParents(int studentId)
        {
            throw new NotImplementedException();
        }

        public void SetParents(int studentId, IList<int> parentIds)
        {
            throw new NotImplementedException();
        }
    }
}
