﻿using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Mapping.ModelMappers;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage.sti;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;
using Infraction = Chalkable.Data.School.Model.Infraction;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoDisciplineService : DemoSchoolServiceBase, IDisciplineService
    {
        private DemoStiDisciplineStorage StiDisciplineStorage { get; set; }
        public DemoDisciplineService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
            StiDisciplineStorage =new DemoStiDisciplineStorage();
        }

        public IList<ClassDisciplineDetails> GetClassDisciplineDetails(int classId, DateTime date)
        {
            var mp = ServiceLocator.MarkingPeriodService.GetMarkingPeriodByDate(date);
            if (mp == null) return null;
            var disciplineRefferals = StiDisciplineStorage.GetList(classId, date);
            var options = ServiceLocator.ClassroomOptionService.GetClassOption(classId);
            if (disciplineRefferals != null)
            {
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
            return null;
        }

        public ClassDisciplineDetails SetClassDiscipline(ClassDiscipline classDiscipline)
        {
            if (!classDiscipline.ClassId.HasValue)
                throw new ChalkableException("Invalid classId param");

            var stiDiscipline = new DisciplineReferral();
            MapperFactory.GetMapper<DisciplineReferral, ClassDiscipline>().Map(stiDiscipline, classDiscipline);

            if (classDiscipline.Id.HasValue)
                StiDisciplineStorage.Update(stiDiscipline);
            else
            {
                stiDiscipline = StiDisciplineStorage.Create(stiDiscipline);
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

        public ClassDiscipline SetClassDiscipline(int classPersonId, int classPeriodId, DateTime date, ISet<int> disciplineTypes, string description)
        {
            throw new NotImplementedException();
        }

        public void DeleteClassDiscipline(int classPersonId, int classPeriodId, DateTime date)
        {
            throw new NotImplementedException();
        }

        public IList<ClassDisciplineDetails> GetClassDisciplineDetails(int schoolYearId, int personId, DateTime start, DateTime end, bool needsAllData = false)
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
