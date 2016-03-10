using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using WindowsAzure.Acs.Oauth2.Protocol;
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

        public async Task<ActionResult> ListOfStandardRelationsByIds(GuidList standardsIds)
        {
            MasterLocator = ServiceLocatorFactory.CreateMasterSysAdmin();
            var academicBenchmarkRelatedStandards = await MasterLocator.AcademicBenchmarkService.GetListOfStandardRelations((standardsIds));
            return Json(academicBenchmarkRelatedStandards.Select(StandardRelationsViewData.Create));
        }

        [AuthorizationFilter("Teacher, DistricAdmin")]
        public async Task<ActionResult> GetAuthorities()
        {
            var authorities = await MasterLocator.AcademicBenchmarkService.GetAuthorities();
            return Json(authorities.Select(AuthorityViewData.Create));
        }

        [AuthorizationFilter("Teacher, DistricAdmin")]
        public async Task<ActionResult> GetDocuments(Guid? authorityId)
        {
            var docs = await MasterLocator.AcademicBenchmarkService.GetDocuments(authorityId);
            return Json(docs.Select(DocumentViewData.Create));
        }

        [AuthorizationFilter("Teacher, DistricAdmin")]
        public async Task<ActionResult> GetSubjects(Guid? authorityId, Guid? documentId)
        {
            var subjects = await MasterLocator.AcademicBenchmarkService.GetSubjects(authorityId, documentId);
            return Json(subjects.Select(SubjectViewData.Create));
        }

        [AuthorizationFilter("Teacher, DistricAdmin")]
        public async Task<ActionResult> GetGradeLevels(Guid? authorityId, Guid? documentId, string subjectCode)
        {
            var gradeLevels = await MasterLocator.AcademicBenchmarkService.GetGradeLevels(authorityId, documentId, subjectCode);
            return Json(gradeLevels.Select(GradeLevelViewData.Create));
        }

        [AuthorizationFilter("Teacher, DistricAdmin")]
        public async Task<ActionResult> SearchStandard(string searchQuery, int start, int count)
        {
            var standards = await MasterLocator.AcademicBenchmarkService.SearchStandard(searchQuery, start, count);
            return Json(standards.Select(StandardViewData.Create));
        }

        [AuthorizationFilter("Teacher, DistricAdmin")]
        public async Task<ActionResult> GetStandards(Guid? authorityId, Guid? documentId, string subjectCode,
            string gradeLevelCode, Guid? parentId)
        {
            var standards = await MasterLocator.AcademicBenchmarkService.GetStandards(authorityId, documentId, subjectCode, gradeLevelCode,
                parentId);
            return Json(standards.Select(StandardViewData.Create));
        }

        [AuthorizationFilter("Teacher, DistricAdmin")]
        public async Task<ActionResult> GetTopics(Guid? subject, string gradeLevel, Guid? parentId, string searchQuery, int start, 
            int count)
        {
            var topics = await MasterLocator.AcademicBenchmarkService.GetTopics(subject, gradeLevel, parentId, searchQuery,
                start, count);
            return Json(topics.Transform(TopicViewData.Create));
        }
    }
}