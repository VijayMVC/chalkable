REQUIRE('chlk.models.people.ShortUserInfo');
REQUIRE('chlk.models.schoolYear.MarkingPeriod');
REQUIRE('chlk.models.attendance.StudentAttendanceHoverBoxItem');

NAMESPACE('chlk.models.attendance', function(){
   "use strict";

    var SJX = ria.serialize.SJX;

    /**@class chlk.models.attendance.StudentAttendanceSummary*/
    CLASS(
        UNSAFE, 'StudentAttendanceSummary', EXTENDS(chlk.models.people.ShortUserInfo), IMPLEMENTS(ria.serialize.IDeserializable),[

            [ria.serialize.SerializeProperty('markingperiods')],
            ArrayOf(chlk.models.schoolYear.MarkingPeriod), 'markingPeriods',

            [ria.serialize.SerializeProperty('currentmarkingperiod')],
            chlk.models.schoolYear.MarkingPeriod, 'currentMarkingPeriod',

            [ria.serialize.SerializeProperty('absences')],
            chlk.models.common.HoverBox.OF(chlk.models.attendance.StudentAttendanceHoverBoxItem), 'absentSection',

            [ria.serialize.SerializeProperty('lates')],
            chlk.models.common.HoverBox.OF(chlk.models.attendance.StudentAttendanceHoverBoxItem), 'lateSection',

            [ria.serialize.SerializeProperty('presents')],
            chlk.models.common.HoverBox.OF(chlk.models.attendance.StudentAttendanceHoverBoxItem), 'presentSection',

            OVERRIDE, VOID, function deserialize(raw){
                BASE(raw);
                this.markingPeriods = SJX.fromArrayOfDeserializables(raw.markingperiods, chlk.models.schoolYear.MarkingPeriod);
                this.currentMarkingPeriod = SJX.fromDeserializable(raw.currentmarkingperiod, chlk.models.schoolYear.MarkingPeriod);
                this.absentSection = SJX.fromDeserializable(raw.absences, chlk.models.common.HoverBox.OF(chlk.models.attendance.StudentAttendanceHoverBoxItem));
                this.lateSection = SJX.fromDeserializable(raw.lates, chlk.models.common.HoverBox.OF(chlk.models.attendance.StudentAttendanceHoverBoxItem));
                this.presentSection = SJX.fromDeserializable(raw.presents, chlk.models.common.HoverBox.OF(chlk.models.attendance.StudentAttendanceHoverBoxItem));
            }
    ]);
});