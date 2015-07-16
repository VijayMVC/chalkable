REQUIRE('chlk.models.attendance.AttendanceSummary');
REQUIRE('chlk.models.common.PageWithClasses');
REQUIRE('chlk.models.schoolYear.GradingPeriod');

NAMESPACE('chlk.models.attendance', function () {
    "use strict";

    /** @class chlk.models.attendance.SummaryPage*/
    CLASS(
        'SummaryPage', EXTENDS(chlk.models.common.PageWithClasses), [
            chlk.models.attendance.AttendanceSummary, 'summary',

            chlk.models.schoolYear.GradingPeriod, 'gradingPeriod',

            //todo: rename
            [[chlk.models.classes.ClassesForTopBar, chlk.models.attendance.AttendanceSummary, chlk.models.schoolYear.GradingPeriod]],
            function $(topData_, summary_, gradingPeriod_){
                BASE(topData_);
                if(summary_)
                    this.setSummary(summary_);
                if(gradingPeriod_)
                    this.setGradingPeriod(gradingPeriod_);
            }
        ]);
});
