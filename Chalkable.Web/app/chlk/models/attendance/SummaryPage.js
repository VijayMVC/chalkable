REQUIRE('chlk.models.attendance.AttendanceSummary');
REQUIRE('chlk.models.common.PageWithClasses');

NAMESPACE('chlk.models.attendance', function () {
    "use strict";

    /** @class chlk.models.attendance.SummaryPage*/
    CLASS(
        'SummaryPage', EXTENDS(chlk.models.common.PageWithClasses), [
            chlk.models.attendance.AttendanceSummary, 'summary',


            //todo: rename
            [[chlk.models.classes.ClassesForTopBar, chlk.models.attendance.AttendanceSummary]],
            function $(topData_, summary_){
                BASE(topData_);
                if(summary_)
                    this.setSummary(summary_);
            }
        ]);
});
