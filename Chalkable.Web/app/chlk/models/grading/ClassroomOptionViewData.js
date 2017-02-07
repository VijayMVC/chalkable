REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.id.StandardsGradingScaleId');
REQUIRE('ria.serialize.SJX');

NAMESPACE('chlk.models.grading', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.grading.ClassroomOptionViewData*/
    CLASS('ClassroomOptionViewData', IMPLEMENTS(ria.serialize.IDeserializable), [
        chlk.models.id.ClassId, 'copyToClassId',
        chlk.models.id.ClassId, 'importFromClassId',
        chlk.models.id.ClassId, 'classId',
        String, 'submitType',
        String, 'copyToYearName',
        String, 'importFromYearName',
        String, 'copyToClassName',
        String, 'importFromClassName',

        String, 'averagingMethod',
        Boolean, 'categoryAveraging',
        Boolean, 'includeWithdrawnStudents',
        Boolean, 'displayStudentAverage',
        Boolean, 'displayTotalPoints',
        Boolean, 'roundDisplayedAverages',
        Boolean, 'displayAlphaGrade',
        chlk.models.id.StandardsGradingScaleId, 'standardsGradingScaleId',
        String, 'standardsCalculationMethod',
        String, 'standardsCalculationRule',
        Boolean, 'standardsCalculationWeightMaximumValues',

        VOID, function deserialize(raw) {
            this.copyToClassId = SJX.fromValue(raw.copyToClassId, chlk.models.id.ClassId);
            this.importFromClassId = SJX.fromValue(raw.importFromClassId, chlk.models.id.ClassId);
            this.classId = SJX.fromValue(raw.classid, chlk.models.id.ClassId);
            this.submitType = SJX.fromValue(raw.submitType, String);
            this.copyToYearName = SJX.fromValue(raw.copyToYearName, String);
            this.importFromYearName = SJX.fromValue(raw.importFromYearName, String);
            this.copyToClassName = SJX.fromValue(raw.copyToClassName, String);
            this.importFromClassName = SJX.fromValue(raw.importFromClassName, String);

            this.averagingMethod = SJX.fromValue(raw.averagingmethod, String);
            this.categoryAveraging = SJX.fromValue(raw.categoryaveraging, Boolean);
            this.includeWithdrawnStudents = SJX.fromValue(raw.includewithdrawnstudents, Boolean);
            this.displayStudentAverage = SJX.fromValue(raw.displaystudentaverage, Boolean);
            this.displayTotalPoints = SJX.fromValue(raw.displaytotalpoints, Boolean);
            this.roundDisplayedAverages = SJX.fromValue(raw.rounddisplayedaverages, Boolean);
            this.displayAlphaGrade = SJX.fromValue(raw.displayalphagrade, Boolean);
            this.standardsGradingScaleId = SJX.fromValue(raw.standardsgradingscaleid, chlk.models.id.StandardsGradingScaleId);
            this.standardsCalculationMethod = SJX.fromValue(raw.standardscalculationmethod, String);
            this.standardsCalculationRule = SJX.fromValue(raw.standardscalculationrule, String);
            this.standardsCalculationWeightMaximumValues = SJX.fromValue(raw.standardscalculationweightmaximumvalues, Boolean);
        }
    ]);
});
