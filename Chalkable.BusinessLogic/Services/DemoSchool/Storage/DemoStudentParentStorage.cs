using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoStudentParentStorage:BaseDemoStorage<int, StudentParent>
    {
        private int index = 0;

        public DemoStudentParentStorage(DemoStorage storage) : base(storage)
        {
        }

        public StudentParent Add(int studentId, int parentId)
        {
            var studentParent = new StudentParent
            {
                ParentRef = parentId,
                StudentRef = studentId
            };
 
            data.Add(index++, studentParent);
            return studentParent;
        }

        public IList<StudentParentDetails> GetParents(int studentId)
        {
            var studentParents = data.Where(x => x.Value.StudentRef == studentId).Select(x => x.Value).ToList();


            var studentParentDetailsList = new List<StudentParentDetails>();


            foreach (var studentParent in studentParents)
            {
                var studentParentDetails = new StudentParentDetails
                {
                    Id = studentParent.Id,
                    ParentRef = studentParent.ParentRef,
                    StudentRef = studentParent.StudentRef
                };



                //studentParentDetails.Parent = Storage.PersonStorage.GetPersonDetails(studentParentDetails.ParentRef);
                studentParentDetailsList.Add(studentParentDetails);
            }
            return studentParentDetailsList;
        }

        public void SetParents(int studentId, IList<int> parentIds)
        {

            throw new System.NotImplementedException();
        }
    }
}
