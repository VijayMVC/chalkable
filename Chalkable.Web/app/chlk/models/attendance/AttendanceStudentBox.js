REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.common.ChlkDate');

NAMESPACE('chlk.models.attendance', function () {
    "use strict";

    /** @class chlk.models.attendance.AttendanceStudentBox*/
    CLASS(
        'AttendanceStudentBox', [
            chlk.models.id.SchoolPersonId, 'id',

            String, 'name',

            chlk.models.common.ChlkDate, 'date'
        ]);
});
