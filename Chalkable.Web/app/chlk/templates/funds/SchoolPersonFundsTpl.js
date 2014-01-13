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
            chlk.models.funds.AddCreditCardModel, 'creditCardInfo',

            chlk.models.funds.AddCreditCardModel, function getCreditCardInfo(){
                var res = this.creditCardInfo;
                if(!res){
                    res = chlk.models.funds.AddCreditCardModel();
                    res.setAmount(10);
                }
                return res
            },

            [ria.templates.ModelPropertyBind],
            Number, 'currentBalance',

            [ria.templates.ModelPropertyBind],
            Boolean, 'transactionSuccess',

            Boolean, function showTransactionResultForm(){
                var res = this.isTransactionSuccess();
                return res === true || res === false;
            }



//            String, function getHiddenCreditNumber(){
//                var card = this.getCreditCardInfo();
//                if(card){
//                    var number = card.getCardNumber().toString();
//                    var arr = [];
//                    for(var i = 0; i < number.length - 4; i++)
//                        arr[i] = '*';
//                    return number.replace(number.substr(0, number.length - 4), arr.join(''));
//                }
//                return "";
//            }
        ]);
});