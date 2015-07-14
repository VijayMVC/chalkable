REQUIRE('chlk.templates.reports.BaseReportTpl');
REQUIRE('chlk.models.reports.SubmitMissingAssignmentsReportViewData');

NAMESPACE('chlk.templates.reports', function () {

    /** @class chlk.templates.reports.MissingAssignmentsReportTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/reports/MissingAssignmentsReport.jade')],
        [ria.templates.ModelBind(chlk.models.reports.SubmitMissingAssignmentsReportViewData)],
        'MissingAssignmentsReportTpl', EXTENDS(chlk.templates.reports.BaseReportTpl), [

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.grading.AlternateScore), 'alternateScores'
        ]);
});