REQUIRE('chlk.templates.JadeTemplate');
REQUIRE('chlk.models.common.PageWithGrades');

NAMESPACE('chlk.templates.common', function () {

    /** @class chlk.templates.common.PageWithGrades*/
    CLASS(
        [ria.templates.ModelBind(chlk.models.common.PageWithGrades)],
        'PageWithGrades', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.grading.GradeLevelsForTopBar, 'topData',

            [ria.templates.ModelPropertyBind],
            Number, 'selectedTypeId',

            [ria.templates.ModelPropertyBind],
            Array, 'selectedIds'
        ])
});