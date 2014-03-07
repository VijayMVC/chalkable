REQUIRE('chlk.templates.common.PageWithClasses');
REQUIRE('chlk.models.attendance.ClassList');

NAMESPACE('chlk.templates.attendance', function () {

    /** @class chlk.templates.attendance.SeatingChartTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/attendance/SeatingChartPage.jade')],
        [ria.templates.ModelBind(chlk.models.attendance.ClassList)],
        'SeatingChartTpl', EXTENDS(chlk.templates.common.PageWithClasses), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.attendance.ClassAttendance), 'items'
        ]);
});