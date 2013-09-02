/**
 * Created with JetBrains WebStorm.
 * User: C01t
 * Date: 8/30/13
 * Time: 5:25 PM
 * To change this template use File | Settings | File Templates.
 */

REQUIRE('ria.templates.IConverter');

NAMESPACE('chlk.converters.attendance', function () {

    /** @class chlk.converters.attendance.ClassAttendanceIdToNameConverter */
    CLASS(
        'ClassAttendanceIdToNameConverter', IMPLEMENTS(ria.templates.IConverter), [
            [[Number]],
            String, function convert(id) {
                switch (id) {
                    case 2: return 'Present';
                    case 4: return 'Excused';
                    case 8: return 'Absent';
                    case 16: return 'Late';
                    case 1: return 'N/A';
                    default: return 'Unknown value ' + id;
                }
            }
        ])
});