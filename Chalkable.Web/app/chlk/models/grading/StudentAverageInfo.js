REQUIRE('ria.serialize.SJX');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.GradingPeriodId');
REQUIRE('chlk.models.grading.AvgCodeHeaderViewData');

NAMESPACE('chlk.models.grading', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.grading.ShortStudentAverageInfo*/
    CLASS(
        UNSAFE, 'ShortStudentAverageInfo', IMPLEMENTS(ria.serialize.IDeserializable), [

            VOID, function deserialize(raw) {
                this.averageId = SJX.fromValue(raw.averageid, Number);
                this.averageName = SJX.fromValue(raw.averagename, String);
                this.calculatedAvg = SJX.fromValue(raw.calculatedavg, Number);
                this.enteredAvg = SJX.fromValue(raw.enteredavg, Number);
                this.calculatedAlphaGrade = SJX.fromValue(raw.calculatedalphagrade, String);
                this.enteredAlphaGrade = SJX.fromValue(raw.enteredalphagrade, String);
                this.mayBeExempt = SJX.fromValue(raw.maybeexempt, Boolean);
                this.exempt = SJX.fromValue(raw.isexempt, Boolean);
                this.studentId = SJX.fromValue(raw.studentid, chlk.models.id.SchoolPersonId);
                this.classId = SJX.fromValue(raw.classid, chlk.models.id.ClassId);
                this.gradingPeriodId = SJX.fromValue(raw.gradingPeriodId, chlk.models.id.GradingPeriodId);
                this.averageValue = SJX.fromValue(raw.averageValue, String);
                this.note = SJX.fromValue(raw.note, String);
                this.oldValue = SJX.fromValue(raw.oldValue, String);
                this.oldExempt = SJX.fromValue(raw.oldExempt, String);
                this.codes = SJX.fromArrayOfValues(raw.codes, null);
                this.codesString = SJX.fromValue(raw.codesString, String);
            },

            Number, 'averageId',
            String, 'averageName',
            Number, 'calculatedAvg',
            Number, 'enteredAvg',
            String, 'calculatedAlphaGrade',
            String, 'enteredAlphaGrade',
            Boolean, 'mayBeExempt',
            Boolean, 'exempt',
            chlk.models.id.SchoolPersonId, 'studentId',
            chlk.models.id.ClassId, 'classId',
            chlk.models.id.GradingPeriodId, 'gradingPeriodId',
            String, 'averageValue',
            String, 'note',
            String, 'oldValue',
            Boolean, 'oldExempt',
            Array, 'codes',
            String, 'codesString',

            [[Number, chlk.models.id.GradingPeriodId, Number, Number, String, String, Boolean, Boolean, chlk.models.id.SchoolPersonId, String]],
            function $(averageId_, gradingPeriodId_, calculatedAvg_, enteredAvg_, calculatedAlphaGrade_, enteredAlphaGrade_, mayBeExempt_, exempt_, studentId_, codesString_){
                BASE();
                if(averageId_)
                    this.setAverageId(averageId_);
                if(gradingPeriodId_)
                    this.setGradingPeriodId(gradingPeriodId_);
                if(calculatedAvg_ || calculatedAvg_ == 0)
                    this.setCalculatedAvg(calculatedAvg_);
                if(enteredAvg_ || enteredAvg_ == 0)
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

            [[Boolean, Boolean, Boolean]],
            String, function displayAvgGradeValue(isAbleDisplayAlphaGrades_, original_, noText_, isRounded_){
                if(this.isExempt() && !original_ && !noText_)
                    return Msg.Exempt;
                var alphaGrade = original_ ? this.getCalculatedAlphaGrade() : this.getAlphaGrade();
                var res = this.displayGrade(original_ ? this.getCalculatedAvg() : this.getNumericAvg(), isRounded_);
                if((res || res == 0) && isAbleDisplayAlphaGrades_ && alphaGrade && alphaGrade.trim() != ''){
                    res += '(' + alphaGrade + ')';
                }
                return res;
            },

            String, function displayGrade(grade, idRounded_){
                return grade || grade == 0 ? grade.toFixed(idRounded_ ? 0 : 2) : '';
            }
        ]);

    /** @class chlk.models.grading.StudentAverageInfo*/
    CLASS(
        UNSAFE, 'StudentAverageInfo', IMPLEMENTS(ria.serialize.IDeserializable), [

            VOID, function deserialize(raw) {
                this.averageId = SJX.fromValue(raw.averageid, Number);
                this.averageName = SJX.fromValue(raw.averagename, String);
                this.gradingPeriodAverage = SJX.fromValue(raw.isgradingperiodaverage, Boolean);
                this.totalAverage = SJX.fromValue(raw.totalaverage, Number);
                this.averages = SJX.fromArrayOfDeserializables(raw.averages, chlk.models.grading.ShortStudentAverageInfo);
            },

            Number, 'averageId',
            String, 'averageName',
            ArrayOf(chlk.models.grading.ShortStudentAverageInfo), 'averages',
            Boolean, 'gradingPeriodAverage',
            Number, 'totalAverage'
        ]);
});
