REQUIRE('chlk.models.id.AttendanceReasonId');

NAMESPACE('chlk.models.attendance', function () {
    "use strict";

    /** @class chlk.models.attendance.AttendanceReason*/
    CLASS(
        'AttendanceReason', [
            [ria.serialize.SerializeProperty('attendancetype')],
            Number, 'attendanceType',

            chlk.models.id.AttendanceReasonId, 'id',

            String, 'description'
        ]);
});
