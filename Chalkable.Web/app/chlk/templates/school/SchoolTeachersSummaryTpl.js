REQUIRE('chlk.templates.school.SchoolClassesSummaryTpl');

NAMESPACE('chlk.templates.school', function () {

    /** @class chlk.templates.school.SchoolTeachersSummaryTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/school/SchoolTeachersSummary.jade')],
        [ria.templates.ModelBind(chlk.models.school.SchoolSummaryViewData)],
        'SchoolTeachersSummaryTpl', EXTENDS(chlk.templates.school.SchoolClassesSummaryTpl), [])
});