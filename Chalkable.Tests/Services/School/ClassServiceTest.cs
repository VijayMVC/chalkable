using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.Model;
using Chalkable.Tests.Services.TestContext;
using NUnit.Framework;

namespace Chalkable.Tests.Services.School
{
    public class ClassServiceTest : BaseSchoolServiceTest
    {
        [Test]
        public void AddDeleteClassTest()
        {
            var districtSl = DistrictTestContext.DistrictLocatorFirstSchool;
            var school1Id = FirstSchoolContext.School.LocalId;
            var mp = MarkingPeriodServiceTest.CreateSchoolYearWithMp(districtSl, FirstSchoolContext.NowDate.AddDays(-5), true);
            districtSl.GradeLevelService.AddGradeLevel(1, "1th", 1);
            var gradeLevel = districtSl.GradeLevelService.AddSchoolGradeLevel(1, school1Id);
            var newId = 1;
            AssertForDeny(sl => sl.ClassService.Add(newId, mp.SchoolYearRef, null, "class1", "first class", FirstSchoolContext.FirstTeacher.Id
                , gradeLevel.Id), FirstSchoolContext
                , SchoolContextRoles.AdminGrade | SchoolContextRoles.AdminEditor | SchoolContextRoles.FirstTeacher
                | SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstParent
                | SchoolContextRoles.AdminViewer | SchoolContextRoles.Checkin);

            var cService = districtSl.ClassService;
            var c = cService.Add(newId, mp.SchoolYearRef, null, "class1", "first class", FirstSchoolContext.FirstTeacher.Id, gradeLevel.Id);

            Assert.AreEqual(c.SchoolYearRef, mp.SchoolYearRef);
            Assert.AreEqual(c.Name, "class1");
            Assert.AreEqual(c.Description, "first class");
            Assert.AreEqual(c.TeacherRef, FirstSchoolContext.FirstTeacher.Id);
            Assert.AreEqual(c.StudentsCount, 0);
            Assert.AreEqual(c.GradeLevelRef, gradeLevel.Id);
            AssertAreEqual(c, cService.GetClassById(c.Id));
            Assert.AreEqual(FirstSchoolContext.AdminGradeSl.ClassService.GetClasses(mp.SchoolYearRef, null, null).Count, 1);
            Assert.AreEqual(SecondSchoolContext.AdminGradeSl.ClassService.GetClasses(null).Count, 0);

            var sy2 = SchoolYearServiceTest.CreateNextSchoolYear(districtSl);
            var mp2 = MarkingPeriodServiceTest.CreateNextMp(districtSl, sy2.Id);
            newId++;
            AssertException<Exception>(() => cService.Add(newId, sy2.Id, null, "class2", "second class"
                                  , SecondSchoolContext.FirstTeacher.Id, gradeLevel.Id));
            
            var c2 = cService.Add(newId, sy2.Id, null, "class2", "second class"
                                  , FirstSchoolContext.FirstTeacher.Id, gradeLevel.Id);

            Assert.AreEqual(cService.GetClasses(null, null, null).Count, 2);
            Assert.AreEqual(cService.GetClasses(null, null, FirstSchoolContext.FirstTeacher.Id).Count, 2);

            AssertForDeny(sl => sl.ClassService.Edit(c.Id, null, "class3", "third class", FirstSchoolContext.FirstTeacher.Id
                    , gradeLevel.Id), FirstSchoolContext
                    , SchoolContextRoles.AdminGrade | SchoolContextRoles.AdminEditor | SchoolContextRoles.FirstTeacher
                    | SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstParent
                    | SchoolContextRoles.AdminViewer | SchoolContextRoles.Checkin);

            AssertException<Exception>(() => cService.Edit(c.Id, null, "class3", "third class"
                           , SecondSchoolContext.FirstTeacher.Id, gradeLevel.Id));

            c = cService.Edit(c.Id, null, "class3", "third class", FirstSchoolContext.FirstTeacher.Id, gradeLevel.Id);
            Assert.AreEqual(c.SchoolYearRef, mp.SchoolYearRef);
            Assert.AreEqual(c.Name, "class3");
            Assert.AreEqual(c.Description, "third class");
            Assert.AreEqual(c.TeacherRef, FirstSchoolContext.FirstTeacher.Id);
            Assert.AreEqual(c.StudentsCount, 0);
            Assert.AreEqual(c.GradeLevelRef, gradeLevel.Id);


            AssertForDeny(sl => sl.ClassService.Delete(c.Id), FirstSchoolContext
                    , SchoolContextRoles.AdminGrade | SchoolContextRoles.AdminEditor | SchoolContextRoles.FirstTeacher
                    | SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstParent
                    | SchoolContextRoles.AdminViewer | SchoolContextRoles.Checkin);
            cService.AddStudent(c.Id, FirstSchoolContext.FirstStudent.Id);
            cService.Delete(c.Id);
            Assert.AreEqual(cService.GetClasses(mp.SchoolYearRef, null, null).Count, 0);
        }

