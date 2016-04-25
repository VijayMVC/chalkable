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
        public async Task<ActionResult> StandardsByIds(GuidList standardsIds)
        {
            MasterLocator = ServiceLocatorFactory.CreateMasterSysAdmin();
            var academicBenchmarkStandards = await MasterLocator.AcademicBenchmarkService.GetStandardsByIds((standardsIds));
            return Json(academicBenchmarkStandards.Select(StandardViewData.Create));
        }

        public async Task<ActionResult> TopicsByIds(GuidList topicsIds)
        {
            MasterLocator = ServiceLocatorFactory.CreateMasterSysAdmin();
            var topics = await MasterLocator.AcademicBenchmarkService.GetTopicsByIds(topicsIds);
            return Json(topics.Select(TopicViewData.Create));
        }

        public async Task<ActionResult> ListOfStandardRelationsByIds(GuidList standardsIds)
        {
            MasterLocator = ServiceLocatorFactory.CreateMasterSysAdmin();
            var academicBenchmarkRelatedStandards = await MasterLocator.AcademicBenchmarkService.GetListOfStandardRelations((standardsIds));
            return Json(academicBenchmarkRelatedStandards.Select(StandardRelationsViewData.Create));
        }

        [AuthorizationFilter("SysAdmin, Developer, DistrictAdmin, Teacher, AppTester")]
        public async Task<ActionResult> Authorities()
        {
            var authorities = await MasterLocator.AcademicBenchmarkService.GetAuthorities();
            return Json(authorities.Select(AuthorityViewData.Create));
        }

        [AuthorizationFilter("SysAdmin, Developer, DistrictAdmin, Teacher, AppTester")]
        public async Task<ActionResult> Documents(Guid? authorityId)
        {
            var docs = await MasterLocator.AcademicBenchmarkService.GetDocuments(authorityId);
            return Json(docs.Select(DocumentViewData.Create));
        }
        
        [AuthorizationFilter("SysAdmin, Developer, DistrictAdmin, Teacher, AppTester")]
        public async Task<ActionResult> SubjectDocuments(Guid? authorityId, Guid? documentId)
        {
            var subDocs = await MasterLocator.AcademicBenchmarkService.GetSubjectDocuments(authorityId, documentId);
            return Json(subDocs.Select(SubjectDocumentViewData.Create));
        }

        [AuthorizationFilter("SysAdmin, Developer, DistrictAdmin, Teacher, AppTester")]
        public async Task<ActionResult> GradeLevels(Guid? authorityId, Guid? documentId, Guid? subjectDocId)
        {
            var gradeLevels = await MasterLocator.AcademicBenchmarkService.GetGradeLevels(authorityId, documentId, subjectDocId);
            return Json(gradeLevels.Select(GradeLevelViewData.Create));
        }

        [AuthorizationFilter("SysAdmin, Developer, DistrictAdmin, Teacher, AppTester")]
        public async Task<ActionResult> Courses(Guid? authorityId, Guid? documentId, Guid? subjectDocId, string gradeLevelCode)
        {
            var courses = await MasterLocator.AcademicBenchmarkService.GetCourses(authorityId, documentId, subjectDocId, gradeLevelCode);
            return Json(courses.Select(CourseViewData.Create));
        }

        [AuthorizationFilter("SysAdmin, Developer, DistrictAdmin, Teacher, AppTester")]
        public async Task<ActionResult> SearchStandards(string searchQuery, bool? deepest, int? start, int? count)
        {
            start = start ?? 0;
            count = count ?? 10;
            var standards = await MasterLocator.AcademicBenchmarkService.SearchStandards(searchQuery, deepest, start.Value, count.Value);
            return Json(standards.Transform(StandardViewData.Create));
        }

        [AuthorizationFilter("SysAdmin, Developer, DistrictAdmin, Teacher, AppTester")]
        public async Task<ActionResult> Standards(Guid? authorityId, Guid? documentId, Guid? subjectDocId, string gradeLevelCode, Guid? parentId, bool? firstLevelOnly, Guid? courseId)
        {
            var standards = await MasterLocator.AcademicBenchmarkService.GetStandards(authorityId, documentId, subjectDocId, gradeLevelCode, parentId, courseId, firstLevelOnly ?? false);
            return Json(standards.Select(StandardViewData.Create));
        }

        [AuthorizationFilter("SysAdmin, Developer, DistrictAdmin, Teacher, AppTester")]
        public async Task<ActionResult> TopicSubjectDocuments()
        {
            var res = await MasterLocator.AcademicBenchmarkService.GetTopicSubjectDocuments();
            return Json(res.Select(SubjectDocumentViewData.Create));
        }

        [AuthorizationFilter("SysAdmin, Developer, DistrictAdmin, Teacher, AppTester")]
        public async Task<ActionResult> TopicCourses(Guid? subjectDocId)
        {
            var res = await MasterLocator.AcademicBenchmarkService.GetTopicCourses(subjectDocId);
            return Json(res.Select(CourseViewData.Create));
        }

        [AuthorizationFilter("SysAdmin, Developer, DistrictAdmin, Teacher, AppTester")]
        public async Task<ActionResult> Topics(Guid? subjectDocId, Guid? courseId, Guid? parentId, bool? firstLevelOnly)
        {
            var topics = await MasterLocator.AcademicBenchmarkService.GetTopics(subjectDocId, courseId, parentId, firstLevelOnly ?? false);
            return Json(topics.Select(TopicViewData.Create));
        }

        [AuthorizationFilter("SysAdmin, Developer, DistrictAdmin, Teacher, AppTester")]
        public async Task<ActionResult> SearchTopics(string searchQuery, bool? deepest, int? start, int? count)
        {
            start = start ?? 0;
            count = count ?? int.MaxValue;
            
            var topics = await MasterLocator.AcademicBenchmarkService.SearchTopics(searchQuery, deepest, start.Value, count.Value);
            return Json(topics.Transform(TopicViewData.Create));
        }
    }
}