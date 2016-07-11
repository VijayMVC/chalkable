REQUIRE('chlk.models.people.ShortUserInfo');
REQUIRE('chlk.models.schoolYear.GradingPeriod');
REQUIRE('chlk.models.attendance.StudentAttendanceHoverBoxItem');

NAMESPACE('chlk.models.attendance', function(){
   "use strict";

    var SJX = ria.serialize.SJX;

    /**@class chlk.models.attendance.StudentAttendanceSummary*/
    CLASS(
        UNSAFE, 'StudentAttendanceSummary', EXTENDS(chlk.models.people.ShortUserInfo), IMPLEMENTS(ria.serialize.IDeserializable),[

            ArrayOf(chlk.models.schoolYear.GradingPeriod), 'gradingPeriods',

            chlk.models.schoolYear.GradingPeriod, 'currentGradingPeriod',

            chlk.models.common.HoverBox.OF(chlk.models.attendance.StudentAttendanceHoverBoxItem), 'absentSection',

            chlk.models.common.HoverBox.OF(chlk.models.attendance.StudentAttendanceHoverBoxItem), 'lateSection',

            chlk.models.common.HoverBox.OF(chlk.models.attendance.StudentAttendanceHoverBoxItem), 'presentSection',

            OVERRIDE, VOID, function deserialize(raw){
                BASE(raw);
                this.gradingPeriods = SJX.fromArrayOfDeserializables(raw.gradingperiods, chlk.models.schoolYear.GradingPeriod);
                this.currentGradingPeriod = SJX.fromDeserializable(raw.currentgradingperiod, chlk.models.schoolYear.GradingPeriod);
                this.absentSection = SJX.fromDeserializable(raw.absences, chlk.models.common.HoverBox.OF(chlk.models.attendance.StudentAttendanceHoverBoxItem));
                this.lateSection = SJX.fromDeserializable(raw.lates, chlk.models.common.HoverBox.OF(chlk.models.attendance.StudentAttendanceHoverBoxItem));
                this.presentSection = SJX.fromDeserializable(raw.presents, chlk.models.common.HoverBox.OF(chlk.models.attendance.StudentAttendanceHoverBoxItem));
            }
    ]);
});