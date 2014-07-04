using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.School.Model;
using Chalkable.Tests.Services.TestContext;
using NUnit.Framework;

namespace Chalkable.Tests.Services.School
{
    public class StandardServiceTest : BaseSchoolServiceTest
    {
        [Test]
        public void AddDeleteTest()
        {
            SysAdminFirstSchoolLocator.GradeLevelService.AddGradeLevel(3, "3", 3);
            SysAdminFirstSchoolLocator.GradeLevelService.AddGradeLevel(4, "4", 4);
            SysAdminFirstSchoolLocator.GradeLevelService.AddGradeLevel(5, "5", 5);
            var subjects = new List<StandardSubject>
                {
                    new StandardSubject
                        {
                            Id = 1,
                            AdoptionYear = 2013,
                            Name = "test_subject",
                            Description = "test subject",
                            IsActive = false
                        }
                };
            SysAdminFirstSchoolLocator.StandardService.AddStandardSubjects(subjects);
            var standards = new List<Standard>
                {
                    new Standard
                        {
                            Id = 1,
                            Description = "first standard",
                            IsActive = true,
                            Name = "first_standard",
                            StandardSubjectRef = subjects[0].Id
                        },
                    new Standard
                        {
                            Id = 2,
                            Description = "second standard",
                            IsActive = true,
                            Name = "second_standard",
                            LowerGradeLevelRef = 3,
                            UpperGradeLevelRef = 5,
                            StandardSubjectRef = subjects[0].Id
                        }
                };
            AssertForDeny(sl => sl.StandardService.AddStandards(standards), FirstSchoolContext
                , SchoolContextRoles.AdminGrade | SchoolContextRoles.AdminEditor | SchoolContextRoles.FirstParent
                | SchoolContextRoles.Checkin | SchoolContextRoles.FirstTeacher | SchoolContextRoles.SecondStudent);

            standards[1].LowerGradeLevelRef = 5;
            standards[1].UpperGradeLevelRef = 3;
            AssertException<Exception>(() => SysAdminFirstSchoolLocator.StandardService.AddStandards(standards));

            standards[1].UpperGradeLevelRef = 5;
            standards[1].LowerGradeLevelRef = 3;

            SysAdminFirstSchoolLocator.StandardService.AddStandards(standards);
            var standards2 =  SysAdminFirstSchoolLocator.StandardService.GetStandards(null, null, null, null);
            AssertAreEqual(standards, standards2);

            SysAdminFirstSchoolLocator.StandardService.DeleteStandard(standards[1].Id);
            standards2 = SysAdminFirstSchoolLocator.StandardService.GetStandards(null, null, null);
            Assert.AreEqual(standards2.Count, 1);
            SysAdminFirstSchoolLocator.StandardService.DeleteStandard(standards[0].Id);
            Assert.AreEqual(SysAdminFirstSchoolLocator.StandardService.GetStandards(null, null, null).Count, 0);
        }
    }
}
