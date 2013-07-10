using System;
using Chalkable.Data.School.Model;
using NUnit.Framework;

namespace Chalkable.Tests.Services.School
{
    public class StudentAnnouncementServiceTest : BaseSchoolServiceTest
    {
        [Test]
        public void GetSetStudentAnnouncementTest()
        {
            var context = SchoolTestContext;
            var type = context.FirstTeacherSl.AnnouncementService.GetAnnouncementTypeBySystemType(SystemAnnouncementType.Standard);
            var mathClass = ClassServiceTest.CreateClass(context, context.FirstTeacher, context.FirstStudent, context.SecondStudent);

            var announcement = context.FirstTeacherSl.AnnouncementService.CreateAnnouncement(type.Id);
            var mpId = mathClass.MarkingPeriodClass[0].MarkingPeriodRef;
            context.FirstTeacherSl.AnnouncementService.SubmitAnnouncement(announcement.Id, mathClass.Id, mpId);

            announcement = context.FirstTeacherSl.AnnouncementService.GetAnnouncementDetails(announcement.Id);
            var stAnns = announcement.StudentAnnouncements;
            var sa1 = stAnns[0];
            AssertForDeny(sl=> sl.StudentAnnouncementService.SetGrade(sa1.Id, 20, "testCredits", "test", false, GradingStyleEnum.Numeric100)
                , SchoolTestContext, SchoolContextRoles.SecondTeacher | SchoolContextRoles.FirstStudent | SchoolContextRoles.SecondStudent
                | SchoolContextRoles.FirstParent | SchoolContextRoles.Checkin);

            var gradingStyle = GradingStyleEnum.Numeric100;
            var gradeValue = context.AdminGradeSl.GradingStyleService.GetMapper().MapBack(gradingStyle, 20);
            var sa = context.FirstTeacherSl.StudentAnnouncementService.SetGrade(sa1.Id, 20, "testCredits", "test", false, gradingStyle);
            Assert.AreEqual(sa.Id, sa1.Id);
            Assert.AreEqual(sa.GradeValue, gradeValue);
            Assert.AreEqual(sa.ExtraCredit, "testCredits");
            Assert.AreEqual(sa.Comment, "test");
            Assert.AreEqual(sa.Dropped, false);
            Assert.AreEqual(sa.State, StudentAnnouncementStateEnum.Manual);
            var stAnns2 = context.FirstTeacherSl.StudentAnnouncementService.GetStudentAnnouncements(sa.AnnouncementRef);
            AssertAreEqual(sa, stAnns2[0]);
            Assert.AreEqual(stAnns2.Count, mathClass.StudentsCount);
            sa = context.FirstTeacherSl.StudentAnnouncementService.SetGrade(sa1.Id, 20, "testCredits", "test", true, gradingStyle);
            Assert.AreEqual(sa.Dropped, true);
            stAnns2 = context.FirstTeacherSl.StudentAnnouncementService.GetStudentAnnouncements(sa.AnnouncementRef);
            foreach (var studentAnnouncementDetailse in stAnns2)
            {
                context.FirstTeacherSl.StudentAnnouncementService.SetGrade(studentAnnouncementDetailse.Id, 20, "testCredits", "test", true, gradingStyle);
            }
            announcement = context.FirstTeacherSl.AnnouncementService.GetAnnouncementDetails(announcement.Id);
            Assert.AreEqual(announcement.Dropped, true);
            

        }
    }
}
