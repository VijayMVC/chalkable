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

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.people.User), 'sendToPersons',

            Boolean, function showTransactionResultForm(){
                var res = this.isTransactionSuccess();
                return res === true || res === false;
            },

            ArrayOf(Object), function prepareMethodObjList(){
                var methods = chlk.activities.funds.BuyCreditMethods;
                var res = [
                    { method: methods.CREDIT_CARD, title: 'Credit Card', checked: true, readOnly: false},
                    { method: methods.PAY_PAL, title: 'PayPal', checked: false, readOnly: false}
                ];
                var role = this.getUserRole();
                if(role.isTeacher())
                    res.push({method: methods.SEND_MESSAGE, title: 'Ask Admin', checked: false, readOnly: false});
                if(role.isStudent())
                    res.push({method: methods.SEND_MESSAGE, title: 'Ask Parent', checked: false, readOnly: false});
                return res;
            }
        ]);
});