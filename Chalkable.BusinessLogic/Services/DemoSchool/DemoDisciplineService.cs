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
            var disciplineRefferals = Storage.StiDisciplineStorage.GetList(classId, date);
            if (disciplineRefferals != null)
            {
                IList<Person> students = new List<Person>();
                if (personId.HasValue)
                {
                    disciplineRefferals = disciplineRefferals.Where(x => x.StudentId == personId).ToList();
                    students.Add(ServiceLocator.PersonService.GetPerson(personId.Value));
                }
                else students = ServiceLocator.ClassService.GetStudents(classId);
                var cClass = ServiceLocator.ClassService.GetClassById(classId);

                if (disciplineRefferals.Count > 0)
                {

                }

                var res = new List<ClassDisciplineDetails>();
                foreach (var student in students)
                {
                    var discipline = new ClassDisciplineDetails { Class = cClass, Student = student };
                    var discRefferal = disciplineRefferals.FirstOrDefault(x => x.StudentId == student.Id);
                    if (discRefferal != null)
                        MapperFactory.GetMapper<ClassDiscipline, DisciplineReferral>()
                                     .Map(disciplineRefferals, discRefferal);
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

        public ClassDiscipline SetClassDiscipline(ClassDiscipline classDiscipline)
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
            return classDiscipline;
        }

        public ClassDiscipline SetClassDiscipline(int classPersonId, int classPeriodId, DateTime date, ISet<int> disciplineTypes, string description)
        {
            return Storage.DisciplineStorage.SetDiscipline(classPersonId, classPeriodId, date, disciplineTypes, description);
        }

        public void DeleteClassDiscipline(int classPersonId, int classPeriodId, DateTime date)
        {
            Storage.DisciplineStorage.DeleteDiscipline(classPersonId, classPeriodId, date);
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
