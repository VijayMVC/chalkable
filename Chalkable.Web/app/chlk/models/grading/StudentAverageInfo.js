REQUIRE('chlk.models.id.SchoolPersonId');

NAMESPACE('chlk.models.grading', function () {
    "use strict";

    /** @class chlk.models.grading.ShortStudentAverageInfo*/
    CLASS(
        'ShortStudentAverageInfo', [

            [ria.serialize.SerializeProperty('avgvalue')],
            Number, 'numericValue',

            [ria.serialize.SerializeProperty('alphagradevalue')],
            String, 'alphaGradeValue',

            [ria.serialize.SerializeProperty('studentid')],
            chlk.models.id.SchoolPersonId, 'studentId'

    ]);

    /** @class chlk.models.grading.StudentAverageInfo*/
    CLASS('StudentAverageInfo',[

        [ria.serialize.SerializeProperty('averageid')],
        Number, 'averageId',

        [ria.serialize.SerializeProperty('averagename')],
        String, 'averageName',

        ArrayOf(chlk.models.grading.ShortStudentAverageInfo), 'averages',

        [ria.serialize.SerializeProperty('isgradingperiodaverage')],
        Boolean, 'gradingPeriodAverage',

        [ria.serialize.SerializeProperty('totalaverage')],
        Number, 'totalAverage'

    ]);
});
