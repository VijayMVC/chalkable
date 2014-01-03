REQUIRE('chlk.models.funds.SchoolPersonFundsViewData');
REQUIRE('chlk.templates.ChlkTemplate');

NAMESPACE('chlk.templates.funds', function () {
    /** @class chlk.templates.funds.SchoolPersonFundsTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/funds/SchoolPersonFundsView.jade')],
        [ria.templates.ModelBind(chlk.models.funds.SchoolPersonFundsViewData)],
        'SchoolPersonFundsTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.funds.FundsHistory), 'fundsHistory',

            [ria.templates.ModelPropertyBind],
            chlk.models.funds.CreditCardInfo, 'creditCardInfo',

            [ria.templates.ModelPropertyBind],
            Number, 'currentBalance'
        ]);
});