REQUIRE('chlk.templates.reports.BaseAttendanceReportTpl');
REQUIRE('chlk.models.reports.AttendanceProfileReportViewData');

NAMESPACE('chlk.templates.reports', function () {

    /** @class chlk.templates.reports.AttendanceProfileReportTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/reports/AttendanceProfileReport.jade')],
        [ria.templates.ModelBind(chlk.models.reports.AttendanceProfileReportViewData)],
        'AttendanceProfileReportTpl', EXTENDS(chlk.templates.reports.BaseAttendanceReportTpl), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.schoolYear.MarkingPeriod), 'markingPeriods',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.attendance.AttendanceReason), 'attendanceReasons'
        ])
});