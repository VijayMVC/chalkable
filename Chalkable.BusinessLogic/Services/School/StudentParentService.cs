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


        //TODO: needs tests 

        //private StudentParent CreateStudentParent(Guid studentId, Guid parentId)
        //{
        //    return new StudentParent
        //    {
        //        Id = Guid.NewGuid(),
        //        ParentRef = parentId,
        //        StudentRef = studentId
        //    };
        //}
        //private IList<StudentParent> CreateStudentParents(Guid studentId, IEnumerable<Guid> parentIds)
        //{
        //   return parentIds.Select(parentId => CreateStudentParent(studentId, parentId)).ToList();
        //}

        //public StudentParent Add(Guid studentId, Guid parentId)
        //{
        //    if(!BaseSecurity.IsAdminEditor(Context))
        //        throw new ChalkableSecurityException();

        //    using (var uow = Update())
        //    {
        //        var res = CreateStudentParent(studentId, parentId);
        //        new StudentParentDataAccess(uow).Insert(res);
        //        uow.Commit();
        //        return res;
        //    }
        //}

        //public void Delete(int studentParentId)
        //{
        //    if(!BaseSecurity.IsAdminEditor(Context))
        //        throw new ChalkableSecurityException();
        //    using (var uow = Update())
        //    {
        //        new StudentParentDataAccess(uow).Delete(studentParentId);
        //        uow.Commit();
        //    }
        //}

        //public IList<StudentParentDetails> GetParents(int studentId)
        //{
        //    using (var uow = Read())
        //    {
        //        return new StudentParentDataAccess(uow)
        //            .GetParents(studentId, Context.UserId, Context.Role.Id);
        //    }
        //}

        //public void SetParents(Guid studentId, IList<Guid> parentIds)
        //{
        //    if(!BaseSecurity.IsAdminEditorOrCurrentPerson(studentId, Context))
        //        throw new ChalkableSecurityException();

        //    using (var uow = Update())
        //    {
        //        var da = new StudentParentDataAccess(uow);
        //        da.Delete(new StudentParentQuery{StudentId = studentId});
        //        da.Insert(CreateStudentParents(studentId, parentIds));
        //        uow.Commit();
        //    }
        //}


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
