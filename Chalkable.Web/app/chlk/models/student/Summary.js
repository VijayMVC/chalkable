REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.class.Class');
REQUIRE('chlk.models.common.AttendanceHoverBox');
REQUIRE('chlk.models.common.DisciplineHoverBox');
REQUIRE('chlk.models.announcement.AnnouncementClassPeriod');

NAMESPACE('chlk.models.student', function () {
    "use strict";

    /** @class chlk.models.student.Summary*/
    CLASS(
        'Summary', EXTENDS(chlk.models.people.User), [
            [ria.serialize.SerializeProperty('gradelevelnumber')],
            Number, 'gradeLevelNumber',

            [ria.serialize.SerializeProperty('currentclassname')],
            String, 'currentClassName',

            [ria.serialize.SerializeProperty('currentattendancetype')],
            Number, 'currentAttendanceType',

            [ria.serialize.SerializeProperty('maxPeriodNumber')],
            Number, 'maxPeriodNumber',

            [ria.serialize.SerializeProperty('roomid')],
            chlk.models.id.RoomId, 'roomId',

            [ria.serialize.SerializeProperty('roomname')],
            String, 'roomName',

            [ria.serialize.SerializeProperty('roomnumber')],
            Number, 'roomNumber',

            [ria.serialize.SerializeProperty('attendancebox')],
            chlk.models.common.AttendanceHoverBox, 'attendanceBox',

            [ria.serialize.SerializeProperty('disciplinebox')],
            chlk.models.common.DisciplineHoverBox, 'disciplineBox',

            [ria.serialize.SerializeProperty('classessection')],
            ArrayOf(chlk.models.class.Class), 'classesSection',

            [ria.serialize.SerializeProperty('periodsection')],
            ArrayOf(chlk.models.announcement.AnnouncementClassPeriod), 'periodSection'
        ]);
});
