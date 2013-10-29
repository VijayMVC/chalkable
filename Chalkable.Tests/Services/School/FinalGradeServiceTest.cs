//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Chalkable.Data.School.Model;
//using Chalkable.Tests.Services.TestContext;
//using NUnit.Framework;

//namespace Chalkable.Tests.Services.School
//{
//    public class FinalGradeServiceTest : BaseSchoolServiceTest
//    {
//        private FinalGradeDetails UpdateFinalGrade(Guid finalGradeId, SchoolTestContext context, out int percentmode, 
//            out int percentByType, bool dropLowest, GradingStyleEnum gradingStyle = GradingStyleEnum.Numeric100, 
//            GradingStyleEnum annTypeGradingStyle = GradingStyleEnum.Numeric100)
//        {
//            var finalGradeAnnTypes = context.FirstTeacherSl.FinalGradeService.GetFinalGradeAnnouncementTypes(finalGradeId);
//            percentByType = 0;
//            if (finalGradeAnnTypes != null && finalGradeAnnTypes.Count != 0)
//            {
//                percentByType = 50 / finalGradeAnnTypes.Count;
//                percentmode = 50 % finalGradeAnnTypes.Count;
//                foreach (var finalGradeAnnType in finalGradeAnnTypes)
//                {
//                    finalGradeAnnType.PercentValue = percentByType;
//                    finalGradeAnnType.DropLowest = dropLowest;
//                    finalGradeAnnType.GradingStyle = annTypeGradingStyle;
//                }
//                return context.FirstTeacherSl.FinalGradeService.Update(finalGradeId, 20 + percentmode, 20, false, 10, 
//                    true, gradingStyle,  finalGradeAnnTypes);
//            }
//            percentmode = 0;
//            return null;
//        }


//        private void CheckFinalGradeAnnTypesEquality(FinalGradeDetails finalGrade1, FinalGradeDetails finalGrade2, Action<object, object> equalityAction)
//        {
//            Assert.AreEqual(finalGrade1.FinalGradeAnnouncementTypes.Count, finalGrade2.FinalGradeAnnouncementTypes.Count);
//            var fg1At = finalGrade1.FinalGradeAnnouncementTypes.OrderBy(x => x.AnnouncementTypeRef).ToList();
//            var fg2At = finalGrade2.FinalGradeAnnouncementTypes.OrderBy(x => x.AnnouncementTypeRef).ToList();
//            for (int i = 0; i < fg1At.Count; i++)
//            {
//                Assert.AreEqual(fg1At[i].AnnouncementTypeRef, fg2At[i].AnnouncementTypeRef);
//                equalityAction(fg1At[i].PercentValue, fg2At[i].PercentValue);
//                equalityAction(fg1At[i].GradingStyle, fg2At[i].GradingStyle);
//                equalityAction(fg1At[i].DropLowest, fg2At[i].DropLowest);
//            }
//        }

//        private void CheckFinalGradeEquality(FinalGradeDetails finalGrade1, FinalGradeDetails finalGrade2, Action<object, object> equalityAction)
//        {
//            equalityAction(finalGrade1.Discipline, finalGrade2.Discipline);
//            equalityAction(finalGrade1.Attendance, finalGrade2.Attendance);
//            equalityAction(finalGrade1.ParticipationPercent, finalGrade2.ParticipationPercent);
//            CheckFinalGradeAnnTypesEquality(finalGrade1, finalGrade2, equalityAction);
//        }

//        [Test]
//        public void GetBuildFinalGradesTest()
//        {
//            var mathClass1 = ClassServiceTest.CreateClass(SchoolTestContext, SchoolTestContext.FirstTeacher,
//                                                         SchoolTestContext.FirstStudent, null);
//            var mpc1 = mathClass1.MarkingPeriodClasses[0];
//            var mpId = mpc1.MarkingPeriodRef;
//            var an1 = AnnouncementServiceTest.CreateDefaulTeacherAnnouncement(SchoolTestContext.FirstTeacherSl, mathClass1.Id, mpId, null, SystemAnnouncementType.HW);


//            var item1 = an1.StudentAnnouncements.First();
//            SchoolTestContext.FirstTeacherSl.StudentAnnouncementService.SetGrade(item1.Id, 70, "0", null, false);

//            AssertForDeny(sl => sl.FinalGradeService.GetFinalGrade(mpc1.Id, true), SchoolTestContext
//                , SchoolContextRoles.FirstParent | SchoolContextRoles.FirstStudent | SchoolContextRoles.Checkin
//                | SchoolContextRoles.SecondTeacher);

