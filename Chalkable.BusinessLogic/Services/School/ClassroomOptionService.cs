using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IClassroomOptionService
    {
        void Add(IList<ClassroomOption> classroomOptions);
        void Edit(IList<ClassroomOption> classroomOptions);
        void Delete(IList<ClassroomOption> classroomOptions);
        ClassroomOption GetClassOption(int classId, bool useInowApi = false);
        ClassroomOption SetUpClassroomOption(ClassroomOption classroomOption);
    }

    public class ClassroomOptionService : SisConnectedService, IClassroomOptionService
    {
        public ClassroomOptionService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public void Add(IList<ClassroomOption> classroomOptions)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u=>new DataAccessBase<ClassroomOption>(u).Insert(classroomOptions));
        }

        public void Edit(IList<ClassroomOption> classroomOptions)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<ClassroomOption>(u).Update(classroomOptions));
        }

        public void Delete(IList<ClassroomOption> classroomOptions)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<ClassroomOption>(u).Delete(classroomOptions));
        }

        public ClassroomOption GetClassOption(int classId, bool useInowApi = false)
        {
            if (useInowApi)
            {
                var stiClassrommOption = ConnectorLocator.ClassroomOptionConnector.GetClassroomOption(classId);
                return stiClassrommOption != null ? CreateClassroomOption(stiClassrommOption) : null;
            }
            return DoRead(u => new DataAccessBase<ClassroomOption, int>(u).GetByIdOrNull(classId));
        }

        public ClassroomOption SetUpClassroomOption(ClassroomOption classroomOption)
        {
            var inowClassroomOption = CreateInowClassroomOption(classroomOption);
            ConnectorLocator.ClassroomOptionConnector.UpdateClassroomOption(inowClassroomOption.SectionId, inowClassroomOption);
            inowClassroomOption = ConnectorLocator.ClassroomOptionConnector.GetClassroomOption(inowClassroomOption.SectionId);
            return classroomOption;
        }

        private ClassroomOption CreateClassroomOption(StiConnector.Connectors.Model.ClassroomOption stiClassoption)
        {
            return new ClassroomOption
                {
                    Id = stiClassoption.SectionId,
                    AveragingMethod = stiClassoption.AveragingMethod,
                    CategoryAveraging = stiClassoption.CategoryAveraging,
                    DisplayAlphaGrade = stiClassoption.DisplayAlphaGrades,
                    DisplayStudentAverage = stiClassoption.DisplayStudentAverage,
                    DisplayTotalPoints = stiClassoption.DisplayTotalPoints,
                    IncludeWithdrawnStudents = stiClassoption.IncludeWithdrawnStudents,
                    StandardsCalculationMethod = stiClassoption.StandardsCalculationMethod,
                    StandardsCalculationRule = stiClassoption.StandardsCalculationRule,
                    StandardsCalculationWeightMaximumValues = stiClassoption.StandardsCalculationWeightMaximumValues,
                    StandardsGradingScaleRef = stiClassoption.StandardsGradingScaleId,
                    RoundDisplayedAverages = stiClassoption.RoundAverages
                };
        }

        private StiConnector.Connectors.Model.ClassroomOption CreateInowClassroomOption(ClassroomOption stiClassoption)
        {
            return new StiConnector.Connectors.Model.ClassroomOption
            {
                SectionId = stiClassoption.Id,
                AveragingMethod = stiClassoption.AveragingMethod,
                CategoryAveraging = stiClassoption.CategoryAveraging,
                DisplayAlphaGrades = stiClassoption.DisplayAlphaGrade,
                DisplayStudentAverage = stiClassoption.DisplayStudentAverage,
                DisplayTotalPoints = stiClassoption.DisplayTotalPoints,
                IncludeWithdrawnStudents = stiClassoption.IncludeWithdrawnStudents,
                StandardsCalculationMethod = stiClassoption.StandardsCalculationMethod,
                StandardsCalculationRule = stiClassoption.StandardsCalculationRule,
                StandardsCalculationWeightMaximumValues = stiClassoption.StandardsCalculationWeightMaximumValues,
                StandardsGradingScaleId = stiClassoption.StandardsGradingScaleRef,
                RoundAverages = stiClassoption.RoundDisplayedAverages
            };
        }
    }
}
