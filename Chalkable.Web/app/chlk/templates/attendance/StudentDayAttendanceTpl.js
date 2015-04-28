REQUIRE('chlk.templates.Popup');
REQUIRE('chlk.converters.attendance.AttendanceTypeToNameConverter');
REQUIRE('chlk.models.attendance.StudentDayAttendances');

NAMESPACE('chlk.templates.attendance', function () {

    /** @class chlk.templates.attendance.StudentDayAttendanceTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/attendance/StudentDayAttendance.jade')],
        [ria.templates.ModelBind(chlk.models.attendance.StudentDayAttendances)],
        'StudentDayAttendanceTpl', EXTENDS(chlk.templates.Popup), [
            [ria.templates.ModelPropertyBind],
            chlk.models.calendar.attendance.StudentAttendanceCalendarMonthItem, 'item',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.attendance.AttendanceReason), 'reasons',

            [ria.templates.ModelPropertyBind],
            Boolean, 'canRePost',

            [ria.templates.ModelPropertyBind],
            Boolean, 'canSetAttendance',

            [ria.templates.ModelPropertyBind],
            Boolean, 'canChangeReasons',

            [ria.templates.ModelPropertyBind],
            String, 'controller',

            [ria.templates.ModelPropertyBind],
            String, 'action',

            [ria.templates.ModelPropertyBind],
            String, 'params',

            [[String]],
            ArrayOf(chlk.models.attendance.AttendanceReason), function getReasonsForType(level){
                return (this.getReasons() || []).filter(function(item){
                    return item.hasLevel(level);
                });
            }/*,

            [[Number]],
            String, function getTypeName(id){
                return chlk.converters.attendance.AttendanceTypeToNameConverter.prototype.convert(id)
            }*/
        ])
});