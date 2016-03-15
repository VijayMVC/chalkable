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

        [AuthorizationFilter("Teacher, DistricAdmin")]
        public async Task<ActionResult> Authorities()
        {
            var authorities = await MasterLocator.AcademicBenchmarkService.GetAuthorities();
            return Json(authorities.Select(AuthorityViewData.Create));
        }

        [AuthorizationFilter("Teacher, DistricAdmin")]
        public async Task<ActionResult> Documents(Guid? authorityId)
        {
            var docs = await MasterLocator.AcademicBenchmarkService.GetDocuments(authorityId);
            return Json(docs.Select(DocumentViewData.Create));
        }

        [AuthorizationFilter("Teacher, DistricAdmin")]
        public async Task<ActionResult> Subjects(Guid? authorityId, Guid? documentId)
        {
            var subjects = await MasterLocator.AcademicBenchmarkService.GetSubjects(authorityId, documentId);
            return Json(subjects.Select(SubjectViewData.Create));
        }

        [AuthorizationFilter("Teacher, DistricAdmin")]
        public async Task<ActionResult> GradeLevels(Guid? authorityId, Guid? documentId, Guid? subjectDocId)
        {
            var gradeLevels = await MasterLocator.AcademicBenchmarkService.GetGradeLevels(authorityId, documentId, subjectDocId);
            return Json(gradeLevels.Select(GradeLevelViewData.Create));
        }

        [AuthorizationFilter("Teacher, DistricAdmin")]
        public async Task<ActionResult> SearchStandards(string searchQuery, int? start, int? count)
        {
            start = start ?? 0;
            count = count ?? 10;
            var standards = await MasterLocator.AcademicBenchmarkService.SearchStandards(searchQuery, start.Value, count.Value);
            return Json(standards.Select(StandardViewData.Create));
        }

        [AuthorizationFilter("Teacher, DistricAdmin")]
        public async Task<ActionResult> Standards(Guid? authorityId, Guid? documentId, Guid? subjectDocId, string gradeLevelCode, Guid? parentId, bool? firstLevelOnly)
        {
            var standards = await MasterLocator.AcademicBenchmarkService.GetStandards(authorityId, documentId, subjectDocId, gradeLevelCode, parentId, firstLevelOnly ?? false);
            return Json(standards.Select(StandardViewData.Create));
        }

        [AuthorizationFilter("Teacher, DistricAdmin")]
        public async Task<ActionResult> Topics(string subjectCode, string gradeLevel, Guid? parentId, bool? firstLevelOnly)
        {
            var topics = await MasterLocator.AcademicBenchmarkService.GetTopics(subjectCode, gradeLevel, parentId, null, firstLevelOnly ?? false);
            return Json(topics.Select(TopicViewData.Create).ToList());
        }


        [AuthorizationFilter("Teacher, DistricAdmin")]
        public async Task<ActionResult> SearchTopics(string searchQuery, int? start, int? count)
        {
            start = start ?? 0;
            count = count ?? int.MaxValue;
            var topics = await MasterLocator.AcademicBenchmarkService.GetTopics(null, null, null, searchQuery, false, start.Value, count.Value);
            return Json(topics.Transform(TopicViewData.Create));
        }

        [AuthorizationFilter("Teacher, DistricAdmin")]
        public async Task<ActionResult> GetSubjectDocuments(Guid? authorityId, Guid? documentId)
        {
            var subDocs = await MasterLocator.AcademicBenchmarkService.GetSubjectDocuments(authorityId, documentId);
            return Json(subDocs.Select(SubjectDocumentViewData.Create));
        }
    }
}