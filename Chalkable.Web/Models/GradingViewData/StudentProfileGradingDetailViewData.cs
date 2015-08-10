using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models.GradingViewData
{
    public class StudentProfileGradingDetailViewData : StudentViewData
    {

        public GradingPeriodViewData CurrentGradingPeriod { get; set; }
        public IEnumerable<StudentAnnouncement> StudentAnnouncements { get; set; } 

        protected StudentProfileGradingDetailViewData(StudentDetails person)
            : base(person)
        {
        }


        public static StudentProfileGradingDetailViewData Create(StudentDetails student, StudentGradingDetails gradingDetails, GradingPeriod gp)
        {
            var res = new StudentProfileGradingDetailViewData(student)
            {
                CurrentGradingPeriod = GradingPeriodViewData.Create(gp),
                StudentAnnouncements = gradingDetails.StudentAnnouncements
            };

            return res;
        }
    }
}