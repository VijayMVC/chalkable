using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Logic
{
    public class PersonLogic
    {
        private const int DEFAULT_COUNT = 10;

        public static PaginatedList<PersonViewData> GetPersons(IServiceLocatorSchool locator, int? start, int? count,
            bool? byLastName = true, string filter = null, string roleName = null, int? classId = null
            , IntList gradeLevelsIds = null, int? teacherId = null, bool? onlyMyTeachers = false)
        {
            var query = new PersonQuery
                {
                    ClassId = classId,
                    GradeLevelIds = gradeLevelsIds,
                    TeacherId = teacherId,
                    Filter = filter,
                    RoleId = string.IsNullOrEmpty(roleName) ? default(int?) : CoreRoles.GetByName(roleName).Id,
                    Start = start ?? 0,
                    Count = count ?? DEFAULT_COUNT,
                    SortType = byLastName.HasValue && byLastName.Value ? SortTypeEnum.ByLastName : SortTypeEnum.ByFirstName,
                    OnlyMyTeachers = onlyMyTeachers ?? false
                };
            IList<ClassPerson> classPersons = null;
            if (classId.HasValue && query.RoleId == CoreRoles.STUDENT_ROLE.Id)
            {
                var classRoomOption = locator.ClassroomOptionService.GetClassOption(classId.Value);
                query.IsEnrolled = classRoomOption != null && !classRoomOption.IncludeWithdrawnStudents ? true : default(bool?);
                if (!query.IsEnrolled.HasValue)
                    classPersons = locator.ClassService.GetClassPersons(null, classId, query.IsEnrolled, query.MarkingPeriodId);
            }
            var res = locator.PersonService.GetPaginatedPersons(query);
            if (classPersons != null && classPersons.Count > 0)
                foreach (var person in res)
                {
                    person.IsWithdrawn = classPersons.Any(x => x.PersonRef == person.Id && !x.IsEnrolled);
                }

            return res.Transform(PersonViewData.Create);
        }
    }
}