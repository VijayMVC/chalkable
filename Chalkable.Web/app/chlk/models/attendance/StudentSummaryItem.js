REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.attendance.StudentAttendanceHoverBoxItem');

NAMESPACE('chlk.models.attendance', function () {
    "use strict";

    /** @class chlk.models.attendance.StudentSummaryItem*/
    CLASS(
        'StudentSummaryItem', [
            [ria.serialize.SerializeProperty('studentinfo')],
            chlk.models.people.User, 'studentInfo',

            [ria.serialize.SerializeProperty('statbyclass')],
            ArrayOf(chlk.models.attendance.StudentAttendanceHoverBoxItem), 'statByClass',

            ArrayOf(String), 'alerts'
        ]);
});
