using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Services;
using Chalkable.Common;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models.AcademicBenchmarksViewData;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class AcademicBenchmarkController : ChalkableController
    {
        //TODO: impl some auth logic for these methods later
        public ActionResult StandardsByIds(GuidList standardsIds)
        {
            MasterLocator = ServiceLocatorFactory.CreateMasterSysAdmin();
            var standards = AcademicBenchmarkLocator.StandardService.GetStandardInfosByIds(standardsIds);
            return Json(standards.Select(StandardViewData.Create));
        }

        public ActionResult TopicsByIds(GuidList topicsIds)
        {
            MasterLocator = ServiceLocatorFactory.CreateMasterSysAdmin();
            var topics = AcademicBenchmarkLocator.TopicService.GetByIds(topicsIds);
            return Json(topics.Select(TopicViewData.Create));
        }

        public ActionResult ListOfStandardRelationsByIds(GuidList standardsIds)
        {
            MasterLocator = ServiceLocatorFactory.CreateMasterSysAdmin();
            var academicBenchmarkRelatedStandards = AcademicBenchmarkLocator.StandardService.GetStandardsRelations(standardsIds);
            return Json(academicBenchmarkRelatedStandards.Select(StandardRelationsViewData.Create));
        }

        [AuthorizationFilter("SysAdmin, Developer, DistrictAdmin, Teacher, AppTester")]
        public ActionResult Authorities()
        {
            var authorities = AcademicBenchmarkLocator.AuthorityService.GetAll();
            return Json(authorities.Select(AuthorityViewData.Create));
        }

        [AuthorizationFilter("SysAdmin, Developer, DistrictAdmin, Teacher, AppTester")]
        public ActionResult Documents(Guid? authorityId)
        {
            var docs = AcademicBenchmarkLocator.DocumentService.GetByAuthority(authorityId);
            return Json(docs.Select(DocumentViewData.Create));
        }
        
        [AuthorizationFilter("SysAdmin, Developer, DistrictAdmin, Teacher, AppTester")]
        public ActionResult SubjectDocuments(Guid? authorityId, Guid? documentId)
        {
            var subDocs = AcademicBenchmarkLocator.SubjectDocService.GetForStandards(authorityId, documentId);
            return Json(subDocs.Select(SubjectDocumentViewData.Create));
        }

        [AuthorizationFilter("SysAdmin, Developer, DistrictAdmin, Teacher, AppTester")]
        public ActionResult GradeLevels(Guid? authorityId, Guid? documentId, Guid? subjectDocId)
        {
            var gradeLevels = AcademicBenchmarkLocator.GradeLevelService.Get(authorityId, documentId, subjectDocId);
            return Json(gradeLevels.Select(GradeLevelViewData.Create));
        }

        [AuthorizationFilter("SysAdmin, Developer, DistrictAdmin, Teacher, AppTester")]
        public ActionResult Courses(Guid? authorityId, Guid? documentId, Guid? subjectDocId, string gradeLevelCode)
        {
            var courses = AcademicBenchmarkLocator.CourseService.GetForStandards(authorityId, documentId, subjectDocId, gradeLevelCode);
            return Json(courses.Select(CourseViewData.Create));
        }

        [AuthorizationFilter("SysAdmin, Developer, DistrictAdmin, Teacher, AppTester")]
        public ActionResult SearchStandards(string searchQuery, bool? deepest, int? start, int? count)
        {
            start = start ?? 0;
            count = count ?? 10;
            var standards = AcademicBenchmarkLocator.StandardService.Search(searchQuery, deepest, start.Value, count.Value);
            return Json(standards.Transform(StandardViewData.Create));
        }

        [AuthorizationFilter("SysAdmin, Developer, DistrictAdmin, Teacher, AppTester")]
        public ActionResult Standards(Guid? authorityId, Guid? documentId, Guid? subjectDocId, string gradeLevelCode, Guid? parentId, bool? firstLevelOnly, Guid? courseId)
        {
            var standards = AcademicBenchmarkLocator.StandardService.Get(authorityId, documentId, subjectDocId, gradeLevelCode, parentId, courseId, firstLevelOnly ?? false);
            return Json(standards.Select(StandardViewData.Create));
        }

        [AuthorizationFilter("SysAdmin, Developer, DistrictAdmin, Teacher, AppTester")]
        public ActionResult TopicSubjectDocuments()
        {
            var res = AcademicBenchmarkLocator.SubjectDocService.GetForTopics();
            return Json(res.Select(SubjectDocumentViewData.Create));
        }

        [AuthorizationFilter("SysAdmin, Developer, DistrictAdmin, Teacher, AppTester")]
        public ActionResult TopicCourses(Guid? subjectDocId)
        {
            var res = AcademicBenchmarkLocator.CourseService.GetForTopics(subjectDocId);
            return Json(res.Select(CourseViewData.Create));
        }

        [AuthorizationFilter("SysAdmin, Developer, DistrictAdmin, Teacher, AppTester")]
        public ActionResult Topics(Guid? subjectDocId, Guid? courseId, Guid? parentId, bool? firstLevelOnly)
        {
            var topics = AcademicBenchmarkLocator.TopicService.Get(subjectDocId, courseId, parentId, firstLevelOnly ?? false);
            return Json(topics.Select(TopicViewData.Create));
        }

        [AuthorizationFilter("SysAdmin, Developer, DistrictAdmin, Teacher, AppTester")]
        public ActionResult SearchTopics(string searchQuery, bool? deepest, int? start, int? count)
        {
            start = start ?? 0;
            count = count ?? int.MaxValue;
            
            var topics = AcademicBenchmarkLocator.TopicService.SearchTopics(searchQuery, deepest, start.Value, count.Value);
            return Json(topics.Transform(TopicViewData.Create));
        }
    }
}