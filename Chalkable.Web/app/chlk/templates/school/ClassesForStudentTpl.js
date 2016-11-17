REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.school.ClassesForStudentViewData');

NAMESPACE('chlk.templates.school', function () {

    /** @class chlk.templates.school.ClassesForStudentTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/school/ClassesForSchool.jade')],
        [ria.templates.ModelBind(chlk.models.school.ClassesForStudentViewData)],
        'ClassesForStudentTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.admin.BaseStatisticGridViewData, 'itemsStatistic',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.GradingPeriodId, 'gradingPeriodId',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.schoolYear.GradingPeriod), 'gradingPeriods',
        ])
});