//            var finalGradeService = SchoolTestContext.FirstTeacherSl.FinalGradeService;
//            var finalGrade = finalGradeService.GetFinalGrade(mpc1.Id, true);

//            Assert.AreEqual(finalGrade.Id, mpc1.Id);
//            Assert.AreEqual(finalGradeService.GetPaginatedFinalGrades(0).Count, 1);
            
//            AssertForDeny(sl => sl.FinalGradeService.ReBuildFinalGrade(finalGrade.Id), SchoolTestContext
//                , SchoolContextRoles.FirstParent | SchoolContextRoles.FirstStudent
//             | SchoolContextRoles.SecondStudent | SchoolContextRoles.Checkin);

//            int percentmode, percentByType;
//            finalGrade = UpdateFinalGrade(finalGrade.Id, SchoolTestContext, out percentmode, out percentByType, true, GradingStyleEnum.Abcf);
//            var finalGrade2 = finalGradeService.ReBuildFinalGrade(finalGrade.Id);
//            Assert.AreNotEqual(finalGrade2.GradingStyle, finalGrade.GradingStyle);
//            Assert.AreNotEqual(finalGrade2.ParticipationPercent, finalGrade.ParticipationPercent);
//            Assert.AreNotEqual(finalGrade2.Discipline, finalGrade.Discipline);
//            Assert.AreNotEqual(finalGrade2.Attendance, finalGrade.Attendance);
//            finalGrade = UpdateFinalGrade(finalGrade2.Id, SchoolTestContext, out percentmode, out percentByType, true, GradingStyleEnum.Abcf, GradingStyleEnum.Abcf);
            
//            var mathClass2 = ClassServiceTest.CreateClass(SchoolTestContext, SchoolTestContext.FirstTeacher,
//                             SchoolTestContext.FirstStudent, null, "math2", mathClass1.SchoolYearRef);
//            var fg3 = finalGradeService.GetFinalGrade(mathClass2.MarkingPeriodClasses[0].Id);
//            CheckFinalGradeEquality(fg3, finalGrade, Assert.AreEqual);
            
//            var mathClass3 = ClassServiceTest.CreateClass(SchoolTestContext, SchoolTestContext.SecondTeahcer,
//                             SchoolTestContext.FirstStudent, null, "math3", mathClass1.SchoolYearRef);
//            var fg4 = SchoolTestContext.SecondTeacherSl.FinalGradeService.GetFinalGrade(mathClass3.MarkingPeriodClasses[0].Id);
//            CheckFinalGradeEquality(fg4, finalGrade, Assert.AreNotEqual);

//            //check calculate grade by ann 
//            var a1 = AnnouncementServiceTest.CreateDefaulTeacherAnnouncement(SchoolTestContext.FirstTeacherSl, mathClass1.Id, mpId, null,  SystemAnnouncementType.Project);
//            var stA1 = SchoolTestContext.FirstTeacherSl.StudentAnnouncementService.SetGrade(a1.StudentAnnouncements.First().Id, 40, null, null, false);
//            var a3 = AnnouncementServiceTest.CreateDefaulTeacherAnnouncement(SchoolTestContext.FirstTeacherSl, mathClass1.Id, mpId, null,  SystemAnnouncementType.Midterm);
//            SchoolTestContext.FirstTeacherSl.StudentAnnouncementService.SetGrade(a3.StudentAnnouncements.First().Id, 60, null, null, false);

//            foreach (var type in finalGrade.FinalGradeAnnouncementTypes)
//            {
//                type.PercentValue = 0;
//                var sysType = type.AnnouncementType.SystemType;
//                if (sysType == SystemAnnouncementType.HW)
//                    type.PercentValue = 20;
//                if (sysType == SystemAnnouncementType.Final)
//                    type.PercentValue = 20;
//                if (sysType == SystemAnnouncementType.Project)
//                    type.PercentValue = 40;
//                if (sysType == SystemAnnouncementType.Midterm)
//                    type.PercentValue = 20;
//            }

//            finalGradeService.Update(finalGrade.Id, 0, 0, false, 0, false, GradingStyleEnum.Numeric100, finalGrade.FinalGradeAnnouncementTypes);
//            finalGrade = finalGradeService.GetFinalGrade(finalGrade.Id, true);
//            Assert.AreEqual(finalGrade.StudentFinalGrades.First().GradeByAnnouncement, (int)(0.2 * 70 + 0.4 * 40 + 0.2 * 60));
//        }

