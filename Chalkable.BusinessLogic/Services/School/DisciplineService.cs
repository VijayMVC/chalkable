using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IDisciplineService
    {
        ClassDiscipline SetClassDiscipline(Guid classPersonId, Guid classPeriodId, DateTime date, ISet<Guid> disciplineTypes, string description);
        void DeleteClassDiscipline(Guid classPersonId, Guid classPeriodId, DateTime date);
        IList<ClassDisciplineDetails> GetClassDisciplineDetails(ClassDisciplineQuery query, IList<Guid> gradeLevelIds = null);
        IList<ClassDisciplineDetails> GetClassDisciplineDetails(Guid schoolYearId, Guid personId, DateTime start, DateTime end, bool needsAllData = false);
        IList<ClassDisciplineDetails> GetClassDisciplineDetails(Guid schoolYearId, DateTime date);
        IList<DisciplineTotalPerType> CalcDisciplineTypeTotalForStudent(Guid studentId, Guid? markingPeriodId, Guid? schoolYearId, DateTime? fromDate, DateTime? toDate);
        IList<DisciplineTotalPerType> CalcDisciplineTypeTotalForStudents(IList<Guid> studentIds, Guid? markingPeriodId, Guid? schoolYearId, DateTime? fromDate, DateTime? toDate);
    }

    public class DisciplineService : SchoolServiceBase, IDisciplineService
    {
        public DisciplineService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
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




        public ClassDiscipline SetClassDiscipline(Guid classPersonId, Guid classPeriodId, DateTime date, ISet<Guid> disciplineTypes, string description)
        {
            throw new NotImplementedException();
        }

        public void DeleteClassDiscipline(Guid classPersonId, Guid classPeriodId, DateTime date)
        {
            throw new NotImplementedException();
        }

        public IList<ClassDisciplineDetails> GetClassDisciplineDetails(ClassDisciplineQuery query, IList<Guid> gradeLevelIds = null)
        {
            throw new NotImplementedException();
        }

        public IList<ClassDisciplineDetails> GetClassDisciplineDetails(Guid schoolYearId, Guid personId, DateTime start, DateTime end, bool needsAllData = false)
        {
            throw new NotImplementedException();
        }

        public IList<ClassDisciplineDetails> GetClassDisciplineDetails(Guid schoolYearId, DateTime date)
        {
            throw new NotImplementedException();
        }

        public IList<DisciplineTotalPerType> CalcDisciplineTypeTotalForStudent(Guid studentId, Guid? markingPeriodId, Guid? schoolYearId, DateTime? fromDate, DateTime? toDate)
        {
            throw new NotImplementedException();
        }

        public IList<DisciplineTotalPerType> CalcDisciplineTypeTotalForStudents(IList<Guid> studentIds, Guid? markingPeriodId, Guid? schoolYearId, DateTime? fromDate, DateTime? toDate)
        {
            throw new NotImplementedException();
        }
    }
}
