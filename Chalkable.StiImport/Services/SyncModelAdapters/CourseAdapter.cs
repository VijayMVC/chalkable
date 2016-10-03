using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Common;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class CourseAdapter : SyncModelAdapter<Course>
    {
        public CourseAdapter(AdapterLocator locator) : base(locator)
        {
            departmenPairs = PrepareChalkableDepartmentKeywords();
        }

        private List<Pair<string, Guid>> departmenPairs;
        private List<Pair<string, Guid>> PrepareChalkableDepartmentKeywords()
        {
            var departments = ServiceLocatorMaster.ChalkableDepartmentService.GetChalkableDepartments();
            var sep = new[] { ',' };
            var pairs = new List<Pair<string, Guid>>();

            foreach (var chalkableDepartment in departments)
            {
                pairs.AddRange(chalkableDepartment.Keywords.ToLower().Split(sep, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => new Pair<string, Guid>(x, chalkableDepartment.Id)));
            }
            return pairs;
        }

        private Guid? FindClosestDepartment(string name)
        {
            int minDist = int.MaxValue;
            Pair<string, Guid> closestDep = null;
            for (int i = 0; i < departmenPairs.Count; i++)
            {
                var d = StringTools.LevenshteinDistance(name, departmenPairs[i].First);
                if (d < minDist && (d <= 4 || d <= 0.3 * name.Length + 2))
                {
                    minDist = d;
                    closestDep = departmenPairs[i];
                } else if (name.Contains(departmenPairs[i].First))
                {
                    minDist = name.Length - departmenPairs[i].First.Length;
                    closestDep = departmenPairs[i];
                }
            }
            return closestDep?.Second;
        }

        private Class Selecor(Course course)
        {
            return new Class
            {
                ChalkableDepartmentRef = FindClosestDepartment(course.ShortName.ToLower()),
                Description = course.FullName,
                MinGradeLevelRef = course.MinGradeLevelID,
                MaxGradeLevelRef = course.MaxGradeLevelID,
                Id = course.CourseID,
                ClassNumber = course.FullSectionNumber,
                Name = course.ShortName,
                SchoolYearRef = course.AcadSessionID,
                PrimaryTeacherRef = course.PrimaryTeacherID,
                RoomRef = course.RoomID,
                CourseRef = course.SectionOfCourseID,
                GradingScaleRef = course.GradingScaleID,
                CourseTypeRef = course.CourseTypeID
            };
        }

        protected override void InsertInternal(IList<Course> entities)
        {
            var classes = entities.Select(Selecor).ToList();
            ServiceLocatorSchool.ClassService.Add(classes);
        }

        protected override void UpdateInternal(IList<Course> entities)
        {
            var classes = entities.Select(Selecor).ToList();
            ServiceLocatorSchool.ClassService.Edit(classes);
        }

        protected override void DeleteInternal(IList<Course> entities)
        {
            var courses = entities.Select(x => new Class
                {
                    Id = x.CourseID
                }).ToList();
            ServiceLocatorSchool.ClassService.Delete(courses);
        }

        protected override void PrepareToDeleteInternal(IList<Course> entities)
        {
            var courses = entities.Select(x => new Class
            {
                Id = x.CourseID
            }).ToList();
            ServiceLocatorSchool.ClassService.PrepareToDelete(courses);
        }
    }
}