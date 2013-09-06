REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.apps.AppMarketDetails');

NAMESPACE('chlk.activities.apps', function () {

    /** @class chlk.activities.apps.AppMarketDetailsPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.apps.AppMarketDetails)],
        'AppMarketDetailsPage', EXTENDS(chlk.activities.lib.TemplatePage), [
        ]);
});