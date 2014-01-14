REQUIRE('chlk.models.funds.SchoolPersonFundsViewData');
REQUIRE('chlk.templates.ChlkTemplate');

NAMESPACE('chlk.templates.funds', function () {
    /** @class chlk.templates.funds.SchoolPersonFundsTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/funds/SchoolPersonFundsView.jade')],
        [ria.templates.ModelBind(chlk.models.funds.SchoolPersonFundsViewData)],
        'SchoolPersonFundsTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.common.PaginatedList, 'fundsHistory',

            [ria.templates.ModelPropertyBind],
            chlk.models.funds.AddCreditCardModel, 'addCreditCardData',

            chlk.models.funds.AddCreditCardModel, function getCreditCardInfo(){
                return this.getAddCreditCardData();
            },

            [ria.templates.ModelPropertyBind],
            Number, 'currentBalance',

            [ria.templates.ModelPropertyBind],
            Boolean, 'transactionSuccess',

            Boolean, function showTransactionResultForm(){
                var res = this.isTransactionSuccess();
                return res === true || res === false;
            }
        ]);
});