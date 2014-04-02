REQUIRE('chlk.templates.common.PageWithClasses');
REQUIRE('chlk.models.attendance.SeatingChart');

NAMESPACE('chlk.templates.attendance', function () {

    /** @class chlk.templates.attendance.SeatingChartTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/attendance/SeatingChartPage.jade')],
        [ria.templates.ModelBind(chlk.models.attendance.SeatingChart)],
        'SeatingChartTpl', EXTENDS(chlk.templates.common.PageWithClasses), [
            [ria.templates.ModelPropertyBind],
            Number, 'columns',

            [ria.templates.ModelPropertyBind],
            Number, 'rows',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'date',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.attendance.ClassAttendance), 'notSeatingStudents',

            [ria.templates.ModelPropertyBind],
            ArrayOf(ArrayOf(chlk.models.attendance.ClassAttendanceWithSeatPlace)), 'seatingList',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.attendance.AttendanceReason), 'reasons'
        ]);
});