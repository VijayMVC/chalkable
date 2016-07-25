REQUIRE('chlk.templates.reports.BaseReportTpl');
REQUIRE('chlk.models.reports.GradeVerificationReportViewData');

NAMESPACE('chlk.templates.reports', function () {

    /** @class chlk.templates.reports.GradeVerificationReportTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/reports/GradeVerificationReport.jade')],
        [ria.templates.ModelBind(chlk.models.reports.GradeVerificationReportViewData)],
        'GradeVerificationReportTpl', EXTENDS(chlk.templates.reports.BaseReportTpl), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.schoolYear.GradingPeriod), 'gradingPeriods',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.grading.GradedItemViewData), 'studentAverages',

            [ria.templates.ModelPropertyBind],
            Boolean, 'includeWithdrawnStudents',
        ])
});