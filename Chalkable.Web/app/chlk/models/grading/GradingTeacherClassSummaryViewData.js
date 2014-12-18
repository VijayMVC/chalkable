REQUIRE('chlk.models.grading.StudentGradingViewData');

NAMESPACE('chlk.models.grading', function () {
    "use strict";
    /** @class chlk.models.grading.GradingTeacherClassSummaryViewData*/
    CLASS(
        'GradingTeacherClassSummaryViewData', [
            [ria.serialize.SerializeProperty('class')],
            chlk.models.classes.Class, 'clazz',

            Number, 'maxScore',

            function $(){
                BASE();
                this.maxScore = 100;
            },

            ArrayOf(chlk.models.grading.StudentGradingViewData), 'well',

            ArrayOf(chlk.models.grading.StudentGradingViewData), 'trouble',

            [ria.serialize.SerializeProperty('allstudents')],
            ArrayOf(chlk.models.grading.StudentGradingViewData), 'allStudents',

            function getUpdatedTrouble(){
                var res = [], i, len = this.getTrouble().length;
                for(i = 0; i < 5 - len; i++)
                    res.push(new chlk.models.grading.StudentGradingViewData());

                for(i = 0; i < len; i++)
                    res.push(this.getTrouble()[i])
                return res;
            }
        ]);
});
