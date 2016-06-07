using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models
{
    public class ClassroomOptionViewData
    {
        public int ClassId { get; set; }
        public int AveragingMethod { get; set; }
        public bool IncludeWithdrawnStudents { get; set; }
        public bool DisplayStudentAverage { get; set; }
        public bool DisplayTotalPoints { get; set; }
        public bool RoundDisplayedAverages { get; set; }
        public bool DisplayAlphaGrade { get; set; }
        public int? StandardsGradingScaleId { get; set; }
        public string StandardsCalculationMethod { get; set; }
        public string StandardsCalculationRule { get; set; }
        public bool StandardsCalculationWeightMaximumValues { get; set; }

        public static ClassroomOptionViewData Create(ClassroomOption classroomOption)
        {
            return new ClassroomOptionViewData
            {
                ClassId = classroomOption.Id,
                AveragingMethod = (int)classroomOption.AveragingMethodType,
                DisplayAlphaGrade = classroomOption.DisplayAlphaGrade,
                DisplayStudentAverage = classroomOption.DisplayStudentAverage,
                DisplayTotalPoints = classroomOption.DisplayTotalPoints,
                IncludeWithdrawnStudents = classroomOption.IncludeWithdrawnStudents,
                RoundDisplayedAverages = classroomOption.RoundDisplayedAverages,
                StandardsCalculationMethod = classroomOption.StandardsCalculationMethod,
                StandardsCalculationRule = classroomOption.StandardsCalculationRule,
                StandardsCalculationWeightMaximumValues = classroomOption.StandardsCalculationWeightMaximumValues,
                StandardsGradingScaleId = classroomOption.StandardsGradingScaleRef
            };
        }
    }
}