REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.grading.GradeBookReportViewData');

NAMESPACE('chlk.templates.grading', function () {

    /** @class chlk.templates.grading.WorksheetReportTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/grading/WorksheetReport.jade')],
        [ria.templates.ModelBind(chlk.models.grading.GradeBookReportViewData)],
        'WorksheetReportTpl', EXTENDS(chlk.templates.ChlkTemplate), [])
});