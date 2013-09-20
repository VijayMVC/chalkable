REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.apps.AppMarket');

NAMESPACE('chlk.activities.apps', function () {

    /** @class chlk.activities.apps.AppMarketPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.apps.AppMarket)],

        'AppMarketPage', EXTENDS(chlk.activities.lib.TemplatePage), [
        ]);
});