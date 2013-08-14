REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.funds.Funds');

NAMESPACE('chlk.activities.funds', function () {

    /** @class chlk.activities.apps.FundsListPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.funds.Funds)],
        'FundsListPage', EXTENDS(chlk.activities.lib.TemplatePage), [ ]);
});