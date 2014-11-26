REQUIRE('chlk.models.attendance.StudentSummaryItem');
REQUIRE('chlk.models.attendance.AttendanceSummaryStatItem');

NAMESPACE('chlk.models.attendance', function () {
    "use strict";

    var SJX = ria.serialize.SJX;
    /** @class chlk.models.attendance.AbsentLateSummaryItem*/
    CLASS(
        UNSAFE, FINAL, 'AbsentLateSummaryItem', IMPLEMENTS(ria.serialize.IDeserializable), [

            VOID, function deserialize(raw){
                this.classesStats = SJX.fromArrayOfDeserializables(raw.classesstats, chlk.models.attendance.AttendanceSummaryStatItem);
                this.students = SJX.fromArrayOfDeserializables(raw.students, chlk.models.attendance.StudentSummaryItem);
            },

            ArrayOf(chlk.models.attendance.AttendanceSummaryStatItem), 'classesStats',
            ArrayOf(chlk.models.attendance.StudentSummaryItem), 'students'
        ]);
});
