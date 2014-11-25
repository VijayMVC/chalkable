﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chalkable.BusinessLogic.Mapping.ModelMappers;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;
using Infraction = Chalkable.Data.School.Model.Infraction;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IDisciplineService
    {

        IList<ClassDisciplineDetails> GetClassDisciplineDetails(int classId, DateTime date);
        ClassDisciplineDetails SetClassDiscipline(ClassDiscipline classDiscipline);
        //TODO: old
        ClassDiscipline SetClassDiscipline(int classPersonId, int classPeriodId, DateTime date, ISet<int> disciplineTypes, string description);
        void DeleteClassDiscipline(int classPersonId, int classPeriodId, DateTime date);
        IList<ClassDisciplineDetails> GetClassDisciplineDetails(ClassDisciplineQuery query, IList<int> gradeLevelIds = null);
        IList<ClassDisciplineDetails> GetClassDisciplineDetails(int schoolYearId, int personId, DateTime start, DateTime end, bool needsAllData = false);
        //IList<ClassDisciplineDetails> GetClassDisciplineDetails(int schoolYearId, DateTime date);
        //IList<DisciplineTotalPerType> CalcDisciplineTypeTotalForStudent(int studentId, int? markingPeriodId, int? schoolYearId, DateTime? fromDate, DateTime? toDate);
        //IList<DisciplineTotalPerType> CalcDisciplineTypeTotalForStudents(IList<int> studentIds, int? markingPeriodId, int? schoolYearId, DateTime? fromDate, DateTime? toDate);
    }

    public class DisciplineService : SisConnectedService, IDisciplineService
    {
        public DisciplineService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }


        public IList<ClassDisciplineDetails> GetClassDisciplineDetails(int classId, DateTime date)
        {
            //var classPeriod = ServiceLocator.ClassPeriodService.GetNearestClassPeriod(classId, date);
            //if (classPeriod == null) return null;
            var mp = ServiceLocator.MarkingPeriodService.GetMarkingPeriodByDate(date);
            if (mp == null) return null;
            var disciplineRefferals = ConnectorLocator.DisciplineConnector.GetList(classId, date);
            var options = ServiceLocator.ClassroomOptionService.GetClassOption(classId);
            if (disciplineRefferals != null)
            {
                var students = ServiceLocator.PersonService.GetClassStudents(classId, mp.Id
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
            if(!classDiscipline.ClassId.HasValue)
                throw new ChalkableException("Invalid classId param");
            Trace.Assert(Context.SchoolYearId.HasValue);
            
            var stiDiscipline = new DisciplineReferral();
            MapperFactory.GetMapper<DisciplineReferral, ClassDiscipline>().Map(stiDiscipline, classDiscipline);

            if (classDiscipline.Id.HasValue)
            {
                if (classDiscipline.Infractions == null || classDiscipline.Infractions.Count == 0)
                {
                    ConnectorLocator.DisciplineConnector.Delete(classDiscipline.Id.Value, classDiscipline.ClassId.Value, classDiscipline.Date);
                    classDiscipline.Id = null;
                }
                else 
                    ConnectorLocator.DisciplineConnector.Update(stiDiscipline);                
            }
            else
            {               
                stiDiscipline = ConnectorLocator.DisciplineConnector.Create(stiDiscipline);
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
        
        //TODO: needs test... security
        //public IList<ClassDisciplineDetails> GetClassDisciplineDetails(ClassDisciplineQuery query, IList<Guid> gradeLevelIds = null)
        //{
        //    using (var uow = Read())
        //    {
        //        var res = new ClassDisciplineDataAccess(uow).GetClassDisciplinesDetails(query);
        //        if (gradeLevelIds != null && gradeLevelIds.Count > 0)
        //            res = res.Where(x => gradeLevelIds.Contains(x.Class.GradeLevelRef)).ToList();
        //        return res;
        //    }
        //}

        //public IList<ClassDisciplineDetails> GetClassDisciplineDetails(Guid schoolYearId, Guid personId, DateTime start, DateTime end, bool needsAllData = false)
        //{
        //    return GetClassDisciplineDetails(new ClassDisciplineQuery
        //        {
        //            SchoolYearId = schoolYearId,
        //            PersonId = personId,
        //            FromDate = start,
        //            ToDate = end,
        //            NeedAllData = needsAllData
        //        });
        //}

        //public ClassDiscipline SetClassDiscipline(Guid classPersonId, Guid classPeriodId, DateTime date, ISet<Guid> disciplineTypes, string description)
        //{
        //    using (var uow = Update())
        //    {
        //        var cPeriodDa = new ClassPeriodDataAccess(uow);
        //        var cPeriod = cPeriodDa.GetClassPeriods(new ClassPeriodQuery { Id = classPeriodId }).First();
        //        var c = new ClassDataAccess(uow).GetById(cPeriod.ClassRef);
        //        if (!BaseSecurity.IsAdminEditorOrClassTeacher(c, Context))
        //            throw new ChalkableSecurityException();
                
        //        var dateDa = new DateDataAccess(uow);
        //        if (!dateDa.Exists(new DateQuery { ToDate = date, FromDate = date, SectionRef = cPeriod.Period.SectionRef }))
        //            throw new ChalkableException(ChlkResources.ERR_CLASS_IS_NOT_SCHEDULED_FOR_DAY);

        //        var disciplineDa = new ClassDisciplineDataAccess(uow);
        //        var discipline = disciplineDa.GetClassDiscipline(classPeriodId, classPersonId, date);
        //        bool insertDiscipline = false;
        //        if (discipline == null)
        //        {
        //            discipline = new ClassDiscipline { Id = Guid.NewGuid() };
        //            insertDiscipline = true;
        //        }
        //        discipline.ClassPeriodRef = classPeriodId;
        //        discipline.ClassPersonRef = classPersonId;
        //        discipline.Date = date;
        //        discipline.Description = description;

        //        if(insertDiscipline) disciplineDa.Insert(discipline);
        //        else disciplineDa.Update(discipline);

        //        var cDisciplineTypes = new List<ClassDisciplineType>();
        //        var clDiscTypeDa = new ClassDisciplineTypeDataAccess(uow);
        //        clDiscTypeDa.Delete(discipline.Id, null);
        //        foreach (var disciplineType in disciplineTypes)
        //        {
        //            cDisciplineTypes.Add(new ClassDisciplineType 
        //                        {
        //                            Id = Guid.NewGuid(),
        //                            ClassDisciplineRef = discipline.Id,
        //                            DisciplineTypeRef = disciplineType
        //                        });
        //        }
        //        clDiscTypeDa.Insert(cDisciplineTypes);
        //        uow.Commit();
        //        return discipline;
        //    }
        //}

        //public void DeleteClassDiscipline(Guid classPersonId, Guid classPeriodId, DateTime date)
        //{
        //    using (var uow = Update())
        //    {
        //        var cPeriodDa = new ClassPeriodDataAccess(uow);
        //        var cPeriod = cPeriodDa.GetClassPeriods(new ClassPeriodQuery { Id = classPeriodId }).First();
        //        var c = new ClassDataAccess(uow).GetById(cPeriod.ClassRef);
        //        if (!BaseSecurity.IsAdminEditorOrClassTeacher(c, Context))
        //            throw new ChalkableSecurityException();
                
        //        var dateDa = new DateDataAccess(uow);
        //        if (!dateDa.Exists(new DateQuery { ToDate = date, FromDate = date, SectionRef = cPeriod.Period.SectionRef }))
        //            throw new ChalkableException(ChlkResources.ERR_CLASS_IS_NOT_SCHEDULED_FOR_DAY);

        //        var disciplineDa = new ClassDisciplineDataAccess(uow);
        //        var discipline = disciplineDa.GetClassDiscipline(classPeriodId, classPersonId, date);
        //        if (discipline != null)
        //        {
        //            var classDiscTypeDa = new ClassDisciplineTypeDataAccess(uow);
        //            classDiscTypeDa.Delete(discipline.Id, null);
        //            disciplineDa.Delete(discipline.Id);
        //        }
        //        uow.Commit();
        //    }
        //}

        //public IList<ClassDisciplineDetails> GetClassDisciplineDetails(Guid schoolYearId, DateTime date)
        //{
        //    return GetClassDisciplineDetails(new ClassDisciplineQuery
        //        {
        //            SchoolYearId = schoolYearId,
        //            FromDate = date,
        //            ToDate = date
        //        });
        //}
        
        //private IList<DisciplineTotalPerType> CalcDisciplineTypeTotal(Guid? studentId, Guid? markingPeriodId, Guid? schoolYearId, DateTime? fromDate, DateTime? toDate) 
        //{
        //    using (var uow = Read())
        //    {
        //        return  new ClassDisciplineDataAccess(uow).CalcDisciplineTypeTotal(schoolYearId, 
        //            markingPeriodId, studentId, fromDate, toDate);
        //    }
        //}
        ////TODO: needs test
        //public IList<DisciplineTotalPerType> CalcDisciplineTypeTotalForStudent(Guid studentId, Guid? markingPeriodId, Guid? schoolYearId, DateTime? fromDate, DateTime? toDate)
        //{
        //    return CalcDisciplineTypeTotal(studentId, markingPeriodId, schoolYearId, fromDate, toDate);
        //}

        //public IList<DisciplineTotalPerType> CalcDisciplineTypeTotalForStudents(IList<Guid> studentIds, Guid? markingPeriodId, Guid? schoolYearId, DateTime? fromDate, DateTime? toDate)
        //{
        //    var res = CalcDisciplineTypeTotal(null, markingPeriodId, schoolYearId, fromDate, toDate);
        //    return res.Where(x=>studentIds.Contains(x.PersonId)).ToList();
        //}


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

        /*public IList<ClassDisciplineDetails> GetClassDisciplineDetails(int schoolYearId, DateTime date)
        {
            throw new NotImplementedException();
        }*/

        //public IList<DisciplineTotalPerType> CalcDisciplineTypeTotalForStudent(int studentId, int? markingPeriodId, int? schoolYearId, DateTime? fromDate, DateTime? toDate)
        //{
        //    throw new NotImplementedException();
        //}

        //public IList<DisciplineTotalPerType> CalcDisciplineTypeTotalForStudents(IList<int> studentIds, int? markingPeriodId, int? schoolYearId, DateTime? fromDate, DateTime? toDate)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
