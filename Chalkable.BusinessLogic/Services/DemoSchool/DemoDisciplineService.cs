using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Mapping.ModelMappers;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors;
using Chalkable.StiConnector.Connectors.Model;
using Infraction = Chalkable.Data.School.Model.Infraction;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoDisciplineService : DemoSchoolServiceBase, IDisciplineService
    {
        public DemoDisciplineService(IServiceLocatorSchool serviceLocator, DemoStorage storage) : base(serviceLocator, storage)
        {
        }

        public IList<ClassDisciplineDetails> GetClassDisciplineDetails(int classId, DateTime date, int? personId)
        {
            var classPeriod = ServiceLocator.ClassPeriodService.GetNearestClassPeriod(classId, date);
            if (classPeriod == null) return null;
            var mp = ServiceLocator.MarkingPeriodService.GetMarkingPeriodByDate(date);
            if (mp == null) return null;
            var disciplineRefferals = Storage.StiDisciplineStorage.GetList(classId, date);
            var options = ServiceLocator.ClassroomOptionService.GetClassOption(classId);
            if (disciplineRefferals != null)
            {
                IList<Person> students = new List<Person>();
                if (personId.HasValue)
                {
                    disciplineRefferals = disciplineRefferals.Where(x => x.StudentId == personId).ToList();
                    var student = ServiceLocator.PersonService.GetPerson(personId.Value);
                    var cp = ServiceLocator.ClassService.GetClassPerson(classId, student.Id);
                    if ((cp.IsEnrolled || (options != null && options.IncludeWithdrawnStudents)) && cp.MarkingPeriodRef == mp.Id) students.Add(student);
                }
                else students = ServiceLocator.ClassService.GetStudents(classId
                    , options != null && options.IncludeWithdrawnStudents ? (bool?)null : true, mp.Id);
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
            return null;
        }

        public ClassDisciplineDetails SetClassDiscipline(ClassDiscipline classDiscipline)
        {
            if (!classDiscipline.ClassId.HasValue)
                throw new ChalkableException("Invalid classId param");

            var stiDiscipline = new DisciplineReferral();
            MapperFactory.GetMapper<DisciplineReferral, ClassDiscipline>().Map(stiDiscipline, classDiscipline);

            if (classDiscipline.Id.HasValue)
                Storage.StiDisciplineStorage.Update(stiDiscipline);
            else
            {
                stiDiscipline = Storage.StiDisciplineStorage.Create(stiDiscipline);
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
                Student = ServiceLocator.PersonService.GetPerson(classDiscipline.StudentId)
            };
        }

        public ClassDiscipline SetClassDiscipline(int classPersonId, int classPeriodId, DateTime date, ISet<int> disciplineTypes, string description)
        {
            throw new NotImplementedException();
        }

        public void DeleteClassDiscipline(int classPersonId, int classPeriodId, DateTime date)
        {
            throw new NotImplementedException();
        }

        public IList<ClassDisciplineDetails> GetClassDisciplineDetails(ClassDisciplineQuery query, IList<int> gradeLevelIds = null)
        {
            throw new NotImplementedException();
        }

        public IList<ClassDisciplineDetails> GetClassDisciplineDetails(int schoolYearId, int personId, DateTime start, DateTime end, bool needsAllData = false)
        {
            throw new NotImplementedException();
        }

        public IList<ClassDisciplineDetails> GetClassDisciplineDetails(int schoolYearId, DateTime date)
        {
            throw new NotImplementedException();
        }

        public IList<DisciplineTotalPerType> CalcDisciplineTypeTotalForStudent(int studentId, int? markingPeriodId, int? schoolYearId, DateTime? fromDate, DateTime? toDate)
        {
            throw new NotImplementedException();
        }

        public IList<DisciplineTotalPerType> CalcDisciplineTypeTotalForStudents(IList<int> studentIds, int? markingPeriodId, int? schoolYearId, DateTime? fromDate, DateTime? toDate)
        {
            throw new NotImplementedException();
        }
    }
}
