REQUIRE('chlk.models.apps.AppMarketApplication');
REQUIRE('chlk.models.common.PaginatedList');

NAMESPACE('chlk.templates.apps', function () {

    /** @class chlk.templates.apps.AppMarketBanTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/banned-app.jade')],
        [ria.templates.ModelBind(chlk.models.apps.AppMarketApplication)],
        'AppMarketBanTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            Boolean, 'banned',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AppId, 'id'

        ])
});