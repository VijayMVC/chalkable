﻿using System;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Data.School.DataAccess;
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
            if (classId.HasValue && query.RoleId == CoreRoles.STUDENT_ROLE.Id)
            {
                var classRoomOption = locator.ClassroomOptionService.GetClassOption(classId.Value);
                query.IsEnrolled = classRoomOption != null && !classRoomOption.IncludeWithdrawnStudents ? true : default(bool?);
                var mp = locator.MarkingPeriodService.GetLastMarkingPeriod(locator.Context.NowSchoolYearTime.Date);
                if (mp != null) query.MarkingPeriodId = mp.Id;

            }
            var res = locator.PersonService.GetPaginatedPersons(query);
            return res.Transform(PersonViewData.Create);
        }
    }
}