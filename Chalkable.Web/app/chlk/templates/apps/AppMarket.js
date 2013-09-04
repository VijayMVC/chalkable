REQUIRE('chlk.models.common.PaginatedList');


NAMESPACE('chlk.templates.apps', function () {

    /** @class chlk.templates.apps.AppMarket*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/AppMarket.jade')],
        [ria.templates.ModelBind(chlk.models.common.PaginatedList)],
        'AppMarket', EXTENDS(chlk.templates.JadeTemplate), [

        ])
});