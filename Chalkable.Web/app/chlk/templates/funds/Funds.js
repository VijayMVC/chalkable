REQUIRE('chlk.templates.PaginatedList');
REQUIRE('chlk.models.funds.Fund');

NAMESPACE('chlk.templates.funds', function () {
    /** @class chlk.templates.funds.Funds*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/funds/funds.jade')],
        [ria.templates.ModelBind(chlk.models.common.PaginatedList)],
        'Funds', EXTENDS(chlk.templates.PaginatedList), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.funds.Fund), 'items'
        ])
});