REQUIRE('chlk.models.id.SchoolPersonId');

NAMESPACE('chlk.models.grading', function () {
    "use strict";

    /** @class chlk.models.grading.ShortStudentAverageInfo*/
    CLASS(
        'ShortStudentAverageInfo', [

            [ria.serialize.SerializeProperty('calculatedavg')],
            Number, 'calculatedAvg',

            [ria.serialize.SerializeProperty('enteredavg')],
            Number, 'enteredAvg',

            [ria.serialize.SerializeProperty('calculatedalphagrade')],
            Number, 'calculatedAlphaGrade',

            [ria.serialize.SerializeProperty('enteredalphagrade')],
            Number, 'enteredAlphaGrade',

            [ria.serialize.SerializeProperty('studentid')],
            chlk.models.id.SchoolPersonId, 'studentId',

            function getNumericAvg(){
                return this.getEnteredAvg() || this.getCalculatedAvg()
            },

            function getAlphaGrade(){
                return this.getEnteredAlphaGrade() || this.getCalculatedAlphaGrade()
            }

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
