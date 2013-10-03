REQUIRE('chlk.templates.JadeTemplate');
REQUIRE('chlk.models.common.PageWithClasses');

NAMESPACE('chlk.templates.common', function () {

    /** @class chlk.templates.common.PageWithClasses*/
    CLASS(
        [ria.templates.ModelBind(chlk.models.common.PageWithClasses)],
        'PageWithClasses', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.classes.ClassesForTopBar, 'topData',

            [ria.templates.ModelPropertyBind],
            Number, 'selectedTypeId'
        ])
});