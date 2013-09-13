/**
 * Created with JetBrains WebStorm.
 * User: C01t
 * Date: 8/30/13
 * Time: 5:25 PM
 * To change this template use File | Settings | File Templates.
 */

REQUIRE('ria.templates.IConverter');
REQUIRE('chlk.models.attendance.ClassAttendance');

NAMESPACE('chlk.converters.attendance', function () {

    /** @class chlk.converters.attendance.AttendanceTypeToNameConverter */
    CLASS(
        'AttendanceTypeToNameConverter', IMPLEMENTS(ria.templates.IConverter), [
            [[Number]],
            String, function convert(id) {
                var enums = chlk.models.attendance.AttendanceTypeEnum;
                switch (id) {
                    case enums.PRESENT: return Msg.Present;
                    case enums.EXCUSED: return Msg.Excused;
                    case enums.ABSENT: return Msg.Absent;
                    case enums.LATE: return Msg.Late;
                    case enums.NA: return Msg.NA;
                    default: return 'Unknown value ' + id;
                }
            }
        ])
});