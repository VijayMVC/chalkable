REQUIRE('chlk.models.id.StandardId');
REQUIRE('chlk.models.id.GradingPeriodId');
REQUIRE('chlk.models.id.GradeId');

NAMESPACE('chlk.models.standard', function () {
    "use strict";
    /** @class chlk.models.standard.StandardGrading*/
    CLASS(
        'StandardGrading', [
            [ria.serialize.SerializeProperty('gradingperiodid')],
            chlk.models.id.GradingPeriodId, 'gradingPeriodId',

            [ria.serialize.SerializeProperty('standardid')],
            chlk.models.id.StandardId, 'standardId',

            [ria.serialize.SerializeProperty('gradeid')],
            chlk.models.id.GradeId, 'gradeId',

            [ria.serialize.SerializeProperty('studentid')],
            chlk.models.id.SchoolPersonId, 'studentId',

            [ria.serialize.SerializeProperty('gradevalue')],
            String, 'gradeValue'
        ]);
});