        [Test]
        public void AddDeleteStudentTest()
        {
            var districtSl = DistrictTestContext.DistrictLocatorFirstSchool;
            var school1Id = FirstSchoolContext.School.LocalId;
            districtSl.GradeLevelService.AddGradeLevel(1, "1th", 1);
            var gradeLevelId = districtSl.GradeLevelService.AddSchoolGradeLevel(1, school1Id).Id;
            var mp = MarkingPeriodServiceTest.CreateSchoolYearWithMp(districtSl, FirstSchoolContext.NowDate.AddDays(-5), true);
            var cService = districtSl.ClassService;
            var newId = 1;
            var c = cService.Add(newId, mp.SchoolYearRef, null, "class1", "first class", FirstSchoolContext.FirstTeacher.Id, gradeLevelId);

            AssertForDeny(sl => sl.ClassService.AddStudent(c.Id, FirstSchoolContext.FirstStudent.Id), FirstSchoolContext
                , SchoolContextRoles.AdminGrade | SchoolContextRoles.AdminEditor | SchoolContextRoles.FirstTeacher
                | SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstParent | SchoolContextRoles.AdminViewer | SchoolContextRoles.Checkin);

            AssertException<Exception>(() => cService.AddStudent(c.Id, FirstSchoolContext.FirstTeacher.Id));
            AssertException<Exception>(()=> DistrictTestContext.DistrictLocatorSecondSchool.ClassService.AddStudent(c.Id, FirstSchoolContext.FirstStudent.Id));
            c = cService.AddStudent(c.Id, FirstSchoolContext.FirstStudent.Id);
            var clPerson = FirstSchoolContext.AdminGradeSl.ClassService.GetClassPerson(c.Id, FirstSchoolContext.FirstStudent.Id);

            AssertException<Exception>(() => DistrictTestContext.DistrictLocatorSecondSchool.ClassService.GetClassPerson(c.Id, FirstSchoolContext.FirstStudent.Id));
            
            Assert.AreEqual(clPerson.ClassRef, c.Id);
            Assert.AreEqual(clPerson.PersonRef, FirstSchoolContext.FirstStudent.Id);
            Assert.AreEqual(c.StudentsCount, 1);
            var classes = cService.GetClasses(null, null, FirstSchoolContext.FirstStudent.Id);
            Assert.AreEqual(classes.Count, 1);
            AssertAreEqual(classes[0], c);
            c = cService.AddStudent(c.Id, FirstSchoolContext.FirstStudent.Id);
            Assert.AreEqual(c.StudentsCount, 1);

            var dayType = districtSl.DayTypeService.Add(1, 1, "A", mp.SchoolYearRef);
            var cDate = districtSl.CalendarDateService.GetCalendarDateByDate(FirstSchoolContext.NowDate);
            var period = districtSl.PeriodService.Add(1, mp.Id, 450, 500, 1);
            var room = districtSl.RoomService.AddRoom(1, school1Id, "001", "first room", "10X10", null, "333-444");
            var cPeriod = districtSl.ClassPeriodService.Add(1, period.Id, c.Id, room.Id, dayType.Id);
            var c2 = cService.Add(2, mp.SchoolYearRef, null, "class2", "second class", FirstSchoolContext.FirstTeacher.Id, gradeLevelId);
            var room2 = districtSl.RoomService.AddRoom(2, school1Id, "002", "second room", "10X10", null, "333-444");
            districtSl.ClassPeriodService.Add(2, period.Id, c2.Id, room2.Id, dayType.Id);
            AssertException<Exception>(() => cService.AddStudent(c2.Id, FirstSchoolContext.FirstStudent.Id));


            AssertForDeny(sl => sl.ClassService.DeleteStudent(c.Id, FirstSchoolContext.FirstStudent.Id), FirstSchoolContext
                , SchoolContextRoles.AdminGrade | SchoolContextRoles.AdminEditor | SchoolContextRoles.FirstTeacher 
                | SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstParent | SchoolContextRoles.AdminViewer | SchoolContextRoles.Checkin);

            c = cService.DeleteStudent(c.Id, FirstSchoolContext.FirstStudent.Id);
            Assert.AreEqual(cService.GetClasses(null, null, FirstSchoolContext.FirstStudent.Id).Count, 0);
            Assert.AreEqual(c.StudentsCount, 0);

        }

