REQUIRE('chlk.models.apps.Application');


NAMESPACE('chlk.templates.apps', function () {

    /** @class chlk.templates.apps.AppMarketDetails*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/AppMarketDetails.jade')],
        [ria.templates.ModelBind(chlk.models.apps.Application)],
        'AppMarketDetails', EXTENDS(chlk.templates.JadeTemplate), [

        ])
});