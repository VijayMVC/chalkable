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

            [ria.serialize.SerializeProperty('maybeexempt')],
            Boolean, 'mayBeExempt',

            [ria.serialize.SerializeProperty('isexempt')],
            Boolean, 'exempt',

            [ria.serialize.SerializeProperty('studentid')],
            chlk.models.id.SchoolPersonId, 'studentId',

            chlk.models.id.ClassId, 'classId',

            chlk.models.id.GradingPeriodId, 'gradingPeriodId',

            String, 'averageValue',

            String, 'oldValue',

            //ArrayOf(chlk.models.grading.AvgCodeHeaderViewData), 'codes',

            Array, 'codes',

            String, 'codesString',

            [[Number, chlk.models.id.GradingPeriodId, Number, Number, String, String, Boolean, Boolean, chlk.models.id.SchoolPersonId, String]],
            function $(averageId_, gradingPeriodId_, calculatedAvg_, enteredAvg_, calculatedAlphaGrade_, enteredAlphaGrade_, mayBeExempt_, exempt_, studentId_, codesString_){
                BASE();
                if(averageId_)
                    this.setAverageId(averageId_);
                if(gradingPeriodId_)
                    this.setGradingPeriodId(gradingPeriodId_);
                if(calculatedAvg_)
                    this.setCalculatedAvg(calculatedAvg_);
                if(enteredAvg_)
                    this.setEnteredAvg(enteredAvg_);
                if(calculatedAlphaGrade_)
                    this.setCalculatedAlphaGrade(calculatedAlphaGrade_);
                if(enteredAlphaGrade_)
                    this.setEnteredAlphaGrade(enteredAlphaGrade_);
                if(mayBeExempt_)
                    this.setMayBeExempt(mayBeExempt_);
                if(exempt_)
                    this.setExempt(exempt_);
                if(studentId_)
                    this.setStudentId(studentId_);
                if(codesString_)
                    this.setCodesString(codesString_);
            },

            function getNumericAvg(){
                return this.getEnteredAvg() || this.getEnteredAvg() == 0 ? this.getEnteredAvg()  : this.getCalculatedAvg()
            },

            function getAlphaGrade(){
                return this.getEnteredAlphaGrade() || this.getCalculatedAlphaGrade()
            },

            [[Boolean, Boolean]],
            String, function displayAvgGradeValue(isAbleDisplayAlphaGrades_, original_){
                if(this.isExempt())
                    return Msg.Exempt;
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
