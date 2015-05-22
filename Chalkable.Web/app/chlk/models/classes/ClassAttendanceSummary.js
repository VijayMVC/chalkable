REQUIRE('chlk.models.classes.Class');

NAMESPACE('chlk.models.classes', function(){
    "use strict";

    var SJX = ria.serialize.SJX;

    /**@class chlk.models.classes.ClassAttendanceSummary*/
    CLASS(
        UNSAFE, 'ClassAttendanceSummary', EXTENDS(chlk.models.classes.Class), IMPLEMENTS(ria.serialize.IDeserializable),[
            chlk.models.common.HoverBox.OF(chlk.models.attendance.StudentAttendanceHoverBoxItem), 'absentSection',

            chlk.models.common.HoverBox.OF(chlk.models.attendance.StudentAttendanceHoverBoxItem), 'lateSection',

            chlk.models.common.HoverBox.OF(chlk.models.attendance.StudentAttendanceHoverBoxItem), 'presentSection',

            OVERRIDE, VOID, function deserialize(raw){
                BASE(raw);
                this.absentSection = SJX.fromDeserializable(raw.absences, chlk.models.common.HoverBox.OF(chlk.models.attendance.StudentAttendanceHoverBoxItem));
                this.lateSection = SJX.fromDeserializable(raw.lates, chlk.models.common.HoverBox.OF(chlk.models.attendance.StudentAttendanceHoverBoxItem));
                this.presentSection = SJX.fromDeserializable(raw.presents, chlk.models.common.HoverBox.OF(chlk.models.attendance.StudentAttendanceHoverBoxItem));
            }
    ]);
});