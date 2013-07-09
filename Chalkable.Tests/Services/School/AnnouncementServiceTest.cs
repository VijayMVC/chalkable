using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.School.Model;
using NUnit.Framework;

namespace Chalkable.Tests.Services.School
{
    public class AnnouncementServiceTest : BaseSchoolServiceTest
    {
        [Test]
        public void CreateDeleteDraftAnnouncementTest()
        {
            var c = ClassServiceTest.CreateClass(SchoolTestContext, SchoolTestContext.FirstTeacher,
                                                 SchoolTestContext.FirstStudent, null);

            var annTypeHw = SchoolTestContext.AdminGradeSl.AnnouncementService.GetAnnouncementTypeBySystemType(
                    SystemAnnouncementType.HW);
            
            AssertForDeny(sl=>sl.AnnouncementService.CreateAnnouncement(annTypeHw.Id), 
                SchoolTestContext, SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstParent 
                | SchoolContextRoles.SecondStudent | SchoolContextRoles.Checkin); 

            var ann = SchoolTestContext.FirstTeacherSl.AnnouncementService.CreateAnnouncement(annTypeHw.Id);
            Assert.AreEqual(ann.Created.Date, SchoolTestContext.NowDate);
            Assert.IsTrue(ann.IsDraft);
            Assert.AreEqual(ann.AnnouncementTypeRef, annTypeHw.Id);
            Assert.AreEqual(ann.AnnouncementTypeName, annTypeHw.Name);
            Assert.IsTrue(ann.IsGradableType);
            Assert.AreEqual(ann.MarkingPeriodClassRef, null);
            Assert.AreEqual(ann.IsOwner, true);
            Assert.AreEqual(ann.Dropped, false);
            AssertAreEqual(ann, SchoolTestContext.FirstTeacherSl.AnnouncementService.GetAnnouncementDetails(ann.Id));
            Assert.AreEqual(SchoolTestContext.FirstTeacherSl.AnnouncementService.GetAnnouncements(false, 0 , int.MaxValue, null).Count, 0);

            AssertForDeny(sl => sl.AnnouncementService.DeleteAnnouncements(SchoolTestContext.FirstTeacher.Id), SchoolTestContext,
                SchoolContextRoles.AdminEditor | SchoolContextRoles.AdminGrade | SchoolContextRoles.AdminViewer
                | SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstParent | SchoolContextRoles.SecondStudent
                | SchoolContextRoles.Checkin);

            SchoolTestContext.FirstTeacherSl.AnnouncementService.DeleteAnnouncements(SchoolTestContext.FirstTeacher.Id);
            Assert.IsNull(SchoolTestContext.FirstTeacherSl.AnnouncementService.GetAnnouncementDetails(ann.Id));
            
        }
    }
}
