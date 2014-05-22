REQUIRE('chlk.templates.common.AttendanceDisciplinePopUpTpl');
REQUIRE('chlk.converters.attendance.AttendanceTypeToNameConverter');
REQUIRE('chlk.models.attendance.StudentDayAttendances');

NAMESPACE('chlk.templates.attendance', function () {

    /** @class chlk.templates.attendance.StudentDayAttendanceTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/attendance/StudentDayAttendance.jade')],
        [ria.templates.ModelBind(chlk.models.attendance.StudentDayAttendances)],
        'StudentDayAttendanceTpl', EXTENDS(chlk.templates.common.AttendanceDisciplinePopUpTpl), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.attendance.ClassAttendance), 'attendances',

            [ria.templates.ModelPropertyBind],
            chlk.models.attendance.ClassAttendance, 'allItem',

            [ria.templates.ModelPropertyBind],
            chlk.models.attendance.DailyAttendance, 'dailyAttendance',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.attendance.AttendanceReason), 'reasons',

            [[String]],
            ArrayOf(chlk.models.attendance.AttendanceReason), function getReasonsForType(level){
                return (this.getReasons() || []).filter(function(item){
                    return item.hasLevel(level);
                });
            },

            [[Number]],
            String, function getTypeName(id){
                return chlk.converters.attendance.AttendanceTypeToNameConverter.prototype.convert(id)
            }
        ])
});