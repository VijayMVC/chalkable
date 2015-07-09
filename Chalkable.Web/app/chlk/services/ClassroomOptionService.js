REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.grading.ClassroomOptionViewData');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.ClassroomOptionService */
    CLASS(
        'ClassroomOptionService', EXTENDS(chlk.services.BaseService), [
            [[chlk.models.id.ClassId]],
            ria.async.Future, function getClassroomOption(classId) {
                return this.get('ClassroomOption/Get.json', chlk.models.grading.ClassroomOptionViewData, {
                    classId : classId.valueOf()
                });
            },

            [[chlk.models.id.ClassId, String, Boolean, Boolean, Boolean, Boolean, Boolean, Boolean, chlk.models.id.StandardsGradingScaleId,
                String, String, Boolean]],
            ria.async.Future, function updateClassroomOption(classId, averagingMethod, categoryAveraging_, includeWithdrawnStudents_,
                displayStudentAverage_, displayTotalPoints_, roundDisplayedAverages_, displayAlphaGrade_, standardsGradingScaleId_,
                standardsCalculationMethod_, standardsCalculationRule_, standardsCalculationWeightMaximumValues_) {
                return this.get('ClassroomOption/Update.json', chlk.models.grading.ClassroomOptionViewData, {
                    classId : classId.valueOf(),
                    averagingMethod : averagingMethod.replace(' ', ''),
                    categoryAveraging: categoryAveraging_,
                    includeWithdrawnStudents: includeWithdrawnStudents_,
                    displayStudentAverage: displayStudentAverage_,
                    displayTotalPoints: displayTotalPoints_,
                    roundDisplayedAverages: roundDisplayedAverages_,
                    displayAlphaGrade: displayAlphaGrade_,
                    standardsGradingScaleId: standardsGradingScaleId_ && standardsGradingScaleId_.valueOf(),
                    standardsCalculationMethod: standardsCalculationMethod_,
                    standardsCalculationRule: standardsCalculationRule_,
                    standardsCalculationWeightMaximumValues: standardsCalculationWeightMaximumValues_
                });
            }
        ])
});