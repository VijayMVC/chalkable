using System.Collections.Generic;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.StiConnector.Connectors;

namespace Chalkable.BusinessLogic.Mapping.EnumMappers
{
    class TeacherSortTypeToSectionSummarySortOpt : BaseEnumMapper<TeacherSortType, SectionSummarySortOption>
    {
        public TeacherSortTypeToSectionSummarySortOpt()
        {
            _mapperDictionary = new Dictionary<TeacherSortType, SectionSummarySortOption>
            {               
                [TeacherSortType.TeacherAsc] = SectionSummarySortOption.TeacherAscending,
                [TeacherSortType.TeacherDesc] = SectionSummarySortOption.TeacherDescending,
                [TeacherSortType.StudentsAsc] = SectionSummarySortOption.StudentsAscending,
                [TeacherSortType.StudentsDesc] = SectionSummarySortOption.StudentsDescending,
                [TeacherSortType.AttendanceAsc] = SectionSummarySortOption.AttendanceAscending,
                [TeacherSortType.AttendanceDesc] = SectionSummarySortOption.AttendanceDescending,
                [TeacherSortType.DisciplineAsc] = SectionSummarySortOption.DisciplineAscending,
                [TeacherSortType.DisciplineDesc] = SectionSummarySortOption.DisciplineDescending,
                [TeacherSortType.GradesAsc] = SectionSummarySortOption.GradesAscending,
                [TeacherSortType.GradesDesc] = SectionSummarySortOption.GradesDescending
            };
        }
    }
}
