REQUIRE('chlk.models.common.ChlkDate');

NAMESPACE('chlk.models.attendance', function () {
    "use strict";

    var SJX = ria.serialize.SJX;
    /** @class chlk.models.attendance.AttendanceStatItem*/
    CLASS(
        UNSAFE, FINAL, 'AttendanceStatItem', IMPLEMENTS(ria.serialize.IDeserializable), [
            VOID, function deserialize(raw){
                this.studentCount = SJX.fromValue(raw.studentcount, Number);
                this.summary = SJX.fromValue(raw.summary, String);
                this.date = SJX.fromDeserializable(raw.date, chlk.models.common.ChlkDate);
            },

            Number, 'studentCount',
            String, 'summary',
            chlk.models.common.ChlkDate, 'date'
        ]);
});
