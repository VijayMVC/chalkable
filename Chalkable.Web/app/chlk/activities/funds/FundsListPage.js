REQUIRE('chlk.activities.lib.TemplateActivity');
REQUIRE('chlk.templates.funds.Funds');

NAMESPACE('chlk.activities.funds', function () {

    /** @class chlk.activities.apps.FundsListPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.BindTemplate(chlk.templates.funds.Funds)],
        'FundsListPage', EXTENDS(chlk.activities.lib.TemplateActivity), [ ]);
});