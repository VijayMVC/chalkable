REQUIRE('chlk.models.apps.AppMarketDetailsViewData');


NAMESPACE('chlk.templates.apps', function () {

    /** @class chlk.templates.apps.AppMarketDetails*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/app-market-details.jade')],
        [ria.templates.ModelBind(chlk.models.apps.AppMarketDetailsViewData)],
        'AppMarketDetails', EXTENDS(chlk.templates.JadeTemplate), [

            [ria.templates.ModelPropertyBind],
            chlk.models.apps.AppMarketApplication, 'app',

            [ria.templates.ModelPropertyBind],
            String, 'installBtnTitle'
        ])
});