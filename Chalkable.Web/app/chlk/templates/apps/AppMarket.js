REQUIRE('chlk.models.apps.Application');


NAMESPACE('chlk.templates.apps', function () {

    /** @class chlk.templates.apps.AppMarket*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/AppMarket.jade')],
        [ria.templates.ModelBind(chlk.models.apps.Application)],
        'AppMarket', EXTENDS(chlk.templates.JadeTemplate), [

        ])
});