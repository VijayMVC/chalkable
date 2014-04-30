REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.GradingPeriodId');
REQUIRE('chlk.models.grading.AvgCodeHeaderViewData');

NAMESPACE('chlk.models.grading', function () {
    "use strict";

    /** @class chlk.models.grading.ShortStudentAverageInfo*/
    CLASS(
        'ShortStudentAverageInfo', [
            [ria.serialize.SerializeProperty('averageid')],
            Number, 'averageId',

            [ria.serialize.SerializeProperty('calculatedavg')],
            Number, 'calculatedAvg',

            [ria.serialize.SerializeProperty('enteredavg')],
            Number, 'enteredAvg',

            [ria.serialize.SerializeProperty('calculatedalphagrade')],
            String, 'calculatedAlphaGrade',

            [ria.serialize.SerializeProperty('enteredalphagrade')],
            String, 'enteredAlphaGrade',

            [ria.serialize.SerializeProperty('studentid')],
            chlk.models.id.SchoolPersonId, 'studentId',

            chlk.models.id.ClassId, 'classId',

            chlk.models.id.GradingPeriodId, 'gradingPeriodId',

            String, 'averageValue',

            String, 'oldValue',

            //ArrayOf(chlk.models.grading.AvgCodeHeaderViewData), 'codes',

            Array, 'codes',

            String, 'codesString',

            function getNumericAvg(){
                return this.getEnteredAvg() || this.getEnteredAvg() == 0 ? this.getEnteredAvg()  : this.getCalculatedAvg()
            },

            function getAlphaGrade(){
                return this.getEnteredAlphaGrade() || this.getCalculatedAlphaGrade()
            },

            [[Boolean, Boolean]],
            String, function displayAvgGradeValue(isAbleDisplayAlphaGrades_, original_){
                var alphaGrade = original_ ? this.getCalculatedAlphaGrade() : this.getAlphaGrade();
                var res = this.displayGrade(original_ ? this.getCalculatedAvg() : this.getNumericAvg());
                if(res && this.getNumericAvg() != 0 && isAbleDisplayAlphaGrades_ && alphaGrade && alphaGrade.trim() != ''){
                    res += '(' + alphaGrade + ')';
                }
                return res;
            },

            String, function displayGrade(grade){
                return grade || grade == 0 ? grade.toFixed(2) : '';
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
