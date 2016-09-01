REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.attendance.StudentAttendanceHoverBoxItem');

NAMESPACE('chlk.models.attendance', function () {
    "use strict";

    var SJX = ria.serialize.SJX;
    /** @class chlk.models.attendance.StudentSummaryItem*/
    CLASS(
        UNSAFE, FINAL, 'StudentSummaryItem', IMPLEMENTS(ria.serialize.IDeserializable), [
            VOID, function deserialize(raw){
                this.studentInfo = SJX.fromDeserializable(raw.studentinfo, chlk.models.people.User);
                this.statByClass = SJX.fromArrayOfDeserializables(raw.statbyclass, chlk.models.attendance.StudentAttendanceHoverBoxItem);
                this.alerts = SJX.fromArrayOfValues(raw.alerts, String);
            },

            chlk.models.people.User, 'studentInfo',
            ArrayOf(chlk.models.attendance.StudentAttendanceHoverBoxItem), 'statByClass',
            ArrayOf(String), 'alerts'
        ]);
});
