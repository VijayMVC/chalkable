REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.id.SchoolPersonId');

NAMESPACE('chlk.models.attendance', function () {
    "use strict";

    /** @class chlk.models.attendance.AttendanceList*/
    CLASS(
        'AttendanceList', [
            chlk.models.id.SchoolPersonId, 'personId',

            String, 'classIds',

            String, 'attendanceTypes',

            String, 'attReasons',

            String, 'controller',

            String, 'action',

            String, 'params',

            Boolean, 'newStudent',

            chlk.models.common.ChlkDate, 'date'
        ]);
});
