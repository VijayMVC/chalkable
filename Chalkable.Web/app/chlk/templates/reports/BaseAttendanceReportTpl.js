REQUIRE('chlk.templates.reports.BaseReportTpl');
REQUIRE('chlk.models.reports.BaseReportViewData');

NAMESPACE('chlk.templates.reports', function () {

    ASSET('~/assets/jade/activities/reports/ReportBase.jade')();

    /** @class chlk.templates.reports.BaseAttendanceReportTpl*/
    CLASS(
        [ria.templates.ModelBind(chlk.models.reports.BaseReportViewData)],
        'BaseAttendanceReportTpl', EXTENDS(chlk.templates.reports.BaseReportTpl), [
            [[String]],
            OVERRIDE, ArrayOf(chlk.models.common.ActionLinkModel), function buildReportLinksData(pressedAction){
                var controllerName = 'attendance';
                var args = [this.getGradingPeriodId(), this.getClassId(), this.getStartDate().toStandardFormat(), this.getEndDate().toStandardFormat()];
                var classNames = ['report-button'];
                return [
                    new chlk.models.common.ActionLinkModel(controllerName, 'attendanceProfile', 'Attendance Profile', 'attendanceProfile' == pressedAction, args, classNames),
                    new chlk.models.common.ActionLinkModel(controllerName, 'attendanceRegister', 'Attendance Register', 'attendanceRegister' == pressedAction, args, classNames),
                    new chlk.models.common.ActionLinkModel(controllerName, 'seatingChart', 'Seating Chart', 'seatingChart' == pressedAction, args, classNames)
                ];
            }
        ])
});