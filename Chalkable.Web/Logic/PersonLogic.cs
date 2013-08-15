using System;
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
            bool? byLastName = true, string filter = null, string roleName = null, Guid? classId = null
            , GuidList gradeLevelsIds = null, Guid? teacherId = null)
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
                    SortType = byLastName.Value ? SortTypeEnum.ByLastName : SortTypeEnum.ByFirstName
                };
            var res = locator.PersonService.GetPaginatedPersons(query);
            return res.Transform(PersonViewData.Create);
        }
    }
}