//        [Test]
//        public void SetGetStudentFinalGradesTest()
//        {
//            var mathClass1 = ClassServiceTest.CreateClass(SchoolTestContext, SchoolTestContext.FirstTeacher,
//                                                         SchoolTestContext.FirstStudent, null);
//            var mpc1 = mathClass1.MarkingPeriodClasses[0];
//            var mpId = mpc1.MarkingPeriodRef;
//            AnnouncementServiceTest.CreateDefaulTeacherAnnouncement(SchoolTestContext.FirstTeacherSl, mathClass1.Id, mpId);

//            var finalGrade = SchoolTestContext.FirstTeacherSl.FinalGradeService.GetFinalGrade(mpc1.Id, true);

//            var studentFinalGrade = finalGrade.StudentFinalGrades.First();
            
//            AssertForDeny(sl => sl.FinalGradeService.SetStudentFinalGrade(studentFinalGrade.Id, 10, 10, 5, 40, 20, ""), SchoolTestContext
//                , SchoolContextRoles.FirstParent | SchoolContextRoles.FirstStudent | SchoolContextRoles.Checkin
//                | SchoolContextRoles.SecondTeacher | SchoolContextRoles.AdminViewer);

//            var studentFinalGrade2 = SchoolTestContext.FirstTeacherSl.FinalGradeService.SetStudentFinalGrade(studentFinalGrade.Id, 10, 10, 5, 40, 20, "");
//            Assert.AreEqual(studentFinalGrade2.GradeByAttendance, 10);
//            Assert.AreEqual(studentFinalGrade2.GradeByDiscipline, 10);
//            Assert.AreEqual(studentFinalGrade2.GradeByParticipation, 5);
//            Assert.AreEqual(studentFinalGrade2.TeacherGrade, 40);
//            Assert.AreEqual(studentFinalGrade2.AdminGrade, 0);
//            finalGrade = SchoolTestContext.FirstTeacherSl.FinalGradeService.GetFinalGrade(mpc1.Id);
//            AssertAreEqual(studentFinalGrade2, finalGrade.StudentFinalGrades.First());
//        }

//        [Test]
//        public void SubmitApproveFinalGradeTest()
//        {
//            var mathClass1 = ClassServiceTest.CreateClass(SchoolTestContext, SchoolTestContext.FirstTeacher,
//                                                         SchoolTestContext.FirstStudent, null);

//            var mpc1 = mathClass1.MarkingPeriodClasses.First();
//            var teacherFgService = SchoolTestContext.FirstTeacherSl.FinalGradeService;
//            var adminFgService = SchoolTestContext.AdminGradeSl.FinalGradeService;
//            var finalGrade = teacherFgService.GetFinalGrade(mpc1.Id, true);

//            Assert.IsTrue(teacherFgService.Submit(finalGrade.Id));
//            Assert.AreEqual(teacherFgService.GetFinalGrade(mpc1.Id).Status, FinalGradeStatus.Submit);
//            Assert.IsTrue(!teacherFgService.Submit(finalGrade.Id));

//            Assert.IsTrue(adminFgService.ApproveReject(finalGrade.Id, false));
//            Assert.AreEqual(teacherFgService.GetFinalGrade(mpc1.Id).Status, FinalGradeStatus.Open);
//            Assert.IsTrue(!adminFgService.ApproveReject(finalGrade.Id, true));

//            Assert.IsTrue(teacherFgService.Submit(finalGrade.Id));
//            Assert.IsTrue(adminFgService.ApproveReject(finalGrade.Id, true));
//            Assert.AreEqual(teacherFgService.GetFinalGrade(mpc1.Id).Status, FinalGradeStatus.Approve);
//            Assert.IsTrue(!adminFgService.ApproveReject(finalGrade.Id, true));
//            Assert.IsTrue(!adminFgService.Submit(finalGrade.Id));


//            AssertForDeny(sl => sl.FinalGradeService.Submit(finalGrade.Id), SchoolTestContext,
//                SchoolContextRoles.FirstParent | SchoolContextRoles.FirstStudent | SchoolContextRoles.Checkin
//                | SchoolContextRoles.SecondTeacher | SchoolContextRoles.AdminViewer);

//            AssertForDeny(sl => sl.FinalGradeService.ApproveReject(finalGrade.Id, true), SchoolTestContext
//                , SchoolContextRoles.FirstParent | SchoolContextRoles.FirstStudent | SchoolContextRoles.Checkin
//                | SchoolContextRoles.SecondTeacher | SchoolContextRoles.AdminViewer | SchoolContextRoles.AdminEditor);

//        }
//    }
//}