        [Test]
        public void AddDeleteClassFromMarkingPeriodTest()
        {
            var disctrictL = DistrictTestContext.DistrictLocatorFirstSchool;
            var school1Id = FirstSchoolContext.School.LocalId;
            var mp1 = MarkingPeriodServiceTest.CreateSchoolYearWithMp(disctrictL, FirstSchoolContext.NowDate.AddDays(-5), true);
            var mp2 = MarkingPeriodServiceTest.CreateNextMp(disctrictL, mp1.SchoolYearRef);
            var cService = disctrictL.ClassService;
            disctrictL.GradeLevelService.AddGradeLevel(1, "1th", 1);
            var gradeLevelId = disctrictL.GradeLevelService.AddSchoolGradeLevel(1, school1Id).Id;
            var c = cService.Add(1, mp1.SchoolYearRef, null, "class1", "first class", FirstSchoolContext.FirstTeacher.Id, gradeLevelId);
            var mp3 = MarkingPeriodServiceTest.CreateSchoolYearWithMp(disctrictL, null);

            //Security check 
            AssertForDeny(sl => sl.ClassService.AssignClassToMarkingPeriod(c.Id, mp2.Id), FirstSchoolContext,
                SchoolContextRoles.AdminGrade | SchoolContextRoles.AdminEditor | SchoolContextRoles.FirstTeacher
                | SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstParent | SchoolContextRoles.AdminViewer | SchoolContextRoles.Checkin);
           
            AssertForDeny(sl => sl.ClassService.UnassignClassFromMarkingPeriod(c.Id, mp1.Id), FirstSchoolContext,
                SchoolContextRoles.AdminGrade | SchoolContextRoles.AdminEditor | SchoolContextRoles.FirstTeacher
                | SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstParent | SchoolContextRoles.AdminViewer | SchoolContextRoles.Checkin);

            AssertException<Exception>(() => cService.AssignClassToMarkingPeriod(c.Id, mp3.Id));

            cService.AssignClassToMarkingPeriod(c.Id, mp2.Id);
            Assert.AreEqual(cService.GetClasses(mp1.SchoolYearRef, mp3.Id, null).Count, 0);
            Assert.AreEqual(cService.GetClasses(mp1.SchoolYearRef, mp2.Id, null).Count, 1);
            cService.UnassignClassFromMarkingPeriod(c.Id, mp2.Id);
            Assert.AreEqual(cService.GetClasses(mp1.SchoolYearRef, mp2.Id, null).Count, 0);

        }



        public static ClassDetails CreateClass(IServiceLocatorSchool serviceLocator, Person teacher, Person student1
        , Person student2, int? classId, int? gradeLevelId, string name = "math", int? schoolYearId = null)
        {
            MarkingPeriod mp;
            if (!schoolYearId.HasValue)
            {
                var date = serviceLocator.Context.NowSchoolTime.AddDays(-7);
                mp = MarkingPeriodServiceTest.CreateSchoolYearWithMp(serviceLocator, date);
            }
            else
            {
                var sy = serviceLocator.SchoolYearService.GetSchoolYearById(schoolYearId.Value);
                mp = serviceLocator.MarkingPeriodService.GetLastMarkingPeriod(sy.EndDate);
                if (mp == null)
                {
                    var year = serviceLocator.SchoolYearService.GetSchoolYears(0, 10).Last();
                    var dateStart = year.StartDate;
                    var dateEnd = year.EndDate;
                    mp = MarkingPeriodServiceTest.CreateNextMp(serviceLocator, year.Id);
                }
            }

            GradeLevel gradeLevel;
            var glId = gradeLevelId ?? 1;
            var gl = serviceLocator.GradeLevelService.GetGradeLevels().FirstOrDefault(x => x.Id == glId);
            if (gl == null)
            {
                serviceLocator.GradeLevelService.AddGradeLevel(glId, glId.ToString(), glId);
                gl = serviceLocator.GradeLevelService.GetGradeLevels().First(x => x.Id == glId);
            }
            if (!classId.HasValue)
                classId = GetNewId(serviceLocator, sl => sl.ClassService.GetClasses(null), c => c.Id); 
            
            var mathClass = serviceLocator.ClassService.Add(classId.Value, mp.SchoolYearRef, null, name, name, teacher.Id, gl.Id);
            if (student1 != null)
                mathClass = serviceLocator.ClassService.AddStudent(mathClass.Id, student1.Id);
            if (student2 != null)
                mathClass = serviceLocator.ClassService.AddStudent(mathClass.Id, student2.Id);

            return mathClass;
        }
    }
}
