using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Mapping.ModelMappers;
using Chalkable.BusinessLogic.Model;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;
using Infraction = Chalkable.Data.School.Model.Infraction;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IDisciplineService
    {
        IList<ClassDisciplineDetails> GetClassDisciplineDetails(int classId, DateTime date);
        ClassDisciplineDetails SetClassDiscipline(ClassDiscipline classDiscipline);
        IList<ClassDisciplineDetails> GetDisciplineByDateRange(int studentId, DateTime start, DateTime end);
        IList<InfractionSummaryInfo> GetStudentInfractionSummary(int studentId, int? gradingPeriodId);
        Task<IList<DisciplineDailySummaryInfo>> GetClassDisciplineDailySummary(int classId, DateTime startDate, DateTime endDate);
    }

    public class DisciplineService : SisConnectedService, IDisciplineService
    {
        public DisciplineService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }


        public IList<ClassDisciplineDetails> GetClassDisciplineDetails(int classId, DateTime date)
        {
            var mp = ServiceLocator.MarkingPeriodService.GetLastClassMarkingPeriod(classId, date);
            if (mp == null)
                return new List<ClassDisciplineDetails>();

            var disciplineRefferals = ConnectorLocator.DisciplineConnector.GetList(classId, date);
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
                        MapperFactory.GetMapper<ClassDiscipline, DisciplineReferral>().Map(discipline, discRefferal);
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
            DemandClassId(classDiscipline.ClassId);
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
              
        private void DemandClassId(int? classId)
        {
            if (!classId.HasValue)
                throw new ChalkableException("Invalid classId param");
        }

        public IList<ClassDisciplineDetails> GetDisciplineByDateRange(int studentId, DateTime start, DateTime end)
        {
            var syId = ServiceLocator.SchoolYearService.GetCurrentSchoolYear().Id;
            var student = ServiceLocator.StudentService.GetById(studentId, syId);
            var classes = ServiceLocator.ClassService.GetStudentClasses(syId, studentId);
            var infractions = ServiceLocator.InfractionService.GetInfractions(true);
            var stiDiscipline = ConnectorLocator.StudentConnector.GetStudentDisciplineDetailDashboard(studentId, syId, start, end);
            var res = new List<ClassDisciplineDetails>();
            foreach (var referral in stiDiscipline.Referrals)
            {
                var classDetails = classes.FirstOrDefault(c => c.Id == referral.SectionId);
                if (classDetails != null)
                {
                    res.Add(new ClassDisciplineDetails
                    {
                        Id = referral.Id,
                        ClassId = referral.SectionId,
                        Date = referral.Date,
                        Description = referral.Note,
                        StudentId = referral.StudentId,
                        Student = student,
                        Class = classDetails,
                        Infractions = infractions.Where(x=>referral.Infractions.Any(y=> y.Id == x.Id)).ToList()
                    });
                }

            }
            return res;
        }
        
        public IList<InfractionSummaryInfo> GetStudentInfractionSummary(int studentId, int? gradingPeriodId)
        {
            var syId = ServiceLocator.SchoolYearService.GetCurrentSchoolYear().Id;
            var stiDisciplineSummary = ConnectorLocator.StudentConnector.GetStudentDisciplineSummary(studentId, syId, gradingPeriodId);
            var infractions = ServiceLocator.InfractionService.GetInfractions(true);
            return InfractionSummaryInfo.Create(stiDisciplineSummary.Infractions.ToList(), infractions);
        }

        public async Task<IList<DisciplineDailySummaryInfo>> GetClassDisciplineDailySummary(int classId, DateTime startDate, DateTime endDate)
        {
            var inowRes = await ConnectorLocator.SectionDashboardConnector.GetDisciplineSummaryDashboard(classId, startDate, endDate);
            return inowRes.Select(x=> new DisciplineDailySummaryInfo
            {
                Date = x.Date,
                Occurrences = x.Infractions
            }).ToList();
        }
    }
}
