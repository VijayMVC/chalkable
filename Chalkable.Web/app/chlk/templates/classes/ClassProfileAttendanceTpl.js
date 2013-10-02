REQUIRE('chlk.templates.profile.ClassProfileTpl');
REQUIRE('chlk.templates.calendar.attendance.ClassAttendanceMonthCalendarTpl');
REQUIRE('chlk.models.classes.ClassProfileAttendanceViewData');

NAMESPACE('chlk.templates.classes', function () {
    "use strict";
    /** @class chlk.templates.classes.ClassProfileAttendanceTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/class/ClassProfileAttendance.jade')],
        [ria.templates.ModelBind(chlk.models.classes.ClassProfileAttendanceViewData)],
        'ClassProfileAttendanceTpl', EXTENDS(chlk.templates.profile.ClassProfileTpl), [

            [ria.templates.ModelPropertyBind],
            chlk.models.classes.ClassAttendanceSummary, 'classAttendanceSummary',

            [ria.templates.ModelPropertyBind],
            chlk.models.calendar.attendance.ClassAttendanceMonthCalendar, 'monthCalendar'
        ]);
});