REQUIRE('chlk.models.funds.AddCreditCardModel');
REQUIRE('chlk.templates.ChlkTemplate');

NAMESPACE('chlk.templates.funds', function () {

    "use strict";

    /** @class chlk.templates.funds.CreditCardTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/funds/CreditCardInfoView.jade')],
        [ria.templates.ModelBind(chlk.models.funds.AddCreditCardModel)],
        'CreditCardTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            String, 'cardNumber',

            [ria.templates.ModelPropertyBind],
            Number, 'month',

            [ria.templates.ModelPropertyBind],
            Number, 'year',

            [ria.templates.ModelPropertyBind],
            Number, 'cvcNumber',

            [ria.templates.ModelPropertyBind],
            String, 'cardType',

            [ria.templates.ModelPropertyBind],
            Object, 'mappedCardType',

//            [ria.templates.ModelPropertyBind],
//            chlk.models.funds.CreditCardInfo, 'creditCardInfo',
//
//            String, function getCardNumber(){
//                return this.getCreditCardInfo() ? this.getCreditCardInfo().getCardNumber() : null;
//            },
//            Number, function getMonth(){
//                return this.getCreditCardInfo() ? this.getCreditCardInfo().getMonth() : null;
//            },
//            Number, function getYear(){
//                return this.getCreditCardInfo() ? this.getCreditCardInfo().getYear() : null;
//            },
//            Number, function getCvcNumber(){
//                return this.getCreditCardInfo() ? this.getCreditCardInfo().getCvcNumber() : null;
//            },
//            String, function getCardType(){
//                return this.getCreditCardInfo() ? this.getCreditCardInfo().getCardType() : null;
//            },

            [ria.templates.ModelPropertyBind],
            Number, 'amount',

            Boolean, function hasCardData(){
                return !!(this.getYear() && this.getMonth()
                    && this.getCardNumber() && this.getCvcNumber());
            },

            String, function getCardCssClass(){
                //var card = this.getCreditCardInfo();
                if(this.getMappedCardType() && this.getMappedCardType().cssClass)
                    return this.getMappedCardType().cssClass;
                return 'simple-card-icon';
            },

            String, function getHiddenCreditNumber(){
                var number = this.getCardNumber();
                var arr = [];
                if(!number) return null;
                number = number.toString();
                for(var i = 0; i < number.length - 4; i++)
                    arr[i] = '*';
                return number.replace(number.substr(0, number.length - 4), arr.join(''));
            }
        ]);
});