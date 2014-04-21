REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.grading.GradeBookReportViewData');

NAMESPACE('chlk.templates.grading', function () {

    /** @class chlk.templates.grading.GradeBookReportTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/grading/GradeBookReport.jade')],
        [ria.templates.ModelBind(chlk.models.grading.GradeBookReportViewData)],
        'GradeBookReportTpl', EXTENDS(chlk.templates.ChlkTemplate), [])
});