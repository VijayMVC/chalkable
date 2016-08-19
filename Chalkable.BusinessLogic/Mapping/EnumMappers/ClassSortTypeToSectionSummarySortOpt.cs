using System.Collections.Generic;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.StiConnector.Connectors;

namespace Chalkable.BusinessLogic.Mapping.EnumMappers
{
    public class ClassSortTypeToSectionSummarySortOpt : BaseEnumMapper<ClassSortType, SectionSummarySortOption>
    {
        public ClassSortTypeToSectionSummarySortOpt()
        {
            _mapperDictionary = new Dictionary<ClassSortType, SectionSummarySortOption>
            {
                [ClassSortType.ClassAsc] = SectionSummarySortOption.SectionAscending,
                [ClassSortType.ClassDesc] = SectionSummarySortOption.SectionDescending,
                [ClassSortType.TeacherAsc] = SectionSummarySortOption.TeacherAscending,
                [ClassSortType.TeacherDesc] = SectionSummarySortOption.TeacherDescending,
                [ClassSortType.StudentsAsc] = SectionSummarySortOption.StudentsAscending,
                [ClassSortType.StudentsDesc] = SectionSummarySortOption.StudentsDescending,
                [ClassSortType.AttendanceAsc] = SectionSummarySortOption.AttendanceAscending,
                [ClassSortType.AttendanceDesc] = SectionSummarySortOption.AttendanceDescending,
                [ClassSortType.DisciplineAsc] = SectionSummarySortOption.DisciplineAscending,
                [ClassSortType.DisciplineDesc] = SectionSummarySortOption.DisciplineDescending,
                [ClassSortType.GradesAsc] = SectionSummarySortOption.GradesAscending,
                [ClassSortType.GradesDesc] = SectionSummarySortOption.GradesDescending
            };
        }
    }
}
