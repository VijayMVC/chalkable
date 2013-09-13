/**
 * Created with JetBrains WebStorm.
 * User: C01t
 * Date: 8/30/13
 * Time: 5:25 PM
 * To change this template use File | Settings | File Templates.
 */

REQUIRE('ria.templates.IConverter');

NAMESPACE('chlk.converters.attendance', function () {

    /** @class chlk.converters.attendance.AttendanceTypeToNameConverter */
    CLASS(
        'AttendanceTypeToNameConverter', IMPLEMENTS(ria.templates.IConverter), [
            [[Number]],
            String, function convert(id) {
                switch (id) {
                    case 2: return Msg.Present;
                    case 4: return Msg.Excused;
                    case 8: return Msg.Absent;
                    case 16: return Msg.Late;
                    case 1: return Msg.NA;
                    default: return 'Unknown value ' + id;
                }
            }
        ])
});