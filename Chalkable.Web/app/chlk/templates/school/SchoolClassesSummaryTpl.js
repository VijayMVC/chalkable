REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.school.SchoolClassesSummaryViewData');

NAMESPACE('chlk.templates.school', function () {

    /** @class chlk.templates.school.SchoolClassesSummaryTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/school/SchoolClassesSummary.jade')],
        [ria.templates.ModelBind(chlk.models.school.SchoolClassesSummaryViewData)],
        'SchoolClassesSummaryTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.common.PaginatedList, 'classesStatistic',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.SchoolId, 'schoolId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.SchoolYearId, 'schoolYearId',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.schoolYear.Year), 'schoolYears',

            [ria.templates.ModelPropertyBind],
            String, 'schoolName'
        ])
});