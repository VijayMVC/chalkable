REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.reports.SubmitStudentReportViewData');

NAMESPACE('chlk.templates.reports', function () {

    /** @class chlk.templates.reports.StudentReportTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/reports/StudentReport.jade')],
        [ria.templates.ModelBind(chlk.models.reports.SubmitStudentReportViewData)],
        'StudentReportTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.schoolYear.GradingPeriod), 'gradingPeriods',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.SchoolPersonId, 'studentId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.GradingPeriodId, 'gradingPeriodId',

            [ria.templates.ModelPropertyBind],
            Boolean, 'ableDownload',

            ArrayOf(chlk.models.common.ActionLinkModel), function buildReportLinksData(pressedAction){
                var controllerName = 'reporting';
                var args = [];
                var classNames = ['report-button'];
                return [
                    new chlk.models.common.ActionLinkModel(controllerName, 'studentComprehensiveProgressReport',
                        'Comprehensive Progress', 'studentComprehensiveProgressReport' == pressedAction, args, classNames),
                ];
            }
        ])
});