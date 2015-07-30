REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.classes.LECreditsDialogViewData');

NAMESPACE('chlk.templates.classes', function () {

    /** @class chlk.templates.classes.LECreditsDialogTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/class/le-credits-dialog.jade')],
        [ria.templates.ModelBind(chlk.models.classes.LECreditsDialogViewData)],
        'LECreditsDialogTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            String, 'url'

        ])
});