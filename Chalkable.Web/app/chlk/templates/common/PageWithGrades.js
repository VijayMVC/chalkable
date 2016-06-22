REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.common.PageWithGrades');

NAMESPACE('chlk.templates.common', function () {

    /** @class chlk.templates.common.PageWithGrades*/
    CLASS(
        [ria.templates.ModelBind(chlk.models.common.PageWithGrades)],
        'PageWithGrades', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.grading.GradeLevelsForTopBar, 'topData',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.GradeLevelId, 'selectedTypeId',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.id.GradeLevelId), 'selectedIds'
        ])
});