REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.StandardsGradingScaleId');

NAMESPACE('chlk.models.grading', function () {
    "use strict";

    /** @class chlk.models.grading.ClassroomOptionViewData*/
    CLASS('ClassroomOptionViewData', [
        [ria.serialize.SerializeProperty('classid')],
        chlk.models.id.ClassId, 'classId',

        [ria.serialize.SerializeProperty('averagingmethod')],
        String, 'averagingMethod',

        [ria.serialize.SerializeProperty('categoryaveraging')],
        Boolean, 'categoryAveraging',

        [ria.serialize.SerializeProperty('includewithdrawnstudents')],
        Boolean, 'includeWithdrawnStudents',

        [ria.serialize.SerializeProperty('displaystudentaverage')],
        Boolean, 'displayStudentAverage',

        [ria.serialize.SerializeProperty('displaytotalpoints')],
        Boolean, 'displayTotalPoints',

        [ria.serialize.SerializeProperty('rounddisplayedaverages')],
        Boolean, 'roundDisplayedAverages',

        [ria.serialize.SerializeProperty('displayalphagrade')],
        Boolean, 'displayAlphaGrade',

        [ria.serialize.SerializeProperty('standardsgradingscaleid')],
        chlk.models.id.StandardsGradingScaleId, 'standardsGradingScaleId',

        [ria.serialize.SerializeProperty('standardscalculationmethod')],
        String, 'standardsCalculationMethod',

        [ria.serialize.SerializeProperty('standardscalculationrule')],
        String, 'standardsCalculationRule',

        [ria.serialize.SerializeProperty('standardscalculationweightmaximumvalues')],
        Boolean, 'standardsCalculationWeightMaximumValues'
    ]);
});
