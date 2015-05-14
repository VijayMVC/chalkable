using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Mapping.ModelMappers;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;
using Infraction = Chalkable.Data.School.Model.Infraction;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoDisciplineStorage : BaseDemoIntStorage<DisciplineReferral>
    {
        public DemoDisciplineStorage()
            : base(x => x.Id, true)
        {
        }

        public IList<DisciplineReferral> GetList(int classId, DateTime date)
        {
            return data.Where(x => x.Value.SectionId == classId && x.Value.Date == date).Select(x => x.Value).ToList();
        }

        public IList<DisciplineReferral> GetList(DateTime date)
        {
            return data.Where(x => x.Value.Date == date).Select(x => x.Value).ToList();
        }

        public IList<DisciplineReferral> GetSectionDisciplineSummary(int classId, DateTime startDate, DateTime endDate)
        {
            var result = new List<DisciplineReferral>();
            for (var start = startDate; start <= endDate; start = start.AddDays(1))
            {
                result.AddRange(GetList(classId, start));
            }
            return result;
        }
    }

    public class DemoDisciplineService : DemoSchoolServiceBase, IDisciplineService
    {
        private DemoDisciplineStorage DisciplineStorage { get; set; }
        public DemoDisciplineService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
            DisciplineStorage = new DemoDisciplineStorage();
        }

        public IList<DisciplineReferral> GetSectionDisciplineSummary(int classId, DateTime startDate, DateTime endDate)
        {
            return DisciplineStorage.GetSectionDisciplineSummary(classId, startDate, endDate);
        }

        public void AddDisciplineReferral(DisciplineReferral disciplineReferral)
        {
            DisciplineStorage.Add(disciplineReferral);
        }

        public IList<ClassDisciplineDetails> GetClassDisciplineDetails(int classId, DateTime date)
        {
            var mp = ServiceLocator.MarkingPeriodService.GetMarkingPeriodByDate(date);
            if (mp == null) return null;
            var disciplineRefferals = DisciplineStorage.GetList(classId, date);
            var options = ServiceLocator.ClassroomOptionService.GetClassOption(classId);
            if (disciplineRefferals == null) return null;

            var students = ServiceLocator.StudentService.GetClassStudents(classId, mp.Id
                , options != null && options.IncludeWithdrawnStudents ? (bool?)null : true);
            var cClass = ServiceLocator.ClassService.GetClassDetailsById(classId);
            var res = new List<ClassDisciplineDetails>();
            foreach (var student in students)
            {
                var discipline = new ClassDisciplineDetails
                {
                    Class = cClass,
                    Student = student,
                    Infractions = new List<Infraction>()
                };
                var discRefferal = disciplineRefferals.FirstOrDefault(x => x.StudentId == student.Id);
                if (discRefferal != null)
                    MapperFactory.GetMapper<ClassDiscipline, DisciplineReferral>()
                        .Map(discipline, discRefferal);
                else
                {
                    discipline.Date = date;
                    discipline.ClassId = classId;
                    discipline.StudentId = student.Id;
                }
                res.Add(discipline);
            }
            return res;
        }

        public ClassDisciplineDetails SetClassDiscipline(ClassDiscipline classDiscipline)
        {
            if (!classDiscipline.ClassId.HasValue)
                throw new ChalkableException("Invalid classId param");

            var stiDiscipline = new DisciplineReferral();
            MapperFactory.GetMapper<DisciplineReferral, ClassDiscipline>().Map(stiDiscipline, classDiscipline);

            if (classDiscipline.Id.HasValue)
                DisciplineStorage.Update(stiDiscipline);
            else
            {
                stiDiscipline = DisciplineStorage.Add(stiDiscipline);
                MapperFactory.GetMapper<ClassDiscipline, DisciplineReferral>().Map(classDiscipline, stiDiscipline);
            }
            return new ClassDisciplineDetails
            {
                Date = classDiscipline.Date,
                ClassId = classDiscipline.ClassId,
                Description = classDiscipline.Description,
                Id = classDiscipline.Id,
                Infractions = classDiscipline.Infractions,
                StudentId = classDiscipline.StudentId,
                Class = ServiceLocator.ClassService.GetClassDetailsById(classDiscipline.ClassId.Value),
                Student = ServiceLocator.StudentService.GetById(classDiscipline.StudentId, Context.SchoolYearId.Value)
            };
        }

        public IList<DisciplineReferral> GetList(DateTime today)
        {
            return DisciplineStorage.GetList(today);
        }
        
        public IList<ClassDisciplineDetails> GetDisciplineByDateRange(int studentId, DateTime? start, DateTime? end)
        {
            throw new NotImplementedException();
        }

        public IList<InfractionSummaryInfo> GetStudentInfractionSummary(int studentId, int? gradingPeriodId)
        {
            throw new NotImplementedException();
        }
    }
}
