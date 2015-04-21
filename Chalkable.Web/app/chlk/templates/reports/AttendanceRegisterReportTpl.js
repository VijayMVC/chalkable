REQUIRE('chlk.templates.reports.BaseAttendanceReportTpl');
REQUIRE('chlk.models.reports.AttendanceRegisterReportViewData');

NAMESPACE('chlk.templates.reports', function () {

    /** @class chlk.templates.reports.AttendanceRegisterReportTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/reports/AttendanceRegisterReport.jade')],
        [ria.templates.ModelBind(chlk.models.reports.AttendanceRegisterReportViewData)],
        'AttendanceRegisterReportTpl', EXTENDS(chlk.templates.reports.BaseAttendanceReportTpl), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.attendance.AttendanceReason), 'attendanceReasons',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.common.NameId), 'attendanceMonths'
        ])
});